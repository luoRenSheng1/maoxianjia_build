using BigMap;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChapterStageDetailView : UIViewBase
{
    private UI_ChapterStageDetail stageDetail => this.main as UI_ChapterStageDetail;

    private ConfigStageUnit stageUnit;
    
    /// <summary>
    /// 章节数据
    /// </summary>
    /// <returns></returns>
    private ChapterMapInfoData chapterMapData;
    
    private List<int> traitArr; // 属性配置列表
    private ConfigStageMonsterAttrUnit monsterAttr; //关卡对应的怪物词条
    private List<ConfigMonsterEntryUnit> monsterEntryList = new List<ConfigMonsterEntryUnit>(); // 属性配置列表
    public List<ConfigMonsterEntryUnit> monsterEntryGroupUnits; //怪物词条组
    private string[] rewardArr; // 掉落奖励配置列表
    private string[] badgeArr; // 徽章配置列表
    private string[] firstRewardArr; // 首次通关奖励配置列表
    
    private List<int> dropItems = new List<int>();//掉落奖励配置列表

    private bool isEnoughBadge = false; //是否集齐徽章

    private JumpTypeEnum _jumpTypeEnum;  //是否显示手指
    
    public ChapterStageDetailView()
    {
        this.package = "BigMap";
        this.name = "ChapterStageDetail";
        this.component = "ChapterStageDetail";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        BigMapBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.stageDetail.btnFight.onClick.Add(this.OnClickFightBtn);
        this.stageDetail.btnFight2.onClick.Add(this.OnClickFightBtn);
        this.stageDetail.closeBtn.onClick.Add(this.OnClickCloseButton);
        this.stageDetail.attrList.itemRenderer = MonsterAttributeRender;
        this.stageDetail.rewardList.itemRenderer = ShowDropRewardRenfer;
        this.stageDetail.badgeList.itemRenderer = BadgeListRender;
        this.stageDetail.firstRwList.itemRenderer = FirstRewardRender;
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        stageUnit = values[0] as ConfigStageUnit;
        
        _jumpTypeEnum = JumpTypeEnum.Normal;
        if (values.Length > 1 && values[1] is JumpTypeEnum)
        {
            if ((JumpTypeEnum)values[1] != JumpTypeEnum.Normal)
            {
                _jumpTypeEnum = (JumpTypeEnum)values[1];
            }
        }
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        MapChapterManager.Instance.theLastChallengeStageId = 0;
        
        chapterMapData = MapChapterManager.Instance.GetChapterMapInfoData();

        int groupId = stageUnit.MonsterData;
        var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
        ConfigMonsterGroupUnit data = monsterGroupArr[0];
        ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);
        
        // this.stageDetail.txtName.text = ConfigUtils.GetTextById(stageUnit.Name, stageUnit.NameParam);
        this.stageDetail.txtName.text = string.Format(ConfigUtils.GetTextById(stageUnit.Name), stageUnit.Chapter, stageUnit.LevelId % 100);
        this.stageDetail.txtDesc.text = ConfigUtils.GetTextById(monsterUnit.Name);
        // this.stageDetail.monsterIcon.url = UIResource.GetPetIcon(monsterUnit.MonsterBody);
        
        var monsterModelPath = monsterUnit.Model;
        Utils.SetSpineModelOnFGUI(this.stageDetail.monsterIcon, monsterModelPath, monsterUnit.BossSize, "idle", null, true);

        // this.stageDetail.txtMonsterAtk.text = stageUnit.MonsterAtk;
        this.stageDetail.txtMonsterAtk.text = stageUnit.SuggestionStr;
        
        traitArr = Utils.GetMonsterEntryById(stageUnit.Id);
        if (traitArr != null && traitArr.Count > 0)
        {
            monsterAttr = ConfigUtils.GetStageMonsterAttrUnitByIndexId(stageUnit.Id);
            monsterEntryList.Clear();
            monsterEntryGroupUnits = ConfigUtils.GetMonsterEntryByGroup(monsterAttr.EntryGroup).ToList(); //改词条组列表
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
                this.stageDetail.attrList.numItems = monsterEntryList.Count;
                this.stageDetail.attrList.ResizeToFit();
                this.stageDetail.isTx.selectedIndex = 0;
            }
            else
            {
                this.stageDetail.isTx.selectedIndex = 1;
            }
        }
        else
        {
            this.stageDetail.isTx.selectedIndex = 1;
        }

        // stageUnit.Drop = "2000," + stageUnit.Gold + "|" + stageUnit.Drop;  //奖励要包含金币
        // rewardArr = stageUnit.Drop.Split("|");
        // List<string> tempList = new List<string>(rewardArr);
        // tempList.Insert(0,"2000,"+stageUnit.Gold);
        // rewardArr = tempList.ToArray(); 
        // // rewardArr.SetValue("2000,"+stageUnit.Gold,0); 
        // this.stageDetail.rewardList.numItems = rewardArr.Length;

        // List<ConfigDropUnit> dropUnits = ConfigUtils.GetDropUnitByDropGroup(int.Parse(stageUnit.Drop));
        dropItems.Clear();
        //掉落金币
        dropItems.Add((int)DropItemType.GOLD);
        List<ConfigDropUnit> dropUnits = new List<ConfigDropUnit>();
        string[] dorpStr = stageUnit.Drop.Split('|');
        foreach (var str in dorpStr)
        {
            List<ConfigDropUnit> dropUnitList = new List<ConfigDropUnit>();
            dropUnitList = ConfigUtils.GetDropUnitByDropGroup(int.Parse(str));
            dropUnits.AddRange(dropUnitList);
        }
        //掉落展示处理
        List<int> dropItems2 = new List<int>();//处理完之后的物品列表
        dropItems2 = HanlderDorpShowItem(dropUnits);
        dropItems.AddRange(dropItems2);
        this.stageDetail.rewardList.numItems = dropItems.Count;
        
        firstRewardArr = stageUnit.FirstReward.Split("|");
        this.stageDetail.firstRwList.numItems = firstRewardArr.Length;
        
        this.stageDetail.btnFight2.grayed = false;
        this.stageDetail.bossStageCtrl.selectedIndex = 0;
        if ((stageUnit.LevelId%5) == 0)  //是隐藏boss关卡
        {
            this.stageDetail.btnFight2.grayed = true;
            this.stageDetail.bossStageCtrl.selectedIndex = 1;
            this.stageDetail.badgeList.visible = true;
            this.stageDetail.unlockTypeCtrl.selectedIndex = 0;
            if (stageUnit.UnlockType == 0) //没有限制 都可以挑战
            {
                this.stageDetail.badgeList.visible = false;
                this.stageDetail.btnFight2.grayed = false;
                isEnoughBadge = true;
            }
            else if (stageUnit.UnlockType == 1) //通过几关显示  通过前几关才会隐藏遮挡 所以直接显示
            {
                this.stageDetail.badgeList.visible = false;
                this.stageDetail.btnFight2.grayed = false;
                this.stageDetail.unlockTypeCtrl.selectedIndex = 1;
                isEnoughBadge = true;
            }
            else if (stageUnit.UnlockType == 2) //收集徽章
            {
                isEnoughBadge = false;
                string[] badgeArr = stageUnit.Level.Split("|");
                int itemNun = 0;
                foreach (var items in badgeArr)
                {
                    string[] itemArr = items.Split(",");
                    if (ItemInfoManager.Instance.GetItemCount(int.Parse(itemArr[0])) >= int.Parse(itemArr[1]))
                        itemNun++;
                }
                
                if (itemNun >= badgeArr.Length)
                {
                    this.stageDetail.btnFight2.grayed = false;
                    isEnoughBadge = true;
                }
            }
            else if (stageUnit.UnlockType == 3) //收集宠物
            {
            }
        }

        //引导-挑战按钮
        if(this.stageDetail.bossStageCtrl.selectedIndex == 0 && stageUnit.LevelId == 1001)
        {
            //{//因为有一个解锁提示，所以需要等它消失
            GuideManager.Instance.StarGuideByData(new GuideData()
            {
                bid = GuideID.NewAccount_ClickEquip,
                giding = GuideID.NewAccount_ClickGate,
                gid = GuideID.NewAccount_ClickChallenge,
                tui = this.stageDetail.btnFight,
                isForce = true,
                isSend = true,
                isLucency = false,
            });
        }

        if (_jumpTypeEnum != JumpTypeEnum.Normal && !GuideManager.Instance.IsShowGuiding)
        {
            JumpManager.Instance.ShowFinger(_jumpTypeEnum, this.stageDetail.btnFight);
        }
    }

    /// <summary>
    /// 掉落奖励展示处理
    /// </summary>
    private List<int> HanlderDorpShowItem(List<ConfigDropUnit> dropUnitList2)
    {
        //规则：首掉落位：金币    第2掉落位置：秘典，展示通用秘典图标  第3掉落位置：圣物，展示通用圣物图标  第4掉落位置：传承装备，展示通用传承装备图标  第5掉落位置：常规读取
        List<int> dropItemList = new List<int>();
        foreach (var dropUnit in dropUnitList2)
        {
            ConfigItemTypeUnit itemUnit = ConfigUtils.GetConfigItemTypeUnitById(dropUnit.Item);
            if (itemUnit.Type == 11)
            {
                dropItemList.Add((int)DropItemType.CLASSIC);
            }
            else if (itemUnit.Type == 16)
            {
                dropItemList.Add((int)DropItemType.HOLY);
            }
            else
            {
                dropItemList.Add(dropUnit.Item);
            }
        }
        
        dropItemList = dropItemList.Distinct().ToList();//去除重复奖励
        
        dropItemList.Add((int)DropItemType.INHERIT);//传承装备
        
        //排序
        dropItemList = dropItemList.OrderBy(item => 
            {
                // 优先级顺序
                if (item == (int)DropItemType.CLASSIC) return 0;
                if (item == (int)DropItemType.HOLY) return 1;
                if (item == (int)DropItemType.INHERIT) return 2;
                return 3; // 其他类型
            })
            .ThenBy(item => item) // 相同优先级时按大小升序
            .ToList();
        
        return dropItemList;
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.stageDetail.monsterIcon);
        if (_jumpTypeEnum != JumpTypeEnum.Normal)
        {
            _jumpTypeEnum = JumpTypeEnum.Normal;
            JumpManager.Instance.HideFinger();
        }
    }

    #region 弹属性详情
    private void MonsterAttributeRender(int index, GObject item)
    {
        if (monsterEntryList[index] == null) return;
        
        // ((UI_attrBtn)item).icon = UIResource.GetAttributeIconById(monsterEntryList[index].Icon);
        ((UI_attrBtn)item).icon = UIResource.GetMonsterEntryIcon(monsterEntryList[index].Icon);//等美术资源
        ((UI_attrBtn)item).qualityCtrl.selectedIndex = monsterEntryList[index].EntryGroup - 1;
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
    #endregion
    
    #region 掉落奖励展示  徽章列表  首次通关可获得的奖励
    private void ShowDropRewardRenfer(int index, GObject item)
    {
        // string[] items = rewardArr[index].Split(",");
        // SetListItem(items, item);

        int itemId = dropItems[index];
        ((UI_ItemCom)item).itemSpineEff.visible = false;
        ((UI_ItemCom) item).txtLv.visible = false;
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemId);
        ((UI_ItemCom) item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());
        ((UI_ItemCom) item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        ((UI_ItemCom)item).data = itemId;
        ((UI_ItemCom)item).onClick.Set(this.OnClickDropReward);
    }

    private void OnClickDropReward(EventContext context)
    {
        int itemId = (int)(context.sender as UI_ItemCom).data;
        TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
    }

    private void BadgeListRender(int index, GObject item)
    {
        string[] items = badgeArr[index].Split(",");
        // ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(items[0]));
        // ((UI_ItemCom) item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        // ((UI_ItemCom)item).txtLv.text = "";
        // ((UI_ItemCom)item).data = int.Parse(items[0]);
        // ((UI_ItemCom) item).onClick.Set(ClickBadgeItem);
        SetListItem(items, item);
        
        item.grayed = true;

        if (index < chapterMapData.badgeStageIdsList.Count)
            item.grayed = false;
        
        // if (ItemInfoManager.Instance.GetItemCount(int.Parse(items[0])) > 0)
        // {
            // item.grayed = false;
        // }
    }

    private void ClickBadgeItem(EventContext context)
    {   //物品提示
        int itemId = (int)((UI_ItemCom)context.sender).data;
        TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
    }
    
    private void FirstRewardRender(int index, GObject item)
    {
        string[] items = firstRewardArr[index].Split(",");
        SetListItem(items, item);
    }

    private void SetListItem(string [] items, GObject item)
    {
        ((UI_ItemCom)item).itemSpineEff.visible = false;
        
        ((UI_ItemCom) item).txtLv.text = items[1].ToString();
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(items[0]));
        ((UI_ItemCom) item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        
        ItemData itemData = new ItemData()
        {
            id = int.Parse(items[0]),
            count = 0,//int.Parse(items[1])
        };
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);  //提示
    }
    #endregion
    
    private void OnClickFightBtn(EventContext context)
    {
        if ((stageUnit.LevelId % 5) == 0 && !isEnoughBadge) //是隐藏boss关卡
        {
            UIManager.Instance.ToastByKey(8008);
            return;
        }
        
        var builder = ChooseStage_CS.CreateBuilder();
        builder.StartStageId = (ulong)stageUnit.LevelId;
        builder.StartChapterId = (ulong)stageUnit.Chapter;
        Debug.Log("=发送数据===selectId="+stageUnit.Id+"==LevelId="+stageUnit.LevelId+"==ChapterId="+stageUnit.Chapter);
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ChooseStage_CS, builder.Build()); //通知服务器当前关卡完成
        
        UIManager.Instance.CloseUIPanel("ChapterStageDetail");

        GuideManager.Instance.HideGuide();
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.CloseUIPanel("ChapterStageDetail");
    }
}
