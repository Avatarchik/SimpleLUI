//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Handlers;
using JEM.QNet.UnityEngine.Messages;
using JEM.QNet.UnityEngine.Objects;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JEM.QNet.UnityEngine
{
    /// <summary>
    ///     Defines a state of network scene.
    /// </summary>
    public enum QNetSceneState
    {
        /// <summary>
        ///     The state of scene is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        ///     The network scene is not currently loaded.
        ///     The player is currently in menu like scene.
        /// </summary>
        NotLoaded,

        /// <summary>
        ///     The network scene is currently loading in.
        ///     Scene loading includes objects deserialization.
        /// </summary>
        Loading,

        /// <summary>
        ///     The network scene is currently unloading.
        ///     Scene unloading includes object destroy.
        /// </summary>
        UnLoading,

        /// <summary>
        ///     A network scene is currently loaded.
        /// </summary>
        Loaded
    }

    // Network level initialization methods
    public static partial class QNetNetworkScene
    {
        /// <summary>
        ///     Server initialization process called by <see cref="QNetManager.StartServer"/>.
        /// </summary>
        /// <param name="onInitializationDone">
        ///     Called at the end of server initialization.
        ///     Used by <see cref="QNetManager.StartHost()"/> to know when to start internal client connection.
        /// </param>
        internal static IEnumerator RunServerInitialization(Action onInitializationDone = null)
        {
            // Get the level name to load.
            var levelName = QNetManager.Instance.DefaultLevel;
            // TODO: We may want to create a ManagerBehaviour event method like.: OnServerInitializationBegin(ref string levelName)
            QNetManager.PrintLogInfo($"Starting the server initialization: {levelName}");

            // Run a stopwatch for lolz.
            var stopwatch = Stopwatch.StartNew();

            // Restart scene
            RestartScene();

            // Update scene state: LOADING
            SceneState = QNetSceneState.Loading;
            // Load network level.
            yield return LoadNetworkScene(levelName);

            // Allow objects to be spawned.
            CanSpawnNetworkObjects = true;
            // Update scene state: LOADED
            SceneState = QNetSceneState.Loaded;
            // Start the server.
            QNetManager.Instance.InternalStartServer(QNetManager.Instance.CashedServerConfiguration);

            // Spawn predefined network objects.
            QNetObject.SpawnPredefinedObjects();

            // Call onNetworkWorldInitialized.
            if (!QNetManager.Instance.IsHostActive)
            {
                QNetManagerBehaviour.ForEach(b => b.CallOnNetworkWorldInitialized());
            }

            // Reset routine process
            _runningProcess = null;

            stopwatch.Stop();
            QNetManager.PrintLogInfo("Server initialization done. " +
                                     $"It took {stopwatch.Elapsed.TotalMilliseconds:0.00} ms to complete.");

            onInitializationDone?.Invoke();
        }

        /// <summary>
        ///     First phase initialization for client called by <see cref="QNetHandlerWorld.OnServerLevelLoading"/>
        /// </summary>
        internal static IEnumerator RunClientInitializationFirstPhase(string levelName)
        {
            QNetManager.PrintLogInfo($"Starting first phase of client initialization: {levelName}");

            // Run a stopwatch for lolz.
            var stopwatch = Stopwatch.StartNew();
            // Restart scene only
            //  if the server is not currently active
            if (!QNetManager.Instance.IsServerActive)
            {
                RestartScene();
            } 

            // Update scene state: LOADING
            SceneState = QNetSceneState.Loading;
            // Load network level.
            yield return LoadNetworkScene(levelName);

            // First phase done.
            // Call the server about.
            QNetManager.Send(QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered, QNetUnityHeader.LEVEL_LOADED);

            // reset routine process
            _runningProcess = null;

            stopwatch.Stop();
            QNetManager.PrintLogInfo("First client initialization phase done (network level). " +
                                    $"It took {stopwatch.Elapsed.TotalMilliseconds:0.00} ms to complete.");
        }

        /// <summary>
        ///     Second phase of initialization of client. Called by <see cref=""/>
        /// </summary>
        internal static IEnumerator RunClientInitializationSecondPhase()
        {
            QNetManager.PrintLogInfo("Starting second phase of " +
                                    $"client initialization: {QNetHandlerObject.SerializedObjectsCount} serialized objects.");

            // Run a stopwatch for lolz.
            var stopwatch = Stopwatch.StartNew();
            // Allow objects to be spawned.
            CanSpawnNetworkObjects = true;
            // Run the objects de-serialization
            yield return QNetHandlerObject.DeserializeAllObjects();

            QNetManager.PrintLogInfo("Second phase has successfully deserialized all object." +
                                    $" We now have {QNetObject.Objects.Count} objects in network scene.");

            SceneState = QNetSceneState.Loaded;
            yield return new WaitForEndOfFrame();

            // Second phase done.
            // Call the server about.
            QNetManager.Send(QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered, QNetUnityHeader.WORLD_SERIALIZED);

            stopwatch.Stop();
            QNetManager.PrintLogInfo("Second client initialization phase done (object deserialization). " +
                                    $"It took {stopwatch.Elapsed.TotalMilliseconds:0.00} ms to complete.");
        }

        /// <summary>
        ///     Load network scene called by <see cref="QNetHandlerWorld.OnServerLevelLoading"/> method
        ///         or by the server initialization process.
        /// </summary>
        internal static IEnumerator LoadNetworkScene(string levelName)
        {
            // Check if the scene is valid.
            if (!Application.CanStreamedLevelBeLoaded(levelName))
            {
                throw new InvalidOperationException($"Unable to load network scene '{levelName}'. " +
                                                    "Is the scene name correct?");
            }

            // Update the active scene name.
            SceneName = levelName;

            // call onUnitySceneLoadingBegin
            QNetManagerBehaviour.ForEach(b => b.CallOnUnitySceneLoadingBegin(levelName));
            yield return new WaitForEndOfFrame();

            var asyncOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
            yield return asyncOperation;

            // call onUnitySceneLoadingEnd
            // NOTE: As a additional feature, we will give user a ability to load his content on base network level load.
            bool process = false;
            bool isMethod = false;
            void OnProcess() => process = true;
            QNetManagerBehaviour.ForEach(b =>
            {
                if (b.CallOnUnitySceneLoadingEnd(OnProcess))
                {
                    isMethod = true;
                }
            });

            if (isMethod)
            {
                while (!process)
                    yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        ///     A QNet de-serialization like method that destroys all QNet objects and restores the default scene.
        /// </summary>
        /// <param name="onUnloadDone">
        ///     Called when scene unloading process is done.
        ///     Utilized by <see cref="QNetManager"/> to call it's event metgods or cleanup fields/properties.
        /// </param>
        internal static IEnumerator UnloadNetworkScene(Action onUnloadDone)
        {
            QNetManager.PrintLogInfo("Starting a network scene unloading process.");

            // Run a stopwatch for lolz.
            var stopwatch = Stopwatch.StartNew();

            // Update scene state: UNLOADING
            SceneState = QNetSceneState.UnLoading;

            var asyncOperation = SceneManager.LoadSceneAsync(QNetManager.Instance.MenuLevel, LoadSceneMode.Single);
            yield return asyncOperation;

            // Restart scene.
            RestartScene();

            // reset routine process
            _runningProcess = null;

            stopwatch.Stop();
            QNetManager.PrintLogInfo("Unload network scene done. " +
                                $"It took {stopwatch.Elapsed.TotalMilliseconds:0.00} ms to complete.");
        }

        /// <summary>
        ///     Restart all fields and properties related to network scene.
        /// </summary>
        internal static void RestartScene()
        {
            QNetManager.PrintLogInfo("Restarting the network scene state to default.");

            CanSpawnNetworkObjects = false;
            SceneState = QNetSceneState.NotLoaded;
        }

        /// <summary>
        ///     Runs new initialization process.
        /// </summary>
        internal static void RunInitializationProcess(IEnumerator routine)
        {
            if (_runningProcess != null)
            {
                QNetManager.PrintLogWarning("New initialization process is starting but whe have reference to the last one. " +
                                            "We will try to stop it and run the new one but it may cause future problems.");
                QNetManager.Instance.StopCoroutine(_runningProcess);
            }

            _runningProcess = QNetManager.Instance.StartCoroutine(routine);
        }

        /// <summary>
        ///     Defines if network objects can be currently spawned in locale scene or not.
        /// </summary>
        /// <remarks>
        ///     This value is usually only false if network is not active or the base network scene is loading.
        /// </remarks>
        public static bool CanSpawnNetworkObjects { get; private set; } = false;

        /// <summary>
        ///     Defines a current state of network scene.
        /// </summary>
        public static QNetSceneState SceneState
        {
            get => _sceneState;
            private set
            {
                _sceneState = value;
                QNetManagerBehaviour.ForEach(b => b.CallOnSceneStateChange(value));
            }
        }

        /// <summary>
        ///     Defines a name of scene that will or is currently loaded.
        ///     NOTE: The scene name will only restart onSceneLoad, so the value will stay the same on sceneUnload.
        /// </summary>
        public static string SceneName { get; private set; } = string.Empty;

        /// <summary>
        ///     Defines if the second phase initialization is currently running.
        ///     This helps to tell if newly received object should be instantly created/deleted or added to pool.
        /// </summary>
        internal static bool IsSecondPhaseRunning { get; private set; } = false;

        private static QNetSceneState _sceneState = QNetSceneState.NotLoaded;
        private static Coroutine _runningProcess;
    }
}
