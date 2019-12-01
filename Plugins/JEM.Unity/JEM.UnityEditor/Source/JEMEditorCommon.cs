//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor
{
    /// <summary>
    ///     Some simple common options that are missing in unity but nice to have.
    /// </summary>
    public static class JEMEditorCommon
    {
        /// <summary>
        ///     Makes a duplicate of selected assets.
        /// </summary>
        [MenuItem("Assets/Duplicate", priority = -11)]
        public static void Duplicate(MenuCommand c)
        {
            var assets = Selection.assetGUIDs;
            if (assets.Length == 0)
            {
                Debug.Log("No context to duplicate.");
                return;
            }

            foreach (var assetGuid in assets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (!File.Exists(assetPath))
                {
                    Debug.Log($"Invalid asset path received. ({assetPath})");
                }

                var newAssetPath = $"{Path.GetDirectoryName(assetPath)}{JEMProgram.DirectorySeparator}" +
                                   $"{Path.GetFileNameWithoutExtension(assetPath)} (Duplicate){Path.GetExtension(assetPath)}";

                if (AssetDatabase.CopyAsset(assetPath, newAssetPath))
                {
                    Debug.Log($"Asset '{assetPath}' copied" +
                              $"\n\tin to '{newAssetPath}'");

                    // Refresh and save the database.
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();

                    if (assets.Length == 0)
                    {
                        // Select the copied asset.
                        var newAssetObject = AssetDatabase.LoadAssetAtPath<Object>(newAssetPath);
                        Selection.SetActiveObjectWithContext(newAssetObject, newAssetObject);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to copy asset '{assetPath}' " +
                                   $"\n\t in to '{newAssetPath}'");
                }
            }
        }

        /// <summary>
        ///     Selects the selected asset to copy.
        /// </summary>
        [MenuItem("Assets/Copy", priority = -9)]
        public static void Copy(MenuCommand c)
        {
            _copiedFiles = Selection.assetGUIDs;
        }

        [MenuItem("Assets/Paste", priority = -8)]
        public static void Paste(MenuCommand c)
        {
            var copyDirectory = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            copyDirectory = Path.GetDirectoryName(copyDirectory) + "\\" + Path.GetFileNameWithoutExtension(copyDirectory);
            if (!Directory.Exists(copyDirectory))
            {
                Debug.LogError($"Invalid directory received. ({copyDirectory})");
                return;
            }

            if (_copiedFiles == null || _copiedFiles.Length == 0)
            {
                Debug.Log("Nothing to paste.");
                return;
            }

            foreach (var s in _copiedFiles)
            {
                var path = AssetDatabase.GUIDToAssetPath(s);
                if (!File.Exists(path))
                {
                    Debug.Log($"Not a file. ({path})");
                }

                var newPath = $"{copyDirectory}{JEMProgram.DirectorySeparator}" +
                              $"{Path.GetFileNameWithoutExtension(path)} (Duplicate){Path.GetExtension(path)}";
                if (File.Exists(newPath))
                {
                    continue;
                }

                if (AssetDatabase.CopyAsset(path, newPath))
                {
                    Debug.Log($"{path} copied in to {newPath}");
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    Debug.LogError($"Failed to copy {path} in to {newPath}");
                }
            }
        }

        //[MenuItem("Assets/Parent/Make Root", priority = -8)]
        //public static void SelectRoot(MenuCommand command)
        //{
        //    _rootingTarget = Selection.activeObject;
        //}

        //[MenuItem("Assets/Parent/Make Child", priority = -8)]
        //public static void SelectParentingTarget()
        //{
        //    if (_rootingTarget == null)
        //    {
        //        Debug.LogError("No rooting target selected.");
        //        return;
        //    }

        //    // Check if object are valid.
        //    var targets = Selection.objects;
        //    for (var index = 0; index < targets.Length; index++)
        //    {
        //        var t = targets[index];
        //        if (t == _rootingTarget)
        //        {
        //            Debug.LogError($"Invalid parenting target received. target == RootingTarget");
        //            return;
        //        }
        //    }

        //    // Process.
        //    for (var index = 0; index < targets.Length; index++)
        //    {
        //        var t = targets[index];
        //        var tPath = AssetDatabase.GetAssetPath(t);

        //        var copy = Object.Instantiate(t);
        //        AssetDatabase.AddObjectToAsset(copy, _rootingTarget);
        //        AssetDatabase.DeleteAsset(tPath);
        //        AssetDatabase.Refresh();
        //    }
        //}

        //private static Object _rootingTarget;
        private static string[] _copiedFiles;
    }
}
