//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using UnityEngine;

namespace JEM.Test.Unity
{
    [DefaultExecutionOrder(2)]
    public class JEMSmartTextTest : MonoBehaviour
    {
        private void Awake()
        {
            // JEMSmartText Test!
            // Register default events to handle locales
            JEMSmartText.RegisterDefaultEvents();
            JEMSmartText.RegisterEvent("player", (ref string content, string text) =>
            {
                content = "Player nickname!";
            });

            var sampleText = "23rfds <locale=MY_KEY> >!!e=<MY_KEY>>!! <locale=TEXT_OK>%@#FD<>S <player>> <locale=SYSTEM:MY_KEY> <locale=SYSTEM:FORMATTING_TEST;SYSTEM:MY_KEY>";
            Debug.Log(JEMSmartText.CheckAndReplace(sampleText));
        }
    }
}
