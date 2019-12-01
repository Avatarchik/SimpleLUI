//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Simulation;
using JEM.UnityEngine.Attribute;
using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JEM.Test.Unity.Editor
{
    [Flags]
    internal enum TestFlags : sbyte
    {
        Hello = 1 << 0,
        World = 2 << 0,
        Flag3 = 3 << 0
    }

    internal class InspectorAttributes : MonoBehaviour
    {
        [JEMFoldoutBegin("lol")]
        public string Meme;
        
        [JEMFoldoutEnd]
        public string Hoho;

        [JEMFoldoutBegin("My Test Group")]
        [JEMReadonly]
        [JEMHeader("Readonly Test")]
        public string MyReadonly = "This is readonly!";

        [JEMFoldoutItem]
        [JEMHeader("Type Select Test")]
        [JEMTypeSelect(typeof(QNetSimulableObject))]
        public string MyType;

        [JEMFoldoutItem]
        [JEMHeader("Scene Asset Test")]
        [JEMSceneAsset(OnlyFromBuildSettings = true)]
        public string MyScene;

        [JEMFoldoutItem]
        [JEMHeader("Enum Flags Test")]
        [JEMEnumFlags]
        public TestFlags MyFlag;

        [Space]
        [JEMHeader("Hello, World")]
        [JEMFoldoutItem]
        public string MyString;

        [Space(16)]
        [JEMFoldoutItem]
        [JEMRange(0, 10)]
        public float MyRangeFloat;

        [JEMFoldoutItem]
        [JEMRange(0, 10)]
        public int MyRangeInt;

        [Space]
        [JEMFoldoutItem]
        [JEMMinMaxRange(0, 10)]
        public Vector2 MyMinMaxRangeFloat;

        [JEMFoldoutItem]
        [JEMMinMaxRange(0, 10)]
        public Vector2Int MyMinMaxRangeInt;

        [JEMSpace]
        [JEMFoldoutItem]
        [JEMRange(0, 10)]
        public float MinTest;

        [JEMFoldoutItem]
        [JEMRange(10, 20)]
        public float MaxTest;

        [JEMSpace]
        [JEMIndentLevel]
        [JEMFoldoutItem]
        [JEMRange(nameof(MinTest), nameof(MaxTest))]
        public float MyRangeFloatTest;

        [JEMIndentLevel]
        [JEMFoldoutItem]
        [JEMRange(nameof(MinTest), nameof(MaxTest))]
        public int MyRangeIntTest;

        [JEMIndentLevel]
        [JEMFoldoutItem]
        [JEMMinMaxRange(nameof(MinTest), nameof(MaxTest))]
        public Vector2 MyMinMaxRangeFloatTest;

        [JEMIndentLevel]
        [JEMFoldoutItem]
        [JEMMinMaxRange(nameof(MinTest), nameof(MaxTest))]
        public Vector2Int MyMinMaxRangeIntTest;

        [JEMSpace]
        [JEMIndentLevel]
        [JEMFoldoutItem]
        [JEMPropertyInfo(JEMPropertyInfoCondition.NegativeValue, "The reference is missing!", MessageType = JEMMessageType.Error)]
        public GameObject MyGameObject;

        [JEMFoldoutItem]
        public Vector2 MyVector2;

        [JEMIndentLevel(2)]
        [JEMFoldoutItem]
        [JEMPropertyBased(nameof(MyGameObject))]
        public bool MyBool1;

        [JEMIndentLevel(3)]
        [JEMFoldoutItem]
        [JEMPropertyBased(nameof(MyBool1))]
        public bool MyBool2;

        [JEMButton("Randomize", nameof(MyButtonMethod), Position = JEMButtonAttributePosition.Bottom)]
        public string MyStringWithButton;

        [JEMSelectLocaleGroup]
        public string MyLocaleGroup;

        [JEMSelectLocaleKey(null, GroupProperty = nameof(MyLocaleGroup))]
        public string MyLocaleKey;

        private void MyButtonMethod()
        {
#if UNITY_EDITOR
            // Register undo.
            Undo.RegisterFullObjectHierarchyUndo(this, "Randomize string");

            // Randomize the value.
            MyStringWithButton = Random.Range(int.MinValue, int.MaxValue).ToString();

            // Record changes of prefab.
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
        }
    }
}
