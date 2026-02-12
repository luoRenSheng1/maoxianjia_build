using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using Engine;
using EngineBase;

namespace Engine
{
    public class MapObjectSkillVoFx
    {
        public bool bLoopEffect;
        public MapFxObject fx;
    }

    public class MapObjectSkillVo
    {
        public void Reset()
        {
            battleActionID = 0;
            skillID = 0;
            aniName = "";
            if (lstFx != null)
            {
                lstFx.Clear();
            }

            AtkSpeed = 1f;
        }

        public void AddFx(MapFxObject fx, bool bLoopEffect)
        {
            if (lstFx == null)
            {
                lstFx = new List<MapObjectSkillVoFx>();
            }

            MapObjectSkillVoFx infofx = new MapObjectSkillVoFx();
            infofx.fx = fx;
            infofx.bLoopEffect = bLoopEffect;
            lstFx.Add(infofx);
        }

        public int battleActionID = 0;
        public int skillID = 0;
        public string aniName = "";
        public List<MapObjectSkillVoFx> lstFx;
        public bool bLoopSound;
        public float AtkSpeed = 1f;
    }

    public class MapObjectStateVo
    {
        public int id;
        public int cfgId;
        public int conRound;
        public MapFxObject fx;
        public bool bLoopSound;
    }
    
    public class MapFxObject : MapObject
    {
        public int EffectID { get; protected set; }
        public int EndFx { get; protected set; }
        public int EndSound { get; protected set; }
        public float DelayTime { get; protected set; }
        public int ownerUnitID { get; protected set; }
        public string AniName { get; protected set; } = "";
        public bool Loop { get; protected set; } = false;
        public int AniTimeScale { get; protected set; } = 1;

        // 使用时才赋值，增加复用材质的可能性
        protected Material material;
        protected bool bInitColor = false;
        protected Color colorInit;
        protected int nameIDInit;
        protected bool bInitSetBlackdropFactor = false;

        public override MapObjectType ObjectType
        {
            get { return MapObjectType.VirtualFx; }
        }

        public bool RegisterTick { get; set; }
        public bool RegisterEndPlay { get; set; }
        public bool RegisterBeginPlay { get; set; }
        public Action EndAction { get; set; }

        public virtual void InitByConfig(Config.ConfigSkillEffectUnit cfg, Vector3 pos, Quaternion rot, MapObject owner)
        {
            if (cfg == null)
            {
                return;
            }

            this.ownerUnitID = owner != null ? owner.id : 0;
            float size = cfg.Size * ConstDefine.CONFIG_PLACE * ConstDefine.MODEL_FX_SCALE;
            float lifeTime = cfg.Time * ConstDefine.CONFIG_PLACE_TIME;
            this.EffectID = cfg.Id;
            this.DelayTime = cfg.Delay * ConstDefine.CONFIG_PLACE_TIME;
            this.EndFx = cfg.Endfx;
            this.EndSound = cfg.Endsound;
            this.AniName = cfg.Ani;
            this.Loop = cfg.Loop != 0;
            this.AniTimeScale = cfg.AniScale;
            
            int gid = MapObjectManager.Instance.NewMapVirtualObjectID(MapObjectType.VirtualFx);
            this.BeginPlay(gid, "", pos, rot);

            if (!Mathf.Approximately(size, 1.0f))
            {
                if (owner is MapMonsterObject || (owner != null && owner.Attr.Camp == EN_CAMP_TYPE.ENEMY))
                {
                    this.SetScale(new Vector3(-size, size, size));
                }
                else
                {
                    this.SetScale(new Vector3(size, size, size));
                }

            }

            if (this.DelayTime > 0)
            {
                this.SetActive(false);
                RegisterBeginPlay = true;
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.BATTLE, 20,this.DelayTime, this.StartPlay);
            }
            else
            {
                this.StartPlay();
            }

            if (lifeTime > 0)
            {
                RegisterEndPlay = true;
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.BATTLE,21,lifeTime, this.EndPlay);
            }
        }

        public override void Destroyed()
        {
            base.Destroyed();
            ownerUnitID = 0;
        }

        public virtual void StartPlay()
        {
            this.SetActive(true);
            this.CheckAnimator();
            this.DelayTime = 0;
            RegisterBeginPlay = false;
        }

        public virtual void EndPlay()
        {
            if (this.EndAction != null)
            {
                this.EndAction.Invoke();
                this.EndAction = null;
            }

            if (EndFx != 0)
            {
                MapObjectManager.Instance.SpawnFxActor(EndFx, this.position, Quaternion.identity);
                EndFx = 0;
            }

            if (RegisterBeginPlay)
            {
                GameManager.Instance.TimerManager.ClearTimer(this.StartPlay);
                RegisterEndPlay = false;
            }

            if (RegisterEndPlay)
            {
                GameManager.Instance.TimerManager.ClearTimer(this.EndPlay);
                RegisterEndPlay = false;
            }

            if (RegisterTick)
            {
                SetTick(false);
            }

            MapObjectManager.Instance.DestroyActor(this);
        }

        protected void SetTick(bool bTick)
        {
            if (this.RegisterTick != bTick)
            {
                RegisterTick = bTick;

                if (this.RegisterTick)
                {
                    MapObjectManager.Instance.RegisterMapFxObject(this);
                }
                else
                {
                    MapObjectManager.Instance.UnRegisterMapFxObject(this);
                }
            }
        }
        
        public override void SetGameObject(GameObject go)
        {

            this.SetScale(this.Scale);

            base.SetGameObject(go);

            this.CheckAnimator();
          
            if (this.DelayTime > 0)
            {
                this.SetActive(false);
            }

            if (bInitColor)
            {
                SetColor(nameIDInit, colorInit);
                bInitColor = false;
            }

            if (bInitSetBlackdropFactor)
            {
                SetBlackdropFactor(true);
                bInitSetBlackdropFactor = false;
            }

            if (UIContainerRoot != null)
            {
                UIContainerRoot.data = 999;
            }
        }

        private void CheckAnimator()
        {
            if (string.IsNullOrEmpty(this.AniName))
            {
                return;
            }
            
            var animator = GetAnimator();

            if (animator != null)
            {
                AnimatorPlay(this.AniName, this.Loop, this.AniTimeScale);
            }
            else
            {
                U3DAnimatorPlay(this.AniTimeScale);
            }
        }
        
        public void SetOnlyColor(Color color)
        {
            SetColor(ConstDefine.COLOR_ID, color);
        }

        public void SetTintColor(Color color)
        {
            SetColor(ConstDefine.TINT_COLOR_ID, color);
        }

        public void SetColor(int nameID, Color color)
        {
            if (material == null && gameObj != null)
            {
                Renderer r = gameObj.GetComponentInChildren<Renderer>();

                if (r != null)
                {
                    material = r.material;
                }
            }

            if (material != null)
            {
                if (material.GetColor(nameID) != color)
                {
                    material.SetColor(nameID, color);
                }
            }
            else
            {
                bInitColor = true;
                colorInit = color;
                nameIDInit = nameID;
            }
        }

        public void SetBlackdropFactor(bool bIgnoreBlackdrop)
        {
            if (gameObj != null)
            {
                Renderer[] arrRender = gameObj.GetComponentsInChildren<Renderer>();

                foreach (var r in arrRender)
                {
                    if (r != null && r.material != null)
                    {
                        r.material.SetFloat("_IgnoreBlackFactor", bIgnoreBlackdrop ? 1.0f : 0.0f);
                    }
                }
            }
            else
            {
                bInitSetBlackdropFactor = bIgnoreBlackdrop;
            }
        }
    }
}