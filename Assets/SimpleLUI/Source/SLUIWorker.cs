//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using NLua;
using SimpleLUI.API;
using UnityEngine;

namespace SimpleLUI
{
    internal class SLUIWorker
    {
        public delegate void EventHandler(string s);

        internal SLUIDebugger Debugger { get; }
        internal SLUICore Core { get; }

        internal SLUIManager Parent { get; }
        internal SLUIWorker(SLUIManager parent)
        {
            Parent = parent;

            Debugger = new SLUIDebugger(this);
            Core = new SLUICore(this);
        }

        internal void PrepareState(Lua state)
        {
            state.LoadCLRPackage();
            state.RegisterFunction("setEventHandler", this, GetType().GetMethod("SetEventHandler"));

            state["debug"] = Debugger;
            state["core"] = Core;
        }


        public void SetEventHandler(EventHandler eventHandler)
        {
            Debug.Log("Event hanlder?");
            eventHandler.Invoke("xd");
        }

        /// <summary>
        ///     Clears/unloads all the objects and resources created by worker.
        /// </summary>
        internal void ClearWorker()
        {
            Core.DestroyContent();
        }
    }
}
