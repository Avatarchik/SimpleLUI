//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Reflection;

namespace JEM.Core.Configuration
{
    /// <summary>
    ///     JEM Smart Configuration class.
    ///     It manages 'Smart Configuration' system.
    /// </summary>
    /// <remarks>Smart Configuration is a system that allows to define configuration properties (CVar like)</remarks>
    public static class JEMSmartConfiguration
    {
        /// <summary>
        ///     Try to load configuration from given instance of object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static JEMSmartConfigSave LoadConfiguration(Type t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            var configAttributes = t.GetCustomAttributes(typeof(JEMSmartConfigAttribute), false);
            if (configAttributes.Length <= 0) throw new NullReferenceException($"Type {t.Name} does not have JEMSmartConfigAttribute set!");
            var configAttribute = (JEMSmartConfigAttribute) configAttributes[0];

            // Load data
            var loaded = JEMConfiguration.LoadData<JEMSmartConfigSave>(JEMConfiguration.ResolveFilePath(configAttribute.ConfigName), JEMConfigurationSaveMethod.JSON);
            loaded.MountedConfigurationAttribute = configAttribute;
            loaded.MountedType = t;

            //  Collect properties
            loaded.CollectPropertiesFromMountedType();

            return loaded;
        }

        /// <summary>
        ///     Checks if given Property is static.
        /// </summary>
        public static bool IsPropertyStatic(PropertyInfo property)
        {
            var setter = property.GetSetMethod();
            var getter = property.GetGetMethod();
            if (setter == null && getter == null)
                return false; // lul
            return (setter?.IsStatic ?? false) || (getter?.IsStatic ?? false);
        }
    }
}
