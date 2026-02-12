using UnityEngine;
using Engine;

namespace Engine
{
    public class MapLineFxObject : MapFxObject
    {
        protected string targetBone;

        protected MapObject target;
        protected Vector3 targetPos;
        protected float speed = 1.0f;

        public void InitLine(Config.ConfigSkillEffectUnit cfg, Vector3 pos, Quaternion rot, MapObject owner, MapObject target, Vector3 tarPos, float time)
        {
            base.InitByConfig(cfg, pos, rot, owner);

            this.target = target;
            this.targetBone = cfg.TargetBone;
            this.targetPos = target == null ? tarPos : (string.IsNullOrEmpty(targetBone) ? target.Position : target.DummyPos(targetBone));
            this.speed = cfg.Speed * ConstDefine.CONFIG_PLACE;

            // 传入时间，使用时间
            if (time > 0)
            {
                this.speed = Vector3.Distance(pos, this.targetPos) / time;
            }

            Vector3 forward = this.targetPos - this.position;
            if(forward != Vector3.zero)
                this.SetRotation(Quaternion.LookRotation(forward), true);
            this.SetTick(true);
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);

            Vector3 posTarget;

            if (target == null || target.Recycled || target.IsDead)
            {
                // 目标已死亡
                posTarget = targetPos;
            }
            else
            {
                posTarget = string.IsNullOrEmpty(targetBone) ? target.Position : target.DummyPos(targetBone);
            }

            Vector3 forward = posTarget - this.position;
            Vector3 pos = Vector3.MoveTowards(this.position, posTarget, deltaSeconds * this.speed);
            this.SetPosition(pos, true);
            if (forward != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(forward);
                this.SetRotation(rot, true);
                if(this.animComp != null)
                {
                    var transform = this.animComp.transform;
                    Quaternion r = transform.rotation;
                    r.z = -rot.z;
                    transform.rotation = r;
                }

                if (this.gameObj != null)
                {
                    var transform = this.gameObj.transform;
                    Quaternion r = transform.rotation;
                    r.z = -rot.z;
                    transform.rotation = r;
                }
            }

            if (Vector3.Distance(pos, posTarget) < ConstDefine.MIN_DISTANCE)
            {
                this.EndPlay();
            }
        }
    }
}