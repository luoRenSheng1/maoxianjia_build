using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Sokoban;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 深埋宝藏 领取宝箱的奖励界面
/// </summary>
public class SokobanRewardView : UIViewBase
{
    private UI_SokobanRewardView view => this.main as UI_SokobanRewardView;

    private List<ItemData> _itemList = new List<ItemData>();
    private List<ItemData> _boxShowitemList = new List<ItemData>();
    private Queue<int> _petIdQueue = new Queue<int>();
    private bool _isShowPetGet = false;
    float _delay = 0f;
    private float _playDelayTime = 0;
    private int _quality;
    private bool _isStopSend;
    private Queue<int> _heroIdQueue = new Queue<int>();
    private bool _isShowHeroGet = false;

    public SokobanRewardView()
    {
        this.package = "Sokoban";
        this.name = "SokobanRewardView";
        this.component = "SokobanRewardView";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.type = UIType.Tip;

    }
    public override void BindAll()
    {
        base.BindAll();
        SokobanBinder.BindAll();
    }
    
    protected override void OnInit()
    {
        base.OnInit();
        view.sortingOrder = 998;
        view.btn.onClick.Add(this.Hide);
        
    }




    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _itemList = values[0] as List<ItemData>;
        if (values.Length >= 2)
        {
            _boxShowitemList = values[1] as List<ItemData>;
        }
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(view == null || view.rewardList == null || _itemList == null || _itemList.Count <= 0) { return; }
        view.rewardList.touchable = false;
        view.rewardList.itemRenderer = ShopItemRender;
        view.rewardList.numItems = _itemList.Count;
    }
    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);

        _playDelayTime = 0;
        _heroIdQueue.Clear();
        view.rewardList.itemRenderer = ShopItemRender;
        view.rewardList.numItems = _itemList.Count;
    }
    private void ShopItemRender(int index, GObject item)
    {
        ItemData itemData = _itemList[index];
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
        var ritem = (UI_RewardItemComLarge)item;
        var itemCom = ritem.itemCom;
        itemCom.SetItemDataWithGuid(itemData, true);
        _delay = index * 0.03f + _playDelayTime;
        DelayItem delayItem = new DelayItem();
        delayItem.Index = index;
        delayItem.ItemCom = itemCom;
        delayItem.ItemTypeUnit = itemTypeUnit;
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 1, _delay, DelayPlayItem, delayItem);
    }

    private void DelayPlayItem(object param)
    {
        DelayItem delayItem = param is DelayItem ? (DelayItem)param : default;
        delayItem.ItemCom.visible = true;
        delayItem.ItemCom.icon = UIResource.GetItemUrl(delayItem.ItemTypeUnit.Icon);

        if (delayItem.Index >= _itemList.Count - 1)
        {
            _isStopSend = false;
            view.rewardList.touchable = true;
        }
        view.rewardList.ScrollToView(delayItem.Index);

        if (delayItem.ItemTypeUnit.Type == 3)
        {
            (delayItem.ItemCom).PlayGetItemSpineEff(delayItem.ItemTypeUnit.Quality);
            _heroIdQueue.Enqueue(delayItem.ItemTypeUnit.Param);
            GameManager.Instance.TimerManager.SetTimer(ConfigUtils.GetHeroOutTime(), PlayHeroGetEffect);
        }
        else
        {
            (delayItem.ItemCom).PlayGetItemSpineEff(delayItem.ItemTypeUnit.Quality);
        }
    }

    private void PlayHeroGetEffect()
    {
        if (_heroIdQueue.Count > 0)
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


    protected override void OnHide()
    {
        base.OnHide();

        GameManager.Instance.TimerManager.ClearTimerBySourceID(EN_TIMER_SOURCE.UI, 1);
        _heroIdQueue.Clear();
        if (_boxShowitemList.Count > 0)
        {
            UIManager.Instance.ShowUIPanel("SokobanReward", _boxShowitemList);
        }
        _boxShowitemList.Clear();
        view.rewardList.touchable = true;
        //view.rewardList.RemoveChildren();
    }
}