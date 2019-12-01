//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using System;
using System.Collections.Generic;

namespace JEM.Core.Text
{
    /// <summary>
    ///     The delegate of JEMSmartText.
    /// </summary>
    public delegate void JEMSmartTextEvent(ref string content, string sourceText);

    /// <summary>
    ///     JEM Smart Text Event Data.
    ///     A data of smart text event :)
    /// </summary>
    public class JEMSmartTextEventData
    {
        /// <summary>
        ///     Name of the event.
        /// </summary>
        public string EventName;

        /// <summary>
        ///     The event.
        /// </summary>
        public JEMSmartTextEvent OnEventTrigger;
    }

    // TODO: Make JEMSmartText not static so you could create multiple JEMSmartText instances with different configurations.

    /// <summary>
    ///     JEM Smart Text.
    ///     A text formatting utility that will help with standard text formatting operations.
    ///     It can for ex. replace locale="key" with the key's value.
    ///     JEMSmartText gives also ability to define custom keys with it's own replacements events.
    /// </summary>
    public static class JEMSmartText
    {
        /// <summary>
        ///     Registers default events like for ex. JEMLocale keys replacement.
        /// </summary>
        public static void RegisterDefaultEvents()
        {
            RegisterEvent("locale", (ref string content, string text) =>
            {
                if (string.IsNullOrEmpty(content))
                    return;

                // Split content to find additional parameters.
                var args = content.Split(';');
                var mainLocale = args[0];

                var argsObjects = new object[0];
                if (args.Length > 1)
                {
                    argsObjects = new object[args.Length - 1];
                    for (int index = 1; index < args.Length; index++)
                    {
                        argsObjects[index - 1] = args[index];
                    }
                }

                // Try apply locale to parameters as well.
                var selectedLocale = JEMLocale.GetSelectedLocale();
                if (selectedLocale != null)
                {
                    for (int index = 1; index < args.Length; index++)
                    {
                        var groupNKey = args[index].Split(':');
                        var group = selectedLocale.GetGroup(groupNKey[0]) ?? selectedLocale.GetDefault();
                        if (group != null)
                        {
                            var key = groupNKey.Length == 1 ? groupNKey[0] : groupNKey[1];
                            if (group.Keys.ContainsKey(key))
                            {
                                argsObjects[index - 1] = group.Keys[key];
                            }
                        }
                    }

                    var mainGroupNKey = mainLocale.Split(':');
                    var mainGroup = selectedLocale.GetGroup(mainGroupNKey[0]) ?? selectedLocale.GetDefault();
                    if (mainGroup != null)
                    {
                        var key = mainGroupNKey.Length == 1 ? mainGroupNKey[0] : mainGroupNKey[1];
                        if (mainGroup.Keys.ContainsKey(key))
                        {
                            mainLocale = mainGroup.Keys[key];
                        }
                    }

                    content = string.Format(mainLocale, argsObjects);
                }
            });
        }

        /// <summary>
        ///     Creates a string that JEMSmartText could use to resolve a data from JEMLocale.
        /// </summary>
        public static string FormatLocaleString(string key, params string[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var a = string.Empty;
            for (var index1 = 0; index1 < args.Length; index1++)
            {
                var i = args[index1];
                a += ";";
                a += $"{i}";
            }

            return $"<locale={key}{a}>";
        }

        /// <summary>
        ///     Creates a string that JEMSmartText could use to resolve a data from JEMLocale.
        /// </summary>
        public static string FormatLocaleStringTuple(string key, params Tuple<string, string>[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var a = string.Empty;
            for (var index1 = 0; index1 < args.Length; index1++)
            {
                var i = args[index1];
                a += ";";
                a += $"{i.Item1}:{i.Item2}";
            }

            return $"<locale={key}{a}>";
        }

        /// <summary>
        ///     Creates a string that JEMSmartText could use to resolve a data from JEMLocale.
        /// </summary>
        public static string FormatLocaleString(string group, string key, params string[] args)
        {
            if (@group == null) throw new ArgumentNullException(nameof(@group));
            if (key == null) throw new ArgumentNullException(nameof(key));
            var a = string.Empty;
            for (var index1 = 0; index1 < args.Length; index1++)
            {
                var i = args[index1];
                a += ";";
                a += $"{i}";
            }

            return $"<locale={group}:{key}{a}>";
        }

        /// <summary>
        ///     Creates a string that JEMSmartText could use to resolve a data from JEMLocale.
        /// </summary>
        public static string FormatLocaleStringTuple(string group, string key, params Tuple<string, string>[] args)
        {
            if (@group == null) throw new ArgumentNullException(nameof(@group));
            if (key == null) throw new ArgumentNullException(nameof(key));
            var a = string.Empty;
            for (var index1 = 0; index1 < args.Length; index1++)
            {
                var i = args[index1];
                a += ";";
                a += $"{i.Item1}:{i.Item2}";
            }

            return $"<locale={group}:{key}{a}>";
        }

        /// <summary>
        ///     Registers new event.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RegisterEvent(string eventName, JEMSmartTextEvent @event)
        {
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            if (GetEvent(eventName) != null)
                throw new NotSupportedException($"Event of name {eventName} already exists.");

            var newEvent = new JEMSmartTextEventData
            {
                EventName = eventName,
                OnEventTrigger = @event
            };

            Events.Add(newEvent);
        }

        /// <summary>
        ///     Checks if given string known key and replaces it with received by event new string.
        /// </summary>
        public static string CheckAndReplace(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                    return str;

                int index = 0;
                bool isKeyBegin = false;
                int keyBeginIndex = 0;
                while (index < str.Length)
                {
                    if (str[index] == '<')
                    {
                        isKeyBegin = true;
                        keyBeginIndex = index;
                    }
                    else if (isKeyBegin && str[index] == '>')
                    {
                        isKeyBegin = false;

                        var full = str.Substring(keyBeginIndex + 1, index - keyBeginIndex - 1).Split('=');
                        var key = full[0];
                        string content = null;
                        var keyEvent = GetEvent(key);
                        if (keyEvent != null)
                        {
                            if (full.Length >= 2)
                            {
                                content = full[1];
                            }

                            keyEvent.OnEventTrigger(ref content, str);
                            // TODO: Check if received content has any of registered events to prevent infinite loop
                        }

                        str = str.Remove(keyBeginIndex, index - keyBeginIndex + 1); // somehow > is not being removed so we need to move count by one
                        if (!string.IsNullOrEmpty(content))
                        {
                            str = str.Insert(keyBeginIndex, content);
                        }

                        // There was a bug that only one key in string has been resolving properly.
                        // So.. after the key end we need to restart the loop
                        index = 0;
                    }

                    index++;
                }


                return str;
            }
            catch (Exception e)
            {
                JEMLogger.LogException(e);
                throw;
            }
        }

        /// <summary>
        ///     Gets the ata of event of given name.
        /// </summary>
        public static JEMSmartTextEventData GetEvent(string eventName)
        {
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            foreach (var e in Events)
            {
                if (e.EventName == eventName)
                    return e;
            }

            return null;
        }

        /// <summary>
        ///     List of all registered events.
        /// </summary>
        public static List<JEMSmartTextEventData> Events { get; } = new List<JEMSmartTextEventData>();
    }
}
