//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Attribute;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JEM.UnityEngine.Effects
{
    /// <summary>
    ///     Settings that defines what effect and how <see cref="JEMEffects2D"/> should spawn.
    /// </summary>
    public struct JEMEffect2DSettings
    {
        /// <summary>
        ///     Reference to the prefab of this effect.
        /// </summary>
        public GameObject Prefab { get; }

        /// <summary>
        ///     Point that defines where to spawn this effect.
        /// </summary>
        public Vector2 Point { get; set; }

        /// <summary>
        ///     Angle of effect to spawn at.
        /// </summary>
        public float Angle { get; set; }

        public JEMEffect2DSettings(GameObject prefab)
        {
            Prefab = prefab;
            Point = Vector2.zero;
            Angle = 0f;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     A effects manager for 2D space.
    ///     Optimizes in game particle/sound effects by reducing/adjusting the same effects spawned at the same point.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public abstract class JEMEffects2D<TManager> : JEMSingletonBehaviour<TManager> where TManager : JEMEffects2D<TManager>
    {
        /// <summary>
        ///     Defines if <see cref="JEMEffects2D{T}"/> should try to optimize spawned effects.
        /// </summary>
        [Header("Settings")]
        public bool UseEffectsOptimization = true;

        /// <summary>
        ///     Amount of effect spawn at the same frame that will enable optimization.
        /// </summary>
        [JEMIndentLevel, JEMPropertyBased(nameof(UseEffectsOptimization))]
        public int OverheatCap = 5;

        /// <summary>
        ///     Defines minimal distance of spawned effects.
        ///     When effects on the same type has been spawned with distance less than <see cref="MinimalEffectsDistance"/>, they will be combined.
        /// </summary>
        [JEMIndentLevel, JEMPropertyBased(nameof(UseEffectsOptimization))]
        public float MinimalEffectsDistance = 0.3f;

        /// <summary>
        ///     Amount of ticks that should be skipped before processing next effects.
        /// </summary>
        [JEMIndentLevel, JEMPropertyBased(nameof(UseEffectsOptimization))]
        public int SkipTicks = 10;

        /// <summary>
        ///     Effects spawn queue.
        /// </summary>
        private List<JEMEffect2DSettings> Queue { get; } = new List<JEMEffect2DSettings>();

        private int _tick;
        private bool _hasItems;

        private void LateUpdate()
        {
            if (!UseEffectsOptimization)
                return;

            if (!_hasItems)
            {
                return;
            }

            if (_tick < SkipTicks)
            {
                _tick++;
                return;
            }

            _tick = 0;
            while (Queue.Count != 0)
            {
                var rootEffect = Queue[0];
                var removedEffects = 1;
                for (var index = 1; index < Queue.Count; index++)
                {
                    var q = Queue[index];
                    var distance = Vector2.Distance(rootEffect.Point, q.Point);
                    if (distance < MinimalEffectsDistance &&
                        q.Prefab.GetInstanceID() == rootEffect.Prefab.GetInstanceID())
                    {
                        Queue.RemoveAt(index);
                        removedEffects++;
                    }
                }

                Queue.RemoveAt(0);
                OnItemSpawn(rootEffect, removedEffects);
            }

            Queue.Clear();
            _hasItems = false;
        }

        /// <summary>
        ///     Adds given effect to the queue.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void QueueItem(GameObject effectPrefab, Vector2 point) => QueueItem(effectPrefab, point, 0);

        /// <summary>
        ///     Adds given effect to the queue.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void QueueItem([NotNull] GameObject effectPrefab, Vector2 point, float angle)
        {
            if (effectPrefab == null) throw new ArgumentNullException(nameof(effectPrefab));
            QueueItem(new JEMEffect2DSettings(effectPrefab) { Point = point, Angle = angle });
        }

        /// <summary>
        ///     Adds given <see cref="JEMEffect2DSettings"/> to the queue.
        /// </summary>
        public void QueueItem(JEMEffect2DSettings settings)
        {
            if (settings.Prefab == null) throw new ArgumentNullException(nameof(settings.Prefab));

            var shouldQueue = UseEffectsOptimization && OverheatCap >= Queue.Count;
            if (shouldQueue)
            {
                Queue.Add(settings);
                _hasItems = true;
            }
            else
            {
                // Optimization disabled, spawn instantly.
                OnItemSpawn(settings, 0);
            }
        }

        /// <summary>
        ///     Called when item need to be spawned.
        /// </summary>
        /// <param name="settings"/>
        /// <param name="quantityRemoved">Amount of items removed to optimize spawned effects.</param>
        protected abstract void OnItemSpawn(JEMEffect2DSettings settings, int quantityRemoved);
    }
}
