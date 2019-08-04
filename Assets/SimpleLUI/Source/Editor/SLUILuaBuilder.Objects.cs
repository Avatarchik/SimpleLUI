//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.Editor.API;
using System;

namespace SimpleLUI.Editor
{
    internal sealed partial class SLUILuaBuilder
    {
        private void RegisterBuilders()
        {
            RegisterBuilder(new SLUIBuilderRectTransform());
            RegisterBuilder(new SLUIBuilderCanvasRenderer());
            RegisterBuilder(new SLUIBuilderImage());
            RegisterBuilder(new SLUIBuilderText());
            RegisterBuilder(new SLUIBuilderButton());

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in a.GetTypes())
                {
                    var attributes = t.GetCustomAttributes(typeof(SLUIBuilderObjectAttribute), true);
                    if (attributes.Length > 0)
                    {
                        var instance = (SLUIBuilderObject) Activator.CreateInstance(t);
                        RegisterBuilder(instance);
                    }
                }
            }
        }
    }
}
