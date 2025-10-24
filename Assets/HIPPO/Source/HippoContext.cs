using UnityEngine;

namespace HIPPO
{
    public class HippoContext
    {
        public Transform Transform { get; }

        public Transform AreaCenter { get; set; }
        public Vector3 HomePosition { get; set; }
        public float WanderRadius { get; set; }
        public float HomeBias { get; set; } = 0.5f;
        public Vector2 MoveTimeRange { get; set; }
        public Vector2 IdleTimeRange { get; set; }

        public Vector3 MoveDir { get; set; }
        public float MoveEndTime { get; set; }
        public float IdleEndTime { get; set; }
        public bool IsMoving { get; set; }
        public bool IsIdling { get; set; }

        public HippoContext(Transform transform)
        {
            Transform = transform;
            MoveDir = transform.forward;
        }
    }
}

