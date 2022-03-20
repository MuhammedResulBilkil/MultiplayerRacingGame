using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace RacingMode
{
    public class PlayerSetup : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _cameraParent;
        [SerializeField] private TextMeshProUGUI _playerNameText;

        private CarMovement _carMovement;
        private LapController _lapController;

        private void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
            _lapController = GetComponent<LapController>();
        }

        private void Start()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue(MultiplayerRacingGame.RacingModeName))
            {
                if (photonView.IsMine)
                {
                    _carMovement.enabled = true;
                    _lapController.enabled = true;
                    _cameraParent.SetActive(true);
                }
                else
                {
                    _carMovement.enabled = false;
                    _lapController.enabled = false;
                    _cameraParent.SetActive(false);
                }
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue(MultiplayerRacingGame.DeathRaceModeName))
            {
                if (photonView.IsMine)
                {
                    _carMovement.enabled = true;
                    _carMovement.SetIsControlsEnabled(true);
                    _cameraParent.SetActive(true);
                }
                else
                {
                    _carMovement.enabled = false;
                    _carMovement.SetIsControlsEnabled(false);
                    _cameraParent.SetActive(false);
                }
            }
            else
            {
                Debug.LogErrorFormat($"Nickname: {PhotonNetwork.LocalPlayer.NickName}. Available Custom Property has not been found!");
            }
            
            SetPlayerUI();
        }

        public GameObject GetCameraParent()
        {
            return _cameraParent;
        }

        private void SetPlayerUI()
        {
            if (_playerNameText != null)
            {
                _playerNameText.text = photonView.Owner.NickName;

                if (photonView.IsMine)
                    _playerNameText.gameObject.SetActive(false);
            }
            
        }
    }
}