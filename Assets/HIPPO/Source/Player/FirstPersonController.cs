using UnityEngine;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HIPPO
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _cameraRoot;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float _moveSpeed = 4.5f;
        [SerializeField, Min(0f)] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -19.62f;
        [SerializeField] private float _groundedGravity = -2.0f;

        [Header("Look")]
        [SerializeField, Range(0.01f, 5f)] private float _mouseSensitivity = 0.12f;
        [SerializeField, Range(0.01f, 5f)] private float _gamepadSensitivity = 2.0f;
        [SerializeField, Range(1f, 89f)] private float _pitchClamp = 85f;
        [SerializeField] private bool _lockCursor = true;

        [Header("Input")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _lookAction;
        [SerializeField] private InputActionReference _jumpAction;

        private CharacterController _controller;
        
        private float _pitch;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _moveAction.action.Enable();
            _lookAction.action.Enable();
            _jumpAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveAction.action.Disable();
            _lookAction.action.Disable();
            _jumpAction.action.Disable();
        }

        private void Start()
        {
            if (_lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            HandleLook();
            HandleMove();
        }
        
        private void HandleLook()
        {
            if (_cameraRoot == null)
                return;

            Vector2 look = Vector2.zero;
            if (_lookAction != null)
                look = _lookAction.action.ReadValue<Vector2>();

            transform.Rotate(0f, look.x * _mouseSensitivity, 0f);

            _pitch -= look.y * _mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, -_pitchClamp, _pitchClamp);
            _cameraRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            Vector2 moveInput = Vector2.zero;
            
            if (_moveAction != null)
                moveInput = _moveAction.action.ReadValue<Vector2>();

            bool wantsJump = _jumpAction != null && _jumpAction.action.triggered;

            Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
            input = Vector3.ClampMagnitude(input, 1f);
            
            Vector3 move = transform.TransformDirection(input);

            Vector3 horizontal = move * _moveSpeed;

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0f)
                    _verticalVelocity = _groundedGravity;

                if (wantsJump)
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
            else
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }

            Vector3 velocity = horizontal;
            velocity.y = _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }
    }
}