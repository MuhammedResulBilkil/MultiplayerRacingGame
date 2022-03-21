using System;
using System.Collections;
using Photon.Pun;
using RacingMode;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DeathRaceMode
{
    public class TakeDamage : MonoBehaviourPun
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Image _healthBar;
        [SerializeField] private GameObject _playerGraphics;
        [SerializeField] private GameObject _playerUI;
        [SerializeField] private GameObject _playerWeaponHolder;
        [SerializeField] private GameObject _deathPanelUIPrefab;

        private GameObject _deathPanelUIGameObject;
        private Rigidbody _rigidbody;
        private CarMovement _carMovement;
        private Shooting _shooting;

        private float _currentHealth;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _carMovement = GetComponent<CarMovement>();
            _shooting = GetComponent<Shooting>();

            _currentHealth = _maxHealth;

            _healthBar.fillAmount = _currentHealth / _maxHealth;
        }

        [PunRPC]
        public void DoDamage(float damage)
        {
            _currentHealth -= damage;

            Debug.LogFormat($"Current Health: {_currentHealth}");

            _healthBar.fillAmount = _currentHealth / _maxHealth;

            if (_currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            _carMovement.enabled = false;
            _shooting.enabled = false;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _playerGraphics.SetActive(false);
            _playerUI.SetActive(false);
            _playerWeaponHolder.SetActive(false);

            if (photonView.IsMine)
            {
                // Respawn
                StartCoroutine(Respawn());
            }
        }

        private IEnumerator Respawn()
        {
            GameObject canvasGameObject = GameObject.Find("Canvas");

            if (_deathPanelUIGameObject == null)
                _deathPanelUIGameObject = Instantiate(_deathPanelUIPrefab, canvasGameObject.transform);
            else
                _deathPanelUIGameObject.SetActive(true);

            TextMeshProUGUI respawnTimeText = _deathPanelUIGameObject.transform.Find("Text_RespawnTime")
                .GetComponent<TextMeshProUGUI>();

            float respawnTime = 8.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            while (respawnTime > 0f)
            {
                yield return new WaitForSeconds(1f);
                respawnTime -= 1f;
                respawnTimeText.text = respawnTime.ToString(".00");
            }
            
            _deathPanelUIGameObject.SetActive(false);

            _carMovement.enabled = true;
            _shooting.enabled = true;

            int randomPoint = Random.Range(-20, 20);
            transform.position = new Vector3(randomPoint, 0f, randomPoint);

            photonView.RPC(nameof(Reborn), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void Reborn()
        {
            _currentHealth = _maxHealth;
            _healthBar.fillAmount = _currentHealth / _maxHealth;

            _playerGraphics.SetActive(true);
            _playerUI.SetActive(true);
            _playerWeaponHolder.SetActive(true);
        }
    }
}