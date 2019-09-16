//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using UnityEngine;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Set of utility methods: Sprite
    /// </summary>
    public static class JEMSprite
    {
        /// <summary>
        ///     Creates sprite from texture2D.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static Sprite FromTexture2D([NotNull] Texture2D texture2D) => FromTexture2D(texture2D, new Vector2(0.5f, 0.5f));
        
        /// <summary>
        ///     Creates sprite from texture2D.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static Sprite FromTexture2D([NotNull] Texture2D texture2D, Vector2 pivot) => FromTexture2D(texture2D, pivot, 100f);
        
        /// <summary>
        ///     Creates sprite from texture2D.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static Sprite FromTexture2D([NotNull] Texture2D texture2D, Vector2 pivot, float pixelsPerUnit)
        {
            if (texture2D == null) throw new ArgumentNullException(nameof(texture2D), "Target texture is null.");
            return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), pivot, pixelsPerUnit);
        }
    }
}