//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Interface
{
    /// <inheritdoc />
    /// <summary>
    ///     Locale interface component for UI.Text.
    /// </summary>
    [AddComponentMenu("JEM/Interface/JEM Locale Text")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class JEMInterfaceLocaleText : JEMInterfaceLocaleElement
    {
        /// <summary>
        ///     Key of the locale.
        /// </summary>
        [Header("Settings")]
        public string Key = "UNKNOWN_KEY";

        /// <summary>
        ///     Group of the locale.
        /// </summary>
        public string Group = "SYSTEM";

        /// <summary>
        ///     String that will be given as parameter to string.Format
        /// </summary>
        public string AutoFormat;

        /// <summary>
        ///     String that will be added at the beginning of the string.
        /// </summary>
        public string AutoStart;

        /// <summary>
        ///     String that will be added at the end of the string.
        /// </summary>
        public string AutoEnd;

        /// <summary>
        ///     If true, the result string will be updated to upper case.
        /// </summary>
        public bool ToUpper;

        /// <summary>
        ///     Target text of component.
        /// </summary>
        public Text Text { get; private set; }

        /// <inheritdoc />
        public override void RefreshLocale()
        {
            if (Text == null)
                Text = GetComponent<Text>();

            Text.text = AutoStart;
            if (JEMLocale.GetSelectedLocale() != null)
            {
                Text.text += JEMLocale.Resolve(Group, Key, AutoFormat);
            }
            Text.text += AutoEnd;

            if (ToUpper)
            {
                Text.text = Text.text.ToUpper();
            }
        }
    }
}
