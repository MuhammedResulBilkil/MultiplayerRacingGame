using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace DeathRaceMode
{
    public class Shooting : MonoBehaviourPun
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
            if(!photonView.IsMine && PhotonNetwork.IsConnected)
                return;

            if (Input.GetKey(KeyCode.Space))
            {
                if (_fireTimer > _fireRate)
                {
                    //Fire
                    photonView.RPC(nameof(Fire), RpcTarget.All, _firePosition.position);
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer < _fireRate)
                _fireTimer += Time.deltaTime;
        }

        [PunRPC]
        private void Fire(Vector3 firePosition, PhotonMessageInfo photonMessageInfo)
        {
            Debug.LogFormat($"{nameof(Fire)} RPC Called on {PhotonNetwork.LocalPlayer.NickName}! GameObject Name: {gameObject.name} Message: {photonMessageInfo.ToString()}");
            
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            
            if (_useLaser)
            {
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 200f))
                {
                    if (!_lineRenderer.enabled)
                        _lineRenderer.enabled = true;
                    
                    _lineRenderer.SetPosition(0, firePosition);
                    _lineRenderer.SetPosition(1, raycastHit.point);

                    if (raycastHit.collider.CompareTag("Player"))
                    {
                        TakeDamage hitPlayerTakeDamage = raycastHit.collider.GetComponent<TakeDamage>();
                        PhotonView hitPlayerPhotonView = raycastHit.collider.GetComponent<PhotonView>();
                        
                        if(hitPlayerPhotonView.IsMine)
                            hitPlayerPhotonView.RPC(nameof(hitPlayerTakeDamage.DoDamage), RpcTarget.AllBuffered, _deathRacePlayerProperties.damage);
                    }
                    
                    StopAllCoroutines();
                    StartCoroutine(DisableLaserAfterSecs(0.3f));
                }
            }
            else
            {
                GameObject bulletGameObject = Instantiate(_bulletPrefab, firePosition, Quaternion.identity);
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