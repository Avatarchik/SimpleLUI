//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JEM.Core.Configuration
{
    /// <summary>
    ///     JEM Smart Configuration Save.
    ///     Holds data for any Smart Configuration Class.
    /// </summary>
    [Serializable]
    public class JEMSmartConfigSave
    {
        /// <summary>
        ///     Reference to Smart Configuration Attribute this Save was loaded for.
        /// </summary>
        internal JEMSmartConfigAttribute MountedConfigurationAttribute;

        /// <summary>
        ///     Reference to the Type of class that this Save was loaded for.
        /// </summary>
        internal Type MountedType;

        /// <summary>
        ///     List of loaded properties.
        /// </summary>
        internal List<JEMSmartConfigProperty> Properties { get; } = new List<JEMSmartConfigProperty>();

        /// <summary>
        ///     A Database instance that holds all data of Smart Configuration.
        /// </summary>
        public JEMKeyBasedDatabase Database = new JEMKeyBasedDatabase();

        /// <summary>
        ///     Save the data!
        /// </summary>
        /// <exception cref="NullReferenceException"/>
        public void Save()
        {
            if (MountedConfigurationAttribute == null)
                throw new NullReferenceException(nameof(MountedConfigurationAttribute));

            // just write this Config...
            JEMConfiguration.WriteData(JEMConfiguration.ResolveFilePath(MountedConfigurationAttribute.ConfigName), this, JEMConfigurationSaveMethod.JSON);
        }

        /// <summary>
        ///     Update loaded Properties to check if the value has been changed.
        /// </summary>
        public void Update()
        {
            // Collect Changes
            var changed = new List<JEMSmartConfigProperty>();
            foreach (var p in Properties)
            {
                if (p.CheckForPropertyChanges())
                {
                    changed.Add(p);
                }
            }

            // Write Changes
            foreach (var p in changed)
            {
                p.ShouldFixProperty = false;
                Database.WriteToSystem(p.Attribute.VarName, p.Data);
                p.ShouldFixProperty = true;
            }
        }

        /// <summary>
        ///     Collects properties from currently Mounted Type.
        /// </summary>
        /// <exception cref="NullReferenceException"/>
        internal void CollectPropertiesFromMountedType()
        {
            if (MountedType == null)
                throw new NullReferenceException(nameof(MountedType));

            // Clear List
            Properties.Clear();

            // Get Properties
            var allProperties = new List<PropertyInfo>(MountedType.GetProperties(BindingFlags.NonPublic | BindingFlags.Static));
            var allPublicProperties = MountedType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            allProperties.AddRange(allPublicProperties.Where(JEMSmartConfiguration.IsPropertyStatic));
            foreach (var p in allProperties)
            {
                var varAttributes = p.GetCustomAttributes(typeof(JEMSmartConfigVarAttribute), false);
                JEMSmartConfigVarAttribute varAttribute;
                if (varAttributes.Length <= 0)
                {
                    switch (MountedConfigurationAttribute.PropertyMode)
                    {
                        case JEMSmartConfigPropertyMode.Default:
                            continue;
                        case JEMSmartConfigPropertyMode.ForcedAll:
                            if (p.PropertyType == typeof(JEMSmartConfigSave))
                                continue; // yikes

                            varAttribute = new JEMSmartConfigVarAttribute(p.Name);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else varAttribute = (JEMSmartConfigVarAttribute) varAttributes[0];
                if (varAttribute == null) continue;
                var prop = new JEMSmartConfigProperty
                {
                    Property = p,
                    Attribute = varAttribute
                };
                Properties.Add(prop);

                // Update Property Value
                if (Database.HasKey(varAttribute.VarName))
                {
                    var varData = Database.ResolveFromSystem(varAttribute.VarName);
                    prop.SetPropertyValue(varData);
                }
                else
                {
                    // No Record in loaded Database :/
                    Database.WriteToSystem(varAttribute.VarName, p.GetValue(null));
                }

                // Look for change event
                prop.LookForChangeEvent();
                prop.CheckForPropertyChanges();
                Database.RegisterObjectChange(varAttribute.VarName, obj =>
                {
                    // We want to update database on database prop change
                    prop.ApplyFixedPropertyChange(obj);

                    // Change event found
                    // Hook
                    prop.ChangeMethod?.Invoke(obj);
                }, false);
            }
        }
    }
}
