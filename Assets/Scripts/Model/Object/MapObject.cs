using System;
using FairyGUI;
using System.Collections.Generic;
using System.Linq;
using Common;
using CommonEx;
using Config;
using Lobby;
using Spine.Unity;
using UnityEngine;
using Engine;

namespace Engine
{
    public enum MapObjectType
    {
        None = 0,
        Hero = 1,               // 英雄
        Monster = 2,            // NPC怪物
        Pet = 3,                // 宠物
        
        ITEM = 101,             // 道具图标

        VirtualFx = 2000010000, // 虚拟物件-光效
    }
    
    /// <summary>
    /// 地图物件数据
    /// </summary>
    public class MapObjectAttr
    {
        /// <summary>
        /// 角色唯一id
        /// </summary>
        public int unitID { get; set; }
        
        /// <summary>
		/// 宠物唯一id
        /// </summary>
        public ulong PetGuid { get; set; }
        
        /// <summary>
        /// 角色类型ID
        /// </summary>
        public int ObjectTypeID { get; set; }

        /// <summary>
        /// 角色阵营
        /// </summary>
        public EN_CAMP_TYPE Camp { get; set; } = EN_CAMP_TYPE.NEUTRALITY;
        
        #region 属性
        
        /// <summary>
        /// 移动速度
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 攻击
        /// </summary>
        public double Atk { get; set; }

        /// <summary>
        /// 生命
        /// </summary>
        public double HP { get; set; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        public double HPMax{ get; set; }
        

        /// <summary>
        /// 攻速
        /// </summary>
        public float AtkSpeed {get;set; }
        
        /// <summary>
        /// 暴击比率（概率）暴击率
        /// </summary>
        public double CriticalStrike { get; set; }

        /// <summary>
        /// 连击
        /// </summary>
        public float ComboAtk { get; set; }

        /// <summary>
        /// 反击
        /// </summary>
        public float CounterAtk { get; set; }

        /// <summary>
        /// 恢复
        /// </summary>
        public double Recovery { get; set; }

        /// <summary>
        /// 爆伤比率（爆伤的伤害倍数）  暴击伤害百分比
        /// </summary>
        public double CriticalInjury { get; set; }

        /// <summary>
        /// BOSS伤害加成
        /// </summary>
        public float BossDamageAdd { get; set; }

        /// <summary>
        /// 小怪伤害加成
        /// </summary>
        public float MonsterDamageAdd { get; set; }

        /// <summary>
        /// 减伤
        /// </summary>
        public float Mitigation { get; set; }

        /// <summary>
        /// 吸血
        /// </summary>
        public float Bloodsucking { get; set; }
        

        /// <summary>
        /// 金币加成
        /// </summary>
        public float GoldAdd { get; set; }

        /// <summary>
        /// 技能冷却
        /// </summary>
        public float SkillCd { get; set; }
        public double SkillDamage { get; set; } // 技能伤害
        public float MagicTimes { get; set; }  // 技能伤害次数(倍数)
        public float MagicTimesAdd { get; set; }  // 技能伤害次数(倍数)加成比率
        
        /// <summary>
        /// 防御值  总防御力   需取服务器端下发的eBattleAttr_FinalDefence = 102 用作防御值
        /// </summary>
        public double Def { get; set; }
        
        /// <summary>
        /// 无视防御
        /// </summary>
        public double IgnoreDef { get; set; }
        /// <summary>
        /// 格挡率
        /// </summary>
        public double ParryRate { get; set; }
        /// <summary>
        /// 格挡值
        /// </summary>
        public double ParryValue { get; set; }
        /// <summary>
        /// 闪避率
        /// </summary>
        public double JoukRate { get; set; }
        /// <summary>
        /// 普通攻击命中率
        /// </summary>
        public double AtkHitRate { get; set; }
        
        /// <summary>
        /// 生命倍率
        /// </summary>
        public double HPMultiple { get; set; }
        /// <summary>
        /// 伤害倍率
        /// </summary>
        public double AtkMultiple { get; set; }
        
        /// <summary>
        /// 战斗最终伤害 ---- 还未投放，暂时为0
        /// </summary>
        public float BattleFinalAttack { get; set; }
        
        #endregion
        
        /// <summary>
        /// 普通攻击技能ID
        /// </summary>
        public int skillNormalID { get; set; }

        /// <summary>
        /// XP攻击技能ID
        /// </summary>
        public int skillXPID { get; set; }

        /// <summary>
        /// 地图活物状态
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 角色伤害类型
        /// </summary>
        public int HarmType { get; set; }
        
        /// <summary>
        /// buff列表 怪物专用
        /// </summary>
        public List<int> MonsterBuffs = new List<int>();
        
        /// <summary>
        /// 伤害提升
        /// </summary>
        public int DemageUpRate { get; set; }

        /// <summary>
        /// 扣除角色的伤害值,直到怪物死亡
        /// </summary>
        public double debuffDemageValue { get; set; }
        
        /// <summary>
        /// 扣除角色的攻击速度,直到怪物死亡
        /// </summary>
        public float debuffAtkSpeedValue { get; set; }
        
        public override string ToString()
        {
            return "atk=" + Atk + " HP=" + HP + " HPMax=" + HPMax;
        }

        public virtual void Clear()
        {
            this.ObjectTypeID = 0;
            this.Speed = 0;
    
            this.HP = 0;
            HPMax = 0;
            this.Atk = 0;
            this.Recovery = 0;
            this.CriticalStrike = 0;
            this.CriticalInjury = 0;
            this.BossDamageAdd = 0;
            this.MonsterDamageAdd = 0;
            this.Mitigation = 0;
            this.Bloodsucking = 0;
            this.GoldAdd = 0;
            this.SkillCd = 0;
            this.AtkSpeed = 0;
            this.ComboAtk = 0;
            this.CounterAtk = 0;

            this.Def = 0;
            this.IgnoreDef = 0;
            this.ParryRate = 0;
            this.ParryValue = 0;
            this.JoukRate = 0;
            this.AtkHitRate = 0;
            this.HPMultiple = 0;
            this.AtkMultiple = 0;
            this.debuffDemageValue = 0;
            this.debuffAtkSpeedValue = 0;
            this.MonsterBuffs.Clear();
        }
        
        public void InitByAttrID(int atkSpeed, float speed)
        {
            this.Speed = speed;
            this.AtkSpeed = atkSpeed * ConstDefine.CONFIG_PLACE_EX;

        }

        public void UpdateHp(double hp, double maxHp)
        {
            this.HP = hp;
            this.HPMax = maxHp;
        }

        public void AdditionalBuff(int skillId)
        {
            
        }

        /// <summary>
        /// 检查是否有指定的状态
        /// </summary>
        /// <param name="st">见ConstDefine文件UNIT_STATE枚举</param>
        /// <returns></returns>
        public bool CheckState(int st)
        {
            return (state & st) > 0;
        }
    }

    public class MapObjectBone
    {
        //public FName dummy;
        public Transform tran;
        public List<MapObject> bindActors;
    }

    public class MapObject
    {
        /// <summary>
        /// 是否已经打上待回收标记（true:需要回收，false：不需要回收）
        /// </summary>
        public bool IsTryRecycleSign { get; set; }

        /// <summary>
        /// 是否已回收
        /// </summary>
        public bool Recycled { get; set; }

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead { get; protected set; }

        /// <summary>
        /// 是否可以被攻击
        /// </summary>
        public bool CanFight { get { return !Recycled && !IsDead; } }

        /// <summary>
        /// 是否已回收
        /// </summary>
        public float RecycledTime { get; set; }

        /// <summary>
        /// 模型半径
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 Scale { get; protected set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Active { get; protected set; }

        /// <summary>
        /// 是否在相机视野中
        /// </summary>
        public bool InCameraView { get; protected set; }

        /// <summary>
        /// 是否是隐身状态
        /// </summary>
        public bool InInvisible { get; protected set; }

        /// <summary>
        /// 是否需要同步HP
        /// </summary>
        public bool IsNeedSyncHP { get; protected set; } = false;

        /// <summary>
        /// UI节点
        /// </summary>
        public GComponent UIContainerRoot { get; protected set; }
        /// <summary>
        /// UI节点-角色
        /// </summary>
        public GGraph UIContainerBody { get; protected set; }
        /// <summary>
        /// UI-血条
        /// </summary>
        public UI_BarHp UIContainerHP { get; protected set; }

        /// <summary>
        /// UI-3D 特效节点 受击流血冰冻等
        /// </summary>
        public Dictionary<int,GLoader3D> HitSpineDic = new Dictionary<int,GLoader3D>();
        
        /// <summary>
        /// 是否格挡
        /// </summary>
        public bool IsParry { get; set; }
        
        /// <summary>
        /// 是否无敌
        /// </summary>
        public bool IsBuffInvincible { get; set; }
        
        /// <summary>
        /// 是否反弹 伤害值
        /// </summary>
        public float BounceValue { get; set; }
        
        /// <summary>
        /// 护盾数量
        /// </summary>
        public int lifeShieldCount { get; set; }
        /// <summary>
        /// 护盾生命值
        /// </summary>
        public List<double> lifeShieldList = new List<double>();
        /// <summary>
        /// 护盾调用间隔
        /// </summary>
        public float lifeShieldTime { get; set; }
        
        protected SkeletonAnimation animComp;
        protected Rect rectAnim;
        protected Animator u3dAnimator;

        /// <summary>
        /// UI屏幕位置
        /// </summary>
        private Vector2 UIViewPos = Vector3.zero;
        private int UIViewPosFrameCount = 0;
        private Vector2 HeadUIViewPos = Vector3.zero;
        private int HeadUIViewPosFrameCount = 0;
        private Vector2 MiddleUIViewPos = Vector3.zero;
        private int MiddleUIViewPosFrameCount = 0;

        // 异步加载-序号，成功事件
        public int asyncLoadIndex = 0;

        /// <summary>
        /// 实体逻辑
        /// </summary>
        protected GameObject go;
        protected Vector3 position;
        protected Quaternion rotation;

        protected MapObject boneParentObj;
        protected string boneParentName;

        public string Name { get; set; }
        public int id { get; set; }
        public string playerId { get; set; }
        public object customData { get; set; }
        public MapObjectAttr Attr { get; set; }

        public virtual MapObjectType ObjectType
        {
            get { return MapObjectType.None; }
        }
        
        public Vector2 PositionV2
        {
            get { return new Vector2(position.x, position.z); }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
        }

        public GameObject gameObj
        {
            get { return this.go; }
        }

        /// <summary>
        /// 是否需要状态机
        /// </summary>
        public bool IsNeedStateControl = false;

        public MapObject()
        {
            id = 0;
            Recycled = true;
            Active = true;
            InCameraView = true;
            InInvisible = false;
            Scale = Vector3.one;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            customData = null;
        }

        public void BeginPlay(int id, string playerId, Vector3 pos, Quaternion rot, object data = null)
        {
            this.id = id;
            this.playerId = playerId;
            this.position = pos;
            this.rotation = rot;
            this.customData = data;

            OnBeginPlay();
        }

        /// <summary>
        /// 创建数据属性
        /// </summary>
        public void InitAttr()
        {
            if (this.Attr == null)
            {
                this.Attr = CreateNewAttr();
            }
        }

        protected virtual MapObjectAttr CreateNewAttr()
        {
            return new MapObjectAttr();
        }

        public virtual void Destroyed()
        {
            UnAttachBoneParent();

            /*if
            if (IsNeedStateControl)
            {
                UIClient.Instance.RemoveStateControl(this.id);
            }*/

            id = 0;
            Recycled = true;
            RecycledTime = Time.realtimeSinceStartup;
            Active = true;
            InCameraView = true;
            InInvisible = false;
            Scale = Vector3.one;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            customData = null;

            if (this.Attr != null)
            {
                this.Attr.Clear();
            }
            
            if (this.UIContainerRoot != null)
            {
                this.UIContainerRoot.visible = false;
                this.UIContainerRoot.RemoveFromParent();
                if (this.UIContainerRoot.container.gameObject != null)
                {
                    this.UIContainerRoot.container.gameObject.transform.SetParent(MapObjectManager.Instance.transRecycle);
                    this.UIContainerRoot.position = Vector3.zero;
                }
            }
            
            // if (UIContainerHP != null)
            // {
            //     MapObjectManager.Instance.Pool.ReturnObject(UIContainerHP);
            // }
            
        }

        public virtual void Dispose()
        {
            this.Destroyed();

            if (this.go != null)
            {
                GameObject.Destroy(this.go);
                this.go = null;
            }

            if (this.UIContainerRoot != null)
            {
                this.UIContainerRoot.Dispose();
                this.UIContainerRoot = null;
            }
        }

        protected virtual void OnBeginPlay()
        {
            Active = true;
            Recycled = false;
            RecycledTime = 0.0f;
            InCameraView = true;
            InInvisible = false;

            this.SyncObjectFromAttr();

            if (this.go != null)
            {
                this.TriggerSetGameObject();
            }
            else
            {
                if (this is MapFxObject && ConfigUtils.GetSkillEffectPathById((this as MapFxObject).EffectID) == "" )//近战英雄不需要 加载技能特效
                {
                }
                else
                {
                    MapObjectManager.Instance.LoadGameObject(this);
                }
            }
        }

        public virtual void SyncObjectFromAttr()
        {
        }

        public virtual void UpdateObject()
        {
            this.SyncObjectFromAttr();
        }

        public virtual void SetGameObject(GameObject go)
        {
            this.go = go;

            if (Recycled)
            {
                this.go.SetActive(false);
            }
            else
            {
                this.TriggerSetGameObject();
            }
        }

        public virtual void TriggerSetGameObject()
        {
            if (this.gameObj != null)
            {
                this.gameObj.transform.localPosition = new Vector3(80, 0, 0);//Vector3.zero;  //TODO 英雄初始位置
                this.gameObj.transform.localScale = Vector3.one;

                animComp = this.gameObj.GetComponentInChildren<SkeletonAnimation>();
                animComp?.Initialize(true);
                
                u3dAnimator = this.gameObj.GetComponentInChildren<Animator>();
                
                var objRoot = MapObjectManager.Instance.GetMapObjectSceneObjRootTrans();

                if (objRoot == null)
                {
                    LogUtils.LogWarning("TriggerSetGameObject Error!!!");
                    return;
                }

                // 初始化容器
                if (UIContainerRoot == null)
                {
                    UIContainerRoot = new GComponent();
                    objRoot.AddChild(UIContainerRoot);
                }
                else
                {
                    UIContainerRoot.RemoveFromParent();
                    objRoot.AddChild(UIContainerRoot);
                    UIContainerRoot.visible = true;
                }
                
#if UNITY_EDITOR
                UIContainerRoot.container.gameObject.name = string.Format("{0}_{1}", this.id, this.Name);
#endif
                
                if (UIContainerBody == null)
                {
                    UIContainerBody = new GGraph();
                    UIContainerRoot.AddChild(UIContainerBody);
                }
                else
                {
                    UIContainerBody.RemoveFromParent();
                    UIContainerRoot.AddChild(UIContainerBody);
                }
                
                foreach (int value in Enum.GetValues(typeof(EN_BUFF_TYPE)))
                {
                    GLoader3D UIHitSpine;
                    if (!HitSpineDic.TryGetValue(value, out UIHitSpine))
                    {
                        UIHitSpine = new GLoader3D();
                        UIHitSpine.name = value.ToString();
                        // loader3D.SetSize(400, 400); // 设置尺寸
                        // loader3D.fill = FillType.ScaleFree; // 填充策略（无/等比缩放等）
                        UIHitSpine.SetXY(22, -88);
                        // UIHitSpine.SetScale(1.9f, 1.9f);
                        UIContainerRoot.AddChild(UIHitSpine);
                        HitSpineDic.Add(value, UIHitSpine);
                    }
                    else
                    {
                        UIHitSpine = HitSpineDic[value]; 
                    }
                    UIHitSpine.visible = false;
                    UIHitSpine.loop = false;
                }
                
                GameObject goUIWrapper = this.go;
                
                var wrapper = UIContainerBody.displayObject as GoWrapper;

                if (wrapper != null)
                {
                    wrapper.wrapTarget = goUIWrapper;
                }
                else
                {
                    wrapper = new GoWrapper(goUIWrapper);
                    UIContainerBody.SetNativeObject(wrapper);
                }

                if (IsNeedSyncHP)
                {
                    if (UIContainerHP == null)
                    {
                        UIContainerHP = UIPackage.CreateObject("CommonEx","BarHp") as UI_BarHp; //MapObjectManager.Instance.Pool.GetObject("ui://CommonEx/BarHp") as UI_BarHp;
                        UIContainerHP.visible = true;
                        UIContainerRoot.AddChild(UIContainerHP);
                        UIContainerRoot.EnsureBoundsCorrect();
                    }
                    else
                    {
                        UIContainerHP.RemoveFromParent();
                        UIContainerRoot.AddChild(UIContainerHP);
                    }
                    
                    UIContainerHP.InitUI(Attr);
                    
                    if (this is MapMonsterObject && DungeonMapManager.Instance.IsInCopy)
                    {
                        UIContainerHP.visible = false;
                        ((UI_BossBlood) MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("copyBlood")).InitMonsterHP(this.Attr);
                    }
                }
                
                /*if (IsNeedStateControl)
                {
                    UIClient.Instance.CreateStateControl(this.id, this.ObjectType);
                }*/

                this.SetActive(this.Active);
                this.SetTransform(this.position, this.rotation, true);
                this.SetScale(this.Scale);
            }
        }

        public virtual void SetActive(bool value)
        {
            this.Active = value;
            Utils.SetActiveEx(this.go, this.Active);
        }
        
        protected virtual SkeletonAnimation GetAnimator()
        {
            return animComp;
        }
        
        protected virtual string GetCurAnimatorName()
        {
            var animator = GetAnimator();

            if (animator != null)
            {
                var currentAnimation = animator.AnimationState.GetCurrent(0);
                
                if (currentAnimation != null)
                {
                    return currentAnimation.Animation.Name;
                }
            }

            return "";
        }
        
        protected virtual float GetCurAnimatorTime()
        {
            var animator = GetAnimator();
            if (animator != null)
            {
                animator.timeScale = MapObjectManager.Instance.Speed;
                var currentAnimation = animator.AnimationState.GetCurrent(0);
                
                if (currentAnimation != null)
                {
                    return currentAnimation.Animation.Duration;
                }
            }

            return ConstDefine.DEFAULT_SKILL_TIME;
        }
        
        // 获取 Spine 动画时长（秒）
        public virtual float GetAnimatorTimeByName(string animationName)
        {
            var animator = GetAnimator();
            if (animator != null)
            {
                var skeletonData = animator.skeletonDataAsset.GetSkeletonData(true);
                var animation = skeletonData.FindAnimation(animationName);
                return animation.Duration;
            }
            return 0;
        }
        
        public virtual void AnimatorTrigger(string strAni, bool isXpSkill = false)
        {
            var animator = GetAnimator();

            if (animator != null && !string.IsNullOrEmpty(strAni))
            {
                animator.timeScale = 1;
                if (isXpSkill)
                {
                    animator.timeScale = GetCurAnimatorTime() / (MapObjectManager.Instance.Speed * (this.Attr.AtkSpeed <= 0 ? 1 : this.Attr.AtkSpeed));
                }
                animator.AnimationState.SetAnimation(0, strAni, false);
            }
        }

        public virtual void AnimatorStopTrigger(string strAni)
        {
            var animator = GetAnimator();

            if (animator != null && !string.IsNullOrEmpty(strAni))
            {
                animator.AnimationState.SetAnimation(0, "idle", true);
            }
        }

        public void StopAllAnimation()
        {
            var animator = GetAnimator();
            if (MapObjectManager.Instance.isMeleeHero())
            {
                animator = MapObjectManager.Instance.GetLocalHero().GetAnimator();
            }
            if (animator != null)
            {
                // 停止当前动画
                animator.AnimationState.ClearTracks();
                // 设置到初始姿势
                animator.Skeleton.SetToSetupPose();
        
                // 可选：立即播放一个空闲动画
                animator.AnimationState.SetAnimation(0, "idle", true);
            }
        }
        
        public virtual void AnimatorPlay(string stateName, bool loop, int aniScale)
        {
            var animator = GetAnimator();

            if (animator != null)
            {
                animator.AnimationState.SetAnimation(0, stateName, loop);
                animator.timeScale = aniScale;
            }

        }

        public virtual void U3DAnimatorPlay(int aniScale)
        {
            if(this.gameObj == null) return;
            u3dAnimator = this.gameObj.GetComponentInChildren<Animator>();
            if (u3dAnimator != null)
            {
                u3dAnimator.speed = aniScale * MapObjectManager.Instance.Speed;
            }
            ParticleSystem[] particleSystems = this.gameObj.GetComponentsInChildren<ParticleSystem>();
            foreach (var item in particleSystems)
            {
                var main = item.main;
                main.simulationSpeed = aniScale * MapObjectManager.Instance.Speed;
            }
        }

        public virtual void SetTransform(Vector3 pos, Quaternion rot, bool bSyncGo)
        {
            position = pos;
            rotation = rot;

            if (bSyncGo && UIContainerRoot != null)
            {
                UIContainerRoot.position = pos;
            }
        }

        public virtual void SetPosition(Vector3 pos, bool bSyncGo)
        {
            position = pos;
            
            if (bSyncGo && UIContainerRoot != null)
            {
                UIContainerRoot.position = pos;
            }
        }
        
        public virtual void SetRotation(Quaternion rot, bool bSyncGo)
        {
            rotation = rot;
            
            if (bSyncGo && UIContainerRoot != null)
            {
                UIContainerRoot.rotationV3 = rot;
            }
        }

        public Vector3 GetGoPosition()
        {
            if (UIContainerRoot != null)
            {
                return UIContainerRoot.position;
            }

            return Vector3.zero;
        }

        public Quaternion GetGoRotation()
        {
            if (UIContainerRoot != null)
            {
                return UIContainerRoot.rotationV3;
            }

            return Quaternion.identity;
        }
        
        public virtual void Tick(float deltaSeconds)
        {
        }

        public virtual void LateTick(float deltaSeconds)
        {
        }

        public Vector2 GetHeadUIPos()
        {
            var pos = Vector2.zero;
            
            if (UIContainerRoot != null)
            {
                pos = UIContainerRoot.xy;
            }

            if (UIContainerHP != null)
            {
                pos += UIContainerHP.xy;
            }

            return pos;
        }
        
        public Vector2 GetRootGlobalPos()
        {
            var pos = Vector2.zero;
            
            if (UIContainerRoot != null)
            {
                pos = UIContainerRoot.LocalToGlobal(Vector2.zero);
            }

            return pos;
        }
        
        /// <summary>
        /// 设置是否在视野中
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lod"></param>
        public virtual bool IsInCameraViewRect()
        {
            return false;//MapCameraManager.Instance.IsInCameraViewRect(this.Position);
        }

        /// <summary>
        /// 是否在视野中(真实计算),IsInCameraViewRect是估算,提高效率
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInCameraViewReal()
        {
            /*
            if (MapCameraManager.Instance.MainCamera != null)
            {
                Vector3 vPosView = MapCameraManager.Instance.MainCamera.WorldToViewportPoint(Position);

                if (vPosView.x >= 0.0f && vPosView.x <= 1.0f && vPosView.y >= 0.0f && vPosView.y <= 1.0f)
                {
                    return true;
                }
            }
            */
            return false;
        }

        /// <summary>
        /// 同步视野状态
        /// </summary>
        /// <param name="value"></param>
        public virtual void TickByCheckCameraView()
        {
            bool value = IsInCameraViewRect();

            if (this.InCameraView != value)
            {
                this.InCameraView = value;
                CheckActive();
            }
        }
        
        /// <summary>
        /// 检查隐身状态
        /// </summary>
        public virtual void CheckInvisible()
        {
        }

        /// <summary>
        /// 隐身状态
        /// </summary>
        public virtual void Invisible(bool value)
        {
            if (this.InInvisible != value)
            {
                this.InInvisible = value;
                CheckActive();
            }
        }

        public virtual void SetScale(Vector3 value)
        {
            this.Scale = value;
            Utils.SetScaleEx(this.go, value);
        }
        
        public virtual void SetScaleX(int valueX)
        {
            this.Scale = new Vector3(this.Scale.x * valueX, this.Scale.y);
            Utils.SetScaleEx(this.go, this.Scale);
        }

        public virtual void CheckActive()
        {
            // 主节点显示状态
            if (this.InCameraView && !this.InInvisible)
            {
                SetActive(true);
            }
            else
            {
                SetActive(false);
            }

            // UI节点显示状态
            if (UIContainerRoot != null && !UIContainerRoot.isDisposed)
            {
                if (this.InCameraView && !this.InInvisible)
                {
                    UIContainerRoot.visible = true;
                }
                else
                {
                    UIContainerRoot.visible = false;
                }
            }
        }

        public void AttachBoneParent(MapObject parent, string name)
        {
            this.boneParentObj = parent;
            this.boneParentName = name;
        }

        private void UnAttachBoneParent()
        {
            this.boneParentObj = null;
            this.boneParentName = "";
        }

        public virtual Vector3 DummyPos(string dummy)
        {
            var animator = GetAnimator();

            if (animator != null)
            {
                // 获取指定骨骼的Transform信息
                var bone = animator.Skeleton.FindBone(dummy);
                
                if (bone != null)
                {
                    //Vector3 bonePositionV1 = new Vector3(bone.WorldX, bone.WorldY, 0f);
                    Vector3 bonePositionV2 = new Vector3(bone.WorldX * this.Scale.x, bone.WorldY * -this.Scale.y, 0f);
                    /*Vector3 boneScaleV1 = new Vector3(bone.WorldScaleX, bone.WorldScaleY, 0f);
                    Vector3 boneScaleV2 = new Vector3(bone.ScaleX, bone.ScaleY, 0f);

                    Debug.Log("Bone World: " + bonePositionV1 + " " + boneScaleV1);
                    Debug.Log("Bone Local: " + bonePositionV2 + " " + boneScaleV2);
                    Debug.Log("Bone position: " + position);*/

                    return position + bonePositionV2;
                }
            }
            
            return position;
        }
        
        //副本 怪添加属性
        public void AddStageAttr(ConfigDungeonStageUnit attr)
        {
            if (attr != null)
            {
                this.Attr.HP = Convert.ToDouble(attr.Hp);
                this.Attr.HPMax = this.Attr.HP;
                this.Attr.Recovery = int.Parse(attr.Reply); //生命恢复
                
                if (attr.HarmType == (int)EN_HARM_TYPE.PhysicAtk)
                {
                    this.Attr.Atk += double.Parse(attr.PhysicalAtk); //攻击力
                    this.Attr.Def = double.Parse(attr.PhysicalDef); //防御值
                    this.Attr.HarmType = (int)EN_HARM_TYPE.PhysicAtk;
                }else if (attr.HarmType == (int)EN_HARM_TYPE.MagicAtk)
                {
                    this.Attr.Atk += double.Parse(attr.MagicAtk);
                    this.Attr.Def = double.Parse(attr.MagicDef);
                    this.Attr.HarmType = (int)EN_HARM_TYPE.MagicAtk;
                }else if (attr.HarmType == (int)EN_HARM_TYPE.SorceryAtk)
                {
                    this.Attr.Atk += double.Parse(attr.SorceryAtk);
                    this.Attr.Def = double.Parse(attr.SorceryDef);
                    this.Attr.HarmType = (int)EN_HARM_TYPE.SorceryAtk;
                }
            }
        }

        // 逃跑的宠物和野外BOSS
        public void AddStageAttr(ConfigEventStageUnit attr,int monsterId = 0)
        {
            if (attr != null)
            {
                // 新加怪物属性
                if (monsterId != 0)
                {
                    // ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(monsterId);
                    // ConfigStageMonsterAttrUnit monsterAttr = ConfigUtils.GetStageMonsterAttrUnitByIndexId(attr.Id);

                    this.Attr.HP = Convert.ToDouble(attr.Hp);
                    this.Attr.HPMax = this.Attr.HP;
                    this.Attr.Recovery = int.Parse(attr.Reply); //生命恢复
                    
                    if (attr.HarmType == (int)EN_HARM_TYPE.PhysicAtk)
                    {
                        this.Attr.Atk += double.Parse(attr.PhysicalAtk); //攻击力
                        this.Attr.Def = double.Parse(attr.PhysicalDef); //防御值
                        this.Attr.HarmType = (int)EN_HARM_TYPE.PhysicAtk;
                    }else if (attr.HarmType == (int)EN_HARM_TYPE.MagicAtk)
                    {
                        this.Attr.Atk += double.Parse(attr.MagicAtk);
                        this.Attr.Def = double.Parse(attr.MagicDef);
                        this.Attr.HarmType = (int)EN_HARM_TYPE.MagicAtk;
                    }else if (attr.HarmType == (int)EN_HARM_TYPE.SorceryAtk)
                    {
                        this.Attr.Atk += double.Parse(attr.SorceryAtk);
                        this.Attr.Def = double.Parse(attr.SorceryDef);
                        this.Attr.HarmType = (int)EN_HARM_TYPE.SorceryAtk;
                    }

                    // 战斗最终伤害 ---- 还未投放，暂时为角色的值
                    this.Attr.BattleFinalAttack = DataManager.Instance.GetRoleData().FightAttrVo.BattleFinalAttack;
                    
                    // monsterEntryUnits.Clear();
                    // monsterEntryUnits = ConfigUtils.GetMonsterEntryByGroup(attr.EntryGroup).ToList(); //改词条组列表
                    // var rates = new List<int>();
                    // for (int i = 0; i < monsterEntryUnits.Count; i++)
                    // {
                    //     rates.Add(monsterEntryUnits[i].Rate);
                    // }
                    //
                    // entryList.Clear();
                    // entryList = Utils.GetMonsterEntryByWeight(rates, attr.EntryNumber);
                    
                    entryList = Utils.GetEventStageMonsterEntry(attr);
                    monsterEntryUnits.Clear();
                    monsterEntryUnits = ConfigUtils.GetMonsterEntryByGroup(attr.EntryGroup).ToList(); //改词条组列表
                    
                    AddMonsterEntry();
                }
                
            }
        }
        
        // 普通boss
        public void AddStageAttr(ConfigStageMonsterAttrUnit attr, int monsterId = 0)
        {
            if (attr != null)
            {
                // 新加怪物属性
                if (monsterId != 0)
                {
                    // ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(monsterId);
                    // ConfigStageMonsterAttrUnit monsterAttr = ConfigUtils.GetStageMonsterAttrUnitByIndexId(attr.Id);

                    this.Attr.HP += Convert.ToDouble(attr.Hp);
                    this.Attr.HPMax = this.Attr.HP;
                    this.Attr.Recovery = int.Parse(attr.Reply); //生命恢复
                    
                    if (attr.HarmType == (int)EN_HARM_TYPE.PhysicAtk)
                    {
                        this.Attr.Atk += double.Parse(attr.PhysicalAtk); //攻击力
                        this.Attr.Def = double.Parse(attr.PhysicalDef); //防御值
                        this.Attr.HarmType = (int)EN_HARM_TYPE.PhysicAtk;
                    }else if (attr.HarmType == (int)EN_HARM_TYPE.MagicAtk)
                    {
                        this.Attr.Atk += double.Parse(attr.MagicAtk);
                        this.Attr.Def = double.Parse(attr.MagicDef);
                        this.Attr.HarmType = (int)EN_HARM_TYPE.MagicAtk;
                    }else if (attr.HarmType == (int)EN_HARM_TYPE.SorceryAtk)
                    {
                        this.Attr.Atk += double.Parse(attr.SorceryAtk);
                        this.Attr.Def = double.Parse(attr.SorceryDef);
                        this.Attr.HarmType = (int)EN_HARM_TYPE.SorceryAtk;
                    }

                    // 战斗最终伤害 ---- 还未投放，暂时为角色的值
                    this.Attr.BattleFinalAttack = DataManager.Instance.GetRoleData().FightAttrVo.BattleFinalAttack;
                    
                    // monsterEntryUnits.Clear();
                    // monsterEntryUnits = ConfigUtils.GetMonsterEntryByGroup(attr.EntryGroup).ToList(); //改词条组列表
                    // var rates = new List<int>();
                    // for (int i = 0; i < monsterEntryUnits.Count; i++)
                    // {
                    //     rates.Add(monsterEntryUnits[i].Rate);
                    // }
                    //
                    // entryList.Clear();
                    // entryList = Utils.GetMonsterEntryByWeight(rates, attr.EntryNumber);
                    
                    entryList = Utils.GetMonsterEntryById(attr.Id);
                    monsterEntryUnits.Clear();
                    monsterEntryUnits = ConfigUtils.GetMonsterEntryByGroup(attr.EntryGroup).ToList(); //改词条组列表
                    
                    AddMonsterEntry();
                }
                
            }
        }

        public List<ConfigMonsterEntryUnit> monsterEntryUnits = new List<ConfigMonsterEntryUnit>();
        public List<int> entryList = new List<int>();
        //关卡 怪添加属性
        public void AddStageAttr(ConfigStageUnit attr,int monsterId = 0)
        {
            if (attr != null)
            {
                // 新加怪物属性
                if (monsterId != 0)
                {
                    ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(monsterId);
                    ConfigStageMonsterAttrUnit monsterAttr = ConfigUtils.GetStageMonsterAttrUnitByIndexId(attr.Id);
                    
                    this.Attr.HP = Convert.ToDouble(monsterAttr.Hp);
                    this.Attr.HPMax = this.Attr.HP;
                    this.Attr.Recovery = int.Parse(monsterAttr.Reply); //生命恢复
                    
                    if (monsterUnit.HarmType == (int)EN_HARM_TYPE.PhysicAtk)
                    {
                        this.Attr.Atk += double.Parse(monsterAttr.PhysicalAtk); //攻击力
                        this.Attr.Def = double.Parse(monsterAttr.PhysicalDef); //防御值
                        this.Attr.HarmType = (int)EN_HARM_TYPE.PhysicAtk;
                    }else if (monsterUnit.HarmType == (int)EN_HARM_TYPE.MagicAtk)
                    {
                        this.Attr.Atk += double.Parse(monsterAttr.MagicAtk);
                        this.Attr.Def = double.Parse(monsterAttr.MagicDef);
                        this.Attr.HarmType = (int)EN_HARM_TYPE.MagicAtk;
                    }else if (monsterUnit.HarmType == (int)EN_HARM_TYPE.SorceryAtk)
                    {
                        this.Attr.Atk += double.Parse(monsterAttr.SorceryAtk);
                        this.Attr.Def = double.Parse(monsterAttr.SorceryDef);
                        this.Attr.HarmType = (int)EN_HARM_TYPE.SorceryAtk;
                    }

                    // 战斗最终伤害 ---- 还未投放，暂时为角色的值
                    this.Attr.BattleFinalAttack = DataManager.Instance.GetRoleData().FightAttrVo.BattleFinalAttack;
                    
                    entryList = Utils.GetMonsterEntryById(monsterAttr.Id);
                    monsterEntryUnits.Clear();
                    monsterEntryUnits = ConfigUtils.GetMonsterEntryByGroup(monsterAttr.EntryGroup).ToList(); //改词条组列表
                    AddMonsterEntry();
                }
                
            }
        }

        public void AddMonsterEntry()
        {
            //增加词条
            // entryList = Utils.GetMonsterEntryById(monsterAttr.Id);
            if (entryList != null && entryList.Count > 0)
            {
                for (int i = 0; i < entryList.Count; i++)
                {
                    // monsterEntryUnits[entryList[i]].SkillAchieveId = "86018";  //测试
                    
                    ConfigSkillAchieveUnit buffSkillAchieve = ConfigUtils.GetSkillAchieveById(int.Parse(monsterEntryUnits[entryList[i]].SkillAchieveId));
                    if (buffSkillAchieve != null && buffSkillAchieve.AttrOrAction != -1)
                    {
                        if (buffSkillAchieve.AttrOrAction == 0 && buffSkillAchieve.HandleObject == 3) //直接加在属性上
                        {
                            // buffSkillAchieve.CommonAttr = "34,10000";  //测试
                            string[] commonParams =  buffSkillAchieve.CommonAttr.Split('|');
                            foreach (var common in commonParams)
                            {
                                if (common.Split(",").Length > 1)
                                {
                                    FightUtils.SetMonsterAttrBySkillAchieve(this.Attr,int.Parse(common.Split(',')[0]), int.Parse(common.Split(',')[1]));
                                }
                            }
                        }
                        else
                        {
                            if (buffSkillAchieve.Trigger == 402)
                            {
                                ConfigBuffActionTemplateUnit buffAction = ConfigUtils.GetBuffActionByBuffId(buffSkillAchieve.ActID).Clone();
                                buffAction.CoverLastTime = buffSkillAchieve.CoverLastTime != 0 ? buffSkillAchieve.CoverLastTime : buffAction.CoverLastTime; //持续时间
                                buffAction.EffectTime = buffSkillAchieve.CoverEffectTime != 0 ? buffSkillAchieve.CoverEffectTime : buffAction.EffectTime; //生效时间
                                buffAction.COpFrequency = buffSkillAchieve.COpFrequency != 0 ? buffSkillAchieve.COpFrequency : buffAction.COpFrequency;  //生效次数
                                buffAction.Common = buffSkillAchieve.CoverCommonParam != "0" ? buffSkillAchieve.CoverCommonParam : buffAction.Common;
                                buffAction.BuffTime = buffSkillAchieve.BuffTime != 0 ? buffSkillAchieve.BuffTime : buffAction.BuffTime;
                                buffAction.Ruantity = buffSkillAchieve.Ruantity != 0 ? buffSkillAchieve.Ruantity : buffAction.Ruantity;
                                
                                MapMoveObject attack = MapObjectManager.Instance.GetLocalHero();
                                if (attack != null && attack.HitSpineDic.TryGetValue(buffAction.Id, out GLoader3D load3D))
                                {
                                    load3D.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                                    load3D.animationName = "Common_" +buffAction.BuffEffect + "_buff";
                                    load3D.frame = 0;
                                    load3D.loop = true;
                                    load3D.playing = true;
                                    load3D.visible = true;
                                    load3D.parent.SetChildIndex(load3D, 100);//显示最上层
                                    load3D.SetXY(-20, -100);

                                    if (buffAction.Id == (int)EN_BUFF_TYPE.SpeedReduce)
                                    {
                                        load3D.SetXY(-20, -95);
                                        SkeletonAnimation skeleton = load3D.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>();
                                        skeleton?.AnimationState.SetAnimation(0, "guang", true); // 基础层动画
                                        skeleton?.AnimationState.SetAnimation(1, "jian", true);

                                        GLoader3D loadSpeedReduce = attack.HitSpineDic[(int)EN_BUFF_TYPE.SpeedReduce1];
                                        loadSpeedReduce.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                                        loadSpeedReduce.animationName = "diquan";
                                        loadSpeedReduce.frame = 0;
                                        loadSpeedReduce.loop = true;
                                        loadSpeedReduce.playing = true;
                                        loadSpeedReduce.visible = true;
                                        loadSpeedReduce.SetXY(-20, -95);
                                        load3D.parent.RemoveChild(loadSpeedReduce);
                                        load3D.parent.AddChildAt(loadSpeedReduce, 0);

                                        var value = int.Parse(buffAction.Common.Split(",")[1]);
                                        if (this is MapMonsterObject && BuffInfoManager.Instance.TriggerRoleHoly(this as MapMonsterObject, (int)EN_BUFF_TYPE.SpeedReduce) != 0)
                                        {   //角色佩戴的圣物
                                            value -=  BuffInfoManager.Instance.TriggerRoleHoly(this as MapMonsterObject, (int)EN_BUFF_TYPE.SpeedReduce);
                                            value = value > 0 ? value : 0;
                                        }
                                        this.Attr.debuffAtkSpeedValue = DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed * value * ConstDefine.CONFIG_PLACE_EX;
                                        DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed -= this.Attr.debuffAtkSpeedValue;
                                    }else if (buffAction.Id == (int)EN_BUFF_TYPE.AtkReduce)
                                    {
                                        var value = int.Parse(buffAction.Common.Split(",")[1]);
                                        if (this is MapMonsterObject && BuffInfoManager.Instance.TriggerRoleHoly(this as MapMonsterObject, (int)EN_BUFF_TYPE.AtkReduce) != 0)
                                        {   //角色佩戴的圣物
                                            value -= BuffInfoManager.Instance.TriggerRoleHoly(this as MapMonsterObject, (int)EN_BUFF_TYPE.AtkReduce);
                                            value = value > 0 ? value : 0;
                                        }
                                        this.Attr.debuffDemageValue = DataManager.Instance.GetRoleData().FightAttrVo.Atk * value * ConstDefine.CONFIG_PLACE_EX;
                                        DataManager.Instance.GetRoleData().FightAttrVo.Atk -= this.Attr.debuffDemageValue;
                                    }
                                }
                            }
                            else
                            {
                                this.Attr.MonsterBuffs.Add(int.Parse(monsterEntryUnits[entryList[i]].SkillAchieveId));
                            }
                            
                        }
                    }
                }
            }
            
            //角色佩戴的圣物
            if (BuffInfoManager.Instance._buffInfoHolyItemInfoList.Count <= 0)
            {
                BuffInfoManager.Instance.SetBuffInfoHolyItemInfo();
            }
            foreach (var itemInfo in BuffInfoManager.Instance._buffInfoHolyItemInfoList)
            {
                ConfigSkillAchieveUnit buffSkillAchieve = ConfigUtils.GetSkillAchieveById(itemInfo.TableKeyId);
                if (buffSkillAchieve.Type == 7 && buffSkillAchieve.Trigger == 401)
                {
                    if (buffSkillAchieve.HandleObject == 3)
                    {
                        string[] commonAttrs1 = buffSkillAchieve.CommonAttr.Split('|');
                        if (commonAttrs1.Length > 0)
                        {
                            foreach (var attrs in commonAttrs1)
                            {
                                string[] commonAttrs = attrs.Split(',');
                                if (commonAttrs.Length > 1)
                                {
                                    if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.CriticalStrike)
                                    {
                                        this.Attr.CriticalStrike = this.Attr.CriticalStrike * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                    }
                                    else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.CriticalInjury)
                                    {
                                        this.Attr.CriticalInjury = this.Attr.CriticalInjury * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                    }
                                    else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.AtkSpeed)
                                    {
                                        this.Attr.AtkSpeed = this.Attr.AtkSpeed * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                        this.Attr.AtkSpeed = this.Attr.AtkSpeed <= 0 ? 0.01f : this.Attr.AtkSpeed;
                                    }
                                    else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ATK)
                                    {
                                        this.Attr.Atk = this.Attr.Atk * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                    }
                                    else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.Recovery)
                                    {
                                        this.Attr.Recovery = this.Attr.Recovery * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                    }
                                    else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.Def)
                                    {
                                        this.Attr.Def = this.Attr.Def * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                    }
                                    else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ParryRate)
                                    {
                                        this.Attr.ParryRate = this.Attr.ParryRate * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                    }
                                    else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ParryValue)
                                    {
                                        this.Attr.ParryValue = this.Attr.ParryValue * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                    }
                                    else
                                    {
                                        FightUtils.SetMonsterAttrBySkillAchieve(this.Attr,int.Parse(commonAttrs[0]), -int.Parse(commonAttrs[1]));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
