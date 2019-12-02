//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Core.Math;
using SimpleLUI.API.Util;
using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SimpleLUI.API.Core
{
    public abstract class SLUILayoutGroup : SLUIComponent
    {
        public SLUIRectOffset padding
        {
            get => Original.padding.ToSLUIRect();
            set => Original.padding = value.ToRealRect();
        }

        public string childAlignment
        {
            get => Original.childAlignment.ToString();
            set
            {
                if (Enum.TryParse<TextAnchor>(value, true, out var t))
                {
                    Original.childAlignment = t;
                }
                else Debug.LogError($"Failed to parse '{value}' in to {typeof(TextAnchor)}");
            }
        }

        internal new LayoutGroup Original { get; private set; }

        /// <inheritdoc />
        public override void OnComponentLoaded()
        {
            Original = (LayoutGroup) OriginalComponent;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void CollectObjectProperty(Object obj)
        {
            var t = (LayoutGroup) obj;
            var name = SLUILuaBuilderSyntax.CollectVar(t);

            String.AppendLine($"{name}.padding = {SLUILuaBuilderSyntax.CollectRectOffset(t.padding)}");
            String.AppendLine($"{name}.childAlignment = '{t.childAlignment.ToString()}'");
        }
#endif
    }
}
