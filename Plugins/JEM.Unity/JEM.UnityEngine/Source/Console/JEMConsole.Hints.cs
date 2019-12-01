//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging.Commands;
using JEM.Core.Debugging.CVar;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.UnityEngine.Console
{
    public sealed partial class JEMConsole
    {
        [Header("Hints Settings")]
        public GameObject ConsoleHints;
        public RectTransform ConsoleHintContent;
        public GameObject ConsoleHintPrefab;

        /// <summary>
        ///     List of all hints console can draw.
        /// </summary>
        private List<JEMConsoleHint> Hints { get; } = new List<JEMConsoleHint>();

        /// <summary>
        ///     Called at the end of <see cref="RegenerateHints"/> so you can insert hints of your custom command system.
        /// </summary>
        public event Action OnGenerateHints;

        private void SetupHints()
        {
            // Regenerate the hints.
            RegenerateHints();

            // Hook the input change event.
            ConsoleInput.onValueChanged.AddListener(str => { OnConsoleInputChange(); });
        }

        private void UpdateHintsNavigation()
        {
            var e = Event.current;
            if (e.isKey && e.keyCode == KeyCode.DownArrow && e.type == EventType.KeyDown)
            {
                if (FirstHintDrawn != null && ConsoleInput.isFocused)
                {
                    JEMOperation.InvokeAction(0.1f,
                        () => { EventSystem.current.SetSelectedGameObject(FirstHintDrawn.Button.gameObject); });
                }
            }
        }

        /// <summary>
        ///     Checks if any of the hints is currently selected.
        /// </summary>
        private bool IsAnyHintSelected()
        {
            if (!ConsoleHints.activeSelf)
                return false;

            foreach (var h in Hints)
            {
                if (h.Button.gameObject == EventSystem.current.currentSelectedGameObject)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///    Regenerates cashed list of hints in console.
        /// </summary>
        public void RegenerateHints()
        {
            // Destroy old hints.
            foreach (var hint in Hints)
                Destroy(hint.gameObject);
            Hints.Clear();

            // Generate hints from JEMCommandManager
            var allCommands = JEMCommandManager.GetAllCommands();
#if DEBUG
            
#endif
            foreach (var consoleCommand in allCommands)
            {
                var hint = CreateAndPoolHint();
                var hintName = $"{consoleCommand.Group.GroupName}.{consoleCommand.Name}";
                string parameters = "";
                foreach (var parameter in consoleCommand.Parameters)
                {
                    if (parameter.ParameterType.Name == "QNetExecutor") continue; // big yikes
                    if (!string.IsNullOrEmpty(parameters))
                        parameters += " ";
                    parameters += $"[{parameter.ParameterType.Name}]";
                }

                if (!string.IsNullOrEmpty(parameters)) parameters = $"<b>{parameters}</b>";
                var execScope = consoleCommand.ExecutionScope != JEMCommandExecScope.Always ? $"<color=#4292f4>|{consoleCommand.ExecutionScope.ToString().ToLower()}</color>" : string.Empty;
                hint.Initialize(hintName, $"<color=#34cc2c>(cmd{execScope})</color>", parameters, consoleCommand.Description);
            }

            // Generate hints from JEMCVarManager
            foreach (var cVar in JEMCVarManager.RegisteredVars)
            {
                var hint = CreateAndPoolHint();
                var hintName = cVar.Name;
                var isNet = cVar.IsNetworkVar ? "<color=#4292f4>|net</color>" : string.Empty;
                hint.Initialize(hintName, $"<color=#f4bc42>(var{isNet})</color>", $"{cVar.GetString().ToLower()} <b>[{cVar.FixedDataType}]</b>", cVar.Description);
            }

            OnGenerateHints?.Invoke();
        }

        /// <summary>
        ///     Creates new <see cref="JEMConsoleHint"/> and adds it to the pool.
        /// </summary>
        public JEMConsoleHint CreateAndPoolHint()
        {
            var obj = JEMObject.Instantiate(ConsoleHintPrefab, ConsoleHintContent);
            var hint = obj.GetComponent<JEMConsoleHint>();
            Hints.Add(hint);
            obj.SetActive(false);
            return hint;
        }

        private JEMConsoleHint FirstHintDrawn;

        /// <summary>
        ///     An event which is called whenever console input changes.
        /// </summary>
        private void OnConsoleInputChange()
        {
            var command = ConsoleInput.text;
            if (string.IsNullOrEmpty(command))
            {
                ConsoleHints.SetActive(false);
                return;
            }

            // Set hints active
            var hintsDrawn = new List<Tuple<JEMConsoleHint, int>>();
            foreach (var hint in Hints)
            {
                var contains = hint.HintFullName.Contains(command);
                if (contains)
                {
                    var toDraw = ApplyTextHighlight(hint.HintFullName, command);
                    var strct = string.IsNullOrEmpty(hint.HintStructInfo) ? string.Empty : $" {hint.HintStructInfo}";
                    var desc = string.IsNullOrEmpty(hint.HintDescription) ? string.Empty : $" - {hint.HintDescription}";
                    toDraw = $"{hint.HintPrefix} {toDraw}{strct}{desc}";

                    hint.SetInterface(toDraw);
                    hintsDrawn.Add(new Tuple<JEMConsoleHint, int>(hint, hint.HintFullName.Length - command.Length));
                }
                
                hint.gameObject.SetActive(contains);
            }

            // Sort hints by remaining string that has been not highlighted.
            hintsDrawn = hintsDrawn.OrderBy(c => c.Item2).ToList();

            // Set the first hint drawn
            FirstHintDrawn = hintsDrawn.Count == 0 ? null : hintsDrawn[0].Item1;

            // Update the hints order.
            foreach (var hint in hintsDrawn)
                hint.Item1.transform.SetAsLastSibling();

            // Activate hint content if any of hints can be drawn.
            ConsoleHints.SetActive(hintsDrawn.Count != 0);
        }

        /// <summary>
        ///     Disable all the hints.
        /// </summary>
        private void DisableHints()
        {
            foreach (var hint in Hints)
            {
                hint.gameObject.SetActive(false);
            }
        }

        /// <summary>
        ///     Highlights target string in given <paramref name="str"/>.
        /// </summary>
        private static string ApplyTextHighlight(string str, string highlight)
        {
            var textToDraw = str;
            var start = textToDraw.IndexOf(highlight, StringComparison.Ordinal);
            textToDraw = textToDraw.Insert(start, "<color=green>");
            var end = start + "<color=green>".Length + highlight.Length;
            textToDraw = textToDraw.Insert(end, "</color>");
            return textToDraw;
        }
    }
}
