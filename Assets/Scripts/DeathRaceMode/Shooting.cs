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

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Fire();
            }
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