using CommonEx;
using Config;
using EngineBase;
using FairyGUI;
using msg;
using Spine.Unity;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

namespace Engine
{
    public class MapVillagePetObjectAttr : MapMoveObjectAttr
    {
        /// <summary>
        /// 0=家园 1=抓宠
        /// </summary>
        public int VillagePetType;
        public ulong PetGuid;
        public int PosIndex;
        public ConfigPetBasisUnit BasisUnit;
        public void Init(int unitID, int posIndex, ulong petGuid, int villagePetType)
        {
            this.unitID = unitID;
            this.ObjectTypeID = unitID;
            this.Camp = EN_CAMP_TYPE.FRIEND;
            this.PosIndex = posIndex;
            this.PetGuid = petGuid;
            this.VillagePetType = villagePetType;
            
            ConfigPetBasisUnit petBasisUnit = ConfigUtils.GetPetById(this.ObjectTypeID);
            if (petBasisUnit != null)
            {
                BasisUnit = petBasisUnit;
                // MapHeroObject hero = MapObjectManager.Instance.GetLocalHero();
                // InitByAttrID(petBasisUnit.AtkSpeed, hero.Attr.Speed);
            }
        }
    }
    
    public class MapVillagePetObject : MapMoveObject
    {
        private UI_VillagePetTalk UIContainerPaopao;
        private UI_VillagePetWorkTop _villagePetWork;
        public bool IsNeedShowPaopao = true;
        private GButton emptyBtn;
        private GLoader tagIcon;
        
        protected override MapObjectAttr CreateNewAttr()
        {
            return new MapVillagePetObjectAttr();
        }

        public MapVillagePetObjectAttr PetAttr
        {
            get { return this.Attr as MapVillagePetObjectAttr; }
        }
        
        public MapVillagePetObject()
        {
            IsNeedSyncHP = false;
            MoveSpeed = ConstDefine.DEFAULT_MOVE_SPEED;
        }

        public GComponent GetClickBtn()
        {
            return _villagePetWork;
        }
        protected override void OnBeginPlay()
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
                MapVillagePetManager.Instance.LoadGameObject(this);
            }
        }
        
        public override void Destroyed()
        {
            emptyBtn?.onClick.Remove(this.OnClickPet);
            ClearMovePath();
            base.Destroyed();

        }

        public override void SetGameObject(GameObject go)
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
        
        public override void TriggerSetGameObject()
        {
            float size = ConstDefine.MODEL_SCALE_MIN;
            this.Scale = new Vector3(size, size, 1);
            
            if (this.gameObj != null)
            {
                this.gameObj.transform.localPosition = Vector3.zero;
                this.gameObj.transform.localScale = Vector3.one;

                animComp = this.gameObj.GetComponentInChildren<SkeletonAnimation>();
                animComp?.Initialize(true);


                var objRoot = MapVillagePetManager.Instance.GetMapObjectRootTrans(this.PetAttr.VillagePetType);

                if (objRoot == null)
                {
                    LogUtils.LogError("TriggerSetGameObject Error!!!");
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
                
                if (IsNeedShowPaopao)
                {
                    if (UIContainerPaopao == null)
                    {
                        UIContainerPaopao = UIPackage.CreateObject("CommonEx", "VillagePetTalk") as UI_VillagePetTalk;
                        UIContainerRoot.AddChild(UIContainerPaopao);
                        UIContainerRoot.EnsureBoundsCorrect();
                    }
                    else
                    {
                        UIContainerPaopao.RemoveFromParent();
                        UIContainerRoot.AddChild(UIContainerPaopao);
                    }
                    
                    if (UIContainerPaopao != null)
                    {
                        UIContainerPaopao.xy = new Vector2(-5, -110);
                        UIContainerPaopao.visible = false;
                    }
                    
                    if (_villagePetWork == null)
                    {
                        _villagePetWork = UIPackage.CreateObject("CommonEx", "VillagePetWorkTop") as UI_VillagePetWorkTop;
                        UIContainerRoot.AddChild(_villagePetWork);
                        UIContainerRoot.EnsureBoundsCorrect();
                    }
                    else
                    {
                        _villagePetWork.RemoveFromParent();
                        UIContainerRoot.AddChild(_villagePetWork);
                    }
                    
                    if (_villagePetWork != null)
                    {
                        _villagePetWork.workBar.max = 100;
                        _villagePetWork.workBar.min = 0;
                        _villagePetWork.workBar.value = 50;
                        _villagePetWork.xy = new Vector2(-35, -130);
                        _villagePetWork.visible = false;
                        
                        _villagePetWork.onClick.Set(this.OnClickPet);
                    }
                    
                }
                
                this.SetActive(this.Active);
                this.SetTransform(this.position, this.rotation, true);
                this.SetScale(this.Scale);
                if (emptyBtn == null)
                {
                    emptyBtn = UIPackage.CreateObject("CommonEx", "EmptyBtn").asButton;
                    emptyBtn.SetSize(size*2.5f, size*3f);
                    // emptyBtn.icon = UIResource.GoldIcon;
                    UIContainerRoot.AddChild(emptyBtn);
                    emptyBtn.xy = new Vector2(-30, -80);
                }
          
                emptyBtn.onClick.Set(this.OnClickPet);

                if (this.PetAttr.BasisUnit.Type == 2)
                {
                    if(tagIcon == null)
                        tagIcon = new GLoader();
                    tagIcon.url = "ui://5moj1x39vcwq2j";
                    UIContainerRoot.AddChild(tagIcon);
                    tagIcon.xy = new Vector2(-15, -60);
                }
                else if(tagIcon != null)
                {
                    UIContainerRoot.RemoveChild(tagIcon);
                    tagIcon = null;
                }
            }
        }

        private void OnClickPet()
        {
            if(this.PetAttr.VillagePetType == 0)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_PET_ONCLICK, PetAttr.ObjectTypeID, PetAttr.PetGuid);
        }

        public void ShowTalk(string talk)
        {
            if (UIContainerPaopao != null && !UIContainerPaopao.visible)
            {
                UIContainerPaopao.visible = true;
                UIContainerPaopao.talkDes.text = talk;
                GameManager.Instance.TimerManager.SetTimer(1f, () =>
                {
                    UIContainerPaopao.visible = false;
                });
            }
        }
    }
}