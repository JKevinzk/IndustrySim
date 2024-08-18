using System;
using UnityEngine;

namespace Components.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class SquarePallet : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.mass = 1f;
            _rigidbody.angularDrag = 0.5f;
            _rigidbody.drag = 0.5f;
        }
    }
}
