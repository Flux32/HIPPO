using System;
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
            if (_ctx == null) _ctx = GetComponent<HippoContext>();
            if (_head == null) _head = FindChildByNameExact(transform, "Head") ?? FindChildContains("Head");
            if (_mouth == null) _mouth = FindChildByNameExact(transform, "M_Hippo_Head_Mouth") ?? FindChildByNameExact(transform, "Head_Mouth") ?? FindChildContains("Mouth");
            _headBase = _head ? _head.localRotation : Quaternion.identity;
            _mouthBase = _mouth ? _mouth.localRotation : Quaternion.identity;
        }

        private void Update()
        {
            var shouldBeg = false;
            if (_ctx != null && _ctx.Target != null)
            {
                var self = _ctx.transform.position; self.y = 0f;
                var tgt = _ctx.Target.position; tgt.y = 0f;
                var near = Vector3.Distance(self, tgt) <= _ctx.FollowStartDistance + 0.01f;
                if (near)
                {
                    var interactor = _ctx.PlayerInteractor ? _ctx.PlayerInteractor : _ctx.Target.GetComponentInParent<PlayerFoodInteractor>();
                    if (interactor != null && interactor.IsHoldingFood) shouldBeg = true;
                }
            }

            if (_head)
            {
                if (shouldBeg) _headHoldUntil = Time.time + _headDownDelay;
                var holdUp = shouldBeg || Time.time < _headHoldUntil;
                var sign = _invertHeadAxisX ? -1f : 1f;
                var angle = holdUp ? _headUpAngle * sign : 0f;
                var target = _headBase * Quaternion.Euler(angle, 0f, 0f);
                _head.localRotation = Quaternion.RotateTowards(_head.localRotation, target, _rotateSpeed * Time.deltaTime);
            }

            if (_mouth)
            {
                if (shouldBeg) _mouthHoldUntil = Time.time + _mouthCloseDelay;
                var holdOpen = shouldBeg || Time.time < _mouthHoldUntil;
                var sign = _invertMouthAxisX ? -1f : 1f;
                var angle = holdOpen ? _mouthOpenAngle * sign : 0f;
                var target = _mouthBase * Quaternion.Euler(angle, 0f, 0f);
                _mouth.localRotation = Quaternion.RotateTowards(_mouth.localRotation, target, _rotateSpeed * Time.deltaTime);
            }
        }

        private static Transform FindChildByNameExact(Transform root, string name)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.Equals(name, StringComparison.Ordinal)) return t;
            }
            return null;
        }

        private static Transform FindChildContains(string token)
        {
            return FindChildContains(FindObjectOfType<HippoAI>()?.transform ?? null, token);
        }

        private static Transform FindChildContains(Transform root, string token)
        {
            if (root == null) return null;
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0) return t;
            }
            return null;
        }
    }
}
