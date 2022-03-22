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
        [SerializeField] private List<GameObject> _lapTriggers = new List<GameObject>();
        [SerializeField] private List<TextMeshProUGUI> _finishOrderUITexts = new List<TextMeshProUGUI>();

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
                    Debug.LogFormat(
                        $"Racing Scene Start! PlayerSelectionNumber: {playerSelectionNumber}");
                    
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

            foreach (var finishOrderUIText in _finishOrderUITexts)
                finishOrderUIText.gameObject.SetActive(false);
            
        }

        public List<GameObject> GetLapTriggers()
        {
            return _lapTriggers;
        }

        public List<TextMeshProUGUI> GetFinishOrderUITexts()
        {
            return _finishOrderUITexts;
        }
    }
}