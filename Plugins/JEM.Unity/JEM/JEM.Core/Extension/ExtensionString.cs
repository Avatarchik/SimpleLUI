//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Text.RegularExpressions;

namespace JEM.Core.Extension
{
    /// <summary>
    ///     Set of utility methods: String
    /// </summary>
    public static class ExtensionString
    {
        /// <summary>
        ///     Checks if string contains string like.
        /// </summary>
        public static bool Like(this string toSearch, string toFind)
        {
            return new Regex(
                @"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch)
                    .Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
        }
    }
}