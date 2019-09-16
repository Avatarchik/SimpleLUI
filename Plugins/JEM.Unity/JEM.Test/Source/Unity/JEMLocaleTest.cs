//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using UnityEngine;

namespace JEM.Test.Unity
{
    /// <inheritdoc />
    /// <summary>
    ///     A small script that will test for us JEMLocale functionality.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    internal class JEMLocaleTest : MonoBehaviour
    {
        private void Awake()
        {
            // Load the test 'eng' locale data
            JEMLocale.LoadLocale("eng", "Locale\\eng");
            // Load the test 'pl' locale data
            JEMLocale.LoadLocale("pl", "Locale\\pl");

            // Set the 'eng' locale
            JEMLocale.SetLocale("eng");

            // Run locale test
            TestLocale();

            // Set the 'pl' locale
            JEMLocale.SetLocale("pl");

            // Run locale test
            TestLocale();
        }

        private void TestLocale()
        {
            Debug.Log($"The key MY_KEY returns {JEMLocale.Resolve("MY_KEY")}");
        }
    }
}
