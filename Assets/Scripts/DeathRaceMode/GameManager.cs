using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace DeathRaceMode
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<GameObject> _playerPrefabs = new List<GameObject>();

        private void Start()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PlayerSelectionNumber,
                        out object playerSelectionNumber))
                {
                    Debug.LogFormat(
                        $"Death Race Scene Start! PlayerSelectionNumber: {((int) playerSelectionNumber).ToString()}");
                    
                    int randomPositon = Random.Range(-15, 15);

                    PhotonNetwork.Instantiate(_playerPrefabs[(int)playerSelectionNumber].name,
                        new Vector3(randomPositon, 0, randomPositon), Quaternion.identity);
                }
                else
                {
                    Debug.LogErrorFormat(
                        $"Player Name: {PhotonNetwork.LocalPlayer.NickName}. PlayerSelectionNumber Custom Property is not found!!!");
                }
            }
        }
        
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.LogFormat($"OnPlayerPropertiesUpdate! Death Race Scene! Target Player Name: {targetPlayer.NickName}");

            if (changedProps.TryGetValue(MultiplayerRacingGame.PlayerSelectionNumber, out object playerSelectionNumber))
            {
                Debug.LogFormat($"Target Player's Selection Number: {playerSelectionNumber}");
            }
        }

        public void OnQuitMatchButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnJoinedRoom()
        {
            Debug.LogFormat($"OnJoinedRoom! Scene Name: {SceneManager.GetActiveScene().name}. Player Name: {PhotonNetwork.LocalPlayer.NickName}");
        }

        public override void OnLeftRoom()
        {
            Debug.LogFormat($"OnLeftRoom! Scene Name: {SceneManager.GetActiveScene().name}");
            
            PhotonNetwork.LoadLevel(MultiplayerRacingGame.LobbySceneName);
        }
    }
}