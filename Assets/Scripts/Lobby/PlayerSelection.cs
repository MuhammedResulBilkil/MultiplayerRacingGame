using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Lobby
{
    public class PlayerSelection : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _selectablePlayers = new List<GameObject>();

        private int _playerSelectionNumber = 0;

        private void Start()
        {
            //ActivatePlayer(0);
        }

        public void ActivatePlayer(int index)
        {
            foreach (GameObject selectablePlayer in _selectablePlayers)
                selectablePlayer.SetActive(false);

            _selectablePlayers[index].SetActive(true);

            //Setting up player selection custom property
            Hashtable playerCustomProperties = new Hashtable
                { { MultiplayerRacingGame.PlayerSelectionNumber, _playerSelectionNumber } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
            
            Debug.LogFormat($"Activate Player! Index Number: {index}");
        }

        public void NextPlayer()
        {
            _playerSelectionNumber++;

            if (_playerSelectionNumber >= _selectablePlayers.Count)
                _playerSelectionNumber = 0;

            ActivatePlayer(_playerSelectionNumber);
        }

        public void PreviousPlayer()
        {
            _playerSelectionNumber--;

            if (_playerSelectionNumber < 0)
                _playerSelectionNumber = _selectablePlayers.Count - 1;

            ActivatePlayer(_playerSelectionNumber);
        }

        public void SetPlayerSelectionNumber(int playerSelectionNumber)
        {
            _playerSelectionNumber = playerSelectionNumber;
        }
    }
}