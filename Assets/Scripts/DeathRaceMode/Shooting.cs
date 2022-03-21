using System;
using System.Collections;
using UnityEngine;

namespace DeathRaceMode
{
    public class Shooting : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _firePosition;
        [SerializeField] private Camera _camera;
        [SerializeField] private DeathRacePlayer _deathRacePlayerProperties;
        [SerializeField] private LineRenderer _lineRenderer;

        private float _fireRate;
        private float _fireTimer;
        private bool _useLaser;

        private void Awake()
        {
            _fireRate = _deathRacePlayerProperties.fireRate;

            _useLaser = string.Equals(_deathRacePlayerProperties.weaponName, "Laser Gun");
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
            
            if (_useLaser)
            {
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 200f))
                {
                    if (!_lineRenderer.enabled)
                        _lineRenderer.enabled = true;
                    
                    _lineRenderer.SetPosition(0, _firePosition.position);
                    _lineRenderer.SetPosition(1, raycastHit.point);
                    
                    StopAllCoroutines();
                    StartCoroutine(DisableLaserAfterSecs(0.3f));
                }
            }
            else
            {
                GameObject bulletGameObject = Instantiate(_bulletPrefab, _firePosition.position, Quaternion.identity);
                bulletGameObject.GetComponent<Bullet>().Initializer(ray.direction, _deathRacePlayerProperties.bulletSpeed,
                    _deathRacePlayerProperties.damage);
            }
        }

        private IEnumerator DisableLaserAfterSecs(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _lineRenderer.enabled = false;
        }
    }
}