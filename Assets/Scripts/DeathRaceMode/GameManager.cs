using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathRaceMode
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _playerPrefabs = new List<GameObject>();

        private void Start()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PlayerSelectionNumber,
                        out object playerSelectionNumber))
                {
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
    }
}