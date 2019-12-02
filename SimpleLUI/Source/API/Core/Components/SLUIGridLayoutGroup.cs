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

namespace SimpleLUI.API.Core.Components
{
    public sealed class SLUIGridLayoutGroup : SLUILayoutGroup
    {
        public SLUIVector2 cellSize
        {
            get => Original.cellSize.ToSLUIVector();
            set => Original.cellSize = value.ToRealVector();
        }

        public string constraint
        {
            get => Original.constraint.ToString();
            set
            {
                if (Enum.TryParse<GridLayoutGroup.Constraint>(value, true, out var t))
                {
                    Original.constraint = t;
                }
                else Debug.LogError($"Failed to parse '{value}' in to {typeof(GridLayoutGroup.Constraint)}");
            }
        }

        public int constraintCount
        {
            get => Original.constraintCount;
            set => Original.constraintCount = value;
        }

        public SLUIVector2 spacing
        {
            get => Original.spacing.ToSLUIVector();
            set => Original.spacing = value.ToRealVector();
        }

        public string startAxis
        {
            get => Original.startAxis.ToString();
            set
            {
                if (Enum.TryParse<GridLayoutGroup.Axis>(value, true, out var t))
                {
                    Original.startAxis = t;
                }
                else Debug.LogError($"Failed to parse '{value}' in to {typeof(GridLayoutGroup.Axis)}");
            }
        }

        public string startCorner
        {
            get => Original.startCorner.ToString();
            set
            {
                if (Enum.TryParse<GridLayoutGroup.Corner>(value, true, out var t))
                {
                    Original.startCorner = t;
                }
                else Debug.LogError($"Failed to parse '{value}' in to {typeof(GridLayoutGroup.Corner)}");
            }
        }

        internal new GridLayoutGroup Original { get; private set; }

        /// <inheritdoc />
        public override Type ResolveObjectType() => typeof(GridLayoutGroup);

        /// <inheritdoc />
        public override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<GridLayoutGroup>();
        }

#if UNITY_EDITOR

        /// <inheritdoc />
        public override void CollectObjectDefinition(Object obj)
        {
            var t = (GridLayoutGroup) obj;
            var parentName = SLUILuaBuilderSyntax.CollectVar(t.GetComponent<RectTransform>());
            var name = SLUILuaBuilderSyntax.CollectVar(t);

            String.AppendLine($"local {name} = {parentName}:AddComponent('GridLayoutGroup')");
        }

        /// <inheritdoc />
        public override void CollectObjectProperty(Object obj)
        {
            // Invoke base method.
            base.CollectObjectProperty(obj);

            var t = (GridLayoutGroup) obj;
            var name = SLUILuaBuilderSyntax.CollectVar(t);

            String.AppendLine($"{name}.cellSize = {SLUILuaBuilderSyntax.CollectVector2(t.cellSize)}");
            String.AppendLine($"{name}.spacing = {SLUILuaBuilderSyntax.CollectVector2(t.spacing)}");
            if (t.startCorner != GridLayoutGroup.Corner.UpperLeft)
                String.AppendLine($"{name}.startCorner = {'"'}{t.startCorner}{'"'}");
            if (t.startAxis != GridLayoutGroup.Axis.Horizontal)
                String.AppendLine($"{name}.startAxis = {'"'}{t.startAxis}{'"'}");
            if (t.constraint != GridLayoutGroup.Constraint.Flexible)
            {
                String.AppendLine($"{name}.constraint = {'"'}{t.constraint}{'"'}");
                String.AppendLine($"{name}.constraintCount = {t.constraintCount}");
            }
        }
#endif
    }
}
