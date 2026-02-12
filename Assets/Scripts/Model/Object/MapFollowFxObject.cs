using UnityEngine;
using Engine;

namespace Engine
{
    public class MapFollowFxObject : MapFxObject
    {
        protected string bone;

        /// <summary>
        /// 跟随特效
        /// </summary>
        private MapObject actorOwner = null;

        public void InitFollow(Config.ConfigSkillEffectUnit cfg, Vector3 pos, Quaternion rot,
            MapObject owner)
        {
            base.InitByConfig(cfg, pos, rot, owner);

            this.bone = cfg.Bone;

            this.actorOwner = owner;

            SetTick(true);
        }

        public override void Destroyed()
        {
            base.Destroyed();
            this.actorOwner = null;
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);

            if (this.actorOwner == null || this.actorOwner.Recycled)
            {
                this.SetActive(false);
            }
            else
            {
                if (this.actorOwner != null)
                {
                    if (this.Active != this.actorOwner.Active)
                    {
                        this.SetActive(this.actorOwner.Active);
                    }

                    if (this.Active)
                    {
                        this.SetPosition(this.actorOwner.DummyPos(this.bone), true);
                    }
                }
            }
        }
    }
}