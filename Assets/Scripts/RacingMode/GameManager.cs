using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace RacingMode
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _playerPrefabs = new List<GameObject>();
        [SerializeField] private List<Transform> _instantiatePositons = new List<Transform>();

        private void Start()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PlayerSelectionNumber,
                        out object playerSelectionNumber))
                {
                    int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                    Vector3 instantiatePositon = _instantiatePositons[actorNumber - 1].position;

                    PhotonNetwork.Instantiate(_playerPrefabs[(int)playerSelectionNumber].name, instantiatePositon,
                        Quaternion.identity);
                }
            }
        }
    }
}