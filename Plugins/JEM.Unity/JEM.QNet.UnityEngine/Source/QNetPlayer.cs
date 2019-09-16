//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Components;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JEM.QNet.UnityEngine
{
    /// <summary>
    ///     QNet Player.
    ///     In short, a QNetConnection extension defined in QNet.Unity.
    ///     Contains core data used by QNet like token or spawned Object controlled by the player.
    /// </summary>
    /// <remarks>
    ///     QNetPlayer is only available on server-side.
    /// </remarks>
    public class QNetPlayer
    {
        /// <summary>
        ///     Connection of this player.
        ///     This property is set only on server.
        ///     Clients will always see this property set to default.
        /// </summary>
        public QNetConnection Connection { get; set; }

        /// <summary>
        ///     Identity of connection.
        /// </summary>
        public ushort ConnectionIdentity { get; set; }

        /// <summary>
        ///     Token of player. Used for authorization and save loading.
        /// </summary>
        public uint Token { get; }

        /// <summary>
        ///     Nickname of the player.
        /// </summary>
        public string Nickname { get; private set; }

        /// <summary>
        ///     Defines whether this player is ready.
        ///     If set to ready, will receive all QNetPlayers and some object data.
        /// </summary>
        public bool Ready { get; set; }

        /// <summary>
        ///     Defines whether the player is loaded.
        ///     Used for map loading on fly.
        /// </summary>
        public bool Loaded { get; set; } = true;

        /// <summary>
        ///     Player network controller.
        /// </summary>
        public Objects.QNetObject PlayerObject { get; private set; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        private QNetPlayer(QNetConnection connection, uint token, string nickname)
        {
            Connection = connection;
            ConnectionIdentity = connection.ConnectionIdentity;

            Token = token;
            Nickname = nickname;
        }

        /// <summary>
        ///     Gets QNetObjectBehaviour based component from PlayerObject.
        /// </summary>
        public T GetPlayerObject<T>() where T : Objects.QNetObject => PlayerObject == null ? default : PlayerObject.GetComponent<T>();
        
        /// <summary>
        ///     Tags this player as ready.
        ///     An actual loading method.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        internal void TagAsReady()
        {
            if (Ready)
            {
                if (!QNetManager.Instance.IsHostActive)
                    throw new InvalidOperationException("Unable to tag player as ready while is already ready.");

                return;
            }

            Ready = true;
#if DEBUG
            QNetManager.PrintLogMsc($"Server is tagging player {ToString()} as ready.");
#endif
            bool hasSpawn = false;
            QNetManagerBehaviour.ForEach(b =>
            {
                if (hasSpawn)
                {
                    // We want o call only one spawn method.
                    return;
                }

                Objects.QNetObject obj = null;
                if (b.CallOnServerSpawnPlayer(Connection, ref obj))
                {
                    hasSpawn = true;
                    PlayerObject = obj;
                }
            });

            if (!hasSpawn)
            {
                // Generate reliable point.
                JEMArea.GetRandomArea().GenerateReliablePoint(out var spawnPoint, out var spawnForward);
                var spawnRotation = Quaternion.LookRotation(spawnForward);

                // Create player's object.
                PlayerObject = Objects.QNetObject.ServerSpawn(QNetManager.Instance.DatabaseReference.PlayerPrefab, Connection, spawnPoint, spawnRotation);
            }

            // Check if PlayerObject is not null.
            if (PlayerObject == null)
                throw new NullReferenceException($"Failed to tag player {ToString()} as ready. " +
                                                 "PlayerObject is null!");

            QNetManager.PrintLogInfo($"Player {ToString()} is ready.", PlayerObject);
        }

        /// <summary>
        ///     Tags this player as not ready.
        ///     An actual unloading method.
        /// </summary>
        internal void TagAsNotReady()
        {
            if (!Ready)
                throw new InvalidOperationException($"Unable to tag player {ToString()} as not ready. " +
                                                    "This player is already set as not ready.");

#if DEBUG
            QNetManager.PrintLogMsc($"Server is tagging {ToString()} as not ready.");
#endif
            Ready = false;

            // Destroy all owned objects.
            QNetNetworkScene.DestroyAllOwnedObjectsOfConnection(Connection);

            QNetManager.PrintLogInfo($"Player {ToString()} is now not ready.");
        }

        /// <inheritdoc />
        public override string ToString() => $"QNetPlayer-{ConnectionIdentity}/{Token}/{Nickname}";

        /// <summary>
        ///     Creates new QNetPlayer instance based on given connection identity.
        /// </summary>
        /// <param name="connectionIdentity">Connection identity of player.</param>
        /// <param name="token">Token of the player.</param>
        /// <param name="nickname">Default nickname of player.</param>
        public static QNetPlayer CreateQNetPlayer(ushort connectionIdentity, uint token, string nickname)
        {
            var connection = QNetManager.GetConnection(connectionIdentity);
            if (connection.Equals(default(QNetConnection)))
                throw new InvalidOperationException("System was unable to create QNetPlayer " +
                                                    $"on server. Connection of identity {connectionIdentity} not exist.");

            var player = new QNetPlayer(connection, token, nickname);
            if (GetQNetPlayer(player.ConnectionIdentity) != null)
                throw new InvalidOperationException("System is trying to create QNetPlayer " +
                                                    "of identity that already exist in this machine.");

            QNetPlayers.Add(player);
            return player;
        }

        /// <summary>
        ///     Destroys QNetPlayer of given connection identity.
        /// </summary>
        internal static void DestroyQNetPlayer(ushort connectionIdentity)
        {
            var player = GetQNetPlayer(connectionIdentity);
            if (player == null)
                throw new InvalidOperationException("You are trying to destroy QNetPlayer " +
                                                    $"of identity {connectionIdentity}, but player of this identity not exists.");

            DestroyQNetPlayer(player);
        }

        /// <summary>
        ///     Destroys given QNetPlayer.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static void DestroyQNetPlayer([NotNull] QNetPlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            QNetPlayers.Remove(player);
        }

        /// <summary>
        ///     Destroys all QNetPlayers.
        /// </summary>
        internal static bool DestroyAllQNetPlayers()
        {
            QNetManager.PrintLogInfo("QNetUnity is destroying all QNetPlayers.");
            while (QNetPlayers.Count > 0)
                DestroyQNetPlayer(QNetPlayers[0]);
            
            return true;
        }

        /// <summary>
        ///     Gets QNetPlayer by it's connection identity.
        /// </summary>
        public static QNetPlayer GetQNetPlayer(QNetConnection connection) => GetQNetPlayer(connection.ConnectionIdentity);
        
        /// <summary>
        ///     Gets QNetPlayer by it's connection identity.
        /// </summary>
        public static QNetPlayer GetQNetPlayer(ushort connectionIdentity) =>
            QNetPlayers.FirstOrDefault(player => player.ConnectionIdentity == connectionIdentity);
        
        /// <summary>
        ///     Gets QNetPlayer by it's token.
        /// </summary>
        public static QNetPlayer GetQNetPlayerByToken(uint token) =>
            QNetPlayers.FirstOrDefault(player => player.Token == token);
        
        /// <summary>
        ///     List of all QNetPlayers created in server.
        /// </summary>
        public static List<QNetPlayer> QNetPlayers { get; } = new List<QNetPlayer>();
    }
}