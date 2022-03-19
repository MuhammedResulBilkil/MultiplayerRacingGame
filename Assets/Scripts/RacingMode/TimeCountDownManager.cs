using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace RacingMode
{
    public class TimeCountDownManager : MonoBehaviourPunCallbacks
    {
        private CarMovement _carMovement;
        private TextMeshProUGUI _countDownText;
        private float _timeToStartRace = 5f;

        private void Awake()
        {
            _countDownText = GameManager.Instance.countDownText;
            _carMovement = GetComponent<CarMovement>();
        }

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (_timeToStartRace >= 0f)
                {
                    _timeToStartRace -= Time.deltaTime;
                    photonView.RPC(nameof(SetTime), RpcTarget.AllBuffered, _timeToStartRace);
                }
                else if(_timeToStartRace < 0f)
                {
                    photonView.RPC(nameof(StartTheRace), RpcTarget.AllBuffered);
                    enabled = false;
                }
            }
        }

        [PunRPC]
        public void SetTime(float time, PhotonMessageInfo photonMessageInfo)
        {
            if (photonView.IsMine)
            {
                Debug.LogFormat($"{nameof(SetTime)} RPC Called on {PhotonNetwork.LocalPlayer.NickName}! GameObject Name: {gameObject.name} Message: {photonMessageInfo.ToString()}");

                _countDownText.text = time > 0f ? time.ToString("F1") : "";
            }
        }

        [PunRPC]
        public void StartTheRace(PhotonMessageInfo photonMessageInfo)
        {
            Debug.LogFormat($"{nameof(StartTheRace)} RPC Called on {PhotonNetwork.LocalPlayer.NickName}! GameObject Name: {gameObject.name} Message: {photonMessageInfo.ToString()}");
            _carMovement.SetIsControlsEnabled(true);
            enabled = false;
        }
    }
}