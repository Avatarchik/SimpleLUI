//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// #define DEEP_DEBUG_HITBOX

using System;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    /// <inheritdoc cref="IQNetObjectHitbox" />
    /// <summary>
    ///     A simple hitbox component utilized by <see cref="QNetSimpleHitboxBody" />
    ///     for simple hitbox detection in 3D space.
    /// </summary>
    [AddComponentMenu("JEM/QNet/Hitbox/QNet Simple Hitbox")]
    [DisallowMultipleComponent]
    public sealed partial class QNetSimpleHitbox : MonoBehaviour, IQNetObjectHitbox
    {
        [Header("Hitbox Settings")]
        [SerializeField] private string _hitboxName;
        [SerializeField] private bool _isProximityHitbox;

        /// <summary>
        ///     Reference to the active collider used by this <see cref="QNetSimpleHitbox"/>.
        /// </summary>
        public Collider ColliderReference { get; private set; }

        /// <inheritdoc />
        public string HitboxName
        {
            get => _hitboxName;
            set => _hitboxName = value;
        }

        /// <inheritdoc />
        public int HitboxId { get; set; }

        /// <inheritdoc />
        public bool IsProximityHitbox
        {
            get => _isProximityHitbox;
            set => _isProximityHitbox = value;
        }

        /// <inheritdoc />
        public bool IsValid() => true;

        /// <inheritdoc />
        public bool IsActive() => isActiveAndEnabled;

        private void Awake()
        {
            ColliderReference = GetComponent<Collider>();

#if DEBUG
            QNetManager.PrintLogAssert(ColliderReference, $"There is no ColliderReference detected on SimpleHitbox.", this);
#endif
        }

        /// <inheritdoc />
        public IQNetObjectHitboxSample GetSample()
        {
            return new QNetSimpleHitboxSample
            {
                WorldPosition = transform.position,
                WorldOrientation = transform.rotation
            };
        }

        /// <inheritdoc />
        public void ApplyState(IQNetObjectHitboxSample sample)
        {
            if (!(sample is QNetSimpleHitboxSample simpleSample))
                throw new ArgumentException($"Received sample is not a correct type ({sample.GetType().FullName}).");

#if DEBUG && DEEP_DEBUG_HITBOX
            QNetManager.PrintLogMsc("QNetSimpleHitbox.ApplyState()", this);
#endif

            transform.position = simpleSample.WorldPosition;
            transform.rotation = simpleSample.WorldOrientation;
        }

        /// <inheritdoc />
        public void RestoreState()
        {
#if DEBUG && DEEP_DEBUG_HITBOX
            QNetManager.PrintLogMsc("QNetSimpleHitbox.RestoreState()", this);
#endif

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /// <inheritdoc />
        public void SetColliderActiveState(bool activeState) => ColliderReference.enabled = activeState;
    }
}
