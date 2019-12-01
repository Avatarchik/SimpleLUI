//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JEM.Core.Debugging.CVar
{
    /// <summary>
    ///     CVars manager class.
    /// </summary>
    public static class JEMCVarManager
    {
        /// <summary>
        ///     Update CVarManager to check if any changes to properties has been made.
        /// </summary>
        public static void Update()
        {    
            var changed = new List<JEMCVar>();
            foreach (var v in RegisteredVars)
            {
                if (v.CheckForPropertyChanges())
                {
                    changed.Add(v);
                }
            }

            foreach (var c in changed)
                c.InvokeChangeEvent();
        }

        /// <summary>
        ///     Registers new var.
        /// </summary>
        public static void RegisterCVar(string name, object @default, Action<object> onValueChanged = null, bool invokeAtStart = true)
        {
            RegisterCVar(name, @default, isNetworkVar: false, onValueChanged: onValueChanged, invokeAtStart: invokeAtStart);
        }

        /// <summary>
        ///     Registers new CVar.
        /// </summary>
        public static bool RegisterCVar(string name, object @default, string description = "No description.", bool isNetworkVar = false, Action<object> onValueChanged = null, bool invokeAtStart = true)
        {
            var cVarSettings = new JEMCVarSettings<object>(name, @default, description, isNetworkVar, onValueChanged)
            {
                InvokeAtStart = invokeAtStart
            };
            return RegisterCVar(cVarSettings);
        }

        /// <summary>
        ///     Registers new CVar.
        /// </summary>
        public static bool RegisterCVar<T>(string name, T @default, string description = "No description.", bool isNetworkVar = false, Action<T> onValueChanged = null, bool invokeAtStart = true)
        {
            var cVarSettings = new JEMCVarSettings<T>(name, @default, description, isNetworkVar, onValueChanged)
            {
                InvokeAtStart = invokeAtStart
            };
            return RegisterCVar(cVarSettings);
        }

        /// <summary>
        ///     Registers new CVar by given settings.
        /// </summary>
        public static bool RegisterCVar(JEMCVarSettings<object> settings)
        {
            return RegisterCVar<object>(settings);
        }

        /// <summary>
        ///     Registers new CVar by given settings.
        /// </summary>
        public static bool RegisterCVar<T>(JEMCVarSettings<T> settings)
        {
            if (settings.Default == null) throw new ArgumentNullException(nameof(settings.Default));

            // check if target CVar name is not in use
            if (GetCVarData(settings.Name) != null)
                return false; // name in use

            // collect CVar type
            var dataType = ResolveFixedDataType(settings.Default.GetType());
            if (dataType == JEMCVarType.Unknown)
                return false; // invalid type

            // try to load saved data instead of applying default
            var targetData = settings.Default;
            var saved = _cVarSave?.Get(settings.Name);
            if (saved != null)
            {
                var savedDataType = ResolveFixedDataType(saved.GetType());
                if (savedDataType != dataType && !JEMCVar.CanConvertTypes(savedDataType, dataType))
                {
                    // error
                    // types are not the same.
#if DEBUG
                    JEMLogger.Log($"savedDataType({savedDataType}) != dataType({dataType})", "CVAR");
#endif
                    return false;
                }

                targetData = FixSavedType(dataType, saved);
            }

            var newCVarData = new JEMCVar
            {
                Name = settings.Name,
                Data = targetData,
                DataType = settings.Default.GetType(),
                FixedDataType = dataType,
                Description = settings.Description,
                IsNetworkVar = settings.IsNetworkVar,
                Property = settings.Property
            };

            RegisteredVars.Add(newCVarData);

            if (settings.OnValueChanged != null)
            {
                RegisterCVarChange(settings.Name, settings.OnValueChanged, false);
            }

            if (settings.InvokeAtStart)
            {
                if (OnValueChangeEvents.ContainsKey(settings.Name))
                    OnValueChangeEvents[settings.Name]?.Invoke(targetData);
            }

            return true;
        }

        /// <summary>
        ///     Register CVar change event.
        /// </summary>
        public static Action<object> RegisterCVarChange<T>(string key, Action<T> onValueChanged, bool invokeAtStart = true)
        {
            if (onValueChanged == null) throw new ArgumentNullException(nameof(onValueChanged));
            if (!OnValueChangeEvents.ContainsKey(key))
            {
                void New(object obj) { }
                OnValueChangeEvents.Add(key, New);
            }

            var a = new Action<object>(obj => { onValueChanged.Invoke((T)obj); });
            OnValueChangeEvents[key] += a;
            if (invokeAtStart)
            {
                var varData = GetCVarData(key);
                if (varData != null)
                {
                    onValueChanged((T)varData.Data);
                }
            }

            return a;
        }

        /// <summary>
        ///     Register CVar change event.
        /// </summary>
        public static Action<object> RegisterCVarChange(string key, Action<object> onValueChanged, bool invokeAtStart = true)
        {
            if (onValueChanged == null) throw new ArgumentNullException(nameof(onValueChanged));
            if (!OnValueChangeEvents.ContainsKey(key))
            {
                void New(object obj) { }
                OnValueChangeEvents.Add(key, New);
            }

            var a = new Action<object>(onValueChanged.Invoke);
            OnValueChangeEvents[key] += a;
            if (invokeAtStart)
            {
                var varData = GetCVarData(key);
                if (varData != null)
                {
                    onValueChanged(varData.Data);
                }
            }

            return a;
        }

        /// <summary>
        ///     Edits var at given key by setting new data.
        /// </summary>
        public static string EditCVar(string name, string data, bool silently = false, bool log = true)
        {
            var var = GetCVarData(name);
            if (var == null)
            {
                return $"cvar \'{name}\' not exist";
            }

            object fixedData;
            switch (var.FixedDataType)
            {
                case JEMCVarType.String:
                    // string does not need any type check
                    fixedData = data;
                    break;
                case JEMCVarType.Int32:
                    if (int.TryParse(data, out var resultInt))
                        fixedData = resultInt;
                    else return "invalid parameter type were given";
                    break;
                case JEMCVarType.UInt32:
                    if (uint.TryParse(data, out var resultUInt))
                        fixedData = resultUInt;
                    else return "invalid parameter type were given";
                    break;
                case JEMCVarType.Int64:
                    if (long.TryParse(data, out var resultLong))
                        fixedData = resultLong;
                    else return "invalid parameter type were given";
                    break;
                case JEMCVarType.Single:
                    data = data.Replace('.', ',');
                    if (float.TryParse(data, out var resultSingle))
                        fixedData = resultSingle;
                    else return "invalid parameter type were given";
                    break;
                case JEMCVarType.Double:
                    data = data.Replace('.', ',');
                    if (double.TryParse(data, out var resultDouble))
                        fixedData = resultDouble;
                    else return "invalid parameter type were given";
                    break;
                case JEMCVarType.Boolean:
                    if (bool.TryParse(data, out var resultBoolean))
                        fixedData = resultBoolean;
                    else if (int.TryParse(data, out var resultBoolInt))
                        fixedData = resultBoolInt != 0;

                    else return "invalid parameter type were given";
                    break;
                default:
                    return "command target method has invalid type in parameters!";
            }

            var.Change(fixedData, silently, log);
            return string.Empty;
        }

        /// <summary>
        ///     Edits var at given key by setting new data.
        /// </summary>
        public static string EditCVar(string name, object data, bool silently = false, bool log = true)
        {
            var var = GetCVarData(name);
            if (var == null)
            {
                return $"cvar \'{name}\' not exist";
            }

            object fixedData;
            switch (var.FixedDataType)
            {
                case JEMCVarType.String:
                    fixedData = (string) data;
                    break;
                case JEMCVarType.Int32:
                    fixedData = (int) data;
                    break;
                case JEMCVarType.UInt32:
                    fixedData = (uint) data;
                    break;
                case JEMCVarType.Int64:
                    fixedData = Convert.ToInt64(data);
                    break;
                case JEMCVarType.Single:
                    fixedData = (float) data;
                    break;
                case JEMCVarType.Double:
                    fixedData = Convert.ToDouble(data);
                    break;
                case JEMCVarType.Boolean:
                    fixedData = (bool) data;
                    break;
                default:
                    return "command target method has invalid type in parameters!";
            }

            var.Change(fixedData, silently, log);
            return string.Empty;
        }

        /// <summary>
        ///     Unregister CVar of given Name.
        /// </summary>
        public static void UnregisterCVar(string name)
        {
            var varData = GetCVarData(name);
            if (varData == null)
                return;

            RegisteredVars.Remove(varData);
            if (OnValueChangeEvents.ContainsKey(name))
            {
                OnValueChangeEvents.Remove(name);
            }
        }

        /// <summary>
        ///     UnRegister CVar change event.
        /// </summary>
        public static void UnregisterCVarChange(string key, Action<object> onValueChanged)
        {
            if (onValueChanged == null) throw new ArgumentNullException(nameof(onValueChanged));
            if (OnValueChangeEvents.ContainsKey(key))
            {
                OnValueChangeEvents[key] -= onValueChanged;
            }
        }
       
        /// <summary>
        ///     Collects all fields with CVarAttribute.
        /// </summary>
        public static void CollectAllCVars()
        {
            var c = 0;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var allTypes = assembly.GetTypes();
                foreach (var t in allTypes)
                {
                    try
                    {
                        if (!t.IsClass)
                            continue;

                        var allProperties =
                            new List<PropertyInfo>(t.GetProperties(BindingFlags.NonPublic | BindingFlags.Static));
                        var allPublicProperties = t.GetProperties(BindingFlags.Public | BindingFlags.Static);
                        allProperties.AddRange(allPublicProperties.Where(IsPropertyStatic));
                        foreach (var p in allProperties)
                        {
                            var attributes = p.GetCustomAttributes(typeof(JEMCVarAttribute), false);
                            if (attributes.Length <= 0) continue;
                            var attribute = (JEMCVarAttribute) attributes[0];

                            var cVarSettings = new JEMCVarSettings<object>(attribute.Name, p.GetValue(null))
                            {
                                Description = attribute.Description,
                                IsNetworkVar = attribute.IsNetworkVar,
                                Property = p
                            };

                            var success = RegisterCVar(cVarSettings);
                            if (success)
                            {
                                p.SetValue(null, GetCVarData(cVarSettings.Name).Data);
                                c++;
                            }
                            else
                            {
                                JEMLogger.LogError($"Failed to register CVar {attribute.Name} ({t.FullName}.{p.Name})", "CVAR");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        JEMLogger.LogException($"Failed to register cVar. {e}", e.StackTrace, "CVAR");
                    }
                }
            }

            JEMLogger.Log($"{c} cVars has been registered.", "CVAR");
        }

        /// <summary>
        ///     Loads CVar save file.
        /// </summary>
        public static void LoadFile()
        {
            _cVarSave = JEMConfiguration.LoadData<JEMCVarSave>(JEMConfiguration.ResolveFilePath("cvar"),
                JEMConfigurationSaveMethod.JSON);
        }

        /// <summary>
        ///     Save CVar file.
        /// </summary>
        public static void SaveFile()
        {
            _cVarSave.Items.Clear();
            foreach (var r in RegisteredVars)
            {
                if (r.Name.StartsWith("cfg."))
                    continue;// exclude configuration cvars
                _cVarSave.Items.Add(new JEMCVarSave.Item { Key = r.Name, Object = r.Data });
            }

            JEMConfiguration.WriteData(JEMConfiguration.ResolveFilePath("cvar"), _cVarSave, JEMConfigurationSaveMethod.JSON);
        }

        //internal static string GetNameFromProperty()
        //{

        //}

        /// <summary>
        ///     Gets var by key data.
        /// </summary>
        public static JEMCVar GetCVarData(string name)
        {
            foreach (var r in RegisteredVars)
            {
                if (r.Name == name)
                    return r;
            }

            return null;
        }

        /// <summary>
        ///     Resolves CVarData type of given Type.
        /// </summary>
        public static JEMCVarType ResolveFixedDataType(Type t)
        {
            switch (t.Name.ToLower())
            {
                case "string":
                    return JEMCVarType.String;
                case "int32":
                    return JEMCVarType.Int32;
                case "uint32":
                    return JEMCVarType.UInt32;
                case "int64":
                    return JEMCVarType.Int64;
                case "single":
                    return JEMCVarType.Single;
                case "double":
                    return JEMCVarType.Double;
                case "boolean":
                    return JEMCVarType.Boolean;
                default:
                    JEMLogger.LogError($"Not supported CVar type were given ({t.Name.ToLower()})", "CVAR");
                    return JEMCVarType.Unknown;
            }
        }

        private static object FixSavedType(JEMCVarType type, object obj)
        {
            switch (type)
            {
                case JEMCVarType.Unknown:
                    throw new NullReferenceException("Unknown cVar data type has been received.");
                case JEMCVarType.String:
                    return Convert.ToString(obj);
                case JEMCVarType.Int32:
                    return Convert.ToInt32(obj);
                case JEMCVarType.UInt32:
                    return Convert.ToUInt32(obj);
                case JEMCVarType.Int64:
                    return Convert.ToInt64(obj);
                case JEMCVarType.Single:
                    return Convert.ToSingle(obj);
                case JEMCVarType.Double:
                    return Convert.ToDouble(obj);
                case JEMCVarType.Boolean:
                    return Convert.ToBoolean(obj);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static bool IsPropertyStatic(PropertyInfo p)
        {
            var setter = p.GetSetMethod();
            var getter = p.GetGetMethod();
            if (setter == null && getter == null)
                return false; // lul
            return (setter?.IsStatic ?? false) || (getter?.IsStatic ?? false);
        }

        private static JEMCVarSave _cVarSave;
        internal static Dictionary<string, Action<object>> OnValueChangeEvents { get; } = new Dictionary<string, Action<object>>();

        /// <summary>
        ///     List of registered console vars.
        /// </summary>
        public static List<JEMCVar> RegisteredVars { get; } = new List<JEMCVar>();
    }
}
