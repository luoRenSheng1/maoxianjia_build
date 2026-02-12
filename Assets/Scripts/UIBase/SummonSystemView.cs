
using BestHTTP.Extensions;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Summon;
using System.Collections.Generic;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class SummonSystemView : UIViewBase
{
    private UI_SummonSystem SummonSystem => this.main as UI_SummonSystem;

    private int _openIndex = -1;

    private List<ConfigGiftUnit> _giftUnits;
    private List<ConfigGiftUnit> _currentGiftUnits = new List<ConfigGiftUnit>();

    private List<ConfigPayListUnit> _payList;
    private int _preSelectIndex;

    private ConfigCommonUnit _common3006;
    
    private JumpTypeEnum _jumpTypeEnum;  //是否显示手指
    
    public SummonSystemView()
    {
        this.name = "SummonSystem";
        this.package = "Summon";
        this.component = "SummonSystem";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        SummonBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        
        _jumpTypeEnum = JumpTypeEnum.Normal;
        if (values[0] != null)
        {
            if (values[0] is int)
                _openIndex = (int)values[0];
            else if (values[0] is JumpTypeEnum)
                _jumpTypeEnum = (JumpTypeEnum)values[0];
        }
        // UpdateUI();
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        this.SummonSystem.tabList.onClickItem.Add(OnClickBottomItem);

        this.SummonSystem.summonInfo.summonListShade.summonList.itemRenderer = SummonItemRender;
        this.SummonSystem.specialInfo.specialList.itemRenderer = SpecialItemRender;

        this.SummonSystem.giftInfo.summonListShade.giftList.itemRenderer = GiftItemRender;
        _giftUnits = ConfigUtils.GetGiftUnitsByType(GiftType.GiftPack);

        _payList = ConfigUtils.GetPayListUnitsByType();
        this.SummonSystem.diamondInfo.diamondList.itemRenderer = ChargeItemListRender;

        _common3006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3006);

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PET_LOTTERY_SUCCESS, this.UpdateSummonInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SKILL_LOTTERY_SUCCESS, this.UpdateSummonInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_HERO_LOTTERY_SUCCESS, this.UpdateSpecialRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECHARGE_SUCCESS, this.UpdateDiamondInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECHARGE_GIFT_UPDATE, this.UpdateGiftInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SKILL_LOTTERY_SUCCESS, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PET_LOTTERY_SUCCESS, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.RefreshBottomRedDot);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PET_LOTTERY_SUCCESS, this.UpdateSummonInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SKILL_LOTTERY_SUCCESS, this.UpdateSummonInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_HERO_LOTTERY_SUCCESS, this.UpdateSpecialRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECHARGE_SUCCESS, this.UpdateDiamondInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECHARGE_GIFT_UPDATE, this.UpdateGiftInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SKILL_LOTTERY_SUCCESS, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PET_LOTTERY_SUCCESS, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.RefreshBottomRedDot);
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (!GuideManager.Instance.IsShowGuiding && _jumpTypeEnum != JumpTypeEnum.Normal)
        {
            if (_jumpTypeEnum == JumpTypeEnum.CallPet || _jumpTypeEnum == JumpTypeEnum.CallSkill)
                _openIndex = 0;
            else if (_jumpTypeEnum == JumpTypeEnum.CallHero)
                _openIndex = 3;
        }
        
        OnRoleUpdate();
        if (_openIndex != -1)
        {
            ChangeIndex(_openIndex); ;
        }
        else
        {
            if (this.SummonSystem.tabList.selectedIndex == -1)
                this.SummonSystem.tabList.selectedIndex = 0;
            ChangeIndex(this.SummonSystem.tabList.selectedIndex);
        }
        //if (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_SummonPet))
        //{
        //    ChangeIndex(0);
        //}
        UpdateSpecialRedDot();
        RefreshBottomRedDot();

        ShowFinger();
    }

    private void ShowFinger()
    {
        if (_jumpTypeEnum == JumpTypeEnum.Normal) return;
        
        if (_jumpTypeEnum == JumpTypeEnum.CallPet)
        {
            var pet30Btn = ((UI_SummonItem)this.SummonSystem.summonInfo.summonListShade.summonList.GetChildAt(0)).Summon30;
            var globalPos = pet30Btn.LocalToGlobal(Vector2.zero);
            Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(globalPos);
            JumpManager.Instance.ShowFinger(_jumpTypeEnum, pet30Btn, (int)logicScreenPos.x, (int)logicScreenPos.y);
        }else if (_jumpTypeEnum == JumpTypeEnum.CallSkill)
        {
            var skill30Btn = ((UI_SummonItem)this.SummonSystem.summonInfo.summonListShade.summonList.GetChildAt(1)).Summon30;
            var globalPos = skill30Btn.LocalToGlobal(Vector2.zero);
            Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(globalPos);
            JumpManager.Instance.ShowFinger(_jumpTypeEnum, skill30Btn, (int)logicScreenPos.x, (int)logicScreenPos.y);
        }else if (_jumpTypeEnum == JumpTypeEnum.CallHero)
        {
            var goBtn = ((UI_SpecialItem)this.SummonSystem.specialInfo.specialList.GetChildAt(0)).goBtn;
            var globalPos = goBtn.LocalToGlobal(Vector2.zero);
            Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(globalPos);
            JumpManager.Instance.ShowFinger(_jumpTypeEnum, goBtn, (int)logicScreenPos.x, (int)logicScreenPos.y);
        }
    }
    
    protected override void OnHide()
    {
        base.OnHide();
        _openIndex = -1;
        // this.SummonSystem.summonInfo.bannerBg.url = null;
        this.SummonSystem.specialInfo.bannerBg.url = null;
        this.SummonSystem.giftInfo.bannerBg.url = null;
        this.SummonSystem.diamondInfo.bannerBg.url = null;
        Utils.ClearSpineModelOnFGUI(this.SummonSystem.giftInfo.spine);
        Utils.ClearSpineModelOnFGUI(this.SummonSystem.diamondInfo.spine);
    }

    private void OnClickBottomItem(EventContext context)
    {
        GButton item = context.data as GButton;
        var index = this.SummonSystem.tabList.GetChildIndex(item);

        ChangeIndex(index);
    }

    private void ChangeIndex(int index)
    {
        switch (index)
        {
            case 0:
                _preSelectIndex = index;
                this.UpdateSummonInfo();
                this.SummonSystem.summonInfo.summonListShade.summonList.EnsureBoundsCorrect();
                var skill10Btn = ((UI_SummonItem)this.SummonSystem.summonInfo.summonListShade.summonList.GetChildAt(1)).Summon10;
                if (GuideManager.Instance.GuideIsComplete(GuideID.Click_SummonSkill))
                    skill10Btn.ctrl.selectedIndex = 0;
                var pet10Btn = ((UI_SummonItem)this.SummonSystem.summonInfo.summonListShade.summonList.GetChildAt(0)).Summon10;
                if (GuideManager.Instance.GuideIsComplete(GuideID.Click_SummonPet))
                    pet10Btn.ctrl.selectedIndex = 0;
                //引导-点击技能10抽
                if (GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.SummonSkill,
                    giding = GuideID.Click_Summon,
                    gid = GuideID.Click_SummonSkill,
                    tui = skill10Btn,
                    isForce = true,
                    isSend = true,
                    npcTxt = "Beginner_Doc_009",
                    npcPosType = PosType.Down,
                    isLucency = false
                    //skewing = new Vector2(0f, 100f)
                }))
                {
                    skill10Btn.ctrl.selectedIndex = 3;
                    skill10Btn.Free.x = skill10Btn.width * 0.5f - skill10Btn.Free.width * 0.5f;
                }
                else if (
                    //引导 - 点击宠物10抽
                    GuideManager.Instance.StarGuideByData(new GuideData()
                    {
                        fid = FuncOpenType.SummonPet,
                        //giding = GuideID.Click_Summon2,
                        gid = GuideID.Click_SummonPet,
                        tui = pet10Btn,
                        isForce = true,
                        isSend = true,
                        isLucency = false,
                        npcTxt = "Beginner_Doc_012",
                        npcPosType = PosType.Down,
                    })
                )
                {
                    pet10Btn.ctrl.selectedIndex = 3;
                    pet10Btn.Free.x = pet10Btn.width * 0.5f - pet10Btn.Free.width * 0.5f;
                }
                break;
            case 1:
                UIManager.Instance.Toast("暂未开放！");
                // var diaMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.DiamondStore);
                // if (!diaMap.Item1)
                // {
                //     UIManager.Instance.Toast(diaMap.Item2);
                // }
                // else
                // {
                //     _preSelectIndex = index;
                //     UpdateDiamondInfo();
                // }
                break;
            case 2:
                UIManager.Instance.Toast("暂未开放！");
                // var giftMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonGift);
                // if (!giftMap.Item1)
                // {
                //     UIManager.Instance.Toast(giftMap.Item2);
                // }
                // else
                // {
                //     _preSelectIndex = index;
                //     UpdateGiftInfo();
                // }
                break;
            case 3:
                // _preSelectIndex = index;
                // this.UpdateSpecialInfo();
                var heroMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonHero);
                if (!heroMap.Item1)
                {
                    UIManager.Instance.Toast(heroMap.Item2);
                    ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(index)).selected = false;
                }
                else
                {
                    _preSelectIndex = index;
                    this.UpdateSpecialInfo();
                }
                break;
        }
        for (int i = 0; i < 4; i++)
        {
            if (i == _preSelectIndex)
                ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(_preSelectIndex)).selected = true;
            else
            {
                ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(i)).selected = false;
            }
        }

        this.SummonSystem.tabCtrl.selectedIndex = _preSelectIndex;
    }

    private void RefreshBottomRedDot()
    {
        // var giftMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonGift);
        // if (!giftMap.Item1)
        // {
        //     ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 1;
        // }
        // else
        // {
        //     ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 0;
        // }
        
        
        // var diaMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.DiamondStore);
        // if (!diaMap.Item1)
        // {
        //     ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(1)).lockCtrl.selectedIndex = 1;
        // }
        // else
        // {
        //     ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(1)).lockCtrl.selectedIndex = 0;
        // }
        
        ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 1;
        ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(1)).lockCtrl.selectedIndex = 1;

        // 召唤-宠物、技能
        ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(0)).redCtrl.selectedIndex = ShopInfoManager.Instance.SummonPetAndSkillRedPoint() ? 1 : 0;
        
        // 召唤英雄-特殊
        var summonHeroMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonHero);
        if (!summonHeroMap.Item1)
        {
            ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(3)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(3)).lockCtrl.selectedIndex = 0;
            // ((UI_TabBtn)this.SummonSystem.tabList.GetChildAt(3)).redCtrl.selectedIndex = ;
        }

    }

    private void OnRoleUpdate()
    {
        ((UI_Currency)(this.SummonSystem.btnGold)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        ((UI_Currency)(this.SummonSystem.btnDia)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia);
        UpdateSummonInfo();
    }

    #region 宠物和技能抽卡

    private void UpdateSummonInfo()
    {
        this.SummonSystem.summonInfo.summonListShade.summonList.numItems = 2;
        // this.SummonSystem.summonInfo.bannerBg.url = UIResource.GetSummonUrl("n3");

        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_UPDATE);
    }

    private void SummonItemRender(int index, GObject item)
    {
        var itemCom = (UI_SummonItem)item;
        itemCom.typeCtrl.selectedIndex = index;
        RoleData roleData = DataManager.Instance.GetRoleData();
        itemCom.expBar.min = 0;
        // itemCom.itemBgUrl.url = UIResource.GetSummonUrl("n10");
        if (index == 0) //宠物
        {
            ConfigRaffleLevelUnit raffleUnit = ConfigUtils.GetRaffleUnit(roleData.petLotteryLv + 1, 1);
            if (raffleUnit == null)
            {
                ((UI_SummonBarExp)itemCom.expBar).maxCtrl.selectedIndex = 1;
            }
            else
            {
                ((UI_SummonBarExp)itemCom.expBar).maxCtrl.selectedIndex = 0;
                itemCom.expBar.value = roleData.petLotteryExp;
                itemCom.expBar.max = raffleUnit.Exp;
            }
            itemCom.lvLb.SetVar("value", roleData.petLotteryLv.ToString()).FlushVars();
            if (!GuideManager.Instance.GuideIsComplete(GuideID.Click_SummonPet))
            {//引导免费
                itemCom.Summon10.ctrl.selectedIndex = 3;
                itemCom.Summon10.Free.x = itemCom.Summon10.width * 0.5f - itemCom.Summon10.Free.width * 0.5f;
            }
            else
            {
                itemCom.Summon10.ctrl.selectedIndex = 0;
                itemCom.Summon10.tickNumLb.text = (ShopInfoManager.Instance.GetPetLotteryOne() * 10).ToString();
                itemCom.Summon10.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 10;
            }
            itemCom.Summon30.tickNumLb.text = (ShopInfoManager.Instance.GetPetLotteryOne() * 30).ToString();
            int adNum = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_PetLottoTimesByAd);
            var petItem = ShopInfoManager.Instance.GetPetLotterFreeAdTime();
            itemCom.adSummon.adLb.SetVar("cur", adNum.ToString()).SetVar("total", petItem.Item1.ToString()).FlushVars();
            itemCom.adSummon.adSummLb.SetVar("value", petItem.Item2.ToString()).FlushVars();
            itemCom.Summon30.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 30;
            itemCom.Summon30.redCtrl.selectedIndex = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 30 || ChkPetLimit(30, false) ? 0 : 1;
            itemCom.adSummon.grayed = adNum <= 0;
            itemCom.adSummon.redCtrl.selectedIndex = adNum <= 0 || ChkPetLimit(30, false) ? 0 : 1;
            itemCom.adSummon.data = 1;
            itemCom.adSummon.onClick.Add(this.OnClickPetSummonBtn);
            itemCom.Summon10.data = 2;
            itemCom.Summon10.onClick.Add(this.OnClickPetSummonBtn);
            itemCom.Summon30.data = 3;
            itemCom.Summon30.onClick.Add(this.OnClickPetSummonBtn);

            itemCom.tipsBtn.onClick.Add(this.OnClickPetTipsBtn);

            var petMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonPet);
            itemCom.lockCtrl.selectedIndex = petMap.Item1 ? 0 : 1;
            itemCom.lockDesc.text = petMap.Item2;
        }
        else if (index == 1)//技能
        {
            ConfigRaffleLevelUnit raffleUnit = ConfigUtils.GetRaffleUnit(roleData.SkillLotteryLv + 1, 2);
            if (raffleUnit == null)
            {
                ((UI_SummonBarExp)itemCom.expBar).maxCtrl.selectedIndex = 1;
            }
            else
            {
                ((UI_SummonBarExp)itemCom.expBar).maxCtrl.selectedIndex = 0;
                itemCom.expBar.value = roleData.SkillLotteryExp;
                itemCom.expBar.max = raffleUnit.Exp;
            }
            itemCom.lvLb.SetVar("value", roleData.SkillLotteryLv.ToString()).FlushVars();
            if (!GuideManager.Instance.GuideIsComplete(GuideID.Click_SummonSkill))
            {//引导免费
                itemCom.Summon10.ctrl.selectedIndex = 3;
                itemCom.Summon10.Free.x = itemCom.Summon10.width * 0.5f - itemCom.Summon10.Free.width * 0.5f;
            }
            else
            {
                itemCom.Summon10.ctrl.selectedIndex = 0;
                itemCom.Summon10.tickNumLb.text = (ShopInfoManager.Instance.GetSkillLotteryOne() * 10).ToString();
                itemCom.Summon10.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 10;
            }

            itemCom.Summon30.tickNumLb.text = (ShopInfoManager.Instance.GetSkillLotteryOne() * 30).ToString();
            int adNum = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_SkillLottoTimesByAd);
            var skillItem = ShopInfoManager.Instance.GetSkillLotterFreeAdTime();
            itemCom.adSummon.adLb.SetVar("cur", adNum.ToString()).SetVar("total", skillItem.Item1.ToString()).FlushVars();
            itemCom.adSummon.adSummLb.SetVar("value", skillItem.Item2.ToString()).FlushVars();
            itemCom.Summon30.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 30;
            itemCom.Summon30.redCtrl.selectedIndex = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 30 ? 0 : 1;
            itemCom.adSummon.grayed = adNum <= 0;
            itemCom.adSummon.redCtrl.selectedIndex = adNum <= 0 ? 0 : 1;
            itemCom.adSummon.data = 1;
            itemCom.adSummon.onClick.Add(this.OnClickSkillSummonBtn);
            itemCom.Summon10.data = 2;
            itemCom.Summon10.onClick.Add(this.OnClickSkillSummonBtn);
            itemCom.Summon30.data = 3;
            itemCom.Summon30.onClick.Add(this.OnClickSkillSummonBtn);

            itemCom.tipsBtn.onClick.Add(this.OnClickSkillTipsBtn);

            var skillMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
            itemCom.lockCtrl.selectedIndex = skillMap.Item1 ? 0 : 1;
            itemCom.lockDesc.text = skillMap.Item2;
        }
    }

    private void OnClickPetTipsBtn()
    {
        UIManager.Instance.ShowUIPanel("SummonProbability", 1);
    }

    private void OnClickSkillTipsBtn()
    {
        UIManager.Instance.ShowUIPanel("SummonProbability", 2);
    }

    private void OnClickPetSummonBtn(EventContext context)
    {
        int summonType = (int)((GButton)context.sender).data;
        switch (summonType) //type=1广告 type=2 10抽 type=3 30抽
        {
            case 1:
                int adNum = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_PetLottoTimesByAd);
                if (adNum > 0)
                {
                    if (this.ChkPetLimit(30))
                    {
                        return;
                    }
                    AdManager.Instance.WatchAd(() =>
                    {
                        var builder1 = PetLottery_CS.CreateBuilder();
                        builder1.IsByAd = true;
                        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_PetLottery_CS, builder1.Build());
                    });

                }
                else
                {
                    UIManager.Instance.ToastByKey(10185);
                }

                break;
            case 2:
                if (GuideManager.Instance.IsShowGuiding && GuideManager.Instance.GuideId == (int)GuideID.Click_SummonPet)
                {//引导中免费
                    var builder2 = FreePetLottery4Guide_CS.CreateBuilder();
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FreePetLottery4Guide_CS, builder2.Build());
                }
                else if (ShopInfoManager.Instance.GetPetLotteryOne() * 10 <= DataManager.Instance.GetRoleData().dia)
                {
                    if (this.ChkPetLimit(10))
                    {
                        return;
                    }
                    var builder2 = PetLottery_CS.CreateBuilder();
                    builder2.IsByAd = false;
                    builder2.PlayerTimes = 10;
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_PetLottery_CS, builder2.Build());
                }
                else
                {
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();//暂时注释
                }

                break;
            case 3:
                if (ShopInfoManager.Instance.GetPetLotteryOne() * 30 <= DataManager.Instance.GetRoleData().dia)
                {
                    if (this.ChkPetLimit(30))
                    {
                        return;
                    }
                    var builder3 = PetLottery_CS.CreateBuilder();
                    builder3.IsByAd = false;
                    builder3.PlayerTimes = 30;
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_PetLottery_CS, builder3.Build());
                }
                else
                {
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();//暂时注释
                }

                break;
        }
    }

    private void OnClickSkillSummonBtn(EventContext context)
    {
        int summonType = (int)((GButton)context.sender).data;
        switch (summonType)//type=1广告 type=2 10抽 type=3 30抽
        {
            case 1:
                int adNum = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_SkillLottoTimesByAd);
                if (adNum > 0)
                {
                    AdManager.Instance.WatchAd(() =>
                    {
                        var builder1 = SkillLottery_CS.CreateBuilder();
                        builder1.IsByAd = true;
                        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_SkillLottery_CS, builder1.Build());
                    });

                }
                else
                {
                    UIManager.Instance.ToastByKey(10185);
                }

                break;
            case 2:
                if (GuideManager.Instance.IsShowGuiding && GuideManager.Instance.GuideId == (int)GuideID.Click_SummonSkill)
                //if (true)
                {//引导中免费
                    var builder2 = FreeSkillLottery4Guide_CS.CreateBuilder();
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FreeSkillLottery4Guide_CS, builder2.Build());
                }
                else if (ShopInfoManager.Instance.GetSkillLotteryOne() * 10 <= DataManager.Instance.GetRoleData().dia)
                {
                    var builder2 = SkillLottery_CS.CreateBuilder();
                    builder2.IsByAd = false;
                    builder2.PlayerTimes = 10;
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_SkillLottery_CS, builder2.Build());
                }
                else
                {
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();//暂时注释
                }

                break;
            case 3:
                if (ShopInfoManager.Instance.GetSkillLotteryOne() * 30 <= DataManager.Instance.GetRoleData().dia)
                {
                    var builder3 = SkillLottery_CS.CreateBuilder();
                    builder3.IsByAd = false;
                    builder3.PlayerTimes = 30;
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_SkillLottery_CS, builder3.Build());
                }
                else
                {
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();//暂时注释
                }

                break;
        }
    }


    #endregion

    #region 特殊（角色抽卡）

    private void UpdateSpecialInfo()
    {
        this.SummonSystem.specialInfo.specialList.numItems = 1;
        this.SummonSystem.specialInfo.bannerBg.url = UIResource.GetSummonUrl("n13");
    }

    private void SpecialItemRender(int index, GObject item)
    {
        if (index == 0)
        {
            var summonHeroMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonHero);
            ((UI_SpecialItem)item).lockCtrl.selectedIndex = summonHeroMap.Item1 ? 0 : 1;
            ((UI_SpecialItem)item).lockDesc.text = summonHeroMap.Item2;
            bool hasHeroToken = ItemInfoManager.Instance.GetItemCount(int.Parse(ShopInfoManager.Instance.GetHeroLotteryCfg().Param1)) >= int.Parse(_common3006.Param1);
            ((UI_SpecialItem)item).redDot.visible = ActivityManager.Instance.SummonSpecialRedDot() || hasHeroToken;
            ((UI_SpecialItem)item).bgLoader.url = UIResource.GetImageUrlWithLang("yxjjbg", "Summon");
        }
        ((UI_SpecialItem)item).goBtn.onClick.Add(this.OnClickSpecialItem);
    }

    private void OnClickSpecialItem(EventContext context)
    {
        UI_SpecialItem item = ((GButton)context.sender).parent as UI_SpecialItem;
        int childIndex = this.SummonSystem.specialInfo.specialList.GetChildIndex(item);
        switch (childIndex)
        {
            case 0:
                var summonHeroMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonHero);
                if (!summonHeroMap.Item1)
                {
                    UIManager.Instance.Toast(summonHeroMap.Item2);
                }
                else
                {
                    UIManager.Instance.ShowUIPanel("SummonHero", _jumpTypeEnum);
                }

                break;
        }
    }

    private void UpdateSpecialRedDot()
    {
        UI_TabBtn tabBtn = this.SummonSystem.tabList.GetChildAt(3) as UI_TabBtn;
        bool hasHeroToken = ItemInfoManager.Instance.GetItemCount(int.Parse(ShopInfoManager.Instance.GetHeroLotteryCfg().Param1)) >= int.Parse(_common3006.Param1);
        tabBtn.redCtrl.selectedIndex = ActivityManager.Instance.SummonSpecialRedDot() || hasHeroToken ? 1 : 0;
        this.SummonSystem.specialInfo.specialList.numItems = 1;
    }

    #endregion

    #region 礼包

    private void UpdateGiftInfo()
    {
        if (IsShow() && IsOnStage())
        {
            string petPath = ConfigUtils.GetPetModelPathByID(10150);
            Utils.SetSpineModelOnFGUI(this.SummonSystem.giftInfo.spine, petPath, 100);
            this.SummonSystem.giftInfo.bannerBg.url = UIResource.GetSummonUrl("nn1069");
            _currentGiftUnits.Clear();
            _currentGiftUnits = _giftUnits.FindAll((unit =>
                FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)unit.System).Item1));
            this.SummonSystem.giftInfo.summonListShade.giftList.numItems = _currentGiftUnits.Count;
        }
    }

    private void GiftItemRender(int index, GObject item)
    {
        ConfigGiftUnit giftUnit = _currentGiftUnits[index];
        ((UI_GiftListItem)item).bgCtrl.selectedIndex = giftUnit.BgUrl - 1;
        ((UI_GiftListItem)item).limitCtrl.selectedIndex = giftUnit.Buy == 1 ? 0 : 1;
        // ((UI_GiftListItem) item).nameLb.text = giftUnit.Name;
        ((UI_GiftListItem)item).nameLb.text = ConfigUtils.GetTextById(giftUnit.Name);
        ((UI_GiftListItem)item).rebate.SetVar("value", ((giftUnit.Rebate).ToInt32() / 100).ToString()).FlushVars();

        string[] rewardArr = giftUnit.ItemId.Split("|");
        List<ItemData> rewardItemList = new List<ItemData>();
        for (int i = 0; i < rewardArr.Length; i++)
        {
            string[] oneRewardArr = rewardArr[i].Split(",");
            ItemData itemData = new ItemData();
            itemData.id = int.Parse(oneRewardArr[0]);
            itemData.count = double.Parse(oneRewardArr[1]);
            rewardItemList.Add(itemData);
        }
        ((UI_GiftListItem)item).rewardList.itemRenderer = this.OnRewardItemListRender;
        ((UI_GiftListItem)item).rewardList.data = rewardItemList;
        ((UI_GiftListItem)item).rewardList.numItems = rewardItemList.Count;

        //1=英雄相关2=物品类3=宠物类4=技能类
        if (giftUnit.BgUrl == 1)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(rewardItemList[0].id);
            string heroModel = ConfigUtils.GetHeroModelPathByID(itemTypeUnit.Param);
            Utils.SetSpineModelOnFGUI(((UI_GiftListItem)item).spine, heroModel, 80f);
        }
        else if (giftUnit.BgUrl == 3)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(rewardItemList[0].id);
            string petPath = ConfigUtils.GetPetModelPathByID(itemTypeUnit.Param);
            Utils.SetSpineModelOnFGUI(((UI_GiftListItem)item).spine, petPath, 80f);
        }
        // else if (giftUnit.BgUrl == 4)
        // {
        //     ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(rewardItemList[0].id);
        //     ConfigSkillEffectUnit skillEffect = ConfigUtils.GetSkillEffectById(skillUnit.AttackEffect);
        //     Utils.ShowUIPrefab(((UI_GiftListItem) item).spine, skillEffect.Path, 80f, "Effect/");
        // }
        else if (giftUnit.BgUrl == 2 || giftUnit.BgUrl == 4)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(rewardItemList[0].id);
            ((UI_GiftListItem)item).itemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
            ((UI_GiftListItem)item).t0.Play(-1, 0, null);
        }

        if (giftUnit.BuyNumber == 0)
        {
            ((UI_GiftListItem)item).limitCtrl.selectedIndex = 0;
        }
        else
        {
            LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum(giftUnit.Id);
            ((UI_GiftListItem)item).limitCtrl.selectedIndex = 1;
            ((UI_GiftListItem)item).limitLb.SetVar("cur", limitPackVo.BuyCounter.ToString()).SetVar("total", giftUnit.BuyNumber.ToString()).FlushVars();
            int totalSecond = (int)(limitPackVo.EndTime - ServerTimeManager.Instance.CurServerTime);
            ((UI_GiftListItem)item).timeLb.text = StringUtils.GetTimeString(totalSecond);
        }

        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(giftUnit.PayList);
        if (payListUnit != null)
        {
            ((UI_EmptyRMB)((UI_GiftListItem)item).buyBtn).moneyType.selectedIndex = payListUnit.MoneyType - 1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            ((UI_EmptyRMB)((UI_GiftListItem)item).buyBtn).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
            ((UI_EmptyRMB)((UI_GiftListItem)item).buyBtn).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        }

        ((UI_GiftListItem)item).buyBtn.data = giftUnit;
        ((UI_GiftListItem)item).buyBtn.onClick.Add(this.onClickBuyGiftItem);
    }
    private void OnRewardItemListRender(int index, GObject item)
    {
        List<ItemData> rewardItemList = item.parent.data as List<ItemData>;
        ((UI_GiftItemRewardItem)item).count.SetVar("value", StringUtils.FormatCurrency(rewardItemList[index].count)).FlushVars();
        ((UI_ItemCom)((UI_GiftItemRewardItem)item).item).SetItemDataWithGuid(rewardItemList[index], false);
    }

    private void onClickBuyGiftItem(EventContext context)
    {
        ConfigGiftUnit dailyUnit = (context.sender as GButton).data as ConfigGiftUnit;
        if (dailyUnit != null)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(dailyUnit.PayList);
            if (payListUnit != null)
            {
                UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
            }
        }
    }

    #endregion

    #region 钻石商店

    private void UpdateDiamondInfo()
    {
        string petPath = ConfigUtils.GetPetModelPathByID(10150);
        Utils.SetSpineModelOnFGUI(this.SummonSystem.diamondInfo.spine, petPath, 100);
        this.SummonSystem.diamondInfo.bannerBg.url = UIResource.GetSummonUrl("nn1069");
        this.SummonSystem.diamondInfo.diamondList.numItems = _payList.Count;
    }

    private void ChargeItemListRender(int index, GObject item)
    {
        ((UI_DiamondItem)item).itemCtrl.selectedIndex = index;
        ((UI_DiamondItem)item).curLb.text = _payList[index].AddDiamonds.ToString();
        bool isFirstCharge = !RoleManager.Instance.GetHasRechargeByPayListId(_payList[index].Id);
        ((UI_DiamondItem)item).doubleCtrl.selectedIndex = (_payList[index].FirstAddDiamonds > 0 && isFirstCharge) ? 0 : 1;


        // ((UI_EmptyRMB) ((UI_DiamondItem)item).chargeBtn).moneyType.selectedIndex = _payList[index].MoneyType-1;
        // double price = double.Parse((_payList[index].RechargeAmount / 100f).ToString("f2"));
        // ((UI_EmptyRMB) ((UI_DiamondItem)item).chargeBtn).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
        // ((UI_EmptyRMB) ((UI_DiamondItem)item).chargeBtn).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        // ((UI_DiamondItem) item).chargeBtn.data = _payList[index];
        // ((UI_DiamondItem) item).chargeBtn.onClick.Set(this.ChargeVipItemClick);
        // ((UI_DiamondItem) item).img.url = UIResource.GetImageUrlWithLang("scsb", "Summon");

        ((UI_ChargeBtn)((UI_DiamondItem)item).chargeBtn2).moneyType.selectedIndex = _payList[index].MoneyType - 1;
        double price = double.Parse((_payList[index].RechargeAmount / 100f).ToString("f2"));
        ((UI_ChargeBtn)((UI_DiamondItem)item).chargeBtn2).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
        ((UI_ChargeBtn)((UI_DiamondItem)item).chargeBtn2).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        ((UI_DiamondItem)item).chargeBtn2.data = _payList[index];
        ((UI_DiamondItem)item).chargeBtn2.onClick.Set(this.ChargeVipItemClick);
        // ((UI_DiamondItem) item).img.url = UIResource.GetImageUrlWithLang("scsb", "Summon");
    }

    private void ChargeVipItemClick(EventContext context)
    {
        ConfigServerUnit serverUnit = ConfigDataGroup.GetInstance<ConfigServer>().Get(GameManager.Instance.CurServerUnit.Id);
        if (serverUnit != null && serverUnit.IsRecharge == 0)
        {
            return;
        }
        ConfigPayListUnit payListUnit = ((GButton)context.sender).data as ConfigPayListUnit;
        if (payListUnit != null)
        {
            //发送充值成功给服务器
            UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
        }

    }

    #endregion

    /// <summary>
    /// 检测宠物上限是否充足
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public bool ChkPetLimit(int num, bool showTips = true)
    {
        var petList = PetInfoManager.Instance.GetAllHavePetList();
        if (petList.Count + num > int.Parse(ItemInfoManager.Instance.common300008.Param1))
        {
            if (showTips)
            {
                UIManager.Instance.ToastByKey(10202);
            }

            return true;
        }
        return false;
    }
}
