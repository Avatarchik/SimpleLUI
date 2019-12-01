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
    ///     A simple hitbox component utilized by <see cref="QNetSimpleHitbox2DBody" />
    ///     for simple hitbox detection in 2D space.
    /// </summary>
    [AddComponentMenu("JEM/QNet/Hitbox/QNet Simple Hitbox 2D")]
    [DisallowMultipleComponent]
    public sealed partial class QNetSimpleHitbox2D : MonoBehaviour, IQNetObjectHitbox
    {
        [Header("Hitbox Settings")]
        [SerializeField] private string _hitboxName;
        [SerializeField] private bool _isProximityHitbox;

        /// <summary>
        ///     Reference to the active collider2D used by this <see cref="QNetSimpleHitbox2D"/>.
        /// </summary>
        public Collider2D ColliderReference { get; private set; }

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
            ColliderReference = GetComponent<Collider2D>();

#if DEBUG
            QNetManager.PrintLogAssert(ColliderReference, "There is no ColliderReference detected on SimpleHitbox2D.", this);
#endif
        }

        /// <inheritdoc />
        public IQNetObjectHitboxSample GetSample()
        {
            return new QNetSimpleHitbox2DSample
            {
                WorldPosition = transform.position,
                WorldAngle = transform.eulerAngles.z
            };
        }

        /// <inheritdoc />
        public void ApplyState(IQNetObjectHitboxSample sample)
        {
            if (!(sample is QNetSimpleHitbox2DSample simpleSample))
                throw new ArgumentException($"Received sample is not a correct type ({sample.GetType().FullName}).");

#if DEBUG && DEEP_DEBUG_HITBOX
            QNetManager.PrintLogMsc("QNetSimpleHitbox2D.ApplyState()", this);
#endif

            transform.position = simpleSample.WorldPosition;
            transform.eulerAngles = new Vector3(0f, 0f, simpleSample.WorldAngle);
        }

        /// <inheritdoc />
        public void RestoreState()
        {
#if DEBUG && DEEP_DEBUG_HITBOX
            QNetManager.PrintLogMsc("QNetSimpleHitbox2D.RestoreState()", this);
#endif

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /// <inheritdoc />
        public void SetColliderActiveState(bool activeState) => ColliderReference.enabled = activeState;
    }
}
