using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Lobby;
using msg;
using RoleMain;
using Spine;
using Spine.Unity;
using UnityEngine;
using Animation = Spine.Animation;
using EventDispatcher = EngineBase.EventDispatcher;

public class RoleSelectView : UIViewBase
{
    private UI_RoleSelect roleUI => this.main as UI_RoleSelect;
    
    private HeroInfo _heroInfo;
    private List<ConfigHeroUnit> _heroUnits;
    private List<ConfigHeroAttrUnit> _heroAttrUnits;
    private ConfigCommonUnit _common100002;
    private SkeletonAnimation _spine;
    private readonly string[] _RoleAniName = new[] {"skill", "run", "attack", "idle"};
    private int _curRoleAnimIndex = -1;
    public RoleSelectView()
    {
        this.name = "RoleSelect";
        this.package = "RoleMain";
        this.component = "RoleSelect";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _heroInfo = values[0] as HeroInfo;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.roleUI.roleSelectBottom.closeBtn.onClick.Add(this.Hide);
        this.roleUI.roleSelectBottom.upLoadBtn.onClick.Add(this.OnClickUpLoadBtn);
        this.roleUI.roleSelectBottom.levelUpBtn.onTouchBegin.Add(OnTouchBeginItem);
        this.roleUI.roleSelectBottom.levelUpBtn.onTouchEnd.Add(OnTouchEndItem);
        // this.roleUI.levelUpBtn.onClick.Add(this.OnClickBreakBtn);
        this.roleUI.roleSelectBottom.breakBtn.onClick.Add(this.OnClickBreakBtn);
        this.roleUI.roleSelectBottom.getHeroBtn.onClick.Add(this.OnClickGetHeroBtn);
        this.roleUI.heroList.itemRenderer = HeroItemRender;
        this.roleUI.heroList.onClickItem.Add(this.OnClickHeroListItem);
        this.roleUI.tipsbtn.onClick.Add(this.OnClickTipsBtn);
        this.roleUI.levelAttrList.itemRenderer = HeroLevelAttrItemRender;
        this.roleUI.starList.itemRenderer = HeroStarListRender;
        this.roleUI.playSpineBtn.onClick.Add(this.OnClickPetToShowAni);
        this.roleUI.roleSelectBottom.adBtn.onClick.Add(this.OnClickAdBtn);
        this.roleUI.roleSelectBottom.composeHeroBtn.onClick.Add(this.OnClickHCBtn);

        //所有英雄数据
        _heroUnits = HeroInfoManager.Instance.GetAllHeroUnits();
        
        _common100002 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(100002);
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_common100002.Param1));
        this.roleUI.roleSelectBottom.levelUpBtn.levelUpBtn.itemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
        
        ConfigCommonUnit common100001 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(100001);
        ConfigItemTypeUnit itemTypeUnit2 = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(common100001.Param1));
        this.roleUI.roleSelectBottom.lvItemIcon.url = UIResource.GetItemUrl(itemTypeUnit2.Icon);
        this.roleUI.roleSelectBottom.avGetLb.text = common100001.Param2;
        
        this.roleUI.strongBtn.onClick.Add(this.OnClickStrongBtn);//强化
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, OnItemUpdate);
        FairyGUI.Stage.inst.onTouchEnd.AddCapture(__stageTouchEnd);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HERO, this.UpdateBattleHero);
        EventDispatcher.GameWorld.Regist<HeroInfo, int>(EventDefine.EVENT_UPDATE_HEROInfo_LevelUp, this.UpdateHeroLevelUp);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_HEROInfo_Break, this.UpdateHeroBreakUp);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_HEROInfo, this.UpdateHeroInfos);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_WATCHADTOGET_HEROLEVELUP_Item, this.UpdateHeroLevelUpItemUpdate);
        EventDispatcher.GameWorld.Regist<HeroInfo>(EventDefine.EVENT_UPDATE_HEROInfo_Merge, this.UpdateHeroMergeInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_HEROInfo, this.UpdateRoleInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_HeroSkillLV_INFO, this.UpdateRoleInfo);
    }
    
    private void __stageTouchEnd(EventContext context)
    {
        OnTouchEndItem();
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, OnItemUpdate);
        FairyGUI.Stage.inst.onTouchEnd.RemoveCapture(__stageTouchEnd);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HERO, this.UpdateBattleHero);
        EventDispatcher.GameWorld.UnRegist<HeroInfo, int>(EventDefine.EVENT_UPDATE_HEROInfo_LevelUp, this.UpdateHeroLevelUp);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_HEROInfo_Break, this.UpdateHeroBreakUp);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_HEROInfo, this.UpdateHeroInfos);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_WATCHADTOGET_HEROLEVELUP_Item, this.UpdateHeroLevelUpItemUpdate);
        EventDispatcher.GameWorld.UnRegist<HeroInfo>(EventDefine.EVENT_UPDATE_HEROInfo_Merge, this.UpdateHeroMergeInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_HEROInfo, this.UpdateRoleInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_HeroSkillLV_INFO, this.UpdateRoleInfo);
    }
    
    
    private void OnClickPetToShowAni()
    {
        if(_spine == null) return;
        _curRoleAnimIndex++;
        if (_curRoleAnimIndex >= _RoleAniName.Length)
            _curRoleAnimIndex = 0;
        string aniName = _RoleAniName[_curRoleAnimIndex];
        _spine.skeleton.SetToSetupPose();
        _spine.state.ClearTracks();
        Animation animation = _spine.skeleton.Data.FindAnimation(aniName);
        while (animation == null)
        {
            _curRoleAnimIndex++;
            if (_curRoleAnimIndex >= _RoleAniName.Length)
                _curRoleAnimIndex = 0;
            aniName = _RoleAniName[_curRoleAnimIndex];
            animation = _spine.skeleton.Data.FindAnimation(aniName);
        }

        _spine.state.SetAnimation(0, aniName, false).Complete += (Spine.TrackEntry track) =>
        {
            _spine.state.SetAnimation(0, "idle", true);
        };
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        UpdateRoleInfo();
        OnItemUpdate();
        _heroUnits.Sort(SortHeroByGetAndQualityAndLevelAndId);
        this.roleUI.heroList.numItems = _heroUnits.Count;
        for (int i = 0; i < _heroUnits.Count; i++)
        {
            if (_heroInfo.HeroUnit.Id == _heroUnits[i].Id)
            {
                this.roleUI.heroList.selectedIndex = i;
                this.roleUI.heroList.ScrollToView(i);
                break;
            }
        }

        //var roleMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroLevelUp);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_AddHeroLvBtn) && roleMap.Item1)
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.roleUI.roleSelectBottom.levelUpBtn, GuideID.Trigger_Click_AddHeroLvBtn, PosType.Left, true, true);
        //}

        UpdateHeroLevelUpItemUpdate();
    }

    private void UpdateHeroInfos()
    {
        _heroUnits.Sort(SortHeroByGetAndQualityAndLevelAndId);
        this.roleUI.heroList.numItems = _heroUnits.Count;
        UpdateRoleAttr();
    }

    /// <summary>
    /// 排序按照  已拥有>品质>等级>Id
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static int SortHeroByGetAndQualityAndLevelAndId(ConfigHeroUnit a, ConfigHeroUnit b)
    {
        int aLevel = 0;
        int bLevel = 0;
        int aGet = HeroInfoManager.Instance.GetThisHero(a.Id) == null ? 0 : 1;
        int bGet = HeroInfoManager.Instance.GetThisHero(b.Id) == null ? 0 : 1;

        if (aGet == 0)
        {
            int needItemCntA = ItemInfoManager.Instance.GetItemCount(a.Item);
            if (needItemCntA >= a.Conflate)
            {
                aGet = 1;
                aLevel = 1;
            }
        }
        else
        {
            aLevel = HeroInfoManager.Instance.GetThisHero(a.Id).Level;
        }
        
        if (bGet == 0)
        {
            int needItemCntB = ItemInfoManager.Instance.GetItemCount(b.Item);
            if (needItemCntB >= b.Conflate)
            {
                bGet = 1;
                bLevel = 1;
            }
        }
        else
        {
            bLevel = HeroInfoManager.Instance.GetThisHero(b.Id).Level;
        }

        int result = aGet > bGet ? -1 : (aGet == bGet ? 0 : 1);//已拥有
        if (result == 0)
        {
            result = a.HeroQuality > b.HeroQuality ? -1 : (a.HeroQuality == b.HeroQuality ? 0 : 1);//品质
        }

        if (result == 0 && aGet == 1 && bGet == 1)//已拥有的才用等级
        {
            result = aLevel > bLevel ? -1 : (aLevel == bLevel ? 0 : 1);
        }

        if (result == 0)
        {
            result = a.Id > b.Id ? 1 : -1;//按照Id
        }

        return result;
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.roleUI.spine);
    }

    private void UpdateHeroLevelUpItemUpdate()
    {
        int adTimes = AdManager.Instance.GetAdFreeTimes((int) ePlayerAttrID.ePlayerAttrID_HeroExpItemFreeTimes);
        this.roleUI.roleSelectBottom.adWatchGroup.visible = adTimes > 0;
        
        int cnt = ItemInfoManager.Instance.GetItemCount(int.Parse(_common100002.Param1));
        this.roleUI.roleSelectBottom.levelUpBtn.levelUpBtn.itemCnt.SetVar("cur", cnt.ToString()).SetVar("need", "1").FlushVars();
        this.roleUI.roleSelectBottom.levelUpBtn.levelUpBtn.grayed = cnt <= 0;
        this.roleUI.roleSelectBottom.levelUpBtn.levelUpBtn.colorType.selectedIndex = cnt >= 1 ? 0 : 1;
    }
    
    private void OnItemUpdate()
    {
        int cnt = ItemInfoManager.Instance.GetItemCount(int.Parse(_common100002.Param1));
        this.roleUI.roleSelectBottom.levelUpBtn.levelUpBtn.itemCnt.SetVar("cur", cnt.ToString()).SetVar("need", "1").FlushVars();
        this.roleUI.roleSelectBottom.levelUpBtn.levelUpBtn.grayed = cnt <= 0;
        this.roleUI.roleSelectBottom.levelUpBtn.levelUpBtn.colorType.selectedIndex = cnt >= 1 ? 0 : 1;

        // 金币
        ConfigItemTypeUnit itemTypeUnit1 = ConfigUtils.GetConfigItemTypeUnitById(2000);
        // ((UI_Currency)(this.roleUI.btnGold)).txtValue.text = ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_HeroLevelId).ToString();
        // ((UI_Currency)(this.roleUI.btnGold)).icon = UIResource.GetItemUrl(itemTypeUnit1?.Icon);
        // ((UI_Currency) (this.roleUI.btnGold)).data = itemTypeUnit1;
        // ((UI_Currency)(this.roleUI.btnGold)).onClick.Add(this.OnClickBtnTips);
        ((UI_RoleCurrency)(this.roleUI.btnGold2)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.gold);//ItemInfoManager.Instance.GetItemCount(2000).ToString();
        ((UI_RoleCurrency)(this.roleUI.btnGold2)).icon = UIResource.GetItemUrl(itemTypeUnit1?.Icon);
        ((UI_RoleCurrency) (this.roleUI.btnGold2)).data = itemTypeUnit1;
        ((UI_RoleCurrency)(this.roleUI.btnGold2)).onClick.Add(this.OnClickBtnTips);
        
        // 技能强化石
        ConfigItemTypeUnit itemTypeUnit2 = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_HeroSkillUp);
        ((UI_RoleCurrency)(this.roleUI.btnStone)).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_HeroSkillUp));
        ((UI_RoleCurrency)(this.roleUI.btnStone)).icon = UIResource.GetItemUrl(itemTypeUnit2?.Icon);
        ((UI_RoleCurrency) (this.roleUI.btnStone)).data = itemTypeUnit2;
        ((UI_RoleCurrency)(this.roleUI.btnStone)).onClick.Add(this.OnClickBtnTips);
        
        // 奶酪
        ConfigItemTypeUnit itemTypeUnit3 = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_HeroLevelId);
        ((UI_RoleCurrency)(this.roleUI.btnUpLv)).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_HeroLevelId));
        ((UI_RoleCurrency)(this.roleUI.btnUpLv)).icon = UIResource.GetItemUrl(itemTypeUnit3?.Icon);
        ((UI_RoleCurrency) (this.roleUI.btnUpLv)).data = itemTypeUnit3;
        ((UI_RoleCurrency)(this.roleUI.btnUpLv)).onClick.Add(this.OnClickBtnTips);
        
        // 突破道具
        ConfigItemTypeUnit itemTypeUnit4 = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_HeroBreakId);
        // ((UI_Currency)(this.roleUI.btnDia)).txtValue.text = ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_HeroBreakId).ToString();
        // ((UI_Currency)(this.roleUI.btnDia)).icon = UIResource.GetItemUrl(itemTypeUnit2?.Icon);
        // ((UI_Currency) (this.roleUI.btnDia)).data = itemTypeUnit2;
        // ((UI_Currency)(this.roleUI.btnDia)).onClick.Add(this.OnClickBtnTips);
        ((UI_RoleCurrency)(this.roleUI.btnBreak)).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_HeroBreakId));
        ((UI_RoleCurrency)(this.roleUI.btnBreak)).icon = UIResource.GetItemUrl(itemTypeUnit4?.Icon);
        ((UI_RoleCurrency) (this.roleUI.btnBreak)).data = itemTypeUnit4;
        ((UI_RoleCurrency)(this.roleUI.btnBreak)).onClick.Add(this.OnClickBtnTips);

        // 突破
        // ConfigItemTypeUnit itemTypeUnit5 = ConfigUtils.GetConfigItemTypeUnitById(_heroInfo.HeroUnit.Item);
        // ((UI_Currency)(this.roleUI.btnSplitItem)).txtValue.text = ItemInfoManager.Instance.GetItemCount(_heroInfo.HeroUnit.Item).ToString();
        // ((UI_Currency)(this.roleUI.btnSplitItem)).icon = UIResource.GetItemUrl(itemTypeUnit3?.Icon);
        // if (_heroInfo.HeroUnit.Id != HeroInfoManager.Instance.GetHeroFirstID())
        // {
        //     ((UI_RoleCurrency)(this.roleUI.btnBreak)).icon = UIResource.GetItemUrl(itemTypeUnit4?.Icon);
        //     ((UI_RoleCurrency)this.roleUI.btnBreak).visible = true;
        // }
        // else
        // {
        //     ((UI_RoleCurrency)this.roleUI.btnBreak).visible = false;
        // }

    }

    private void OnClickBtnTips(EventContext context)
    {
        ConfigItemTypeUnit itemTypeUnit = (context.sender as UI_Currency).data as ConfigItemTypeUnit;
        if (itemTypeUnit != null)
        {
            TipsManger.Instance.ShowPopupTip((UI_Currency)context.sender, Tipstype.None, itemTypeUnit.Id);
        }
    }

    private void UpdateRoleInfo()
    {
        string strResName = ConfigUtils.GetHeroModelUIPathByID(_heroInfo.HeroUnit.Id);
        // float scale = 180f * Math.Min(1.2f,GRoot.contentScaleFactor);
        float scale = 120f * Math.Min(1f,GRoot.contentScaleFactor);
        if (_heroInfo.HeroUnit.Id == 20036 || _heroInfo.HeroUnit.Id == 20046)
        {
            scale = 145f;
        }
        Utils.SetSpineModelOnFGUI(this.roleUI.spine, strResName, scale, "idle", (o) =>
        {
            if(o is SkeletonAnimation animation)
                _spine = animation;
        });
        
        this.roleUI.roleName.text = ConfigUtils.GetTextById(_heroInfo.HeroUnit.Name);
        
        this.roleUI.occupationLb.url = UIResource.GetRoleOccupationImg(_heroInfo.HeroUnit.Vocation.ToString());
        this.roleUI.attrLb.url = UIResource.GetRoleAttrImgImg(_heroInfo.HeroUnit.VocationAttr.ToString());

        ((UI_qualityLabel) this.roleUI.roleName).qualityCtrl.selectedIndex = _heroInfo.HeroUnit.HeroQuality-1;
        ((UI_roleQualityItem) this.roleUI.qIcon).quality.selectedIndex = _heroInfo.HeroUnit.HeroQuality-1;

        ConfigSkillUnit activeSkillCfg = ConfigUtils.GetSkillById(_heroInfo.HeroUnit.ActiveSkill);
        // this.roleUI.roleSkillIcon.url = UIResource.GetItemUrl(activeSkillCfg.SkillIcon);
        this.roleUI.roleSkillIcon.url = UIResource.GetHeroSkillIcon(activeSkillCfg.SkillIcon);
        this.roleUI.skillNameLb.text = ConfigUtils.GetTextById(activeSkillCfg.Name);
        // this.roleUI.cdLb.SetVar("value", (activeSkillCfg.Cd * ConstDefine.CONFIG_PLACE_EX).ToString("")).FlushVars();
        
        int heroId = _heroInfo.HeroUnit.Id;
        var skillDict = HeroInfoManager.Instance.GetHeroSkillLevelUp();
        int curSkillLv = skillDict.TryGetValue(heroId, out int level) ? level : 1;
        this.roleUI.skillLv.SetVar("cur",curSkillLv.ToString()).FlushVars();
        
        
        // 获取下一级技能配置
        ConfigHeroSkillUnit nextSkillUnit = ConfigUtils.GetHeroNextSKillUnitByHeroIdAndSkillLv(_heroInfo.HeroUnit.Id, curSkillLv);
        int costGold = nextSkillUnit == null ? 0 : nextSkillUnit.Gold;
        int costStone = nextSkillUnit == null ? 0: nextSkillUnit.SkillGem;
        

        this.roleUI.cost1.icon = UIResource.GetItemUrl(2000.ToString());
        this.roleUI.cost1.num.text = costGold.ToString();
        this.roleUI.cost2.icon = UIResource.GetItemUrl(1010004.ToString());
        this.roleUI.cost2.num.text =  costStone.ToString();
        
        this.roleUI.cost1.status.selectedIndex = DataManager.Instance.GetRoleData().gold < costGold ? 1 : 0;
        this.roleUI.cost2.status.selectedIndex = ItemInfoManager.Instance.GetItemCount(1010004) < costStone ? 1 : 0;
        
        // 技能强化功能解锁
        var heroSkillLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroSkillLvUp);
        if (!heroSkillLvUpMap.Item1)
        {
            this.roleUI.strongLockIcon.visible = true;
            this.roleUI.strongCtrl.selectedIndex = 1;
        }
        else
        {
            // 已解锁
            this.roleUI.strongLockIcon.visible = false;
            // this.roleUI.strongCtrl.selectedIndex = 0;
            
            // 主动技能当前等级大于最高等级
            if (curSkillLv >= ConfigUtils.GetHeroSKillMaxLvByHeroId(_heroInfo.HeroUnit.Id).SkillLevel)
            {
                this.roleUI.strongCtrl.selectedIndex = 1;
                ((UI_comBtn)this.roleUI.strongBtn).isMax.selectedIndex = 1;
            }
            else
            {
                this.roleUI.strongCtrl.selectedIndex = 0;
                ((UI_comBtn)this.roleUI.strongBtn).isMax.selectedIndex = 0;
            }
        }
        
        // 主动技能当前等级大于最高等级
        // if (curSkillLv >= ConfigUtils.GetHeroSKillMaxLvByHeroId(_heroInfo.HeroUnit.Id).SkillLevel)
        // {
        //     this.roleUI.strongCtrl.selectedIndex = 1;
        //     ((UI_comBtn)this.roleUI.strongBtn).isMax.selectedIndex = 1;
        // }
        // else
        // {
        //     this.roleUI.strongCtrl.selectedIndex = 0;
        //     ((UI_comBtn)this.roleUI.strongBtn).isMax.selectedIndex = 0;
        // }
        
        if (!HeroInfoManager.Instance.GetHeroSkillLevelUp().ContainsKey(_heroInfo.HeroUnit.Id))
        {
            GButton strongBtn = this.roleUI.strongBtn;
            strongBtn.visible = false;
        }
        else
        {
            GButton strongBtn = this.roleUI.strongBtn;
            strongBtn.visible = true;
        }
            
        this.roleUI.roleSkillIcon.onClick.Set(this.OnClickSkillIconToShowEff);
        
        _heroAttrUnits = ConfigUtils.GetHeroAttrsByHeroId(_heroInfo.HeroUnit.Id);

        UpdateRoleAttr();

        this.roleUI.roleSelectBottom.uploadCtrl.selectedIndex = _heroInfo == HeroInfoManager.Instance.GetMyHero() ? 1 : 0;
        
        var stateMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroLevelUp);
        this.roleUI.roleSelectBottom.levelLock.selectedIndex = stateMap.Item1 ? 0 : 1;
    }

    private void UpdateRoleAttr()
    {
        int heroId = _heroInfo.HeroUnit.Id;
        var skillDict = HeroInfoManager.Instance.GetHeroSkillLevelUp();
        int curSkillLv = skillDict.TryGetValue(heroId, out int level) ? level : 1;
        this.roleUI.skillLv.SetVar("cur",curSkillLv.ToString()).FlushVars();
        ConfigSkillUnit activeSkillCfg = ConfigUtils.GetSkillById(_heroInfo.HeroUnit.ActiveSkill);
        this.roleUI.skillDesc.text = StringUtils.Format(ConfigUtils.GetTextById(activeSkillCfg.SkillDes), (_heroInfo.SkillDamageRate*ConstDefine.CONFIG_PLACE).ToString("f2"));
        
        this.roleUI.roleLv.SetVar("value",_heroInfo.Level.ToString()).FlushVars();
        this.roleUI.expBar.min = 0;
        ((UI_HeroBarExp) this.roleUI.expBar).maxCtrl.selectedIndex = 0;
        this.roleUI.roleSelectBottom.tupoLock.selectedIndex = 0;
        ConfigHeroLevelUnit levelUnit = ConfigUtils.GetHeroLevel(_heroInfo.HeroUnit.Id, _heroInfo.Level+1);
        if (levelUnit != null)
        {
            this.roleUI.expBar.max = levelUnit.Exp;
            if (!_heroInfo.IsGet)
            {
                int needItemCnt = ItemInfoManager.Instance.GetItemCount(_heroInfo.HeroUnit.Item);
                if (needItemCnt >= _heroInfo.HeroUnit.Conflate)
                {
                    this.roleUI.optCtrl.selectedIndex = 3;
                }
                else
                {
                    this.roleUI.optCtrl.selectedIndex = 2;
                }
                this.roleUI.hasCtrl.selectedIndex = 1;
            }
            else
            {
                this.roleUI.hasCtrl.selectedIndex = 0;
                ConfigHeroLevelUnit breakUnit = ConfigUtils.GetHeroLevel(_heroInfo.HeroUnit.Id, _heroInfo.Level);
                //判断是突破还是升级？ 
                if (breakUnit != null && breakUnit.Item != "0" && breakUnit.Level > _heroInfo.BreakLevel)//突破
                {
                    this.roleUI.optCtrl.selectedIndex = 1;
                    this.roleUI.maxCtrl.selectedIndex = 1;
                    ((UI_HeroBarExp) this.roleUI.expBar).maxCtrl.selectedIndex = 1;

                    var tupoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroTupo);
                    if (!tupoMap.Item1)
                    {
                        this.roleUI.roleSelectBottom.tupoLock.selectedIndex = 1;
                    }
                    else
                    {
                        this.roleUI.roleSelectBottom.tupoLock.selectedIndex = 0;
                    }

                }
                else//升级
                {
                    int cnt = ItemInfoManager.Instance.GetItemCount(int.Parse(_common100002.Param1));
                    this.roleUI.optCtrl.selectedIndex = 0;
                    this.roleUI.maxCtrl.selectedIndex = 0;
                }
     
            }
        }
        else
        {
            if (!_heroInfo.IsGet)
            {
                this.roleUI.hasCtrl.selectedIndex = 1;
                this.roleUI.maxCtrl.selectedIndex = 0;
                this.roleUI.optCtrl.selectedIndex = 2;
            }
            else
            {
                // 最大等级
                this.roleUI.hasCtrl.selectedIndex = 0;
                this.roleUI.maxCtrl.selectedIndex = 2;
                this.roleUI.optCtrl.selectedIndex = 0;
                ((UI_HeroBarExp) this.roleUI.expBar).maxCtrl.selectedIndex = 1;
            }

        }
        this.roleUI.expBar.value = _heroInfo.Exp;
       
        // if (_heroInfo.HeroUnit.Id == HeroInfoManager.Instance.GetHeroFirstID())
        // {
        //     this.roleUI.starList.numItems = 0;
        //     this.roleUI.levelAttrList.numItems = _heroAttrUnits.Count;//初始角色没有突破属性
        // }
        // else
        // {
        //     this.roleUI.starList.numItems = _heroInfo.BreakLevelLayer > 0 ? 3 : 0;
        //     this.roleUI.levelAttrList.numItems = _heroAttrUnits.Count + 1;//有一个是大于100级的突破属性
        // }
        
        this.roleUI.starList.numItems = _heroInfo.BreakLevelLayer > 0 ? 3 : 0;
        this.roleUI.levelAttrList.numItems = _heroAttrUnits.Count + 1;//有一个是大于100级的突破属性

        // ConfigItemTypeUnit itemTypeUnit3 = ConfigUtils.GetConfigItemTypeUnitById(_heroInfo.HeroUnit.Item);
        // ((UI_Currency)(this.roleUI.btnSplitItem)).txtValue.text = ItemInfoManager.Instance.GetItemCount(_heroInfo.HeroUnit.Item).ToString();
        // // ((UI_Currency)(this.roleUI.btnSplitItem)).icon = UIResource.GetItemUrl(itemTypeUnit3?.Icon);
        // if (_heroInfo.HeroUnit.Id != HeroInfoManager.Instance.GetHeroFirstID())
        // {
        //     ((UI_Currency)(this.roleUI.btnSplitItem)).icon = UIResource.GetItemUrl(itemTypeUnit3?.Icon);
        //     ((UI_Currency)this.roleUI.btnSplitItem).visible = true;
        // }
        // else
        // {
        //     ((UI_Currency)this.roleUI.btnSplitItem).visible = false;
        // }

        this.roleUI.roleSelectBottom.bottom.EnsureBoundsCorrect();
    }

    private void OnClickSkillIconToShowEff(EventContext context)
    {
        int skillLevelId = _heroInfo.HeroUnit.ActiveSkill;
        ConfigSkillUnit skillLevel = ConfigUtils.GetSkillById(skillLevelId);
        UIManager.Instance.ShowUIPanel("SkillEffectPre", skillLevel.AttackEffect);
    }

    private void OnClickUpLoadBtn()
    {
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralUploadSE);
        var builder = HeroReplaceInBattle_CS.CreateBuilder();
        builder.HeroId = (uint) _heroInfo.HeroUnit.Id;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroReplaceInBattle_CS, builder.Build());
    }

    private void UpdateBattleHero()
    {
        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
        lobbyView?.OpenBottomPanel(0, 0);
        UIManager.Instance.CloseAllUIPanelExcept("Lobby");
        MapObjectManager.Instance.CleanUp();
        MapObjectManager.Instance.InitGuanKaFSM(0);
        RoleManager.Instance.SetHeroSkillProxy();
        List<Engine.SkillInfo> skillInfos = SkillInfoManager.Instance.GetBattleSkillList();
        for (int i = 0; i < skillInfos.Count; i++)
        {
            RoleManager.Instance.SetSkillIdByIndex(skillInfos[i].SkillId, skillInfos[i].BattleIndex);
        }
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHANGE_HERO_TO_BATTLE);
    }
    
    private Coroutine _delayCoroutine;
    private WaitForSeconds _wait = new WaitForSeconds(.1f);
    private bool _isTouchStart;
    private bool _isReceived;
    private float _longPressTimer = 0f;
    private void OnTouchBeginItem(EventContext context)
    {
        var stateMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroLevelUp);
        if (!stateMap.Item1)
        {
            UIManager.Instance.Toast(stateMap.Item2);
            return;
        }
        HeroInfoManager.Instance.IsOperating = true;
        _longPressTimer = 0;
        UI_RoleOptAniBtn heroOptBtn = (context.sender as UI_RoleOptAniBtn);
        if(GuideManager.Instance.IsShowGuiding)
            GuideManager.Instance.HideGuide();
        _isTouchStart = true;
        _delayCoroutine = GameManager.Instance.StartCoroutine(DoDelayActionCoroutine(heroOptBtn)); 
    }

    private IEnumerator DoDelayActionCoroutine(UI_RoleOptAniBtn heroOptBtn)
    {
        while (_isTouchStart)
        {
            if(this.roleUI.optCtrl.selectedIndex == 1)
                yield break;
            if(this.roleUI.maxCtrl.selectedIndex == 2)
                yield break;
            int cnt = ItemInfoManager.Instance.GetItemCount(int.Parse(_common100002.Param1));
            if (cnt > 0)
            {
                heroOptBtn.t0.Play();
                GameManager.Instance.SoundManager.PlayEffect((int)SoundType.UpLvSE);
                float clickNum = 1;
                if (_longPressTimer > 3f)
                {
                    clickNum = Mathf.Min(1000, Mathf.Pow(2, Mathf.RoundToInt(_longPressTimer / 3)));
                }
                SendToLevelUpHero((int) clickNum);
            }
            else
            {
                //UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(10180, ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_HeroLevelId).Name));
                // UIManager.Instance.ShowUIPanel("BuryGiftPack", Gift_Bury.Gift_yxsj);

                LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum((int) Gift_Bury.Gift_yxsj);
                if (limitPackVo.BuyCounter >= ConfigUtils.GetGiftUnitsById((int) Gift_Bury.Gift_yxsj).BuyNumber)
                {
                    UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(10180, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_HeroLevelId).Name)));
                }
                else
                {
                    // UIManager.Instance.ShowUIPanel("BuryGiftPack", Gift_Bury.Gift_yxsj);// 关闭商业化
                }
                
                yield break;
            }
            yield return _isReceived;
            yield return _wait;
            _longPressTimer += 0.1f;
        }
    }

    private void OnTouchEndItem()
    {
        _isTouchStart = false;
        if (_delayCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(_delayCoroutine);
        }

        _longPressTimer = 0;
        HeroInfoManager.Instance.IsOperating = false;
    }

    private void SendToLevelUpHero(int clickNum)
    {
        _isReceived = false;

        clickNum = Mathf.Min(clickNum, ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_HeroLevelId));
        var builder = HeroAddExp_CS.CreateBuilder();
        builder.HeroId = (uint) _heroInfo.HeroUnit.Id;
        var itemBuilder = ItemInfo.CreateBuilder();
        itemBuilder.Id = ConstDefine.Item_HeroLevelId;
        itemBuilder.Num = clickNum;
        builder.UsedItemsList.Add(itemBuilder.Build());
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroAddExp_CS, builder.Build());
    }

    private void OnClickBreakBtn()
    {
        var tupoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroTupo);
        if (!tupoMap.Item1)
        {
            UIManager.Instance.Toast(tupoMap.Item2);
        }
        else
        {
            UIManager.Instance.ShowUIPanel("RoleToBreak", _heroInfo);
        }

    }

    private void OnClickGetHeroBtn()
    {
        var summonHeroMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonHero);
        if(summonHeroMap.Item1)
            UIManager.Instance.ShowUIPanel("SummonHero");
        else
        {
            UIManager.Instance.Toast(summonHeroMap.Item2);
        }
    }

    private void HeroItemRender(int index, GObject item)
    {
        ConfigHeroUnit heroUnit = _heroUnits[index];
        ((UI_RoleItem) item).qIcon.quality.selectedIndex = heroUnit.HeroQuality-1;
        ((UI_RoleItem) item).state.selectedIndex = 0;
        ((UI_RoleItem) item).icon = UIResource.GetHeroBody(heroUnit.HeroBody);
        // ((UI_RoleItem) item).roleName.text = heroUnit.Name;
        ((UI_RoleItem) item).roleName.text = ConfigUtils.GetTextById(heroUnit.Name);
        ((UI_RoleItem) item).quality.selectedIndex = heroUnit.HeroQuality - 1;
        HeroInfo heroInfo = HeroInfoManager.Instance.GetThisHero(heroUnit.Id);
        ((UI_RoleItem) item).hcBtn.onClick.Clear();
        ((UI_RoleItem) item).redDot.visible = false;
        if (heroInfo == null)
        {
            int needItemCnt = ItemInfoManager.Instance.GetItemCount(heroUnit.Item);
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(heroUnit.Item);
            ((UI_RoleItem) item).splitItemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
            ((UI_RoleItem) item).needItem.SetVar("cur",needItemCnt.ToString()).SetVar("need", heroUnit.Conflate.ToString()).FlushVars();
            ((UI_RoleItem) item).state.selectedIndex = 1;
            ((UI_RoleItem) item).hasLv.selectedIndex = 1;
            if (needItemCnt >= heroUnit.Conflate)
            {
                ((UI_RoleItem) item).redDot.visible = true;
                ((UI_RoleItem) item).compondCtrl.selectedIndex = 1;
            }
            else
            {
                ((UI_RoleItem) item).compondCtrl.selectedIndex = 0;
            }
        }
        else
        {
            ((UI_RoleItem) item).lvLb.SetVar("value",heroInfo.Level.ToString()).FlushVars();
            ((UI_RoleItem) item).state.selectedIndex = 0;
            ((UI_RoleItem) item).hasLv.selectedIndex = 0;

            bool isUpload = HeroInfoManager.Instance.GetMyHero() == heroInfo;
            ((UI_RoleItem) item).upLoadCtrl.selectedIndex = isUpload ? 0 : 1;
        }

    }

    private void OnClickHCBtn(EventContext context)
    {
        if (_heroInfo.HeroUnit != null)
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.HeroCompoundSE);

            //发送合成协议
            var builder = HeroPiecesMerge_CS.CreateBuilder();
            builder.HeroId = (uint) _heroInfo.HeroUnit.Id;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroPiecesMerge_CS, builder.Build());
        }
    }

    private void OnClickHeroListItem(EventContext context)
    {
        UI_RoleItem item = context.data as UI_RoleItem;
        int index = this.roleUI.heroList.GetChildIndex(item);
        ConfigHeroUnit heroUnit = _heroUnits[index];
        HeroInfo heroInfo = HeroInfoManager.Instance.GetThisHero(heroUnit.Id);
        if (heroInfo != null)
            _heroInfo = heroInfo;
        else
        {
            _heroInfo = new HeroInfo()
            {
                HeroUnit = heroUnit,
                BreakLevel = 0,
                BreakLevelLayer = 0,
                Exp = 0,
                Level = 1,
                IsGet = false,
                SkillDamageRate = ConfigUtils.GetHeroSkillDamage(heroUnit.Id, 1)
            };
        }

        UpdateRoleInfo();
    }

    private void OnClickTipsBtn()
    {
        // UIManager.Instance.ShowUIPanel("RoleHelp");
        UIManager.Instance.ShowUIPanel("Help",HelpType.Helo_RoleHelp);
    }
    
    private void HeroLevelAttrItemRender(int index, GObject item)
    {
        if (index != _heroAttrUnits.Count)
        {
            ((UI_RoleAttrItem) item).typeCtrl.selectedIndex = 0;
            ConfigHeroAttrUnit heroAttr = _heroAttrUnits[index];
            ((UI_RoleAttrItem) item).lockCtrl.selectedIndex = _heroInfo.Level >= heroAttr.Level ? 0 : 1;
            // ((UI_RoleAttrItem) item).attrCtrl.selectedIndex = heroAttr.AttrId - 1;
            ((UI_RoleAttrItem)item).pIcon.url = UIResource.GetAttrIconById(heroAttr.AttrId.ToString());
            if (((UI_RoleAttrItem) item).lockCtrl.selectedIndex == 1)
            {
                ((UI_RoleAttrItem) item).lvLb.SetVar("value", heroAttr.Level.ToString()).FlushVars();
            }
        }
        else
        {
            var starParam = Utils.GetHeroStar(_heroInfo.BreakLevelLayer);
            for (int i = 0; i < 3; i++)
            {
                ((UI_RoleStarItem) ((UI_RoleAttrItem) item).GetChild("star" + i)).lockCtrl.selectedIndex= starParam.Item2 > i ? 0 : 1;
                ((UI_RoleStarItem) ((UI_RoleAttrItem) item).GetChild("star" + i)).type.selectedIndex = starParam.Item1;
            }
            (double, int) attrParam = ConfigUtils.GetHeroAttrsByBreakLevel(_heroInfo.HeroUnit.Id, _heroInfo.BreakLevel);
            // ((UI_RoleAttrItem) item).attrCtrl.selectedIndex = attrParam.Item2-1;
            ((UI_RoleAttrItem)item).pIcon.url = UIResource.GetAttrIconById(attrParam.Item2.ToString());
            ((UI_RoleAttrItem) item).starCount.selectedIndex = starParam.Item2;
            ((UI_RoleAttrItem) item).typeCtrl.selectedIndex = 1;
            ((UI_RoleAttrItem) item).lockCtrl.selectedIndex = starParam.Item2 == 0 ? 1 : 0;
            ((UI_RoleAttrItem) item).starLb.text = _heroInfo.BreakLevelLayer.ToString();
        }
        
        ((UI_RoleAttrItem) item).data = index;
        ((UI_RoleAttrItem) item).onClick.Set(OnHeroLevelAttrTips);
    }
    
    private void OnHeroLevelAttrTips(EventContext context)
    {
        int index = (int)((UI_RoleAttrItem)context.sender).data;
        if (index >= 0 && index < _heroAttrUnits .Count)
        {
            ConfigHeroAttrUnit heroAttr = _heroAttrUnits[index];
            // TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, StringUtils.ConvertToAttributeValue(heroAttr.AttrId, heroAttr.Value), _heroInfo.Level>=heroAttr.Level, heroAttr.Level,heroAttr.AttrId );
            
            int attrId = heroAttr.AttrId;
            double value = double.Parse(heroAttr.Value.ToString());
            string lastValue = EquipManager.Instance.SetAttributeValue(attrId, value,  true);
            TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, attrId, lastValue, _heroInfo.Level>=heroAttr.Level, heroAttr.Level);
        }
        else
        {
            int breakLevel = _heroInfo.BreakLevel;
            if (breakLevel == 0)
            {
                breakLevel = 100;
            }
            (double, int) attrParam = ConfigUtils.GetHeroAttrsByBreakLevel(_heroInfo.HeroUnit.Id, breakLevel);
            int attrId = attrParam.Item2;
            double value = attrParam.Item1;
            string lastValue = EquipManager.Instance.SetAttributeValue(attrId, value,  true);
            TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, attrId, lastValue, true, 0);
        }
    }
    
    private void HeroStarListRender(int index, GObject item)
    {
        var starParam = Utils.GetHeroStar(_heroInfo.BreakLevelLayer);
        ((UI_RoleStarItem) item).type.selectedIndex = starParam.Item1;
        ((UI_RoleStarItem) item).lockCtrl.selectedIndex = starParam.Item2 > index ? 0 : 1;
    }

    private void OnClickAdBtn()
    {
        AdManager.Instance.WatchAd(() =>
        {
            var builder = HeroExpItemByFreeAD_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroExpItemByFreeAD_CS, builder.Build());
        });
    }

    private void UpdateHeroBreakUp()
    {
        _isReceived = true;
        UpdateRoleAttr();
        UIManager.Instance.ShowUIPanel("RoleBreak", _heroInfo);
    }
    
    private void UpdateHeroLevelUp(HeroInfo heroInfo, int preLevel)
    {
        ConfigHeroAttrUnit heroAttrUnit = HeroInfoManager.Instance.HasUnlockHeroAttr(_heroInfo.HeroUnit.Id, preLevel, heroInfo.Level);
        if (heroAttrUnit != null)
        {
            UIManager.Instance.ShowUIPanel("RoleLevelUpGet", _heroInfo, heroAttrUnit);
            OnTouchEndItem();
        }
        _isReceived = true;
        _heroInfo = heroInfo;
        UpdateRoleAttr();
        ConfigSkillUnit activeSkillCfg = ConfigUtils.GetSkillById(_heroInfo.HeroUnit.ActiveSkill);
        this.roleUI.heroList.numItems = _heroUnits.Count;
    }

    private void UpdateHeroMergeInfo(HeroInfo heroInfo)
    {
        GetPetRewardView.GetPetParam getPetParam = new GetPetRewardView.GetPetParam();
        getPetParam.Id = heroInfo.HeroUnit.Id;
        getPetParam.IsRole = true;
        getPetParam.CloseCallback = null;
        UIManager.Instance.ShowUIPanel("GetPetReward", getPetParam);
        _heroInfo = heroInfo;
        _heroUnits.Sort(SortHeroByGetAndQualityAndLevelAndId);
        this.roleUI.heroList.numItems = _heroUnits.Count;
        UpdateRoleInfo();
        UpdateRoleAttr();
        
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(17);
    }
    
    // 主动技能升级
    private void OnClickStrongBtn()
    {
        // 技能强化功能解锁
        var heroSkillLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroSkillLvUp);
        if (!heroSkillLvUpMap.Item1)
        {
            UIManager.Instance.Toast(heroSkillLvUpMap.Item2);
            return;
        }
        
        // 升级材料不足时，按钮状态变更，无法升级
        int curSkillLv = HeroInfoManager.Instance.GetHeroSkillLevelUp()[_heroInfo.HeroUnit.Id];//当前主动技能等级
        
        ConfigHeroSkillUnit nextSkillUnit = ConfigUtils.GetHeroNextSKillUnitByHeroIdAndSkillLv(_heroInfo.HeroUnit.Id, curSkillLv);
        int costGold = nextSkillUnit == null ? 0 : nextSkillUnit.Gold;//升级需消耗的金币数量
        int costStone = nextSkillUnit == null ? 0: nextSkillUnit.SkillGem;//升级需消耗的强化石数量
        
        // 技能等级不可超过角色等级-现在没有这个限制了
        // if (curSkillLv < _heroInfo.Level)
        // {
        //
        //     if (DataManager.Instance.GetRoleData().gold >= costGold && ItemInfoManager.Instance.GetItemCount(1010004) >= costStone)
        //     {
        //         var builder = HeroSkillLevelUp_CS.CreateBuilder();
        //         builder.HeroId = (uint) _heroInfo.HeroUnit.Id;
        //         builder.LevelUpValues = 1;//升级一级，后续需要一次升多级再修改
        //         GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroSkillLevelUp_CS, builder.Build());
        //     }
        //     else
        //     {
        //         UIManager.Instance.ToastByKey(10081);
        //     }
        //     
        // }
        // else
        // {
        //     if (curSkillLv >= ConfigUtils.GetHeroSKillMaxLvByHeroId(_heroInfo.HeroUnit.Id).SkillLevel)
        //     {
        //         UIManager.Instance.ToastByKey(10014);
        //     }
        //     else
        //     {
        //         UIManager.Instance.ToastByKey(8039);
        //     }
        // }
        
        
        if (DataManager.Instance.GetRoleData().gold >= costGold && ItemInfoManager.Instance.GetItemCount(1010004) >= costStone)
        {
            var builder = HeroSkillLevelUp_CS.CreateBuilder();
            builder.HeroId = (uint) _heroInfo.HeroUnit.Id;
            builder.LevelUpValues = 1;//升级一级，后续需要一次升多级再修改
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroSkillLevelUp_CS, builder.Build());
        }
        else
        {
            UIManager.Instance.ToastByKey(10081);
        }
        
    }
}
