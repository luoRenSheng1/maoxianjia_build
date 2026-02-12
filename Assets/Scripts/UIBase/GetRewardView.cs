using System.Collections.Generic;
using Common;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Shop;

public class GetRewardView : UIViewBase
{
    private UI_GetReward shopGetRewardUI => this.main as UI_GetReward;
    
    private List<ItemData> _itemList = new List<ItemData>();
    private List<ItemData> _boxShowitemList = new List<ItemData>();
    private int stringId = -1;
    private Queue<int> _petIdQueue = new Queue<int>();
    private bool _isShowPetGet = false;
    float _delay = 0f;
    private float _playDelayTime = 0;
    private int _quality;
    private bool _isStopSend;
    private Queue<int> _heroIdQueue = new Queue<int>();
    private bool _isShowHeroGet = false;
    public GetRewardView()
    {
        this.name = "GetReward";
        this.package = "Common";
        this.component = "GetReward";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.type = UIType.Tip;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.shopGetRewardUI.sortingOrder = 998;
        this.shopGetRewardUI.closeBtn.onClick.Add(this.Hide);
        this.shopGetRewardUI.rewardList.itemList.itemRenderer = ShopItemRender;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        stringId = -1;
        _itemList = values[0] as List<ItemData>;
        if (values.Length >= 2)
        {
            _boxShowitemList = values[1] as List<ItemData>;

            // if (values[1] is List<ItemData>)
            // {
            //     _boxShowitemList = values[1] as List<ItemData>;
            // }
            //
            // if (values[1] is int)
            // {
            //     stringId = (int)values[1];
            // }
            
        }
        if (values.Length >= 3)
        {
            if ((int)values[2] == 0)
            {
                shopGetRewardUI.title.visible = true;
                shopGetRewardUI.titaction.visible = false;
            }
            else
            {
                shopGetRewardUI.title.visible = false;
                shopGetRewardUI.titaction.visible = true;
            }
        }
        if (values.Length >= 4)
        {
            stringId = (int)values[3];
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        Utils.PlaySpineAnim(this.shopGetRewardUI.gxhdSpine, "Chuxian", false,
            () => { Utils.PlaySpineAnim(this.shopGetRewardUI.gxhdSpine, "Loop", true); });

        this.shopGetRewardUI.rewardList.itemList.touchable = false;
        this.shopGetRewardUI.rewardList.itemList.numItems = _itemList.Count;
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);
        // this.shopGetRewardUI.img.url = UIResource.GetImageUrlWithLang("g2", "Common");
    }

    protected override void OnShow()
    {
        base.OnShow();
        _playDelayTime = 0;
        _heroIdQueue.Clear();

        if (stringId != -1)
        {
            this.shopGetRewardUI.name.text = ConfigUtils.GetStringByKey(10205);
        }
        else
        {
            this.shopGetRewardUI.name.text = ConfigUtils.GetStringByKey(10204);
        }
        
        this.shopGetRewardUI.rewardList.itemList.numItems = _itemList.Count;
    }

    private void ShopItemRender(int index, GObject item)
    {
        ItemData itemData = _itemList[index];
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
        _delay = index * 0.03f + _playDelayTime;
        DelayItem delayItem = new DelayItem();
        delayItem.Index = index;
        delayItem.ItemCom = (UI_ItemCom) item;
        delayItem.ItemTypeUnit = itemTypeUnit;
        delayItem.ItemGuid = itemData.ItemGuid;
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 1, _delay, DelayPlayItem, delayItem);
    }

    private void DelayPlayItem(object param)
    {
        DelayItem delayItem = param is DelayItem ? (DelayItem) param : default;
        delayItem.ItemCom.visible = true;
        delayItem.ItemCom.icon = UIResource.GetItemUrl(delayItem.ItemTypeUnit.Icon);

        if (delayItem.Index >= _itemList.Count - 1)
        {
            _isStopSend = false;
            this.shopGetRewardUI.rewardList.itemList.touchable = true;
        }
        this.shopGetRewardUI.rewardList.itemList.ScrollToView(delayItem.Index);
        
        if (delayItem.ItemTypeUnit.Type == 3)
        {
            (delayItem.ItemCom).PlayGetItemSpineEff(delayItem.ItemTypeUnit.Quality);
            _heroIdQueue.Enqueue(delayItem.ItemTypeUnit.Param);
            GameManager.Instance.TimerManager.SetTimer(ConfigUtils.GetHeroOutTime(), PlayHeroGetEffect);
        }
        else if (delayItem.ItemGuid > 0 && delayItem.ItemTypeUnit.Type == 14)
        {
            // 传承装备
            EquipData equipData = EquipManager.Instance.GetNoWearLoreEquipByGuid(delayItem.ItemGuid);
            if (equipData != null)
            {
                int quality = equipData.quality;
                (delayItem.ItemCom).PlayGetItemSpineEff(quality);
            }
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
            UIManager.Instance.ShowUIPanel("GetReward", _boxShowitemList);
        }
        _boxShowitemList.Clear();
        this.shopGetRewardUI.rewardList.itemList.touchable = true;

    }
}
