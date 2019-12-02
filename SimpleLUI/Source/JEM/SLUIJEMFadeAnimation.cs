//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface.Animation;
using SimpleLUI.API.Core;
using SimpleLUI.API.Util;
using System;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleLUI.JEM
{
    [SLUIComponent]
    public sealed class SLUIJEMFadeAnimation : SLUIComponent
    {
        public float fadeSpeed
        {
            get => Original.FadeSpeed;
            set => Original.FadeSpeed = value;
        }

        public float fadeInScale
        {
            get => Original.FadeInScale;
            set => Original.FadeInScale = value;
        }

        public float fadeOutScale
        {
            get => Original.FadeOutScale;
            set => Original.FadeOutScale = value;
        }

        public string animationMode => Original.AnimationMode.ToString();

        internal JEMInterfaceFadeAnimation Original { get; private set; }

        /// <inheritdoc />
        public override Type ResolveObjectType() => typeof(JEMInterfaceFadeAnimation);

        /// <inheritdoc />
        public override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.AddComponent<JEMInterfaceFadeAnimation>();
        }

        public void SetAnimationMode(string mode)
        {
            if (Enum.TryParse<JEMFadeAnimationMode>(mode, true, out var t))
            {
               SetAnimationMode(t);
            }
            else Debug.LogError($"Failed to parse '{mode}' in to {typeof(JEMFadeAnimationMode)}");
        }

        public void SetAnimationMode(JEMFadeAnimationMode mode) => Original.AnimationMode = mode;

#if UNITY_EDITOR

        /// <inheritdoc />
        public override void CollectObjectDefinition(Object obj)
        {
            var i = (JEMInterfaceFadeAnimation)obj;
            var name = SLUILuaBuilderSyntax.CollectVar(i);
            var parentName = SLUILuaBuilderSyntax.CollectVar(i.GetComponent<RectTransform>());

            String.AppendLine($"local {name} = {parentName}:AddComponent('{nameof(SLUIJEMFadeAnimation)}')");
        }

        /// <inheritdoc />
        public override void CollectObjectProperty(Object obj)
        {
            var i = (JEMInterfaceFadeAnimation)obj;
            var name = SLUILuaBuilderSyntax.CollectVar(i);

            if (Math.Abs(i.FadeSpeed - 24f) > float.Epsilon)
                String.AppendLine($"{name}.fadeSpeed = {i.FadeSpeed.ToString(CultureInfo.InvariantCulture)}");
            if (Math.Abs(i.FadeInScale - 1.06f) > float.Epsilon)
                String.AppendLine($"{name}.fadeInScale = {i.FadeInScale.ToString(CultureInfo.InvariantCulture)}");
            if (Math.Abs(i.FadeOutScale - 1.03f) > float.Epsilon)
                String.AppendLine($"{name}.fadeOutScale = {i.FadeOutScale.ToString(CultureInfo.InvariantCulture)}");
            if (i.AnimationMode != JEMFadeAnimationMode.UsingLocalScale)
                String.AppendLine($"{name}:SetAnimationMode('{i.AnimationMode.ToString()}')");
        }
#endif
    }
}
