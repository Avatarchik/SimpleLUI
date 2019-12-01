//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface.Animation;
using System.Collections;
using UnityEngine;

namespace JEM.UnityEngine.Console
{
    public sealed partial class JEMConsole
    {
        [Header("Error Warning")]
        public JEMInterfaceFadeAnimation ConsoleErrorWarn;
        public float WarnLifeTime = 5.0f;

        private Coroutine _pokeWorker;
        private void PokeWarnMessage()
        {
            if (_pokeWorker != null)
                StopCoroutine(_pokeWorker);

            _pokeWorker = StartCoroutine(PokeWarnWorker());
        }

        private IEnumerator PokeWarnWorker()
        {
            ConsoleErrorWarn.SetActive(true);
            yield return new WaitForSeconds(WarnLifeTime);
            ConsoleErrorWarn.SetActive(false);
            _pokeWorker = null;
        }
    }
}
