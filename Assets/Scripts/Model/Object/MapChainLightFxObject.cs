using UnityEngine;
using Engine;

namespace Engine
{
    /// <summary>
    /// 连锁闪电
    /// </summary>
    public class MapChainLightFxObject : MapFxObject
    {
        protected string targetBone;

        private MapObject actorOwner = null;
        private MapObject actorStart = null;
        private MapObject actorEnd = null;
        private Vector3 posEnd = Vector3.zero;
        private float fTimeStart = 0.0f;
        private float fTimeEnd = 0.0f;
        private float fTimeCur = 0.0f;         // 计时
        private float fTimeFadeOut = 0.0f;
        private Vector3 vScale = Vector3.one;
        private Color cColor = Color.white;

        public void InitChainLight(Config.ConfigSkillEffectUnit cfg, Vector3 pos, Quaternion rot,
            MapObject owner, MapObject playerStart, MapObject playerEnd, float timeStart, float timeEnd)
        {
            base.InitByConfig(cfg, pos, rot, owner);

            this.targetBone = cfg.TargetBone;
            this.actorOwner = owner as MapObject;
            this.fTimeFadeOut = Utils.GetFloat(cfg.TypeData);

            this.fTimeCur = 0.0f;
            this.fTimeStart = timeStart;
            this.fTimeEnd = timeEnd;
            this.actorStart = playerStart != null ? playerStart : this.actorOwner;
            this.actorEnd = playerEnd;

            if (this.actorEnd != null && !this.actorEnd.Recycled)
            {
                this.posEnd = this.actorEnd.DummyPos(this.targetBone);
            }

            if (actorStart != null)
            {
                SetPosition(actorStart.DummyPos(this.targetBone), true);
            }

            vScale.Set(cfg.Size * ConstDefine.CONFIG_PLACE, 1, 0);
            SetScale(vScale);

            SetTick(true);
        }

        public override void Destroyed()
        {
            base.Destroyed();

            cColor.a = 1.0f;
            SetTintColor(cColor);
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);

            if (this.fTimeCur > this.fTimeEnd + this.fTimeFadeOut || this.actorStart == null || this.actorEnd == null)
            {
                EndPlay();
                return;
            }

            this.fTimeCur += deltaSeconds;

            if (this.fTimeCur <= this.fTimeEnd)
            {
                // 伸长阶段
                // 计算闪电长度
                Vector3 delta = Vector3.zero;

                if (!this.actorEnd.Recycled)
                {
                    delta = this.actorEnd.DummyPos(this.targetBone) - this.position;
                }
                else
                {
                    delta = this.posEnd - this.position;
                }

                float fPass = Mathf.Max(this.fTimeCur - fTimeStart, 0);
                float fPlace = Mathf.Max(fTimeEnd - fTimeStart, 0.001f);

                vScale.Set(vScale.x, vScale.y, fPass / fPlace * delta.magnitude);
                SetScale(vScale);

                SetRotation(Quaternion.LookRotation(delta), true);
            }
            else if (this.fTimeFadeOut > 0)
            {
                // 淡出阶段
                float fPass = Mathf.Max(this.fTimeCur - this.fTimeEnd, 0);
                cColor.a = 1.0f - Mathf.Clamp01(fPass / this.fTimeFadeOut);
                SetTintColor(cColor);
            }
        }

        public override void EndPlay()
        {
            base.EndPlay();
        }
    }
}