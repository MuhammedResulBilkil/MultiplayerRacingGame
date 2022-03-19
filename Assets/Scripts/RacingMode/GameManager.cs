using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace RacingMode
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public TextMeshProUGUI countDownText;
        
        [SerializeField] private List<GameObject> _playerPrefabs = new List<GameObject>();
        [SerializeField] private List<Transform> _instantiatePositons = new List<Transform>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

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
                else
                {
                    Debug.LogError("MultiplayerRacingGame.PlayerSelectionNumber is not in CustomProperties!!!");
                }
            }
            else
            {
                Debug.LogError("PhotonNetwork is not connected and ready yet!!!");
            }
        }
    }
}