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
        public SLUIUnityEvent onClickUnityEvent;

        private string _onClickEventDefault;

        internal new Button Original { get; private set; }

        public SLUIButton() { }

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

        public void SetOnClick(string funcName)
        {
            _onClickEventDefault = funcName;
        }
    }
}
