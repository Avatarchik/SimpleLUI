//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Core
{
    /// <summary>
    ///     Contains some basic methods and properties related to windows program.
    /// </summary>
    public static class JEMProgram
    {
        /// <summary>
        ///     True, if the <see cref="JEMProgram"/> is active on linux.
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                var p = (int) Environment.OSVersion.Platform;
                return p == 4 || p == 6 || p == 128;
            }
        }

        /// <summary>
        ///     Returns a directory separator character for active os (Windows or Linux).
        /// </summary>
        public static char DirectorySeparator => IsLinux ? '/' : '\\';

        /// <summary>
        ///     Try to parse application version string in to int32 value.
        /// </summary>
        /// <remarks>
        ///     Removes all '.' and parses in to int32.
        /// </remarks>
        public static int ParseVersionString(string str)
        {
            if (str == null) return 0;
            str = string.Join("", str.Split('.'));
            if (int.TryParse(str, out var ver))
                return ver;

            throw new NullReferenceException($"Failed to parse version string to number. ({str})");
        }

        /// <summary>
        ///     Checks if the <see cref="Environment.GetCommandLineArgs"/> contains a argument of given name.
        /// </summary>
        public static bool CheckForArgument(string argument) =>
            CheckForArgument(Environment.GetCommandLineArgs(), argument, false, out _);

        /// <summary>
        ///     Checks if the <see cref="Environment.GetCommandLineArgs"/> contains a argument of given name.
        ///     Method will also try to extract a value of this argument.
        /// </summary>
        public static bool CheckForArgument(string argument, out string value) =>
            CheckForArgument(Environment.GetCommandLineArgs(), argument, true, out value);

        /// <summary>
        ///     Checks if the <paramref name="args"/> contains a argument of given name.
        /// </summary>
        public static bool CheckForArgument(string[] args, string argument) =>
            CheckForArgument(args, argument, false, out _);

        /// <summary>
        ///     Checks if the <paramref name="args"/> contains a argument of given name.
        ///     Method will also try to extract a value of this argument.
        /// </summary>
        public static bool CheckForArgument(string[] args, string argument, out string value) =>
            CheckForArgument(args, argument, true, out value);

        /// <summary>
        ///     Checks if the <paramref name="args"/> contains a argument of given name.
        ///     If you set <paramref name="haveValue"/> to true, method will try to extract a value of this argument.
        /// </summary>
        public static bool CheckForArgument(string[] args, string argument, bool haveValue, out string value)
        {
            value = string.Empty;
            for (var index = 0; index < args.Length; index++)
            {
                if (args[index] != argument) continue;
                if (haveValue)
                {
                    if (index + 1 > args.Length) continue;
                    value = args[index + 1];

                    if (value.StartsWith('"'.ToString())) value = value.Remove(0);
                    if (value.EndsWith('"'.ToString())) value = value.Remove(value.Length - 1);
                    return true;
                }

                value = string.Empty;
                return true;
            }

            return false;
        }
    }
}