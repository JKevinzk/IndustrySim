using UnityEngine;

namespace Components.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class PalletizingBox : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.mass = 0.2f;
            _rigidbody.angularDrag = 0.5f;
            _rigidbody.drag = 0f;
        }
    }
}