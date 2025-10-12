using System;
using UnityEngine;

namespace PC.Scripts.GameMechanic
{
[RequireComponent(typeof(Rigidbody))]
    public class YEET : NetworkedBehaviour
    {
        private Rigidbody _rigidbody;
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void HandleMessage(NetworkedObject data)
        {
            if (!data.Is<AccData>()) return;
            
            AccData yeet = data.GetData<AccData>();
            
            Debug.Log(yeet.value);
            _rigidbody.AddForce(yeet.value, ForceMode.Impulse);
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                transform.position = Vector3.zero;
                _rigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
}