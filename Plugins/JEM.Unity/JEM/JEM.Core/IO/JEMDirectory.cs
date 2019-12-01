//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// NOTE: This class is Obsolete and may be removed in the future.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JEM.Core.IO
{
    /// <summary>
    ///     Directory operation class.
    /// </summary>
    public static class JEMDirectory
    {
        /// <summary>
        ///     Returns the names of files (including their paths) that math the specific directory and don't contains ignored names.
        /// </summary>
        /// <param name="path">The relative or absolute path to directory to search.</param>
        /// <param name="namesIgnored">Names of directories to ignore.</param>
        [Obsolete("Use Directory.GetFiles instead.")]
        public static string[] GetFiles(string path, string[] namesIgnored)
        {
            var files = new List<string>(Directory.GetFiles(path));
            var dirs = new List<string>(Directory.GetDirectories(path));
            while (dirs.Count != 0)
            {
                var b = namesIgnored != null && namesIgnored.Any(t => dirs[0].EndsWith(t));
                if (b)
                {
                    dirs.RemoveAt(0);
                }
                else
                {
                    dirs.AddRange(Directory.GetDirectories(dirs[0]));
                    var nextFiles = Directory.GetFiles(dirs[0]);
                    foreach (var t in nextFiles)
                    {
                        if (files.Contains(t)) continue;
                        files.Add(t);
                    }

                    dirs.RemoveAt(0);
                }
            }

            return files.ToArray();
        }
    }
}