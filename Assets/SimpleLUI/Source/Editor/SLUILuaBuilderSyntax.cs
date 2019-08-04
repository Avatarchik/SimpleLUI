//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Linq;

namespace SimpleLUI.Editor
{
    internal static class SLUILuaBuilderSyntax
    {
        internal static string FixVarName(string str)
        {
            char[] banned =
            {
                ' ', '(', ')', '[', ']', '"', '"',
                "'"[0], '<', '>', ',', '.', '?', '/',
                '!', '-', '+', '='
            };

            return banned.Aggregate(str, (current, c) => current.Replace(c, '_'));
        }
    }
}
