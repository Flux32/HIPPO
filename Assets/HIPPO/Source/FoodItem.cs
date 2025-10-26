using UnityEngine;

namespace HIPPO
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class FoodItem : MonoBehaviour
    {
        private bool _isHeld;

        private Rigidbody _rb;
        private Collider _collider;

        public bool IsHeld => _isHeld;
        public Transform Holder => _holder;
        
        private Transform _holder;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void PickUp(Transform holder)
        {
            _isHeld = true;
            _holder = holder;
            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _collider.enabled = false;
            transform.SetParent(holder, worldPositionStays: true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Drop()
        {
            _isHeld = false;
            _holder = null;
            transform.SetParent(null, true);
            _collider.enabled = true;
            _rb.isKinematic = false;
        }

        public void Throw(Vector3 direction, float force)
        {
            Drop();
            _rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        }

        public void Consume()
        {
            Destroy(gameObject);
        }
    }
}
