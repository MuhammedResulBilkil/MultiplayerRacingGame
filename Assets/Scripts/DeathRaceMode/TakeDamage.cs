using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace DeathRaceMode
{
    public class TakeDamage : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Image _healthBar;
        
        private float _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;

            _healthBar.fillAmount = _currentHealth / _maxHealth;
        }

        [PunRPC]
        public void DoDamage(float damage)
        {
            _currentHealth -= damage;
            
            Debug.LogFormat($"Current Health: {_currentHealth}");
            
            _healthBar.fillAmount = _currentHealth / _maxHealth;
            
            if(_currentHealth <= 0)
                Die();
            
        }

        private void Die()
        {
            
        }
    }
}