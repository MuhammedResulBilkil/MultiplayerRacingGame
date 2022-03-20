using System;
using UnityEngine;

namespace DeathRaceMode
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);
        }

        public void Initializer(Vector3 direction, float speed, float damage)
        {
            transform.forward = direction;

            _rigidbody.velocity = direction * speed;
        }
    }
}