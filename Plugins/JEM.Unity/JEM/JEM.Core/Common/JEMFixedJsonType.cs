//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Core.Debugging;

namespace JEM.Core.Common
{
    /// <summary>
    ///     Small utility class that trays to properly parse object to given Type.
    /// </summary>
    public static class JEMFixedJsonType
    {
        /// <summary>
        ///     Try parse object Type to T.
        /// </summary>
        public static T Fix<T>(object obj)
        {
            Fix(obj, out T type);
            return type;
        }

        /// <summary>
        ///     Try parse object Type to T.
        /// </summary>
        public static bool Fix<T>(object obj, out T type)
        {
            type = default(T);
            if (obj.GetType() == typeof(T))
            {
                type = (T) obj;
                return true;
            }

            bool y = true;
            Type t = typeof(T);
            if (typeof(T) == typeof(object))
                t = typeof(T);

            if (typeof(T) == typeof(int))
            {
                type = (T) (object) Convert.ToInt32(obj);
            }
            else if (typeof(T) == typeof(float))
            {
                type = (T) (object) Convert.ToSingle(obj);
            }
            else if (typeof(T) == typeof(uint))
            {
                type = (T) (object) Convert.ToUInt32(obj);
            }
            else if (typeof(T) == typeof(double))
            {
                type = (T) (object) Convert.ToDouble(obj);
            }
            else if (typeof(T) == typeof(string))
            {
                type = (T) (object) Convert.ToString(obj);
            }
            else if (typeof(T) == typeof(object))
            {
                // target type is object
                // all we can is just skip..
                type = (T) obj;
            }
            else
            {
                y = false;
            }

            if (!y)
            {
                JEMLogger.LogError("Specified cast is not valid. " +
                                   $"Resolved object of key have different type than given one {obj.GetType()} != {typeof(T)} ({type?.GetType().ToString() ?? "null"})");
            }

            return y;
        }
    }
}
