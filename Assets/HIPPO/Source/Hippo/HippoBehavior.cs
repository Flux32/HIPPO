using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

namespace HIPPO
{
    public class HippoBehavior
    {
        private readonly GameObject _owner;
        private readonly HippoContext _ctx;
        private readonly HippoSensors _sensors;
        private readonly HippoLocomotion _locomotion;

        public HippoBehavior(GameObject owner, HippoContext ctx, HippoSensors sensors, HippoLocomotion locomotion)
        {
            _owner = owner;
            _ctx = ctx;
            _sensors = sensors;
            _locomotion = locomotion;
        }

        public BehaviorTree Build()
        {
            return new BehaviorTreeBuilder(_owner)
                .RepeatForever()
                    .Selector("Root")
                        .Sequence("Follow Player")
                            .Condition("Has Target", () => _ctx.Target != null)
                            .Condition("In Follow Range", () =>
                            {
                                var self = _ctx.transform.position; self.y = 0f;
                                var tgt = _ctx.Target.position; tgt.y = 0f;
                                return Vector3.Distance(self, tgt) <= _ctx.FollowStartDistance;
                            })
                            .Do("Follow", FollowStep)
                        .End()
                        .Sequence("Wander Loop")
                            .Do("Move", MoveStep)
                            .Do("Idle", IdleStep)
                        .End()
                    .End()
                .End()
                .Build();
        }

        private bool InFollowRange()
        {
            if (_ctx.Target == null) return false;
            var a = _ctx.transform.position; a.y = 0f;
            var b = _ctx.Target.position; b.y = 0f;
            return Vector3.Distance(a, b) <= _ctx.FollowStartDistance;
        }

        private TaskStatus MoveStep()
        {
            Transform t = _ctx.transform;
            if (InFollowRange())
            {
                _ctx.IsMoving = false;
                _ctx.Speed = 0f;
                return TaskStatus.Failure;
            }
            
            if (!_ctx.IsMoving)
            {
                _ctx.IsMoving = true;
                _ctx.MoveEndTime = Time.time + Random.Range(_ctx.MoveTimeRange.x, _ctx.MoveTimeRange.y);

                Vector3 dir = Random.onUnitSphere; 
                dir.y = 0f; 
                dir.Normalize();
                
                if (dir.sqrMagnitude < 0.01f) 
                    dir = t.forward;

                Vector3 toHome = (_ctx.HomePosition - t.position); toHome.y = 0f;
                if (toHome.magnitude > _ctx.WanderRadius)
                {
                    dir = Vector3.Slerp(dir, toHome.normalized, _ctx.HomeBias);
                }

                for (int i = 0; i < 6; i++)
                {
                    if (_sensors.DirectionIsSafe(dir)) break;
                    dir = Quaternion.Euler(0f, Random.Range(-140f, 140f), 0f) * dir;
                }

                _ctx.MoveDir = dir.normalized;
            }

            Vector3 flatPos = t.position; 
            flatPos.y = 0f;
            Vector3 flatHome = _ctx.HomePosition; 
            flatHome.y = 0f;
            Vector3 toHomeFlat = (flatHome - flatPos);
            
            if (toHomeFlat.magnitude > _ctx.WanderRadius)
                _ctx.MoveDir = Vector3.Slerp(_ctx.MoveDir, toHomeFlat.normalized, _ctx.HomeBias * Time.deltaTime).normalized;

            _locomotion.TurnTowards(_ctx.MoveDir, Time.deltaTime);
            
            if (_sensors.IsNearEdgeAhead() || _sensors.IsObstacleAhead())
            {
                float turn = Random.Range(60f, 140f) * (Random.value < 0.5f ? -1f : 1f);
                _ctx.MoveDir = Quaternion.Euler(0f, turn, 0f) * t.forward;
            }

            _locomotion.MoveForward(Time.deltaTime);
            _ctx.Speed = _locomotion.CurrentHorizontalSpeed;
            _ctx.IsMoving = true;

            if (Time.time >= _ctx.MoveEndTime)
            {
                _ctx.IsMoving = false;
                return TaskStatus.Success;
            }

            return TaskStatus.Continue;
        }

        private TaskStatus IdleStep()
        {
            var t = _ctx.transform;
            if (InFollowRange())
            {
                _ctx.IsIdling = false;
                _ctx.IsMoving = false;
                _ctx.Speed = 0f;
                return TaskStatus.Failure;
            }
            if (!_ctx.IsIdling)
            {
                _ctx.IsIdling = true;
                _ctx.IdleEndTime = Time.time + Random.Range(_ctx.IdleTimeRange.x, _ctx.IdleTimeRange.y);
            }

            if (Random.value < 0.02f)
            {
                var turn = Random.Range(-30f, 30f) * Time.deltaTime;
                t.rotation = Quaternion.Euler(0f, t.eulerAngles.y + turn, 0f);
            }

            _locomotion.MoveVerticalOnly(Time.deltaTime);
            _ctx.Speed = 0f;
            _ctx.IsMoving = false;

            if (Time.time >= _ctx.IdleEndTime)
            {
                _ctx.IsIdling = false;
                return TaskStatus.Success;
            }

            return TaskStatus.Continue;
        }

        private TaskStatus FollowStep()
        {
            if (_ctx.Target == null) return TaskStatus.Failure;

            var self = _ctx.transform.position; self.y = 0f;
            var tgt = _ctx.Target.position; tgt.y = 0f;
            var to = (tgt - self);
            var dist = to.magnitude;

            if (dist > _ctx.FollowLoseDistance)
            {
                _ctx.IsMoving = false;
                _ctx.Speed = 0f;
                return TaskStatus.Failure;
            }

            var dir = dist > 0.001f ? to / dist : _ctx.transform.forward;

            if (dist > _ctx.FollowStopDistance)
            {
                _ctx.MoveDir = dir;
                _locomotion.TurnTowards(_ctx.MoveDir, Time.deltaTime);
                if (_sensors.IsNearEdgeAhead() || _sensors.IsObstacleAhead())
                {
                    var turn = Random.Range(60f, 120f) * (Random.value < 0.5f ? -1f : 1f);
                    _ctx.MoveDir = Quaternion.Euler(0f, turn, 0f) * _ctx.transform.forward;
                }
                _locomotion.MoveForward(Time.deltaTime);
                _ctx.Speed = _locomotion.CurrentHorizontalSpeed;
                _ctx.IsMoving = true;
            }
            else
            {
                _locomotion.TurnTowards(dir, Time.deltaTime);
                _locomotion.MoveVerticalOnly(Time.deltaTime);
                _ctx.Speed = 0f;
                _ctx.IsMoving = false;
            }

            return TaskStatus.Continue;
        }
    }
}
