//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Components;
using System;
using UnityEngine;

namespace JEM.Test.Unity
{
    [RequireComponent(typeof(JEMCharacterController2DTest)), DisallowMultipleComponent]
    internal class JEMCharacterController2DTest : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float Speed = 9f;
        public float Acceleration = 75f;
        public float Deceleration = 70f;
        public float VelocityClamp = 20f;
        public float InputSensitivity = 5f;

        public JEMCharacterController2D CharacterController { get; private set; }

        private Vector2 _velocity;

        private void Awake()
        {
            CharacterController = GetComponent<JEMCharacterController2D>();
        }

        private void FixedUpdate()
        {
            // Get input.
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var input = new Vector2(horizontal, vertical);
            input *= InputSensitivity;

            // Calculate velocity
            _velocity.x = Math.Abs(input.x) > 0f
                ? Mathf.MoveTowards(_velocity.x, Speed * input.x, Acceleration * Time.fixedDeltaTime)
                : Mathf.MoveTowards(_velocity.x, 0f, Deceleration * Time.fixedDeltaTime);

            _velocity.y = Math.Abs(input.y) > 0f
                ? Mathf.MoveTowards(_velocity.y, Speed * input.y, Acceleration * Time.fixedDeltaTime)
                : Mathf.MoveTowards(_velocity.y, 0f, Deceleration * Time.fixedDeltaTime);

            // Clamp velocity.
            _velocity = Vector2.ClampMagnitude(_velocity, VelocityClamp);

            // Move the character
            CharacterController.Move(_velocity * Time.fixedDeltaTime);

#if DEBUG
            Debug.DrawLine(transform.position, transform.position + (Vector3) CharacterController.Velocity.normalized, Color.red);
            Debug.DrawLine(transform.position, transform.position + (Vector3) _velocity.normalized, Color.blue);
#endif
        }
    }
}
