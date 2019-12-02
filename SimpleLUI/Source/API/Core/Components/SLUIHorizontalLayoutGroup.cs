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
    public sealed class SLUIHorizontalLayoutGroup : SLUIHorizontalOrVerticalLayoutGroup
    {
        /// <inheritdoc />
        public override Type ResolveObjectType() => typeof(HorizontalLayoutGroup);

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void CollectObjectDefinition(Object obj)
        {
            var t = (HorizontalLayoutGroup) obj;
            var parentName = SLUILuaBuilderSyntax.CollectVar(t.GetComponent<RectTransform>());
            var name = SLUILuaBuilderSyntax.CollectVar(t);

            String.AppendLine($"local {name} = {parentName}:AddComponent('HorizontalLayoutGroup')");
        }
#endif
    }
}
