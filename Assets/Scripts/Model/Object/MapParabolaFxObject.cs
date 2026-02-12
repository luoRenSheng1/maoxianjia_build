using Config;
using Engine;
using UnityEngine;
using Engine;

namespace Engine
{
    public class MapParabolaFxObject : MapFxObject
    {
        protected Vector3 targetPos;
        protected float speedHorizontal;
        protected float speedVertical;
        protected bool bNoRotate = false;
        protected string targetBone;
        protected MapObject target;
        protected float heightPosY = 0.0f;
        protected float gravity = 0.0f;
        protected float gravity2 = 0.0f;

        public void InitParabola(ConfigSkillEffectUnit cfg, Vector3 pos, Quaternion rot,
            MapObject owner, MapObject target, Vector3 tarPos, float time)
        {
            base.InitByConfig(cfg, pos, rot, owner);

            this.target = target;
            this.targetBone = cfg.TargetBone;

            this.targetPos = target == null
                ? tarPos
                : (string.IsNullOrEmpty(targetBone) ? target.Position : target.DummyPos(targetBone));
            this.speedHorizontal = cfg.Speed * ConstDefine.CONFIG_PLACE;
            this.bNoRotate = !string.IsNullOrEmpty(cfg.TypeData);

            Vector3 delta = this.targetPos - this.position;
            delta.y = 0;
            delta.z = 0;
            
            if (time > 0)
            {
                // 传入时间，使用时间
                this.speedHorizontal = delta.x / time;
            }

            float heightParam = 100.0f;
            float hTime = delta.magnitude / this.speedHorizontal;
            float hTimeHalf = hTime * 0.5f;
            // 最高点设置为高100px的地方
            this.heightPosY = Mathf.Min(this.targetPos.y, this.position.y) - heightParam;
            this.gravity = heightParam * 2 / (hTimeHalf * hTimeHalf);
            this.speedVertical = -(heightParam + Mathf.Max(this.position.y - this.targetPos.y, 0)) * 2 / hTimeHalf;
            this.gravity2 = Mathf.Abs(this.speedVertical) / hTimeHalf;

            if (!this.bNoRotate)
            {
                this.SetRotation(Quaternion.LookRotation(delta), true);
            }

            this.SetTick(true);

            //LogUtils.LogLocalFormat("MapParabolaFxObject Init {0} {1} {2} {3} {4} {5}", this.id, this.position, targetPos, this.speedVertical, this.gravity, hTime);
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

            Vector3 delta = posTarget - this.position;
            delta.z = 0;
            Vector3 pos = position;
            pos.x = Mathf.MoveTowards(pos.x, posTarget.x, speedHorizontal * deltaSeconds);
            if (speedVertical > 0)
            {
                // 下落过程，最多逼近目标
                pos.y = Mathf.MoveTowards(pos.y, posTarget.y, speedVertical * deltaSeconds);
                speedVertical += this.gravity * deltaSeconds;
            }
            else
            {
                pos.y += speedVertical * deltaSeconds;
                speedVertical += this.gravity2 * deltaSeconds;
            }
            
            pos.z = 0;
            this.SetPosition(pos, true);
            if (!this.bNoRotate)
            {
                Quaternion rot = Quaternion.LookRotation(delta);
                this.SetRotation(rot, true);
            }

            // LogUtils.LogLocalFormat("MapParabolaFxObject Tick {0} {1} {2} {3} {4}", this.id, pos, posTarget, speedVertical, deltaSeconds);

            float dist = Vector3.Distance(pos, posTarget);

            if (dist < ConstDefine.MIN_DISTANCE)
            {
                //LogUtils.LogLocalFormat("MapParabolaFxObject End {0} {1} {2}", this.id, posTarget, pos);
                this.EndPlay();
            }
        }
    }
}