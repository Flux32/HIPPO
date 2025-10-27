using UnityEngine;

namespace HIPPO
{
    public class HippoSensors
    {
        private readonly Transform _root;
        private readonly LayerMask _groundMask;
        private readonly float _edgeCheckDistance;
        private readonly float _obstacleCheckDistance;
        private readonly float _groundRayLength;

        public HippoSensors(Transform root, LayerMask groundMask, float edgeCheckDistance, float obstacleCheckDistance, float groundRayLength)
        {
            _root = root;
            _groundMask = groundMask;
            _edgeCheckDistance = edgeCheckDistance;
            _obstacleCheckDistance = obstacleCheckDistance;
            _groundRayLength = groundRayLength;
        }

        public bool DirectionIsSafe(Vector3 dir)
        {
            Vector3 start = _root.position + Vector3.up * 0.2f + dir.normalized * _edgeCheckDistance;
            
            if (!RaycastDownIgnoreSelf(start, _groundRayLength, out _)) 
                return false;

            Vector3 origin = _root.position + Vector3.up * 0.6f;
            
            if (Physics.Raycast(origin, dir.normalized, out var hit, _obstacleCheckDistance, _groundMask, QueryTriggerInteraction.Ignore))
            {
                if (!hit.transform.IsChildOf(_root)) return false;
            }

            return true;
        }

        public bool IsNearEdgeAhead()
        {
            Vector3 start = _root.position + Vector3.up * 0.2f + _root.forward * _edgeCheckDistance;
            
            return !RaycastDownIgnoreSelf(start, _groundRayLength, out _);
        }

        public bool IsObstacleAhead()
        {
            Vector3 origin = _root.position + Vector3.up * 0.6f;
            
            if (Physics.Raycast(origin, _root.forward, out var hit, _obstacleCheckDistance, _groundMask, QueryTriggerInteraction.Ignore))
                return !hit.transform.IsChildOf(_root);
            
            return false;
        }

        private bool RaycastDownIgnoreSelf(Vector3 start, float length, out RaycastHit hit)
        {
            var hits = Physics.RaycastAll(start, Vector3.down, length, _groundMask, QueryTriggerInteraction.Ignore);
            
            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].transform.IsChildOf(_root))
                {
                    hit = hits[i];
                    return true;
                }
            }
            
            hit = default;
            return false;
        }
    }
}

