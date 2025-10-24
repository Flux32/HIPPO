using UnityEngine;

namespace HIPPO
{
    public class HippoContext : MonoBehaviour
    {
        public Transform AreaCenter { get; set; }
        public Vector3 HomePosition { get; set; }
        public float WanderRadius { get; set; }
        public float HomeBias { get; set; } = 0.5f;
        public Vector2 MoveTimeRange { get; set; }
        public Vector2 IdleTimeRange { get; set; }
        public Transform Target { get; set; }
        public float FollowStartDistance { get; set; }
        public float FollowStopDistance { get; set; }
        public float FollowLoseDistance { get; set; }

        public Vector3 MoveDir { get; set; }
        public float MoveEndTime { get; set; }
        public float IdleEndTime { get; set; }
        public bool IsMoving { get; set; }
        public bool IsIdling { get; set; }
        public float Speed { get; set; }

        private void Awake()
        {
            MoveDir = transform.forward;
        }
    }
}
