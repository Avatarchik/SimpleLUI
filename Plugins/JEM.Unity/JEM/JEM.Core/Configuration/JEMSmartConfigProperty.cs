//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace JEM.Core.Configuration
{
    /// <summary>
    ///     JEM Smart Configuration Property.
    ///     A instance of property loaded by Smart Configuration.
    /// </summary>
    internal class JEMSmartConfigProperty
    {
        internal PropertyInfo Property;
        internal JEMSmartConfigVarAttribute Attribute;

        internal object Data;

        internal JEMSmartMethod ChangeMethod;

        internal bool ShouldFixProperty = true;

        /// <summary>
        ///     Checks if any changes has been made to this Property.
        /// </summary>
        internal bool CheckForPropertyChanges()
        {
            var propertyValue = Property.GetValue(null);
            var change = !Data?.Equals(propertyValue) ?? true;
            Data = propertyValue;
            return change;
        }

        internal void ApplyFixedPropertyChange(object data)
        {
            if (!ShouldFixProperty)
                return;

            Data = data;
            SetPropertyValue(data);
        }

        internal void LookForChangeEvent()
        {
            if (string.IsNullOrEmpty(Attribute.OnValueChanged))
            {
                ChangeMethod = null;
            } else ChangeMethod = new JEMSmartMethod(Property.DeclaringType, Attribute.OnValueChanged);
        }

        internal void SetPropertyValue(object obj)
        {
            var p = Property;
            var varType = p.PropertyType;
            if (varType == typeof(short))
            {
                p.SetValue(null, Convert.ToInt16(obj));
            }
            else if (varType == typeof(int))
            {
                p.SetValue(null, Convert.ToInt32(obj));
            }
            else if (varType == typeof(long))
            {
                p.SetValue(null, Convert.ToInt64(obj));
            }
            else if (varType == typeof(ushort))
            {
                p.SetValue(null, Convert.ToUInt16(obj));
            }
            else if (varType == typeof(uint))
            {
                p.SetValue(null, Convert.ToUInt32(obj));
            }
            else if (varType == typeof(ulong))
            {
                p.SetValue(null, Convert.ToUInt64(obj));
            }
            else if (varType == typeof(float))
            {
                p.SetValue(null, Convert.ToSingle(obj));
            }
            else if (varType == typeof(double))
            {
                p.SetValue(null, Convert.ToDouble(obj));
            }
            else if (varType.BaseType == typeof(Enum))
            {
                p.SetValue(null, Enum.ToObject(varType, obj));
            }
            else
            {
                // try to yolo it :D
                if (obj.GetType() == typeof(JObject))
                    p.SetValue(null, ((JObject)obj).ToObject(varType));
                else
                {
                    p.SetValue(null, obj);
                }
            }
        }
    }
}