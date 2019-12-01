//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// #define MEMORY_DEBUG

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JEM.UnityEngine.Console
{
    [Serializable]
    public class JEMConsoleMemory
    {
        public List<string> Commands = new List<string>();
    }

    public sealed partial class JEMConsole
    {
        /// <summary>
        ///     Relative path to the console memory file without extension.
        /// </summary>
        [Header("Memory Settings")]
        public string ConsoleMemoryFile = "Config\\ConsoleMem";

        /// <summary>
        ///     If false, console will memorize every received text.
        /// </summary>
        public bool MemorizeOnlyValid = true;

        /// <summary>
        ///     Loaded <see cref="JEMConsoleMemory"/>.
        /// </summary>
        public JEMConsoleMemory LoadedMemory { get; private set; }

        /// <summary>
        ///     Reference to the commands in memory.
        /// </summary>
        public List<string> MemoryCommands => LoadedMemory.Commands;

        /// <summary>
        ///     Full path to console memory file.
        /// </summary>
        public string FullConsoleMemoryFilePath => $"{Environment.CurrentDirectory}\\{ConsoleMemoryFile}.bin";

        /// <summary>
        ///     Loads the console memory file.
        /// </summary>
        private void LoadMemoryFile()
        {
            try
            {
                var directory = Path.GetDirectoryName(FullConsoleMemoryFilePath) ?? throw new InvalidOperationException();
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(FullConsoleMemoryFilePath))
                {
                    LoadedMemory = new JEMConsoleMemory();
                }
                else
                {
                    LoadedMemory = JsonConvert.DeserializeObject<JEMConsoleMemory>(
                        File.ReadAllText(FullConsoleMemoryFilePath));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            // lol
            if (LoadedMemory == null) LoadedMemory = new JEMConsoleMemory();
            else if (LoadedMemory.Commands == null) LoadedMemory.Commands = new List<string>();     
        }

        /// <summary>
        ///     Saves the console memory file.
        /// </summary>
        public void SaveMemoryFile()
        {
            if (LoadedMemory == null)
            {
                return;
            }

            var directory = Path.GetDirectoryName(FullConsoleMemoryFilePath) ?? throw new InvalidOperationException();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(FullConsoleMemoryFilePath, JsonConvert.SerializeObject(LoadedMemory));
        }

        private int MemoryPosition { get; set; }

        /// <summary>
        ///     Moves up the position in commands memory.
        /// </summary>
        private bool MoveMemoryUp(out string str)
        {
#if DEBUG && MEMORY_DEBUG
            JEMLogger.Log($"JEMConsole.MoveMemoryUp() memory_position_{MemoryPosition}", "CONSOLE");
#endif

            str = string.Empty;
            if (MemoryCommands.Count == 0) return false;
            if (MemoryPosition >= MemoryCommands.Count)
                return false;

            if (MemoryPosition < 0)
                MemoryPosition = 0;
            else
                MemoryPosition++;

            str = MemoryCommands[MemoryPosition];   
            return true;
        }

        /// <summary>
        ///     Moves down the position in commands memory.
        /// </summary>
        private bool MoveMemoryDown(out string str)
        {
#if DEBUG && MEMORY_DEBUG
            JEMLogger.Log($"JEMConsole.MoveMemoryDown() memory_position_{MemoryPosition}", "CONSOLE");
#endif

            str = string.Empty;
            if (MemoryCommands.Count == 0) return false;
            if (MemoryPosition < 0)
            {
                MemoryPosition = 0;
                return false;
            }

            if (MemoryPosition >= MemoryCommands.Count)
            {
                MemoryPosition = MemoryCommands.Count - 1;
            }
            else
                MemoryPosition--;

            if (MemoryPosition < 0)
            {
                MemoryPosition = 0;
                return false;
            }

            str = MemoryCommands[MemoryPosition];         
            return true;
        }

        /// <summary>
        ///     Restarts the position of console memory.
        /// </summary>
        internal void RestartMemoryPosition()
        {
#if DEBUG && MEMORY_DEBUG
            JEMLogger.Log("JEMConsole.RestartMemoryPosition()", "CONSOLE");
#endif
            MemoryPosition = -1;
        }

        /// <summary>
        ///     Insert command in to memory and reset the position.
        /// </summary>
        private void InsertToMemoryAndReset(string command)
        {
#if DEBUG && MEMORY_DEBUG
            JEMLogger.Log($"JEMConsole.InsertToMemoryAndReset({command})", "CONSOLE");
#endif

            RestartMemoryPosition();
            if (MemoryCommands.Count != 0 && MemoryCommands[0] == command)
                return; // Do not insert duplicates!

            MemoryCommands.Insert(0, command);
        }
    }
}
