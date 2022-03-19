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