using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;
using EngineBase;
using msg;

namespace Engine
{
    public class MapMonsterObjectAttr : MapMoveObjectAttr
    {
        public MonsterVo MonsterVo;
        public ConfigMonsterUnit MonsterUnit { get; private set; }
        public void Init(MonsterVo vo)
        {
            if (vo == null)
            {
                return;
            }

            this.MonsterVo = vo;
            this.ObjectTypeID = vo.generalsType;
            this.Camp = EN_CAMP_TYPE.ENEMY;
            MonsterUnit = ConfigUtils.GetMonsterById(this.ObjectTypeID);

            if (MonsterUnit != null)
            {
                InitByAttrID(MonsterUnit.AtkSpeed, MonsterUnit.Speed);
                
                this.skillNormalID = MonsterUnit.AtkSkill;
            }
        }
    }

    /// <summary>
    /// 地图怪物显示操作类
    /// </summary>
    public class MapMonsterObject : MapMoveObject
    {
        protected Dictionary<int, float> m_mapSkillCastTime = new Dictionary<int, float>(); // 下一次技能允许释放的时间
        private MapFxObject _deathFx;
        public override MapObjectType ObjectType
        {
            get { return MapObjectType.Monster; }
        }

        protected override MapObjectAttr CreateNewAttr()
        {
            return new MapMonsterObjectAttr();
        }

        public MapMonsterObjectAttr MonsterAttr
        {
            get { return this.Attr as MapMonsterObjectAttr; }
        }

        public MapMonsterObject()
        {
            IsNeedSyncHP = true;
            MoveSpeed = ConstDefine.DEFAULT_MOVE_SPEED * Random.Range(0.8f, 1.2f);
        }

        public override void TriggerSetGameObject()
        {
            int monsterSize = 100;
            if (MonsterAttr.MonsterUnit.Size != 0)
                monsterSize = MonsterAttr.MonsterUnit.Size;
            float size = monsterSize * ConstDefine.CONFIG_PLACE * ConstDefine.MODEL_FX_SCALE;
            this.Scale = new Vector3(size, size, 1);
            
            base.TriggerSetGameObject();
            // 怪物血条位置
            UIContainerHP.xy = new Vector2(80, MonsterAttr.MonsterUnit.HpShifting -  monsterSize/100f);
            if (this.MonsterAttr.MonsterVo.IsBoss)
                UIContainerHP.bossType.selectedIndex = 2;
            else
            {
                UIContainerHP.bossType.selectedIndex = 1;
            }
        }

        protected float RecordCastSkillTime(int nSkillID, float attackTime)
        {
            var skillType = ConfigUtils.GetSkillById(nSkillID);

            if (skillType == null)
            {
                return 0.0f;
            }

            float fCDTime = 0.0f;

            if (skillType.SkillType == (int)EN_SKILL_TYPE.NORMAL)
            {
                fCDTime = 1 / this.Attr.AtkSpeed/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed);
            }
            else
            {
                fCDTime = skillType.Cd * ConstDefine.CONFIG_PLACE_EX/ this.Attr.AtkSpeed/MapObjectManager.Instance.Speed;
                fCDTime += attackTime;
            }

            float fTime = RealTime.time + fCDTime;

            if (m_mapSkillCastTime.ContainsKey(nSkillID))
            {
                m_mapSkillCastTime[nSkillID] = fTime;
            }
            else
            {
                m_mapSkillCastTime.Add(nSkillID, fTime);
            }

            return fCDTime;
        }

        public float GetCastSkillTime(int nSkillID)
        {
            float value;
            return m_mapSkillCastTime.TryGetValue(nSkillID, out value) ? value : 0.0f;
        }

        public bool IsCanCastSkillByCDTime(int nSkillID)
        {
            float fTime = GetCastSkillTime(nSkillID);

            if (Mathf.Approximately(fTime, 0.0f))
            {
                return true;
            }

            return RealTime.time >= fTime;
        }
        
        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);
            
            if (isPause)
                return;
            
            AutoAttack();
        }
        
        private bool AutoAttack()
        {
            if (DungeonMapManager.Instance.IsInCopy && DungeonMapManager.Instance.GuanKaStep != EN_GUANKA_STEP.FIGHTING)
            {
                return false;
            }
            if (!DungeonMapManager.Instance.IsInCopy && MapObjectManager.Instance.GuanKaStep != EN_GUANKA_STEP.FIGHTING)
            {
                return false;
            }
            
            if (IsDead)
            {
                return false;
            }

            if (IsInMove())
            {
                return false;
            }
            
            if (IsInPlaySkill())
            {
                return false;
            }
            
            var skillID = GetIdleSkill();

            if (skillID == 0)
            {
                return false;
            }
            
            CastSkill(skillID);

            return false;
        }
        
        public int GetIdleSkill()
        {
            if (IsCanCastSkillByCDTime(this.Attr.skillNormalID))
            {
                return this.Attr.skillNormalID;
            }

            return 0;
        }
        
        private void CastSkill(int skillID)
        {
            var TargetAttackRole = MapObjectManager.Instance.GetLocalHero();
            
            if (TargetAttackRole == null)
            {
                return;
            }

            var skillType = ConfigUtils.GetSkillById(skillID);

            if (skillType == null)
            {
                return;
            }

            float distance = Mathf.Abs(this.Position.x - TargetAttackRole.Position.x);//Utils.DistanceIgnoreZ(this.Position, TargetAttackRole.Position);

            if (distance > skillType.Radius)
            {
                // 施法距离不够，继续行进
                var lstPos = new List<Vector3>();
                lstPos.Add(this.Position);
                var forward = TargetAttackRole.Position.x > this.Position.x ? 1 : -1;
                lstPos.Add(new Vector3(this.Position.x + this.MoveSpeed * 0.1f * forward, this.Position.y, 0));
                this.UpdateMoveByPath(lstPos, null, null);
                return;
            }
            
            if (!MapObjectManager.Instance.battleSign)
            {
                return;
            }
            
            CastSkillVo vo = new CastSkillVo();
            
            vo.unitID = this.id;
            vo.skillID = skillID;
            vo.lstTarget = new List<UnitDamageVo>();
            
            // 主目标 及附属目标

            List<MapObject> lstTarget;
            Vector3 posAttack = Vector3.zero;
            
            MapObjectManager.Instance.GetSkillTarget(this, TargetAttackRole, skillID, out lstTarget, out posAttack);
            
            // 攻击伤害
            foreach (var target in lstTarget)
            {
                CalcDamageByCastSkill(vo, target, CastSkillType.Monster);
            }
            // 反击伤害
            CalcCounterDamageByCastSkill(vo, lstTarget);
            
            //计算buff
            AddBuff(vo);
            
            var skillTime = this.PlaySkill(vo, posAttack, this.Attr.AtkSpeed);
            if (skillType.SkillType == (int)EN_SKILL_TYPE.NORMAL)
            {
                // 普通攻击 cd时间不含攻击时间 补上攻击时间
                this.RecordCastSkillTime(skillID, skillTime);
            }
            else
            {
                // 技能攻击 cd时间含攻击时间
                this.RecordCastSkillTime(skillID, 0.0f);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_HERO_CASTSKILL, skillID, this.Attr.Camp);
        }
        
        public override void PlayDead(MapMoveObject attack = null, ConfigSkillUnit cfgSkill = null)
        {
            base.PlayDead(attack, cfgSkill);

            if (_deathFx != null)
                ClearDeathFx();
            _deathFx = MapObjectManager.Instance.SpawnFxActor(1001, this.Position, this.Rotation, this,Vector3.zero, null, 0, ClearDeathFx);

            GameManager.Instance.TimerManager.SetTimer(1.0f, ClearDeathFx);
            if (MapObjectManager.Instance.IsMonsterBoss && !MapObjectManager.Instance.fightLose && !DungeonMapManager.Instance.IsInCopy)
            {
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.STAGE_COMPLETE_INFO, GetRootGlobalPos());
            }
            else
            {
                if (MapObjectManager.Instance.ChapterIndex >= 0)
                {
                    var chapterUnit = ConfigUtils.GetChapterUnitById(MapObjectManager.Instance.ChapterIndex);
                    if (chapterUnit != null)
                    {
                        var stageId = MapObjectManager.Instance.GuanKaStageId;
                        ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIdAndNode(stageId, MapObjectManager.Instance.GuanKaMonsterIndex);
                        if (!DungeonMapManager.Instance.IsInCopy)
                        {
                            //杀怪的奖励
                            var builderA = Stage_AwardCommit_CS.CreateBuilder();
                            builderA.NodeId = (uint) stageUnit.Node;
                            builderA.MonsterId = MonsterAttr.MonsterVo.generalsType;
                            builderA.MonsterIdx = MonsterAttr.MonsterVo.MonsterIndex;
                            builderA.PosX = GetRootGlobalPos().x;
                            builderA.PosY = GetRootGlobalPos().y;
                            builderA.KilledBySkillId = 0;
                            builderA.KilledByPetGuid = 0;
                            
                            if (attack != null && cfgSkill != null)
                            {
                                if (attack is MapHeroObject)
                                {
                                    builderA.KilledByPet = attack.ObjectType == MapObjectType.Hero?false:true;
                                    builderA.KilledByAtk = true;
                                    if (attack.ObjectType == MapObjectType.Pet)
                                    {
                                        builderA.KilledByPetGuid = (attack as MapPetObject).PetAttr.petItemInfo.PetGuid;
                                    }
                                }else if (attack is MapHeroSkillProxy)
                                {
                                    builderA.KilledByPet = false;
                                    builderA.KilledByAtk = false;
                                    builderA.KilledBySkillId = (uint)attack.Attr.skillXPID;
                                }else if (attack is MapSkillProxy)
                                {
                                    if (BuffInfoManager.Instance.IsPetSkill(attack, cfgSkill))
                                    {   //宠物初始技能
                                        builderA.KilledByPet = true;
                                        builderA.KilledByPetGuid = attack.Attr.PetGuid;
                                    }
                                    else
                                    {   //角色抽卡技能
                                        builderA.KilledByPet = false;
                                    }
                                    builderA.KilledBySkillId = (uint)attack.Attr.skillXPID;
                                    builderA.KilledByAtk = false;
                                }
                            }
                            
                            Stage_AwardCommit_CS awardCommitCs = builderA.Build();
                            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Stage_AwardCommit_CS, awardCommitCs);
                        }
                    }

                }
 
            }
        }

        private void ClearDeathFx()
        {
            if (_deathFx != null && !_deathFx.Recycled)
            {
                // Debug.Log("ClearDeathFx:" + _deathFx.id);
                MapObjectManager.Instance.DestroyActor(_deathFx);
                _deathFx = null;
            }
        }
    }
}