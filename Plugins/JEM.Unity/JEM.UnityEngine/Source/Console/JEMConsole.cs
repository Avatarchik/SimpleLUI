//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.Core.Debugging.Commands;
using JEM.Core.Debugging.CVar;
using JEM.UnityEngine.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Console
{
    /// <inheritdoc />
    /// <summary>
    ///     A simple in-game console.
    /// </summary>
    /// <remarks>
    ///     Default execution order is set to -20.
    /// </remarks>
    [DefaultExecutionOrder(-20)]
    [RequireComponent(typeof(JEMConsoleUtil))]
    public sealed partial class JEMConsole : JEMSingletonBehaviour<JEMConsole>
    {
        [Header("Base Settings")]
        public GameObject ConsolePanel;

        [Space]
        public Transform ConsoleMessagesRoot;
        public GameObject ConsoleMessagePrefab;

        [Space]
        public InputField ConsoleInput;

        [Header("Input Settings")]
        public KeyCode ConsoleActivationKey = KeyCode.F1;

        /// <summary>
        ///     Defines the console active state.
        /// </summary>
        public bool IsConsoleActive { get; private set; }

        /// <summary>
        ///     Reference to the <see cref="JEMConsoleUtil"/>.
        /// </summary>
        public JEMConsoleUtil ConsoleUtility { get; private set; }

        /// <summary>
        ///     List of all records in console.
        /// </summary>
        private List<JEMConsoleRecord> Records { get; } = new List<JEMConsoleRecord>();

        /// <summary>
        ///     Called at the very beginning of <see cref="ExecuteCommand"/> method for custom commands execution.
        ///     Return true if command has been found.
        /// </summary>
        public event Func<string, bool> OnCommandExecution; 

        /// <inheritdoc />
        protected override void OnAwake()
        {
            // Get components
            ConsoleUtility = GetComponent<JEMConsoleUtil>();
            gameObject.AddComponent<JEMObjectKeepOnScene>();

            // Prepare records pool.
            for (int index = 0; index < MaxConsoleMessages; index++)
                CreateAndPoolMessage(true);

            // Setup hints.
            Invoke(nameof(SetupHints), 0.1f);

            // Set console activation state.
            SetConsoleActive(false);

            // Load console memory
            LoadMemoryFile();

            // Hook JEMLogger event.
            JEMLogger.OnLogAppended += OnJEMLogAppended;
        }

        private void OnApplicationQuit()
        {         
            // Save console memory
            SaveMemoryFile();
        }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            JEMLogger.OnLogAppended -= OnJEMLogAppended;
        }

        private void OnJEMLogAppended(string source, JEMLogType type, string value, string stacktrace)
        {
            switch (type)
            {
                case JEMLogType.Log:
                case JEMLogType.Warning:
                case JEMLogType.Assert:
                    break;
                case JEMLogType.Error:
                case JEMLogType.Exception:
                    PokeWarnMessage();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var result = ConsoleUtility.FormatLogForInterface(source, type, value, stacktrace);
            AppendMessage(result.Item1, result.Item2);
        }

        private void OnGUI()
        {
            var e = Event.current;
            if (IsConsoleActive)
            {
                var isAnyHintSelected = IsAnyHintSelected();
                if (!isAnyHintSelected)
                {
                    if (e.isKey && e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.UpArrow)
                        {
                            if (MoveMemoryUp(out var str))
                                SetConsoleText(str);

                            return;
                        }
                        else if (e.keyCode == KeyCode.DownArrow)
                        {
                            if (MoveMemoryDown(out var str))
                            {
                                SetConsoleText(str);
                                return;
                            }
                        }
                        else
                        {
                            // If user is typing, always reset the memory position.
                            RestartMemoryPosition();
                        }
                    }
                }

                UpdateHintsNavigation();

                if (!isAnyHintSelected && !ConsoleInput.isFocused)
                     ConsoleInput.ActivateInputField();
                else if (ConsoleInput.isFocused)
                {
                    var inputHasContent = !string.IsNullOrEmpty(ConsoleInput.text) && ConsoleInput.text.Length > 0;
                    if (e.isKey && e.keyCode == KeyCode.Return && e.type == EventType.KeyDown && inputHasContent)
                    {
                        ExecuteCommand(ConsoleInput.text);
                        ConsoleInput.text = string.Empty;
                    }
                }
            }

            if (e.isKey && e.keyCode == KeyCode.F1 && e.type == EventType.KeyDown)
            {
                SetConsoleActive(!IsConsoleActive);
            }
        }

        /// <summary>
        ///     Basically add new message to console.
        /// </summary>
        private void AppendMessage(string message, string stackTrace)
        {
            // Get the record.
            JEMConsoleRecord record;
            if (MaxConsoleMessages > 0 && Records.Count >= MaxConsoleMessages)
            {
                record = Records[0];
                Records.RemoveAt(0);
                Records.Add(record);
                record.transform.SetAsLastSibling();
            }
            else
            {
                record = CreateAndPoolMessage(false);
            }

            // Set the interface of resolved record.
            record.SetInterface(message, stackTrace);

            // Make sure that record is active.
            if (!record.isActiveAndEnabled)
            {
                record.gameObject.SetActive(true);
            }
        }

        /// <summary>
        ///     Creates new <see cref="JEMConsoleRecord"/> and adds it to the pool.
        /// </summary>
        private JEMConsoleRecord CreateAndPoolMessage(bool disableAtStart)
        {
            var obj = JEMObject.Instantiate(ConsoleMessagePrefab, ConsoleMessagesRoot);
            var record = obj.GetComponent<JEMConsoleRecord>();
            Records.Add(record);
            obj.SetActive(!disableAtStart);
            return record;
        }

        /// <summary>
        ///     Executes the command from console input string.
        /// </summary>
        public void ExecuteCommand(string command)
        {
            DisableHints();
            AppendMessage($"<color=#8bc34a>{command}</color>", new StackTrace().ToString());

            if (!MemorizeOnlyValid)
            {
                InsertToMemoryAndReset(command);
            }

            if (OnCommandExecution != null)
            {
                if (OnCommandExecution.Invoke(command))
                {
                    if (MemorizeOnlyValid)
                    {
                        InsertToMemoryAndReset(command);
                    }

                    return;
                }
            }

            // Try to edit cVar.
            if (TryExecuteCVar(command, out var successful))
            {
                if (successful && MemorizeOnlyValid)
                    InsertToMemoryAndReset(command);

                return;
            }

            var err = JEMCommandManager.ExecuteRaw(command);
            if (!string.IsNullOrEmpty(err))
            {
                JEMLogger.LogWarning($"Command exec failed. {err}", "COMMAND");
            }
            else
            {
                if (MemorizeOnlyValid)
                {
                    InsertToMemoryAndReset(command);
                }
            }
        }

        /// <summary>
        ///     Sets the console text.
        /// </summary>
        public void SetConsoleText(string str)
        {
            ConsoleInput.text = str;
            ConsoleInput.ActivateInputField();
            Invoke(nameof(FixInputField), 0.1f);

            OnConsoleInputChange();
        }

        /// <summary>
        ///     Called when user clicks on hint, it fixes the carret position.
        /// </summary>
        internal void FixInputField()
        {
            ConsoleInput.MoveTextEnd(false);
        }

        /// <summary>
        ///     Set the console activation state.
        /// </summary>
        public void SetConsoleActive(bool state)
        {
            IsConsoleActive = state;

            ConsolePanel.SetActive(state);
            if (state)
            {
                ConsoleInput.ActivateInputField();
                DisableHints();
                RestartMemoryPosition();
            }
        }

        /// <summary>
        ///     Try to edit CVar via command.
        /// </summary>
        /// <returns>Returns false when operation has failed and execute queue may continue.</returns>
        public static bool TryExecuteCVar(string command, out bool successful)
        {
            successful = false;
            var cVarArgs = command.Split(' ');
            if (cVarArgs.Length >= 1)
            {
                var cVar = JEMCVarManager.GetCVarData(cVarArgs[0]);
                if (cVar != null)
                {
                    if (cVarArgs.Length < 2)
                    {
                        JEMLogger.Log($"{cVar.Name} is {cVar.GetString()}", "COMMAND");
                        return true;
                    }

                    var cVarErr = JEMCVarManager.EditCVar(cVarArgs[0], cVarArgs[1]);
                    if (!string.IsNullOrEmpty(cVarErr))
                    {
                        JEMLogger.LogError($"CVar exec failed. {cVarErr}", "COMMAND");
                    }
                    else successful = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Amount of maximal console messages that can be instanced at once.
        /// </summary>
        [JEMCVar("console.max_messages", "Amount of maximal console messages that can be instanced at once.")]
        public static int MaxConsoleMessages { get; set; } = 120;
    }
}
