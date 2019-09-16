//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using JEM.QNet.Messages;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace JEM.QNet.UnityEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     QNetManager Behaviour class.
    ///     Allows to handle methods like OnServerStarted etc.
    /// </summary>
    public class QNetManagerBehaviour : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            LoadMethods();
            Behaviours.Add(this);
        }

        protected virtual void OnDisable() => Behaviours.Remove(this);
        
        /// <summary>
        ///     RegisterCustomHandlers is called at the start of the peer to register your custom network handlers.
        /// </summary>
        private JEMSmartMethod _registerCustomHandlers;

        #region SERVER
        /// <summary>
        ///     Called right after the server peer start.
        /// </summary>
        private JEMSmartMethod _onServerStarted;

        /// <summary>
        ///     Called on client connection to authorize if this client may connect the server.
        ///     Here we also can send som initial data to the client like for ex. currently running gamemode.
        /// </summary>
        private JEMSmartMethod _onServerAuthorizeClient;

        /// <summary>
        ///     Called when server receives approved client connection with newly created QNetPlayer object.
        ///     Here we may also read data that has been written by client in XXX method.
        /// </summary>
        private JEMSmartMethod _onServerNewPlayer;

        /// <summary>
        ///     Called right after the player connection has been lost but before object destroy.
        /// </summary>
        private JEMSmartMethod _onServerLostPlayer;

        /// <summary>
        ///     Called when the server stops.
        /// </summary>
        private JEMSmartMethod _onServerStop;

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
        ///     Called right after the client peer start.
        /// </summary>
        private JEMSmartMethod _onClientStarted;

        /// <summary>
        ///     Called then the client connect to the target server.
        /// </summary>
        private JEMSmartMethod _onClientConnected;

        /// <summary>
        ///     Called when the client stops (disconnects from server).
        /// </summary>
        private JEMSmartMethod _onClientStop;
        #endregion

        /// <summary>
        ///     Called at the end of the server's or client world initialization.
        ///     From here you can spawn or access over network whatever you want.
        /// </summary>
        /// <remarks>
        ///     While host is active, this message is called by the internal client.
        /// </remarks>
        private JEMSmartMethod _onNetworkWorldInitialized;

        /// <summary>
        ///     Called by the <see cref="QNetNetworkScene.LoadNetworkScene"/> when the unity scene loading begins.
        /// </summary>
        private JEMSmartMethod _onUnitySceneLoadingBegin;

        /// <summary>
        ///     Called by the <see cref="QNetNetworkScene.LoadNetworkScene"/> when the unity scene loading ends.
        /// </summary>
        private JEMSmartMethod _onUnitySceneLoadingEnd;

        /// <summary>
        ///     Called when the <see cref="QNetNetworkScene.SceneState"/> changes.
        /// </summary>
        private JEMSmartMethod _onSceneStateChange;

        /// <summary>
        ///     Load the JEMSmartMethod based methods!
        /// </summary>
        private void LoadMethods()
        {
            _registerCustomHandlers = new JEMSmartMethod(this, "RegisterCustomHandlers");

            _onServerStarted = new JEMSmartMethod(this, "OnServerStarted");
            _onNetworkWorldInitialized = new JEMSmartMethod(this, "OnNetworkWorldInitialized");
            _onServerAuthorizeClient = new JEMSmartMethod(this, "OnServerAuthorizeClient");
            _onServerNewPlayer = new JEMSmartMethod(this, "OnServerNewPlayer");
            _onServerLostPlayer = new JEMSmartMethod(this, "OnServerLostPlayer");
            _onServerStop = new JEMSmartMethod(this, "OnServerStop");
            _onServerSpawnPlayer = new JEMSmartMethod(this, "OnServerSpawnPlayer");

            _onClientPrepare = new JEMSmartMethod(this, "OnClientPrepare");
            _onClientStarted = new JEMSmartMethod(this, "OnClientStarted");
            _onClientConnected = new JEMSmartMethod(this, "OnClientConnected");
            _onClientStop = new JEMSmartMethod(this, "OnClientStop");

            _onUnitySceneLoadingBegin = new JEMSmartMethod(this, "OnUnitySceneLoadingBegin");
            _onUnitySceneLoadingEnd = new JEMSmartMethod(this, "OnUnitySceneLoadingEnd");
            _onSceneStateChange = new JEMSmartMethod(this, "OnSceneStateChange");
        }

        internal void CallRegisterCustomHandlers(QNetPeer peer, bool isServer) => _registerCustomHandlers.Invoke(peer, isServer);

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
        internal bool CallOnServerSpawnPlayer(QNetConnection connection, ref Objects.QNetObject obj)
        {
            if (!_onServerSpawnPlayer.IsValid())
                return false;

            object[] p = {obj};
            _onServerSpawnPlayer.Invoke(p);
            obj = (Objects.QNetObject) p[0];

            return true;
        }

        internal void CallOnClientPrepare(ref uint token, ref string nickname)
        {
            object[] p = {token, nickname};
            _onClientPrepare.Invoke(p);
            token = (uint) p[0];
            nickname = (string) p[1];
        }
        internal void CallOnClientStarted(QNetConfiguration configurationUsed) => _onClientStarted.Invoke(configurationUsed);
        internal void CallOnClientConnected() => _onClientConnected.Invoke();
        internal void CallOnClientStop(bool connectionLost, string reason) => _onClientStop.Invoke(connectionLost, reason);

        internal void CallOnNetworkWorldInitialized() => _onNetworkWorldInitialized.Invoke();
        internal void CallOnUnitySceneLoadingBegin(string sceneName) => _onUnitySceneLoadingBegin.Invoke(sceneName);
        internal bool CallOnUnitySceneLoadingEnd(Action process)
        {
            if (!_onUnitySceneLoadingEnd.IsValid())
                return false;

            _onUnitySceneLoadingEnd.Invoke(process);
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
