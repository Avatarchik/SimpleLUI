//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using System;
using UnityEngine;

namespace JEM.UnityEngine.Console
{
    /// <inheritdoc />
    /// <summary>
    ///     Some utility stuff for <see cref="JEMConsole" />.
    /// </summary>
    [DefaultExecutionOrder(-20)]
    [RequireComponent(typeof(JEMConsole))]
    public sealed class JEMConsoleUtil : MonoBehaviour
    {
        [Header("Color Settings")]
        public string WarningColor = "#F1C40F";
        public string ErrorColor = "#7B241C";
        public string FatalColor = "#E74C3C";
        public string AssertColor = "#A04000";
        public string CommandColor = "#2CB4CC";
        public string ExecutorColor = "#DB23C6";

        /// <summary>
        ///     Called at the beginning of <see cref="FormatLogForInterface"/> to format your custom stuff in to the log.
        ///     As parameter it passes the source string and as a return you should return a 'prefix' of the string that
        ///      will be displayed after dateTime and before the actual content.
        ///     When null or empty prefix is returned, the default one is inserted instead (source string).
        /// </summary>
        public event Func<string, string> OnFormatCustomPrefix; 

        /// <summary>
        ///     Format the received log to draw in console interface.
        /// </summary>
        /// <remarks>A tuple where item1 is a log content and item2 the stackTrace.</remarks>
        public Tuple<string, string> FormatLogForInterface(string source, JEMLogType type, string value, string stacktrace)
        {
            var prefixes = $"[{DateTime.Now:T}]";

            string customPrefix = string.Empty;
            if (OnFormatCustomPrefix != null)
                customPrefix = OnFormatCustomPrefix.Invoke(source);

            if (!string.IsNullOrEmpty(customPrefix))
                prefixes += $" {customPrefix}";
            else
            {
                prefixes += $" {source.ToUpper()}";
#if !DEBUG
                // internal stacktrace should never be drawn to user outside unity
                stacktrace = "STACKTRACE NOT AVAILABLE FOR THIS BUILD";
#endif   
            }

            if (source == "COMMAND")
            {
                value = $"<color={CommandColor}>{value}</color>";
            }
            else if (source == "QNetExecutor")
            {
                value = $"<color={ExecutorColor}>{value}</color>";
            }

            if (string.IsNullOrWhiteSpace(stacktrace) || string.IsNullOrEmpty(stacktrace))
            {
                stacktrace = "Stacktrace is not available for this log.";
            }

            string colouredMessage;
            switch (type)
            {
                case JEMLogType.Log:
                    colouredMessage = $"{prefixes} {value}";
                    break;
                case JEMLogType.Warning:
                    colouredMessage = $"<color={WarningColor}>{prefixes} WARN {value}</color>";
                    break;
                case JEMLogType.Error:
                    colouredMessage = $"<color={ErrorColor}>{prefixes} ERR {value}</color>";
                    break;
                case JEMLogType.Exception:
                    colouredMessage = $"<color={FatalColor}>{prefixes} FATAL {value}</color>";
                    break;
                case JEMLogType.Assert:
                    colouredMessage = $"<color={AssertColor}>{prefixes} ASSERT {value}</color>";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return new Tuple<string, string>(colouredMessage, stacktrace);
        }
    }
}
