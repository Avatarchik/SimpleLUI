//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.UnityEngine.Components;
using JEM.UnityEngine.Extension;
using System;
using UnityEngine;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Defines a log forwarding method.
    /// </summary>
    public enum JEMLogForward
    {
        /// <summary>
        ///     Don't forward logs.
        /// </summary>
        None,

        /// <summary>
        ///     Forward unity logs to JEM.
        /// </summary>
        UnityToJEM,

        /// <summary>
        ///     Forward JEM logs to unity.
        /// </summary>
        JEMToUnity
    }

    /// <inheritdoc />
    /// <summary>
    ///     JEM Hub implements few utility systems.
    /// </summary>
    [AddComponentMenu("JEM/Systems/JEM Hub")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-10)]
    public class JEMHub : JEMSingletonBehaviour<JEMHub>
    {
        /// <summary>
        ///     If true, all JEM scripts will be spawned at start.
        /// </summary>
        [Header("Performance Settings")]
        public bool PrepareScripts = true;

        /// <summary>
        ///     Defines current method of log forwarding.
        /// </summary>
        [Header("Debugging Settings")]
        public JEMLogForward LogForwarding = JEMLogForward.JEMToUnity;

        /// <inheritdoc />
        protected override void OnAwake()
        {
            // Dont destroy this object.
            gameObject.CollectComponent<JEMObjectKeepOnScene>();

            // Register logs events.
            JEMLogger.OnLogAppended += OnJEMLog;
            Application.logMessageReceived += OnUnityLog;

            // Clear and init JEM Logger.
            JEMLogger.ClearLoggerDirectory();
            JEMLogger.Start();

            // Prepare scripts.
            if (PrepareScripts)
            {
                JEMUnity.PrepareJEMScripts();
            }
        }

        private void OnDestroy() => JEMLogger.Stop();
        private void OnApplicationQuit() => JEMLogger.Stop();

        private void OnJEMLog(string source, JEMLogType type, string value, string stacktrace)
        {
            switch (LogForwarding)
            {
                case JEMLogForward.None:
                    // ignore.
                    break;
                case JEMLogForward.UnityToJEM:
                    // ignore.
                    break;
                case JEMLogForward.JEMToUnity:
                    switch (type)
                    {
                        case JEMLogType.Log:
                            Debug.Log($"{source} :: {value}", this);
                            break;
                        case JEMLogType.Warning:
                            Debug.LogWarning($"{source} :: {value}", this);
                            break;
                        case JEMLogType.Error:
                            Debug.LogError($"{source} :: {value}", this);
                            break;
                        case JEMLogType.Exception:
                            Debug.LogError($"{source} :: {value}\n{stacktrace}", this);
                            break;
                        case JEMLogType.Assert:
                            Debug.LogError($"{source} :: {value}", this);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnUnityLog(string condition, string stacktrace, LogType type)
        {
            switch (LogForwarding)
            {
                case JEMLogForward.None:
                    // ignore
                    break;
                case JEMLogForward.UnityToJEM:
                    switch (type)
                    {
                        case LogType.Error:
                            JEMLogger.LogError(condition, "UNITY");
                            break;
                        case LogType.Assert:
                            JEMLogger.LogAssert(condition, "UNITY");
                            break;
                        case LogType.Warning:
                            JEMLogger.LogWarning(condition, "UNITY");
                            break;
                        case LogType.Log:
                            JEMLogger.Log(condition, "UNITY");
                            break;
                        case LogType.Exception:
                            JEMLogger.LogException(condition, stacktrace, "UNITY");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    break;
                case JEMLogForward.JEMToUnity:
                    // ignore
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
