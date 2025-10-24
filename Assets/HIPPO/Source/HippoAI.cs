using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

namespace HIPPO
{
    [RequireComponent(typeof(HippoContext), typeof(CharacterController))]
    public class HippoAI : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float _moveSpeed = 1.4f;
        [SerializeField, Range(30f, 720f)] private float _turnSpeed = 180f;
        [SerializeField] private Vector2 _moveTimeRange = new Vector2(1.6f, 3.2f);
        [SerializeField] private Vector2 _idleTimeRange = new Vector2(1.2f, 2.6f);
        [SerializeField, Min(0f)] private float _wanderRadius = 12f;

        [Header("Ground / Avoidance")]
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField, Min(0.1f)] private float _edgeCheckDistance = 0.7f;
        [SerializeField, Min(0.1f)] private float _obstacleCheckDistance = 0.7f;
        [SerializeField, Min(0.2f)] private float _groundRayLength = 2.0f;

        [Header("Gravity (CharacterController)")]
        [SerializeField] private float _gravity = -19.62f;
        [SerializeField] private float _groundedGravity = -2.0f;

        [Header("Optional")]
        [SerializeField, Tooltip("Bias to home when outside radius (0-1)")] private float _homeBias = 0.5f;
        [SerializeField, Tooltip("Behavior tree cache for visualization")] private BehaviorTree _tree;

        private CharacterController _controller;
        private HippoContext _ctx;
        private HippoSensors _sensors;
        private HippoLocomotion _locomotion;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            
            _controller.height = 1.4f;
            _controller.radius = 0.45f;
            _controller.center = new Vector3(0f, 0.7f, 0f);
            _controller.slopeLimit = 45f;
            _controller.stepOffset = 0.3f;
   
            _ctx = GetComponent<HippoContext>();
            _ctx.HomePosition = transform.position;
            _ctx.WanderRadius = _wanderRadius;
            _ctx.HomeBias = Mathf.Clamp01(_homeBias);
            _ctx.MoveTimeRange = _moveTimeRange;
            _ctx.IdleTimeRange = _idleTimeRange;

            _sensors = new HippoSensors(transform, _groundMask, _edgeCheckDistance, _obstacleCheckDistance, _groundRayLength);
            _locomotion = new HippoLocomotion(transform, _controller, _moveSpeed, _turnSpeed, _gravity, _groundedGravity);
            _tree = new HippoBehavior(gameObject, _ctx, _sensors, _locomotion).Build();
        }

        private void Update()
        {
            _tree?.Tick();
        }

        private void OnDrawGizmosSelected()
        {
            var home = transform.position;
            Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.25f);
            Gizmos.DrawWireSphere(home, _wanderRadius);

            Gizmos.color = Color.yellow;
            var start = transform.position + Vector3.up * 0.2f + transform.forward * _edgeCheckDistance;
            Gizmos.DrawLine(start, start + Vector3.down * _groundRayLength);
        }
    }
}
