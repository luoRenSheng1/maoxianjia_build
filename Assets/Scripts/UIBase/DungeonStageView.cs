using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonEx;
using Config;
using DungeonMap;
using Engine;
using EngineBase;
using FairyGUI;
using Lobby;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class DungeonStageView : UIViewBase
{
    private UI_DungeonStage dungeonStage => this.main as UI_DungeonStage;

    private List<ConfigDungeonChapterUnit> _dungeonChapterUnits = new List<ConfigDungeonChapterUnit>();
    private ConfigCommonUnit _commonUnit;
    private Coroutine _coTimeFlow;

    private DungeonType _dungeonType = DungeonType.NONE;
    public DungeonStageView()
    {
        this.name = "DungeonStage";
        this.package = "DungeonMap";
        this.component = "DungeonStage";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        DungeonMapBinder.BindAll();
    }

    // TODO 暂时注释
    // protected override void OnUpdateParams(params object[] values)
    // {
    //     base.OnUpdateParams(values);
    //     if(values[0] != null)
    //         _dungeonType = (DungeonType) values[0];
    // }

    protected override void OnInit()
    {
        base.OnInit();
        // this.dungeonStage.closeBtn.onClick.Add(this.Hide);
        this.dungeonStage.closeBtn.onClick.Add(this.HideWithSoundEffect);
        _commonUnit = ConfigDataGroup.GetInstance<ConfigCommon>().Get(20);
        var list = ConfigDataGroup.GetInstance<ConfigDungeonChapter>().Data.Values.ToList();
        _dungeonChapterUnits.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Id < 100000) // 随机事件副本不显示在列表 (int)DungeonType.BossWithPet)
            {
                _dungeonChapterUnits.Add(list[i]);
            }
        }
        // _dungeonChapterUnits = ConfigDataGroup.GetInstance<ConfigDungeonChapter>().Data.Values.ToList();
        this.dungeonStage.stageList.itemRenderer = StageListItemRender;
        ((UI_ComMessage) this.dungeonStage.comMessage).InitChatInfo();
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateDungeonStage);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DUNGEON_OPEN_STAGE_UPDATE, this.UpdateDungeonStage);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateDungeonItem);
    }

    protected override void OnShow()
    {
        base.OnShow();
        var builder = CopyInfo_CS.CreateBuilder();
        CopyInfo_CS copyInfoCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_CopyInfo_CS, copyInfoCs);

        this.dungeonStage.tiemdesc.text = ConfigUtils.GetStringByKey(1001);
        StartTimeLeftCd();
    }
    
    public void StartTimeLeftCd()
    {
        if (null != _coTimeFlow)
            GameManager.Instance.StopCoroutine(_coTimeFlow);

        int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
        if (totalSecond > 0)
        {
            string x = StringUtils.GetTimeString((int)totalSecond);
            // this.dungeonStage.timeLeft.SetVar("value", StringUtils.GetTimeString((int)totalSecond)).FlushVars();
            this.dungeonStage.timeLeft.text = StringUtils.GetTimeString((int)totalSecond);
            _coTimeFlow = GameManager.Instance.StartCoroutine(TimeFlow());
        }
        
            
    }

    IEnumerator TimeFlow()
    {
        while (true)
        {
            int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
            if (totalSecond > 0)
            {
                this.dungeonStage.timeLeft.SetVar("value", StringUtils.GetTimeString((int)totalSecond)).FlushVars();
            }

            if (totalSecond <= 0)
            {
                GameManager.Instance.TimerManager.SetTimer(1, () =>
                {
                    //发送协议 宝箱倒计时完成
                    var builder = EquipBox_Levelup_CS.CreateBuilder();
                    EquipBox_Levelup_CS levelup = builder.Build();
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_EquipBox_Levelup_CS, levelup);
                });
                break;
            }

            yield return GameManager.Instance.waitSec1;
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        if (null != _coTimeFlow)
            GameManager.Instance.StopCoroutine(_coTimeFlow);
        _coTimeFlow = null;
        _dungeonType = DungeonType.NONE;
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateDungeonStage);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DUNGEON_OPEN_STAGE_UPDATE, this.UpdateDungeonStage);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateDungeonItem);
    }

    private void UpdateDungeonItem()
    {
        if (IsShow() && IsOnStage())
        {
            this.dungeonStage.stageList.numItems = _dungeonChapterUnits.Count;
        }
    }

    private void UpdateDungeonStage()
    {
        this.dungeonStage.stageList.numItems = _dungeonChapterUnits.Count;
        this.dungeonStage.stageList.EnsureBoundsCorrect();
        
        //var dungeonMap1 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_gold);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_Dungeon_Gold))
        //{
        //    GuideManager.Instance.HideGuide();
        //    this.dungeonStage.stageList.ScrollToView(0);
        //    UI_DungeonItem dungeonItem =  this.dungeonStage.stageList.GetChildAt(0) as UI_DungeonItem;
        //    GuideManager.Instance.StartGuide(dungeonItem.gotoBtn,GuideID.Trigger_Click_Dungeon_Gold, PosType.Left, true, true);
        //}
        
        //var dungeonMap2 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_zhuzhao);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_Dungeon_Zhuzhaochui) && dungeonMap2.Item1)
        //{
        //    GuideManager.Instance.HideGuide();
        //    this.dungeonStage.stageList.ScrollToView(1);
        //    UI_DungeonItem dungeonItem =  this.dungeonStage.stageList.GetChildAt(1) as UI_DungeonItem;
        //    GuideManager.Instance.StartGuide(dungeonItem.gotoBtn,GuideID.Trigger_Click_Dungeon_Zhuzhaochui, PosType.Left, true, true);
        //}
        
        //var dungeonMap3 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_dimoand);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_Dungeon_Dimond) && dungeonMap3.Item1)
        //{
        //    GuideManager.Instance.HideGuide();
        //    this.dungeonStage.stageList.ScrollToView(2);
        //    UI_DungeonItem dungeonItem =  this.dungeonStage.stageList.GetChildAt(2) as UI_DungeonItem;
        //    GuideManager.Instance.StartGuide(dungeonItem.gotoBtn,GuideID.Trigger_Click_Dungeon_Dimond, PosType.Left, true, true);
        //}
        
        //var dungeonMap4 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_exp);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_Dungeon_Exp) && dungeonMap4.Item1)
        //{
        //    GuideManager.Instance.HideGuide();
        //    this.dungeonStage.stageList.ScrollToView(3);
        //    UI_DungeonItem dungeonItem =  this.dungeonStage.stageList.GetChildAt(3) as UI_DungeonItem;
        //    GuideManager.Instance.StartGuide(dungeonItem.gotoBtn,GuideID.Trigger_Click_Dungeon_Exp, PosType.Left, true, true);
        //}

        //var dungeonMap5 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_petMatial);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_Dungeon_Rune) && dungeonMap5.Item1)
        //{
        //    GuideManager.Instance.HideGuide();
        //    this.dungeonStage.stageList.ScrollToView(4);
        //    UI_DungeonItem dungeonItem =  this.dungeonStage.stageList.GetChildAt(4) as UI_DungeonItem;
        //    GuideManager.Instance.StartGuide(dungeonItem.gotoBtn,GuideID.Trigger_Click_Dungeon_Rune, PosType.Left, true, true);
        //}
        
        if (_dungeonType == DungeonType.Exp)
        {
            this.dungeonStage.stageList.ScrollToView(3);
        }else if (_dungeonType == DungeonType.Gold)
        {
            this.dungeonStage.stageList.ScrollToView(0);
        }else if (_dungeonType == DungeonType.Zhuzhao)
        {
            this.dungeonStage.stageList.ScrollToView(1);
        }
        
    }

    private void StageListItemRender(int index, GObject item)
    {
        ConfigDungeonChapterUnit chapterUnit = _dungeonChapterUnits[index];
        ((UI_DungeonItem) item).icon = UIResource.GetDungeonStageItemUrl(chapterUnit.Banner);
        // ((UI_DungeonItem) item).title = chapterUnit.Name;
        ((UI_DungeonItem) item).title = ConfigUtils.GetTextById(chapterUnit.Name);
        string[] rewardArr = chapterUnit.PassReward.Split("|");
        List<ItemData> rewardItemList = new List<ItemData>();
        for (int i = 0; i < rewardArr.Length; i++)
        {
            string[] oneRewardArr = rewardArr[i].Split(",");
            if (oneRewardArr.Length > 1)
            {
                ItemData itemData = new ItemData();
                itemData.id = int.Parse(oneRewardArr[0]);
                itemData.count = double.Parse(oneRewardArr[1]);
                rewardItemList.Add(itemData);
            }
        }
        ((UI_DungeonItem) item).stageItemList.itemRenderer = this.OnDungeonItemListRender;
        ((UI_DungeonItem) item).stageItemList.data = rewardItemList;
        ((UI_DungeonItem) item).stageItemList.numItems = rewardItemList.Count;
        int total = ConfigUtils.GetDungeonTotalCount(chapterUnit.Id);
        ((UI_DungeonItem) item).itemCntLb.SetVar("cur",ItemInfoManager.Instance.GetItemCount(chapterUnit.ItemID).ToString()).SetVar("total", total.ToString()).FlushVars();
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(chapterUnit.ItemID);
        ((UI_DungeonItem) item).itemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
        // var funcType = this.GetDungeonFuncType((DungeonType) chapterUnit.Type);
        var funcType = DungeonMapManager.Instance.GetDungeonFuncType((DungeonType) chapterUnit.Type);
        var dungeonMap = FuncPreviewManger.Instance.GetFuncOpenState(funcType);
        ((UI_DungeonItem) item).lockCtrl.selectedIndex = dungeonMap.Item1 ? 0 : 1;
        ((UI_DungeonItem) item).lockDesc.text = dungeonMap.Item2;
        ((UI_DungeonItem)item).gotoBtn.onClick.Set(() =>
        {
            GuideManager.Instance.HideGuide();
            if (!dungeonMap.Item1)
            {
                UIManager.Instance.Toast(dungeonMap.Item2);
            }
            else
            {
                UIManager.Instance.ShowUIPanel("DungeonStageDetail", _dungeonChapterUnits[index]);
            }
        });

        ((UI_DungeonItem) item).countCtr.selectedIndex =
            ItemInfoManager.Instance.GetItemCount(chapterUnit.ItemID) > 0 ? 1 : 0;

        if (ItemInfoManager.Instance.GetItemCount(chapterUnit.ItemID) <= 0 && AdManager.Instance.GetAdFreeTimes(1000+(int) chapterUnit.Type) > 0)
        {
            int totalAd = ConfigUtils.GetDungeonAdTotalCount(chapterUnit.Id);
            ((UI_EmptyAdBtn2) ((UI_DungeonItem) item).adBtn).adCntLb.SetVar("cur",AdManager.Instance.GetAdFreeTimes(1000+(int) chapterUnit.Type).ToString()).SetVar("total", totalAd.ToString()).FlushVars();
            ((UI_DungeonItem) item).adCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_DungeonItem) item).adCtrl.selectedIndex = 0;
        }
        ((UI_DungeonItem)item).adBtn.onClick.Set(() =>
        {
            AdManager.Instance.WatchAd(() =>
            {
                var builder = GetCopyFreeTimesByAD_CS.CreateBuilder();
                builder.CopyType = (eCopyType) chapterUnit.Type;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GetCopyFreeTimesByAD_CS, builder.Build());
            });

        });
        
        ((UI_DungeonItem)item).tipBtn.onClick.Set(() =>
        {
            // UIManager.Instance.ShowUIPanel("TipInfoShow", chapterUnit.HelpInformation);
            UIManager.Instance.ShowUIPanel("TipInfoShow", ConfigUtils.GetTextById(chapterUnit.HelpInformation));
        });
        
        // 红点
        ((UI_DungeonItem)item).gotoBtnRed.visible = ItemInfoManager.Instance.GetItemCount(chapterUnit.ItemID) > 0;
        ((UI_DungeonItem)item).adBtnRed.visible = ItemInfoManager.Instance.GetItemCount(chapterUnit.ItemID) <= 0 && AdManager.Instance.GetAdFreeTimes(1000+(int) chapterUnit.Type) > 0;
    }
    
    private void OnDungeonItemListRender(int index, GObject item)
    {
        List<ItemData> rewardItemList = item.parent.data as List<ItemData>;
        ((UI_ItemCom) item).SetItemDataWithGuid(rewardItemList[index], false);
    }

    //转移到DungeonMapManager中了
    // private FuncOpenType GetDungeonFuncType(DungeonType type)
    // {
    //     FuncOpenType openType = 0;
    //     switch (type)
    //     {
    //         case DungeonType.Diamond:
    //             openType = FuncOpenType.Dungeon_dimoand;
    //             break;
    //         case DungeonType.Gold:
    //             openType = FuncOpenType.Dungeon_gold;
    //             break;
    //         case DungeonType.Zhuzhao:
    //             openType = FuncOpenType.Dungeon_zhuzhao;
    //             break;
    //         case DungeonType.Exp:
    //             openType = FuncOpenType.Dungeon_exp;
    //             break;
    //         case DungeonType.PetMaterial:
    //             openType = FuncOpenType.Dungeon_petMatial;
    //             break;
    //         case DungeonType.PetSkillBook:
    //             openType = FuncOpenType.Dungeon_petSkillBook;
    //             break;
    //         case DungeonType.GodEquip:
    //             openType = FuncOpenType.Dungeon_godEquip;
    //             break;
    //         case DungeonType.Holy:
    //             openType = FuncOpenType.Dungeon_holy;
    //             break;
    //     }
    //
    //     return openType;
    // }
}
