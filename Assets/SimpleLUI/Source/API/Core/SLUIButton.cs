//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using NLua;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    public sealed class SLUIButton : SLUISelectable
    {
        private string _onClickEvent;
        internal new Button Original { get; private set; }

        public SLUIButton() { }

        /// <inheritdoc />
        internal override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<Button>();
        }

        /// <inheritdoc />
        internal override void OnComponentLoaded()
        {
            // invoke base method
            base.OnComponentLoaded();

            Original.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(_onClickEvent)) return;
                var func = Manager.State[_onClickEvent] as LuaFunction;
                func?.Call();
            });
        }

        public void SetOnClick(string funcName)
        {
            _onClickEvent = funcName;
        }
    }
}
