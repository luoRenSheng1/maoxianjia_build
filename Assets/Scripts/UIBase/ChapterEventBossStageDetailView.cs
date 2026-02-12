using BigMap;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChapterEventBossStageDetailView : UIViewBase
{
    private UI_ChapterEventBossStageDetail eventBossDetail => this.main as UI_ChapterEventBossStageDetail;

    private RandomEventData mapEventData;
    private int batchStuffId = -1; //怪物的索引id
    public bool isRandom = false; //随机位置的
    public int extraCfgId = -1; //深埋宝藏
    private Coroutine _coTimeFlow;
    private ConfigEventUnit eventData = null;//静态表
    
    private List<int> traitArr; // 属性配置列表
    private ConfigStageMonsterAttrUnit monsterAttr; //关卡对应的怪物词条
    private List<ConfigMonsterEntryUnit> monsterEntryList = new List<ConfigMonsterEntryUnit>(); // 属性配置列表
    public List<ConfigMonsterEntryUnit> monsterEntryGroupUnits; //怪物词条组
    private string[] rewardArr; // 掉落奖励配置列表
    private List<string> showRewardArr = new List<string>(); // 掉落奖励展示列表
    
    private JumpTypeEnum _jumpTypeEnum;  //是否显示手指
    
    public ChapterEventBossStageDetailView()
    {
        this.package = "BigMap";
        this.name = "ChapterEventBossStageDetail";
        this.component = "ChapterEventBossStageDetail";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        BigMapBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.eventBossDetail.btnFight.onClick.Add(this.OnClickFightBtn);
        this.eventBossDetail.closeBtn.onClick.Add(this.OnClickCloseButton);
        
        this.eventBossDetail.attrList.itemRenderer = MonsterAttributeRender;
        this.eventBossDetail.rewardList.itemRenderer = ShowDropRewardRenfer;
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        mapEventData = values[0] as RandomEventData;
        eventData = ConfigUtils.GetEventDataById(mapEventData.eventId);
        extraCfgId = -1;
        if (values[1] != null && values[1] is int)
        {
            batchStuffId = (int)values[1];
        }
        if (values.Length >= 2 )
        {
            isRandom = values[2] as bool? == true;

            if (values.Length > 3)
            {
                if(values[3] as bool? == true)
                {//深埋宝藏boss
                    foreach (var item in mapEventData.batchStuffList)
                    {
                        if(item.id == batchStuffId)
                        {
                            //eventData = ConfigUtils.GetEventDataById(item.extraCfgId);
                            extraCfgId = item.extraCfgId;
                            break;
                        }
                    }
                }
            }
        }

        _jumpTypeEnum = JumpTypeEnum.Normal;
        if (values.Length >= 4 && values[4] is JumpTypeEnum)
        {
            if ((JumpTypeEnum)values[4] != JumpTypeEnum.Normal)
            {
                _jumpTypeEnum = (JumpTypeEnum)values[4];
            }
        }

        SetData();
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.eventBossDetail.monsterIcon);
        if (_jumpTypeEnum != JumpTypeEnum.Normal)
        {
            _jumpTypeEnum = JumpTypeEnum.Normal;
            JumpManager.Instance.HideFinger();
        }
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        
        if (_jumpTypeEnum != JumpTypeEnum.Normal && !GuideManager.Instance.IsShowGuiding)
        {
            JumpManager.Instance.ShowFinger(_jumpTypeEnum, this.eventBossDetail.btnFight);
        }
    }
    
    private void SetData()
    { 
        //ConfigEventUnit eventData = ConfigUtils.GetEventDataById(mapEventData.eventId);
        if (eventData == null)
        {
            return;
        }
        this.eventBossDetail.txtName.text = ConfigUtils.GetTextById(eventData.Name);

        int cfgId = 0;
        foreach (var stuffData in mapEventData.batchStuffList)
        {
            if (stuffData.id == batchStuffId)
            {
                cfgId = stuffData.cfgId;
                break;
            }
        }

        ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(extraCfgId == -1 ? cfgId : extraCfgId);
        
        int groupId = eventStageUnit.MonsterData;
        //Debug.Log($"==cfgId==={cfgId}=====extraCfgId=={extraCfgId}========groupId==={groupId}");
        var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
        ConfigMonsterGroupUnit data = monsterGroupArr[0];
        ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);
        
        this.eventBossDetail.txtDesc.text = ConfigUtils.GetTextById(monsterUnit.Name);
        var monsterModelPath = monsterUnit.Model;
        Utils.SetSpineModelOnFGUI(this.eventBossDetail.monsterIcon, monsterModelPath, monsterUnit.BossSize, "idle", null, true);
        
        // 词条
        if (eventStageUnit.EntryNumber > 0)
        {
            traitArr = Utils.GetEventStageMonsterEntry(eventStageUnit);
            //monsterAttr = ConfigUtils.GetStageMonsterAttrUnitByIndexId(eventStageUnit.Id);
            monsterEntryList.Clear();
            monsterEntryGroupUnits = ConfigUtils.GetMonsterEntryByGroup(eventStageUnit.EntryGroup).ToList(); //改词条组列表
            for (int i = 0; i < traitArr.Count; i++)
            {
                for (int j = 0; j < monsterEntryGroupUnits.Count; j++)
                {
                    if (traitArr[i] == j)
                    {
                        monsterEntryList.Add(monsterEntryGroupUnits[j]);
                    }
                }
            }

            if (monsterEntryList.Count > 0)
            {
                this.eventBossDetail.attrList.numItems = monsterEntryList.Count;
                this.eventBossDetail.attrList.ResizeToFit();
                this.eventBossDetail.isTx.selectedIndex = 0;
            }
            else
            {
                this.eventBossDetail.isTx.selectedIndex = 1;
            }
        }
        else
        {
            this.eventBossDetail.isTx.selectedIndex = 1;
        }
        
        //rewardArr = eventStageUnit.Drop.Split("|"); //只有一个组，改为int类型
        // rewardArr.SetValue("2000,"+stageUnit.Gold,0); //奖励要包含金币
        
        showRewardArr.Clear();
        if (eventData.Type == (int)StageEventType.Pet)
        {
            //怪物对应的宠物物品id（怪物表里面的宠物id前面加20是宠物的物品id）
            showRewardArr.Add("20"+monsterUnit.PetBasis+",1");
        }
        
        // showRewardArr.Add("2000,"+eventStageUnit.Gold);
        // List<ConfigDropUnit> dropUnits = new List<ConfigDropUnit>();
        // foreach (var reward in rewardArr)
        // {
            // List<ConfigDropUnit> dropUnitList = new List<ConfigDropUnit>();
            // dropUnitList = ConfigUtils.GetDropUnitByDropGroup(reward);
            // dropUnits.AddRange(dropUnitList);
        // }
        
        //掉落展示处理
        List<string> dropItems2 = new List<string>();//处理完之后的物品列表
        dropItems2 = HanlderDorpShowItem( ConfigUtils.GetDropUnitByDropGroup(eventStageUnit.Drop) );
        showRewardArr.AddRange(dropItems2); 
        this.eventBossDetail.rewardList.numItems = showRewardArr.Count;

        var frame = main.GetChild("frame");
        if (null != frame)
        {
            frame.asCom.onClick.Set(() => { SetVisible((false)); });
        }
        
        //随机事件点位处理
        this.eventBossDetail.isRandom.selectedIndex = 0;
        if (isRandom)
        {
            this.eventBossDetail.isRandom.selectedIndex = 1;
            
            if (null != frame)
            {
                frame.asCom.onClick.Set(() => { });
            }
            
            if (null != _coTimeFlow)
                GameManager.Instance.StopCoroutine(_coTimeFlow);
            _coTimeFlow = GameManager.Instance.StartCoroutine(OnTimeDown());
        }
    }

    IEnumerator OnTimeDown()
    {
        int time = 5;
        while (time >= 0)
        {
            this.eventBossDetail.txtTitleTime.SetVar("value", time.ToString()).FlushVars();
            yield return new WaitForSeconds(1);
            time--;
        }
        OnClickFightBtn();
    }
    
    /// <summary>
    /// 掉落奖励展示处理
    /// </summary>
    private List<string> HanlderDorpShowItem(List<ConfigDropUnit> dropUnitList2)
    {
        //规则：首掉落位：金币    第2掉落位置：秘典，展示通用秘典图标  第3掉落位置：圣物，展示通用圣物图标  第4掉落位置：传承装备，展示通用传承装备图标  第5掉落位置：常规读取
        List<string> dropItemList = new List<string>();
        foreach (var dropUnit in dropUnitList2)
        {
            ConfigItemTypeUnit itemUnit = ConfigUtils.GetConfigItemTypeUnitById(dropUnit.Item);
            if(itemUnit == null)
            {
                continue;
            }
            if (itemUnit.Type == 11)
            {
                dropItemList.Add(((int)DropItemType.CLASSIC).ToString() +",1");
            }
            else if (itemUnit.Type == 16)
            {
                dropItemList.Add(((int)DropItemType.HOLY).ToString() + ",1");
            }
            else
            {
                dropItemList.Add(dropUnit.Item.ToString() + ",1");
            }
        }
        
        dropItemList = dropItemList.Distinct().ToList();//去除重复奖励
        
        dropItemList.Add(((int)DropItemType.INHERIT).ToString() + ",1");
        
        //排序
        dropItemList = dropItemList.OrderBy(item => 
            {
                // 优先级顺序
                if (item == ((int)DropItemType.CLASSIC).ToString() +",1") return 0;
                if (item == ((int)DropItemType.HOLY).ToString() + ",1") return 1;
                if (item == ((int)DropItemType.INHERIT).ToString() + ",1") return 2;
                return 3; // 其他类型
            })
            .ToList();
        
        return dropItemList;
    }

    private void OnClickFightBtn()
    {
        if (null != _coTimeFlow)
            GameManager.Instance.StopCoroutine(_coTimeFlow);
        
        //保存数据
        DungeonMapManager.Instance.mapEventData = this.mapEventData;
        DungeonMapManager.Instance.batchStuffId = this.batchStuffId;
        
        MapChapterManager.Instance.HideMapObject();
        
        //RandomEventData taskData = mapStageData.stageTaskData;
        //ConfigEventUnit eventData = ConfigUtils.GetEventDataById(mapEventData.eventId);
        if (eventData != null)
        {
            //通知服务器挑战随机事件的boss或宠物
            if (eventData.Type == (int)StageEventType.Boss) //游荡的BOSS
            {
                var builder = AttackWildBossBegin_CS.CreateBuilder();
                builder.EventGuid = (ulong)mapEventData.guid;
                builder.BatchStuffId = (uint)batchStuffId;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AttackWildBossBegin_CS, builder.Build()); 
            }
            else if (eventData.Type == (int)StageEventType.Pet) //逃跑的宠物
            {
                var builder = AttackWildPetBegin_CS.CreateBuilder();
                builder.EventGuid = (ulong)mapEventData.guid;
                builder.BatchStuffId = (uint)batchStuffId;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AttackWildPetBegin_CS, builder.Build()); 
            }
            else if (eventData.Type == (int)eRandomEventType.eRandomEventType_RandomBox) //深埋宝藏
            {
                var builder = AttackBoxBossBegin_CS.CreateBuilder();
                builder.EventGuid = (ulong)mapEventData.guid;
                builder.StuffId = (uint)batchStuffId;

                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackBoxBossBegin_CS, builder.Build());
            }
        }
        UIManager.Instance.CloseUIPanel("ChapterEventBossStageDetail");
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.CloseUIPanel("ChapterEventBossStageDetail");
    }
    
    #region 弹属性详情  掉落奖励展示
    private void MonsterAttributeRender(int index, GObject item)
    {
        if (monsterEntryList[index] == null) return;
        
        // ((UI_attrBtn)item).icon = UIResource.GetAttributeIconById(monsterEntryList[index].Icon);
        ((UI_attrBtn)item).icon = UIResource.GetMonsterEntryIcon(monsterEntryList[index].Icon);//等美术资源
        ((UI_attrBtn) item).qualityCtrl.selectedIndex = monsterEntryList[index].EntryGroup - 1;
        ((UI_attrBtn) item).name.text = ConfigUtils.GetTextById(monsterEntryList[index].MonsterEntryName);
        ((UI_attrBtn) item).data = index;
        ((UI_attrBtn) item).onClick.Set(OnClickAttrTips);
    }
    
    private void OnClickAttrTips(EventContext context)
    {
        int index = (int)((UI_attrBtn)context.sender).data;
        
        TipsManger.Instance.ShowPopupTip((UI_attrBtn)context.sender, Tipstype.Monster, ConfigUtils.GetTextById(monsterEntryList[index].MonsterEntryName),
            ConfigUtils.GetTextById(monsterEntryList[index].MonsterEntryDoc), monsterEntryList[index].Icon);
    }
    
    private void ShowDropRewardRenfer(int index, GObject item)
    {
        string[] items = showRewardArr[index].Split(",");
        ((UI_ItemCom)item).itemSpineEff.visible = false;
        
        // ((UI_ItemCom) item).txtLv.text = items[1].ToString();
        ((UI_ItemCom) item).txtLv.visible = false;
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(items[0]));
        ((UI_ItemCom) item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        ((UI_ItemCom) item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        
        ((UI_ItemCom)item).data = itemTypeUnit.Id;
        ((UI_ItemCom)item).onClick.Set(this.OnClickDropReward);
        
        // ItemData itemData = new ItemData()
        // {
        //     id = int.Parse(items[0]),
        //     count = 0,//int.Parse(items[1])
        // };
        // ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);  //提示
    }
    
    private void OnClickDropReward(EventContext context)
    {
        int itemId = (int)(context.sender as UI_ItemCom).data;
        TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
    }
    
    #endregion
}
