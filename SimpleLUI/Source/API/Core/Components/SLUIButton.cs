//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using NLua;
using SimpleLUI.API.Util;
using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SimpleLUI.API.Core.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     A standard button that can be clicked in order to trigger an event.
    /// </summary>
    public sealed class SLUIButton : SLUISelectable
    {
        /// <summary>
        ///     UnityEvent that is triggered when the Button is pressed.
        /// </summary>
        public SLUIUnityEvent onClickUnityEvent;

        private string _onClickEventDefault;

        internal new Button Original { get; private set; }

        /// <summary/>
        public SLUIButton() { }

        /// <inheritdoc />
        public override Type ResolveObjectType() => typeof(Button);

        /// <inheritdoc />
        public override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<Button>();
        }

        /// <inheritdoc />
        public override void OnComponentLoaded()
        {
            // invoke base method
            base.OnComponentLoaded();

            Original.onClick.AddListener(() =>
            {
                if (onClickUnityEvent != null)
                    Core.ExecuteUnityEvent(onClickUnityEvent);

                if (string.IsNullOrEmpty(_onClickEventDefault)) return;
                var func = Manager.State[_onClickEventDefault] as LuaFunction;
                func?.Call();
            });
        }

        /// <summary>
        ///     Sets the onClick event hook that is triggered when the Button is pressed.
        /// </summary>
        public void SetOnClick(string funcName)
        {
            _onClickEventDefault = funcName;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void CollectObjectDefinition(Object obj)
        {
            var b = (Button)obj;
            var parentName = SLUILuaBuilderSyntax.CollectVar(b.GetComponent<RectTransform>());
            var name = SLUILuaBuilderSyntax.CollectVar(b);

            String.AppendLine($"local {name} = {parentName}:AddComponent('Button')");
        }

        /// <inheritdoc />
        public override void CollectObjectProperty(Object obj)
        {
            var b = (Button)obj;
            var name = SLUILuaBuilderSyntax.CollectVar(b);

            String.AppendLine(
                $"{name}.targetGraphic = {(b.targetGraphic == null ? "nil" : SLUILuaBuilderSyntax.CollectVar(b.image))}");
            if (!b.interactable)
                String.AppendLine($"{name}.interactable = false");
            String.AppendLine($"{name}.normalColor = {SLUILuaBuilderSyntax.CollectColor(b.colors.normalColor)}");
            String.AppendLine(
                $"{name}.highlightedColor = {SLUILuaBuilderSyntax.CollectColor(b.colors.highlightedColor)}");
            String.AppendLine($"{name}.pressedColor = {SLUILuaBuilderSyntax.CollectColor(b.colors.pressedColor)}");
            String.AppendLine($"{name}.selectedColor = {SLUILuaBuilderSyntax.CollectColor(b.colors.selectedColor)}");
            String.AppendLine($"{name}.disabledColor = {SLUILuaBuilderSyntax.CollectColor(b.colors.disabledColor)}");

            if (b.onClick.GetPersistentEventCount() != 0)
            {
                String.AppendLine(SLUILuaBuilderSyntax.CollectEvent(b, b.onClick, b.GetComponent<SLUIUnityEventHelper>(), out var eventName));
                String.AppendLine($"{name}.onClickUnityEvent = {eventName}");
            }
        }
#endif
    }
}
