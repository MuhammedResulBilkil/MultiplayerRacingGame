using System;
using Photon.Pun;
using UnityEngine;

namespace RacingMode
{
    public class PlayerSetup : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _cameraParent;

        private CarMovement _carMovement;

        private void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                _carMovement.enabled = true;
                _cameraParent.SetActive(true);
            }
            else
            {
                _carMovement.enabled = false;
                _cameraParent.SetActive(false);
            }
        }

        public GameObject GetCameraParent()
        {
            return _cameraParent;
        }
    }
}