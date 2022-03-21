using System;
using Photon.Pun;
using UnityEngine;

namespace DeathRaceMode
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private float _bulletDamage;
        
        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);

            if (other.CompareTag("Player"))
            {
                TakeDamage otherTakeDamage = other.GetComponent<TakeDamage>();
                PhotonView otherPhotonView = other.GetComponent<PhotonView>();

                if (otherPhotonView.IsMine)
                    otherPhotonView.RPC(nameof(otherTakeDamage.DoDamage), RpcTarget.AllBuffered, _bulletDamage);
            }
            
        }

        public void Initializer(Vector3 direction, float speed, float damage)
        {
            _bulletDamage = damage;
            
            transform.forward = direction;

            _rigidbody.velocity = direction * speed;
        }
    }
}