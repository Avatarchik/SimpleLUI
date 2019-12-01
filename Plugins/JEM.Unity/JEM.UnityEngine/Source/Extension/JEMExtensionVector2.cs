//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility methods: Vector2
    /// </summary>
    public static class JEMExtensionVector2
    {
        /// <summary>
        ///     Snap a Vector2.
        /// </summary>
        public static Vector2 Snap(this Vector2 v, float value) => new Vector2(v.x - v.x % value, v.y - v.y % value);

        /// <summary>
        ///     Converts <see cref="Vector2"/> in to <see cref="Vector2Int"/>
        /// </summary>
        public static Vector2Int ToInt(this Vector2 v) => new Vector2Int((int)v.x, (int)v.y);

        /// <summary>
        ///     Converts <see cref="Vector2Int"/> in to <see cref="Vector2"/>
        /// </summary>
        public static Vector2 ToSingle(this Vector2Int v) => new Vector2(v.x, v.y);

        /// <summary>
        ///     Converts <see cref="Vector2Int"/> int to <see cref="Vector3Int"/>.
        /// </summary>
        public static Vector3Int ToVector3(this Vector2Int v) => new Vector3Int(v.x, v.y, 0);
    }
}
