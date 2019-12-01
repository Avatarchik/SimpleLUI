//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using JEM.UnityEngine.Interface.Animation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.QNet.UnityEngine.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     A base interface (UI) for QNet connecting and network scene loading.
    /// </summary>
    [DisallowMultipleComponent]
    public class QNetBaseInterface : QNetManagerBehaviour
    {
        [Header("Connection Settings")]
        public JEMInterfaceFadeAnimation ConnectingPanel;
        public Text ConnectingStatus;

        [Space]
        public Button ConnectingButton;
        public Text ConnectingButtonText;

        [Header("Network Scene Settings")]
        public JEMInterfaceFadeAnimation SceneLoadingPanel;
        public Text SceneLoadingConnectionName;
        public Text SceneLoadingLevelName;
        public Text SceneLoadingStatus;
        public Image SceneLoadingProgressFill;
        public float SceneLoadingProgressSmooth = 5f;

        [Header("Locale Settings")]
        public string LocaleGroup = "SYSTEM";

        public static bool WasUserDisconnection { get; set; }
        private bool _isClientConnecting;
        private bool _wasClientActiveWithHost;

        private string _connectionName = "Unknown";
        private float _progress;
        private float _progressInterpolated;

        private void Awake()
        {
            if (Instance != null)
            {
                gameObject.SetActive(false);
                return;
            }

            Instance = this;

            // Register events for connecting.
            RegisterConnecting();

            // Register scene loading progress report.
            QNetNetworkScene.OnReportSceneLoadingProgress += OnReportSceneLoadingProgress;
        }

        private void RegisterConnecting()
        {
            if (ConnectingPanel == null)
            {
                return;
            }

            ConnectingButton.onClick.AddListener(() =>
            {
                if (_isClientConnecting)
                {
                    // Cancel the connection.
                    WasUserDisconnection = true;
                    QNetManager.Instance.StopCurrentConnection("UserDisconnecting");
                }

                // Disable the panel.
                ConnectingPanel.SetActive(false);
            });
        }

        private void Update()
        {
            if (!SceneLoadingPanel.IsActive) return;
            _progressInterpolated = Mathf.Lerp(_progressInterpolated, _progress, Time.deltaTime * SceneLoadingProgressSmooth);
            SceneLoadingProgressFill.fillAmount = _progressInterpolated;
        }

        // Handle onClientStarted.
        private void OnClientStarted(QNetPeerConfiguration configurationUsed)
        {
            // Set the connection name.
            _connectionName = configurationUsed.IpAddress + ":" + configurationUsed.Port;

#if DEBUG
            QNetManager.PrintLogMsc($"QNetBaseInterface.OnClientStarted({_connectionName})", this);
#endif

            if (ConnectingPanel == null || QNetManager.Instance.IsHostActive)
            {
                return;
            }

            // Activate the panel.
            ConnectingPanel.SetActive(true);
            ConnectingButton.gameObject.SetActive(true);

            // Update status text.
            ConnectingStatus.text = $"{JEMLocale.TryResolve(LocaleGroup, "CONNECTING_TO").ToUpper()}: {configurationUsed.IpAddress}:{configurationUsed.Port}";
            ConnectingButtonText.text = JEMLocale.TryResolve(LocaleGroup, "CANCEL").ToUpper();

            // Set isClientConnecting.
            _isClientConnecting = true;
            WasUserDisconnection = false;
            _wasClientActiveWithHost = QNetManager.Instance.IsHostActive;
        }

        // Handle onClientConnected.
        private void OnClientConnected()
        {
            if (ConnectingPanel == null)
            {
                return;
            }

#if DEBUG
            QNetManager.PrintLogMsc("QNetBaseInterface.OnClientConnected()", this);
#endif

            // Disable the button because there is no going back right now.
            ConnectingButton.gameObject.SetActive(false);

            // Update the status.
            ConnectingStatus.text = JEMLocale.TryResolve(LocaleGroup, "CONNECTED").ToUpper();
        }

        // Handle onClientStop.
        private void OnClientStop(bool connectionLost, string reason)
        {
            if (ConnectingPanel == null)
            {
                return;
            }

#if DEBUG
            QNetManager.PrintLogMsc($"QNetBaseInterface.OnClientStop({connectionLost}, {reason})", this);
#endif

            // Check if the active connection was a host.
            if (_wasClientActiveWithHost || WasUserDisconnection)
            {
                // It was a host, so we don't want to draw any disconnection info.
                return;
            }

            // Activate the panel.
            ConnectingPanel.SetActive(true);
            ConnectingButton.gameObject.SetActive(true);

            // Update status.
            ConnectingStatus.text = $"{JEMLocale.TryResolve(LocaleGroup, connectionLost ? "DISCONNECTED_FROM_SERVER" : "CONNECTION_FAILED").ToUpper()} " +
                                    $"{reason}";
            ConnectingButtonText.text = JEMLocale.TryResolve(LocaleGroup, "OK").ToUpper();

            // Restart.
            _isClientConnecting = false;
            _connectionName = "Unknown";
        }

        // Handle onSceneStateChange.
        private void OnSceneStateChange(QNetSceneState sceneState)
        {
            if (SceneLoadingPanel == null)
            {
                return;
            }

#if DEBUG
            QNetManager.PrintLogMsc($"QNetBaseInterface.OnSceneStateChange({sceneState})", this);
#endif

            switch (sceneState)
            {
                case QNetSceneState.Loading:
                    // Enable the loading panel.
                    SetLoadingScene(true);

                    // Set the status.
                    SceneLoadingConnectionName.text = $"{JEMLocale.TryResolve(LocaleGroup, "JOINING_TO")}: {_connectionName}".ToUpper();
                    SceneLoadingLevelName.text = QNetNetworkScene.SceneName.ToUpper();
                    SceneLoadingStatus.text = $"{JEMLocale.TryResolve(LocaleGroup, "LOADING_SCENE")}".ToUpper();
                    break;
                case QNetSceneState.UnLoading:
                    // Enable the loading panel.
                    SetLoadingScene(true);

                    // Set the status.
                    SceneLoadingConnectionName.text = $"{JEMLocale.TryResolve(LocaleGroup, "DISCONNECTING_FROM")}: {_connectionName}".ToUpper();
                    SceneLoadingLevelName.text = "Menu".ToUpper();
                    SceneLoadingStatus.text = $"{JEMLocale.TryResolve(LocaleGroup, "RESTORING_MENU")}".ToUpper();
                    break;
                case QNetSceneState.NotLoaded:
                    // Disable the loading panel.
                    SetLoadingScene(false);
                    break;
                case QNetSceneState.Loaded:
                    // Disable scene in onNetworkWorldInitialized instead of here.
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sceneState), sceneState, null);
            }
        }

        // Handle onNetworkWorldInitialized.
        private void OnNetworkWorldInitialized()
        {
            if (ConnectingPanel != null)
            {
                ConnectingPanel.SetActive(false);
            }

            if (SceneLoadingPanel == null)
            {
                return;
            }

#if DEBUG
            QNetManager.PrintLogMsc("QNetBaseInterface.OnNetworkWorldInitialized()", this);
#endif

            // Disable the loading panel.
            SetLoadingScene(false);
        }

        private void SetLoadingScene(bool activeState)
        {
            SceneLoadingPanel.SetActive(activeState);
            if (activeState)
            {
                _progress = 0f;
                _progressInterpolated = 0f;
                if (SceneLoadingProgressFill != null)
                    SceneLoadingProgressFill.fillAmount = 0f;
            }
        }

        private void OnReportSceneLoadingProgress(float progress) => _progress = progress;

        /// <summary>
        ///     Reference to active instance of script.
        /// </summary>
        private static QNetBaseInterface Instance { get; set; }
    }
}
