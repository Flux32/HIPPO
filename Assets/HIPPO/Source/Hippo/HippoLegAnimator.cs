using UnityEngine;

namespace HIPPO
{
    public class HippoLegAnimator : MonoBehaviour
    {
        [Header("Legs")]
        [SerializeField] private Transform _frontLeft;
        [SerializeField] private Transform _frontRight;
        [SerializeField] private Transform _backLeft;
        [SerializeField] private Transform _backRight;

        [Header("Swing")] 
        [SerializeField, Range(0f, 60f)] private float _swingAngle = 22f;
        [SerializeField, Range(0.2f, 6f)] private float _baseFrequency = 2.0f;
        [SerializeField, Min(0f)] private float _referenceSpeed = 1.4f;
        [SerializeField, Range(0f, 1f)] private float _amplitudeDamp = 0.2f;
        [SerializeField, Min(0f)] private float _moveThreshold = 0.02f;
        [SerializeField] private HippoContext _ctx;

        private Quaternion _flBase;
        private Quaternion _frBase;
        private Quaternion _blBase;
        private Quaternion _brBase;
        
        private float _amp;
        private float _ampVel;

        private void Awake()
        {
            CacheBaseRotations();
        }

        private void Update()
        {
            float deltaTime = Mathf.Max(Time.deltaTime, 1e-4f);
            float speed = _ctx ? _ctx.Speed : 0f;
            bool moving = _ctx ? _ctx.IsMoving : speed > _moveThreshold;
            float targetAmp = moving ? 1f : 0f;
            
            _amp = Mathf.SmoothDamp(_amp, targetAmp, ref _ampVel, Mathf.Max(0.0001f, _amplitudeDamp), Mathf.Infinity, deltaTime);

            float freq = _baseFrequency * Mathf.Clamp(_referenceSpeed > 0f ? (speed / _referenceSpeed) : 1f, 0.5f, 2.0f);
            float phase = Time.time * Mathf.PI * 2f * freq;
            float sin = Mathf.Sin(phase) * _amp;

            ApplyLegSwing(_frontLeft, _flBase, +sin);
            ApplyLegSwing(_backRight, _brBase, +sin);
            ApplyLegSwing(_frontRight, _frBase, -sin);
            ApplyLegSwing(_backLeft, _blBase, -sin);
        }

        private void ApplyLegSwing(Transform leg, Quaternion baseRot, float sin)
        {
            var angle = sin * _swingAngle;
            leg.localRotation = baseRot * Quaternion.Euler(angle, 0f, 0f);
        }

        private void CacheBaseRotations()
        {
            _flBase = _frontLeft ? _frontLeft.localRotation : Quaternion.identity;
            _frBase = _frontRight ? _frontRight.localRotation : Quaternion.identity;
            _blBase = _backLeft ? _backLeft.localRotation : Quaternion.identity;
            _brBase = _backRight ? _backRight.localRotation : Quaternion.identity;
        }
    }
}
