//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using JEM.UnityEngine.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.QNet.UnityEngine.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     A base interface (UI) for QNet connecting and network scene loading.
    /// </summary>
    public class QNetBaseInterface : QNetManagerBehaviour
    {
        [Header("Connection Settings")]
        public JEMInterfaceFadeAnimation ConnectingPanel;
        public Text ConnectingStatus;

        [Space]
        public Button ConnectingButton;
        public Text ConnectingButtonText;

        private bool _wasUserDisconnection;
        private bool _isClientConnecting;
        private bool _wasClientActiveWithHost;

        private void Awake()
        {
            RegisterConnecting();
        }

        private void RegisterConnecting()
        {
            ConnectingButton.onClick.AddListener(() =>
            {
                if (_isClientConnecting)
                {
                    // Cancel the connection.
                    _wasUserDisconnection = true;
                    QNetManager.Instance.StopCurrentConnection("UserDisconnecting");
                }

                // Disable the panel.
                ConnectingPanel.SetActive(false);
            });
        }

        // Handle onClientStarted.
        private void OnClientStarted(QNetConfiguration configurationUsed)
        {
            // Activate the panel.
            ConnectingPanel.SetActive(true);
            ConnectingButton.gameObject.SetActive(true);

            // Update status text.
            ConnectingStatus.text = $"{JEMLocale.TryResolve("CONNECTING_TO").ToUpper()}: {configurationUsed.IpAddress}:{configurationUsed.Port}";
            ConnectingButtonText.text = JEMLocale.TryResolve("CANCEL").ToUpper();

            // Set isClientConnecting.
            _isClientConnecting = true;
            _wasUserDisconnection = false;
            _wasClientActiveWithHost = QNetManager.Instance.IsHostActive;
        }

        private void OnClientConnected()
        {
            // Disable the button because there is no going back right now.
            ConnectingButton.gameObject.SetActive(false);

            // Update the status.
            ConnectingStatus.text = JEMLocale.TryResolve("CONNECTED").ToUpper();
        }

        // Handle onClientStop.
        private void OnClientStop(bool connectionLost, string reason)
        {
            // Check if the active connection was a host.
            if (_wasClientActiveWithHost || _wasUserDisconnection)
            {
                // It was a host, so we don't want to draw any disconnection info.
                return;
            }

            // Activate the panel.
            ConnectingPanel.SetActive(true);
            ConnectingButton.gameObject.SetActive(true);

            // Update status.
            ConnectingStatus.text = $"{JEMLocale.TryResolve(connectionLost ? "DISCONNECTED_FROM_SERVER" : "CONNECTION_FAILED").ToUpper()} " +
                                    $"{reason}";
            ConnectingButtonText.text = JEMLocale.TryResolve("OK").ToUpper();

            _isClientConnecting = false;
        }
    }
}
