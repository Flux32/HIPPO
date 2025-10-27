using UnityEngine;

namespace HIPPO
{
    public class HippoHead : MonoBehaviour
    {
        [SerializeField] private HippoContext _ctx;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _mouth;
        [SerializeField, Range(-80f, 80f)] private float _headUpAngle = -25f;
        [SerializeField, Range(-80f, 80f)] private float _mouthOpenAngle = 30f;
        [SerializeField, Range(30f, 720f)] private float _rotateSpeed = 240f;
        [SerializeField, Min(0f)] private float _headDownDelay = 1.5f;
        [SerializeField, Min(0f)] private float _mouthCloseDelay = 1.5f;
        [SerializeField] private bool _invertHeadAxisX;
        [SerializeField] private bool _invertMouthAxisX = true;

        private Quaternion _headBase;
        private Quaternion _mouthBase;
        private float _headHoldUntil;
        private float _mouthHoldUntil;

        private void Awake()
        {
            _headBase = _head ? _head.localRotation : Quaternion.identity;
            _mouthBase = _mouth ? _mouth.localRotation : Quaternion.identity;
        }

        private void Update()
        {
            bool shouldBeg = false;
            
            if (_ctx != null && _ctx.Target != null)
            {
                Vector3 self = _ctx.transform.position; self.y = 0f;
                Vector3 tgt = _ctx.Target.position; tgt.y = 0f;
                bool near = Vector3.Distance(self, tgt) <= _ctx.FollowStartDistance + 0.01f;
                
                if (near)
                {
                    PlayerFoodInteractor interactor = _ctx.PlayerInteractor ? _ctx.PlayerInteractor : _ctx.Target.GetComponentInParent<PlayerFoodInteractor>();
                    
                    if (interactor != null && interactor.IsHoldingFood)
                        shouldBeg = true;
                }
            }

            ApplyHoldRotX(_head, _headBase, ref _headHoldUntil, _headDownDelay, _headUpAngle, _invertHeadAxisX, shouldBeg);
            ApplyHoldRotX(_mouth, _mouthBase, ref _mouthHoldUntil, _mouthCloseDelay, _mouthOpenAngle, _invertMouthAxisX, shouldBeg);
        }

        public void ForceCloseMouth()
        {
            _mouthHoldUntil = -1f;
        }

        private void ApplyHoldRotX(Transform joint, Quaternion baseRot, ref float holdUntil, float delay, float angle, bool invert, bool shouldHold)
        {
            if (!joint) 
                return;
            
            if (shouldHold)
                holdUntil = Time.time + delay;
            
            bool hold = shouldHold || Time.time < holdUntil;
            float sign = invert ? -1f : 1f;
            float a = hold ? angle * sign : 0f;
            Quaternion target = baseRot * Quaternion.Euler(a, 0f, 0f);
            joint.localRotation = Quaternion.RotateTowards(joint.localRotation, target, _rotateSpeed * Time.deltaTime);
        }
    }
}
