
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Shop;
using Summon;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class SummonHeroRewardView : UIViewBase
{
    private UI_SummonHeroReward GetRewardUI => this.main as UI_SummonHeroReward;
    
    private List<ItemData> _itemList = new List<ItemData>();
    private ConfigCommonUnit _commonUnit;
    float _delay = 0f;
    private Queue<int> _heroIdQueue = new Queue<int>();
    private bool _isShowHeroGet = false;

    public SummonHeroRewardView()
    {
        this.name = "SummonHeroReward";
        this.package = "Summon";
        this.component = "SummonHeroReward";
        this.removePackage = true;
        this.safeAreaInset = true;
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
        _commonUnit = ShopInfoManager.Instance.GetHeroLotteryCfg();//角色抽奖券信息
        this.GetRewardUI.summon1.tickNumLb.SetVar("value", _commonUnit.Param2).FlushVars();
        this.GetRewardUI.summon10.tickNumLb.SetVar("value", (int.Parse(_commonUnit.Param2)*10).ToString()).FlushVars();
        this.GetRewardUI.summon10.onClick.Add(this.OnClick10TicketBtn);
        this.GetRewardUI.summon1.onClick.Add(this.OnClick1TicketBtn);
        this.GetRewardUI.rewardList.itemList.itemRenderer = ShopItemRender;
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.OnItemUpdate);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.OnItemUpdate);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _itemList = values[0] as List<ItemData>;
        UpdateUI();
    }
    
    private void OnItemUpdate()
    {
        ConfigCommonUnit common = ShopInfoManager.Instance.GetHeroLotteryCfg();
        int count = ItemInfoManager.Instance.GetItemCount(int.Parse(common.Param1));
        
        this.GetRewardUI.summon10.grayed = count < (int.Parse(common.Param2)*10);
        this.GetRewardUI.summon1.grayed = count < (int.Parse(common.Param2));
    }

    private void UpdateUI()
    {
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);
        
        // this.GetRewardUI.img.url = UIResource.GetImageUrlWithLang("g2", "Common");
        // this.GetRewardUI.t1.Play(1,0, null);
        // Utils.PlaySpineAnim(this.GetRewardUI.gxhdSpine, "Chuxian", false, () =>
        // {
        //     //Utils.PlaySpineAnim(this.GetRewardUI.gxhdSpine, "Loop", true);
        // });
        
        this.GetRewardUI.rewardList.itemList.numItems = _itemList.Count;
        this.GetRewardUI.rewardList.itemList.ScrollToView(0);
        _heroIdQueue.Clear();
        _isShowHeroGet = false;
        OnItemUpdate();
    }

    protected override void OnHide()
    {
        base.OnHide();
        GameManager.Instance.TimerManager.ClearTimerBySourceID(EN_TIMER_SOURCE.UI, 1);
    }

    private void ShopItemRender(int index, GObject item)
    {
        ItemData itemData = _itemList[index];
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
        _delay = index * 0.05f;
        ((UI_ItemCom) item).visible = false;
        ((UI_ItemCom) item).itemSpineEff.visible = false;
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 1, _delay, () =>
        {
            ((UI_ItemCom) item).visible = true;
            if (itemTypeUnit.Type == 3)
            {
                ((UI_ItemCom) item).PlayGetItemSpineEff(itemTypeUnit.Quality);
                _heroIdQueue.Enqueue(itemTypeUnit.Param);
                GameManager.Instance.TimerManager.SetTimer(ConfigUtils.GetHeroOutTime(), PlayHeroGetEffect);
            }
        });
    }

    private void PlayHeroGetEffect()
    {
        GetPetRewardView petRewardView = UIManager.Instance.FindByName("GetPetReward") as GetPetRewardView;
        if (_heroIdQueue.Count > 0 && ((petRewardView != null && !petRewardView.IsShow())|| petRewardView == null) && !_isShowHeroGet)
        {
            int petId = _heroIdQueue.Dequeue();
            _isShowHeroGet = true;
            GetPetRewardView.GetPetParam getPetParam = new GetPetRewardView.GetPetParam();
            getPetParam.Id = petId;
            getPetParam.IsRole = true;
            getPetParam.CloseCallback = () =>
            {
                _isShowHeroGet = false;
                this.PlayHeroGetEffect();
            };
            UIManager.Instance.ShowUIPanel("GetPetReward", getPetParam);
        }
    }

    private void OnClick1TicketBtn()
    {
        //10抽
        ConfigCommonUnit common = ShopInfoManager.Instance.GetHeroLotteryCfg();
        int count = ItemInfoManager.Instance.GetItemCount(int.Parse(common.Param1));
        if (count >= (int.Parse(common.Param2)))
        {
            var builder = HeroLottery_CS.CreateBuilder();
            builder.PlayerTimes = 1;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroLottery_CS, builder.Build());
        }
        else
        {
            Utils.YxlNotEnough();
            SetVisible(false);
        }
    }
    
    private void OnClick10TicketBtn()
    {
        //10抽
        ConfigCommonUnit common = ShopInfoManager.Instance.GetHeroLotteryCfg();
        int count = ItemInfoManager.Instance.GetItemCount(int.Parse(common.Param1));
        if (count >= (int.Parse(common.Param2)*10))
        {
            var builder = HeroLottery_CS.CreateBuilder();
            builder.PlayerTimes = 10;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroLottery_CS, builder.Build());
        }      
        else
        {
            Utils.YxlNotEnough();
            SetVisible(false);
        }
    }
}
