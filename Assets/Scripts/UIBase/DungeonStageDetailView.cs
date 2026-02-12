
using System.Collections.Generic;
using CommonEx;
using Config;
using DungeonMap;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;

public class DungeonStageDetailView : UIViewBase
{
    private UI_DungeonStageDetail StageDetail => this.main as UI_DungeonStageDetail;

    private ConfigDungeonChapterUnit _chapterUnit;
    private int _dungeonStage;
    private int _curDungeonStage;
    private ConfigDungeonStageUnit _stageUnit;
    public DungeonStageDetailView()
    {
        this.name = "DungeonStageDetail";
        this.package = "DungeonMap";
        this.component = "DungeonStageDetail";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        DungeonMapBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _chapterUnit = values[0] as ConfigDungeonChapterUnit;
        _dungeonStage = DataManager.Instance.GetRoleData().GetDungeonStageId((DungeonType) _chapterUnit.Type);
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.StageDetail.closeBtn.onClick.Add(this.Hide);
        this.StageDetail.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.StageDetail.sweepBtn.onClick.Add(this.OnClickSweepBtn);
        this.StageDetail.gotoBtn.onClick.Add(this.OnClickGoToBtn);
        this.StageDetail.preBtn.onClick.Add(this.OnClickPreBtn);
        this.StageDetail.nextBtn.onClick.Add(this.OnClickNextBtn);
        this.StageDetail.itemList.itemRenderer = this.OnDungeonItemListRender;
        this.StageDetail.adBtn.onClick.Add(this.OnClickAdBtn);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateDungeonInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateDungeonInfo);
    }

    protected override void OnShow()
    {
        base.OnShow();
        _curDungeonStage = _dungeonStage;
        UpdateDungeonInfo();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateDungeonInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateDungeonInfo);
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

        if (_curDungeonStage == 1)
        {
            UIManager.Instance.ToastByKey(10186);
            return;
        }
        int stage = _curDungeonStage;
        if (_curDungeonStage == _dungeonStage)
        {
            stage = _dungeonStage - 1;
        }
        var builder = Copy_End_CS.CreateBuilder();
        builder.CopyType = (eCopyType) _chapterUnit.Type;
        builder.StartStageId =(uint) ConfigUtils.GetDungeonStageByNandu(stage,_chapterUnit.Type).Id;
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
 
        // UIManager.Instance.CloseUIPanel("DungeonStageDetail");
        // UIManager.Instance.ShowUIPanel("DungeonMap", _stageUnit);
        DungeonMapManager.Instance.GuanKaStage = _curDungeonStage;
        var builder = Copy_Begin_CS.CreateBuilder();
        builder.CopyType = (eCopyType) _chapterUnit.Type;
        builder.StartStageId = (uint) ConfigUtils.GetDungeonStageByNandu(_curDungeonStage, _chapterUnit.Type).Id;
        Copy_Begin_CS copyBeginCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Copy_Begin_CS, copyBeginCs);
    }

    private void OnClickPreBtn()
    {
        if (_curDungeonStage > 1)
            _curDungeonStage--;
        UpdateDungeonInfo();
    }

    private void OnClickNextBtn()
    {
        if (_curDungeonStage < _dungeonStage)
            _curDungeonStage++;
        UpdateDungeonInfo();
    }

    private void UpdateDungeonInfo()
    {
        if (IsShow() && IsOnStage())
        {
            this.StageDetail.stageIdLb.SetVar("value", _curDungeonStage.ToString()).FlushVars();
            // this.StageDetail.preBtn.visible = _curDungeonStage > 1;
            // this.StageDetail.nextBtn.visible = _curDungeonStage < _dungeonStage;
            _stageUnit = ConfigUtils.GetDungeonStageByNandu(_curDungeonStage, _chapterUnit.Type);
            this.StageDetail.stageName.text = ConfigUtils.GetTextById(_stageUnit.Name,_stageUnit.NameParam);
            
            int groupId = _stageUnit.MonsterData[0];
            var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
            ConfigMonsterGroupUnit data = monsterGroupArr[0];
            ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);
            var monsterModelPath = monsterUnit.Model;
            Utils.SetSpineModelOnFGUI(this.StageDetail.spine, monsterModelPath, monsterUnit.BossSize - 30, "idle", null, true);
            
            string[] rewardArr = _stageUnit.PassReward.Split("|");
            List<ItemData> rewardItemList = new List<ItemData>();
            for (int i = 0; i < rewardArr.Length; i++)
            {
                string[] oneRewardArr = rewardArr[i].Split(",");
                ItemData itemData = new ItemData();
                itemData.id = int.Parse(oneRewardArr[0]);
                itemData.count = int.Parse(oneRewardArr[1]);
                rewardItemList.Add(itemData);
            }
            // this.StageDetail.stageIcon.url = UIResource.GetDungeonStageItemUrl(_chapterUnit.Banner);
            this.StageDetail.itemList.data = rewardItemList;
            this.StageDetail.itemList.numItems = rewardItemList.Count;

            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_chapterUnit.ItemID);
            this.StageDetail.itemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);

            int total = ConfigUtils.GetDungeonTotalCount(_chapterUnit.Id);
            this.StageDetail.itemCntLb.SetVar("cur",ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID).ToString()).SetVar("total", total.ToString()).FlushVars();
        
            this.StageDetail.countCtrl.selectedIndex = ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) > 0 ? 1 : 0;
            this.StageDetail.sweepBtnCtrl.selectedIndex = (_curDungeonStage == _dungeonStage) ? 1 : 0;
            this.StageDetail.sweepCtrl.selectedIndex =
                (_dungeonStage > 1 && ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) > 0) ? 0 : 1;
            
            if (ItemInfoManager.Instance.GetItemCount(_chapterUnit.ItemID) <= 0 && AdManager.Instance.GetAdFreeTimes(1000+(int) _chapterUnit.Type) > 0)
            {
                int totalAd = ConfigUtils.GetDungeonAdTotalCount(_chapterUnit.Id);
                ((UI_EmptyAdBtn2) this.StageDetail.adBtn).adCntLb.SetVar("cur",AdManager.Instance.GetAdFreeTimes(1000+(int) _chapterUnit.Type).ToString()).SetVar("total", totalAd.ToString()).FlushVars();
                this.StageDetail.adCtrl.selectedIndex = 1;
            }
            else
            {
                this.StageDetail.adCtrl.selectedIndex = 0;
            }
        }
        
    }
    private void OnDungeonItemListRender(int index, GObject item)
    {
        List<ItemData> rewardItemList = item.parent.data as List<ItemData>;
        ((UI_ItemCom)item).SetItemDataWithGuid(rewardItemList[index], true);
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
