//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.API
{
    internal class SLUIDebugger
    {
        internal SLUIWorker Parent { get; }

        internal SLUIDebugger(SLUIWorker parent)
        {
            Parent = parent;
        }

        public void Log(string str) => Debug.Log($"SLUI ({Parent.Parent.Name}) Debugger :: {str}");
        public void LogWarning(string str) => Debug.LogWarning($"SLUI ({Parent.Parent.Name}) Debugger :: {str}");
        public void LogError(string str) => Debug.LogError($"SLUI ({Parent.Parent.Name}) Debugger :: {str}");
    }
}
