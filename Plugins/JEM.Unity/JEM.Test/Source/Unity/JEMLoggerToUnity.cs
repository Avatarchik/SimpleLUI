//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using System;
using UnityEngine;

namespace JEM.Test.Unity
{
    [DefaultExecutionOrder(-1)]
    internal class JEMLoggerToUnity : MonoBehaviour
    {
        private void Awake()
        {
            JEMLogger.Init();
            JEMLogger.ClearLoggerDirectory();
            JEMLogger.OnLogAppended += OnJEMLogAppended;
        }

        private void OnJEMLogAppended(string source, JEMLogType type, string value, string stacktrace)
        {
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
        }
    }
}
