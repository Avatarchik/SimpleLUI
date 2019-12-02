//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Util;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleLUI.API.Core.Components
{
    public sealed class SLUICanvasGroup : SLUIComponent
    {
        public float alpha
        {
            get => Original.alpha;
            set => Original.alpha = value;
        }

        public bool blocksRaycasts
        {
            get => Original.blocksRaycasts;
            set => Original.blocksRaycasts = value;
        }

        public bool ignoreParentGroups
        {
            get => Original.ignoreParentGroups;
            set => Original.ignoreParentGroups = value;
        }

        public bool interactable
        {
            get => Original.interactable;
            set => Original.interactable = value;
        }

        internal new CanvasGroup Original { get; private set; }

        /// <inheritdoc />
        public override Type ResolveObjectType() => typeof(CanvasGroup);

        /// <inheritdoc />
        public override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<CanvasGroup>();
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void CollectObjectDefinition(Object obj)
        {
            var t = (CanvasGroup) obj;
            var parentName = SLUILuaBuilderSyntax.CollectVar(t.GetComponent<RectTransform>());
            var name = SLUILuaBuilderSyntax.CollectVar(t);

            String.AppendLine($"local {name} = {parentName}:AddComponent('CanvasGroup')");
        }

        /// <inheritdoc />
        public override void CollectObjectProperty(Object obj)
        {
            var t = (CanvasGroup) obj;
            var name = SLUILuaBuilderSyntax.CollectVar(t);
            
            if (System.Math.Abs(t.alpha - 1f) > float.Epsilon)
                String.AppendLine($"{name}.alpha = {t.alpha}");
            if (!t.interactable)
                String.AppendLine($"{name}.interactable = false");
            if (!t.blocksRaycasts)
                String.AppendLine($"{name}.blocksRaycasts = false");
            if (t.ignoreParentGroups)
                String.AppendLine($"{name}. ignoreParentGroups = true");
        }
#endif
    }
}
