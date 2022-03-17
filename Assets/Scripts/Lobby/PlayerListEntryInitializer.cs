using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Lobby
{
    public class PlayerListEntryInitializer : MonoBehaviour
    {
        [Header("UI References")] public TextMeshProUGUI _playerNameText;
        public Button _playerReadyButton;
        public TextMeshProUGUI _playerReadyButtonText;
        public Image _playerReadyImage;

        private bool _isPlayerReady;

        public void Initializer(int playerID, string playerName)
        {
            _playerNameText.text = playerName;

            if (PhotonNetwork.LocalPlayer.ActorNumber != playerID)
            {
                _playerReadyButton.gameObject.SetActive(false);
            }
            else
            {
                // Local Player
                _playerReadyButton.gameObject.SetActive(true);
                Hashtable customProperties = new Hashtable { { MultiplayerRacingGame.IsPlayerReady, _isPlayerReady } };

                PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

                _playerReadyButton.onClick.AddListener(() =>
                {
                    SetPlayerReady(!_isPlayerReady);

                    Hashtable newCustomProperties = new Hashtable
                        { { MultiplayerRacingGame.IsPlayerReady, _isPlayerReady } };

                    PhotonNetwork.LocalPlayer.SetCustomProperties(newCustomProperties);
                });
            }
        }

        public void SetPlayerReady(bool isPlayerReady)
        {
            _isPlayerReady = isPlayerReady;

            _playerReadyImage.enabled = _isPlayerReady;

            _playerReadyButtonText.text = _isPlayerReady ? "Ready!" : "Ready?";
        }
    }
}