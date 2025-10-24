using UnityEngine;

namespace HIPPO
{
    public class HippoLegAnimator : MonoBehaviour
    {
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
            if (_ctx == null) _ctx = GetComponent<HippoContext>();
        }

        private void Update()
        {
            if (_ctx == null) _ctx = GetComponent<HippoContext>();
            var dt = Mathf.Max(Time.deltaTime, 1e-4f);
            var speed = _ctx ? _ctx.Speed : 0f;
            var moving = _ctx ? _ctx.IsMoving : speed > _moveThreshold;
            var targetAmp = moving ? 1f : 0f;
            _amp = Mathf.SmoothDamp(_amp, targetAmp, ref _ampVel, Mathf.Max(0.0001f, _amplitudeDamp), Mathf.Infinity, dt);

            var freq = _baseFrequency * Mathf.Clamp(_referenceSpeed > 0f ? (speed / _referenceSpeed) : 1f, 0.5f, 2.0f);
            var phase = Time.time * Mathf.PI * 2f * freq;
            var s = Mathf.Sin(phase) * _amp;

            ApplyLegSwing(_frontLeft, _flBase, +s);
            ApplyLegSwing(_backRight, _brBase, +s);
            ApplyLegSwing(_frontRight, _frBase, -s);
            ApplyLegSwing(_backLeft, _blBase, -s);
        }

        private void ApplyLegSwing(Transform leg, Quaternion baseRot, float sinVal)
        {
            var angle = sinVal * _swingAngle;
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
