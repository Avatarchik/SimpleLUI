//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Globalization;
using System.Reflection;

namespace JEM.Core.Debugging.CVar
{
    /// <summary>
    ///     CVar data class.
    /// </summary>
    public class JEMCVar
    {
        /// <summary>
        ///     Name of CVar.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Description of Car.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     When true, only server peer could made changes to this cVar.
        ///     NOTE: All client peers will be synchronized with server changes.
        /// </summary>
        public bool IsNetworkVar { get; set; }

        /// <summary>
        ///     Raw Data of CVar.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        ///     Raw Type of CVar.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        ///     Fixed type of CVar.
        /// </summary>
        public JEMCVarType FixedDataType { get; set; }

        /// <summary>
        ///     Property from this CVar has been generated.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        ///     Gets string from current data.
        /// </summary>
        public string GetString()
        {
            switch (FixedDataType)
            {
                case JEMCVarType.String:
                    return Convert.ToString(Data);
                case JEMCVarType.Int32:
                    return Convert.ToInt32(Data).ToString();
                case JEMCVarType.UInt32:
                    return Convert.ToUInt32(Data).ToString();
                case JEMCVarType.Int64:
                    return Convert.ToInt64(Data).ToString();
                case JEMCVarType.Single:
                    return Convert.ToSingle(Data).ToString(CultureInfo.InvariantCulture);
                case JEMCVarType.Double:
                    return Convert.ToDouble(Data).ToString(CultureInfo.InvariantCulture);
                case JEMCVarType.Boolean:
                    return Convert.ToBoolean(Data).ToString();
                case JEMCVarType.Unknown:
                    throw new NullReferenceException("CVar data type is Unknown!");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Changes the data of CVar.
        /// </summary>
        internal void Change(object data, bool silently = false, bool log = true)
        {
            if (data.GetType() != DataType)
            {
                JEMLogger.LogError($"invalid data type for {Name} {Data.GetType()} != {DataType}", "CVAR");
                return;
            }

            Data = data;
            // Debug.Log($"change {Name} -> " + GetString());
            Property?.SetValue(null, Data);
            CheckForPropertyChanges();
            if (silently)
            {
                return;
            }

            if (log)
            {
                JEMLogger.Log($"{Name} updated to '{GetString()}'", "CVAR");
            }

            InvokeChangeEvent();
        } 

        /// <summary>
        ///     Checks if any changes has been made to this CVar.
        /// </summary>
        internal bool CheckForPropertyChanges()
        {
            if (Property == null)
                return false;

            var propertyValue = Property.GetValue(null);
            var change = !Data.Equals(propertyValue);
            Data = propertyValue;
            return change;
        }

        /// <summary>
        ///     Invokes OnValueChange event for this CVar.
        /// </summary>
        internal void InvokeChangeEvent()
        {
            if (JEMCVarManager.OnValueChangeEvents.ContainsKey(Name))
            {
                JEMCVarManager.OnValueChangeEvents[Name]?.Invoke(Data);
            }
        }

        /// <summary>
        ///     Checks if typeA can be converted to typeB.
        /// </summary>
        internal static bool CanConvertTypes(JEMCVarType typeA, JEMCVarType typeB)
        {
            switch (typeA)
            {
                case JEMCVarType.Unknown:
                {
                    return typeB == JEMCVarType.Unknown;
                }
                case JEMCVarType.String:
                {
                    return typeB == JEMCVarType.String;
                }
                case JEMCVarType.Int32:
                {
                    return typeB == JEMCVarType.Int32 || typeB == JEMCVarType.Int64;
                }
                case JEMCVarType.UInt32:
                {
                    return typeB == JEMCVarType.UInt32 || typeB == JEMCVarType.Int64;
                }
                case JEMCVarType.Int64:
                {
                    return typeB == JEMCVarType.Int64 || typeB == JEMCVarType.Int32;
                }
                case JEMCVarType.Single:
                {
                    return typeB == JEMCVarType.Single || typeB == JEMCVarType.Double;
                }
                case JEMCVarType.Double:
                {
                    return typeB == JEMCVarType.Double || typeB == JEMCVarType.Single;
                }
                case JEMCVarType.Boolean:
                {
                    return typeB == JEMCVarType.Boolean;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeA), typeA, null);
            }
        }
    }
}
