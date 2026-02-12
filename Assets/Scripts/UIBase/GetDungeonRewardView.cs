using System.Collections.Generic;
using Common;
using CommonEx;
using Config;
using DungeonMap;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Shop;
using EventDispatcher = EngineBase.EventDispatcher;

public class GetDungeonRewardView : UIViewBase
{
    private UI_GetDungeonReward GetDungeonReward => this.main as UI_GetDungeonReward;
    
    private List<ItemData> _itemList = new List<ItemData>();
    private Queue<int> _petIdQueue = new Queue<int>();
    private bool _isShowPetGet = false;
    float _delay = 0f;
    private ConfigDungeonChapterUnit _chapterUnit;
    private int _curDungeonStage;
    
    public GetDungeonRewardView()
    {
        this.name = "GetDungeonReward";
        this.package = "DungeonMap";
        this.component = "GetDungeonReward";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        DungeonMapBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.GetDungeonReward.itemList.itemRenderer = ItemListRender;
        this.GetDungeonReward.sweepBtn.onClick.Add(this.OnClickSweepBtn);
        this.GetDungeonReward.nextStageBtn.onClick.Add(this.OnClickGoToBtn);
        this.GetDungeonReward.adBtn.onClick.Add(this.OnClickAdBtn);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateDungeonItem);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateDungeonItem);
        
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateDungeonItem);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateDungeonItem);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _itemList = values[0] as List<ItemData>;
        int chapterType = (int) values[1];
        _chapterUnit = ConfigUtils.GetDungeonChapterById((DungeonType)chapterType);
        _curDungeonStage = (int) values[2];
        
        this.UpdateUI();
    }

    private void UpdateDungeonItem()
    {
        if (IsShow() && IsOnStage())
        {
            int total = ConfigUtils.GetDungeonTotalCount(_chapterUnit.Id);
            this.GetDungeonReward.countCtrl.selectedIndex =
                ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) > 0 ? 1 : 0;
            this.GetDungeonReward.itemCntLb
                .SetVar("cur", ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID).ToString())
                .SetVar("total", total.ToString()).FlushVars();
            
            if (ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) <= 0 && AdManager.Instance.GetAdFreeTimes(1000+(int) _chapterUnit.Type) > 0)
            {
                int totalAd = ConfigUtils.GetDungeonAdTotalCount(_chapterUnit.Id);
                ((UI_EmptyAdBtn2) this.GetDungeonReward.adBtn).adCntLb.SetVar("cur",AdManager.Instance.GetAdFreeTimes(1000+(int) _chapterUnit.Type).ToString()).SetVar("total", totalAd.ToString()).FlushVars();
                this.GetDungeonReward.adCtrl.selectedIndex = 1;
            }
            else
            {
                this.GetDungeonReward.adCtrl.selectedIndex = 0;
            }
        }
    }  

    private void UpdateUI()
    {
        this.GetDungeonReward.itemList.numItems = _itemList.Count;
        int total = ConfigUtils.GetDungeonTotalCount(_chapterUnit.Id);
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_chapterUnit.ItemID);
        this.GetDungeonReward.itemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
        this.GetDungeonReward.countCtrl.selectedIndex = ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) > 0 ? 1 : 0;
        this.GetDungeonReward.itemCntLb.SetVar("cur",ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID).ToString()).SetVar("total", total.ToString()).FlushVars();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);

        if (ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) <= 0)
        {
            this.GetDungeonReward.adCtrl.selectedIndex = 1;
        }
        else
        {
            this.GetDungeonReward.adCtrl.selectedIndex = 0;
        }
        
        if (ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) <= 0 && AdManager.Instance.GetAdFreeTimes(1000+(int) _chapterUnit.Type) > 0)
        {
            int totalAd = ConfigUtils.GetDungeonAdTotalCount(_chapterUnit.Id);
            ((UI_EmptyAdBtn2) this.GetDungeonReward.adBtn).adCntLb.SetVar("cur",AdManager.Instance.GetAdFreeTimes(1000+(int) _chapterUnit.Type).ToString()).SetVar("total", totalAd.ToString()).FlushVars();
            this.GetDungeonReward.adCtrl.selectedIndex = 1;
        }
        else
        {
            this.GetDungeonReward.adCtrl.selectedIndex = 0;
        }
    }
    
    private void OnClickSweepBtn()
    {
        if (ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) <= 0)
        {
            Gift_Bury giftBury = Utils.GetDungeonGiftBuryByType((DungeonType) _chapterUnit.Type);
            ConfigGiftUnit giftUnit = ConfigUtils.GetGiftUnitsById((int) giftBury);
            if (giftBury != 0 && ShopInfoManager.Instance.GetPackNum((int) giftBury).BuyCounter < giftUnit.BuyNumber)
            {
                // UIManager.Instance.ShowUIPanel("BuryGiftPack", giftBury);// 关闭商业化
            }
            else
            {
                UIManager.Instance.ToastByKey(10078);
            }
            return;
        }
        
        var builder = Copy_End_CS.CreateBuilder();
        builder.CopyType = (eCopyType) _chapterUnit.Type;
        builder.StartStageId =(uint) ConfigUtils.GetDungeonStageByNandu(_curDungeonStage,_chapterUnit.Type).Id;
        builder.IsWin = true;
        builder.IsDirectFinish = true;
        Copy_End_CS copyEndCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Copy_End_CS, copyEndCs);
    }

    private void OnClickGoToBtn()
    {
        if (ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) <= 0)
        {
            Gift_Bury giftBury = Utils.GetDungeonGiftBuryByType((DungeonType) _chapterUnit.Type);
            ConfigGiftUnit giftUnit = ConfigUtils.GetGiftUnitsById((int) giftBury);
            if (giftBury != 0 && ShopInfoManager.Instance.GetPackNum((int) giftBury).BuyCounter < giftUnit.BuyNumber)
            {
                // UIManager.Instance.ShowUIPanel("BuryGiftPack", giftBury);// 关闭商业化
            }
            else
            {
                UIManager.Instance.ToastByKey(10078);
            }
            return;
        }

        DungeonMapManager.Instance.GuanKaStage = _curDungeonStage+1;
        var builder = Copy_Begin_CS.CreateBuilder();
        builder.CopyType = (eCopyType) _chapterUnit.Type;
        builder.StartStageId = (uint) ConfigUtils.GetDungeonStageByNandu(_curDungeonStage+1, _chapterUnit.Type).Id;
        Copy_Begin_CS copyBeginCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Copy_Begin_CS, copyBeginCs);
        
        SetVisible(false);
    }

    private void ItemListRender(int index, GObject item)
    {
        ItemData itemData = _itemList[index];
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
        _delay = index * 0.03f;
        ((UI_ItemCom) item).visible = false;
        ((UI_ItemCom) item).itemSpineEff.visible = false;
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 1, _delay, () =>
        {
            ((UI_ItemCom) item).visible = true;
            if (itemTypeUnit.Type == 3)
            {
                ((UI_ItemCom) item).PlayGetItemSpineEff(itemTypeUnit.Quality);
            }
        });
    }

    private void OnClickAdBtn()
    {
        AdManager.Instance.WatchAd(() =>
        {
            var builder = GetCopyFreeTimesByAD_CS.CreateBuilder();
            builder.CopyType = (eCopyType) _chapterUnit.Type;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GetCopyFreeTimesByAD_CS, builder.Build());
        });

    }
}
