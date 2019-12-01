//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Objects;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JEM.QNet.UnityEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     QNetManager Behaviour class.
    ///     Allows to handle methods like OnServerStarted etc.
    /// </summary>
    public abstract class QNetManagerBehaviour : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            LoadMethods();
            Behaviours.Add(this);
        }

        protected virtual void OnDisable() => Behaviours.Remove(this);
        
        #region SERVER
        /// <summary>
        ///     Called right after the server peer start.
        /// </summary>
        private JEMSmartMethodS _onServerStarted;

        /// <summary>
        ///     Called on client connection to authorize if this client may connect the server.
        ///     Here we also can send som initial data to the client like for ex. currently running gamemode.
        /// </summary>
        private JEMSmartMethod _onServerAuthorizeClient;

        /// <summary>
        ///     Called when server receives approved client connection with newly created QNetPlayer object.
        ///     Here we may also read data that has been written by client in OnClientCustomData method.
        /// </summary>
        private JEMSmartMethodS<QNetPlayer, QNetMessageReader> _onServerNewPlayer;

        /// <summary>
        ///     Called right after the player connection has been lost but before object destroy.
        /// </summary>
        private JEMSmartMethodS<QNetPlayer, string> _onServerLostPlayer;

        /// <summary>
        ///     Called when the server stops.
        /// </summary>
        private JEMSmartMethodS<string> _onServerStop;

        /// <summary>
        ///     Called when server need to resolve new spawn for a player.
        /// </summary>
        private JEMSmartMethod _onServerSpawnPlayer;
        #endregion

        #region CLIENT
        /// <summary>
        ///     Called as a onConnected response and contains data serialized by server via OnServerAuthorizeClient.
        ///     OnClientPrepare is method where you may broadcast unique client token to the server.
        /// </summary>
        private JEMSmartMethod _onClientPrepare;

        /// <summary>
        ///     Called right after the OnClientPrepare method but here you can read an serialize custom network data.
        /// </summary>
        private JEMSmartMethodS<QNetMessageReader, QNetMessageWriter> _onClientCustomData;

        /// <summary>
        ///     Called right after the client peer start.
        /// </summary>
        private JEMSmartMethodS<QNetPeerConfiguration> _onClientStarted;

        /// <summary>
        ///     Called when the client connect to the target server.
        /// </summary>
        private JEMSmartMethodS _onClientConnected;

        /// <summary>
        ///     Called when the client stops (disconnects from server).
        /// </summary>
        private JEMSmartMethodS<bool, string> _onClientStop;
        #endregion

        /// <summary>
        ///     RegisterCustomHandlers is called at the start of the peer to register your custom network handlers.
        /// </summary>
        private JEMSmartMethodS<QNetPeer, bool> _registerCustomHandlers;

        /// <summary>
        ///     Called at the end of the server's or client world initialization.
        ///     From here you can spawn or access over network whatever you want.
        /// </summary>
        /// <remarks>
        ///     While host is active, this message is called by the internal client.
        /// </remarks>
        private JEMSmartMethodS _onNetworkWorldInitialized;

        /// <summary>
        ///     Called at the very beginning of <see cref="QNetNetworkScene.LoadNetworkScene"/>
        ///      to resolve unity scene of given name. You can use this method to load your scene via asset bundles.
        /// </summary>
        private JEMSmartMethodS<string, Action> _onResolveUnityScene;

        /// <summary>
        ///     Called by the <see cref="QNetNetworkScene.LoadNetworkScene"/> when the unity scene loading begins.
        /// </summary>
        private JEMSmartMethodS<string> _onUnitySceneLoadingBegin;

        /// <summary>
        ///     Called by the <see cref="QNetNetworkScene.LoadNetworkScene"/> when the unity scene loading ends.
        ///     In this method you may load some content at the end of target scene load.
        ///     At the end of this method you always need to invoke received onProcess action.
        /// </summary>
        private JEMSmartMethodS<Action, Action<float>> _onUnitySceneLoadingEnd;

        /// <summary>
        ///     Called when the <see cref="QNetNetworkScene.SceneState"/> changes.
        /// </summary>
        private JEMSmartMethodS<QNetSceneState> _onSceneStateChange;

        /// <summary>
        ///     Load the JEMSmartMethod based methods!
        /// </summary>
        private void LoadMethods()
        {
            _onServerStarted = new JEMSmartMethodS(this, "OnServerStarted");
            _onServerAuthorizeClient = new JEMSmartMethod(this, "OnServerAuthorizeClient");
            _onServerNewPlayer = new JEMSmartMethodS<QNetPlayer, QNetMessageReader>(this, "OnServerNewPlayer");
            _onServerLostPlayer = new JEMSmartMethodS<QNetPlayer, string>(this, "OnServerLostPlayer");
            _onServerStop = new JEMSmartMethodS<string>(this, "OnServerStop");
            _onServerSpawnPlayer = new JEMSmartMethod(this, "OnServerSpawnPlayer");

            _onClientPrepare = new JEMSmartMethod(this, "OnClientPrepare");
            _onClientCustomData = new JEMSmartMethodS<QNetMessageReader, QNetMessageWriter>(this, "OnClientCustomData");
            _onClientStarted = new JEMSmartMethodS<QNetPeerConfiguration>(this, "OnClientStarted");
            _onClientConnected = new JEMSmartMethodS(this, "OnClientConnected");
            _onClientStop = new JEMSmartMethodS<bool, string>(this, "OnClientStop");

            _registerCustomHandlers = new JEMSmartMethodS<QNetPeer, bool>(this, "RegisterCustomHandlers");
            _onNetworkWorldInitialized = new JEMSmartMethodS(this, "OnNetworkWorldInitialized");
            _onResolveUnityScene = new JEMSmartMethodS<string, Action>(this, "OnResolveUnityScene");
            _onUnitySceneLoadingBegin = new JEMSmartMethodS<string>(this, "OnUnitySceneLoadingBegin");
            _onUnitySceneLoadingEnd = new JEMSmartMethodS<Action, Action<float>>(this, "OnUnitySceneLoadingEnd");
            _onSceneStateChange = new JEMSmartMethodS<QNetSceneState>(this, "OnSceneStateChange");
        }

        internal void CallOnServerStarted() => _onServerStarted.Invoke();
        internal void CallOnServerAuthorize(QNetConnection connection, QNetMessageWriter writer, ref bool refuse)
        {
            object[] p = {connection, writer, refuse};
            _onServerAuthorizeClient.Invoke(p);
            refuse = (bool) p[2];
        }
        internal void CallOnServerNewPlayer(QNetPlayer player, QNetMessageReader reader) => _onServerNewPlayer.Invoke(player, reader);
        internal void CallOnServerLostPlayer(QNetPlayer player, string reason) => _onServerLostPlayer.Invoke(player, reason);
        internal void CallOnServerStop(string stopReason) => _onServerStop.Invoke(stopReason);
        internal bool CallOnServerSpawnPlayer(QNetConnection connection, ref QNetObject obj)
        {
            if (!_onServerSpawnPlayer.IsValid())
                return false;

            object[] p = {connection, obj};
            _onServerSpawnPlayer.Invoke(p);
            obj = (QNetObject) p[1];

            return true;
        }

        internal void CallOnClientPrepare(ref uint token, ref string nickname)
        {
            object[] p = {token, nickname};
            _onClientPrepare.Invoke(p);
            token = (uint) p[0];
            nickname = (string) p[1];
        }
        internal void CallOnClientCustomData(QNetMessageReader reader, QNetMessageWriter writer) => _onClientCustomData.Invoke(reader, writer);
        internal void CallOnClientStarted(QNetPeerConfiguration configurationUsed) => _onClientStarted.Invoke(configurationUsed);
        internal void CallOnClientConnected() => _onClientConnected.Invoke();
        internal void CallOnClientStop(bool connectionLost, string reason) => _onClientStop.Invoke(connectionLost, reason);

        internal void CallRegisterCustomHandlers(QNetPeer peer, bool isServer) => _registerCustomHandlers.Invoke(peer, isServer);
        internal void CallOnNetworkWorldInitialized() => _onNetworkWorldInitialized.Invoke();
        internal bool CallOnResolveUnityScene(string sceneName, Action onContinue)
        {
            if (!_onResolveUnityScene.IsValid())
                return false;
            _onResolveUnityScene.Invoke(sceneName, onContinue);
            return true;
        }
        internal void CallOnUnitySceneLoadingBegin(string sceneName) => _onUnitySceneLoadingBegin.Invoke(sceneName);
        internal bool CallOnUnitySceneLoadingEnd(Action process, Action<float> reportProgress)
        {
            if (!_onUnitySceneLoadingEnd.IsValid())
                return false;

            _onUnitySceneLoadingEnd.Invoke(process, reportProgress);
            return true;
        }  
        internal void CallOnSceneStateChange(QNetSceneState state) => _onSceneStateChange.Invoke(state);

        /// <summary>
        ///     Call each <see cref="QNetManagerBehaviour"/> on scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static void ForEach([NotNull] Action<QNetManagerBehaviour> b)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            for (var index = 0; index < Behaviours.Count; index++)
            {
                var beh = Behaviours[index];
                if (beh.isActiveAndEnabled)
                    b.Invoke(beh);
            }
        }

        /// <summary>
        ///     List of all active QNetManager behaviours.
        /// </summary>
        internal static List<QNetManagerBehaviour> Behaviours { get; } = new List<QNetManagerBehaviour>();
    }
}
