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
    public class SLUIFile
    {
        public string File { get; }
        public DateTime LastModified { get; internal set; }

        internal SLUIFile(string file)
        {
            File = file;
        }
    }

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
        public IReadOnlyList<SLUIFile> LuaFiles => _luaFiles;
        private readonly List<SLUIFile> _luaFiles = new List<SLUIFile>();
        private readonly List<SLUIFile> _workingFiles = new List<SLUIFile>();

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
            foreach (var f in _luaFiles)
                if (f.File == luaFile)
                    return;
   
            _luaFiles.Add(new SLUIFile(luaFile)
            {
                LastModified = File.GetLastWriteTime(luaFile)
            });
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
                        state.DoFile(f.File);
                        _workingFiles.Add(f);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);              
                    }
                }
            }

            Debug.Log($"SLUI ({Name}) reloaded {_workingFiles.Count} files. ({_luaFiles.Count - _workingFiles.Count} failed)");
        }

        /// <summary>
        ///     Looks if any of the manager's files changed.
        /// </summary>
        public bool LookForChanges(bool cleanup = true)
        {
            bool hasChange = false;
            for (var index = 0; index < _luaFiles.Count; index++)
            {
                var f = _luaFiles[index];
                if (!File.Exists(f.File))
                {
                    if (cleanup)
                    {
                        Debug.Log($"SLUI ({Name}) file change detected. File '{f.File}' has been removed.");
                        _luaFiles.Remove(f);
                        index--;
                    }

                    hasChange = true;
                }
                else
                {
                    var currentWriteTime = File.GetLastWriteTime(f.File);
                    if (DateTime.Compare(f.LastModified, currentWriteTime) != 0)
                    {
                        if (cleanup)
                        {
                            Debug.Log($"SLUI ({Name}) file change detected. File '{f.File}' has been modified.");
                            f.LastModified = currentWriteTime;
                        }

                        hasChange = true;
                    }
                }
            }

            return hasChange;
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
