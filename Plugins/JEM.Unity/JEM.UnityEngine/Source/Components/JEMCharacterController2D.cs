//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Extension;
using System;
using UnityEngine;

namespace JEM.UnityEngine.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple CharacterController implementation for 2D space.
    /// </summary>
    [AddComponentMenu("JEM/Character/Character Controller 2D")]
    [RequireComponent(typeof(CapsuleCollider2D)), DisallowMultipleComponent]
    public sealed class JEMCharacterController2D : MonoBehaviour
    {
        [Header("Physics Settings")]
        public LayerMask CollisionMask;

        /// <summary>
        ///     Reference to the <see cref="CapsuleCollider2D"/> component.
        /// </summary>
        public CapsuleCollider2D Collider { get; private set; }

        /// <summary>
        ///     True when the character intersects with collider beneath
        ///      them in previous <see cref="Move"/> frame.
        /// </summary>
        public bool IsGrounded { get; private set; }

        /// <summary>
        ///     True when the character intersects with any collider.
        /// </summary>
        public bool HasCollision { get; private set; }

        /// <summary>
        ///     The current relative velocity of the Character.
        /// </summary>
        public Vector2 Velocity { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<CapsuleCollider2D>();
        }

        private void Reset()
        {
            CollisionMask = LayerMask.GetMask("Default");
        }

        /// <summary>
        ///     Moves the character with given <paramref name="velocity"/>.
        /// </summary>
        /// <remarks>
        ///     Gravity is automatically applied.
        /// </remarks>
        public void SimpleMove(Vector2 velocity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     A move function taking absolute movement deltas.
        /// </summary>
        /// <remarks>
        ///     Attempts to move the controller by <paramref name="motion"/>, the motion will only be constrained by collisions.
        ///     This function does not apply any gravity.
        /// </remarks>
        public void Move(Vector2 motion, Space space = Space.Self)
        {
            // Reset the states.
            IsGrounded = false;
            HasCollision = false;

            // Try to fix the motion to match m/s
            motion *= 10f;
            
            // Translate the transform.
            var translateMotion = motion / 10f;
            // Cache position before any translation so it could be restored when wall check fails.
            var prevPosition = transform.position;
            transform.Translate(translateMotion, space);

            // Get all the colliders we have intersected after motion has been applied.
            var hits = Physics2D.OverlapCapsuleAll(transform.position, Collider.size, Collider.direction, transform.GetAngle(), CollisionMask);
            foreach (var hit in hits)
            {
                // Ignore character collider.
                if (hit == Collider)
                    continue;
              
                var colliderDistance = hit.Distance(Collider);
                if (colliderDistance.isOverlapped)
                {
                    var errorMotion = colliderDistance.pointA - colliderDistance.pointB;
                    transform.Translate(errorMotion, Space.World);
                    motion += errorMotion;

                    // If we intersect an object beneath us, set the grounded to true.
                    if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90f && motion.y < 0f)
                    {
                        IsGrounded = true;
                    }
                }

                HasCollision = true;
            }

            if (HasCollision)
            {
                // Character is colliding with something.
                // Run wall test to prevent character being pushed trough the wall.
                var wallTest = Physics2D.LinecastAll(prevPosition, transform.position, CollisionMask);
                // Debug.Log(wallTest.Length);
                foreach (var hit in wallTest)
                {
                    if (hit == Collider)
                        continue;

                    // Wall test failed!
                    // Restore the previous position
                    transform.position = prevPosition;

                    // It is good to try to reset the velocity?
                    motion = Vector2.zero;

#if DEBUG
                    Debug.LogWarning("Wall test for JEMCharacterController2D failed. " +
                                     "Previous position will be restored!", hit.collider);
#endif
                    break;
                }
            }

            // Set the velocity.
            Velocity = motion;
        }
    }
}
