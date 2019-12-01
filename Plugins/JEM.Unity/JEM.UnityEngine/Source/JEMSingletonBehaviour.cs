//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     A base class that implements a 'Singleton pattern' for unity scritps.
    /// </summary>
    public abstract class JEMSingletonBehaviour<TScript> : MonoBehaviour where TScript : JEMSingletonBehaviour<TScript>
    {
        protected void Awake()
        {
            if (Instance != null)
            {
                gameObject.SetActive(false);
                OnDuplicate();
                return;
            }

            Instance = (TScript) this;
            OnAwake();
        }

        /// <summary>
        ///     OnAwake method called directly by unity's <see cref="Awake"/> method.
        ///     IMPORTANT: This method is called only on first object, for duplicates use <see cref="OnDuplicate"/> instead.
        /// </summary>
        protected abstract void OnAwake();

        /// <summary>
        ///     OnDuplicate called when <see cref="Instance"/> is set, and this exact object is a duplicate.
        /// </summary>
        protected virtual void OnDuplicate() { }

        /// <summary>
        ///     Reference to the active instance of <see cref="JEMSingletonBehaviour{TScript}"/>.
        /// </summary>
        public static TScript Instance { get; private set; }
    }
}
