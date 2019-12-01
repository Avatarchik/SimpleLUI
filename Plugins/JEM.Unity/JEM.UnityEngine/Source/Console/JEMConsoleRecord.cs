//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Console
{
    /// <inheritdoc />
    /// <summary>
    ///     A record of <see cref="JEMConsole" />.
    /// </summary>
    internal sealed class JEMConsoleRecord : MonoBehaviour
    {
        [Header("Settings")]
        public TextMeshProUGUI Text;
        public TextMeshProUGUI StackTraceText;

        [Space]
        public Button StackTraceExpandButton;

        /// <summary>
        ///     Updates the interface of this record.
        /// </summary>
        public void SetInterface(string value, string stackTrace)
        {
            if (!isActiveAndEnabled)
            {
                gameObject.LiteSetActive(true);
            }

            if (!string.IsNullOrEmpty(stackTrace))
            {
                StackTraceExpandButton.onClick.RemoveAllListeners();
                StackTraceExpandButton.onClick.AddListener(() =>
                {
                    StackTraceText.gameObject.SetActive(!StackTraceText.gameObject.activeSelf);
                });
            }

            Text.text = value;
            StackTraceText.text = stackTrace;
            StackTraceText.gameObject.SetActive(false);
        }
    }
}
