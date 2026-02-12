using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using System;
using System.Linq;
using System.Text;
using Common;
using Config;
using Spine.Unity;
using EngineBase;
using FairyGUI;
using Google.Protobuf.Collections;
using msg;
using Spine;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Engine
{
    public enum CastSkillType
    {
        None,
        Hero,
        Monster,
        SkillProxy
    }
    /// <summary>
    /// 地图物件数据
    /// </summary>
    public class MapMoveObjectAttr : MapObjectAttr
    {
        public override void Clear()
        {
            base.Clear();
        }
    }

    // 可移动物件（根据移动路径移动）
    public class MapMoveObject : MapObject
    {
        public float MoveSpeed { get; set; }

        /// <summary>
        /// 移动
        /// </summary>
        protected List<Vector3> lstMovePathPos;

        protected List<Quaternion> lstMovePathRot;
        protected int movePathFrame = 0; // 更新路径时的帧号，下一帧才开始行军
        protected System.Action<int> actionMoveDone;
        
        //技能buff
        public Dictionary<int, System.Action> hitCallbackDic = new Dictionary<int, Action>();
        public float buffCount = 0; //buff持续次数
        public float InternalTime = 0; //回调时间
        public float timeRate = 1; //回调频率
        public int atkCount = 0; //普攻次数
        public static int skillAtkCount = 0; //技能攻击次数
        public double isRoleHurtReduced = 0; //减少伤害
        public int hurtCount = 0;  //受伤次数
        public bool nextNormalAtk = false; //下次普通攻击触发
        public double petSkillRecover = 0; //宠物技能给于玩家被攻击时的恢复
        
        /// <summary>
        /// 暂停
        /// </summary>
        public bool isPause = false;
        
        /// <summary>
        /// 战斗
        /// </summary>
        protected MapObjectSkillVo curSkill = new MapObjectSkillVo();
        // protected List<MapObjectStateVo> lstStates;

        protected MapFxObject _hitEffect;
        public MapMoveObjectAttr MoveAttr
        {
            get { return this.Attr as MapMoveObjectAttr; }
            set { this.Attr = value; }
        }

        public MapMoveObject()
        {
        }

        public override void Destroyed()
        {
            PlayAlive();

            if (_hitEffect != null && !_hitEffect.Recycled)
            {
                MapObjectManager.Instance.DestroyActor(_hitEffect);
                _hitEffect = null;
            }
            // ClearState();

            foreach (var key in hitCallbackDic.Keys.ToList())
            {
                hitCallbackDic[key] = null;
            }
            hitCallbackDic.Clear();
            buffCount = 0;
            timeRate = 1;
            foreach (var loader3D in HitSpineDic)
            {
                loader3D.Value.visible = false;
            }
            isPause = false;
            if (animComp != null)
            {
                animComp.timeScale = 1;
            }
            IsBuffInvincible = false;
            BounceValue = 0;
            lifeShieldCount = 0;
            lifeShieldList.Clear();
            lifeShieldTime = 0;
            if (this is MapHeroObject)
            {
                atkCount = 0;
                skillAtkCount = 0;
                hurtCount = 0;
                nextNormalAtk = false;
                petSkillRecover = 0;
            }
            
            base.Destroyed();
        }

        public override void TriggerSetGameObject()
        {
            base.TriggerSetGameObject();

            ForceUpdateAnimator();
        }

        public override void SyncObjectFromAttr()
        {
            base.SyncObjectFromAttr();

            if (this.MoveAttr != null && !Mathf.Approximately(this.MoveAttr.Speed, 0.0f))
            {
                this.MoveSpeed = this.MoveAttr.Speed;
            }
        }

        protected virtual void CalcTransformByTick(float deltaSeconds)
        {
            //TODO 移动
            if (movePathFrame == Time.frameCount || lstMovePathPos == null || lstMovePathPos.Count <= 0)
            {
                return;
            }

            float moveDistance = deltaSeconds * MoveSpeed;

            while (lstMovePathPos.Count > 0)
            {
                Vector3 posTarget = lstMovePathPos[0];
                Vector3 forward = posTarget - this.Position;
                Vector3 posCur = Vector3.MoveTowards(this.Position, posTarget, moveDistance);

                if (lstMovePathRot.Count > 0)
                {
                    Quaternion rot = lstMovePathRot[0];
                    SetTransform(posCur, rot, false);
                }
                else
                {
                    Quaternion rot = Quaternion.identity;
                    if(forward != Vector3.zero)
                        rot= Quaternion.LookRotation(forward);
                    SetTransform(posCur, rot, false);
                }

                if (posCur.Equals(posTarget))
                {    
                    // 超过目标点，继续往下个节点移动
                    RemoveMovePathNode();
                    moveDistance -= forward.magnitude;
                    continue;
                }
                else
                {
                    break;
                }
            }

            if (lstMovePathPos == null || lstMovePathPos.Count <= 0)
            {
                MovePathDone();
            }
        }

        public override void SetTransform(Vector3 pos, Quaternion rot, bool bSyncGo)
        {
            position = pos;
            rotation = rot;

            if (bSyncGo)
            {
                if (UIContainerRoot != null)
                {
                    UIContainerRoot.position = pos;
                }

                try
                {                
                    if (animComp != null)
                    {
                        animComp.skeleton.ScaleX = rot.eulerAngles.x > 180.0f ? -1.0f : 1.0f;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
        }

        public override void SetRotation(Quaternion rot, bool bSyncGo)
        {
            rotation = rot;

            if (bSyncGo)
            {
                if (animComp != null)
                {
                    animComp.skeleton.ScaleX = rot.eulerAngles.y > 180.0f ? -1.0f : 1.0f;
                }
            }
        }

        public virtual void SetVelocity(float velocity, float vX, float vY)
        {
            //TODO 播放移动动作
            var animator = GetAnimator();

            if (animator != null)
            {
                var bRun = velocity >= ConstDefine.VELOCITY_RUN_LIMIT_PLACE;
                var strName = GetCurAnimatorName();

                if (bRun && !strName.Equals("run"))
                {
                    if (this is MapMonsterObject)
                    {
                        // TrackEntry trackEntry = animator.AnimationState.SetAnimation(0, "run", false);
                        // trackEntry.AnimationStart = Random.Range(0f, 0.5f);
                        // animator.AnimationState.AddAnimation(0, "run", true, 0);
                        animComp.AnimationState.SetAnimation(0, "run", true);
                    }
                    else
                    {
                        animComp.AnimationState.SetAnimation(0, "run", true);
                    }
                }
                else if (!bRun && !strName.Equals("idle"))
                {
                    animator.AnimationState.SetAnimation(0, "idle", true);
                }
            }
        }

        public virtual void ForceUpdateAnimator()
        {
            if (IsDead)
            {
                PlayDead(this);
            }
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);
            
            // 计算到达的位置
            CalcTransformByTick(deltaSeconds);

            if (this.gameObj != null)
            {
                var bPlaySkill = IsInPlaySkill();
                
                // 施法中只同步位置
                Vector3 posCur = GetGoPosition();
                Quaternion rotCur = GetGoRotation();
                Vector3 delta = Vector3.zero;

                if (!this.Position.Equals(posCur))
                {
                    delta = Position - posCur;
                    SetPosition(this.Position, true);
                }

                if (!bPlaySkill && !MapObjectManager.Instance.isHeroPlayNormalSkill && !MapObjectManager.Instance.isHeroPlayXPSkill)
                {
                    if (!this.Rotation.Equals(rotCur))
                    {
                        SetRotation(this.Rotation, true);
                    }

                    Vector3 velocity = (delta / deltaSeconds) / ConstDefine.VELOCITY_RUN_LIMIT;
                    float velocityX = 0.0f;
                    float velocityY = velocity.magnitude;
                    this.SetVelocity(velocity.magnitude, velocityX, velocityY);
                }
                
                InternalTime += deltaSeconds;
                if (InternalTime >= timeRate)
                {
                    var keys = hitCallbackDic.Keys.ToList();
                    for(int i = 0; i < keys.Count; i++)
                    {
                        hitCallbackDic[keys[i]]?.Invoke();
                    }
                    InternalTime = 0;
                }
            }
            
        }

        public bool IsInMove()
        {
            return lstMovePathPos != null && lstMovePathPos.Count > 0;
        }

        /// <summary>
        /// 根据现有路径移动
        /// </summary>
        /// <param name="curNode"></param>
        /// <param name="lstPath"></param>
        /// <param name="moveDone"></param>
        public void UpdateMoveByPath(List<Vector3> lstPathPos, List<Quaternion> lstPathRot, System.Action<int> moveDone)
        {
            this.DoUpdateMovePath(lstPathPos, lstPathRot, moveDone);
        }

        protected virtual void DoUpdateMovePath(List<Vector3> lstPathPos, List<Quaternion> lstPathRot, System.Action<int> moveDone)
        {
            this.actionMoveDone = moveDone;

            if (lstMovePathPos == null)
            {
                lstMovePathPos = new List<Vector3>();
            }

            if (lstMovePathRot == null)
            {
                lstMovePathRot = new List<Quaternion>();
            }

            if (lstMovePathPos != null)
            {
                lstMovePathPos.Clear();
            }

            if (lstMovePathRot != null)
            {
                lstMovePathRot.Clear();
            }

            if (lstPathPos != null)
            {
                for (int i = 0; i < lstPathPos.Count; i++)
                {
                    lstMovePathPos.Add(lstPathPos[i]);
                }
            }

            if (lstPathRot != null)
            {
                for (int i = 0; i < lstPathRot.Count; i++)
                {
                    lstMovePathRot.Add(lstPathRot[i]);
                }
            }

            // 去除起点
            RemoveMovePathNode();

            movePathFrame = Time.frameCount;
        }

        protected virtual void MovePathDone()
        {
            // 防止回调中又触发移动导致回调异常清除的问题
            var actionDone = this.actionMoveDone;

            this.DoClearMovePath();

            if (actionDone != null)
            {
                actionDone(this.id);
                actionDone = null;
            }
        }

        protected void RemoveMovePathNode()
        {
            if (lstMovePathPos.Count > 0)
            {
                lstMovePathPos.RemoveAt(0);
            }

            if (lstMovePathRot.Count > 0)
            {
                lstMovePathRot.RemoveAt(0);
            }
        }

        public virtual void ClearMovePath()
        {
            this.DoClearMovePath();
        }

        protected virtual void DoClearMovePath()
        {
            if (lstMovePathPos != null)
            {
                lstMovePathPos.Clear();
            }

            if (lstMovePathRot != null)
            {
                lstMovePathRot.Clear();
            }

            this.movePathFrame = 0;
            this.actionMoveDone = null;
        }

        public virtual void UpdatePos(Vector3 pos, Quaternion rot)
        {
            this.DoClearMovePath();

            SetTransform(pos, rot, false);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="attrIndex">属性索引</param>
        /// <param name="AttrValue">衰减的属性值</param>
        public void SetMoveAttrValue(int attrIndex,int attrValue)
        {
            switch ((eBattleAttr)attrIndex)
                {
                    // case eBattleAttr.eBattleAttr_Atk:
                    case eBattleAttr.eBattleAttr_FinalAttack:
                        this.MoveAttr.Atk = this.MoveAttr.Atk * (1 - attrValue * ConstDefine.CONFIG_PLACE_EX);
                        break;
                    case eBattleAttr.eBattleAttr_HP:
                        this.MoveAttr.HP = this.MoveAttr.HP * (1 - attrValue * ConstDefine.CONFIG_PLACE_EX);
                        break;
                    case eBattleAttr.eBattleAttr_HP_Recovery:
                        this.MoveAttr.Recovery = this.MoveAttr.Recovery * (1 - attrValue * ConstDefine.CONFIG_PLACE_EX);
                        break;
                    case eBattleAttr.eBattleAttr_AtkSpeed_Rate:
                        this.MoveAttr.AtkSpeed = this.MoveAttr.AtkSpeed * (1 - attrValue * ConstDefine.CONFIG_PLACE_EX);
                        break;
                }
        }
        
        #region 战斗相关

        private void SetTimer(int sourceid, float time, Action callback)
        {
            if (sourceid == 0)
            {
                GameManager.Instance.TimerManager.SetTimer(time, callback);
            }
            else
            {
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.BATTLE, sourceid, time, callback);
            }
        }

        private void ClearTimer(int sourceid, Action callback)
        {
            if (sourceid == 0)
            {
                GameManager.Instance.TimerManager.ClearTimer(callback);
            }
            else
            {
                GameManager.Instance.TimerManager.ClearTimerBySourceID(EN_TIMER_SOURCE.BATTLE, sourceid);
            }
        }
        
        public bool IsInPlaySkill()
        {
            return curSkill.skillID != 0;
        }

        /// TODO 播放技能表现
        public virtual float PlaySkill(CastSkillVo vo, Vector3 posAttackEffect, float atkSpeed)
        {
            Config.ConfigSkillUnit cfgSkill = ConfigUtils.GetSkillById(vo.skillID);

            if (cfgSkill == null)
            {
                LogUtils.LogWarningFormat("PlaySkill Error, id {0} SkillID {1}", id, vo.skillID);
                return 0.0f;
            }

            // End Last Skill
            EndCurSkill();

            // LookAt target
            if (vo.lstTarget.Count > 0)
            {
                var target = vo.lstTarget[0];
                var objTarget = MapObjectManager.Instance.GetMapObjectById(target.targetID);

                if (objTarget != null && objTarget.Position != this.Position)
                {
                    Quaternion rotForwardd = Quaternion.LookRotation(objTarget.Position - this.Position);
                    SetRotation(rotForwardd, true);
                }
            }

            curSkill.battleActionID = vo.battleActionID;
            curSkill.skillID = cfgSkill.Id;
            curSkill.aniName = cfgSkill.SkillAction;
            curSkill.AtkSpeed = (cfgSkill.SkillType == (int) EN_SKILL_TYPE.NORMAL) ? Mathf.Max(1, atkSpeed) : 1f;
            
            if (cfgSkill.SkillType == (int) EN_SKILL_TYPE.XP)
            {
                var cfgSkillEffect = ConfigUtils.GetSkillEffectById(cfgSkill.Id);
                if (MapObjectManager.Instance.isMeleeHero() && cfgSkillEffect != null && cfgSkillEffect.Path == "")  //近战英雄 技能特效做在模型身上
                {
                    MapObjectManager.Instance.GetLocalHero().AnimatorTrigger(cfgSkill.SkillAction, true);
                }
                else
                {
                    this.AnimatorTrigger(cfgSkill.SkillAction);
                }
            }
            else
            {
                this.AnimatorTrigger(cfgSkill.SkillAction);
            }
            PlaySkillSound(cfgSkill);
            
            // CP Effect
            Vector3 posCP = this.Position;
            Quaternion rotCP = this.Rotation;

            foreach (var FxCp in cfgSkill.CasterEffect)
            {
                if (FxCp != 0)
                {
                    MapFxObject fx = MapObjectManager.Instance.SpawnFxActor(FxCp, posCP, rotCP, this, posCP, null);
                    curSkill.AddFx(fx, false);
                }
            }

            // TODO 攻击特效 AttackEffect  播放特效 技能表现  普工表现
            if (cfgSkill.AttackEffect != 0)
            {
                if (cfgSkill.AttackTime == 0)
                {
                    PlayFxAttack(cfgSkill, posAttackEffect, Quaternion.identity);  // 技能表现
                }
                else
                {
                    this.SetTimer(vo.battleActionID, cfgSkill.AttackTime * ConstDefine.CONFIG_PLACE_TIME,
                        () => { PlayFxAttack(cfgSkill, posAttackEffect, Quaternion.identity); });
                }
            }

            // Track Effect  // todo 受击 轨迹 路径 Track Effect
            PlayFxTrack(cfgSkill, vo, posAttackEffect);
            for (int i = 0; i < cfgSkill.HitTime.Count; i++)  //攻击受伤飘伤害值表现 延迟时间 （技能播放时间）
            {
                // Hit Effect
                if (cfgSkill.HitTime[i] == 0)
                {
                    PlayHitTrack(cfgSkill, vo.lstTarget, this);
                }
                else
                {
                    this.SetTimer(vo.battleActionID, cfgSkill.HitTime[i] * ConstDefine.CONFIG_PLACE_TIME/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed),
                        () => { PlayHitTrack(cfgSkill, vo.lstTarget, this); });
                }
            }
            
            // End Action
            float time = GetCurAnimatorTime()/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed);
            // Debug.LogWarningFormat("时间:time={0}  SKILLID={1}", time, cfgSkill.Name);
            if (cfgSkill.SkillType == (int) EN_SKILL_TYPE.NORMAL)
            {
                float aniTime = GetCurAnimatorTime() / MapObjectManager.Instance.Speed;
                this.SetTimer(vo.battleActionID, aniTime, () =>
                {
                    if (!string.IsNullOrEmpty(curSkill.aniName))
                    {
                        AnimatorStopTrigger(curSkill.aniName);
                    }
                });

            }
            this.SetTimer(vo.battleActionID, time, this.EndCurSkill);
            return time;
        }

        private void PlayFxAttack(Config.ConfigSkillUnit cfgSkillClient, Vector3 pos, Quaternion rot)
        {
            if (cfgSkillClient == null || cfgSkillClient.AttackEffect == 0)
            {
                return;
            }

            bool bLoopEffect = ConfigUtils.IsSkillEffectLoop(cfgSkillClient.AttackEffect);
            MapFxObject fx =
                MapObjectManager.Instance.SpawnFxActor(cfgSkillClient.AttackEffect, pos, rot, this, pos, null);
            curSkill.AddFx(fx, bLoopEffect);
        }

        public void PlayFxTrack(Config.ConfigSkillUnit cfgSkillClient, CastSkillVo vo, Vector3 posTarget)
        {
            if (cfgSkillClient == null || cfgSkillClient.BulletEffect == 0)
            {
                return;
            }

            float fTimeFly = cfgSkillClient.BulletFlyTime * ConstDefine.CONFIG_PLACE_TIME/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed);
            float fTimeTrack = cfgSkillClient.BulletTime * ConstDefine.CONFIG_PLACE_TIME/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed);

            this.SetTimer(vo.battleActionID, fTimeTrack, () =>
            {
                foreach (var item in vo.lstTarget)
                {
                    if (item.skillID == 0)
                    {
                        continue;
                    }
                    
                    var skill = ConfigUtils.GetSkillById(item.skillID);

                    if (skill == null)
                    {
                        continue;
                    }

                    var skillTarget = ConfigUtils.GetSkillTargetById(skill.SkillTarget);

                    if (skillTarget == null)
                    {
                        continue;
                    }
                    
                    if (skillTarget.RangeType == (int)EN_TARGET_RANGE_TYPE.PARAM)
                    {
                        // 目标为参数范围，弹道目标位置为固定位置不需要跟随目标
                        MapObjectManager.Instance.SpawnFxActor(cfgSkillClient.BulletEffect,
                            this.Position, this.Rotation, this, posTarget, null, fTimeFly);
                    }
                    else
                    {
                        // 弹道目标需要跟随目标
                        var objTarget = MapObjectManager.Instance.GetMapObjectById(item.targetID);

                        if (objTarget != null)
                        {
                            MapObjectManager.Instance.SpawnFxActor(cfgSkillClient.BulletEffect,
                                this.Position, this.Rotation, this, objTarget.Position, objTarget, fTimeFly);
                        }
                    }
                }
            });
        }

        // TODO 收到攻击 飘血表现 同步伤害
        public void PlayHitTrack(Config.ConfigSkillUnit cfgSkill, List<UnitDamageVo> damages, MapMoveObject baseAttack)
        {
            if ( cfgSkill.HitTime.Count > 1 && (cfgSkill.SkillType == (int) EN_SKILL_TYPE.XP || cfgSkill.SkillType == (int) EN_SKILL_TYPE.ChoukaSkill) )
            {
                var skillTarget = ConfigUtils.GetSkillTargetById(cfgSkill.SkillTarget);
                List<MapObject> lstEnemy = MapObjectManager.Instance.GetAllEnemy(baseAttack, baseAttack.Position, skillTarget.RangeParam);
                //没在里面的怪物  重新添加进去
                foreach (var obj in lstEnemy)
                {
                    UnitDamageVo vo = null;
                    foreach (var damage in damages)
                    {
                        if (obj.id == damage.targetID)
                        {
                            vo = damage;
                        }
                    }
                    if (vo == null)
                    {
                        var damage1 = new UnitDamageVo();
                        damage1.unitID = baseAttack.id;
                        damage1.targetID = obj.id;
                        damage1.skillID = cfgSkill.Id;
                        damage1.damage = MapObjectManager.Instance.GetBaseDamage(cfgSkill.Id , this, obj, CastSkillType.Hero);
                        damages.Add(damage1);// 受击
                    }
                }
            }
            
            foreach (var damage in damages)
            {
                // 普通飘血
                var target = MapObjectManager.Instance.GetMapMoveObjectById(damage.targetID);
                if (target != null && !(target is MapPetObject))
                {
                    if (target is MapHeroObject heroObject)
                    {
                        if (!heroObject.IsInvincible)
                        {
                            target.SyncDamage(damage, baseAttack);
                        }
                    }
                    else
                    {
                        target?.SyncDamage(damage, baseAttack);
                    }
                    
                }
                
                // 特殊提示
                if (damage.damageType == EN_DAMAGE_TYPE.COUNTER || damage.damageType == EN_DAMAGE_TYPE.COMBO)
                {
                    // 连击/反击提示
                    var attack = MapObjectManager.Instance.GetMapMoveObjectById(damage.unitID);

                    if (attack != null)
                    {
                        attack.SyncAttackTip(damage);
                    }
                }
            }
        }

        private void PlaySkillSound(ConfigSkillUnit skillUnit)
        {
            if(skillUnit.StartSound > 0 && !VillageInfoManager.Instance.IsInVillageHome)
                GameManager.Instance.SoundManager.PlayEffectWithoutLoop(skillUnit.StartSound);
            
            // Debug.LogWarningFormat("技能音效----{0}",skillUnit.StartSound);
        }
        
        /// <summary>
        /// 受击飘血等
        /// </summary>
        /// <param name="damage"></param>
        public virtual void SyncDamage(UnitDamageVo damage, MapMoveObject baseAttack = null)
        {
            ConfigSkillUnit cfgSkill = null;
            if (damage.skillID > 0)
            {
                cfgSkill = ConfigUtils.GetSkillById(damage.skillID);
            }
            damage.damage = damage.damage < 1 ? 1 : damage.damage;
            
            MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
            if (damage.damageType == EN_DAMAGE_TYPE.RECOVERY)
            {
                if (this is MapHeroObject || this is MapSkillProxy || this is MapHeroSkillProxy || (objTarget != null && objTarget is MapHeroObject))
                {
                    MapObjectManager.Instance.GetLocalHero().Attr.HP += damage.damage;
                    MapObjectManager.Instance.GetLocalHero().Attr.HP = Math.Min(MapObjectManager.Instance.GetLocalHero().Attr.HPMax, MapObjectManager.Instance.GetLocalHero().Attr.HP);
                }
                else
                {
                    this.Attr.HP += damage.damage;
                    this.Attr.HP = Math.Min(this.Attr.HPMax, this.Attr.HP);
                }
            }
            else
            {
                if (cfgSkill != null)
                {   //被攻击时
                    BuffInfoManager.Instance.ShowHitTargetBuff(baseAttack, objTarget,cfgSkill, damage,  skillAtkCount);
                }
                
                if (objTarget != null && objTarget.lifeShieldCount > 0)  //有护盾先扣除护盾血量
                {
                    double valueToSubtract = damage.damage;
                    for (int i = objTarget.lifeShieldList.Count - 1; i >= 0; i--)
                    {
                        valueToSubtract -= objTarget.lifeShieldList[i];
                        if (valueToSubtract > 0)
                            objTarget.lifeShieldList[i] = 0;
                        else
                        {
                            objTarget.lifeShieldList[i] = Math.Abs((int)valueToSubtract);
                            valueToSubtract = 0;
                            break;
                        }
                    }
                    if (valueToSubtract != 0)
                    {
                        this.Attr.HP -= valueToSubtract;
                    }
                    damage.damageType = EN_DAMAGE_TYPE.LifeShield;
                }else
                    this.Attr.HP -= damage.damage;
            }
            
            if (IsNeedSyncHP)
            {
                if (damage.damageType == EN_DAMAGE_TYPE.RECOVERY && (this is MapHeroObject || this is MapSkillProxy || this is MapHeroSkillProxy || (objTarget != null && objTarget is MapHeroObject) ) )
                {
                    if (MapObjectManager.Instance.GetLocalHero().UIContainerHP != null)
                    {
                        MapObjectManager.Instance.GetLocalHero().UIContainerHP.UpdateUI(MapObjectManager.Instance.GetLocalHero().Attr, true);
                        MapObjectManager.Instance.GetLocalHero().UIContainerHP.DamageFly(MapObjectManager.Instance.GetLocalHero(), damage);
                    }
                }
                else
                {
                    if (this is MapMonsterObject && DungeonMapManager.Instance.IsInCopy)
                    {
                        UIContainerHP.DamageFly(this, damage);
                        ((UI_BossBlood) MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("copyBlood")).UpdateMonsterHP(this.Attr, true);
                    }
                    else if (UIContainerHP != null)
                    {
                        UIContainerHP.UpdateUI(this.Attr, true);
                        UIContainerHP.DamageFly(this, damage);
                    }
                }
            }
            
            // 受击
            if (damage.damage > 0 && damage.skillID != 0)
            {
                PlayHit();
            }
            
            if (cfgSkill != null && cfgSkill.HitEffect != 0)
            {
                if (cfgSkill.SkillType == (int)EN_SKILL_TYPE.NORMAL && !VillageInfoManager.Instance.IsInVillageHome)
                    GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralAttackSE);
                if(_hitEffect != null)
                    MapObjectManager.Instance.DestroyActor(_hitEffect);
                _hitEffect = MapObjectManager.Instance.SpawnFxActor(cfgSkill.HitEffect, this.Position, this.Rotation, this);
            }
            
            if (this.Attr.HP<= 0 && !IsDead)
            {
                if (cfgSkill != null)
                {   // 死亡 是否触发buff
                    BuffInfoManager.Instance.ShowHitTargetDeadBuff(baseAttack, cfgSkill, damage);
                }
            }
            
            if (this.Attr.HP<= 0 && !IsDead)
            {
                if (objTarget != null && (objTarget.Attr.debuffDemageValue != 0 || objTarget.Attr.debuffAtkSpeedValue != 0) && MapObjectManager.Instance.GetLocalHero() != null)
                {   // 怪物生成到死亡 一直显示的buff
                    MapMoveObject attack = MapObjectManager.Instance.GetLocalHero();
                    if (objTarget.Attr.debuffDemageValue != 0)
                    {
                        DataManager.Instance.GetRoleData().FightAttrVo.Atk += objTarget.Attr.debuffDemageValue;
                        objTarget.Attr.debuffDemageValue = 0;
                        if (!BuffInfoManager.Instance.GetEnemyCount((int)EN_BUFF_TYPE.AtkReduce))
                        {
                            if (attack.HitSpineDic.TryGetValue((int)EN_BUFF_TYPE.AtkReduce, out GLoader3D load3D))
                            {
                                load3D.visible = false;
                                load3D.playing = false;
                            }
                        }
                    }
                    else if (objTarget.Attr.debuffAtkSpeedValue != 0)
                    {
                        DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed += objTarget.Attr.debuffAtkSpeedValue;
                        objTarget.Attr.debuffAtkSpeedValue = 0;
                        if (!BuffInfoManager.Instance.GetEnemyCount((int)EN_BUFF_TYPE.SpeedReduce))
                        {
                            if (attack.HitSpineDic.TryGetValue((int)EN_BUFF_TYPE.SpeedReduce, out GLoader3D load3D))
                            {
                                load3D.visible = false;
                                load3D.playing = false;
                                attack.HitSpineDic[(int)EN_BUFF_TYPE.SpeedReduce1].visible = false;
                                attack.HitSpineDic[(int)EN_BUFF_TYPE.SpeedReduce1].playing = false;
                            }
                        }
                    }
                }
                
                //击杀回复
                if (baseAttack != null && baseAttack is MapHeroObject && baseAttack.ObjectType == MapObjectType.Hero)
                {
                    if (MapObjectManager.Instance.GetLocalHero() != null && DataManager.Instance.GetRoleData().FightAttrVo.KilledRecovery > 0)
                    {
                        MapObjectManager.Instance.GetLocalHero().PlayRecovery(MapObjectManager.Instance.GetLocalHero(), DataManager.Instance.GetRoleData().FightAttrVo.KilledRecovery);
                    }
                }
                
                PlayDead(baseAttack, cfgSkill);
                MapObjectManager.Instance.DestroyMapObjectById(this.id);
            }
            else if (this.Attr.HP > 0 && IsDead)
            {
                PlayAlive();
            }
        }
        
        /// <summary>
        /// 攻击提示
        /// </summary>
        /// <param name="damage"></param>
        public virtual void SyncAttackTip(UnitDamageVo damage)
        {
            if (IsNeedSyncHP)
            {
                if (UIContainerHP != null)
                {
                    UIContainerHP.DamageFlyAttack(this, damage.damageType);
                }
            }
        }
        
        /// <summary>
        /// 生命恢复
        /// </summary>
        /// <returns></returns>
        public virtual void PlayRecovery(MapMoveObject attack = null, double value = 0)
        {
            if (attack == null)
            {
                if (this.Attr.Recovery <= 0 || this.IsDead)
                {
                    return;
                }
            }
            
            var damage = new UnitDamageVo();
            damage.unitID = attack == null ? this.id : attack.id;
            damage.targetID = attack == null ? this.id : attack.id;
            damage.skillID = 0;
            damage.damage = value == 0 ? this.Attr.Recovery : value;
            damage.damageType = EN_DAMAGE_TYPE.RECOVERY;
            if (attack != null)
            {
                if (attack is MapMonsterObject && BuffInfoManager.Instance.TriggerRoleHoly(attack, (int)EN_BUFF_TYPE.AddBlood) != 0)
                {
                    damage.damage -= damage.damage * BuffInfoManager.Instance.TriggerRoleHoly(attack, (int)EN_BUFF_TYPE.AddBlood) * ConstDefine.CONFIG_PLACE_EX;
                }
                attack.SyncDamage(damage);
            }
            else
            {
                if (this is MapMonsterObject && BuffInfoManager.Instance.TriggerRoleHoly(this, (int)EN_BUFF_TYPE.AddBlood) != 0)
                {
                    damage.damage -= damage.damage * BuffInfoManager.Instance.TriggerRoleHoly(this, (int)EN_BUFF_TYPE.AddBlood) * ConstDefine.CONFIG_PLACE_EX;
                }
                this.SyncDamage(damage);
            }
            
            
            MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
            if (objTarget != null)
            {
                GLoader3D load3D = objTarget.HitSpineDic[(int)EN_BUFF_TYPE.AddBlood];
                load3D.url = "ui://Common/Common_13001_buff";
                load3D.frame = 0;
                load3D.loop = false;
                load3D.visible = true;
                load3D.SetXY(0, -150);
                load3D.parent.SetChildIndex(load3D, 100); //显示最上层
                
                Utils.PlaySpineAnim(load3D, "Common_13001_buff", false, () => { load3D.visible = false; });
            }
        }
        
        public virtual void UpdateHpBar()
        {
            if (IsNeedSyncHP)
            {
                if (UIContainerHP != null)
                {
                    UIContainerHP.UpdateUI(this.Attr, false);
                    if (this is MapMonsterObject && DungeonMapManager.Instance.IsInCopy)
                    {
                        ((UI_BossBlood) MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("copyBlood")).UpdateMonsterHP(this.Attr, false);
                    }
                }
            }
        }
        
        public virtual void EndCurSkill()
        {
            if (curSkill.skillID == 0)
            {
                return;
            }
            
            GameManager.Instance.TimerManager.ClearTimer(this.EndCurSkill);

            // 循环光效技能结束销毁，瞬时光效由TimerManager控制生命周期
            if (curSkill.lstFx != null)
            {
                foreach (var item in curSkill.lstFx)
                {
                    if (item.fx != null)
                    {
                        item.fx.EndPlay();
                    }
                }

                curSkill.lstFx.Clear();
            }

            if (!string.IsNullOrEmpty(curSkill.aniName))
            {
                AnimatorStopTrigger(curSkill.aniName);
            }

            if (curSkill.battleActionID != 0)
            {
                ClearTimer(curSkill.battleActionID, null);
            }
            
            curSkill.Reset();
            
            
        }

        protected void CalcDamageByCastSkill(CastSkillVo vo, MapObject targetRole, CastSkillType type)
        {
            if (vo == null || targetRole == null)
            {
                return;
            }

            int targetID = targetRole.id;
            double targetHP = targetRole.Attr.HP;

            var damage = new UnitDamageVo();
            damage.unitID = vo.unitID;
            damage.targetID = targetID;
            damage.skillID = vo.skillID;

            //是否闪避  普攻才生效
            ConfigSkillUnit skillUnitData = ConfigUtils.GetSkillById(vo.skillID);
            if (skillUnitData.SkillType == (int)EN_SKILL_TYPE.NORMAL) //普通攻击
            {
                var isJouk = MapObjectManager.Instance.IsJoukDamage(this, targetRole);
                if (isJouk)
                {
                    damage.damage = 0;
                    damage.damageType = EN_DAMAGE_TYPE.Jouk;
                    damage.hp = targetHP;
                    vo.lstTarget.Add(damage);
                    return;
                }
            }
            
            // todo 是否暴击
            var bCritical = MapObjectManager.Instance.IsCriticalDamage(vo.skillID, this, targetRole);
            if (!bCritical)
            {
                damage.damage = MapObjectManager.Instance.GetBaseDamage(vo.skillID, this, targetRole, type);
                damage.damageType = EN_DAMAGE_TYPE.NORMAL;
                if (targetRole.IsParry)  //是否格挡
                {
                    damage.damageType = EN_DAMAGE_TYPE.Parry;
                    targetRole.IsParry = false;
                }
            }
            else
            {
                damage.damage = MapObjectManager.Instance.GetCriticalDamage(vo.skillID, this, targetRole, type);
                damage.damageType = EN_DAMAGE_TYPE.STRIKE;
                if (targetRole.IsParry)
                {
                    damage.damageType = EN_DAMAGE_TYPE.Parry;
                    targetRole.IsParry = false;
                }
            }

            targetHP = targetHP - damage.damage;
            damage.hp = targetHP;
            vo.lstTarget.Add(damage);

            // 是否触发连击
            var bCombo = MapObjectManager.Instance.IsComboDamage(this, targetRole);
            if (bCombo)
            {
                var damage1 = new UnitDamageVo();
                damage1.unitID = vo.unitID;
                damage1.targetID = targetID;
                damage1.skillID = vo.skillID;
                damage1.damage = MapObjectManager.Instance.GetComboDamage(this, targetRole);
                damage1.damageType = EN_DAMAGE_TYPE.COMBO;
                targetHP = targetHP - damage1.damage;
                damage1.hp = targetHP;
                vo.lstTarget.Add(damage1);
            }
            
            // 是否触发吸血
            var isBlood = MapObjectManager.Instance.IsBloodDamage(this, targetRole);
            if (isBlood)
            {
                int magicTimes = Mathf.Max(1, (int) this.Attr.MagicTimes);
                ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(vo.skillID);
                if (skillUnit.SkillType == (int) EN_SKILL_TYPE.XP ||
                    skillUnit.SkillType == (int) EN_SKILL_TYPE.ChoukaSkill) //技能伤害
                {
                    magicTimes = RuneInfoManager.Instance.GetSkillTimes(vo.skillID);
                }
                var damage2 = new UnitDamageVo();
                damage2.unitID = this.id;
                damage2.targetID = (type == CastSkillType.Hero || type==CastSkillType.SkillProxy) ? MapObjectManager.Instance.LocakHeroId : this.id;
                damage2.skillID = 0;
                if (magicTimes > 1)
                {
                    damage2.damage = damage.damage/magicTimes * this.Attr.Bloodsucking;
                }
                else
                {
                    damage2.damage = damage.damage * this.Attr.Bloodsucking;
                }
                damage2.damageType = EN_DAMAGE_TYPE.RECOVERY;
                vo.lstTarget.Add(damage2);
            }
        }
        
        protected void CalcCounterDamageByCastSkill(CastSkillVo vo, List<MapObject> targetRole)
        {
            if (vo == null || targetRole == null)
            {
                return;
            }

            double targetHP = this.Attr.HP;

            foreach (var target in targetRole)
            {
                // 是否反击
                var bCounter = MapObjectManager.Instance.IsCounterDamage(target, this);

                if (bCounter)
                {
                    var damage = new UnitDamageVo();
                    damage.unitID = target.id;
                    damage.targetID = vo.unitID;
                    damage.skillID = 0;
                    damage.damage = MapObjectManager.Instance.GetCounterDamage(target, this);
                    damage.damageType = EN_DAMAGE_TYPE.COUNTER;
                    targetHP = targetHP - damage.damage;
                    damage.hp = targetHP;
                    vo.lstTarget.Add(damage);
                }
            }
        }

        public static float GetTimeNow()
        {
            return Time.unscaledTime;
        }

        /*
        public MapObjectStateVo GetState(int id)
        {
            if (lstStates != null)
            {
                for (int i = 0; i < lstStates.Count; ++i)
                {
                    MapObjectStateVo curState = lstStates[i];

                    if (curState != null && curState.id == id)
                    {
                        return curState;
                    }
                }
            }

            return null;
        }

        public void AddState(int id, int cfgid, int conRound)
        {
            if (lstStates == null)
            {
                lstStates = new List<MapObjectStateVo>();
            }

            MapObjectStateVo curState = GetState(id);

            if (curState != null)
            {
                curState.conRound = conRound;
                return;
            }

            MapObjectStateVo info = new MapObjectStateVo();

            info.id = id;
            info.cfgId = cfgid;
            info.conRound = conRound;

            lstStates.Add(info);

            OnAddState(info);
        }

        public void RemoveState(int id)
        {
            if (lstStates != null)
            {
                for (int i = 0; i < lstStates.Count; ++i)
                {
                    MapObjectStateVo curState = lstStates[i];

                    if (curState != null && curState.id == id)
                    {
                        lstStates.RemoveAt(i);
                        OnRemoveState(curState);
                        break;
                    }
                }
            }
        }

        public void SyncStateTrigger(int id)
        {
            MapObjectStateVo curState = GetState(id);

            if (curState == null)
            {
                return;
            }

            OnTriggerState(curState);
        }

        public void ClearState()
        {
            if (lstStates != null)
            {
                for (int i = 0; i < lstStates.Count; ++i)
                {
                    MapObjectStateVo curState = lstStates[i];

                    if (curState != null)
                    {
                        OnRemoveState(curState);
                    }
                }

                lstStates.Clear();
            }
        }

        private void OnAddState(MapObjectStateVo info)
        {
            if (info == null)
            {
                return;
            }

            var cfg = ConfigUtils.GetSkillStatusClientById(info.cfgId);

            if (cfg != null)
            {
                int effectID = cfg.Effect;
                int soundID = cfg.Sound;

                if (effectID != 0)
                {
                    var fx = MapObjectManager.Instance.SpawnFxActor(effectID, this.Position, this.Rotation, this);

                    if (fx != null)
                    {
                        bool bLoopEffect = ConfigUtils.IsSkillEffectLoop(cfg.Effect);
                        info.fx = bLoopEffect ? fx : null;
                    }
                }

                if (soundID != 0 && this.IsInCameraViewReal())
                {
                    SoundEffect sound = SoundManager.Instance.PlayEffect(soundID);

                    if (sound != null && sound.bLoop)
                    {
                        info.sound = sound;
                        info.bLoopSound = true;
                    }
                }
            }
        }

        private void OnRemoveState(MapObjectStateVo info)
        {
            if (info == null)
            {
                return;
            }

            if (info.fx != null)
            {
                info.fx.EndPlay();
                info.fx = null;
            }

            if (info.bLoopSound)
            {
                SoundManager.Instance.StopEffect(info.sound);
                info.sound = null;
            }

            var cfg = ConfigUtils.GetSkillStatusClientById((int)info.cfgId);

            if (cfg != null)
            {
                if (cfg.EffectEnd != 0)
                {
                    MapObjectManager.Instance.SpawnFxActor(cfg.EffectEnd, this.Position, this.Rotation, this);
                }

                if (cfg.SoundEnd != 0 && this.IsInCameraViewReal())
                {
                    SoundManager.Instance.PlayEffect(cfg.SoundEnd, bForceNotLoop: true);
                }
            }
        }

        private void OnTriggerState(MapObjectStateVo info)
        {
            if (info == null)
            {
                return;
            }

            var cfg = ConfigUtils.GetSkillStatusClientById(info.cfgId);

            if (cfg != null && cfg.ActivateEffect != 0)
            {
                MapObjectManager.Instance.SpawnFxActor(cfg.ActivateEffect, this.Position, this.Rotation, this);
            }
        }

        public void OnTriggerRoundEnd()
        {
            if (this.lstStates != null)
            {
                foreach (var curState in this.lstStates)
                {
                    if (curState != null)
                    {
                        curState.conRound -= 1;
                    }
                }
            }
        }
        */

        public virtual void PlayAlive()
        {
            this.IsDead = false;
        }

        public virtual void PlayDead(MapMoveObject attack = null, ConfigSkillUnit cfgSkill = null)
        {
            this.ClearMovePath();
            this.EndCurSkill();

            this.IsDead = true;
        }
        
        public virtual void PlayHit()
        {
        }
        
        /// <summary>
        /// 动画对象 播放速度设置
        /// </summary>
        /// <param name="timeScale"></param>
        public void SetSkeletonAnimationTimeScale(float timeScale)
        {
            if (animComp != null)
            {
                animComp.timeScale = timeScale;
            }
        }
        
        public void SetObjectAnimCompPause(MapMoveObject obj)
        {
            if (obj != null && obj.animComp != null)
            {
                obj.animComp.timeScale = 0; //暂停
                obj.animComp.AnimationState.GetCurrent(0).TrackTime = 0; //回到第一帧
            }
        }
        
        public void SetObjectAnimComp(MapMoveObject obj)
        {
            if (obj != null && obj.animComp != null)
            {
                obj.animComp.timeScale = 1;
            }
        }
        
        #region buff 相关
        /// <summary>
        /// 移除buff效果
        /// </summary>
        public void ResetBuff()
        {
            skillAtkCount = 0;
            nextNormalAtk = false;
            MapObjectManager.Instance.GetLocalHero().atkCount = 0;
            MapObjectManager.Instance.GetLocalHero().hurtCount = 0;
            MapObjectManager.Instance.GetLocalHero().petSkillRecover = 0;
        }
        
        /// <summary>
        /// 技能是否携带buff
        /// </summary>
        /// <param name="vo"></param>
        /// <param name="attack"></param>
        protected void AddBuff(CastSkillVo vo)
        {
            ConfigSkillUnit cfgSkill = ConfigUtils.GetSkillById(vo.skillID);
            if (cfgSkill == null)
            {
                LogUtils.LogWarningFormat("AddBuff Error, SkillID {1}", vo.skillID);
                return ;
            }
            
            for (int i = 0; i < cfgSkill.HitTime.Count; i++)
            {
                if (cfgSkill.HitTime[i] == 0)
                {
                    //受击表现
                    SetAttackBuff(cfgSkill, vo);
                    break;
                }
                else
                {
                    if (i == 0)
                    {
                        //受击表现
                        this.SetTimer(vo.battleActionID, cfgSkill.HitTime[i] * ConstDefine.CONFIG_PLACE_TIME/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed),
                            () => { SetAttackBuff(cfgSkill, vo); });
                        break;
                    }
                }
            }
        }

        private void SetAttackBuff(ConfigSkillUnit cfgSkill, CastSkillVo vo)
        {
            if ((this is MapHeroSkillProxy || this is MapSkillProxy) && MapObjectManager.Instance.GetLocalHero() != null)
            {
                //统计角色技能释放的次数
                skillAtkCount++;
            }

            BuffInfoManager.Instance.ShowAttackBuff(cfgSkill, vo, this,skillAtkCount);
        }
        
        #endregion

        #endregion

        #region 有限状态机

        // FSM
        protected StateMachine m_fsm;

        protected void InitFSM()
        {
            if (this.gameObj != null)
            {
                m_fsm = StateMachine.Initialize(States.Idle, this);
            }
        }

        private void IdleEnter()
        {
            EnterState(States.Idle);
        }

        private void IdleExit(object state)
        {
            ExitState(States.Idle, (States)state);
        }

        private void IdleUpdate()
        {
            UpdateState(States.Idle);
        }

        private void RunEnter()
        {
            EnterState(States.Run);
        }

        private void RunExit(object state)
        {
            ExitState(States.Run, (States)state);
        }

        private void RunUpdate()
        {
            UpdateState(States.Run);
        }

        private void AttackEnter()
        {
            EnterState(States.Attack);
        }

        private void AttackExit(object state)
        {
            ExitState(States.Attack, (States)state);
        }

        private void AttackUpdate()
        {
            UpdateState(States.Attack);
        }

        private void PatrolEnter()
        {
            EnterState(States.Patrol);
        }

        private void PatrolExit(object state)
        {
            ExitState(States.Patrol, (States)state);
        }

        private void PatrolUpdate()
        {
            UpdateState(States.Patrol);
        }

        private void DieEnter()
        {
            EnterState(States.Die);
        }

        private void DieExit(object state)
        {
            ExitState(States.Die, (States)state);
        }

        private void DieUpdate()
        {
            UpdateState(States.Die);
        }

        protected virtual void EnterState(States state)
        {
            // PlayAniByState();

            switch (state)
            {
                case States.Die:
                    break;

                case States.Attack:
                    break;

                case States.Idle:
                    break;

                default:
                    break;
            }
        }

        protected virtual void ExitState(States state, States stateNext)
        {
            switch (state)
            {
                case States.Run:
                    break;

                default:
                    break;
            }
        }

        protected virtual void UpdateState(States state)
        {
            switch (state)
            {
                case States.Run:
                    break;

                case States.Idle:
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}