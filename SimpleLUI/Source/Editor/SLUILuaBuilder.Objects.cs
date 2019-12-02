//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API;
using System;

namespace SimpleLUI.Editor
{
    internal sealed partial class SLUILuaBuilder
    {
        private void RegisterBuilders()
        {
            var baseType = typeof(SLUIBuilderObject);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var types = assembly.GetTypes();
                for (var index = 0; index < types.Length; index++)
                {
                    var t = types[index];
                    if (!baseType.IsAssignableFrom(t)) continue;
                    if (!t.IsSealed) continue; // only sealed type could be serialized by Lua Builder.
                    var instance = Activator.CreateInstance(t) as SLUIBuilderObject;
                    RegisterBuilder(instance);
                }
            }
        }
    }
}
