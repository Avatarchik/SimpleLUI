//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#pragma warning disable 168

namespace JEM.Core.Common
{
    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodS
    {
        private readonly Action _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodS(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Action) Delegate.CreateDelegate(typeof(Action), classObj, method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method.
        /// </summary>
        public void Invoke()
        {
            try
            {
                _delegate?.Invoke();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodS<TParam1>
    {
        private readonly Action<TParam1> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodS(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Action<TParam1>) Delegate.CreateDelegate(typeof(Action<TParam1>), classObj,
                    method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public void Invoke(TParam1 param1)
        {
            try
            {
                _delegate?.Invoke(param1);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodS<TParam1, TParam2>
    {
        private readonly Action<TParam1, TParam2> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodS(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Action<TParam1, TParam2>) Delegate.CreateDelegate(typeof(Action<TParam1, TParam2>),
                    classObj, method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public void Invoke(TParam1 param1, TParam2 param2)
        {
            try
            {
                _delegate?.Invoke(param1, param2);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodS<TParam1, TParam2, TParam3>
    {
        private readonly Action<TParam1, TParam2, TParam3> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodS(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Action<TParam1, TParam2, TParam3>) Delegate.CreateDelegate(
                    typeof(Action<TParam1, TParam2, TParam3>), classObj, method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            try
            {
                _delegate?.Invoke(param1, param2, param3);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodS<TParam1, TParam2, TParam3, TParam4>
    {
        private readonly Action<TParam1, TParam2, TParam3, TParam4> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodS(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Action<TParam1, TParam2, TParam3, TParam4>) Delegate.CreateDelegate(
                    typeof(Action<TParam1, TParam2, TParam3, TParam4>), classObj, method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            try
            {
                _delegate?.Invoke(param1, param2, param3, param4);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodR<TResult, TParam1>
    {
        private readonly Func<TParam1, TResult> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodR(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Func<TParam1, TResult>) Delegate.CreateDelegate(typeof(Func<TParam1, TResult>), classObj,
                    method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public TResult Invoke(TParam1 param1)
        {
            try
            {
                return _delegate == null ? default(TResult) : _delegate.Invoke(param1);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodR<TResult, TParam1, TParam2>
    {
        private readonly Func<TParam1, TParam2, TResult> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodR(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Func<TParam1, TParam2, TResult>) Delegate.CreateDelegate(
                    typeof(Func<TParam1, TParam2, TResult>), classObj, method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public TResult Invoke(TParam1 param1, TParam2 param2)
        {
            try
            {
                return _delegate == null ? default(TResult) : _delegate.Invoke(param1, param2);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodR<TResult, TParam1, TParam2, TParam3>
    {
        private readonly Func<TParam1, TParam2, TParam3, TResult> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodR(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Func<TParam1, TParam2, TParam3, TResult>) Delegate.CreateDelegate(
                    typeof(Func<TParam1, TParam2, TParam3, TResult>), classObj, method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            try
            {
                return _delegate == null ? default(TResult) : _delegate.Invoke(param1, param2, param3);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethodR<TResult, TParam1, TParam2, TParam3, TParam4>
    {
        private readonly Func<TParam1, TParam2, TParam3, TParam4, TResult> _delegate;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethodR(object classObj, string methodName, bool lookBaseType = true)
        {
            if (classObj == null) throw new ArgumentNullException(nameof(classObj));
            var method = new JEMSmartMethod(classObj, methodName, lookBaseType);
            if (method.IsValid())
                _delegate = (Func<TParam1, TParam2, TParam3, TParam4, TResult>) Delegate.CreateDelegate(
                    typeof(Func<TParam1, TParam2, TParam3, TParam4, TResult>), classObj, method.MethodInfo);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => _delegate != null;

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            try
            {
                return _delegate == null ? default(TResult) : _delegate.Invoke(param1, param2, param3, param4);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     JEM Smart Method.
    ///     Simple class that allows to invoke method of given name.
    ///     If the method not exist, execution will be ignored without any errors.
    /// </summary>
    public class JEMSmartMethod
    {
        internal readonly object ClassObj;
        internal readonly MethodInfo MethodInfo;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethod(Type classType, string methodName)
        {
            if (classType == null) throw new ArgumentNullException(nameof(classType));

            // Extract method.
            MethodInfo = classType.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMSmartMethod(object classObj, string methodName, bool lookBaseType = true)
        {
            ClassObj = classObj ?? throw new ArgumentNullException(nameof(classObj));

            // Extract method.
            MethodInfo = ExtractMethod(classObj.GetType(), methodName, lookBaseType);
        }

        /// <summary>
        ///     True, if there is a specified method.
        /// </summary>
        public bool IsValid() => MethodInfo != null;

        /// <summary>
        ///     Try to invoke target method.
        /// </summary>
        public object Invoke() => Invoke(null);

        /// <summary>
        ///     Try to invoke target method with parameters.
        /// </summary>
        public object Invoke(params object[] parameters)
        {
            try
            {
                return MethodInfo?.Invoke(ClassObj, parameters);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        ///// <summary>
        /////     Creates delegate for target method.
        ///// </summary>
        ///// <exception cref="ArgumentNullException"/>
        //public static Delegate CreateDelegate(object instance, MethodInfo method)
        //{
        //    if (instance == null) throw new ArgumentNullException(nameof(instance));
        //    if (method == null) throw new ArgumentNullException(nameof(method));
        //    var parameters = method.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name))
        //        .ToArray();
        //    var call = Expression.Call(Expression.Constant(instance), method, parameters);
        //    return Expression.Lambda(call, parameters).Compile();
        //}

        /// <summary>
        ///     Extracts all methods from given type (including <see cref="Type.BaseType"/>).
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static IList<MethodInfo> ExtractAllMethods(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            // Check cache.
            for (var index = 0; index < AllMethodsCache.Count; index++)
            {
                var c = AllMethodsCache[index];
                if (c.Item1 == type)
                    return c.Item2;
            }

            // Generate new list.
            var originalType = type;
            var allMethods = new List<MethodInfo>();
            while (true)
            {
                var typeMethods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                for (var index = 0; index < typeMethods.Length; index++)
                {
                    var method = typeMethods[index];
                    bool isMethodInUse = allMethods.Any(usedMethod => usedMethod.Name == method.Name);
                    if (!isMethodInUse)
                    {
                        allMethods.Add(method);
                    }
                }

                type = type.BaseType;
                if (type == null)
                {
                    AllMethodsCache.Add(new Tuple<Type, IList<MethodInfo>>(originalType, allMethods));
                    return allMethods;
                }
            }
        }

        /// <summary>
        ///     Extract a method of given name from given type (including <see cref="Type.BaseType"/>).
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static MethodInfo ExtractMethod(Type type, string methodName, bool lookBaseType = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            // Check cache.
            var fullName = type.FullName + "." + methodName;
            for (var index = 0; index < MethodsCache.Count; index++)
            {
                var m = MethodsCache[index];
                if (m.Item2 == fullName && m.Item3 == lookBaseType)
                    return m.Item1;
            }

            // Generate method.
            MethodInfo methodInfo = null;
            while (methodInfo == null)
            {
                methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (!lookBaseType)
                    break;

                if (methodInfo == null)
                {
                    type = type.BaseType;
                    if (type == null)
                        break;
                }
            }

            // Cache method.
            MethodsCache.Add(new Tuple<MethodInfo, string, bool>(methodInfo, fullName, lookBaseType));

            return methodInfo;
        }

        private static List<Tuple<MethodInfo, string, bool>> MethodsCache { get; } = new List<Tuple<MethodInfo, string, bool>>();
        private static List<Tuple<Type, IList<MethodInfo>>> AllMethodsCache { get; } = new List<Tuple<Type, IList<MethodInfo>>>();
    }
}
