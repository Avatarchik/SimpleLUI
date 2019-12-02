//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Util;
using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SimpleLUI.API.Core.Components
{
    public sealed class SLUIVerticalLayoutGroup : SLUIHorizontalOrVerticalLayoutGroup
    {
        /// <inheritdoc />
        public override Type ResolveObjectType() => typeof(VerticalLayoutGroup);

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void CollectObjectDefinition(Object obj)
        {
            var t = (VerticalLayoutGroup) obj;
            var parentName = SLUILuaBuilderSyntax.CollectVar(t.GetComponent<RectTransform>());
            var name = SLUILuaBuilderSyntax.CollectVar(t);

            String.AppendLine($"local {name} = {parentName}:AddComponent('VerticalLayoutGroup')");
        }
#endif
    }
}
