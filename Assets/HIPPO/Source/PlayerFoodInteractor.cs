using UnityEngine;
using UnityEngine.InputSystem;

namespace HIPPO
{
    public class PlayerFoodInteractor : MonoBehaviour
    {
        [SerializeField] private InputActionReference _dropOrPickUpInputAction;
        [SerializeField] private InputActionReference _throwInputAction;
        
        [SerializeField, Min(0.2f)] private float _pickupDistance = 3f;
        [SerializeField, Min(0.1f)] private float _holdDistance = 1.2f;
        [SerializeField, Min(0.1f)] private float _throwForce = 8f;
        [SerializeField] private Transform _holdAnchor;
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _raycastMask = ~0; // default: all layers
        
        private FoodItem _held;

        public bool IsHoldingFood => _held != null;
        public FoodItem HeldItem => _held;

        private void Awake()
        {
            _holdAnchor.localPosition = new Vector3(0f, -0.05f, _holdDistance);
            _holdAnchor.localRotation = Quaternion.identity;
        }

        private void OnEnable()
        {
            if (_camera == null) _camera = Camera.main;
            if (_holdAnchor != null && _camera != null && _holdAnchor.parent != _camera.transform)
            {
                _holdAnchor.SetParent(_camera.transform, worldPositionStays: false);
                _holdAnchor.localPosition = new Vector3(0f, -0.05f, _holdDistance);
                _holdAnchor.localRotation = Quaternion.identity;
            }

            _dropOrPickUpInputAction.action.Enable();
            _throwInputAction.action.Enable();
            _dropOrPickUpInputAction.action.performed += OnTogglePickUpPerformed;
            _throwInputAction.action.performed += OnThrowPerformed;
        }

        private void OnDisable()
        {
            _dropOrPickUpInputAction.action.performed -= OnTogglePickUpPerformed;
            _throwInputAction.action.performed -= OnThrowPerformed;
            _dropOrPickUpInputAction.action.Disable();
            _throwInputAction.action.Disable();
        }

        private void LateUpdate()
        {
            if (_holdAnchor && _camera)
                _holdAnchor.localPosition = new Vector3(0f, -0.05f, _holdDistance);
            if (_held != null && _holdAnchor)
            {
                _held.transform.position = _holdAnchor.position;
                _held.transform.rotation = _holdAnchor.rotation;
            }
        }

        private void OnTogglePickUpPerformed(InputAction.CallbackContext ctx)
        {
            Debug.Log("Picked up food");
            
            if (_held == null) 
                TryPickup();
            else Drop();
        }

        private void OnThrowPerformed(InputAction.CallbackContext ctx)
        {
            if (_held == null) return;
            Throw();
        }

        private void TryPickup()
        {
            int mask = _raycastMask.value == 0 ? Physics.DefaultRaycastLayers : _raycastMask.value;
            
            Debug.Log("TryPickup");
            
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, _pickupDistance, mask))
            {
                Debug.Log("TryPickup: pickup");
                var food = hit.collider.GetComponentInParent<FoodItem>();
                if (food != null)
                {
                    _held = food;
                    _held.PickUp(_holdAnchor);
                }
            }
        }

        private void Drop()
        {
            if (_held == null) return;
            _held.Drop();
            _held = null;
        }

        private void Throw()
        {
            var item = _held;
            _held = null;
            item.Throw(_camera.transform.forward, _throwForce);
        }
    }
}
