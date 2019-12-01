//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// Original Network simulation design and implementation
//  by Damian 'Erdroy' Korczowski (https://github.com/Erdroy)
//

// #define DEEP_DEBUG_HITBOX

using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox
{
    /// <inheritdoc />
    /// <summary>
    ///     Base of QNetObject hitbox body.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class QNetObjectHitboxBody : QNetBehaviour
    {
        /// <summary>
        ///     Amount of maximal hitbox snapshots.
        /// </summary>
        [Header("Hitbox Settings")]
        public int MaxHitboxAmount = 32;

        /// <summary>
        ///     The time error threshold that decides whatever to use interpolated snapshots
        ///      or just the closest one.
        /// </summary>
        public float TimeErrorThreshold = 0.001f;

        /// <summary>
        ///     Contains all hitboxes that this QNetObject has.
        /// </summary>
        public IQNetObjectHitbox[] Hitboxes { get; private set; }

        /// <summary>
        ///     Hitbox used to check approximate hits, and thus,
        ///     to optimize amount of hitboxes being checked for hits.
        /// </summary>
        public IQNetObjectHitbox ProximityHitbox { get; private set; }

        /// <summary>
        ///     Returns true when object is active, spawned and can be hit.
        /// </summary>
        public bool IsActive => isActiveAndEnabled && HitboxesRegistered && Identity.IsSpawned;

        /// <summary>
        ///     True if hitboxes of this object has been registered.
        /// </summary>
        public bool HitboxesRegistered { get; private set; }

        /// <summary>
        ///     List of hitbox body snapshots.
        /// </summary>
        protected LinkedList<QNetObjectHitboxBodySnapshot> Snapshots { get; private set; }

        /// <summary>
        ///     Register object hitboxes.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public void RegisterHitboxes([NotNull] IQNetObjectHitbox proximityHitbox, [NotNull] IQNetObjectHitbox[] hitboxes)
        {
            if (proximityHitbox == null) throw new ArgumentNullException(nameof(proximityHitbox));
            if (hitboxes == null) throw new ArgumentNullException(nameof(hitboxes));
            if (hitboxes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(hitboxes));

#if DEBUG
            QNetManager.PrintLogMsc($"QNetObjectHitboxBody.RegisterHitboxes({proximityHitbox.HitboxName}, {hitboxes.Length})", this);
#endif

            // Set the hitboxes.
            Hitboxes = hitboxes;

            // Set the proximity hitbox and make sure isProximityHitbox is true.
            ProximityHitbox = proximityHitbox;
            ProximityHitbox.IsProximityHitbox = true;

            // Index all hitboxes and disable them.
            for (var index = 0; index < Hitboxes.Length; index++)
            {
                var hitbox = Hitboxes[index];
                if (hitbox.IsValid())
                {
                    hitbox.HitboxId = index;
                    hitbox.SetColliderActiveState(false);
                }
            }

            // Update hitboxesRegistered state.
            HitboxesRegistered = true;

            if (!IsServer)
                return;

            // Preallocate snapshot list for at max one second.
            Snapshots = new LinkedList<QNetObjectHitboxBodySnapshot>();
            for (int i = 0; i < QNetTime.TickRate; i++)
            {
                // Initialize snapshot.
                var snapshot = GetNewSnapshot(MaxHitboxAmount);

                // Add initialized snapshot to last.
                Snapshots.AddLast(snapshot);
            }
        }

        private void OnBeginSimulate()
        {
            if (!IsServer) return;
            // Perform snapshot.
            PerformSnapshot();
        }

        private void PerformSnapshot()
        {
            if (Snapshots == null)
                throw new NullReferenceException("Snapshots list is null. Is HitboxBody initialized?");

            var snapshot = Snapshots.First.Value;
            Snapshots.RemoveFirst();

            // Set current frame and active state.
            snapshot.Frame = QNetTime.ServerFrame;
            snapshot.IsActive = IsActive;

            if (IsActive)
            {
                // Snapshot proximity hitbox.
                if (ProximityHitbox.IsValid())
                {
                    snapshot.ProximitySample = ProximityHitbox.GetSample();
                }

                // Snapshot all hitboxes.
                for (var index = 0; index < Hitboxes.Length; index++)
                {
                    var hitbox = Hitboxes[index];             
                    QNetManager.PrintLogAssert(hitbox.IsValid(), $"Hitbox with index {index} is missing!", this);
                    snapshot.HitboxesSamples[index] = hitbox.GetSample();
                }
            }

            Snapshots.AddLast(snapshot);
        }

        /// <summary>
        ///     Tries to get snapshot based on given frame.
        /// </summary>
        /// <param name="frame">The frame at which the desired snapshot was performed.</param>
        /// <param name="bodySnapshot">The snapshot.</param>
        /// <returns>True when snapshot has been found.</returns>
        public bool TryGetSnapshot(uint frame, out QNetObjectHitboxBodySnapshot bodySnapshot)
        {
            bodySnapshot = Snapshots.FirstOrDefault(x => x.Frame == frame);
            return bodySnapshot != null;
        }

        /// <summary>
        ///     Tries to get snapshot based on given time.
        /// </summary>
        /// <param name="frame">The frame at which the desired snapshot was performed
        /// (and if error is too big two snapshots will be interpolated together).</param>
        /// <param name="bodySnapshot">The snapshot.</param>
        /// <param name="proximityOnly"/>
        /// <returns>True when snapshot has been found.</returns>
        public bool TryGetSnapshot(uint frame, out QNetObjectHitboxBodySnapshot bodySnapshot, bool proximityOnly)
        {
            bodySnapshot = null;

            // When frame is too old or from the future, discard.
            if (Snapshots.First.Value.Frame > frame || frame >= Snapshots.Last.Value.Frame)
            {
                // Debug.LogWarning($"{_snapshots.First.Value.Frame} > {frame} || {frame} >= {_snapshots.Last.Value.Frame}");
                return false;
            }

            // Calculate snapshot error
            var error = Mathf.Abs(Mathf.Round(frame) - frame);

            // Check error, proceed if small enough or there is no
            // enough snapshots, yet.
            if (error <= TimeErrorThreshold || Snapshots.Count <= 1)
            {
                // Just take the closest snapshot
                return TryGetSnapshot(frame, out bodySnapshot);
            }

            // Error is too high, we are going to interpolate.

            // Calculate first and second frame
            var firstFrame = (uint)Mathf.FloorToInt(frame);
            var secondFrame = firstFrame + 1u;

            var hasFirstSnapshot = TryGetSnapshot(firstFrame, out var firstSnapshot);
            var hasSecondSnapshot = TryGetSnapshot(secondFrame, out var secondSnapshot);

            if (!hasFirstSnapshot || !hasSecondSnapshot)
            {
                // Note: We should interpolate these snapshots, but there is missing one,
                // so, let the server do the checks and thus, give the client a chance 
                // to get it's hit anyways.

                if (hasFirstSnapshot)
                {
                    bodySnapshot = firstSnapshot;
                    return true;
                }

                if (hasSecondSnapshot)
                {
                    bodySnapshot = secondSnapshot;
                    return true;
                }

                // Uh, no snapshots, client will not hit anything!
                return false;
            }

            // Calculate interpolation time
            var t = frame - Mathf.Floor(frame); // Does it need some value validation? Not sure.

            // BUG: There is potential bug, when one of the snapshot is not active and it is being interpolated
            // Note: This would be likely to happen when player spawned and at the same frame he gets hit.
            // This is totally minor issue, and maybe should be fixed in the future.

            // Get new snapshot.
            bodySnapshot = GetNewSnapshot(MaxHitboxAmount);

            // Setup snapshot
            bodySnapshot.IsActive = true;
            bodySnapshot.Frame = secondFrame;

            // Interpolate proximity hitbox
            bodySnapshot.ProximitySample = firstSnapshot.ProximitySample.Lerp(secondSnapshot.ProximitySample, t);

            if (!proximityOnly)
            {
                // Interpolate every other hitbox
                for (var i = 0; i < Hitboxes.Length; i++)
                {
                    bodySnapshot.HitboxesSamples[i] = firstSnapshot.HitboxesSamples[i].Lerp(secondSnapshot.HitboxesSamples[i], t);
                }
            }

            return true;
        }

        /// <summary>
        ///     Called then <see cref="QNetObjectHitboxBody"/> need to resolve new snapshot class.
        /// </summary>
        protected abstract QNetObjectHitboxBodySnapshot GetNewSnapshot(int maxHitboxAmount);

        /// <summary>
        ///     Runs a operation on the client side where you can test if your physics hits any of this object's hitboxes.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void ClientSideTest<THitbox, THitboxHit>([NotNull] IList<THitboxHit> hits, [NotNull] Func<THitbox, THitboxHit> testHitbox) 
            where THitbox : IQNetObjectHitbox where THitboxHit : IQNetObjectHitboxHit
        {
            if (hits == null) throw new ArgumentNullException(nameof(hits));
            if (testHitbox == null) throw new ArgumentNullException(nameof(testHitbox));
            if (!IsActive) return;
            // if (!IsActive || IsOwner) return;
            // UPDATE: Local peer should always include it's own body in tests.
            //         He may just remove his object from the list later anyway.

#if DEBUG && DEEP_DEBUG_HITBOX
            QNetManager.PrintLogMsc($"QNetObjectHitboxBody.ClientSideTest({hits.Count})", this);
#endif

            // Test proximity first.
            if (ProximityHitbox.IsValid())
            {
                if (!ProximityHitbox.IsActive()) return;
                ProximityHitbox.SetColliderActiveState(true);
                var hit = testHitbox.Invoke((THitbox) ProximityHitbox);
                ProximityHitbox.SetColliderActiveState(false);
                if (!hit.IsValid)
                {
                    // Proximity hit is not valid, stop right there
                    //  criminal scum!
                    return;
                }

                hits.Add(hit);
            }

            // Test hitboxes.
            for (var index = 0; index < Hitboxes.Length; index++)
            {
                var hitbox = Hitboxes[index];
                if (!hitbox.IsActive()) continue;
                hitbox.SetColliderActiveState(true);
                var hit = testHitbox.Invoke((THitbox) hitbox);
                hitbox.SetColliderActiveState(false);
                if (!hit.IsValid)
                {
                    continue;
                }

                if (hits.Any(h => h.IsValid && h.Hitbox.HitboxId == hit.Hitbox.HitboxId))
                {
                    continue;
                }

                hits.Add(hit);
            }
        }

        /// <summary>
        ///     Runs a operation on the server side where you can test if your physics hits any of this object's hitboxes at the given frame.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void ServerSideTest<THitbox, THitboxHit>(uint frame, [NotNull] IList<THitboxHit> hits, [NotNull] Func<THitbox, THitboxHit> testHitbox)
            where THitbox : IQNetObjectHitbox where THitboxHit : IQNetObjectHitboxHit
        {
            if (hits == null) throw new ArgumentNullException(nameof(hits));
            if (testHitbox == null) throw new ArgumentNullException(nameof(testHitbox));
            if (!IsActive) return;

            QNetManager.PrintLogAssert(IsServer, "Only server can perform lag compensated tests!", this);

#if DEBUG && DEEP_DEBUG_HITBOX
            QNetManager.PrintLogMsc($"QNetObjectHitboxBody.ServerSideTest({frame}, {hits.Count})", this);
#endif

            // Test proximity hitbox first.
            if (TryGetSnapshot(frame, out var proximitySnapshot, true))
            {
                if (!proximitySnapshot.IsActive)
                    return;

                if (ProximityHitbox.IsValid())
                {
                    if (!ProximityHitbox.IsActive()) return;
                    ProximityHitbox.ApplyState(proximitySnapshot.ProximitySample);
                    ProximityHitbox.SetColliderActiveState(true);
                    var hit = testHitbox.Invoke((THitbox) ProximityHitbox);
                    ProximityHitbox.SetColliderActiveState(false);
                    ProximityHitbox.RestoreState();

                    if (!hit.IsValid)
                    {
                        // Proximity hit is not valid, stop right there
                        //  criminal scum!
                        return;
                    }

                    hits.Add(hit);
                }
            }
#if DEBUG
            else
            {
                QNetManager.PrintLogMsc($"Could not find proximity hitbox body snapshot at frame {frame} on {gameObject.name}", this);
            }
#endif

            // Test hitboxes.
            if (TryGetSnapshot(frame, out var hitboxBodySnapshot))
            {
                for (var index = 0; index < Hitboxes.Length; index++)
                {
                    var hitbox = Hitboxes[index];
                    if (!hitbox.IsActive()) continue;
                    hitbox.ApplyState(hitboxBodySnapshot.HitboxesSamples[index]);
                    hitbox.SetColliderActiveState(true);
                    var hit = testHitbox.Invoke((THitbox) hitbox);
                    hitbox.SetColliderActiveState(false);
                    hitbox.RestoreState();

                    if (!hit.IsValid)
                    {
                        continue;
                    }

                    if (hits.Any(h => h.IsValid && h.Hitbox.HitboxId == hit.Hitbox.HitboxId))
                    {
                        continue;
                    }

                    hits.Add(hit);
                }
            }
#if DEBUG
            else
            {
                QNetManager.PrintLogMsc($"Could not find hitbox body snapshot at frame {frame} on {gameObject.name}", this);
            }
#endif
        }
    }
}
