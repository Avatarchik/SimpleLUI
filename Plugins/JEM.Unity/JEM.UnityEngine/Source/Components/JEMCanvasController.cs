//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Canvas Controller component.
    ///     Controls a scale mode, scale factor and ui resolution of target Canvas.
    /// </summary>
    [AddComponentMenu("JEM/Interface/JEM Canvas Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(CanvasScaler))]
    internal class JEMCanvasController : MonoBehaviour
    {
        /// <summary>
        ///     A default canvas resolution.
        /// </summary>
        [Header("Canvas Settings")]
        public Vector2 DefaultResolution = new Vector2(1920, 1080);

        /// <summary>
        ///     Minimal possible resolution.
        /// </summary>
        public Vector2 MinResolution = new Vector2(1280, 720);

        /// <summary>
        ///     Maximal possible resolution.
        /// </summary>
        public Vector2 MaxResolution = new Vector2(3840, 2160);

        /// <summary>
        ///     Reference to the <see cref="global::UnityEngine.Canvas"/> component.
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        ///     Reference to the <see cref="CanvasScaler"/> component.
        /// </summary>
        public CanvasScaler Scaler { get; private set; }

        private Action<object> _cfgUIScaleMode;
        private Action<object> _cfgUIScaleFactor;
        private Action<object> _cfgUIResolution;

        private void Awake() => AllControllers.Add(this);    
        private void Start()
        {
            // Collect the components.
            Canvas = GetComponent<Canvas>();
            Scaler = GetComponent<CanvasScaler>();

            // Check if the database is loaded.
            if (!JEMUnity.HasDatabaseLoaded())
            {
                return;
            }

            // Register the change events.
            _cfgUIScaleMode = JEMUnity.Database.RegisterObjectChange<int>("ui_scale_mode", change =>
            {
                var mode = (CanvasMode)change;
                switch (mode)
                {
                    case CanvasMode.ScaleWithScreenSize:
                        Scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        break;
                    default:
                        Scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                        break;
                }
            });

            _cfgUIScaleFactor = JEMUnity.Database.RegisterObjectChange<float>("ui_scale_factor", change =>
            {
                var factor = Mathf.Clamp(change, 0.8f, 1.5f);
                Scaler.scaleFactor = factor;
            });

            _cfgUIResolution = JEMUnity.Database.RegisterObjectChange<string>("ui_resolution", change =>
            {
                var i = change.Split('x');
                if (i.Length != 2)
                {
                    // apply default
                    Scaler.referenceResolution = DefaultResolution;
                }
                else
                {
                    if (float.TryParse(i[0], out var x))
                    {
                        if (float.TryParse(i[1], out var y))
                        {
                            x = Mathf.Clamp(x, MinResolution.x, MaxResolution.x);
                            y = Mathf.Clamp(y, MinResolution.y, MaxResolution.y);
                            Scaler.referenceResolution = new Vector2(x, y);
                        }
                        else
                        {
                            // Apply default.
                            Scaler.referenceResolution = DefaultResolution;
                        }
                    }
                    else
                    {
                        // Apply default.
                        Scaler.referenceResolution = DefaultResolution;
                    }
                }
            });
        }

        private void OnDestroy()
        {
            // Remove.
            AllControllers.Remove(this);

            // Check if the database is loaded.
            if (!JEMUnity.HasDatabaseLoaded())
            {
                return;
            }

            // Unregister the object change events.
            JEMUnity.Database.UnregisterObjectChange("ui_scale_mode", _cfgUIScaleMode);
            JEMUnity.Database.UnregisterObjectChange("ui_scale_factor", _cfgUIScaleFactor);
            JEMUnity.Database.UnregisterObjectChange("ui_resolution", _cfgUIResolution);
        }

        /// <summary>
        ///     Calls all the <see cref="JEMCanvasController"/> components to update the scale mode.
        /// </summary>
        public static void CallUIScaleMode(CanvasMode canvasMode) => AllControllers.ForEach(c => { c._cfgUIScaleMode.Invoke(canvasMode); });

        /// <summary>
        ///     Calls all the <see cref="JEMCanvasController"/> components to update the scale factor.
        /// </summary>
        /// <param name="value"></param>
        public static void CallUIScaleFactor(float value) => AllControllers.ForEach(c => { c._cfgUIScaleFactor.Invoke(value); });

        /// <summary>
        ///     Calls all the <see cref="JEMCanvasController"/> components to update the resolution.
        /// </summary>
        /// <param name="resolution"></param>
        public static void CallUIResolution(string resolution) => AllControllers.ForEach(c => { c._cfgUIResolution.Invoke(resolution); });

        /// <summary>
        ///     List of all <see cref="JEMCanvasController"/> components in current scene.
        /// </summary>
        public static List<JEMCanvasController> AllControllers { get; } = new List<JEMCanvasController>();
    }
}
