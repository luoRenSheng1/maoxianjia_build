using System.Collections.Generic;
using UnityEngine;
using EngineBase;

namespace Engine
{
    public class MapDelayLightFxObject : MapFxObject
    {
        protected string bone;

        /// <summary>
        /// 延迟激光
        /// </summary>
        private Vector3 targetPos;
        private MapObject actorOwner;
        private MapObject actorTarget;
        private MapFxObject fxFollowObject;

        private Vector3 timerParam = Vector3.zero;
        private float fFollowSpeed = 10.0f;
        private int stageIndex = 0;
        private CTimer timerStage = new CTimer();
        private CTimer timerHit = new CTimer();
        private Vector2 rectCenter = Vector2.zero;
        private Vector2 rectSize = Vector2.zero;

        public override void Destroyed()
        {
            if (fxFollowObject != null)
            {
                fxFollowObject.EndPlay();
                fxFollowObject = null;
            }

            this.stageIndex = 0;
            this.actorOwner = null;
            this.actorTarget = null;

            base.Destroyed();
        }

        public void InitDelayLight(Config.ConfigSkillEffectUnit cfg, Vector3 pos, Quaternion rot, MapObject owner,
            MapObject target, Vector3 tarPos)
        {
            base.InitByConfig(cfg, pos, rot, owner);

            this.bone = cfg.Bone;
            this.actorOwner = owner;
            this.actorTarget = target;
            this.targetPos = this.actorTarget != null ? this.actorTarget.Position : tarPos;

            string[] strLines = cfg.TypeData.Split(';');

            if (strLines != null && strLines.Length >= 4)
            {
                timerParam.x = Utils.GetInt(strLines, 0) * ConstDefine.CONFIG_PLACE_TIME;
                timerParam.y = Utils.GetInt(strLines, 1) * ConstDefine.CONFIG_PLACE_TIME;
                timerParam.z = Utils.GetInt(strLines, 2) * ConstDefine.CONFIG_PLACE_TIME;

                var followId = Utils.GetInt(strLines, 3);

                /*if (followId != 0)
                {
                    fxFollowObject = MapObjectManager.Instance.SpawnFxActor(followId, this.Position, Quaternion.identity);

                    var cfgFollow = ConfigUtils.GetSkillEffectsById(followId);

                    if (cfgFollow != null)
                    {
                        fFollowSpeed = cfgFollow.Speed * ConstDefine.CONFIG_PLACE_TIME;
                    }
                }*/

                var param = Utils.GetInt(strLines, 4) * ConstDefine.CONFIG_PLACE_TIME;
                timerHit.Startup(param);

                rectCenter.x = Utils.GetInt(strLines, 5) * ConstDefine.CONFIG_PLACE_TIME;
                rectCenter.y = Utils.GetInt(strLines, 6) * ConstDefine.CONFIG_PLACE_TIME;
                rectSize.x = Utils.GetInt(strLines, 7) * ConstDefine.CONFIG_PLACE_TIME;
                rectSize.y = Utils.GetInt(strLines, 8) * ConstDefine.CONFIG_PLACE_TIME;
            }

            this.SetTick(true);

            stageIndex = 0;
            OnRunStage();
        }

        private void OnRunStage()
        {
            if (stageIndex == 0)
            {
                timerStage.Startup(timerParam.x);
            }
            else if (stageIndex == 1)
            {
                timerStage.Startup(timerParam.y);
            }
            else if (stageIndex == 2)
            {
                if (fxFollowObject != null)
                {
                    fxFollowObject.SetActive(false);
                }

                timerStage.Startup(timerParam.z);
            }
            else if (stageIndex > 2)
            {
                // EndAction已经提前派发了很多次。
                this.EndAction = null;
                this.EndPlay();
            }
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);

            if (timerStage.TimeOver())
            {
                ++stageIndex;
                OnRunStage();
            }

            if (stageIndex == 0 && this.actorOwner != null)
            {
                Vector3 posTarget;

                if (actorTarget == null || actorTarget.Recycled)
                {
                    // 目标已死亡
                    posTarget = targetPos;
                }
                else
                {
                    posTarget = actorTarget.Position;
                }

                if (fxFollowObject != null)
                {
                    Vector3 pos = Vector3.MoveTowards(fxFollowObject.Position, posTarget, deltaSeconds * this.fFollowSpeed);
                    fxFollowObject.SetPosition(pos, true);
                }

                Vector3 forward = posTarget - this.actorOwner.Position;
                forward.y = 0;
                var rotNow = Quaternion.LookRotation(forward);
                this.actorOwner.SetRotation(rotNow, true);

                this.SetPosition(this.actorOwner.DummyPos(bone), true);
                this.SetRotation(rotNow, true);
            }

            if (stageIndex == 2 && timerHit.ToNextTime() && EndAction != null)
            {
                if (actorTarget != null && !actorTarget.Recycled && !actorTarget.IsDead && this.actorOwner != null)
                {
                    Vector3 posCircle = this.actorTarget.Position - this.actorOwner.Position;
                    posCircle = MathUtils.InverseTransformDirectionMath(this.actorOwner.Rotation, posCircle);
                    //Quaternion rotTmp = Quaternion.Euler(0, -this.actorOwner.Rotation.eulerAngles.y, 0);
                    //posCircle = rotTmp * posCircle;
                    var posCircleV2 = new Vector2(posCircle.x, posCircle.z);
                    if (MathUtils.AreRectangleCircleIntersection(posCircleV2, actorTarget.Radius, rectCenter, rectSize))
                    {
                        EndAction();
                    }
                }
            }
        }
    }
}
