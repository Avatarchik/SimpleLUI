//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     A attribute that allows to select all types that are assignable from given.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class JEMTypeSelectAttribute : JEMMultiPropertyAttribute
    {
        public Type BaseType { get; }

        public JEMTypeSelectAttribute(Type baseType) => BaseType = baseType;      

#if UNITY_EDITOR
        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var typeList = GetTypeList(BaseType);
            var type = GetType(property.stringValue);
            var typeIndex = typeList.Item2.IndexOf(type);
            typeIndex = EditorGUI.Popup(position, label, typeIndex, typeList.Item1);

            if (typeIndex < 0)
            {
                typeIndex = 0;
            }
            else if (typeIndex >= typeList.Item2.Count)
            {
                typeIndex = typeList.Item2.Count - 1;
            }

            type = typeList.Item2.Count == 0 ? null : typeList.Item2[typeIndex];
            property.stringValue = type?.FullName ?? string.Empty;
            EditorGUI.EndProperty();

            return true;
        }

        private static Type GetType(string typeName)
        {
            foreach (var list in AllTypesLists)
            {
                for (var index = 0; index < list.Value.Item2.Count; index++)
                {
                    var type = list.Value.Item2[index];
                    if (type.FullName == typeName)
                        return type;
                }
            }

            return null;
        }

        private static Tuple<GUIContent[], IList<Type>> GetTypeList(Type type)
        {
            if (!AllTypesLists.ContainsKey(type))
            {
                var allTypes = new List<Type>();
                for (var index = 0; index < AppDomain.CurrentDomain.GetAssemblies().Length; index++)
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies()[index];
                    allTypes.AddRange(assembly.GetTypes()
                        .Where(type1 => type.IsAssignableFrom(type1) && type1 != type));
                }

                AllTypesLists.Add(type, new Tuple<GUIContent[], IList<Type>>(allTypes.Select(t => new GUIContent(t.Name)).ToArray(), allTypes.ToList()));
            }

            return AllTypesLists[type];
        }

        private static Dictionary<Type, Tuple<GUIContent[], IList<Type>>> AllTypesLists { get; } = new Dictionary<Type, Tuple<GUIContent[], IList<Type>>>();
#endif
    }
}
