//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.IO;

namespace JEM.Core.Extension
{
    /// <summary>
    ///     Set of utility methods: Path
    /// </summary>
    public static class ExtensionPath
    {
        /// <summary>
        ///     Resolves relative file path.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static string ResolveRelativeFilePath(string fileDirectory) => ResolveRelativeFilePath(Environment.CurrentDirectory, fileDirectory);

        /// <summary>
        ///     Resolves relative file path.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static string ResolveRelativeFilePath(string rootDirectory, string fileDirectory)
        {
            if (rootDirectory == null) throw new ArgumentNullException(nameof(rootDirectory));
            if (fileDirectory == null) throw new ArgumentNullException(nameof(fileDirectory));
            var fileUri = new Uri(fileDirectory);
            var rootUri = new Uri(rootDirectory);
            var relativePath =
                Uri.UnescapeDataString(rootUri.MakeRelativeUri(fileUri).ToString()
                    .Replace('/', Path.DirectorySeparatorChar)).Replace(Path.DirectorySeparatorChar, '/');
            var rootName = Path.GetFileName(rootDirectory);
            return relativePath.Replace($@"{rootName}/", string.Empty);
        }
    }
}
