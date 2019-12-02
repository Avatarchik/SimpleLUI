//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    public abstract class SLUIHorizontalOrVerticalLayoutGroup : SLUILayoutGroup
    {
        public bool childControlHeight
        {
            get => Original.childControlHeight;
            set => Original.childControlHeight = value;
        }

        public bool childControlWidth
        {
            get => Original.childControlWidth;
            set => Original.childControlWidth = value;
        }

        public bool childForceExpandHeight
        {
            get => Original.childForceExpandHeight;
            set => Original.childForceExpandHeight = value;
        }

        public bool childForceExpandWidth
        {
            get => Original.childForceExpandWidth;
            set => Original.childForceExpandWidth = value;
        }

        public bool childScaleHeight
        {
            get => Original.childScaleHeight;
            set => Original.childScaleHeight = value;
        }

        public bool childScaleWidth
        {
            get => Original.childScaleWidth;
            set => Original.childScaleWidth = value;
        }

        internal new HorizontalOrVerticalLayoutGroup Original { get; private set; }

        /// <inheritdoc />
        public override void OnComponentLoaded()
        {
            // Invoke base method
            base.OnComponentLoaded();

            Original = (HorizontalOrVerticalLayoutGroup) OriginalComponent;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void CollectObjectProperty(Object obj)
        {
            // Invoke base method.
            base.CollectObjectProperty(obj);

            var t = (HorizontalOrVerticalLayoutGroup) obj;
            var name = SLUILuaBuilderSyntax.CollectVar(t);

            if (t.childControlHeight)
                String.AppendLine($"{name}.childControlHeight = true");
            if (t.childControlWidth)
                String.AppendLine($"{name}.childControlWidth = true");

            if (!t.childForceExpandHeight)
                String.AppendLine($"{name}.childForceExpandHeight = false");
            if (!t.childForceExpandWidth)
                String.AppendLine($"{name}.childForceExpandWidth = false");

            if (t.childScaleHeight)
                String.AppendLine($"{name}.childScaleHeight = true");
            if (t.childScaleWidth)
                String.AppendLine($"{name}.childScaleWidth = true");
        }
#endif
    }
}
