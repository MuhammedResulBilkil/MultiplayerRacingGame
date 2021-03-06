using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace RacingMode
{
    public class LapController : MonoBehaviourPun
    {
        [SerializeField] private List<GameObject> _lapTriggers = new List<GameObject>();

        private CarMovement _carMovement;
        private PlayerSetup _playerSetup;

        private int _finishOrder;
        private string _nickName;
        private int _viewID;

        private void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
            _playerSetup = GetComponent<PlayerSetup>();
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnPlayerFinished;
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnPlayerFinished;
        }

        private void OnPlayerFinished(EventData photonEventData)
        {
            if (photonEventData.Code == RacingModeEventCodes.WhoFinished)
            {
                object[] eventData = (object[]) photonEventData.CustomData;

                string nickNameOfFinishedPlayer = (string)eventData[0];

                _finishOrder = (int)eventData[1];

                int receivedViewID = (int)eventData[2];
                
                Debug.LogFormat($"{_finishOrder}. {nickNameOfFinishedPlayer}");

                TextMeshProUGUI finishOrderUIText = GameManager.Instance.GetFinishOrderUITexts()[_finishOrder - 1];

                if (receivedViewID == _viewID)
                {
                    finishOrderUIText.text = $"{_finishOrder}. {nickNameOfFinishedPlayer} (YOU)";
                    finishOrderUIText.color = Color.red;
                }
                else
                {
                    finishOrderUIText.text = $"{_finishOrder}. {nickNameOfFinishedPlayer}";
                }
                
                finishOrderUIText.gameObject.SetActive(true);
            }
        }

        private void Start()
        {
            foreach (GameObject lapTrigger in GameManager.Instance.GetLapTriggers())
                _lapTriggers.Add(lapTrigger);

            _nickName = photonView.Owner.NickName;
            _viewID = photonView.ViewID;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_lapTriggers.Contains(other.gameObject))
            {
                int indexOfTrigger = _lapTriggers.IndexOf(other.gameObject);
                _lapTriggers[indexOfTrigger].SetActive(false);

                if (string.Equals(other.name, "FinishTrigger"))
                {
                    GameFinished();
                }
            }
        }

        private void GameFinished()
        {
            _carMovement.enabled = false;
            _playerSetup.GetCameraParent().transform.SetParent(null);

            _finishOrder++;

            object[] eventData = { _nickName, _finishOrder, _viewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = false
            };

            PhotonNetwork.RaiseEvent(RacingModeEventCodes.WhoFinished, eventData, raiseEventOptions, sendOptions);

        }
    }
}