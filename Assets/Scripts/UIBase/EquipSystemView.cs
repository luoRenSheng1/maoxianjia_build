using System.Collections.Generic;
using System.Linq;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using Equip;
using FairyGUI;
using msg;
using Spine.Unity;
using EventDispatcher = EngineBase.EventDispatcher;
using UI_ArtifactAttrItem = Equip.UI_ArtifactAttrItem;
using UI_ArtifactAttrItem2 = Equip.UI_ArtifactAttrItem2;
using UI_ArtifactSelectBtn = Equip.UI_ArtifactSelectBtn;

public class EquipSystemView : UIViewBase
{
    private UI_EquipSystem equipUI => this.main as UI_EquipSystem;

    private int _openIndex = -1;
    private int _preSelectIndex;

    //普通装备
    private SkeletonAnimation _heroSpine;

    //传承装备
    private UI_LoreEquip _loreEquipUI;
    private Dictionary<int, UI_LoreEquipItem> _loreEquipItemDict = new Dictionary<int, UI_LoreEquipItem>(4);

    //神器
    private UI_ArtifactEquip _artifactEquipUI;
    private Dictionary<int, int> _artifactDict = new Dictionary<int, int>();//服务器下发
    private ConfigArtifactEquipUnit curUnit;
    private List<ItemData> curAttrList = new List<ItemData>();//当前等级属性
    private List<ItemData> nextAttrList = new List<ItemData>();//下一等级属性
    private List<ItemData> maxAttrList = new List<ItemData>();//最大等级属性
    private List<ConfigArtifactEquipUnit> _artifactEquipUnitList = new List<ConfigArtifactEquipUnit>();
    private int selectArtifactTypeIndex = 0;
    private Dictionary<int, UI_ArtifactSelectBtn> _artifactSelectDict = new Dictionary<int, UI_ArtifactSelectBtn>();

    public EquipSystemView()
    {
        this.name = "EquipSystem";
        this.package = "Equip";
        this.component = "EquipSystem";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        EquipBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        if (values[0] != null)
            _openIndex = (int)values[0];
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.equipUI.EnsureBoundsCorrect();

        this.equipUI.tabList.onClickItem.Add(OnClickBottomItem);

        //普通装备界面
        this.equipUI.commonEquip.equipList.itemRenderer = CommonEquipItemRenderer;
        this.equipUI.commonEquip.makeBtnAni.onClick.Add(OnMakeBtnClick);//铸造装备
        this.equipUI.commonEquip.upLvBtn.onClick.Add(OnUpLvBtnClick);//升级宝箱等级

        //传承装备界面
        _loreEquipUI = UIPackage.CreateObject("Equip", "LoreEquip") as UI_LoreEquip;
        this.equipUI.AddChildAt(_loreEquipUI, 1);
        _loreEquipUI.AddRelation(this.equipUI, RelationType.Width);
        _loreEquipUI.AddRelation(this.equipUI, RelationType.BottomExt_Bottom);
        _loreEquipUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        // _loreEquipUI.upLoreList.itemRenderer = UpLoadLoreEquipRenderer;
        _loreEquipUI.loreEquipList.itemRenderer = LoreEquipItemRenderer;
        for (int i = 0; i < 4; i++)
        {
            UI_LoreEquipItem loreEquipItem = _loreEquipUI.GetChild("loreEquip" + i) as UI_LoreEquipItem;
            _loreEquipItemDict.Add(i, loreEquipItem);
            loreEquipItem?.onClick.Add(this.OnClickLoreEquipItem);
            LoreListRender(i, loreEquipItem);
        }
        _loreEquipUI.recycleBtn.onClick.Add(OnRecycleBtnClick);//回收传承装备

        //神器界面
        _artifactEquipUI = UIPackage.CreateObject("Equip", "ArtifactEquip") as UI_ArtifactEquip;
        this.equipUI.AddChildAt(_artifactEquipUI, 2);
        _artifactEquipUI.AddRelation(this.equipUI, RelationType.Width);
        _artifactEquipUI.AddRelation(this.equipUI, RelationType.BottomExt_Bottom);
        _artifactEquipUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        _artifactEquipUI.attrList.itemRenderer = ArtifactAttrRenderer;
        _artifactEquipUI.maxList.itemRenderer = ArtifactMaxAttrRenderer;
        _artifactEquipUI.upLvBtn.onClick.Add(OnArtifactUpLvBtnClick);
        _artifactEquipUI.artifact.onClick.Add(OnArtifactClickShowTIps);

        this.equipUI.tabList.selectedIndex = 0;

        EventDispatcher.GameWorld.Regist<EquipData, bool, int, int>(EventDefine.EVENT_USE_TREASURE_CHES_RES, OnUseTreasureChes);
        EventDispatcher.GameWorld.Regist<int, string>(EventDefine.EVENT_TREASURE_LEVELUP_RES, OnTreasureChesLevelUpRes);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PART_REPLACE_RES, OnPartReplaceSucc);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SIGLE_EQUIP_WEAR, OnEquipSingleWear);
        EventDispatcher.GameWorld.Regist<ulong>(EventDefine.EVENT_EQUIP_CHANGE, UpdatePartAll);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateItemCount);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HERO, ShowMyHero);
        EventDispatcher.GameWorld.Regist<bool>(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, UpdateTreasureRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.UpdateInheritInfo);//传承装备
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.RefreshInheritBag);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LORE_EQUIP_REMOVE, this.RefreshInheritBag);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, UpdateLoreCurInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ARTIFACT_UPDATE, this.UpdateArtifactInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ARTIFACT_UPDATE, this.RedPointHandler);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.MakBtnCanUse);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist<EquipData, bool, int, int>(EventDefine.EVENT_USE_TREASURE_CHES_RES, OnUseTreasureChes);
        EventDispatcher.GameWorld.UnRegist<int, string>(EventDefine.EVENT_TREASURE_LEVELUP_RES, OnTreasureChesLevelUpRes);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PART_REPLACE_RES, OnPartReplaceSucc);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SIGLE_EQUIP_WEAR, OnEquipSingleWear);
        EventDispatcher.GameWorld.UnRegist<ulong>(EventDefine.EVENT_EQUIP_CHANGE, UpdatePartAll);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateItemCount);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HERO, ShowMyHero);
        EventDispatcher.GameWorld.UnRegist<bool>(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, UpdateTreasureRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.UpdateInheritInfo);//传承装备
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.RefreshInheritBag);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LORE_EQUIP_REMOVE, this.RefreshInheritBag);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, UpdateLoreCurInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ARTIFACT_UPDATE, this.UpdateArtifactInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ARTIFACT_UPDATE, this.RedPointHandler);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.MakBtnCanUse);
    }

    protected override void OnShow()
    {
        base.OnShow();

        if (_openIndex != -1)
        {
            ChangeIndex(_openIndex); ;
        }
        else
        {
            if (this.equipUI.tabList.selectedIndex == -1)
                this.equipUI.tabList.selectedIndex = 0;
            ChangeIndex(this.equipUI.tabList.selectedIndex);
        }
        this.equipUI.tabList.EnsureBoundsCorrect();

        GameManager.Instance.TimerManager.SetTimer(0.3f, RefreshBottomRedDot);

        //引导-点击矿镐升级
        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            bid = GuideID.NewAccount_ClickSecretCanon1,
            giding = GuideID.NewAccount_UIEquip0,
            gid = GuideID.NewAccount_MPUpLevel,
            tui = this.equipUI.commonEquip.upLvBtn,
            isForce = true,
            isSend = false,
            npcTxt = "Beginner_Doc_006",
            npcPosType = PosType.Down,
            uiName = "EquipSystem"
        })) { return; }


        //引导-传承装备标签
        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.Inherit,
            pid = GuideID.Click_UIClothingEquip,
            giding = GuideID.Click_UIEquip,
            gid = GuideID.Click_UIInheritEquip,
            tui = this.equipUI.tabList.GetChildAt(1),
            isForce = true,
            isSend = false
        })) { return; }
    }

    protected override void OnHide()
    {
        base.OnHide();
        _openIndex = 0;
        GuideManager.Instance.HideGuide();
        
        if (GuideManager.Instance.GuideId == (int)GuideID.Click_UIEquipClose3)
        {
            GuideManager.Instance.SendToCompleteGuide((int)GuideID.Click_UIEquipClose3);
            GuideManager.Instance.HideGuide();
        }
    }

    private void OnClickBottomItem(EventContext context)
    {
        GButton item = context.data as GButton;
        var index = this.equipUI.tabList.GetChildIndex(item);
        ChangeIndex(index);
    }

    private void ChangeIndex(int index)
    {
        bool isChange = true;
        switch (index)
        {
            case 0:
                _preSelectIndex = index;
                UpdateCommonEquipInfo();
                UpdateTreasureRedDot(false);
                break;
            case 1:
                var inheritMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Inherit);
                if (inheritMap.Item1)
                {
                    _preSelectIndex = index;
                    UpdateLoreEquipInfo();
                }
                else
                {
                    UIManager.Instance.Toast(inheritMap.Item2);
                    ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(index)).selected = false;
                    isChange = false;
                }
                break;
            case 2:
                var artifactMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Artifact);
                if (artifactMap.Item1)
                {
                    _preSelectIndex = index;
                    selectArtifactTypeIndex = 0;
                    UpdateArtifactEquipInfo();
                }
                else
                {
                    UIManager.Instance.Toast(artifactMap.Item2);
                    ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(index)).selected = false;
                    isChange = false;
                }
                break;
        }

        if (isChange)
        {
            _loreEquipUI.visible = index == 1;
            _artifactEquipUI.visible = index == 2;
        }

        for (int i = 0; i < 3; i++)
        {
            if (i == _preSelectIndex)
                ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(_preSelectIndex)).selected = true;
            else
            {
                ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(i)).selected = false;
            }
        }

        this.equipUI.typeCtrl.selectedIndex = _preSelectIndex;
    }

    public void RefreshBottomRedDot()
    {
        if (!IsShow() || !IsOnStage()) return;
        this.equipUI.tabList.EnsureBoundsCorrect();

        //传承装备
        var inheritMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Inherit);
        if (!inheritMap.Item1)
        {
            ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(1)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(1)).lockCtrl.selectedIndex = 0;
            //红点
            bool flag2 = EquipManager.Instance.GetHighQualityEquipInBag().Count > 0;
            bool highSkillCount = EquipManager.Instance.GetHighSKillCountInLoreBag().Count > 0;//更高技能次数
            bool highDamageMultipler = EquipManager.Instance.GetHighDamageMultipler().Count > 0;//更高伤害倍率
            bool highHPMultipler = EquipManager.Instance.GetHighHPMultipler().Count > 0;//更高生命倍率
            ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(1)).redCtrl.selectedIndex = EquipManager.Instance.HasNowearEquipRedPoint() || flag2 || highSkillCount || highDamageMultipler || highHPMultipler ? 1 : 0;
        }

        //神器
        var artifactMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Artifact);
        if (!artifactMap.Item1)
        {
            ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 0;
            //红点
            bool artifactHasRed = EquipManager.Instance.ArtifactRedPointHandler();
            ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(2)).redCtrl.selectedIndex = artifactHasRed ? 1 : 0;
        }
    }

    #region 普通装备

    private void UpdateCommonEquipInfo()
    {
        MakBtnCanUse();
        
        UpdatePartAll();

        UpdateBoxInfo();

        ShowMyHero();
    }

    private void OnRoleUpdate()
    {
        UpdateTreasureRedDot(false);
    }

    private void ShowMyHero()
    {
        HeroInfo myHero = HeroInfoManager.Instance.GetMyHero();
        Utils.SetSpineModelOnFGUI(this.equipUI.commonEquip.myHero, myHero.HeroUnit.Model, 130f, HeroState.idle.ToString(), (o =>
        {
            if (o is SkeletonAnimation animation)
                _heroSpine = animation;
        }));
    }

    private void OnUseTreasureChes(EquipData equipData, bool isNew, int deltaTime, int type)
    {
        if (IsShow() && IsOnStage())
        {
            GuideManager.Instance.HideGuide();
            ulong guid = equipData.guid;
            // Debug.Log("isNew:"+isNew);
            EquipManager.Instance.curNoEquipGuid = guid;
            EquipManager.Instance.isNewEquip = isNew;
            EquipManager.Instance.GetEquipById(guid).isNew = true;
            int sourceId = equipData.id;
            ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
            int quality = config.Quality >= 5 ? 5 : config.Quality;
            // Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.spineEff, "lv1", false, delegate()
            // {
            //     Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.spineEff, "idle", true);
            // });
            // GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 2, 0.2f, () =>
            // {
            //     if(!VillageInfoManager.Instance.IsInVillageHome)
            //         GameManager.Instance.SoundManager.PlayEffectWithoutLoop(14);
            //     Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.fx, "skill", false);
            //     Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.fx1, "skill", false);
            //     this.lobbyMain.panel.comBox.btnBoxIcon.enabled = true;
            // });

            UpdateBoxInfo();
            if (EquipManager.Instance.AutoUnpack)
            {
                bool isStopAutopack = true;
                bool isEntry1 = true;
                bool isEntry2 = true;
                AutoOpenEquipStruct openEquipStruct = EquipManager.Instance.OpenEquipStruct;

                if (equipData.quality < openEquipStruct.equipQuality)
                {
                    isStopAutopack = false;
                }

                if (openEquipStruct.isEntry1)
                {
                    if (openEquipStruct.entryId1 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId1))
                    {
                        isEntry1 = false;
                    }

                    if (isEntry1)
                    {
                        if (openEquipStruct.entryId2 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId2))
                        {
                            isEntry1 = false;
                        }
                    }

                }

                if (openEquipStruct.isEntry2)
                {
                    if (openEquipStruct.entryId3 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId3))
                    {
                        isEntry2 = false;
                    }

                    if (isEntry2)
                    {
                        if (openEquipStruct.entryId4 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId4))
                        {
                            isEntry2 = false;
                        }
                    }

                }

                if (isStopAutopack && isEntry1 && isEntry2)
                {
                    EquipManager.Instance.HasNewEquipToStopAutopack = true;
                    // UIManager.Instance.ShowUIPanel("Equip", guid, isNew);
                    GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 11, deltaTime, () =>
                    {
                        UIManager.Instance.ShowUIPanel("Equip", guid, isNew, type);
                    });
                }
                else
                {
                    if (!EquipManager.Instance.HasNewEquipToStopAutopack)
                    {
                        // this.lobbyMain.panel.tempEquipIcon.visible = false;
                        EquipManager.Instance.DecomposeEquip(guid);
                    }
                }

            }
            else
            {
                // UIManager.Instance.ShowUIPanel("Equip", guid, isNew);
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 11, deltaTime, () =>
                {
                    UIManager.Instance.ShowUIPanel("Equip", guid, isNew, type);
                });
            }

        }
    }

    private void OnPartReplaceSucc()
    {
        if (IsShow() && IsOnStage())
        {
            UpdatePartAll();
        }
    }

    private void UpdateItemCount()
    {
        UpdateBoxInfo();
    }

    private void UpdateBoxInfo()
    {
        this.equipUI.commonEquip.goldCur.icon = UIResource.GetItemUrl(2000.ToString());//金币
        this.equipUI.commonEquip.goldCur.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.gold);
        this.equipUI.commonEquip.hammerCur.icon = UIResource.GetItemUrl(10000001.ToString());//铸造锤
        this.equipUI.commonEquip.hammerCur.txtValue.text = DataManager.Instance.GetMagicKeys().ToString();//锤子数量
        this.equipUI.commonEquip.upLvBtn.lvLb.SetVar("value",
                DataManager.Instance.GetTreasureData().id.ToString()).FlushVars(); //宝箱等级
        MakBtnCanUse();
    }

    private void UpdatePartAll(ulong equipId = 0)
    {
        for (int i = (int)EN_EQUIP_PARTS.Weapon; i < 5; i++)
        {
            int index = i - 1;
            var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS)i);
            UpdatePartData(index, partEquipData);
        }

        // for (int i = (int) EN_Other_PARTS.Weapon; i < (int) EN_Other_PARTS.Count; i++)
        // {
        //     int index = i - 1;
        //     GObject gObject = (GButton) this.lobbyMain.panel.listEquip2.GetChildAt(index);
        //     gObject.onClick.Set(() =>
        //     {
        //         UIManager.Instance.ToastByKey(206);
        //     });
        // }

        if (equipId > 0)
        {
            int sourceId = EquipManager.Instance.GetEquipById(equipId).id;
            ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
            // this.lobbyMain.panel.tempEquipIcon.visible = true;
            // this.lobbyMain.panel.tempEquipIcon.tempEquipIcon.icon = UIResource.GetItemUrl(config.Icon);
        }
    }

    private void UpdatePartData(int index, EquipData partEquipData)
    {
        if (index < this.equipUI.commonEquip.equipList.numItems)
        {
            UI_BtnEquip2 gObject = (UI_BtnEquip2)this.equipUI.commonEquip.equipList.GetChildAt(index);
            if (null != gObject)
            {
                gObject.onClick.Clear();
                gObject.qualityIcon.visible = true;
                if (null != partEquipData && !partEquipData.IsNull())
                {
                    var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
                    if (null != config)
                    {
                        gObject.icon = UIResource.GetItemUrl(config.Icon); //UIResource.GetPartURL(config.Parts); //config.Icon;
                        gObject.ctrQuality.selectedIndex = partEquipData.quality - 1;
                        gObject.qualityIcon.visible = true;
                        gObject.hasCnt.selectedIndex = 1;
                        gObject.lvLb.SetVar("value", partEquipData.lv.ToString()).FlushVars();
                    }

                    gObject.onClick.Set(() =>
                    {
                        UIManager.Instance.ShowUIPanel("PopupEquipAttribute", partEquipData);
                    });
                    // if (partEquipData.quality < 4)
                    // {
                    //     gObject.equipSpineEff.visible = false;
                    // }
                    // else
                    // {
                    //     gObject.equipSpineEff.visible = true;
                    //     gObject.equipSpineEff.spineAnimation.AnimationState.AddAnimation(0, "idle_Q"+partEquipData.quality.ToString(), true, 0);
                    // }

                }
                else
                {
                    gObject.icon = "";
                    // gObject.equipSpineEff.visible = false;
                    gObject.ctrbg.selectedIndex = index + 8;
                    gObject.qualityIcon.visible = false;
                    gObject.hasCnt.selectedIndex = 0;
                }
            }
        }
    }

    private void OnEquipSingleWear()
    {
        this.UpdatePartAll();
        // this.lobbyMain.panel.tempEquipIcon.visible = false;
    }

    private void CommonEquipItemRenderer(int index, GObject item)
    {

    }
    
    /// <summary>
    /// 是否解锁
    /// </summary>
    private void MakBtnCanUse()
    {
        var zhuzhaoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.MakeEquip);
        int hammerCount = DataManager.Instance.GetMagicKeys();
    
        if (!zhuzhaoMap.Item1)
        {
            // 功能未解锁
            this.equipUI.commonEquip.makeBtnAni.makeBtn.isLock.selectedIndex = 0;
            this.equipUI.commonEquip.lockCtrl.selectedIndex = 0;
            this.equipUI.commonEquip.makeBtnAni.makeBtn.grayed = false; // 保持正常颜色
            this.equipUI.commonEquip.makeBtnAni.makeBtn.touchable = false; // 不可点击
        }
        else
        {
            // 功能已解锁
            this.equipUI.commonEquip.makeBtnAni.makeBtn.isLock.selectedIndex = 1;
            this.equipUI.commonEquip.lockCtrl.selectedIndex = 1;
        
            if (hammerCount <= 0)
            {
                // 锤子不足，置灰按钮
                this.equipUI.commonEquip.makeBtnAni.makeBtn.grayed = true;
                this.equipUI.commonEquip.makeBtnAni.makeBtn.touchable = false;
            }
            else
            {
                // 锤子充足，恢复正常
                this.equipUI.commonEquip.makeBtnAni.makeBtn.grayed = false;
                this.equipUI.commonEquip.makeBtnAni.makeBtn.touchable = true;
            }
        }
    }

    //铸造装备
    private void OnMakeBtnClick()
    {
        this.equipUI.commonEquip.makeBtnAni.t0.Play();
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.MakeEquipSE);
        
        // 是否解锁铸造功能
        var zhuzhaoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.MakeEquip);
        if (!zhuzhaoMap.Item1)
        {
            UIManager.Instance.Toast(zhuzhaoMap.Item2);
            return;
        }

        if (ItemInfoManager.Instance.GetItemCount(10000001) <= 0)
        {
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(10000001).Name)));
            return;
        }

        this.equipUI.commonEquip.makeBtnAni.enabled = false;
        GameManager.Instance.TimerManager.SetTimer(2f, () =>
        {
            this.equipUI.commonEquip.makeBtnAni.enabled = true;
        });
        if (EquipManager.Instance.AutoUnpack)
        {
            EquipManager.Instance.AutoUnpack = false;
            EquipManager.Instance.HasNewEquipToStopAutopack = false;
            return;
        }

        if (EquipManager.Instance.curNoEquipGuid != 0)
        {
            UIManager.Instance.ShowUIPanel("Equip", EquipManager.Instance.curNoEquipGuid, EquipManager.Instance.isNewEquip, 2);
            return;
        }

        // 播放角色挖宝spine
        if (_heroSpine != null)
        {
            _heroSpine.state.SetAnimation(0, HeroState.wabao.ToString(), true);
            GameManager.Instance.TimerManager.SetTimer(2, () =>
            {
                _heroSpine.state.AddAnimation(0, HeroState.idle.ToString(), true, 0f);
            });

        }

        TreasureChesManager.Instance.UseTreasureChes(DataManager.Instance.GetTreasureData().id, 0, 0);
    }

    //宝箱升级
    private void OnTreasureChesLevelUpRes(int msgCode, string msgData)
    {
        if (IsShow() && IsOnStage())
        {
            JsonObject res = (JsonObject)SimpleJson.DeserializeObject(msgData);
            if (null == res)
                return;

            switch (msgCode)
            {
                case MsgCode.SUCCESS:
                    {
                        // var comCurrency = (UI_ComCurrency) ((UI_ComUserInfo)this.lobbyMain.panel.userInfo).comCurrency;
                        var comCurrency = (UI_EquipCurrency)this.equipUI.commonEquip.goldCur;
                        comCurrency.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
                        this.equipUI.commonEquip.upLvBtn.lvLb.SetVar("value", DataManager.Instance.GetTreasureData().id.ToString()).FlushVars();
                    }
                    break;
                case MsgCode.FAIL:
                    {
                        int errCode = res.GetInt("errCode");
                        switch (errCode)
                        {
                            case 1000:
                                UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_LIMITE_LEVEL_ERROR);
                                break;
                            case 1001:
                                UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_CD_ERROR);
                                break;
                            case 1002:
                                UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_CURRENCY_ERROR);
                                break;
                            case 1003:
                                UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_LEVEL_ERROR);
                                break;
                        }
                    }
                    break;
            }
        }
    }

    //宝箱红点
    private void UpdateTreasureRedDot(bool isLevelUp)
    {
        var zhuzhaiMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Zhuzhao);
        if (!zhuzhaiMap.Item1)
        {
            return;
        }

        bool canUpLv = false;
        int currLv = DataManager.Instance.GetTreasureData().id;
        int maxLv = TreasureChesManager.Instance.GetTreasureMaxLv();
        if (currLv < maxLv)
        {
            if (TreasureChesManager.Instance.TreasureBoxOpenStatus != TreasureBoxOpenStatus.LevelUp)
            {
                var treasureChestUnit = ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id);
                if (treasureChestUnit != null)
                {

                    // canUpLv = DataManager.Instance.GetRoleData().gold >= double.Parse(treasureChestUnit.GoldCoins);
                    canUpLv = DataManager.Instance.GetRoleData().gold >= double.Parse(treasureChestUnit.GoldCoins) * treasureChestUnit.GoldCoinsNum;
                }

            }
        }
        int totalSecond = (int)(DataManager.Instance.GetTreasureData().lastTargetTime - ServerTimeManager.Instance.CurServerTime);
        bool isPro = totalSecond > 0;
        var data = TreasureChesManager.Instance.CheckTreasureChes();
        this.equipUI.commonEquip.redPoint.visible = (canUpLv && data.Item1 == 0 && !isPro);
        ((UI_EquipTabBtn)this.equipUI.tabList.GetChildAt(0)).redCtrl.selectedIndex = (canUpLv && data.Item1 == 0 && !isPro) ? 1 : 0;
    }

    //升级宝箱
    private void OnUpLvBtnClick()
    {
        UIManager.Instance.ShowUIPanel("TreasureLevelup");
    }

    #endregion


    #region 传承装备

    private void UpdateLoreEquipInfo()
    {
        UpdateLoreCurInfo();
        UpdateInheritInfo();
        RefreshInheritBag();
    }

    private void UpdateLoreCurInfo()
    {
        _loreEquipUI.goldCur.icon = UIResource.GetItemUrl(2000.ToString());//金币
        _loreEquipUI.goldCur.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        _loreEquipUI.diaCur.icon = UIResource.GetItemUrl(1000.ToString());//钻石
        _loreEquipUI.diaCur.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia);
    }

    /// <summary>
    /// 传承装备信息
    /// </summary>
    private void UpdateInheritInfo()
    {
        // for (int i = 5; i < 9; i++)
        // {
        //         int index = i - 5;
        //         var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) i);
        //         SetInheritData(index, partEquipData, _loreEquipUI.upLoreList);
        // }

        for (int i = 0; i < 4; i++)
        {
            int partIndex = i + 5;
            var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS)partIndex);
            SetInheritData(i, partEquipData, _loreEquipItemDict[i]);
        }

        RefreshBottomRedDot();
    }

    // 传承背包列表 刷新
    private void RefreshInheritBag()
    {
        // for (int i = 5; i < 9; i++)
        // {
        //         int index = i - 5;
        //         var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) i);
        //         SetInheritData(index, partEquipData, _loreEquipUI.upLoreList);
        // }

        for (int i = 0; i < 4; i++)
        {
            int partIndex = i + 5;
            var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS)partIndex);
            SetInheritData(i, partEquipData, _loreEquipItemDict[i]);
        }

        GetLoreEquipList();

        _loreEquipUI.loreEquipList.numItems = equipBagList.Count;

        _loreEquipUI.vacancyBg.visible = !(equipBagList.Count > 0);

        RefreshBottomRedDot();

        //引导-列表第一个传承装备
        if (_loreEquipUI.loreEquipList.numItems > 0)
        {
            GuideManager.Instance.StarGuideByData(new GuideData()
            {
                giding = GuideID.Click_UIInheritEquip,
                gid = GuideID.Click_UIFirstEquip,
                tui = _loreEquipUI.loreEquipList.GetChildAt(0),
                isForce = true,
                isSend = false,
                npcTxt = "Beginner_Doc_014",
                npcPosType = PosType.Down,
            });
        }
    }

    /// <summary>
    /// 传承装备数据设置
    /// </summary>
    /// <param name="index">装备列表位置索引</param>
    /// <param name="partEquipData">装备部位数据</param>
    /// <param name="equipList">装备列表数据</param>
    // private void SetInheritData(int index, EquipData partEquipData,GList equipList)
    // {
    //         UI_LoreEquipItem gObject = (UI_LoreEquipItem) equipList.GetChildAt(index);
    //         if (null != gObject)
    //         {
    //                 gObject.onClick.Clear();
    //                 gObject.qualityIcon.visible = true;
    //                 if (null != partEquipData && !partEquipData.IsNull())
    //                 {
    //                         var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
    //                         if (null != config)
    //                         {
    //                                 gObject.icon = UIResource.GetItemUrl(config.Icon); //UIResource.GetPartURL(config.Parts); //config.Icon;
    //                                 gObject.qualityCtrl.selectedIndex = partEquipData.quality - 1;
    //                                 gObject.qualityIcon.visible = true;
    //                                 gObject.hasCnt.selectedIndex = 1;
    //                                 gObject.lvLb.SetVar("value",partEquipData.lv.ToString()).FlushVars();
    //                         }
    //
    //                         gObject.onClick.Set(() =>
    //                         {
    //                                 // UIManager.Instance.ShowUIPanel("PopupEquipAttribute", partEquipData);
    //                                 //                                 选中装备的guid    是否新装备 新装备只显示新装备  旧装备 会显示两个装备的信息
    //                                 EquipManager.Instance.isInheritEquip = true;
    //                                 UIManager.Instance.ShowUIPanel("Equip",partEquipData.guid, true);//partEquipData);
    //                         });
    //                 }
    //                 else
    //                 {
    //                         gObject.icon = "";
    //                         // gObject.equipSpineEff.visible = false;
    //                         gObject.ctrbg.selectedIndex = index;
    //                         gObject.qualityIcon.visible = false;
    //                         gObject.hasCnt.selectedIndex = 0;
    //                 }
    //         }
    // }

    private void SetInheritData(int index, EquipData partEquipData, UI_LoreEquipItem gObject)
    {
        if (null != gObject)
        {
            gObject.onClick.Clear();
            gObject.qualityIcon.visible = true;
            if (null != partEquipData && !partEquipData.IsNull())
            {
                var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
                if (null != config)
                {
                    gObject.icon = UIResource.GetItemUrl(config.Icon); //UIResource.GetPartURL(config.Parts); //config.Icon;
                    gObject.qualityCtrl.selectedIndex = partEquipData.quality - 1;
                    gObject.qualityIcon.visible = true;
                    gObject.hasCnt.selectedIndex = 1;
                    gObject.lvLb.SetVar("value", partEquipData.lv.ToString()).FlushVars();
                }

                gObject.onClick.Set(() =>
                {
                    // UIManager.Instance.ShowUIPanel("PopupEquipAttribute", partEquipData);
                    //                                 选中装备的guid    是否新装备 新装备只显示新装备  旧装备 会显示两个装备的信息
                    EquipManager.Instance.isInheritEquip = true;
                    UIManager.Instance.ShowUIPanel("Equip", partEquipData.guid, true);//partEquipData);
                });
            }
            else
            {
                gObject.icon = "";
                // gObject.equipSpineEff.visible = false;
                gObject.ctrbg.selectedIndex = index;
                gObject.qualityIcon.visible = false;
                gObject.hasCnt.selectedIndex = 0;
            }
        }
    }

    private List<EquipData> equipBagList = new List<EquipData>();
    private List<EquipData> highQualityEmptyPartList = new List<EquipData>();
    /// <summary>
    /// 获取背包传承装备列表
    /// </summary>
    private void GetLoreEquipList()
    {
        equipBagList.Clear();
        foreach (var equipItem in EquipManager.Instance.GetNoWearLoreEquip())
        {
            // 未穿戴的传承装备都显示
            equipBagList.Add(equipItem);
        }

        if (equipBagList == null)
            return;

        highQualityEmptyPartList.Clear();
        highQualityEmptyPartList = EmptySlotLoreHighQualityRedPoint();

        equipBagList.Sort((item1, item2) =>
        {
            if (item1 == null || item2 == null)
                return 0;
            if (item1.quality < item2.quality)
            {
                return 1;
            }
            else if (item1.quality == item2.quality)
            {
                if (item1.lv > item2.lv)
                {
                    return 1;
                }
                else if (item1.lv == item2.lv)
                {
                    return item1.id > item2.id ? 1 : -1;
                }
            }
            return -1;
        });
    }

    //背包传承装备列表
    private void LoreEquipItemRenderer(int index, GObject item)
    {
        UI_LoreEquipItem gObject = (UI_LoreEquipItem)item;
        if (null != gObject)
        {
            gObject.onClick.Clear();
            gObject.qualityIcon.visible = true;
            var partEquipData = equipBagList[index];
            if (null != partEquipData && !partEquipData.IsNull())
            {
                var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
                if (null != config)
                {
                    gObject.icon = UIResource.GetItemUrl(config.Icon); //UIResource.GetPartURL(config.Parts); //config.Icon;
                    gObject.qualityCtrl.selectedIndex = partEquipData.quality - 1;
                    gObject.qualityIcon.visible = true;
                    gObject.hasCnt.selectedIndex = 1;
                    gObject.lvLb.SetVar("value", partEquipData.lv.ToString()).FlushVars();
                }

                gObject.onClick.Set(() =>
                {
                    // UIManager.Instance.ShowUIPanel("PopupEquipAttribute", partEquipData);
                    //                                         选中装备的guid     是否新装备 新装备只显示新装备  旧装备 会显示两个装备的信息
                    EquipManager.Instance.isInheritEquip = true;
                    UIManager.Instance.ShowUIPanel("Equip", partEquipData.guid, false);
                });

                List<EquipData> equipBag = EquipManager.Instance.GetHighQualityEquipInBag();
                List<EquipData> highSkillCountList = EquipManager.Instance.GetHighSKillCountInLoreBag();//更高技能次数
                List<EquipData> highDamageMultiplerList = EquipManager.Instance.GetHighDamageMultipler();//更高伤害倍率
                List<EquipData> highHPMultiplerList = EquipManager.Instance.GetHighHPMultipler();//更高生命倍率
                gObject.loreRedPoint.visible = highQualityEmptyPartList.Contains(partEquipData) || equipBag.Contains(partEquipData) || highSkillCountList.Contains(partEquipData) || highDamageMultiplerList.Contains(partEquipData) || highHPMultiplerList.Contains(partEquipData);
            }
            else
            {
                gObject.icon = "";
                gObject.qualityCtrl.selectedIndex = 0;
                gObject.hasCnt.selectedIndex = 0;
            }
        }
    }

    // 有空栏位，对应空栏位装备类型品质最高的展示红点
    private List<EquipData> EmptySlotLoreHighQualityRedPoint()
    {
        List<EquipData> highestQualityEquips = new List<EquipData>();//未穿戴的新类型传承装备品质最高的列表
        
        List<EquipData> noWearLoreEquipList = new List<EquipData>();
        noWearLoreEquipList = EquipManager.Instance.GetNoWearLoreEquip();//没有穿戴的传承装备列表

        List<EquipData> installLoreEquipList = new List<EquipData>();
        installLoreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();//已装备的传承装备列表

        // 获取已装备的所有partType
        HashSet<int> installedPartTypes = new HashSet<int>();
        foreach (var equip in installLoreEquipList)
        {
            installedPartTypes.Add(equip.partType);
        }
        
        // 按部件类型分组，并选择每个类型中品质最高的未穿戴装备
        var equipGroups = noWearLoreEquipList
            .Where(equip => !installedPartTypes.Contains(equip.partType)) // 过滤出新类型
            .GroupBy(equip => equip.partType); // 按部件类型分组
        
        foreach (var group in equipGroups)
        {
            // 选择该组中品质最高的装备
            EquipData highestQualityEquip = group.OrderByDescending(e => e.quality).First();
            highestQualityEquips.Add(highestQualityEquip);
        }

        return highestQualityEquips;
    }

    private void LoreListRender(int index, GObject item)
    {

    }

    private void OnClickLoreEquipItem()
    {

    }

    //一键回收传承装备
    private void OnRecycleBtnClick()
    {
        UIManager.Instance.ShowUIPanel("InheritRecycle");
    }

    #endregion


    #region 神器

    private void UpdateArtifactEquipInfo()
    {
        HideUIAndSpine();

        for (int i = 0; i < 4; i++)
        {
            UI_ArtifactSelectBtn artifactSelectBtn = _artifactEquipUI.GetChild("tab" + i) as UI_ArtifactSelectBtn;
            // _artifactSelectDict.Add(i, artifactSelectBtn);
            _artifactSelectDict[i] = artifactSelectBtn;
            artifactSelectBtn?.onClick.Add(this.OnClickArtifactTypeItem);
            // TabListRender(i, artifactSelectBtn);
        }

        _artifactEquipUI.tabCtrl.selectedIndex = 0;

        ChangeArtifactIndex(0);
        UpdateArtifactInfo();
        RedPointHandler();
    }

    private void HideUIAndSpine()
    {
        _artifactEquipUI.artifact.spineEff.visible = false;
        _artifactEquipUI.artifact.icon1.visible = true;
    }

    private void OnClickArtifactTypeItem(EventContext context)
    {
        UI_ArtifactSelectBtn item = context.sender as UI_ArtifactSelectBtn;
        var index = int.Parse(item.name.Substring(3, 1));
        _artifactEquipUI.tabCtrl.selectedIndex = index;
        selectArtifactTypeIndex = index;
        ChangeArtifactIndex(index);
        UpdateArtifactInfo();

        _artifactEquipUI.artifact.spineEff.visible = false;
        _artifactEquipUI.artifact.icon1.visible = true;
    }

    private void ChangeArtifactIndex(int index)
    {
        switch (index)
        {
            case 0://盾牌
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.SHIELD, _artifactDict[(int)ArtifactType.SHIELD]);
                break;
            case 1://勋章
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.MEDAL, _artifactDict[(int)ArtifactType.MEDAL]);
                break;
            case 2://符石
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.STONE, _artifactDict[(int)ArtifactType.STONE]);
                break;
            case 3://职印
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.JOB, _artifactDict[(int)ArtifactType.JOB]);
                break;
        }
    }

    private void UpdateArtifactInfo()
    {
        _artifactDict = EquipManager.Instance.GetArtifactDict();
        ChangeArtifactIndex(selectArtifactTypeIndex);

        _artifactEquipUI.artifact.icon1.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Icon);
        ((UI_ArtifactItem1)_artifactEquipUI.artifact).qualityIcon.visible = true;
        ((UI_ArtifactItem1)_artifactEquipUI.artifact).qualityCtrl.selectedIndex = ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Quality - 1;
        ((UI_ArtifactItem1)_artifactEquipUI.artifact).lvLb.SetVar("value", curUnit.ArtifactLevel.ToString()).FlushVars();

        _artifactEquipUI.name.text = ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Name);
        _artifactEquipUI.desc.text = ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Desc);

        ConfigArtifactEquipUnit nextArtifactEquipUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(curUnit.Type, curUnit.ArtifactLevel);//下一等级
        int costDia;
        int costItem;
        string[] costItems;
        string costItemId;

        ((UI_TabCom)_artifactEquipUI.tabCom1).type.selectedIndex = 1;
        ((UI_TabCom)_artifactEquipUI.tabCom2).type.selectedIndex = 1;
        if (nextArtifactEquipUnit != null)//是否满级
        {
            costItems = nextArtifactEquipUnit.ItemCost.Split(',');
            costDia = nextArtifactEquipUnit.DiamondsCost;
            costItem = int.Parse(costItems[1]);
            costItemId = costItems[0];

            _artifactEquipUI.isMax.selectedIndex = 0;
        }
        else
        {
            costDia = 0;
            costItem = 0;
            costItemId = curUnit.ItemCost.Split(',')[0];

            _artifactEquipUI.isMax.selectedIndex = 1;
            _artifactEquipUI.status.selectedIndex = 1;
        }
        ((UI_TabCom)_artifactEquipUI.tabCom2).icon.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(costItemId)).Icon);

        ((UI_TabCom)_artifactEquipUI.tabCom1).status.selectedIndex = (costDia <= DataManager.Instance.mRoleData.dia) ? 0 : 1;
        ((UI_TabCom)_artifactEquipUI.tabCom2).status.selectedIndex = (costItem <= ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId))) ? 0 : 1;
        _artifactEquipUI.status.selectedIndex = ((costDia <= DataManager.Instance.mRoleData.dia) && (costItem <= ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId)))) ? 0 : 2;


        // if (curUnit.ArtifactLevel >= GetMaxHeroLv())//神器未满级时，升级上限取玩家所有角色中的等级最高的为上限等级
        // {
        //     _artifactEquipUI.status.selectedIndex = 2;
        // }

        if (nextArtifactEquipUnit == null)
            _artifactEquipUI.status.selectedIndex = 1;

        //消耗砖石StringUtils.FormatCurrency
        ((UI_TabCom)_artifactEquipUI.tabCom1).num2.SetVar("value", StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia)).SetVar("cost", costDia.ToString()).FlushVars();//costDia.ToString()

        //消耗特殊材料
        ((UI_TabCom)_artifactEquipUI.tabCom2).num2.SetVar("value", StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId)))).SetVar("cost", costItem.ToString()).FlushVars();//costItem.ToString()

        curAttrList.Clear();//清理之前数据

        nextAttrList.Clear();//清理之前数据

        _artifactEquipUI.curLv.SetVar("curLv", curUnit.ArtifactLevel.ToString()).FlushVars();
        if (nextArtifactEquipUnit != null)
        {
            nextAttrList = AttrsHandle(nextArtifactEquipUnit);//下一等级属性处理
                                                              // curAttrList = AttrsHandle(curUnit);//当前等级属性处理

            if (curUnit.ArtifactLevel == 0)
            {
                // 当前等级为0级时
                List<ItemData> zeroLvAttrList = new List<ItemData>();
                foreach (var item in nextAttrList)
                {
                    ItemData itemData = new ItemData();
                    itemData.id = item.id;
                    itemData.count = 0;
                    zeroLvAttrList.Add(itemData);
                }

                curAttrList = zeroLvAttrList;
            }
            else
            {
                curAttrList = AttrsHandle(curUnit);//当前等级属性处理
            }

            _artifactEquipUI.nextLv.SetVar("next", nextArtifactEquipUnit.ArtifactLevel.ToString()).FlushVars();
            // nextAttrList = AttrsHandle(nextArtifactEquipUnit);//下一等级属性处理

            _artifactEquipUI.attrList.numItems = nextAttrList.Count;
            _artifactEquipUI.attrList.data = nextAttrList;
            _artifactEquipUI.attrList.ResizeToFit();
        }
        else
        {
            // 最高等级属性
            maxAttrList.Clear();
            ConfigArtifactEquipUnit maxUnit = ConfigUtils.GetMaxLvArtifactEquipUnitByType(curUnit.Type);
            maxAttrList = AttrsHandle(maxUnit);
            _artifactEquipUI.maxLv.SetVar("max", maxUnit.ArtifactLevel.ToString()).FlushVars();
            _artifactEquipUI.maxList.data = maxAttrList;
            _artifactEquipUI.maxList.numItems = maxAttrList.Count;
            _artifactEquipUI.maxList.ResizeToFit();
        }

        // _artifactEquipUI.attrList.numItems = curAttrList.Count;
        // _artifactEquipUI.attrList.data = curAttrList;
        // _artifactEquipUI.attrList.ResizeToFit();

        // 最高等级属性
        // maxAttrList.Clear();
        // ConfigArtifactEquipUnit maxUnit = ConfigUtils.GetMaxLvArtifactEquipUnitByType(curUnit.Type);
        // maxAttrList = AttrsHandle(maxUnit);
        // _artifactEquipUI.maxLv.SetVar("max",maxUnit.ArtifactLevel.ToString()).FlushVars();
        // _artifactEquipUI.maxList.data = maxAttrList;
        // _artifactEquipUI.maxList.numItems = maxAttrList.Count;
        // _artifactEquipUI.maxList.ResizeToFit();

        _artifactEquipUI.upLvBtn.data = curUnit;

        RefreshBottomRedDot();
    }

    private List<ItemData> AttrsHandle(ConfigArtifactEquipUnit unit)
    {
        List<ItemData> attrList = new List<ItemData>();
        string[] attrs = unit.Attr.Split("|");
        foreach (var attr in attrs)
        {
            string[] s = attr.Split(',');
            ItemData itemData = new ItemData();
            itemData.id = int.Parse(s[0]);
            itemData.count = double.Parse(s[1]);

            attrList.Add(itemData);
        }
        return attrList;
    }

    private void ArtifactAttrRenderer(int index, GObject item)
    {
        ItemData curAttrsData = curAttrList[index];
        ItemData nextAttrData = nextAttrList[index];

        // ((UI_ArtifactAttrItem)item).curAttrName.SetVar("name",
        //         ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(curAttrsData.id).AttrName)).FlushVars();
        //
        string lastValue1 = EquipManager.Instance.SetAttributeValue(curAttrsData.id, curAttrsData.count, true);
        // ((UI_ArtifactAttrItem)item).curAttr.text = lastValue1;

        ((UI_ArtifactAttrItem)item).curAttr.SetVar("name", ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(curAttrsData.id).AttrName)).SetVar("value", lastValue1).FlushVars();

        string lastValue2 = EquipManager.Instance.SetAttributeValue(nextAttrData.id, nextAttrData.count, true);
        ((UI_ArtifactAttrItem)item).nextAttr.text = lastValue2;
    }

    private void ArtifactMaxAttrRenderer(int index, GObject item)
    {
        ItemData maxAttrsData = maxAttrList[index];
        ((UI_ArtifactAttrItem2)item).name.text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(maxAttrsData.id).AttrName);

        string lastValue = EquipManager.Instance.SetAttributeValue(maxAttrsData.id, maxAttrsData.count, true);
        ((UI_ArtifactAttrItem2)item).num.text = lastValue;
    }

    // 获取拥有的英雄中等级最高的英雄等级
    private int GetMaxHeroLv()
    {
        return HeroInfoManager.Instance.GetMaxHeroLv();
    }

    private void OnArtifactUpLvBtnClick(EventContext context)
    {
        // 升级条件不符拦截
        ConfigArtifactEquipUnit unit = (context.sender as GButton).data as ConfigArtifactEquipUnit;
        ConfigArtifactEquipUnit nextUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(unit.Type, unit.ArtifactLevel);

        if (nextUnit != null)
        {
            if (DataManager.Instance.mRoleData.dia >= nextUnit.DiamondsCost)
            {
                if (ItemInfoManager.Instance.GetItemCount(int.Parse(nextUnit.ItemCost.Split(',')[0])) >= int.Parse(nextUnit.ItemCost.Split(',')[1]))
                {
                    // 神器等级不可超过角色等级
                    // if (unit.ArtifactLevel < GetMaxHeroLv())
                    // {
                    //     var builder = ArtifactLevelUp_CS.CreateBuilder();
                    //     builder.AttrId = (ePlayerAttrID)unit.Type;
                    //     builder.LevelUpValues = 1;
                    //     GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ArtifactLevelUp_CS, builder.Build());
                    //
                    //     if (unit.ItemId != nextUnit.ItemId)
                    //     {
                    //         // 升级成功播放spine
                    //         ((UI_ArtifactItem1)_artifactEquipUI.artifact).icon1.visible = false;
                    //         _artifactEquipUI.artifact.spineEff.visible = true;
                    //         _artifactEquipUI.artifact.spineEff.SetScale(0.8f, 0.8f);
                    //         Utils.PlaySpineAnim(_artifactEquipUI.artifact.spineEff, "Artifact_sqtx", false, () =>
                    //         {
                    //             _artifactEquipUI.artifact.spineEff.visible = false;
                    //             _artifactEquipUI.artifact.icon1.visible = true;
                    //         });
                    //         _artifactEquipUI.artifact.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(nextUnit.ItemId).Icon);
                    //     }
                    // }
                    // else
                    // {
                    //     UIManager.Instance.ToastByKey(8022);
                    // }
                    
                    var builder = ArtifactLevelUp_CS.CreateBuilder();
                    builder.AttrId = (ePlayerAttrID)unit.Type;
                    builder.LevelUpValues = 1;
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ArtifactLevelUp_CS, builder.Build());
                    
                    if (unit.ItemId != nextUnit.ItemId)
                    {
                        // 升级成功播放spine
                        ((UI_ArtifactItem1)_artifactEquipUI.artifact).icon1.visible = false;
                        _artifactEquipUI.artifact.spineEff.visible = true;
                        _artifactEquipUI.artifact.spineEff.SetScale(0.8f, 0.8f);
                        Utils.PlaySpineAnim(_artifactEquipUI.artifact.spineEff, "Artifact_sqtx", false, () =>
                        {
                            _artifactEquipUI.artifact.spineEff.visible = false;
                            _artifactEquipUI.artifact.icon1.visible = true;
                        });
                        _artifactEquipUI.artifact.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(nextUnit.ItemId).Icon);
                    }

                }
                else
                {
                    // UIManager.Instance.ToastByKey(5001);//材料不足提示
                    UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(nextUnit.ItemCost.Split(',')[0])).Name)));
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5008);//砖石不足提示
            }
        }
        else
        {
            // UIManager.Instance.ToastByKey(5001);//满级提示
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8023, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(unit.ItemId).Name)));//满级提示
        }
    }

    private void OnArtifactClickShowTIps()
    {
        ConfigArtifactEquipUnit data = curUnit;
        UIManager.Instance.ShowUIPanel("ArtifactAttrTip", data);
    }

    private void RedPointHandler()
    {
        _artifactDict = EquipManager.Instance.GetArtifactDict();

        var tabMap = new Dictionary<int, UI_ArtifactSelectBtn>
                {
                        {53, (UI_ArtifactSelectBtn)_artifactEquipUI.tab0},
                        {54, (UI_ArtifactSelectBtn)_artifactEquipUI.tab1},
                        {55, (UI_ArtifactSelectBtn)_artifactEquipUI.tab2},
                        {56, (UI_ArtifactSelectBtn)_artifactEquipUI.tab3}
                };

        foreach (var item in _artifactDict)
        {
            if (!tabMap.TryGetValue(item.Key, out var tabComponent))
                continue;

            var curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv(item.Key, item.Value);
            var nextUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(item.Key, item.Value);

            // 满级时隐藏红点
            if (nextUnit == null)
            {
                tabComponent.redPoint.visible = false;
                continue;
            }

            string[] costItems = nextUnit.ItemCost.Split(',');
            int costDiamond = nextUnit.DiamondsCost;
            int costItemId = int.Parse(costItems[0]);
            int costItemAmount = int.Parse(costItems[1]);

            // 升级条件
            bool canUpgrade = DataManager.Instance.mRoleData.dia >= costDiamond
                              && ItemInfoManager.Instance.GetItemCount(costItemId) >= costItemAmount;
                              // && curUnit.ArtifactLevel < GetMaxHeroLv();

            tabComponent.redPoint.visible = canUpgrade;
        }
    }

    #endregion


    public GComponent GetEquipSystemGoldCur()
    {
        return this.equipUI.commonEquip.goldCur;
    }
}