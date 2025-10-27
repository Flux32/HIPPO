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
        
        [Header("Follow")]
        [SerializeField] private Transform _followTarget;
        [SerializeField, Min(0f)] private float _followStartDistance = 12f;
        [SerializeField, Min(0f)] private float _followStopDistance = 2.5f;
        [SerializeField, Min(0f)] private float _followLoseDistance = 16f;
        [SerializeField, Min(0f)] private float _playerDetectRadius = 24f;
        [SerializeField] private LayerMask _playerDetectMask = ~0;
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
            
            if (_followTarget == null) 
                AcquirePlayerTarget();
            
            _ctx.Target = _followTarget;
            
            if (_ctx.Target != null && _ctx.PlayerInteractor == null)
                _ctx.PlayerInteractor = _ctx.Target.GetComponentInParent<PlayerFoodInteractor>();
            
            _ctx.FollowStartDistance = _followStartDistance;
            _ctx.FollowStopDistance = _followStopDistance;
            _ctx.FollowLoseDistance = Mathf.Max(_followLoseDistance, _followStartDistance + 0.1f);

            _sensors = new HippoSensors(transform, _groundMask, _edgeCheckDistance, _obstacleCheckDistance, _groundRayLength);
            _locomotion = new HippoLocomotion(transform, _controller, _moveSpeed, _turnSpeed, _gravity, _groundedGravity);
            _tree = new HippoBehavior(gameObject, _ctx, _sensors, _locomotion).Build();
        }

        private void Update()
        {
            if (_ctx.Target == null) {
                AcquirePlayerTarget();
                _ctx.Target = _followTarget;
                
                if (_ctx.Target != null) 
                    _ctx.PlayerInteractor = _ctx.Target.GetComponentInParent<PlayerFoodInteractor>();
            }
            _tree?.Tick();
        }
        
        private void OnDrawGizmosSelected()
        {
            Vector3 home = transform.position;
            Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.25f);
            Gizmos.DrawWireSphere(home, _wanderRadius);

            Gizmos.color = Color.yellow;
            Vector3 start = transform.position + Vector3.up * 0.2f + transform.forward * _edgeCheckDistance;
            Gizmos.DrawLine(start, start + Vector3.down * _groundRayLength);
        }

        private void AcquirePlayerTarget()
        {
            Vector3 center = transform.position;
            float radius = Mathf.Max(_playerDetectRadius, _followLoseDistance);
            Collider[] hits = Physics.OverlapSphere(center, radius, _playerDetectMask, QueryTriggerInteraction.Ignore);
            
            Transform bestT = null;
            PlayerFoodInteractor bestInteractor = null;
            float bestDist = float.MaxValue;
            
            for (int i = 0; i < hits.Length; i++)
            {
                var interactor = hits[i].GetComponentInParent<PlayerFoodInteractor>();
                
                if (interactor == null) 
                    continue;
                
                Transform t = interactor.transform;
                float dx = t.position.x - center.x;
                float dz = t.position.z - center.z;
                float d = dx * dx + dz * dz;
                
                if (d < bestDist)
                {
                    bestDist = d;
                    bestT = t;
                    bestInteractor = interactor;
                }
            }
            
            if (bestT != null)
            {
                _followTarget = bestT;
                if (_ctx != null) _ctx.PlayerInteractor = bestInteractor;
            }
            else
            {
                var inter = FindObjectOfType<PlayerFoodInteractor>();
                if (inter != null)
                {
                    _followTarget = inter.transform;
                    
                    if (_ctx != null) 
                        _ctx.PlayerInteractor = inter;
                }
            }
        }
    }
}
