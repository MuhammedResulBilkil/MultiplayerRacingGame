using System;
using UnityEngine;

namespace DeathRaceMode
{
    public class Shooting : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _firePosition;
        [SerializeField] private Camera _camera;
        [SerializeField] private DeathRacePlayer _deathRacePlayerProperties;

        private float _fireRate;
        private float _fireTimer;

        private void Awake()
        {
            _fireRate = _deathRacePlayerProperties.fireRate;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (_fireTimer > _fireRate)
                {
                    Fire();
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer < _fireRate)
                _fireTimer += Time.deltaTime;
        }

        private void Fire()
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            
            GameObject bulletGameObject = Instantiate(_bulletPrefab, _firePosition.position, Quaternion.identity);
            bulletGameObject.GetComponent<Bullet>().Initializer(ray.direction, _deathRacePlayerProperties.bulletSpeed,
                _deathRacePlayerProperties.damage);
        }
    }
}