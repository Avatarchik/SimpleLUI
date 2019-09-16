//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Interface
{
    /// <inheritdoc />
    /// <summary>
    ///     JEM Interface Tab.
    ///     A simple script that binds selected array of buttons with selected panels
    ///      so the only one panel could be active at the time.
    /// </summary>
    [AddComponentMenu("JEM/Interface/Util/JEM Tab Controller")]
    [DisallowMultipleComponent]
    internal class JEMInterfaceTab : MonoBehaviour
    {
        [Serializable]
        public class Item
        {
            public JEMInterfaceFadeAnimation Panel;
            public Button Activator;
        }

        /// <summary>
        ///     Our list of items to bind.
        /// </summary>
        [Header("Settings")]
        [SerializeField]
        internal Item[] Items = new Item[0];

        /// <summary>
        ///     Amount of time to wait between panel activation.
        /// </summary>
        internal float ActivationWait = 0.5f;

        /// <summary>
        ///     If true, the first item in list will be activated at Start.
        /// </summary>
        internal bool ActivateOnStart = true;

        private Coroutine _coroutine;
        private void Start()
        {
            if (Items.Length == 0)
                return;

            foreach (var i in Items)
            {
                i.Activator.onClick.AddListener(() =>
                {
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(Activate(i));
                });
            }

            if (ActivateOnStart)
                Items[0].Activator.onClick.Invoke();
        }

        private IEnumerator Activate(Item i)
        {
            foreach (var i2 in Items)
            {
                if (i == i2) continue;
                i2.Panel.SetActive(false);
            }

            yield return new WaitForSeconds(ActivationWait);
            i.Panel.SetActive(true);
            _coroutine = null;
        }
    }
}
