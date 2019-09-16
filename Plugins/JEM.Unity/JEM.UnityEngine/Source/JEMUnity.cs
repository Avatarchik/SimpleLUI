//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using JEM.Core.Debugging;
using JEM.UnityEngine.Audio;
using JEM.UnityEngine.Components;
using JEM.UnityEngine.Components.Internal;
using JEM.UnityEngine.Extension;
using JEM.UnityEngine.Interface;
using JEM.UnityEngine.Resource;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Diagnostics;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Some core methods of <see cref="JEM"/> for <see cref="UnityEngine"/>.
    /// </summary>
    public static class JEMUnity
    {
        /// <summary>
        ///     Prepares JEM scripts to work.
        ///     Scripts that are used by systems and need to use for ex.: <see cref="IEnumerator"/>.
        /// </summary>
        public static void PrepareJEMScripts()
        {
            var sw = Stopwatch.StartNew();
            JEMLogger.Init();

            JEMOperation.RegenerateLocalScript();

            JEMGameResources.RegenerateLocalScript();

            JEMInterfaceFadeElement.RegenerateLocalScript();
            JEMInterfaceFadeAnimation.RegenerateLocalScript();

            JEMTranslatorScript.RegenerateLocalScript();

            JEMExtensionAudioSource.RegenerateLocalScript();
            JEMExtensionGameObject.RegenerateLocalScript();
            JEMExtensionUIText.RegenerateLocalScript();

            sw.Stop();
            JEMLogger.InternalLog($"JEM scripts prepare took {sw.Elapsed.TotalSeconds:0.00} seconds.");
        }

        /// <summary>
        ///     Checks if the database is loaded.
        /// </summary>
        public static bool HasDatabaseLoaded() => ResolveDatabase != null && Database != null;

        /// <summary>
        ///     Returns the active <see cref="JEMKeyBasedDatabase"/> object loaded by <see cref="JEMUnity"/>.
        ///     The difference between this method and <see cref="Database"/> is that this method checks if <see cref="Database"/> can be loaded.
        ///     If database can't be loaded, returns null value.
        /// </summary>
        [CanBeNull]
        public static JEMKeyBasedDatabase GetDatabase() => !HasDatabaseLoaded() ? null : Database;
        
        /// <summary>
        ///     Called then <see cref="JEMUnity"/> need to resolve it's <see cref="Database"/>.
        /// </summary>
        public static Func<JEMKeyBasedDatabase> ResolveDatabase;

        /// <summary>
        ///     Reference to the <see cref="JEMKeyBasedDatabase"/> used as a current game's configuration.
        /// </summary>
        /// <remarks>
        ///     Utilized by for ex.: <see cref="JEMAudioManager"/> to know what volume what audio type should play on or by
        ///      <see cref="JEMCanvasController"/> to know on what scale mode the canvas should work on.
        /// </remarks>
        [NotNull]
        public static JEMKeyBasedDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    if (ResolveDatabase == null)
                        throw new NullReferenceException("Unable to resolve Database." +
                                                         "You need to set the ResolveDatabase event first.");

                    _database = ResolveDatabase.Invoke();
                    if (_database == null)
                        throw new NullReferenceException("Your handler of ResolveDatabase " +
                                                         "returned a null value with is not allowed.");
                }

                return _database;
            }
        }

        private static JEMKeyBasedDatabase _database;
    }
}