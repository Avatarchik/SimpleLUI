//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Globalization;
using System.Linq;
using UnityEngine;

namespace SimpleLUI.Editor.Util
{
    public static class SLUILuaBuilderSyntax
    {
        public static string FixVarName(string str)
        {
            char[] banned =
            {
                ' ', '(', ')', '[', ']', '"', '"',
                "'"[0], '<', '>', ',', '.', '?', '/',
                '!', '-', '+', '='
            };

            return banned.Aggregate(str, (current, c) => current.Replace(c, '_'));
        }

        public static string CollectVar(RectTransform r)
        {
            return FixVarName($"obj{r.gameObject.GetInstanceID()}");
        }

        public static string CollectVar(Component c)
        {
            return FixVarName($"obj{c.gameObject.GetInstanceID()}_{c.GetType().Name.ToLower()}");
        }

        public static string CollectQuaternion(Quaternion q, bool simple = false)
        {
            var str = $"{q.x.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{q.y.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{q.z.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{q.w.ToString(CultureInfo.InvariantCulture)}";
            if (!simple)
            {
                str = $"SLUIQuaternion({str})";
            }
            return str;
        }

        public static string CollectVector2(Vector2 v, bool simple = false)
        {
            var str = $"{v.x.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{v.y.ToString(CultureInfo.InvariantCulture)}";
            if (!simple)
            {
                str = $"SLUIVector2({str})";
            }
            return str;
        }

        public static string CollectColor(Color c, bool simple = false)
        {
            var str = $"{c.r.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{c.g.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{c.b.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{c.a.ToString(CultureInfo.InvariantCulture)}";
            if (!simple)
            {
                str = $"SLUIColor({str})";
            }
            return str;
        }
    }
}
