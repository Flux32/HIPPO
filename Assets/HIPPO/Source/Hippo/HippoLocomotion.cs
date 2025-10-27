using UnityEngine;

namespace HIPPO
{
    public class HippoLocomotion
    {
        private readonly Transform _transform;
        private readonly CharacterController _controller;
        private readonly float _moveSpeed;
        private readonly float _turnSpeed;
        private readonly float _gravity;
        private readonly float _groundedGravity;

        private float _verticalVelocity;
        public float CurrentHorizontalSpeed { get; private set; }
        public float MoveSpeed => _moveSpeed;

        public HippoLocomotion(Transform transform, CharacterController controller, float moveSpeed, float turnSpeed, float gravity, float groundedGravity)
        {
            _transform = transform;
            _controller = controller;
            _moveSpeed = moveSpeed;
            _turnSpeed = turnSpeed;
            _gravity = gravity;
            _groundedGravity = groundedGravity;
        }

        public void TurnTowards(Vector3 dir, float dt)
        {
            var fwd = dir == Vector3.zero ? _transform.forward : dir;
            var target = Quaternion.LookRotation(fwd, Vector3.up);
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, target, _turnSpeed * dt);
        }

        public void MoveForward(float dt)
        {
            ApplyGravity(dt);
            var horizontal = _transform.forward * _moveSpeed;
            var velocity = new Vector3(horizontal.x, _verticalVelocity, horizontal.z);
            _controller.Move(velocity * dt);
            CurrentHorizontalSpeed = _moveSpeed;
        }

        public void MoveVerticalOnly(float dt)
        {
            ApplyGravity(dt);
            _controller.Move(new Vector3(0f, _verticalVelocity, 0f) * dt);
            CurrentHorizontalSpeed = 0f;
        }

        private void ApplyGravity(float dt)
        {
            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0f) _verticalVelocity = _groundedGravity;
            }
            else
            {
                _verticalVelocity += _gravity * dt;
            }
        }
    }
}
