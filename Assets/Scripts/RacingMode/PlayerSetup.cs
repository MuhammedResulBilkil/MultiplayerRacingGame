using System;
using Photon.Pun;
using UnityEngine;

namespace RacingMode
{
    public class PlayerSetup : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Camera _mainCamera;

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
                _mainCamera.enabled = true;
            }
            else
            {
                _carMovement.enabled = false;
                _mainCamera.enabled = false;
            }
        }
    }
}