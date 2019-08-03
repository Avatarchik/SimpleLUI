//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using NLua;
using SimpleLUI.API;

namespace SimpleLUI
{
    internal class SLUIWorker
    {
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
            state["debug"] = Debugger;
            state["core"] = Core;
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
