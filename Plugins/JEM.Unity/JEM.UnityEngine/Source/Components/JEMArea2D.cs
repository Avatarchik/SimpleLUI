//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JEM.UnityEngine.Components
{
    /// <summary>
    ///     JEM Area2D Mode.
    ///     Defines how JEMArea2D should generate a point.
    /// </summary>
    public enum JEMArea2DMode
    {
        /// <summary>
        ///     Unknown area mode. Should never be used!
        /// </summary>
        Unknown,

        /// <summary>
        ///     The point will be generated in local space (random point inside BoxCollider2D bounds).
        /// </summary>
        Space,

        /// <summary>
        ///     The point will be generated using hit mask.
        /// </summary>
        /// <remarks>
        ///     Point will only be generated if JEMArea2D hit a collider with target layer mask.
        /// </remarks>
        HitMask
    }

    /// <inheritdoc />
    /// <summary>
    ///     JEM Area2D.
    ///     A 2D area component for random point generation using BoxCollider2D component.
    /// </summary>
    [AddComponentMenu("JEM/Level/JEM Area 2D")]
    [RequireComponent(typeof(BoxCollider2D))]
    [DisallowMultipleComponent]
    public class JEMArea2D : MonoBehaviour
    {
        /// <summary>
        ///     Target JEM Area2D Mode.
        /// </summary>
        [Header("Area Settings")]
        public JEMArea2DMode Mode = JEMArea2DMode.HitMask;

        /// <summary>
        ///     Size of target Agent.
        /// </summary>
        public float AgentSize = 0.4f;

        /// <summary>
        ///     Target JEM Area Forward Mode.
        /// </summary>
        public JEMAreaForwardMode ForwardMode = JEMAreaForwardMode.Forward;

        /// <summary>
        ///     Amount of reliable point generation attempts.
        /// </summary>
        public int ReliablePointAttempts = 10;

        /// <summary>
        ///     If true, when reliable point generation fails, the backup point will be always the center of JEMArea2D object.
        /// </summary>
        public bool ReliableFailAlwaysCenter;

        /// <summary>
        ///     Hit layer mask.
        /// </summary>
        [Header("Hit Mask Settings")]
        public LayerMask HitMask;

        private Vector2 BoxSize => new Vector3(AgentSize, AgentSize);
        private BoxCollider2D _collider;

        private void Awake() => _collider = GetComponent<BoxCollider2D>();
        private void Reset()
        {
            Mode = JEMArea2DMode.HitMask;
            AgentSize = 0.4f;

            HitMask = LayerMask.GetMask("Default");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Debug.Assert(gameObject.isStatic, "JEMArea2D object is not static!", this);
            if (_collider) Debug.Assert(_collider.isTrigger, "BoxCollider2D of JEMArea2D isTrigger is not set to true!", this);
        }

        private void OnDrawGizmos()
        {
            if (!_collider) _collider = GetComponent<BoxCollider2D>();

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            switch (Mode)
            {
                case JEMArea2DMode.Space:
                    Gizmos.color = new Color(0.6f, 0.0f, 0.3f, 0.3f);
                    Gizmos.DrawCube(_collider.offset, _collider.size);
                    Gizmos.color = new Color(0.7f, 0.0f, 0.3f, 0.7f);
                    Gizmos.DrawWireCube(_collider.offset, _collider.size);

                    Gizmos.color = new Color(0.1f, 1.0f, 0.1f, 0.3f);
                    Gizmos.DrawCube(_collider.offset, BoxSize);
                    break;
                case JEMArea2DMode.HitMask:
                    Gizmos.color = new Color(0.1f, 0.7f, 0.9f, 0.3f);
                    Gizmos.DrawCube(_collider.offset, _collider.size);
                    Gizmos.color = new Color(0.1f, 0.7f, 0.9f, 0.7f);
                    Gizmos.DrawWireCube(_collider.offset, _collider.size);

                    Gizmos.color = new Color(0.1f, 1.0f, 0.1f, 0.3f);
                    Gizmos.DrawCube(_collider.offset, BoxSize);
                break;
                case JEMArea2DMode.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!_collider) _collider = GetComponent<BoxCollider2D>();
            // TODO: Safe check if collider has any hit.
        }
#endif

        /// <summary>
        ///     Try to random point from this JEM Area2D component.
        ///     Point generation is always reliable and will always end with successful effect. In case if HitMask mode fail to get point, new point will be generated using Space mode instead.
        /// </summary>
        /// <param name="point"/>
        /// <remarks>Returns false, if reliable point generation was forced to return center or point using Space mode.</remarks>
        public bool GenerateReliablePoint(out Vector2 point) => GenerateReliablePoint(out point, out _);
       
        /// <summary>
        ///     Try to random point from this JEM Area2D component.
        ///     Point generation is always reliable and will always end with successful effect. In case if HitMask mode fail to get point, new point will be generated using Space mode instead.
        /// </summary>
        /// <param name="point"/>
        /// <param name="angle"/>
        /// <remarks>Returns false, if reliable point generation was forced to return center or point using Space mode.</remarks>
        public bool GenerateReliablePoint(out Vector2 point, out float angle)
        {
            if (GenerateUnreliablePoint(out point, out angle))
                return true;

            if (Mode == JEMArea2DMode.Space)
            {
                // hey! we somehow failed to generate point using Space mode. Wut?
                throw new NotSupportedException("Fatal. JEMArea2D failed to generate " +
                                                "reliable point using (asDefault)Space mode.");
            }

            int c = 0;
            while (c < ReliablePointAttempts)
            {
                c++;
                if (GenerateUnreliablePoint(out point, out angle))
                    return true;
            }

            if (ReliableFailAlwaysCenter)
            {
#if DEBUG
                Debug.LogWarning("JEMArea2D had problem to generate reliable point (HitMask) " +
                                   "so the center of JEMArea2D will be returned instead.", this);
#endif
                point = (Vector2) transform.position + _collider.offset;
                angle = 0f;
            }
            else
            {
#if DEBUG
                Debug.LogWarning("JEMArea2D had problem to generate reliable point (HitMask) " +
                                 "so we will try to generate using Space mode instead.", this);
#endif
                GenerateUnreliablePoint(out point, out angle, JEMArea2DMode.Space);
            }

            return false;
        }

        /// <summary>
        ///     Try to random point from this JEM Area2D component.
        ///     Point generation is unreliable and can end with unsuccessful effect.
        /// </summary>
        /// <param name="point"/>
        /// <param name="customMode"/>
        public bool GenerateUnreliablePoint(out Vector2 point, JEMArea2DMode customMode = JEMArea2DMode.Unknown) =>
            GenerateUnreliablePoint(out point, out _, customMode);
        
        /// <summary>
        ///     Try to random point from this JEM Area2D component.
        ///     Point generation is unreliable and can end with unsuccessful effect.
        /// </summary>
        /// <param name="point"/>
        /// <param name="angle"/>
        /// <param name="customMode"/>
        public bool GenerateUnreliablePoint(out Vector2 point, out float angle, JEMArea2DMode customMode = JEMArea2DMode.Unknown)
        {
            var prevMode = Mode;
            if (customMode != JEMArea2DMode.Unknown) Mode = customMode;

            point = Vector3.zero;
            angle = 0f;

            var maxX = _collider.size.x - AgentSize / 2f;
            var maxY = _collider.size.y - AgentSize / 2f;

            switch (Mode)
            {
                case JEMArea2DMode.Space:
                    var randomPointInSpace = _collider.offset + new Vector2
                    {
                        x = Random.Range(-maxX / 2f, maxX / 2f),
                        y = Random.Range(-maxY / 2f, maxY / 2f)
                    };
                    randomPointInSpace = transform.TransformPoint(randomPointInSpace);

                    point = randomPointInSpace;

                    switch (ForwardMode)
                    {
                        case JEMAreaForwardMode.Identity:
                            angle = transform.eulerAngles.z;
                            break;
                        case JEMAreaForwardMode.Forward:
                            var diff1 = (Vector3) point - transform.position + (Vector3) _collider.offset;
                            diff1.Normalize();
                            angle = Mathf.Atan2(diff1.y, diff1.x) * Mathf.Rad2Deg - 90f;
                            break;
                        case JEMAreaForwardMode.ForwardCenter:
                            var diff2 = transform.position + (Vector3) _collider.offset - (Vector3) point;
                            diff2.Normalize();
                            angle = Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg - 90f;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    Mode = prevMode;
                    return true;
                case JEMArea2DMode.HitMask:
                    var randomPointInTop = _collider.offset + new Vector2
                    {
                        x = Random.Range(-maxX / 2f, maxX / 2f),
                        y = Random.Range(-maxY / 2f, maxY / 2f)
                    };
                    randomPointInTop = transform.TransformPoint(randomPointInTop);

                    bool hasHit = false;
                    if (RunHitTest(randomPointInTop, out var hitPoint1))
                    {
                        point = hitPoint1;
                        hasHit = true;
                    }
                    else
                    {
                        var centerPoint = new Vector2(_collider.offset.x, _collider.offset.y);
                        if (RunHitTest(centerPoint, out var hitPoint2))
                        {
                            point = hitPoint2;
                            hasHit = true;
                        }
                    }

                    if (hasHit)
                    {
                        switch (ForwardMode)
                        {
                            case JEMAreaForwardMode.Identity:
                                angle = transform.eulerAngles.z;
                                break;
                            case JEMAreaForwardMode.Forward:
                                var diff1 = (Vector3) point - transform.position + (Vector3) _collider.offset;
                                diff1.Normalize();
                                angle = Mathf.Atan2(diff1.y, diff1.x) * Mathf.Rad2Deg - 90f;
                                break;
                            case JEMAreaForwardMode.ForwardCenter:
                                var diff2 = transform.position + (Vector3) _collider.offset - (Vector3) point;
                                diff2.Normalize();
                                angle = Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg - 90f;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        Mode = prevMode;
                        return true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Mode = prevMode;
            return false;
        }

        private bool RunHitTest(Vector2 origin, out Vector2 point)
        {         
            point = Vector2.zero;
            var hasHit = Physics2D.OverlapBox(origin, BoxSize / 2f, transform.eulerAngles.z, HitMask);
            if (hasHit)
            {
                point = hasHit.ClosestPoint(origin);
            }

            return hasHit;
        }

        /// <summary>
        ///     Gets random Area2D from active scene.
        /// </summary>
        public static JEMArea2D GetRandomArea()
        {
            var spawnAreas = FindObjectsOfType<JEMArea2D>();
            if (spawnAreas.Length == 0)
                throw new InvalidOperationException("System was unable to get JEMArea2D. " +
                                                    "There is no JEMAreas2D defined in current world.");

            if (spawnAreas.Length == 1)
                return spawnAreas[0];

            return spawnAreas[Random.Range(0, spawnAreas.Length)];
        }
    }
}
