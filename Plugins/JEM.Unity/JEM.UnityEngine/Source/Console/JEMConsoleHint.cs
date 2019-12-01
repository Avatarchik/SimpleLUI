//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Console
{
    /// <inheritdoc />
    /// <summary>
    ///     A hint of <see cref="JEMConsole" />
    /// </summary>
    public class JEMConsoleHint : MonoBehaviour
    {
        [Header("Settings")]
        public TextMeshProUGUI Text;
        public Button Button;

        /// <summary>
        ///     Full name of hint.
        /// </summary>
        public string HintFullName { get; private set; }

        /// <summary>
        ///     Prefix of the hint.
        /// </summary>
        public string HintPrefix { get; private set; }

        /// <summary>
        ///     Struct info of the hint.
        /// </summary>
        public string HintStructInfo { get; private set; }

        /// <summary>
        ///     Description of the hint.
        /// </summary>
        public string HintDescription { get; private set; }

        /// <summary>
        ///     Initializes hint.
        /// </summary>
        public void Initialize(string fullName, string prefix, string structInfo, string description)
        {
            HintFullName = fullName;
            HintPrefix = prefix;
            HintStructInfo = structInfo;
            HintDescription = description;

            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() =>
            {
                JEMConsole.Instance.SetConsoleText(HintFullName);
                JEMConsole.Instance.RestartMemoryPosition();
            });
        }

        /// <summary>
        ///     Sets the interface of this hint.
        /// </summary>
        public void SetInterface(string text)
        {
            Text.text = text;
        }
    }
}
