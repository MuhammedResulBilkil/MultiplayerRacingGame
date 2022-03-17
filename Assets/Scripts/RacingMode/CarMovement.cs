using System;
using UnityEngine;

namespace RacingMode
{
    public class CarMovement : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        
        private Vector3 _thrustForce = new Vector3(0f, 0f, 45f);
        private Vector3 _rotationTorque = new Vector3(0f, 8f, 0f);

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetKey("w"))
            {
                _rigidbody.AddRelativeForce(_thrustForce);
            }
            
            if (Input.GetKey("s"))
            {
                _rigidbody.AddRelativeForce(-_thrustForce);
            }
            
            if (Input.GetKey("a"))
            {
                _rigidbody.AddRelativeTorque(-_rotationTorque);
            }
            
            if (Input.GetKey("d"))
            {
                _rigidbody.AddRelativeTorque(_rotationTorque);
            }
        }
    }
}