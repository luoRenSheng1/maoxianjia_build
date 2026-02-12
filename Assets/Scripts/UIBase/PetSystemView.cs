using System.Collections.Generic;
using System.Linq;
using Common;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Pet;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using PetBattleAttr = Engine.PetBattleAttr;
using PetSkillBook = Engine.PetSkillBook;
using PetSkillBookSlot = Engine.PetSkillBookSlot;
using PetTalent = Engine.PetTalent;
using UI_PetItem = Pet.UI_PetItem;

public enum SelectBookType
{
    /// <summary>
    /// 技能书道具
    /// </summary>
    BOOKITEM = 0,
    /// <summary>
    /// 技能书
    /// </summary>
    BOOK = 1
}

public class PetSystemView : UIViewBase
{
    private UI_PetSystem petUI => this.main as UI_PetSystem;
    
    private List<PetItemInfo> _petItemInfos;
    private Dictionary<int, PetItemInfo> _upLoadPetDict = new Dictionary<int, PetItemInfo>();
    private Dictionary<int, UI_PetUpLoadItem> _petUpLoadItemDict = new Dictionary<int, UI_PetUpLoadItem>(5);
    private PetItemInfo _uploadPet;
    
    private int _openIndex = -1;
    private int _preSelectIndex;
    
    private UI_PetSystemBattle _petBattleUI;
    
    private UI_PetSystemTalent _petTalentUI;
    // private List<int> _talentIdList = new List<int>();
    private List<PetTalent> _talentIdList =  new List<PetTalent>();
    private List<int> _selectTalentIdList = new List<int>();//需要洗练的天赋id列表
    private ulong _selectedPetGuid;//选择的宠物的guid
    private ConfigCommonUnit _common300009;
    private ConfigCommonUnit _common300010;

    private Dictionary<int,int> itemDict = new Dictionary<int,int>();//天赋锁定消耗道具
    private int costToolId;//天赋锁定道具id
    private int totalCost = 0;
    
    private UI_PetSystemBook _petBookUI;
    private List<PetSkillBookSlot> _skillBookSlotList = new List<PetSkillBookSlot>();
    private ulong _unlockSlotPetGuid;//解锁宠物技能书槽位的宠物id
    private int _unlockSlotId = 1;//解锁槽位id
    private ConfigCommonUnit _common300006;
    private List<PetSkillBook> _skillBookList = new List<PetSkillBook>();//宠物技能书，背包
    private List<ItemData> _skillBookItemList = new List<ItemData>();//宠物技能书-物品表
    // private int _selectedSkillBookId;//选择要打书的技能书id
    private long _selectedSkillBookId;//选择要打书的技能书guid
    private int _selectedBookType;//选择要打书的技能书类型：0-技能书道具  1-技能书

    private ConfigCommonUnit _common3003;
    private ConfigCommonUnit _common3004;
    private ConfigCommonUnit _common3005;
    
    public PetSystemView()
    {
        this.name = "PetSystem";
        this.package = "Pet";
        this.component = "PetSystem";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        PetBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        if(values[0] != null)
            _openIndex = (int) values[0];
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        this.petUI.EnsureBoundsCorrect();
        
        this.petUI.tabList.onClickItem.Add(OnClickBottomItem);

        #region 详情界面
        this.petUI.petDetail.attrList.itemRenderer = PetDetailAttrRender;
        this.petUI.petDetail.petTalentList.itemRenderer = PetDetailTalentRender;
        this.petUI.petDetail.petBookList.itemRenderer = PetDetailBookRender;
        this.petUI.petDetail.petList.itemRenderer = PetDetailBattleRender;
        this.petUI.petDetail.upLvBtn.onClick.Add(OnClickUpLvBtn);
        this.petUI.petDetail.strongBtn.onClick.Add(OnClickStrongBtn);
        this.petUI.petDetail.btnAttr.onClick.Add(OnClickAttributeBtn);
        #endregion
        
        // 上阵界面
        _petBattleUI = UIPackage.CreateObject("Pet", "PetSystemBattle") as UI_PetSystemBattle;
        this.petUI.AddChildAt(_petBattleUI, 1);
        _petBattleUI.AddRelation(this.petUI, RelationType.Width);
        _petBattleUI.AddRelation(this.petUI, RelationType.BottomExt_Bottom);
        _petBattleUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        // this.petUI.petBattle.allStrengthBtn.onClick.Add(this.OnClickAllPetStrengthBtn);
        _petBattleUI.petAllList.itemRenderer = PetAllItemRender;
        for (int i = 0; i < 3; i++)
        {
            UI_PetUpLoadItem upLoadItem = _petBattleUI.GetChild("petUpItem" + i) as UI_PetUpLoadItem;
            _petUpLoadItemDict.Add(i, upLoadItem);
            upLoadItem?.onClick.Add(this.OnClickUpLoadPetListItem);
            PetUpListRender(i, upLoadItem);
        }
        _petBattleUI.recycleBtn.onClick.Add(this.OnClickPetRecycleBtn);
        
        // 宠物天赋界面
        _petTalentUI = UIPackage.CreateObject("Pet", "PetSystemTalent") as UI_PetSystemTalent;
        this.petUI.AddChildAt(_petTalentUI, 2);
        _petTalentUI.AddRelation(this.petUI, RelationType.Width);
        _petTalentUI.AddRelation(this.petUI, RelationType.BottomExt_Bottom);
        _petTalentUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        _common300009 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300009);
        _common300010 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300010);
        _petTalentUI.tipsBtn.onClick.Add(this.OnClickTalentTipsBtn);
        _petTalentUI.list.itemRenderer = PetTalentBattleRender;
        _petTalentUI.talentList.itemRenderer = TalentListRender;
        _petTalentUI.talentBtn.onClick.Add(this.OnClickResetTalentBtn);
        
        // 技能书界面
        _petBookUI = UIPackage.CreateObject("Pet", "PetSystemBook") as UI_PetSystemBook;
        this.petUI.AddChildAt(_petBookUI, 3);
        _petBookUI.AddRelation(this.petUI, RelationType.Width);
        _petBookUI.AddRelation(this.petUI, RelationType.BottomExt_Bottom);
        _petBookUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        _common300006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300006);
        _petBookUI.tipsBtn.onClick.Add(this.OnClickBookTipsBtn);
        _petBookUI.recycleBtn.onClick.Add(this.OnClickBookRecycleBtn);//宠物技能书回收
        _petBookUI.skillBookList.itemRenderer = PetBookSlotListRender;//宠物技能书槽位
        _petBookUI.list.itemRenderer = PetBookListRender;//宠物技能书
        _petBookUI.petList.itemRenderer = PetSkillBookBattleRender;
        // _petBookUI.makeBtn.onClick.Add(this.OnClickMakeBookSlotBtn);
        _petBookUI.makeBtnAni.onClick.Add(this.OnClickMakeBookSlotBtn);
        _petBookUI.openBookBtn.onClick.Add(this.OnClickOpenBookBtn);

        _common3003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3003);
        _common3004 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3004);
        _common3005 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3005);

        this.petUI.tabList.selectedIndex = 0;
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateDetailUI);//宠物升级功能解锁
        EventDispatcher.GameWorld.Regist<PetItemInfo>(EventDefine.EVENT_UPLOAD_PET, this.ShowUploadPetOpt);  //宠物上阵
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PET_SC_SUCC, this.UpdatePetSCSuccsss);  //宠物上阵服务器回包
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PET_LEVELUP_SUCCESS, this.UpdateDetailPetInfo); //宠物升级、宠物技能升级服务器回包
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RESET_PET_TELENT_INFO, this.UpdateTalentInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PETBOOKSLOT_UNLOCK, this.UpdateBookSlot);//技能书开槽刷新
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_GET_PET_SKILLBOOK, this.UpdateSKillBookInPackage);//技能书背包刷新
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PETBOOK_UPDATE, this.UpdateBookUI);//打书刷新
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECYCLE_PET, this.UpdatePetInfo);//宠物回收刷新
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECYCLE_SKILL_BOOK,UpdateSKillBookInPackage);//技能书回收刷新
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateDetailUI);//宠物升级功能解锁
        EventDispatcher.GameWorld.UnRegist<PetItemInfo>(EventDefine.EVENT_UPLOAD_PET, this.ShowUploadPetOpt);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PET_SC_SUCC, this.UpdatePetSCSuccsss);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PET_LEVELUP_SUCCESS, this.UpdateDetailPetInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RESET_PET_TELENT_INFO, this.UpdateTalentInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PETBOOKSLOT_UNLOCK, this.UpdateBookSlot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_GET_PET_SKILLBOOK, this.UpdateSKillBookInPackage);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PETBOOK_UPDATE, this.UpdateBookUI);//打书刷新
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECYCLE_PET, this.UpdatePetInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECYCLE_SKILL_BOOK,UpdateSKillBookInPackage);
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        if (_openIndex != -1)
        {
            ChangeIndex(_openIndex);;
        }
        else
        {
            if (this.petUI.tabList.selectedIndex == -1)
                this.petUI.tabList.selectedIndex = 0;
            ChangeIndex(this.petUI.tabList.selectedIndex);
        }
        this.petUI.tabList.EnsureBoundsCorrect();
        
        GameManager.Instance.TimerManager.SetTimer(0.3f, RefreshBottomRedDot);

        //if (!GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_PetBtn))
        //{
        //    GuideManager.Instance.HideGuide();
        //    //宠物上阵标签按钮
        //    GuideManager.Instance.StartGuide(this.petUI.tabList.GetChildAt(1), GuideID.Click_PetBtn, PosType.Left, true, false);
        //}
        //宠物上阵标签按钮
        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_OpenPet,
            gid = GuideID.Click_PetBtn,
            tui = this.petUI.tabList.GetChildAt(1),
            isForce = true,
            isSend = false
        });
    }

    protected override void OnHide()
    {
        base.OnHide();
        _openIndex = 0;
        Utils.ClearSpineModelOnFGUI(this.petUI.petDetail.petSpine);

        if(GuideManager.Instance.GuideId == (int)GuideID.Click_UIPetClose2)
        {
            GuideManager.Instance.SendToCompleteGuide((int)GuideID.Click_UIPetClose2);
            GuideManager.Instance.HideGuide();
        }
    }
    
    private void OnClickBottomItem(EventContext context)
    {
        GButton item = context.data as GButton;
        var index = this.petUI.tabList.GetChildIndex(item);
        ChangeIndex(index);
    }
    
    private void ChangeIndex(int index)
    {
        bool isChange = true;
        switch (index)
        {
            case 0:
                _preSelectIndex = index;
                selectPetIndex = 0;
                ShowDetailPanel();
                break;
            case 1:
                _preSelectIndex = index;
                UpdatePetInfo();
                //if (!GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_FirstPetItem))
                //{
                //    GuideManager.Instance.HideGuide();
                //    GuideManager.Instance.StartGuide(this._petBattleUI.petAllList.GetChildAt(0), GuideID.Click_FirstPetItem, PosType.Left, true, false);
                //}
                GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    giding = GuideID.Click_PetBtn,
                    gid = GuideID.Click_FirstPetItem,
                    tui = this._petBattleUI.petAllList.GetChildAt(0),
                    isForce = true,
                    isSend = false,
                    npcTxt = "Beginner_Doc_013",
                    npcPosType = PosType.Down,
                });
                break;
            case 2:
                var petTalentMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetTalent);
                if (petTalentMap.Item1)
                {
                    _preSelectIndex = index;
                    UpdateTalentUI();
                }
                else
                {
                    UIManager.Instance.Toast(petTalentMap.Item2);
                    ((UI_TabBtn) this.petUI.tabList.GetChildAt(index)).selected = false;
                    isChange = false;
                }
                break;
            case 3:
                var petSkillBookMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetSkillBook);
                if (petSkillBookMap.Item1)
                {
                    selectBookPetIndex = 0;
                    UpdateBookUI();
                    _preSelectIndex = index;
                    _unlockSlotId = 1;
                }
                else
                {
                    UIManager.Instance.Toast(petSkillBookMap.Item2);
                    ((UI_TabBtn) this.petUI.tabList.GetChildAt(index)).selected = false;
                    isChange = false;
                }
                break;
        }

        if (isChange)
        {
            _petBattleUI.visible = index == 1;
            _petTalentUI.visible = index == 2;
            _petBookUI.visible = index == 3;
        }

        for (int i = 0; i < 4; i++)
        {
            if(i == _preSelectIndex)
                ((UI_TabBtn) this.petUI.tabList.GetChildAt(_preSelectIndex)).selected = true;
            else
            {
                ((UI_TabBtn) this.petUI.tabList.GetChildAt(i)).selected = false;
            }
        }

        this.petUI.typeCtrl.selectedIndex = _preSelectIndex;
    }

    public void RefreshBottomRedDot()
    {
        //宠物详情
        ((UI_TabBtn)this.petUI.tabList.GetChildAt(0)).redCtrl.selectedIndex = PetInfoManager.Instance.PetDetailRedPointHandle() ? 1 : 0;
        
        //宠物上阵
        ((UI_TabBtn)this.petUI.tabList.GetChildAt(1)).redCtrl.selectedIndex = PetInfoManager.Instance.PetBattleRedPointHandle() ? 1 : 0;
        
        //宠物天赋
        var petTalentMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetTalent);
        if (!petTalentMap.Item1)
        {
            ((UI_TabBtn)this.petUI.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_TabBtn)this.petUI.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 0;
            ((UI_TabBtn)this.petUI.tabList.GetChildAt(2)).redCtrl.selectedIndex = PetInfoManager.Instance.PetTalentRedPointHandle() ? 1 : 0;
        }
        
        // 宠物技能书
        var petSkillBookMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetSkillBook);
        if (!petSkillBookMap.Item1)
        {
            ((UI_TabBtn)this.petUI.tabList.GetChildAt(3)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_TabBtn)this.petUI.tabList.GetChildAt(3)).lockCtrl.selectedIndex = 0;
            ((UI_TabBtn)this.petUI.tabList.GetChildAt(3)).redCtrl.selectedIndex = PetInfoManager.Instance.PetSkillBookRedPointHandle() ? 1 : 0;
        }
        
    }

    #region 详情界面
    private int selectPetIndex = 0;  //选中的宠物索引
    private List<PetItemInfo> battlePetInfos = new List<PetItemInfo>(); //详情界面上阵宠物
    private List<(string,string)> petAttrList = new List<(string,string)>();  //宠物基础属性

    private void ShowDetailPanel()
    {
        Debug.Log("===详情界面===");

        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        battlePetInfos.Clear();
        foreach (var data in _upLoadPetDict)
        {
            battlePetInfos.Add(data.Value);
        }

        //天赋解锁
        var unlock1 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetTalent);
        this.petUI.petDetail.TalentUnlock.selectedIndex = unlock1.Item1 ? 1 : 0;
        this.petUI.petDetail.TalentUnlockText.text = unlock1.Item2;
        //技能书解锁
        var unlock2 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetSkillBook);
        this.petUI.petDetail.SkillBookUnlock.selectedIndex = unlock2.Item1 ? 1 : 0;
        this.petUI.petDetail.SkillBookUnlockText.text = unlock2.Item2;

        if (battlePetInfos.Count > 0)
        {
            this.petUI.petDetail.hasUpload.selectedIndex = 1;
            
            //天赋 宠物书 上阵宠物
            this.petUI.petDetail.petTalentList.numItems = battlePetInfos[selectPetIndex].talentsList.Count;//5;
            this.petUI.petDetail.petBookList.numItems = battlePetInfos[selectPetIndex].bookSlotsList.Count;
            this.petUI.petDetail.petList.numItems = battlePetInfos.Count;
            
            this.petUI.petDetail.isShow.selectedIndex = 0;//不显示任何东西：美术要求
            
            //刷新详情界面UI
            UpdateDetailUI();
        }
        else
        {
            this.petUI.petDetail.hasUpload.selectedIndex = 0;
            //todo 没有宠物时 清空界面
            Utils.ClearSpineModelOnFGUI(this.petUI.petDetail.petSpine);
            
            this.petUI.petDetail.isShow.selectedIndex = 1;//不显示任何东西：美术要求
        }
        //砖石
        ((UI_Currency1)this.petUI.petDetail.currency1).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia);
        ((UI_Currency1)this.petUI.petDetail.currency1).icon = UIResource.GetItemUrl(1000.ToString());
        
        //宠物碎片
        ItemData itemData = ItemInfoManager.Instance.GetItemData(ConstDefine.PetPieceId);
        ((UI_Currency1) this.petUI.petDetail.currency2).txtValue.text = StringUtils.FormatCurrency(itemData.count);
        ((UI_Currency1) this.petUI.petDetail.currency2).icon = UIResource.GetItemUrl(itemData.id.ToString());
        
    }
    
    private void PetDetailAttrRender(int index,GObject item)  //属性列表
    {
        UI_CommonAttrItem attrItem = item as UI_CommonAttrItem;
        attrItem.pContent.text = petAttrList[index].Item1+ " " + petAttrList[index].Item2;
    }
    
    private void PetDetailTalentRender(int index,GObject item)  //天赋列表
    {
        ((UI_PetTalentItem)item).lockCtrl.selectedIndex = 0;
        // ConfigPetAptitudeUnit unit = ConfigUtils.GetPetAptitudeUnitById(battlePetInfos[selectPetIndex].talentsList[index]);
        ConfigPetAptitudeUnit unit = ConfigUtils.GetPetAptitudeUnitById(battlePetInfos[selectPetIndex].talentsList[index].talentId);
        ((UI_PetTalentItem)item).qualityIcon.visible = true;
        ((UI_PetTalentItem)item).qualityCtrl.selectedIndex = unit.Quality - 1;
        // ((UI_PetTalentItem)item).icon = UIResource.GetItemUrl(unit.Item.ToString());//天赋图标
        ((UI_PetTalentItem)item).icon = UIResource.GetPetTalentIcon(unit.Item.ToString());//天赋图标
        ((UI_PetTalentItem)item).talentName.visible = true;
        ((UI_PetTalentItem)item).talentName.text = ConfigUtils.GetTextById(unit.Name);
        // ((UI_PetTalentItem)item).data = unit.Id;
        ((UI_PetTalentItem)item).data = battlePetInfos[selectPetIndex].talentsList[index];
        ((UI_PetTalentItem)item).onClick.Set(this.OnClickTalentItem);
    }

    private void OnClickTalentItem(EventContext context)
    {
        // int talentId = (int)(context.sender as UI_PetTalentItem)?.data;
        // UIManager.Instance.ShowUIPanel("PetSkillTips", 1, talentId);
        
        PetTalent talent = (context.sender as UI_PetTalentItem)?.data  as PetTalent;
        UIManager.Instance.ShowUIPanel("PetSkillTips", 1, talent);
    }

    private void PetDetailBookRender(int index,GObject item)  //宠物书列表
    {
        // 解锁且有装备
        if (battlePetInfos[selectPetIndex].bookSlotsList[index].BookId != 0 && battlePetInfos[selectPetIndex].bookSlotsList[index].SlotStatus == eSlotStatus.eSlotStatus_Normal)
        {
            var unit = ConfigUtils.GetPetSkillBookUnitById(battlePetInfos[selectPetIndex].bookSlotsList[index].BookId);
            // ((UI_PetBookItem)item).icon = UIResource.GetItemUrl(unit.Icon.ToString());
            ((UI_PetBookItem)item).icon = UIResource.GetPetSkillBookIcon(unit.Icon.ToString());
            ((UI_PetBookItem)item).name.visible = true;
            ((UI_PetBookItem)item).name.text = ConfigUtils.GetTextById(unit.Name);
            ((UI_PetBookItem)item).qualityIcon.visible = true;
            ((UI_PetBookItem)item).qualityCtrl.selectedIndex = unit.Quality - 1;
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 2;
            // ((UI_PetBookItem)item).data = unit.Id;
            ((UI_PetBookItem)item).data = battlePetInfos[selectPetIndex].bookSlotsList[index];
            ((UI_PetBookItem)item).onClick.Set(this.OnClickSkillBookItem);
        }

        // 未解锁
        if (battlePetInfos[selectPetIndex].bookSlotsList[index].SlotStatus == eSlotStatus.eSlotStatus_Locked)
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 1;
            ((UI_PetBookItem)item).onClick.Set(this.OnClickSkillBookItem2);
        }

        // 解锁且没有装备
        if (battlePetInfos[selectPetIndex].bookSlotsList[index].BookId == 0 &&  battlePetInfos[selectPetIndex].bookSlotsList[index].SlotStatus == eSlotStatus.eSlotStatus_Normal) 
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 0;
            ((UI_PetBookItem)item).onClick.Set(this.OnClickSkillBookItem3);
        }
        
        ((UI_PetBookItem)item).selectIcon.visible = false;
    }

    private void OnClickSkillBookItem(EventContext context)
    {
        PetSkillBookSlot skillBookSlot = (context.sender as UI_PetBookItem)?.data as PetSkillBookSlot;

        if (skillBookSlot == null)
            return;
        
        UIManager.Instance.ShowUIPanel("PetSkillTips", 2, skillBookSlot);
    }
    
    private void OnClickSkillBookItem2()
    {
        UIManager.Instance.Toast("未解锁！");
    }

    private void OnClickSkillBookItem3()
    {
        UIManager.Instance.Toast("未装备技能书！");
    }

    private void PetDetailBattleRender(int index,GObject item)  //上阵宠物列表
    {
        UI_PetItem2 petItem = item as UI_PetItem2;

        ConfigPetBasisUnit petBasisUnit = battlePetInfos[index].petCfg;
        // petItem.icon = UIResource.GetPetIcon(petBasisUnit.IconPath);
        petItem.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petBasisUnit.IconPath)).Icon);;
        // petItem.qualityCtrl.selectedIndex = battlePetInfos[selectPetIndex].quality - 1;//petBasisUnit.Quality-1;
        petItem.qualityCtrl.selectedIndex = battlePetInfos[index].quality - 1;
        // petItem.petLv.text = "";
        petItem.data = index;
        petItem.onClick.Set(this.OnPetItemClick);
                
        petItem.GetController("button").selectedIndex = 0;
        if (selectPetIndex == index)
        {
            petItem.GetController("button").selectedIndex = 1;
        }
        
        // 红点
        petItem.redPoint.visible = PetInfoManager.Instance.ShowPetDetailBattlePetRedPointInPetList(battlePetInfos[index]);
    }

    private void OnPetItemClick(EventContext context)
    {
        // PetItemInfo pet = (context.sender as UI_PetItem)?.data as PetItemInfo;
        // if(pet == null) return;
        int index = (int)(context.sender as UI_PetItem2)?.data;
        if (selectPetIndex == index)
        {
            (context.sender as UI_PetItem2).GetController("button").selectedIndex = 1;
            return;
        }
        selectPetIndex = index;
        //天赋 宠物书 上阵宠物
        this.petUI.petDetail.petTalentList.numItems = battlePetInfos[selectPetIndex].talentsList.Count;
        this.petUI.petDetail.petBookList.numItems = battlePetInfos[selectPetIndex].bookSlotsList.Count;
        this.petUI.petDetail.petList.numItems = battlePetInfos.Count;
        
        UpdateDetailUI();
    }

    private void UpdateDetailUI()
    {
        if(battlePetInfos.Count - 1 < selectPetIndex || battlePetInfos[selectPetIndex] == null) { return; }

        ConfigPetBasisUnit petBasisUnit = battlePetInfos[selectPetIndex].petCfg;
        
        if(petBasisUnit == null) { return; }

        // 没有上阵宠物时，界面不显示任何东西：美术要求
        if (battlePetInfos.Count == 0 || battlePetInfos == null)
        {
            this.petUI.petDetail.isShow.selectedIndex = 1;
        }
        else
        {
            this.petUI.petDetail.isShow.selectedIndex = 0;
        }

        //砖石
        ((UI_Currency1)this.petUI.petDetail.currency1).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia);
        ((UI_Currency1)this.petUI.petDetail.currency1).icon = UIResource.GetItemUrl(1000.ToString());
        
        //宠物碎片
        ItemData itemData = ItemInfoManager.Instance.GetItemData(ConstDefine.PetPieceId);
        ((UI_Currency1) this.petUI.petDetail.currency2).txtValue.text = StringUtils.FormatCurrency(itemData.count);
        ((UI_Currency1) this.petUI.petDetail.currency2).icon = UIResource.GetItemUrl(itemData.id.ToString());
        
        Utils.ClearSpineModelOnFGUI(this.petUI.petDetail.petSpine);
        Utils.SetSpineModelOnFGUI(this.petUI.petDetail.petSpine, petBasisUnit.PetModel, 90, "idle");
        
        this.petUI.petDetail.petName.text = ConfigUtils.GetTextById(petBasisUnit.Name);
        this.petUI.petDetail.petLv.SetVar("value",battlePetInfos[selectPetIndex].PetLv.ToString()).FlushVars();
        this.petUI.petDetail.petQuality.qualityCtrl.selectedIndex = battlePetInfos[selectPetIndex].quality - 1;//petBasisUnit.Quality-1;
        this.petUI.petDetail.quality.selectedIndex = battlePetInfos[selectPetIndex].quality - 1;
        this.petUI.petDetail.type.type.selectedIndex = petBasisUnit.Type-1;
        
        //策划写死
        ConfigAttrEnumerationUnit attr29 = ConfigUtils.GetAttrEnumerationUnitByAttrId(29);
        ConfigAttrEnumerationUnit attr30 = ConfigUtils.GetAttrEnumerationUnitByAttrId(30);
        petAttrList.Clear();
        petAttrList.Add((ConfigUtils.GetStringByKey(8047), (battlePetInfos[selectPetIndex].growRate*100).ToString("f2")+"%" ));  //成长率
        petAttrList.Add((ConfigUtils.GetTextById(attr29.AttrName), (battlePetInfos[selectPetIndex].FightAttrVo.CriticalStrike*100).ToString("f2")+"%" ));  //暴击率
        petAttrList.Add((ConfigUtils.GetStringByKey(8048), (battlePetInfos[selectPetIndex].petCfg.Inherit * ConstDefine.CONFIG_PLACE_EX*100).ToString("f2")+"%" ));  //继承属性
        petAttrList.Add((ConfigUtils.GetTextById(attr30.AttrName), (battlePetInfos[selectPetIndex].FightAttrVo.CriticalInjury*100).ToString("f2")+"%" ));  //暴伤
        petAttrList.Add((ConfigUtils.GetStringByKey(8049), StringUtils.FormatCurrency(battlePetInfos[selectPetIndex].FightAttrVo.Atk) ));  //宠物攻击力
        this.petUI.petDetail.attrList.numItems = petAttrList.Count;
        
        //宠物升级解锁
        ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(battlePetInfos[selectPetIndex].quality, battlePetInfos[selectPetIndex].PetLv+1);
        var petLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetLvUp);
        if (petLvUpMap.Item1)
        {
            this.petUI.petDetail.lockIcon.visible = false;
            
            //宠物升级按钮
            // ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(battlePetInfos[selectPetIndex].quality, battlePetInfos[selectPetIndex].PetLv+1);
            if (petItem != null)
            {
                this.petUI.petDetail.upLvBtn.grayed = false;
                this.petUI.petDetail.upLvBtn.visible = true;
                this.petUI.petDetail.upLvBtnRed.visible = true;
                if (itemData.count < petItem.PetDebris)
                {
                    this.petUI.petDetail.upLvBtn.grayed = true;
                    this.petUI.petDetail.upLvBtnRed.visible = false;
                }
                ((UI_BtnCom2)this.petUI.petDetail.upLvBtn).itemIcon.url = UIResource.GetItemUrl(itemData.id.ToString());
                ((UI_BtnCom2)this.petUI.petDetail.upLvBtn).num.text = petItem.PetDebris.ToString();
            }
            else
                this.petUI.petDetail.upLvBtn.visible = false;
        }
        else
        {
            this.petUI.petDetail.upLvBtn.grayed = true;
            this.petUI.petDetail.lockIcon.visible = true;
            ((UI_BtnCom2)this.petUI.petDetail.upLvBtn).itemIcon.url = UIResource.GetItemUrl(itemData.id.ToString());
            ((UI_BtnCom2)this.petUI.petDetail.upLvBtn).num.text = petItem.PetDebris.ToString();
        }
        
        // //宠物升级按钮
        // ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(battlePetInfos[selectPetIndex].quality, battlePetInfos[selectPetIndex].PetLv+1);
        // if (petItem != null)
        // {
        //     this.petUI.petDetail.upLvBtn.grayed = false;
        //     this.petUI.petDetail.upLvBtn.visible = true;
        //     if (itemData.count < petItem.PetDebris)
        //     {
        //         this.petUI.petDetail.upLvBtn.grayed = true;
        //     }
        //     ((UI_BtnCom2)this.petUI.petDetail.upLvBtn).itemIcon.url = UIResource.GetItemUrl(itemData.id.ToString());
        //     ((UI_BtnCom2)this.petUI.petDetail.upLvBtn).num.text = petItem.PetDebris.ToString();
        // }
        // else
        //     this.petUI.petDetail.upLvBtn.visible = false;
        
        // this.petUI.petDetail.petSkillBtn.icon = UIResource.GetItemUrl(battlePetInfos[selectPetIndex].skillId.ToString());
        this.petUI.petDetail.petSkillBtn.icon = UIResource.GetPetInitSkillIcon(ConfigUtils.GetSkillById(battlePetInfos[selectPetIndex].skillId).SkillIcon);
        this.petUI.petDetail.petSkillBtn.qualityCtrl.selectedIndex = ConfigUtils.GetSkillById(battlePetInfos[selectPetIndex].skillId).SkillQuality - 1;
        this.petUI.petDetail.petSkillBtn.num.visible = true;
        this.petUI.petDetail.petSkillBtn.num.SetVar("value",battlePetInfos[selectPetIndex].skillLevel.ToString()).FlushVars();
        this.petUI.petDetail.skillName.text = ConfigUtils.GetTextById(ConfigUtils.GetSkillById(battlePetInfos[selectPetIndex].skillId).Name);
        
        ConfigPetSkillLevelUnit petSkillLevelData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(battlePetInfos[selectPetIndex].skillId, battlePetInfos[selectPetIndex].skillLevel);
        
        if (petSkillLevelData != null)
        {
            if (petSkillLevelData.SkillHurt > 0)
                this.petUI.petDetail.skillDesc.text = string.Format(ConfigUtils.GetTextById(petSkillLevelData.Doc), (petSkillLevelData.SkillHurt * ConstDefine.CONFIG_PLACE).ToString("f2"));
            else
            {
                if (!petSkillLevelData.CoverCommonParam.Equals("0"))
                {
                    string[] param = petSkillLevelData.CoverCommonParam.Split("|");
                    string[] commons = param[0].Split(",");
                    if (commons.Length > 1)
                    {
                        // string lastValue = EquipManager.Instance.SetAttributeValue(int.Parse(commons[0]), double.Parse(commons[1]), true);
                        // string x = ConfigUtils.GetTextById(petSkillLevelData.Doc);
                        // this.petUI.petDetail.skillDesc.text = string.Format(ConfigUtils.GetTextById(petSkillLevelData.Doc), lastValue);
                        
                        // CoverCommonParam全为万分比
                        string lastValue = (int.Parse(commons[1]) * ConstDefine.CONFIG_PLACE).ToString("f2") + "%";
                        this.petUI.petDetail.skillDesc.text = string.Format(ConfigUtils.GetTextById(petSkillLevelData.Doc), lastValue);
                    }
                    else
                    {
                        this.petUI.petDetail.skillDesc.text = "";
                    }
                }
            }
        }
        
        //宠物初始技能解锁
        ConfigPetSkillLevelUnit petSkillData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(battlePetInfos[selectPetIndex].skillId, battlePetInfos[selectPetIndex].skillLevel+1);
        var petSkillLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetSkillLvUp);
        if (petSkillLvUpMap.Item1)
        {
            this.petUI.petDetail.lockIcon2.visible = false;
            
            //宠物技能升级按钮
            if (petSkillData != null)
            {
                this.petUI.petDetail.strongBtn.grayed = false;
                this.petUI.petDetail.strongBtn.visible = true;
                this.petUI.petDetail.strongBtnRed.visible = true;
                if (itemData.count < petSkillData.PetDebris)
                {
                    this.petUI.petDetail.strongBtn.grayed = true;
                    this.petUI.petDetail.strongBtnRed.visible = false;
                }
                ((UI_BtnCom2)this.petUI.petDetail.strongBtn).itemIcon.url = UIResource.GetItemUrl(itemData.id.ToString());
                ((UI_BtnCom2)this.petUI.petDetail.strongBtn).num.text = petSkillData.PetDebris.ToString();
            }
            else
                this.petUI.petDetail.strongBtn.visible = false;
        }
        else
        {
            this.petUI.petDetail.strongBtn.grayed = true;
            this.petUI.petDetail.lockIcon2.visible = true;
            ((UI_BtnCom2)this.petUI.petDetail.strongBtn).itemIcon.url = UIResource.GetItemUrl(itemData.id.ToString());
            ((UI_BtnCom2)this.petUI.petDetail.strongBtn).num.text = petSkillData.PetDebris.ToString();
        }
        
        // //宠物技能升级按钮
        // ConfigPetSkillLevelUnit petSkillData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(battlePetInfos[selectPetIndex].skillId, battlePetInfos[selectPetIndex].skillLevel+1);
        // if (petSkillData != null)
        // {
        //     this.petUI.petDetail.strongBtn.grayed = false;
        //     this.petUI.petDetail.strongBtn.visible = true;
        //     if (itemData.count < petSkillData.PetDebris)
        //     {
        //         this.petUI.petDetail.strongBtn.grayed = true;
        //     }
        //     ((UI_BtnCom2)this.petUI.petDetail.strongBtn).itemIcon.url = UIResource.GetItemUrl(itemData.id.ToString());
        //     ((UI_BtnCom2)this.petUI.petDetail.strongBtn).num.text = petSkillData.PetDebris.ToString();
        // }
        // else
        //     this.petUI.petDetail.strongBtn.visible = false;
        
        
    }

    private void OnClickAttributeBtn()
    {
        if (battlePetInfos.Count > 0 && battlePetInfos[selectPetIndex] != null)
        {
            // UIManager.Instance.ShowUIPanel("AllHeroAttrPanel", battlePetInfos[selectPetIndex].FightAttrVo, battlePetInfos[selectPetIndex]);
            UIManager.Instance.ShowUIPanel("AllHeroAttrPanel", ObjType.PET, battlePetInfos[selectPetIndex].FightAttrVo, battlePetInfos[selectPetIndex]);
        }
    }
    
    private void OnClickUpLvBtn(EventContext context)
    {
        //宠物升级解锁
        var petLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetLvUp);
        if (!petLvUpMap.Item1)
        {
            UIManager.Instance.Toast(petLvUpMap.Item2);
            return;
        }

        ItemData itemData = ItemInfoManager.Instance.GetItemData(ConstDefine.PetPieceId);
        // PetItemInfo itemInfo = battlePetInfos[selectPetIndex];
        ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(battlePetInfos[selectPetIndex].quality, battlePetInfos[selectPetIndex].PetLv+1);
        if (petItem != null)
        {
            // if (petItem.PetLevel > HeroInfoManager.Instance.GetMaxHeroLv())
            // {
            //     UIManager.Instance.Toast(ConfigUtils.GetStringByKey(8051)); //宠物不能超出最高角色等级
            //     return;
            // }
            if (itemData.count < petItem.PetDebris)
            {
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.PetPieceId);
                UIManager.Instance.Toast(ConfigUtils.GetTextById(itemTypeUnit.Name)+ConfigUtils.GetStringByKey(43));
                return;
            }
            var builder = PetLevelUp_CS.CreateBuilder();
            builder.PetId = battlePetInfos[selectPetIndex].PetGuid;
            builder.LevelupTimes = 1;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetLevelUp_CS, builder.Build());
        }
        else
        {
            UIManager.Instance.Toast(ConfigUtils.GetStringByKey(5095)); //宠物等级达到上限
        }
        
    }
    
    private void OnClickStrongBtn(EventContext context)
    {
        var petSkillLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetSkillLvUp);
        if (!petSkillLvUpMap.Item1)
        {
            UIManager.Instance.Toast(petSkillLvUpMap.Item2);
            return;
        }
        
        ItemData itemData = ItemInfoManager.Instance.GetItemData(ConstDefine.PetPieceId);
        ConfigPetSkillLevelUnit petSkillData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(battlePetInfos[selectPetIndex].skillId, battlePetInfos[selectPetIndex].skillLevel+1);
        if (petSkillData != null)
        {
            if (itemData.count < petSkillData.PetDebris)
            {
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.PetPieceId);
                UIManager.Instance.Toast(ConfigUtils.GetTextById(itemTypeUnit.Name)+ConfigUtils.GetStringByKey(43));
                return;
            }
            var builder = PetSkillLevelUp_CS.CreateBuilder();
            builder.PetId = battlePetInfos[selectPetIndex].PetGuid;
            builder.LevelupTimes = 1;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetSkillLevelUp_CS, builder.Build());
        }
    }

    /// <summary>
    /// 宠物、宠物技能升级回调
    /// </summary>
    private void UpdateDetailPetInfo()
    {
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        battlePetInfos.Clear();
        foreach (var data in _upLoadPetDict)
        {
            battlePetInfos.Add(data.Value);
        }
        
        UpdateDetailUI(); //宠物升级 更新宠物面板
        this.petUI.petDetail.petList.numItems = battlePetInfos.Count;
        RefreshBottomRedDot();
    }
    #endregion
    
    #region 上阵界面
    /// <summary>
    /// 点击批量强化按钮
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    private void OnClickAllPetStrengthBtn()
    {
        // var builder = AllPetLevelUp_CS.CreateBuilder();
        // GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AllPetLevelUp_CS, builder.Build());
    }
    
    /// <summary>
    /// 点击上阵宠物
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    private void OnClickUpLoadPetListItem(EventContext context)
    {
        UI_PetUpLoadItem item = context.sender as UI_PetUpLoadItem;
        int index = int.Parse(item.name.Substring(9, 1));
        if (item.showHandCtrl.selectedIndex == 1)
        {
            // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(18);
            var builder = PetInSlot_CS.CreateBuilder();
            builder.PetId = _uploadPet.PetGuid;//(uint) _uploadPet.petCfg.Id
            builder.PetSlotId = (uint)index;
            PetInSlot_CS petInSlotCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetInSlot_CS, petInSlotCs);
        }
        else
        {
            if (item != null && item.state.selectedIndex == 1)
            {
                if (this._upLoadPetDict.TryGetValue(index, out var pet))
                {
                    //跳转宠物详细界面
                    UIManager.Instance.ShowUIPanel("PetDetail", pet);
                }
            } 
            else
            {
                //功能开启枚举 = 索引+2001
                var stateMap =  FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)(2001+index));
                if(!stateMap.Item1)
                    UIManager.Instance.Toast(stateMap.Item2);
                else
                    OnClickToAddPet();
            }
        }
    
    
    }
    
    private void OnClickToAddPet()
    {
        UIManager.Instance.ToastByKey(10111);
    }
    
    /// <summary>
    /// 初始化列表
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    private void PetAllItemRender(int index, GObject item)
    {
        ConfigPetBasisUnit petBasisUnit = _petItemInfos[index].petCfg;
        var pet = ((UI_PetItem)item);

        // ((UI_PetItem) item).icon = UIResource.GetPetIcon(petBasisUnit.IconPath);
        pet.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petBasisUnit.IconPath)).Icon);
        pet.qualityCtrl.selectedIndex = petBasisUnit.Quality-1;
        pet.petLv.SetVar("value", _petItemInfos[index].PetLv.ToString()).FlushVars();
        PetItemInfo hasPetInfo = PetInfoManager.Instance.GetPet(_petItemInfos[index].PetGuid);
        if (hasPetInfo != null)
        {
            pet.lockCtrl.selectedIndex = 0;
            //是否上阵
            bool isUpload = PetInfoManager.Instance.IsInUpload(hasPetInfo);
            if(isUpload)
            {
                pet.isUpload.selectedIndex = 1;
            }
            else if(hasPetInfo.DispatchBuild != VillageBuildType.None)
            {
                pet.isUpload.selectedIndex = 2;
            }
            else
            {
                pet.isUpload.selectedIndex = 0;
            }
        }
        else
        {
            pet.isUpload.selectedIndex = 0;
        }
        
        ((UI_PetItem) item).data = _petItemInfos[index];
        ((UI_PetItem) item).onClick.Set(this.OnClickPetItem);
        
        pet.redPoint.visible = false;
        int count = PetInfoManager.Instance.GetNoUploadSlotCount();//空栏位数量
        if (count > 0)
        {
            List<PetItemInfo> showRedPets = PetInfoManager.Instance.GetNoUploadPetHighQuality(count);
            if (showRedPets != null && showRedPets.Count > 0)
            {
                foreach (var redPet in showRedPets)
                {
                    if (redPet.PetGuid == hasPetInfo.PetGuid)
                    {
                        pet.redPoint.visible = true;
                    }
                }
            }
        }
        
        List<PetItemInfo> list = PetInfoManager.Instance.GetNoUploadPetHighQuality2();
        if (list != null && list.Count > 0)
        {
            foreach (var i in list)
            {
                if (i.PetGuid == hasPetInfo.PetGuid)
                {
                    pet.redPoint.visible = true;
                }
            }
        }
        
    }
    
    private void OnClickPetItem(EventContext context)
    {
        PetItemInfo pet = (context.sender as UI_PetItem)?.data as PetItemInfo;
        if(pet != null)
            UIManager.Instance.ShowUIPanel("PetDetail", pet);
    }
    
    //更新界面
    private void UpdatePetInfo()
    {
        Debug.Log("===上阵界面===");

        ((UI_Currency1)_petBattleUI.currency1).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.gold);
        ((UI_Currency1)_petBattleUI.currency2).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia);
        
        HideUploadPetView();
        _petItemInfos = PetInfoManager.Instance.GetAllHavePetList(); //改成只显示已经获得的宠物

        //排序数据 已上阵 > 已驻扎 > 品质
        List<PetItemInfo> ysz = new List<PetItemInfo>();
        List<PetItemInfo> yzz = new List<PetItemInfo>();
        List<PetItemInfo> myl = new List<PetItemInfo>();

        foreach (var item in _petItemInfos)
        {
            if (PetInfoManager.Instance.IsInUpload(item))
            {
                ysz.Add(item);
            }
            else if (item.DispatchBuild != VillageBuildType.None)
            {
                yzz.Add(item);
            }
            else
            {
                myl.Add(item);
            }
        }
        ysz.Sort(SortPet);
        ysz.Sort(SortPet);
        yzz.Sort(SortPet);
        yzz.Sort(SortPet);
        myl.Sort(SortPet);

        _petItemInfos = new List<PetItemInfo>(ysz);
        _petItemInfos.AddRange(yzz);
        _petItemInfos.AddRange(myl);

        if (_petItemInfos.Count == 0 || _petItemInfos == null)
        {
            _petBattleUI.isShow.selectedIndex = 1;
        }
        else
        {
            _petBattleUI.isShow.selectedIndex = 0;
        }
        
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        UpdatePetLevelInfo();
        UpdateUnlockPetPos();
        RefrehUpLoadItem();
    }

    public int SortPet(PetItemInfo a, PetItemInfo b)
    {
        // 品质降序排列
        int result = b.petCfg.Quality.CompareTo(a.petCfg.Quality);
        if (result == 0)
        {
            // 品质相同则按等级降序排列
            result = b.PetLv.CompareTo(a.PetLv);
            if (result == 0)
            {
                // 等级也相同则按GUID升序排列
                result = a.PetGuid.CompareTo(b.PetGuid);
            }
        }
        return result;
    }

    private void UpdateUnlockPetPos()
    {
        RefreshUpLoadPets();
    }
    
    private void UpdatePetLevelInfo()
    {
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        _petBattleUI.petAllList.numItems = _petItemInfos.Count;

        _petBattleUI.limit.text = $"{_petItemInfos.Count}/{ItemInfoManager.Instance.common300008.Param1}";

        _petBattleUI.recycleBtnRed.visible = _petItemInfos.Count >= int.Parse(_common3004.Param1);

        bool isCanLevelUp = false;
        foreach (var item in PetInfoManager.Instance.GetAllHavePetList())
        {
            ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(item.quality, item.PetLv+1);
            if (petItem != null)
            {
                // if (petItem.Item1.CardNumber <= item.CardNumber)
                // {
                    isCanLevelUp = true;
                    break;
                // }
            }
        }
    
        // this.petUI.petBattle.allStrengthBtn.enabled = isCanLevelUp;
    
        RefrehUpLoadItem();
    }
    
    private void RefrehUpLoadItem()
    {
        GameManager.Instance.TimerManager.ClearTimer(RefreshUpLoadPets);
        GameManager.Instance.TimerManager.SetTimer(0.1f, RefreshUpLoadPets);
        double atkadd = 0;
        double hpadd = 0;
        /*
        foreach (var item in PetInfoManager.Instance.GetAllHavePetList())
        {
            foreach (var attr in item.OwnerAttrs)
            {
                if (attr.AttrId == (int) EN_BUFF_ADD_TYPE.PetAtkADD)
                {
                    atkadd += attr.AttrVal;
                }else if (attr.AttrId == (int) EN_BUFF_ADD_TYPE.HP_ADD)
                {
                    hpadd += attr.AttrVal;
                }
            }
        }
        
        foreach (var item in PetInfoManager.Instance.GetBattlePetList())
        {
            foreach (var attr in item.CarryAttrs)
            {
                if (attr.AttrId == (int) EN_BUFF_ADD_TYPE.PetAtkADD)
                {
                    atkadd += attr.AttrVal;
                }else if (attr.AttrId == (int) EN_BUFF_ADD_TYPE.HP_ADD)
                {
                    hpadd += attr.AttrVal;
                }
            }
        }
        // this.petUI.petBattle.hpLb.SetVar("value", StringUtils.FormatCurrency(hpadd * ConstDefine.CONFIG_PLACE)).FlushVars();
        // this.petUI.petBattle.atkLb.SetVar("value", StringUtils.FormatCurrency(atkadd * ConstDefine.CONFIG_PLACE)).FlushVars();
        */
    }
    
    private void RefreshUpLoadPets()
    {
        foreach (var item in _petUpLoadItemDict)
        {
            if (_upLoadPetDict.TryGetValue(item.Key, out var pet))
            {
                this.SetUpLoadPetItemData(item.Value, pet, item.Key);
            }
            else
            {
                Utils.ClearSpineModelOnFGUI(item.Value.spine);
                var stateMap =  PetInfoManager.Instance.UnLockPetPos >= (item.Key+1);
                if (!stateMap)
                {
                    item.Value.state.selectedIndex = 2;
                }
                else
                {
                    item.Value.state.selectedIndex = 0;
                }
            }
    
        }
        
        //红点
        // this.petUI.petBattle.redDot.visible = PetInfoManager.Instance.IsCanUpLevel();
        RefreshBottomRedDot();
    }
    
    private void PetUpListRender(int index, GObject item)
    {
        UI_PetUpLoadItem btn = (UI_PetUpLoadItem) item;
        if (this._upLoadPetDict.TryGetValue(index, out var pet))
        {
            SetUpLoadPetItemData(btn, pet, index);
        }
    }
    
    private void SetUpLoadPetItemData(UI_PetUpLoadItem item,PetItemInfo pet, int index)
    {
        pet.BattleIndex = index;
        item.state.selectedIndex = 1;
        ((UI_PetQualityLb)item.petQuality).qualityCtrl.selectedIndex = pet.petCfg.Quality - 1;
        item.petName.text = ConfigUtils.GetTextById(pet.petCfg.Name);
        Utils.SetSpineModelOnFGUI(item.spine, pet.petCfg.PetModel, index>2 ?75 : 90, "idle");
    }
    
    private void HideUploadPetView()
    {
        // this.roleUI.showPetHand.selectedIndex = 0;
        foreach (var item in _petUpLoadItemDict)
        {
            item.Value.showHandCtrl.selectedIndex = 0;
        }
    }
    
    private void ShowUploadPetOpt(PetItemInfo pet)
    {
        _uploadPet = pet;
        // this.roleUI.showPetHand.selectedIndex = 1;
        // this.roleUI.hideUpload1.y = _petInfoUI.rlLoader.y;
        // this.roleUI.hideUpload2.height = _petInfoUI.upGroup.y;
        foreach (var item in _petUpLoadItemDict)
        {
            item.Value.showHandCtrl.selectedIndex = 1;
        }
    }
    
    
	/// <summary>
    /// 宠物上阵下阵通知
    /// </summary>
	private void UpdatePetSCSuccsss()
    {
        HideUploadPetView();
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        _petBattleUI.petAllList.numItems = _petItemInfos.Count;
        _petBattleUI.limit.text = $"{_petItemInfos.Count}/{ItemInfoManager.Instance.common300008.Param1}";
        _petBattleUI.recycleBtnRed.visible = _petItemInfos.Count >= int.Parse(_common3004.Param1);
        RefrehUpLoadItem();
    }

    private void OnClickPetRecycleBtn()
    {
        UIManager.Instance.ShowUIPanel("PetRecycle",0);
    }

    #endregion
    
    #region 天赋界面
    private List<PetItemInfo> battlePetsInTalent =  new List<PetItemInfo>();
    private void UpdateTalentUI()
    {
        Debug.Log("===天赋界面===");

        totalCost = 0;
        _selectTalentIdList.Clear();
        TalentCostToolsHandler();
        
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();

        if (_upLoadPetDict.Count > 0)
        {
            battlePetsInTalent.Clear();
            foreach (var data in _upLoadPetDict)
            {
                battlePetsInTalent.Add(data.Value);
            }
            
            _petTalentUI.hasUpload.selectedIndex = 1;
            // this.petUI.petTalent.list.numItems = _upLoadPetDict.Count;
            _petTalentUI.list.numItems = battlePetsInTalent.Count;
            
            // 默认选中第一个宠物
            _petTalentUI.list.selectedIndex = 0; // 设置列表选中索引
            // var firstPet = _upLoadPetDict[0];

            for (int i = 0; i < 3; i++)
            {
                if (_upLoadPetDict.TryGetValue(i, out PetItemInfo firstPet))
                {
                    _talentIdList = firstPet.talentsList; // 获取第一个宠物的天赋列表
                    // _talentIdList = selectedPetTalentIds; // 获取第一个宠物的天赋列表
                    _selectedPetGuid = firstPet.PetGuid; // 选中宠物GUID
                    break;
                }
            }
            
        }
        else
        {
            _petTalentUI.hasUpload.selectedIndex = 0;
        }

        _petTalentUI.currency1.icon = UIResource.GetItemUrl(_common300010.Param1.Split(',')[0]);//洗练天赋道具
        ((UI_Currency1)_petTalentUI.currency1).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(int.Parse(_common300010.Param1.Split(',')[0])));
        _petTalentUI.currency2.icon = UIResource.GetItemUrl(costToolId.ToString());//锁定天赋道具
        ((UI_Currency1)_petTalentUI.currency2).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(costToolId));
        
        ((UI_BtnCom2)_petTalentUI.talentBtn).itemIcon.url = UIResource.GetItemUrl(_common300010.Param1.Split(',')[0]);//洗练天赋消耗道具图标
        ((UI_BtnCom2)_petTalentUI.talentBtn).num.text = _common300010.Param1.Split(',')[1];
        
        // UpdateTalentInfo();
        if (_upLoadPetDict.Count > 0)
        {
            UpdateTalentInfo();
        }
        else
        {
            _talentIdList.Clear();
            _petTalentUI.talentList.numItems = _talentIdList.Count;
        }
    }
    
    private Dictionary<int, HashSet<int>> _lockedTalentPositions = new Dictionary<int, HashSet<int>>();
    private void UpdateTalentInfo()
    {
        ((UI_Currency1)_petTalentUI.currency1).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(int.Parse(_common300010.Param1.Split(',')[0])));
        ((UI_Currency1)_petTalentUI.currency2).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(costToolId));
        if (ItemInfoManager.Instance.GetItemCount(int.Parse(_common300010.Param1.Split(',')[0])) < int.Parse(_common300010.Param1.Split(',')[1]))
        {
            _petTalentUI.status.selectedIndex = 1;
        }
        else
        {
            _petTalentUI.status.selectedIndex = 0;
        }
        
        // 获取服务端最新天赋列表
        // List<int> selectedPetTalentIds = new List<int>();
        // foreach (var talent in PetInfoManager.Instance.GetPet(_selectedPetGuid).talentsList)
        // {
        //     selectedPetTalentIds.Add(talent.talentId);
        // }
        // var rawTalents = selectedPetTalentIds;
        var rawTalents = PetInfoManager.Instance.GetPet(_selectedPetGuid).talentsList;
        List<int> lockTalentIds = PetInfoManager.Instance.GetLockTalentIds();
        
        // 重建有序列表
        // List<int> orderedList = new List<int>(new int[rawTalents.Count]);
        // List<PetTalent> orderedList = new List<PetTalent>(new PetTalent[rawTalents.Count]);
        List<PetTalent> orderedList = new List<PetTalent>();
        for (int i = 0; i < rawTalents.Count; i++)
        {
            List<PetBattleAttr> attrList = new List<PetBattleAttr>();
            attrList.Add(new PetBattleAttr()
            {
                AttrId = 0,
                AttrVal = 0
            });
            
            orderedList.Add(new PetTalent()
            {
                talentId = 0,
                BattleAttrs = attrList,
                ObjType = 0
            }); // 确保每个位置都有对象实例
        }

        // 填充已锁定项到其原始位置
        foreach (var kvp in _lockedTalentPositions)
        {
            int talentId = kvp.Key;
            foreach (int lockedPos in kvp.Value)
            {
                if (lockedPos < orderedList.Count)
                {
                    // orderedList[lockedPos] = talentId;
                    orderedList[lockedPos].talentId = talentId;
                    orderedList[lockedPos].BattleAttrs = _talentIdList[lockedPos].BattleAttrs;
                    orderedList[lockedPos].ObjType = _talentIdList[lockedPos].ObjType;
                }
            }
        }
        
        // 填充未锁定项到空白位置
        int serverIndex = 0;
        for (int i = 0; i < orderedList.Count; i++)
        {
            if (orderedList[i].talentId == 0) // 空白位置 // orderedList[i] == 0     orderedList[i].talentId == 0
            {
                while (serverIndex < rawTalents.Count)
                {
                    // int currentTalent = rawTalents[serverIndex++];
                    var currentTalent = rawTalents[serverIndex];
                    if (!lockTalentIds.Contains(currentTalent.talentId) || 
                        !IsPositionLocked(currentTalent.talentId, serverIndex))
                    {
                        // orderedList[i] = currentTalent;
                        orderedList[i] = currentTalent;
                        serverIndex++;
                        break;
                    }
                    serverIndex++; // 跳过已锁定项
                }
            }
        }
        
        _talentIdList = orderedList;

        _petTalentUI.talentList.numItems = _talentIdList.Count;

        _petTalentUI.talentRedPoint.visible = ItemInfoManager.Instance.GetItemCount(int.Parse(_common300010.Param1.Split(',')[0])) >= int.Parse(_common3003.Param1);
        _petTalentUI.list.numItems = battlePetsInTalent.Count;
        RefreshBottomRedDot();
    }
    
    private bool IsPositionLocked(int talentId, int serverPosition)
    {
        // 检查该天赋位置是否被客户端锁定
        return _lockedTalentPositions.TryGetValue(talentId, out var positions) 
               && positions.Contains(serverPosition);
    }

    private void OnClickTalentTipsBtn()
    {
        UIManager.Instance.ShowUIPanel("Help",HelpType.Help_PetTalent);
    }
    
    private void TalentListRender(int index, GObject item)
    {
        UI_TalentItem talentItem = (UI_TalentItem)item;
        // var unit = ConfigUtils.GetPetAptitudeUnitById(_talentIdList[index]);
        var unit = ConfigUtils.GetPetAptitudeUnitById(_talentIdList[index].talentId);
        talentItem.talentName.text = ConfigUtils.GetTextById(unit.Name);
        
        List<PetBattleAttr> list = new List<PetBattleAttr>();
        list = _talentIdList[index].BattleAttrs;
        
        // string lastValue = EquipManager.Instance.SetAttributeValue(attrId, value, true);
        string lastValue = EquipManager.Instance.SetAttributeValue(list[0].AttrId, list[0].AttrVal, true);
        talentItem.talentdesc.text = StringUtils.Format(ConfigUtils.GetTextById(unit.Doc), lastValue);
        talentItem.petTalentItem.qualityIcon.visible = true;

        // 选中的index
        bool isSelected = _selectTalentIdList.Contains(index);
        int selectedCount = _selectTalentIdList.Count;

        // 第一个未选中项的索引（按列表顺序）
        int firstUnselectedIndex = -1;
        for (int i = 0; i < _talentIdList.Count; i++)
        {
            if (!_selectTalentIdList.Contains(i)) //检查索引
            {
                firstUnselectedIndex = i;
                break;
            }
        }

        if (isSelected)
        {
            // 获取当前选中项在列表中的顺序
            int selectedOrder = _selectTalentIdList.IndexOf(index) + 1; // 从1开始
            talentItem.goldNum.text = itemDict[selectedOrder].ToString();
            talentItem.isShow.selectedIndex = 0; // 显示goldNum
            talentItem.isCheck.selectedIndex = 1;
        }
        else
        {
            talentItem.isCheck.selectedIndex = 0;
            if (selectedCount < 3)
            {
                // 如果是第一个未选中项，显示下一级消耗
                if (index == firstUnselectedIndex)
                {
                    int nextOrder = selectedCount + 1;
                    talentItem.goldNum.text = itemDict.ContainsKey(nextOrder) ? itemDict[nextOrder].ToString() : "";
                    talentItem.isShow.selectedIndex = 0;
                }
                else
                {
                    talentItem.isShow.selectedIndex = 1; // 隐藏其他未选中项
                }
            }
            else
            {
                talentItem.isShow.selectedIndex = 1; // 满选时隐藏
            }
        }

        // 初始状态:无选中时第一个项显示
        if (selectedCount == 0 && index == 0)
        {
            talentItem.goldNum.text = itemDict[1].ToString();
            talentItem.isShow.selectedIndex = 0;
        }

        talentItem.petTalentItem.lockCtrl.selectedIndex = 0;
        talentItem.petTalentItem.qualityCtrl.selectedIndex = unit.Quality - 1;
        // talentItem.petTalentItem.icon = UIResource.GetItemUrl(unit.Item.ToString());
        talentItem.petTalentItem.icon = UIResource.GetPetTalentIcon(unit.Item.ToString());
        talentItem.petTalentItem.talentName.visible = false;

        talentItem.itemIcon.url = UIResource.GetItemUrl(costToolId.ToString());

        // 存储列表索引
        talentItem.checkBtn.data = index;
        talentItem.checkBtn.onClick.Add(OnClickCheckBox);
    }

    private void OnClickCheckBox(EventContext context)
    {
        GButton checkBtn = (GButton)context.sender;
        UI_TalentItem item = (UI_TalentItem)checkBtn.parent;
        int clickedIndex = (int)checkBtn.data; // 获取选中索引

        bool wasSelected = _selectTalentIdList.Contains(clickedIndex);
        
        int talentId = _talentIdList[clickedIndex].talentId;
        int serverPos = -1;
        
        if (!wasSelected)
        {
            if (_selectTalentIdList.Count >= int.Parse(_common300009.Param2))
            {
                // 提示锁定上限
                UIManager.Instance.ToastByKey(8054);
                return;
            }
            
            // 计算总消耗并检查道具
            // int totalCost = Enumerable.Range(1, _selectTalentIdList.Count + 1).Sum(i => itemDict[i]);
            totalCost = Enumerable.Range(1, _selectTalentIdList.Count + 1).Sum(i => itemDict[i]);
            // costNum = totalCost;
            if (ItemInfoManager.Instance.GetItemCount(costToolId) < totalCost)
            {
                totalCost = Enumerable.Range(1, _selectTalentIdList.Count).Sum(i => itemDict[i]);
                // 提示道具不足
                UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038,ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(costToolId).Name)));
                return;
            }
            
            _selectTalentIdList.Add(clickedIndex);

            // List<int> rawTalents = _talentIdList;//PetInfoManager.Instance.GetPet(_selectedPetGuid).talentsList;
            List<int> list = new List<int>();
            foreach (var i in _talentIdList)
            {
                list.Add(i.talentId);
            }
            List<int> rawTalents = list;
            // 查找所有匹配位置
            List<int> allPositions = new List<int>();
            for (int i = 0; i < rawTalents.Count; i++)
            {
                if (rawTalents[i] == talentId)
                    allPositions.Add(i);
            }
            
            // 确定具体是哪个位置的ID被锁定（根据客户端点击的索引映射）
            if (allPositions.Count > 0)
                serverPos = allPositions[0]; 

            // 记录位置
            if (serverPos != -1)
            {
                if (!_lockedTalentPositions.TryGetValue(talentId, out var positions))
                {
                    positions = new HashSet<int>();
                    _lockedTalentPositions[talentId] = positions;
                }
                positions.Add(serverPos);
            }
        }
        else
        {
            _selectTalentIdList.Remove(clickedIndex);
            
            totalCost = Enumerable.Range(1, _selectTalentIdList.Count).Sum(i => itemDict[i]);
            if (_selectTalentIdList.Count <= 0)
            {
                totalCost = 0;
            }
            
            // 需要先获取serverPos再移除
            // 查找已记录的锁定位置
            if (_lockedTalentPositions.TryGetValue(talentId, out var positions))
            {
                if (positions.Count > 0)
                {
                    serverPos = positions.First();
                    positions.Remove(serverPos);
                    if (positions.Count == 0)
                        _lockedTalentPositions.Remove(talentId);
                }
            }
        }

        // 刷新列表更新所有项的显示
        _petTalentUI.talentList.numItems = _talentIdList.Count;
        // UpdateTalentInfo();
    }
    
    private void TalentCostToolsHandler()
    {
        string[] itemStr = _common300009.Param1.Split('|');
        foreach (var item in itemStr)
        {
            string[] s = item.Split(',');
            itemDict[int.Parse(s[0])] = int.Parse(s[2]);
            costToolId = int.Parse(s[1]);
        }
    }

    private void PetTalentBattleRender(int index, GObject item)
    {
        UI_PetItem2 petItem = item as UI_PetItem2;
        
        PetItemInfo petInfo = battlePetsInTalent[index];
        ConfigPetBasisUnit petBasisUnit = petInfo.petCfg;
        // petItem.icon = UIResource.GetPetIcon(petBasisUnit.IconPath);
        petItem.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petBasisUnit.IconPath)).Icon);
        petItem.qualityCtrl.selectedIndex = petInfo.petCfg.Quality - 1;
        // petItem.petLv.text = "";
        petItem.data = petInfo;
        petItem.onClick.Set(this.OnClickTalentPetItem);
        
        petItem.redPoint.visible = ItemInfoManager.Instance.GetItemCount(int.Parse(_common300010.Param1.Split(',')[0])) >= int.Parse(_common3003.Param1);
    }
    
    private void OnClickTalentPetItem(EventContext context)
    {
        _lockedTalentPositions.Clear();
        _selectTalentIdList.Clear();
        
        var selectedPetItem = context.sender as UI_PetItem2;
        
        if (selectedPetItem?.data is not PetItemInfo selectedPetInfo)
        {
            Debug.LogError("无效的宠物数据");
            return;
        }

        foreach (var item in _petTalentUI.list.GetChildren())
        {
            UI_PetItem2 petItem = (UI_PetItem2)item;
            petItem.selected = (item == selectedPetItem);
        }
        
        _talentIdList = selectedPetInfo.talentsList;
        _selectedPetGuid = selectedPetInfo.PetGuid;

        totalCost = 0;
        _petTalentUI.talentList.numItems = _talentIdList.Count;
    }
    
    
    private void OnClickResetTalentBtn()
    {
        // 当材料不足时提示
        if (ItemInfoManager.Instance.GetItemCount(int.Parse(_common300010.Param1.Split(',')[0])) < int.Parse(_common300010.Param1.Split(',')[1]))
        {
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_common300010.Param1.Split(',')[0])).Name)));
            return;
        }

        if (ItemInfoManager.Instance.GetItemCount(costToolId) < totalCost)
        {
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038,ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(costToolId).Name)));
            return;
        }

        var builder = PetShuffleTalent_CS.CreateBuilder();
        builder.PetId = _selectedPetGuid;

        _petTalentUI.talentBtn.grayed = true;
        _petTalentUI.talentBtn.touchable = false;

        int animCount = 0; // 播放的动画数量
        bool requestSent = false; // 确保请求只发送一次

        // 遍历所有天赋项的索引
        for (int index = 0; index < _talentIdList.Count; index++)
        {
            // 跳过已锁定的天赋项
            if (_selectTalentIdList.Contains(index)) continue;

            var item = _petTalentUI.talentList.GetChildAt(index);
            UI_TalentItem talentItem = (UI_TalentItem)item;

            animCount = 1;
            talentItem.spine.visible = true;
            // talentItem.spine.timeScale = 2f;//两倍速度播放
            Utils.PlaySpineAnim2(talentItem.spine, "Pet_xltf", false, 1.8f, () =>
            {
                
                talentItem.spine.visible = false;
                animCount--; // 动画完成，减少计数

                // 所有spine播放完且未发送请求时发送请求
                if (animCount == 0 && !requestSent)
                {
                    requestSent = true;
                    SendShuffleRequest(builder);
                }
            });
        }

        // 如果没有需要播放的spine，立即发送请求
        if (animCount == 0 && !requestSent)
        {
            requestSent = true;
            SendShuffleRequest(builder);
        }
    }

    private void SendShuffleRequest(PetShuffleTalent_CS.Builder builder)
    {
        // 添加锁定的天赋ID
        if (_selectTalentIdList.Count > 0)
        {
            foreach (var selectedIndex in _selectTalentIdList)
            {
                // uint talentId = (uint)_talentIdList[selectedIndex];
                uint talentId = (uint)_talentIdList[selectedIndex].talentId;
                builder.AddLockedTalentIds(talentId);
            }
        }
        
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_PetShuffleTalent_CS, builder.Build());

        // _selectTalentIdList.Clear(); // 清除已选天赋
        
        _petTalentUI.talentBtn.grayed = false;
        _petTalentUI.talentBtn.touchable = true;
        
    }


    #endregion
    
    #region 宠物书界面
    private int selectBookPetIndex = 0;
    private List<PetItemInfo> battlePetsInBook = new List<PetItemInfo>();
    private void UpdateBookUI()
    {
        Debug.Log("===宠物书界面===");

        PetInfoManager.Instance.SendPetSkillBookInfo_CS();

        _petBookUI.currency1.icon = UIResource.GetItemUrl(_common300006.Param2.Split(',')[0]);
        ((UI_Currency1)_petBookUI.currency1).txtValue.text = ItemInfoManager.Instance.GetItemCount(int.Parse(_common300006.Param2.Split(',')[0])).ToString();
        ((UI_Currency1)_petBookUI.currency2).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.gold);

        if (_upLoadPetDict.Count > 0)
        {
            battlePetsInBook.Clear();
            foreach (var data in _upLoadPetDict)
            {
                battlePetsInBook.Add(data.Value);
            }
            
            _petBookUI.hasUpload.selectedIndex = 1;
            // this.petUI.petBook.petList.numItems = _upLoadPetDict.Count;
            _petBookUI.petList.numItems = battlePetsInBook.Count;
            
            // 默认选中第一个宠物
            _petBookUI.petList.selectedIndex = selectBookPetIndex;
            // var firstPet = _upLoadPetDict[selectBookPetIndex];
            PetItemInfo firstPet;

            if (_upLoadPetDict.ContainsValue(battlePetsInBook[selectBookPetIndex]))
            {
                // firstPet = _upLoadPetDict[selectBookPetIndex];
                firstPet = battlePetsInBook[selectBookPetIndex];
                _skillBookSlotList = firstPet.bookSlotsList;
                _unlockSlotPetGuid = firstPet.PetGuid; // 选中宠物GUID
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_upLoadPetDict.TryGetValue(i, out PetItemInfo firstPet2))
                    {
                        _skillBookSlotList = firstPet2.bookSlotsList;
                        _unlockSlotPetGuid = firstPet2.PetGuid; // 选中宠物GUID
                        break;
                    }
                }
            }
            
            // _skillBookSlotList = firstPet.bookSlotsList;
            // _unlockSlotPetGuid = firstPet.PetGuid; // 选中宠物GUID
            
            // 设置默认选中第一个技能书槽位
            _petBookUI.skillBookList.selectedIndex = 0;
            var firstBookSlotInfo = _skillBookSlotList[0];
            if (firstBookSlotInfo.BookId == 0)
            {
                _petBookUI.status.selectedIndex = 1;
            }
            else
            {
                _petBookUI.status.selectedIndex = 0;
                var unit = ConfigUtils.GetPetSkillBookUnitById(firstBookSlotInfo.BookId);
                // this.petUI.petBook.petSkillBtn.icon = UIResource.GetItemUrl(unit.Icon.ToString());
                _petBookUI.petSkillBtn.icon = UIResource.GetPetSkillBookIcon(unit.Icon.ToString());
                _petBookUI.petSkillBtn.qualityCtrl.selectedIndex = unit.Quality - 1;
                _petBookUI.name.text = ConfigUtils.GetTextById(unit.Name.ToString());
                // var skillAchieveUnit = ConfigUtils.GetSkillAchieveById(unit.Id);
                // this.petUI.petBook.desc.text = StringUtils.Format(skillAchieveUnit.Doc,skillAchieveUnit.LastTime);//ConfigUtils.GetTextById(skillAchieveUnit.Doc)
                if (unit.DocCommon.Equals("0"))//为0时服务器下发
                {
                    PetBattleAttr attr = firstBookSlotInfo.battleAttrList[0];
                    string value = (attr.AttrVal/100).ToString("f2");
                    _petBookUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(unit.Doc),value);
                }
                else
                {
                    //读取配置表
                    double value = double.Parse(unit.DocCommon)/100;
                    _petBookUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(unit.Doc),value);
                }
            }

        }
        else
        {
            _unlockSlotPetGuid = 0;
            _skillBookSlotList.Clear();
            _petBookUI.hasUpload.selectedIndex = 0;
            _petBookUI.status.selectedIndex = 1;
        }
        
        UpdateBookSlot();
        
        // ((UI_BtnCom2)_petBookUI.makeBtn).itemIcon.url = UIResource.GetItemUrl(_common300006.Param2.Split(',')[0]);
        // ((UI_BtnCom2)_petBookUI.makeBtn).num.text = _common300006.Param2.Split(',')[1];
        
        ((UI_MakeBtn)_petBookUI.makeBtnAni.makeBtn).itemIcon.url = UIResource.GetItemUrl(_common300006.Param2.Split(',')[0]);
        ((UI_MakeBtn)_petBookUI.makeBtnAni.makeBtn).num.text = _common300006.Param2.Split(',')[1];
        
        UpdateSKillBookInPackage();
        
        // 所有槽位都解锁，隐藏开槽按钮
        VisiableMakeBtn();
        _petBookUI.makeBtnRedPoint.visible = ItemInfoManager.Instance.GetItemCount(int.Parse(_common300006.Param2.Split(',')[0])) >= int.Parse(_common300006.Param2.Split(',')[1]);

        //打书按钮状态设置
        _selectedSkillBookId = -1;
        _petBookUI.openBookBtn.touchable = true;
        _petBookUI.openBookStatus.selectedIndex = 1;
        _petBookUI.openType.selectedIndex = 0;
        GetHasEmptySlotUpLoadPetIndex();
        GameManager.Instance.TimerManager.SetTimer(0.1f, () =>
        {
            bool hasHighQualityBook = PetInfoManager.Instance.GetPackageHighPetBookQuality() > PetInfoManager.Instance.GetPetLowQualityBook(battlePetsInBook[selectBookPetIndex]);
            _petBookUI.openBookBtnRedPoint.visible = (PetInfoManager.Instance.PetSkillBookEmptySlotAndBookRed() && indexList.Contains(selectBookPetIndex)) || hasHighQualityBook;
        });

    }

    // 控制开槽按钮显示和颜色
    private void VisiableMakeBtn()
    {
        PetItemInfo pet = PetInfoManager.Instance.GetPet(_unlockSlotPetGuid);
        
        if (pet == null)
            return;

        // 该宠物所有槽位都解锁，隐藏开槽按钮
        bool allSlotsUnlocked = true;
        foreach (var bookSlot in pet.bookSlotsList)
        {
            if (bookSlot.SlotStatus == eSlotStatus.eSlotStatus_Locked)
            {
                allSlotsUnlocked = false;
                break;
            }
        }
        if (allSlotsUnlocked)
        {
            _petBookUI.type.selectedIndex = 1;
        }
        else
        {
            _petBookUI.type.selectedIndex = 0;
        }
    }
    
    private void PetSkillBookBattleRender(int index, GObject item)
    {
        UI_PetItem2 petItem = item as UI_PetItem2;
        PetItemInfo petInfo = battlePetsInBook[index];
        ConfigPetBasisUnit petBasisUnit = petInfo.petCfg;
        // petItem.icon = UIResource.GetPetIcon(petBasisUnit.IconPath);
        petItem.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petBasisUnit.IconPath)).Icon);
        petItem.qualityCtrl.selectedIndex = petInfo.petCfg.Quality - 1;
        // petItem.petLv.text = "";
        petItem.data = petInfo;
        petItem.onClick.Set(this.OnClickBookPetItem);

        // 红点
        GameManager.Instance.TimerManager.SetTimer(0.1f, () => {
            // 是否可开槽
            bool makeItem = ItemInfoManager.Instance.GetItemCount(int.Parse(_common300006.Param2.Split(',')[0])) >= int.Parse(_common300006.Param2.Split(',')[1]);
            bool canMake = PetInfoManager.Instance.HasLockedSkillBookSlot(petInfo) && makeItem;
            
            // 有空位，背包有技能
            bool hasEmptySlotAndBook = PetInfoManager.Instance.HasEmptySkillBookSlotAndBook(petInfo);
            
            // 背包中有更高品质的书
            bool hasHighQualityBook = PetInfoManager.Instance.GetPackageHighPetBookQuality() > PetInfoManager.Instance.GetPetLowQualityBook(petInfo);
        
            petItem.redPoint.visible = canMake || hasEmptySlotAndBook || hasHighQualityBook;
        });
        
    }
    
    private void OnClickBookPetItem(EventContext context)
    {
        var selectedPetItem = context.sender as UI_PetItem2;
        if (selectedPetItem?.data is not PetItemInfo selectedPetInfo)
        {
            Debug.LogError("无效的宠物数据");
            return;
        }

        for (int i = 0; i < _petBookUI.petList.GetChildren().Length; i++)
        {
            UI_PetItem2 petItem = _petBookUI.petList.GetChildren()[i] as UI_PetItem2;
            if (petItem == selectedPetItem)
            {
                petItem.selected = true;
                selectBookPetIndex = i;
            }
            else
                petItem.selected = false;
        }

        _petBookUI.skillBookList.selectedIndex = 0;
        _skillBookSlotList = selectedPetInfo.bookSlotsList;
        _unlockSlotPetGuid = selectedPetInfo.PetGuid;
        _petBookUI.skillBookList.numItems = _skillBookSlotList.Count;

        var firstBookSlotInfo = _skillBookSlotList[0];
        if (firstBookSlotInfo.BookId == 0)
        {
            _petBookUI.status.selectedIndex = 1;
        }
        else
        {
            _petBookUI.status.selectedIndex = 0;
            var unit = ConfigUtils.GetPetSkillBookUnitById(firstBookSlotInfo.BookId);
            // this.petUI.petBook.petSkillBtn.icon = UIResource.GetItemUrl(unit.Icon.ToString());
            _petBookUI.petSkillBtn.icon = UIResource.GetPetSkillBookIcon(unit.Icon.ToString());
            _petBookUI.petSkillBtn.qualityCtrl.selectedIndex = unit.Quality - 1;
            _petBookUI.name.text = ConfigUtils.GetTextById(unit.Name.ToString());
            // this.petUI.petBook.desc.text = StringUtils.Format(ConfigUtils.GetTextById(unit.Doc),unit.Time);
            var skillAchieveUnit = ConfigUtils.GetSkillAchieveById(unit.Id);
            _petBookUI.desc.text = StringUtils.Format(skillAchieveUnit.Doc,skillAchieveUnit.LastTime);//ConfigUtils.GetTextById(skillAchieveUnit.Doc)
        }
        
        // 所有槽位都解锁，隐藏开槽按钮
        _unlockSlotId = 1;
        VisiableMakeBtn();
        
        bool hasHighQualityBook = PetInfoManager.Instance.GetPackageHighPetBookQuality() > PetInfoManager.Instance.GetPetLowQualityBook(battlePetsInBook[selectBookPetIndex]);
        _petBookUI.openBookBtnRedPoint.visible = (PetInfoManager.Instance.PetSkillBookEmptySlotAndBookRed() && indexList.Contains(selectBookPetIndex)) || hasHighQualityBook;
        _petBookUI.list.numItems = packageItemNum;
    }

    private void UpdateBookSlot()
    {
        ((UI_Currency1)_petBookUI.currency1).txtValue.text = ItemInfoManager.Instance.GetItemCount(int.Parse(_common300006.Param2.Split(',')[0])).ToString();
        ((UI_Currency1)_petBookUI.currency2).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.gold);
        
        if (ItemInfoManager.Instance.GetItemCount(int.Parse(_common300006.Param2.Split(',')[0])) < int.Parse(_common300006.Param2.Split(",")[1]))
        {
            _petBookUI.makeStatus.selectedIndex = 1;
            _petBookUI.makeBtnRedPoint.visible = false;
        }
        else
        {
            _petBookUI.makeStatus.selectedIndex = 0;
            _petBookUI.makeBtnRedPoint.visible = true;
        }
        
        PetItemInfo pet = PetInfoManager.Instance.GetPet(_unlockSlotPetGuid);
        if (pet == null)
        {
            
            _petBookUI.skillBookList.numItems = 5;
        }
        else
        {
            _petBookUI.skillBookList.numItems = pet.bookSlotsList.Count;
        }
        
        // 所有槽位都解锁，隐藏开槽按钮
        VisiableMakeBtn();

        GetHasEmptySlotUpLoadPetIndex();
        
        bool hasHighQualityBook = PetInfoManager.Instance.GetPackageHighPetBookQuality() > PetInfoManager.Instance.GetPetLowQualityBook(battlePetsInBook[selectBookPetIndex]);
        _petBookUI.openBookBtnRedPoint.visible = (PetInfoManager.Instance.PetSkillBookEmptySlotAndBookRed() && indexList.Contains(selectBookPetIndex)) || hasHighQualityBook;

        _petBookUI.petList.numItems = PetInfoManager.Instance.GetBattlePetList().Count;
    }

    // 宠物技能书槽位
    private void PetBookSlotListRender(int index, GObject item)
    {
        if (_skillBookSlotList.Count == 0)
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 1;
            return;
        }
        
        PetSkillBookSlot petSkillBookSlot = _skillBookSlotList[index];
        
        // 该槽位未解锁
        if (petSkillBookSlot.SlotStatus == eSlotStatus.eSlotStatus_Locked)
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 1;
        }

        // 该槽位解锁且有书
        if (petSkillBookSlot.SlotStatus == eSlotStatus.eSlotStatus_Normal && petSkillBookSlot.BookId != 0)
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 2;
            ((UI_PetBookItem)item).qualityIcon.visible = true;
            
            var skillBookUnit = ConfigUtils.GetPetSkillBookUnitById(petSkillBookSlot.BookId);
            // ((UI_PetBookItem)item).icon = UIResource.GetItemUrl(skillBookUnit.Icon.ToString());//技能书图标
            ((UI_PetBookItem)item).icon = UIResource.GetPetSkillBookIcon(skillBookUnit.Icon.ToString());//技能书图标
            ((UI_PetBookItem)item).qualityCtrl.selectedIndex = skillBookUnit.Quality - 1;
        }
        
        // 该槽位解锁且没有书
        if (petSkillBookSlot.SlotStatus == eSlotStatus.eSlotStatus_Normal && petSkillBookSlot.BookId == 0)
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 0;
        }

        ((UI_PetBookItem)item).data = petSkillBookSlot;
        ((UI_PetBookItem)item).onClick.Add(OnClickBookSlotItem);
        
    }

    private void OnClickBookSlotItem(EventContext context)
    {
        PetSkillBookSlot bookSlotInfo = (context.sender as UI_PetBookItem)?.data as PetSkillBookSlot;

        // 解锁没有书
        if (bookSlotInfo.BookId == 0 && bookSlotInfo.SlotStatus == eSlotStatus.eSlotStatus_Normal)
        {
            return;
        }
        
        // 未解锁
        if (bookSlotInfo.SlotStatus == eSlotStatus.eSlotStatus_Locked)
        {
            _unlockSlotId = bookSlotInfo.SlotId;
            UIManager.Instance.ToastByKey(8053);
            return;
        }
        
        var unit = ConfigUtils.GetPetSkillBookUnitById(bookSlotInfo.BookId);
        // this.petUI.petBook.petSkillBtn.icon = UIResource.GetItemUrl(unit.Icon.ToString());
        _petBookUI.petSkillBtn.icon = UIResource.GetPetSkillBookIcon(unit.Icon.ToString());
        _petBookUI.name.text = ConfigUtils.GetTextById(unit.Name.ToString());
        _petBookUI.petSkillBtn.qualityCtrl.selectedIndex = unit.Quality - 1;
        // var skillAchieveUnit = ConfigUtils.GetSkillAchieveById(unit.Id);
        // this.petUI.petBook.desc.text = StringUtils.Format(skillAchieveUnit.Doc,skillAchieveUnit.LastTime);//ConfigUtils.GetTextById(skillAchieveUnit.Doc)

        if (unit.DocCommon.Equals("0"))//为0时服务器下发
        {
            PetBattleAttr petBattleAttr = new PetBattleAttr();
            petBattleAttr = bookSlotInfo.battleAttrList[0];
            string value = (petBattleAttr.AttrVal/100).ToString("f2");
            _petBookUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(unit.Doc),value);
        }
        else
        {
            //读取配置表
            double value = double.Parse(unit.DocCommon)/100;
            _petBookUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(unit.Doc),value);
        }
        
        _unlockSlotId = bookSlotInfo.SlotId;
    }

    private int packageItemNum = 0;
    private void UpdateSKillBookInPackage()
    {
        _skillBookItemList.Clear();
        ItemData skillbookItemData1 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.UNCOMMON);
        ItemData skillbookItemData2 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.EPIC);
        ItemData skillbookItemData3 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.LEGEND);
        ItemData skillbookItemData4 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.MYTH);
        ItemData skillbookItemData5 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.IMMORTAL);
        ItemData skillbookItemData6 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.RANDOM);
        
        if (skillbookItemData6.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData6);
        }
        if (skillbookItemData5.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData5);
        }
        if (skillbookItemData4.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData4);
        }
        if (skillbookItemData3.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData3);
        }
        if (skillbookItemData2.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData2);
        }
        if (skillbookItemData1.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData1);
        }
        
        SortSkillBookInPackage();
        // _skillBookList = PetInfoManager.Instance.GetPetSkillBookList();
        ((UI_Currency1)_petBookUI.currency2).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.gold);
        // this.petUI.petBook.list.numItems = _skillBookList.Count;
        _petBookUI.list.numItems = _skillBookItemList.Count + _skillBookList.Count;
        packageItemNum = _skillBookItemList.Count + _skillBookList.Count;

        if (_upLoadPetDict.Count > 0)
        {
            _petBookUI.isShow.selectedIndex = 0;
        }
        else
        {
            _petBookUI.isShow.selectedIndex = 1;
        }
        
        // 回收红点
        _petBookUI.recycleBtnRedPoint.visible = PetInfoManager.Instance.GetSkillBookCount() >= int.Parse(_common3005.Param1);
        RefreshBottomRedDot();
    }

    private void SortSkillBookInPackage()
    {
        _skillBookList = PetInfoManager.Instance.GetPetSkillBookList()
            .OrderByDescending(item => ConfigUtils.GetPetSkillBookUnitById(item.BookId).Quality)
            // .ThenByDescending(item => item.Amount)
            .ThenByDescending(item => item.BookId)
            .ToList();
    }

    // 宠物技能书,背包
    private void PetBookListRender(int index, GObject item)
    {
        UI_PetBookItem itemCom = item as UI_PetBookItem;

        int maxQuality = 0;
        bool hasEmptySlotAndBook = PetInfoManager.Instance.PetSkillBookEmptySlotAndBookRed();
        bool hasHighQualityBook = PetInfoManager.Instance.GetPackageHighPetBookQuality() > PetInfoManager.Instance.GetPetLowQualityBook(battlePetsInBook[selectBookPetIndex]);
        if ((hasEmptySlotAndBook && indexList.Contains(selectBookPetIndex)) || hasHighQualityBook)
        {
            maxQuality = Mathf.Max(PetInfoManager.Instance.GetCommonPetBookHighQuality(), PetInfoManager.Instance.GetPetSkillBookHighQualityList());
        }
        
        // bool hasHighQualityBook = PetInfoManager.Instance.GetPackageHighPetBookQuality() > PetInfoManager.Instance.GetPetLowQualityBook(battlePetsInBook[selectBookPetIndex]);
        // if (hasHighQualityBook)
        // {
        //     maxQuality = Mathf.Max(PetInfoManager.Instance.GetCommonPetBookHighQuality(), PetInfoManager.Instance.GetPetSkillBookHighQualityList());
        // }

        itemCom.bookRedPoint.visible = false;
        if (index < _skillBookItemList.Count)
        {
            // 技能书道具
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_skillBookItemList[index].id);
            itemCom.lockCtrl.selectedIndex = 2;
            itemCom.icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
            itemCom.qualityIcon.visible = true;
            itemCom.qualityCtrl.selectedIndex = itemTypeUnit.Quality - 1;
            itemCom.hasCount.selectedIndex = 1;
            itemCom.txtLv.text = _skillBookItemList[index].count.ToString();
            
            itemCom.data = itemTypeUnit;
            itemCom.onClick.Add(this.OnClickBookItem2);

            if (itemTypeUnit.Quality == maxQuality)
            {
                itemCom.bookRedPoint.visible = true;
            }
        }
        else
        {
            // var skillBook = ConfigUtils.GetPetSkillBookUnitById(_skillBookList[index].BookId);
            var skillBook = ConfigUtils.GetPetSkillBookUnitById(_skillBookList[index - _skillBookItemList.Count].BookId);
            itemCom.lockCtrl.selectedIndex = 2;
            // itemCom.icon = UIResource.GetItemUrl(skillBook.Icon.ToString());
            itemCom.icon = UIResource.GetPetSkillBookIcon(skillBook.Icon.ToString());
            itemCom.qualityIcon.visible = true;
            itemCom.qualityCtrl.selectedIndex = skillBook.Quality - 1;
            itemCom.hasCount.selectedIndex = 1;
            itemCom.txtLv.text = _skillBookList[index - _skillBookItemList.Count].Amount.ToString();
            
            // itemCom.data = skillBook;
            itemCom.data =  _skillBookList[index - _skillBookItemList.Count];
            itemCom.onClick.Add(this.OnClickBookItem);
            
            if (skillBook.Quality == maxQuality)
            {
                itemCom.bookRedPoint.visible = true;
            }
        }
        
    }

    private void OnClickBookItem(EventContext context)
    {
        // PetSkillBook petSkillBook = (context.sender as UI_ItemCom)?.data as PetSkillBook;
        // _selectedSkillBookId = petSkillBook.BookId;
        
        _selectedBookItem = context.sender as UI_PetBookItem;//飞书用的
        
        var selectItem = context.sender as UI_PetBookItem;
        // ConfigPetSkillBookUnit petSkillBook = selectItem?.data as ConfigPetSkillBookUnit;
        // _selectedSkillBookId = petSkillBook.Id;

        PetSkillBook petSkillBookInfo = selectItem?.data as PetSkillBook;
        _selectedSkillBookId = petSkillBookInfo.Guid;
        _selectedBookType = (int)SelectBookType.BOOK;

        foreach (var item in _petBookUI.list.GetChildren())
        {
            UI_PetBookItem itemCom = item as UI_PetBookItem;
            itemCom.selected = (item == selectItem);
        }
        
        //打书按钮状态设置
        if (_selectedSkillBookId < 0)
        {
            _petBookUI.openBookStatus.selectedIndex = 1;
        }
        else
        {
            _petBookUI.openBookStatus.selectedIndex = 0;
        }
        _petBookUI.openType.selectedIndex = _selectedBookType == (int)SelectBookType.BOOK ? 1 : 0;

        ConfigPetSkillBookUnit petSkillBook = ConfigUtils.GetPetSkillBookUnitById(petSkillBookInfo.BookId);
        _petBookUI.petSkillBtn.icon = UIResource.GetPetSkillBookIcon(petSkillBook.Icon.ToString());
        _petBookUI.petSkillBtn.qualityCtrl.selectedIndex = petSkillBook.Quality - 1;
        _petBookUI.name.text = ConfigUtils.GetTextById(petSkillBook.Name.ToString());
        // this.petUI.petBook.desc.text = StringUtils.Format(ConfigUtils.GetTextById(petSkillBook.Doc),petSkillBook.Time);
        // var skillAchieveUnit = ConfigUtils.GetSkillAchieveById(petSkillBook.Id);
        // this.petUI.petBook.desc.text = StringUtils.Format(skillAchieveUnit.Doc,skillAchieveUnit.LastTime);//ConfigUtils.GetTextById(skillAchieveUnit.Doc)

        if (petSkillBook.DocCommon.Equals("0"))//为0时服务器下发
        {
            PetBattleAttr petBattleAttr = new PetBattleAttr();
            petBattleAttr = petSkillBookInfo.battleAttrList[0];
            string value = (petBattleAttr.AttrVal/100).ToString("f2");
            _petBookUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(petSkillBook.Doc),value);
        }
        else
        {
            //读取配置表
            double value = double.Parse(petSkillBook.DocCommon)/100;
            _petBookUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(petSkillBook.Doc),value);
        }
        
    }

    private void OnClickBookItem2(EventContext context)
    {
        _selectedBookItem = context.sender as UI_PetBookItem;//飞书用的
        
        var selectItem = context.sender as UI_PetBookItem;
        ConfigItemTypeUnit petSkillBookItemData = selectItem?.data as ConfigItemTypeUnit;
        _selectedSkillBookId = petSkillBookItemData.Id;
        _selectedBookType = (int)SelectBookType.BOOKITEM;
        
        foreach (var item in _petBookUI.list.GetChildren())
        {
            UI_PetBookItem itemCom = item as UI_PetBookItem;
            itemCom.selected = (item == selectItem);
        }
        
        //打书按钮状态设置
        if (_selectedSkillBookId < 0)
        {
            _petBookUI.openBookStatus.selectedIndex = 1;
        }
        else
        {
            _petBookUI.openBookStatus.selectedIndex = 0;
        }
        _petBookUI.openType.selectedIndex = _selectedBookType == (int)SelectBookType.BOOKITEM ? 0 : 1;
        
        _petBookUI.petSkillBtn.icon = UIResource.GetItemUrl(petSkillBookItemData.Icon.ToString());
        _petBookUI.petSkillBtn.qualityCtrl.selectedIndex = petSkillBookItemData.Quality - 1;
        _petBookUI.name.text = ConfigUtils.GetTextById(petSkillBookItemData.Name.ToString());
        _petBookUI.desc.text = ConfigUtils.GetTextById(petSkillBookItemData.Desc.ToString());
    }

    // 开槽
    private void OnClickMakeBookSlotBtn()
    {
        _petBookUI.makeBtnAni.t0.Play();
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.PetBookPunchingSE);
        
        // 材料不足提示
        if (ItemInfoManager.Instance.GetItemCount(int.Parse(_common300006.Param2.Split(',')[0])) < int.Parse(_common300006.Param2.Split(',')[1]))
        {
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038,ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_common300006.Param2.Split(',')[0])).Name)));
            return;
        }

        //当选中的_unlockSlotId为已解锁的槽位时，取下一槽位进行解锁（下一槽位未解锁）。当选中的_unlockSlotId为未解锁时，解锁该槽位
        bool isCurrentUnlocked = false;
        int? firstLockedSlotId = null;
        foreach (var skillBookSlot in _skillBookSlotList)
        {
            if (_unlockSlotId == skillBookSlot.SlotId)
            {
                if (skillBookSlot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                {
                    isCurrentUnlocked = true;
                }
            }
            else if (firstLockedSlotId == null && skillBookSlot.SlotStatus == eSlotStatus.eSlotStatus_Locked)
            {
                firstLockedSlotId = skillBookSlot.SlotId;
            }
        }
        
        // 处理当前槽位已解锁的情况
        if (isCurrentUnlocked && firstLockedSlotId.HasValue)
        {
            _unlockSlotId = firstLockedSlotId.Value;
        }
        
        var builder = UnlockPetBookSlot_CS.CreateBuilder();
        builder.PetId = _unlockSlotPetGuid;
        builder.SlotIndex = (uint)_unlockSlotId;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_UnlockPetBookSlot_CS, builder.Build());
    }

    // 打书
    private void OnClickOpenBookBtn()
    {
        if (_selectedSkillBookId < 0)
        {
            UIManager.Instance.ToastByKey(8058);
            return;
        }
        
        int slotId;
        PetItemInfo pet = PetInfoManager.Instance.GetPet(_unlockSlotPetGuid);
        
        if (pet == null)
        {
            UIManager.Instance.ToastByKey(8056);
            return;
        }
        
        List<int> slotIdList = new List<int>();//解锁的槽位id
        
        List<int> emptySlots = new List<int>();
        List<int> allNormalSlots = new List<int>();
        
        foreach (var bookSlot in pet.bookSlotsList)
        {
            // 解锁没有书：当有空位时，优先空位。     当解锁的槽位中，没有空位时，随机其中一个
            if (bookSlot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
            {
                allNormalSlots.Add(bookSlot.SlotId);
                if (bookSlot.BookId == 0)
                {
                    emptySlots.Add(bookSlot.SlotId);
                }
            }
        }

        if (emptySlots.Count > 0)
        {
            slotIdList.AddRange(emptySlots);
        }
        else
        {
            slotIdList.AddRange(allNormalSlots);
        }
        
        if (slotIdList.Count > 0)
        {
            // int randomIndex = UnityEngine.Random.Range(0, slotIdList.Count);
            // slotId = slotIdList[randomIndex];
            System.Random rand = new System.Random();
            int randomIndex = rand.Next(slotIdList.Count);
            slotId = slotIdList[randomIndex];
        }
        else
        {
            UIManager.Instance.ToastByKey(8046);
            return;
        }
        
        _petBookUI.openBookBtn.touchable = false;

        FlySkillBook(slotId,_selectedSkillBookId);
        
    }

    private UI_PetBookItem _selectedBookItem; // 选中的技能书UI项
    /// <summary>
    /// 飞书
    /// </summary>
    /// <param name="slotId">槽位id</param>
    /// <param name="bookId">技能书id</param>
    private void FlySkillBook(int slotId, long bookId)
    {
        UI_PetBookItem targetSlotItem = null;
        GList slotList = _petBookUI.skillBookList;
        foreach (GObject item in slotList.GetChildren())
        {
            var slotItem = item as UI_PetBookItem;
            var slotData = slotItem?.data as PetSkillBookSlot;
            if (slotData != null && slotData.SlotId == slotId)
            {
                targetSlotItem = slotItem;
                break;
            }
        }
        
        if (targetSlotItem == null)
        {
            return;
        }
        
        Vector2 startPos = _selectedBookItem.LocalToGlobal(Vector2.zero);
        UIItemsGain itemsGain2 = UIGainBasePool.CreateUIGainBase();
        if (_selectedBookType == (int)SelectBookType.BOOK)
        {
            PetSkillBook petSkillBook = PetInfoManager.Instance.GetSkillBookByGuidInPackage(bookId);//bookId修改为guid了
            itemsGain2.ApplyItemSourceToDestinationEx(targetSlotItem.asCom, UIResource.GetPetSkillBookIcon(ConfigUtils.GetPetSkillBookUnitById(petSkillBook.BookId).Icon.ToString()));
        }

        if (_selectedBookType == (int)SelectBookType.BOOKITEM)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById((int)bookId);
            itemsGain2.ApplyItemSourceToDestinationEx(targetSlotItem.asCom, UIResource.GetItemUrl(itemTypeUnit.Icon.ToString()));
        }
        
        itemsGain2.StartItemFly(startPos, 1, false);
        itemsGain2.flyComplete = () =>
        {
            //技能书
            if (_selectedBookType == (int)SelectBookType.BOOK)
            {
                var builder = PutOnPetBooks_CS.CreateBuilder();
                builder.PetId = _unlockSlotPetGuid;
                builder.SkillBookGuid = (ulong)_selectedSkillBookId;
                builder.SlotIndex = (uint)slotId;
                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_PutOnPetBooks_CS, builder.Build());
            }

            //技能书道具
            if (_selectedBookType == (int)SelectBookType.BOOKITEM)
            {
                var builder2 = UseBookItem_CS.CreateBuilder();
                builder2.PetId = _unlockSlotPetGuid;
                builder2.ItemId = (uint)_selectedSkillBookId;
                builder2.SlotIndex = (uint)slotId;
                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_UseBookItem_CS, builder2.Build());
            }
            
            targetSlotItem.spine.visible = true;
            Utils.PlaySpineAnim(targetSlotItem.spine, "Pet_cwds", false, () =>
            {
                targetSlotItem.spine.visible = false;
                _petBookUI.openBookBtn.touchable = true;
            });
            
            foreach (var item in _petBookUI.list.GetChildren())
            {
                if (item is UI_PetBookItem itemCom)
                {
                    itemCom.selected = false; // 清空选中状态
                }
            }
        };

    }

    private void OnClickBookRecycleBtn()
    {
        UIManager.Instance.ShowUIPanel("PetRecycle",1);
    }

    private void OnClickBookTipsBtn()
    {
        UIManager.Instance.ShowUIPanel("Help",HelpType.Help_PetSkillBook);
    }

    List<int> indexList = new List<int>();
    /// <summary>
    /// 获取有空槽位的上阵宠物的索引
    /// </summary>
    private List<int> GetHasEmptySlotUpLoadPetIndex()
    {
        indexList.Clear();
        for (int i = 0; i < battlePetsInBook.Count; i++)
        {
            List<PetSkillBookSlot> bookSlots = battlePetsInBook[i].bookSlotsList;
            foreach (var slot in bookSlots)
            {
                if (slot.BookId == 0 && slot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                {
                    indexList.Add(i);
                    break;
                }
            }
        }

        return indexList;
    }

    #endregion
}
