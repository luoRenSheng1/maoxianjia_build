
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Summon;
using System.Collections.Generic;
using System.Diagnostics;
using EventDispatcher = EngineBase.EventDispatcher;

public class DelayItem
{
    public UI_ItemCom ItemCom;
    public int Index;
    public ConfigItemTypeUnit ItemTypeUnit;
    public ulong ItemGuid;
}
public class SummonGetRewardView : UIViewBase
{
    private UI_SummonGetReward GetRewardUI => this.main as UI_SummonGetReward;
    
    private List<ItemData> _itemList = new List<ItemData>();
    private int _summonType = 0;
    private ConfigCommonUnit _commonUnit;
    float _delay = 0f;
    private float _playDelayTime = 0;
    private int _quality;
    private bool _isStopSend;

    private bool _isGuiding;

    private List<UnityEngine.Vector2> BtnPosList = new List<UnityEngine.Vector2>();

    public SummonGetRewardView()
    {
        this.name = "SummonGetReward";
        this.package = "Summon";
        this.component = "SummonGetReward";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        SummonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        this.GetRewardUI.closeBtn.onClick.Add(this.Hide);
        this.GetRewardUI.close.onClick.Add(this.Hide);
        if(!GuideManager.Instance.GuideIsComplete(GuideID.Click_SkillIntensify) || !GuideManager.Instance.GuideIsComplete(GuideID.Click_EquipPetItem))
        {
            this.GetRewardUI.close.onClick.Add(() =>
            {
                if(!GuideManager.Instance.GuideIsComplete(GuideID.Click_SkillIntensify) || !GuideManager.Instance.GuideIsComplete(GuideID.Click_EquipPetItem))
                {
                    var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                    lobbyView?.UpdateGuideInfo();
                }
            });
        }
        _commonUnit = ConfigDataGroup.GetInstance<ConfigCommon>().Get(5);//宠物抽奖券信息
        this.GetRewardUI.summon10.tickNumLb.SetVar("value", _commonUnit.Param2).FlushVars();
        this.GetRewardUI.summon30.tickNumLb.SetVar("value", _commonUnit.Param4).FlushVars();
        this.GetRewardUI.summon300.tickNumLb.SetVar("value", _commonUnit.Param3).FlushVars();
        this.GetRewardUI.rewardList.itemList.itemRenderer = ShopItemRender;

        //BtnPosList.Add(this.GetRewardUI.summon30.xy);
        //BtnPosList.Add(this.GetRewardUI.summon300.xy);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _itemList = values[0] as List<ItemData>;
        _summonType = (int) values[1];
        UpdateUI();
    }
    

    private void UpdateUI()
    {
        // this.GetRewardUI.img.url = UIResource.GetImageUrlWithLang("g2", "Common");
        // this.GetRewardUI.t1.Play(1, 0, null);
        // Utils.PlaySpineAnim(this.GetRewardUI.gxhdSpine, "Chuxian", false, () =>
        // {
        //     //Utils.PlaySpineAnim(this.GetRewardUI.gxhdSpine, "Loop", true);
        // });
        
        this.GetRewardUI.summon10.onClick.Clear();
        this.GetRewardUI.summon30.onClick.Clear();
        this.GetRewardUI.summon300.onClick.Clear();
        RoleData roleData = DataManager.Instance.GetRoleData();
        if (_summonType == 1)
        {
            this.GetRewardUI.summon10.data = 1;
            this.GetRewardUI.summon10.onClick.Add(this.OnClickPetSummonBtn);
            this.GetRewardUI.summon30.data = 2;
            this.GetRewardUI.summon30.onClick.Add(this.OnClickPetSummonBtn);
            this.GetRewardUI.summon300.data = 3;
            this.GetRewardUI.summon300.visible = false;


            //this.GetRewardUI.summon300.onClick.Add(this.OnClickPetSummonBtn);
            // this.GetRewardUI.summon10.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 10;
            // this.GetRewardUI.summon30.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 30;
            // this.GetRewardUI.summon300.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 300;
            // 先将按钮全部置灰，待抽奖完毕后，根据其数量进行判断是否恢复按钮状态
            this.GetRewardUI.summon10.grayed = true;
            this.GetRewardUI.summon30.grayed = true;
            this.GetRewardUI.summon300.grayed = true;
            this.GetRewardUI.summon10.tickNumLb.text = (ShopInfoManager.Instance.GetPetLotteryOne() * 10).ToString();
            this.GetRewardUI.summon30.tickNumLb.text = (ShopInfoManager.Instance.GetPetLotteryOne() * 30).ToString();
            this.GetRewardUI.summon300.tickNumLb.text = (ShopInfoManager.Instance.GetPetLotteryOne() * 300).ToString();
        }
        else
        {
            this.GetRewardUI.summon10.data = 1;
            this.GetRewardUI.summon10.onClick.Add(this.OnClickSkillSummonBtn);
            this.GetRewardUI.summon30.data = 2;
            this.GetRewardUI.summon30.onClick.Add(this.OnClickSkillSummonBtn);
            this.GetRewardUI.summon300.data = 3;
            this.GetRewardUI.summon300.onClick.Add(this.OnClickSkillSummonBtn);
            this.GetRewardUI.summon300.visible = true;
            

            // this.GetRewardUI.summon10.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 10;
            // this.GetRewardUI.summon30.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 30;
            // this.GetRewardUI.summon300.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 300;
            // 先将按钮全部置灰，待抽奖完毕后，根据其数量进行判断是否恢复按钮状态
            this.GetRewardUI.summon10.grayed = true;
            this.GetRewardUI.summon30.grayed = true;
            this.GetRewardUI.summon300.grayed = true;
            this.GetRewardUI.summon10.tickNumLb.text = (ShopInfoManager.Instance.GetSkillLotteryOne() * 10).ToString();
            this.GetRewardUI.summon30.tickNumLb.text = (ShopInfoManager.Instance.GetSkillLotteryOne() * 30).ToString();
            this.GetRewardUI.summon300.tickNumLb.text = (ShopInfoManager.Instance.GetSkillLotteryOne() * 300).ToString();
        }

        if (_summonType == 1)
        {
            _quality = ShopInfoManager.Instance.GetQualityGet(DataManager.Instance.GetRoleData().petLotteryLv, _summonType);
        }
        else if (_summonType == 2)
        {
            _quality = ShopInfoManager.Instance.GetQualityGet(DataManager.Instance.GetRoleData().SkillLotteryLv, _summonType);
        }

        _isStopSend = true;
        _delay = 0;
        _playDelayTime = 0;
        this.GetRewardUI.rewardList.itemList.touchable = false;
        this.GetRewardUI.rewardList.itemList.visible = true;
        this.GetRewardUI.rewardList.itemList.numItems = _itemList.Count;
        UnityEngine.Debug.Log($"数量{this.GetRewardUI.rewardList.itemList.numItems}");
        this.GetRewardUI.rewardList.itemList.ScrollToView(0);
    }

    protected override void OnShow()
    {
        base.OnShow();

        //引导-技能抽10次关闭
        if(GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_SummonSkill,
            bid = GuideID.Click_SummonSkill,
            gid = GuideID.Click_SummonSkill_Close,
            tui = this.GetRewardUI.close,
            isForce = true,
            isSend = true,
            isOver = true,
            isLucency = false,
            //skewing = new Vector2(0f, this.GetRewardUI.close.height*0.5f)
        }) ||
        //引导-宠物抽10次关闭
        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_SummonPet,
            bid = GuideID.Click_SummonPet,
            gid = GuideID.Click_SummonPet_Close,
            tui = this.GetRewardUI.close,
            isForce = true,
            isSend = true,
            isOver = true,
            isLucency = false,
            //skewing = new Vector2(0f, this.GetRewardUI.close.height * 0.5f)
        })) { }
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);
    }

    protected override void OnHide()
    {
        base.OnHide();
        _isStopSend = false;
        this.GetRewardUI.rewardList.itemList.numItems = 0;
        UnityEngine.Debug.Log($"数量{this.GetRewardUI.rewardList.itemList.numItems}");
        GameManager.Instance.TimerManager.ClearTimerBySourceID(EN_TIMER_SOURCE.UI, 1);
        GameManager.Instance.TimerManager.ClearTimerBySourceID(EN_TIMER_SOURCE.UI, 2);
        if (_isGuiding)
        {
            GuideManager.Instance.HideGuide();
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_UPDATE);
        }
        _isGuiding = false;
    }

    private void ShopItemRender(int index, GObject item)
    {
        UnityEngine.Debug.Log("进来刷新了");
        ItemData itemData = _itemList[index];
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
        
        ((UI_ItemCom) item).visible = false;
        ((UI_ItemCom) item).itemSpineEff.visible = false;

        _delay = index * 0.02f + _playDelayTime;
        DelayItem delayItem = new DelayItem();
        delayItem.Index = index;
        delayItem.ItemCom = (UI_ItemCom) item;
        delayItem.ItemTypeUnit = itemTypeUnit;
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI,1,_delay, DelayPlayItem, delayItem);
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI,2,_delay-0.01f, DelayPlayItem2, delayItem);
       
        if (itemTypeUnit.Quality >= _quality)
        {
            _playDelayTime += 0.5f;
        }
    }

    private void DelayPlayItem2(object param)
    {
        DelayItem delayItem = param is DelayItem ? (DelayItem) param : default;
        delayItem.ItemCom.icon = UIResource.GetItemUrl(delayItem.ItemTypeUnit.Icon);
    }
    
    private void DelayPlayItem(object param)
    {
        DelayItem delayItem = param is DelayItem ? (DelayItem) param : default;
        delayItem.ItemCom.visible = true;
        // delayItem.ItemCom.icon = UIResource.GetItemUrl(delayItem.ItemTypeUnit.Icon);
        if (delayItem.ItemTypeUnit.Quality >= _quality)
        {
            this.GetRewardUI.shake.Play();
            // 1=物品，2=装备，3角色，4宠物，5技能，6符石，7=角色碎片
            delayItem.ItemCom.PlayGetItemSpineEff(delayItem.ItemTypeUnit.Quality);
        }

        if (delayItem.Index >= _itemList.Count - 1)
        {
            _isStopSend = false;
            this.GetRewardUI.rewardList.itemList.touchable = true;
            // 根据钻石的数量，判断是否恢复按钮状态
            RoleData roleData = DataManager.Instance.GetRoleData();
            if (_summonType == 1)
            {
                this.GetRewardUI.summon10.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 10;
                this.GetRewardUI.summon30.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 30;
                this.GetRewardUI.summon300.grayed = roleData.dia < ShopInfoManager.Instance.GetPetLotteryOne() * 300;
            }
            else
            {
                this.GetRewardUI.summon10.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 10;
                this.GetRewardUI.summon30.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 30;
                this.GetRewardUI.summon300.grayed = roleData.dia < ShopInfoManager.Instance.GetSkillLotteryOne() * 300;
            }
        }
        
        this.GetRewardUI.rewardList.itemList.ScrollToView(delayItem.Index);
    }

    private void OnClickPetSummonBtn(EventContext context)
    {
        if(_isStopSend) return;
        int summonType = (int)((UI_SummonBtn) context.sender).data;
        switch (summonType) //type=1 10抽 type=2 30抽 type=3 300抽
        {
            case 1:
                if (ShopInfoManager.Instance.GetPetLotteryOne() * 10 <= DataManager.Instance.GetRoleData().dia)
                {
                    var view = UIManager.Instance.FindByName("SummonSystem") as SummonSystemView;
                    if (view != null && view.ChkPetLimit(10))
                    {
                        return;
                    }
                    var builder2 = PetLottery_CS.CreateBuilder();
                    builder2.IsByAd = false;
                    builder2.PlayerTimes = 10;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetLottery_CS, builder2.Build());
                }
                else
                {
                    SetVisible(false);
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();// 关闭商业化
                }

                break;
            case 2:
                if (ShopInfoManager.Instance.GetPetLotteryOne() * 30 <= DataManager.Instance.GetRoleData().dia)
                {
                    var view = UIManager.Instance.FindByName("SummonSystem") as SummonSystemView;
                    if (view != null && view.ChkPetLimit(30))
                    {
                        return;
                    }
                    var builder2 = PetLottery_CS.CreateBuilder();
                    builder2.IsByAd = false;
                    builder2.PlayerTimes = 30;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetLottery_CS, builder2.Build());
                }
                else
                {
                    SetVisible(false);
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();// 关闭商业化
                }

                break;
            case 3:
                if (ShopInfoManager.Instance.GetPetLotteryOne() * 300 <= DataManager.Instance.GetRoleData().dia)
                {
                    var view = UIManager.Instance.FindByName("SummonSystem") as SummonSystemView;
                    if (view != null && view.ChkPetLimit(300))
                    {
                        return;
                    }
                    var builder3 = PetLottery_CS.CreateBuilder();
                    builder3.IsByAd = false;
                    builder3.PlayerTimes = 300;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetLottery_CS, builder3.Build());
                }
                else
                {
                    SetVisible(false);
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();// 关闭商业化
                }

                break;
        }
    }

    private void OnClickSkillSummonBtn(EventContext context)
    {
        if(_isStopSend) return;
        int summonType = (int)((GButton) context.sender).data;
        switch (summonType)//type=1 10抽 type=2 30抽 type=3 300抽
        {
            case 1:
                if (ShopInfoManager.Instance.GetSkillLotteryOne() * 10 <= DataManager.Instance.GetRoleData().dia)
                {
                    var builder2 = SkillLottery_CS.CreateBuilder();
                    builder2.IsByAd = false;
                    builder2.PlayerTimes = 10;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_SkillLottery_CS, builder2.Build());
                }
                else
                {
                    SetVisible(false);
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();// 关闭商业化
                }

                break;
            case 2:
                if (ShopInfoManager.Instance.GetSkillLotteryOne() * 30 <= DataManager.Instance.GetRoleData().dia)
                {
                    var builder2 = SkillLottery_CS.CreateBuilder();
                    builder2.IsByAd = false;
                    builder2.PlayerTimes = 30;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_SkillLottery_CS, builder2.Build());
                }
                else
                {
                    SetVisible(false);
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();// 关闭商业化
                }

                break;
            case 3:
                if (ShopInfoManager.Instance.GetSkillLotteryOne() * 300 <= DataManager.Instance.GetRoleData().dia)
                {
                    var builder3 = SkillLottery_CS.CreateBuilder();
                    builder3.IsByAd = false;
                    builder3.PlayerTimes = 300;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_SkillLottery_CS, builder3.Build());
                }     
                else
                {
                    SetVisible(false);
                    UIManager.Instance.ToastByKey(5008);
                    // Utils.DiamondNotEnough();// 关闭商业化
                }

                break;
        }
    }
}
