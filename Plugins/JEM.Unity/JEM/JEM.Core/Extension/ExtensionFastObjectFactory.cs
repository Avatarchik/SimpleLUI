//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// source: https://stackoverflow.com/questions/6582259/fast-creation-of-objects-instead-of-activator-createinstancetype/6882881?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace JEM.Core.Extension
{
    /// <summary>
    ///     JEM Fast Object Factory extension.
    ///     Allows for faster object instance creation.
    /// </summary>
    public static class FastObjectFactory<T>
    {
        /// <summary/>
        public static readonly Func<T> Instance = Creator();

        /// <summary/>
        private static Func<T> Creator()
        {
            var t = typeof(T);
            if (t == typeof(string))
                return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

            if (t.HasDefaultConstructor())
                return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

            return () => (T) FormatterServices.GetUninitializedObject(t);
        }
    }

    /// <summary/>
    public static class FastObjectFactoryUtil
    {
        /// <summary>
        ///     Check if given type has default constructor.
        /// </summary>
        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}