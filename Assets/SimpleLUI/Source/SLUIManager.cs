//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SimpleLUI
{
    /// <summary>
    ///     Main SLUI manager class.
    /// </summary>
    public class SLUIManager
    {
        /// <summary>
        ///     Root canvas of this SLUI manager instance.
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        ///     Name of the SLUI manager.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     List of all lua files added to manager.
        /// </summary>
        public IReadOnlyList<string> LuaFiles => _luaFiles;
        private readonly List<string> _luaFiles = new List<string>();
        private readonly List<string> _workingFiles = new List<string>();

        private SLUIManager() { }

        /// <summary>
        ///     Adds list of lua files.
        /// </summary>
        public void AddFiles([NotNull] IEnumerable<string> luaFiles)
        {
            if (luaFiles == null) throw new ArgumentNullException(nameof(luaFiles));
            foreach (var l in luaFiles)
            {
                AddFile(l);
            }
        }

        /// <summary>
        ///     Adds new lua file to load and work with.
        /// </summary>
        public void AddFile([NotNull] string luaFile)
        {
            if (luaFile == null) throw new ArgumentNullException(nameof(luaFile));
            if (!luaFile.EndsWith(".lua"))
                throw new FileLoadException("Invalid file extension.", luaFile);
            if (!File.Exists(luaFile))
                throw new FileNotFoundException(null, luaFile);
            if (_luaFiles.Contains(luaFile))
                return;

            _luaFiles.Add(luaFile);
        }

        /// <summary>
        ///     Reloads the manager.
        /// </summary>
        public void Reload()
        {
            if (_luaFiles.Count == 0)
            {
                Debug.LogError($"Unable to reload SLUI ({Name}). No lua files has been added.");
                return;
            }

            _workingFiles.Clear();
            using (var state = new Lua())
            {
                foreach (var f in _luaFiles)
                {
                    try
                    {
                        state.DoFile(f);
                        _workingFiles.Add(f);
                    }
                    catch (Exception e)
                    {
                        //if (e is LuaScriptException l)
                        //    Debug.LogException(l);
                        //else
                        Debug.LogException(e);              
                    }
                }
            }

            Debug.Log($"SLUI ({Name}) reloaded {_workingFiles.Count} files.");
        }

        /// <summary>
        ///     Creates new SLUI manager with given canvas as a root.
        /// </summary>
        public static SLUIManager CreateNew([NotNull] string name, [NotNull] Canvas root)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (root == null) throw new ArgumentNullException(nameof(root));
            var instance = new SLUIManager
            {
                Name = name,
                Canvas = root
            };

            return instance;
        }
    }
}
