using System;
using System.Collections.Generic;
using UnityEngine;

namespace RacingMode
{
    public class LapController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _lapTriggers = new List<GameObject>();

        private CarMovement _carMovement;
        private PlayerSetup _playerSetup;

        private void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
            _playerSetup = GetComponent<PlayerSetup>();
        }

        private void Start()
        {
            foreach (GameObject lapTrigger in GameManager.Instance.GetLapTriggers())
                _lapTriggers.Add(lapTrigger);
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
        }
    }
}