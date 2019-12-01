//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility methods: Vector3
    /// </summary>
    public static class JEMExtensionVector3
    {
        /// <summary>
        ///     Compare distance between point A + B and A + C.
        /// </summary>
        public static float CompareDistance(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            var s1 = Vector3.Distance(pointA, pointB);
            var s2 = Vector3.Distance(pointA, pointC);
            return Mathf.Abs(s1 - s2);
        }

        /// <summary>
        ///     Multiply two vectors axis by axis.
        /// </summary>
        public static Vector3 Multiply(Vector3 vectorA, Vector3 vectorB) =>
            new Vector3(vectorA.x * vectorB.x, vectorA.y * vectorB.y, vectorA.z * vectorB.z);

        /// <summary>
        ///     Snap a Vector3.
        /// </summary>
        public static Vector3 Snap(this Vector3 v, float value) =>
            new Vector3(v.x - v.x % value, v.y - v.y % value, v.z - v.z % value);

        /// <summary/>
        public static Vector3 Horizontal(this Vector3 vector) => new Vector3(vector.x, 0.0f, vector.z);

        /// <summary/>
        public static Vector3 Vertical(this Vector3 vector) => new Vector3(0.0f, vector.y, 0.0f);

        /// <summary>
        ///     Converts <see cref="Vector3"/> in to <see cref="Vector3Int"/>
        /// </summary>
        public static Vector3Int ToInt(this Vector3 v) => new Vector3Int((int) v.x, (int) v.y, (int) v.z);

        /// <summary>
        ///     Converts <see cref="Vector3Int"/> in to <see cref="Vector3"/>
        /// </summary>
        public static Vector3 ToSingle(this Vector3Int v) => new Vector3(v.x, v.y, v.z);

        /// <summary>
        ///     Converts <see cref="Vector3Int"/> int to <see cref="Vector2Int"/>.
        /// </summary>
        public static Vector2Int ToVector2(this Vector3Int v) => new Vector2Int(v.x, v.y);
    }
}
