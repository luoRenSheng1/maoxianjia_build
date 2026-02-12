using BestHTTP.Extensions;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using RoleMain;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Animation = Spine.Animation;
using EventDispatcher = EngineBase.EventDispatcher;
using HolyItemInfo = Engine.HolyItemInfo;
using RuneInfo = Engine.RuneInfo;
using SkillInfo = Engine.SkillInfo;
using UI_TalentItem = RoleMain.UI_TalentItem;

public enum TalentType
{
    PHY = 1,//物理天赋
    SPELL = 2,//法术天赋
    DEF = 3,//防御天赋
    // ASSIST = 4,//辅助天赋
}

public class RoleSystemView : UIViewBase
{
    private UI_RoleSystem roleUI => this.main as UI_RoleSystem;
    
    private HeroInfo _heroInfo;
    private List<ConfigHeroAttrUnit> _heroAttrUnits;
    private List<int> _skillUnlockLvList = new List<int>();//角色被动技能解锁等级列表

    // private UI_PetMain _petInfoUI;
    // private List<PetItemInfo> _petItemInfos;
    // private Dictionary<int, PetItemInfo> _upLoadPetDict = new Dictionary<int, PetItemInfo>();
    // private Dictionary<int, UI_PetUpLoadItem> _petUpLoadItemDict = new Dictionary<int, UI_PetUpLoadItem>(5);
    // private PetItemInfo _uploadPet;

    private UI_SkillMain _skillInfoUI;
    private List<SkillInfo> _skillInfos;
    private Dictionary<int, SkillInfo> _upLoadSkillDict = new Dictionary<int, SkillInfo>();
    private Dictionary<int, UI_SkillItem> _skillUpLoadItemDict = new Dictionary<int, UI_SkillItem>(6);
    private SkillInfo _uploadSkill;
    private List<int> _pasvIdList = new List<int>();
    
    private UI_RuneMain _runeInfoUI;
    private List<RuneInfo> _runeInfos;
    private Dictionary<int, RuneInfo> _upLoadRuneDict = new Dictionary<int, RuneInfo>();
    private Dictionary<int, UI_RuneItem> _runeUpLoadItemDict = new Dictionary<int, UI_RuneItem>(3);
    private RuneInfo _uploadRune;
    
    // 天赋
    private UI_TalentMain _talentInfoUI;
    private GComponent _currentTalentContainer;
    private ConfigCommonUnit _common100006;
    private List<ConfigAptitudeUnit> _allTalentUnits = new List<ConfigAptitudeUnit>();
    private List<ConfigAptitudeUnlockUnit>  _phyTalentUnits;
    private List<ConfigAptitudeUnlockUnit>  _spellTalentUnits;
    private List<ConfigAptitudeUnlockUnit>  _defTalentUnits;
    // private List<ConfigAptitudeUnlockUnit>  _assistTalentUnits;
    private Dictionary<int,int> _unlockTalentDict = new Dictionary<int,int>(){};//解锁的天赋，由服务器提供
    private int _lastUpgradedTalentId = -1;
    
    // 圣物
    private UI_HolyMain _holyInfoUI;
    // private Dictionary<int, HolyItemInfo> _upLoadHolyDict = new Dictionary<int, HolyItemInfo>();
    private Dictionary<int, UI_HolyUploadItem> _holyUploadItemDict = new Dictionary<int, UI_HolyUploadItem>(3);
    private List<HolyItemInfo> _battleHolyList = new List<HolyItemInfo>();//上阵的圣物
    private List<ConfigHolyUnit> _allHolyUnits = new List<ConfigHolyUnit>();//所有的圣物
    private int holyIndex = 0;//选中的圣物index：圣物列表中
    private ConfigHolyUnit _selectedHolyUnit;//选中的圣物信息
    private int _selectedHolySoltIndex = -1;//选中的圣物槽索引
    private bool _showFlagInHolyList = false; // 引导状态标志
    private bool _isReplaceMode = false; // 替换模式标志
    private bool _isReplace2 = false;// 栏位未满时替换标志

    private bool isUIPanelOpen = false;  //UI界面点击打开圣物界面
    
    private SkeletonAnimation _spine;
    private readonly string[] _RoleAniName = new[] {"skill", "run", "attack", "idle"};
    private int _curRoleAnimIndex = -1;

    private int _openIndex = -1;
    
    private int _preSelectIndex;

    private bool _isGuiding;
    public RoleSystemView()
    {
        this.name = "RoleSystem";
        this.package = "RoleMain";
        this.component = "RoleSystem";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
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
        this.roleUI.EnsureBoundsCorrect();
        
        this.roleUI.tabList.onClickItem.Add(OnClickBottomItem);
        
        this.roleUI.roleInfo.upLevelBtn.onClick.Add(this.OnShowRoleSelect);
        this.roleUI.roleInfo.starList.itemRenderer = HeroStarListRender;
        this.roleUI.roleInfo.levelAttrList.itemRenderer = HeroLevelAttrItemRender;
        this.roleUI.roleInfo.playSpineBtn.onClick.Add(this.OnClickPetToShowAni);
        this.roleUI.roleInfo.list.itemRenderer = PasvItemRender;
        this.roleUI.roleInfo.helpBtn.onClick.Add(this.OnClickHelp);

        /*
        _petInfoUI = UIPackage.CreateObject("RoleMain", "PetMain") as UI_PetMain;
        this.roleUI.AddChildAt(_petInfoUI, 1);
        _petInfoUI.AddRelation(this.roleUI, RelationType.Width);
        _petInfoUI.AddRelation(this.roleUI, RelationType.BottomExt_Bottom);
        _petInfoUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        _petInfoUI.petAllList.SetVirtual();
        _petInfoUI.petAllList.itemRenderer = PetAllItemRender;
        
        this.roleUI.hideUpload1.onClick.Add(this.HideUploadPetView);
        this.roleUI.hideUpload2.onClick.Add(this.HideUploadPetView);
        _petInfoUI.allStrengthBtn.onClick.Add(this.OnClickAllPetStrengthBtn);
        */
        
        _skillInfoUI = UIPackage.CreateObject("RoleMain", "SkillMain") as UI_SkillMain;
        this.roleUI.AddChildAt(_skillInfoUI, 1);
        _skillInfoUI.AddRelation(this.roleUI, RelationType.Width);
        _skillInfoUI.AddRelation(this.roleUI, RelationType.BottomExt_Bottom);
        _skillInfoUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        _skillInfoUI.SkillAllList.SetVirtual();
        _skillInfoUI.SkillAllList.itemRenderer = SkillAllItemRender;
        ConfigCommonUnit _common100004 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(100004);
        string[] lvStrings = _common100004.Param1.Split(',');
        foreach (string idStr in lvStrings)
        {
            _skillUnlockLvList.Add(int.Parse(idStr));
        }
        
        this.roleUI.hideUpload1.onClick.Add(this.HideUploadSkillView);
        this.roleUI.hideUpload2.onClick.Add(this.HideUploadSkillView);
        _skillInfoUI.allStrengthBtn.onClick.Add(this.OnClickAllSkillStrengthBtn);
        
        // 符石部分暂时注释
        // _runeInfoUI = UIPackage.CreateObject("RoleMain", "RuneMain") as UI_RuneMain;
        // this.roleUI.AddChildAt(_runeInfoUI, 1);
        // _runeInfoUI.AddRelation(this.roleUI, RelationType.Width);
        // _runeInfoUI.AddRelation(this.roleUI, RelationType.BottomExt_Bottom);
        // _runeInfoUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        //
        // _runeInfoUI.runeAllList.itemRenderer = RuneAllItemRender;
        // _runeInfoUI.runeAllList.SetVirtual();
        // this.roleUI.hideUpload1.onClick.Add(this.HideUploadRuneView);
        // this.roleUI.hideUpload2.onClick.Add(this.HideUploadRuneView);
        // _runeInfoUI.recycleBtn.onClick.Add(this.OnClickAllRuneRecycleBtn);
        // _runeInfoUI.runDetailList.itemRenderer = RuneDetailItemRender;
        
        // 天赋
        _allTalentUnits = ConfigUtils.GetAptitudeUnits();
        _talentInfoUI = UIPackage.CreateObject("RoleMain", "TalentMain") as UI_TalentMain;
        this.roleUI.AddChildAt(_talentInfoUI, 5);
        _talentInfoUI.AddRelation(this.roleUI, RelationType.Width);
        _talentInfoUI.AddRelation(this.roleUI, RelationType.BottomExt_Bottom);
        _talentInfoUI.SetSize(GRoot.inst.width, GRoot.inst.height);

        _talentInfoUI.talentSelect.btnList.onClickItem.Add(OnClickTalentTypeItem);
        _talentInfoUI.talentSelect.btnList.selectedIndex = 0;
        _phyTalentUnits = ConfigUtils.GetAptitudeUnlockUnits((int)TalentType.PHY);
        _spellTalentUnits =  ConfigUtils.GetAptitudeUnlockUnits((int)TalentType.SPELL);
        _defTalentUnits =  ConfigUtils.GetAptitudeUnlockUnits((int)TalentType.DEF);
        // _assistTalentUnits = ConfigUtils.GetAptitudeUnlockUnits((int)TalentType.ASSIST);
        _common100006 =  ConfigDataGroup.GetInstance<ConfigCommon>().Get(100006);
        _talentInfoUI.resetBtn.onClick.Add(this.OnClickResetTalent);
        
        // 圣物
        _holyInfoUI = UIPackage.CreateObject("RoleMain","HolyMain") as UI_HolyMain;
        this.roleUI.AddChildAt(_holyInfoUI, 1);
        _holyInfoUI.AddRelation(this.roleUI, RelationType.Width);
        _holyInfoUI.AddRelation(this.roleUI, RelationType.BottomExt_Bottom);
        _holyInfoUI.SetSize(GRoot.inst.width, GRoot.inst.height);
        _allHolyUnits = ConfigUtils.GetAllHolyUnits();
        _holyInfoUI.holyList.itemRenderer = HolyItemRender;
        // _holyInfoUI.holyList.SetVirtual();
        _holyInfoUI.holyList.onClick.Add(OnClickHolyList);
        _holyInfoUI.strongBtn.onClick.Add(this.UpLvToHolyItem);// 强化
        _holyInfoUI.upBtn.onClick.Add(this.UpLoadOrReplace);//装备或替换
        _holyInfoUI.downBtn.onClick.Add(this.DownHolyItemInSlot);//卸下圣物
        _holyInfoUI.replaceBtn0.onClick.Add(this.OnClickRepaceBtn0);//替换
        _holyInfoUI.replaceBtn1.onClick.Add(this.OnClickRepaceBtn1);//替换
        _holyInfoUI.replaceBtn2.onClick.Add(this.OnClickRepaceBtn2);//替换
        _holyInfoUI.replaceBtn.onClick.Add(UpLoadOrReplace);
        _holyInfoUI.maxBtn.onClick.Add(this.OnClickMaxBtn);
        _holyInfoUI.helpBtn.onClick.Add(this.OnClickHolyHelp);
        _holyInfoUI.blank1.onClick.Add(this.OnClickBlankAreaToExitGuide);// 点击空白区域退出引导
        _holyInfoUI.blank2.onClick.Add(this.OnClickBlankAreaToExitGuide);
        _holyInfoUI.blank3.onClick.Add(this.OnClickBlankAreaToExitGuide);
        
        /*
        for (int i = 0; i < 5; i++)
        {
            UI_PetUpLoadItem upLoadItem = _petInfoUI.GetChild("petUpItem" + i) as UI_PetUpLoadItem;
            _petUpLoadItemDict.Add(i, upLoadItem);
            upLoadItem?.onClick.Add(this.OnClickUpLoadPetListItem);
            //PetUpListRender(i, upLoadItem);
        }
        */
            
        for (int i = 0; i < 6; i++)
        {
            UI_SkillItem upLoadItem = _skillInfoUI.GetChild("skillItem" + i) as UI_SkillItem;
            _skillUpLoadItemDict.Add(i, upLoadItem);
            upLoadItem?.onClick.Add(this.OnClickUpLoadSkillListItem);
            //SkillUpListRender(i, upLoadItem);
        }

        for (int i = 0; i < 3; i++)
        {
            UI_HolyUploadItem uploadItem = _holyInfoUI.GetChild("holyUploadItem" + i) as UI_HolyUploadItem;
            _holyUploadItemDict.Add(i, uploadItem);
            uploadItem?.onClick.Add(this.OnClickUploadHolyListItem);
            // HolyUpListRender(i, uploadItem);
        }
        
        // 符石
        // for (int i = 0; i < 6; i++)
        // {
        //     UI_RuneItem upLoadItem = _runeInfoUI.GetChild("runeItem" + i) as UI_RuneItem;
        //     _runeUpLoadItemDict.Add(i, upLoadItem);
        //     upLoadItem?.onClick.Add(this.OnClickUpLoadRuneListItem);
        //     //RuneUpListRender(i, upLoadItem);
        // }
        
        this.roleUI.tabList.selectedIndex = 0;
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ADDUNLOCK_PET_POS, this.UpdateUnlockPetPos);
        // EventDispatcher.GameWorld.Regist<PetItemInfo>(EventDefine.EVENT_UPLOAD_PET, this.ShowUploadPetOpt);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PET_SC_SUCC, this.UpdatePetSCSuccsss);
        EventDispatcher.GameWorld.Regist<HeroInfo, int>(EventDefine.EVENT_UPDATE_HEROInfo_LevelUp, this.UpdateRoleLevelInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_HEROInfo_Break, this.UpdateRoleInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_HEROInfo, this.UpdateRoleInfo);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PET_LEVELUP_SUCCESS, this.UpdatePetLevelInfo);
        EventDispatcher.GameWorld.Regist<SkillInfo>(EventDefine.EVENT_UPLOAD_SKILL, this.ShowUploadSkillOpt);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ADDUNLOCK_SKILL_POS, this.UpdateUnlockSkillPos);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_SKILL_SC_SUCC, this.UpdateSkillSCSuccsss);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SKILL_LEVELUP_SUCCESS, this.UpdateSkillLevelInfo);
        EventDispatcher.GameWorld.Regist<RuneInfo>(EventDefine.EVENT_UPLOAD_RUNE, this.ShowUploadRuneOpt);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_RUNE_SC_SUCC, this.UpdateRuneSCSuccsss);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_RuneInfo, this.UpdateRuneList);// TODO 符石
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ADDUNLOCK_RUNE_POS, this.RefreshUpLoadRunes);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_REDPOINT_UPDATE, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_TALENT_UPDATE, UpdateTalentInfo);// 天赋
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HOLY_LIST, UpdateHolySoltInfo);//圣物槽
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPLOAD_HOLY_UPDATE, UpdateAndSortHolyItemList);//圣物
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_HolyInfo, UpdateAndSortHolyItemList);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HOLY_LIST, HolyRedPointInTable);//圣物红点
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, HolyRedPointInTable);//圣物红点
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ADDUNLOCK_PET_POS, this.UpdateUnlockPetPos);
        // EventDispatcher.GameWorld.UnRegist<PetItemInfo>(EventDefine.EVENT_UPLOAD_PET, this.ShowUploadPetOpt);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PET_SC_SUCC, this.UpdatePetSCSuccsss);
        EventDispatcher.GameWorld.UnRegist<HeroInfo, int>(EventDefine.EVENT_UPDATE_HEROInfo_LevelUp, this.UpdateRoleLevelInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_HEROInfo_Break, this.UpdateRoleInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_HEROInfo, this.UpdateRoleInfo);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PET_LEVELUP_SUCCESS, this.UpdatePetLevelInfo);
        EventDispatcher.GameWorld.UnRegist<SkillInfo>(EventDefine.EVENT_UPLOAD_SKILL, this.ShowUploadSkillOpt);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ADDUNLOCK_SKILL_POS, this.UpdateUnlockSkillPos);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_SKILL_SC_SUCC, this.UpdateSkillSCSuccsss);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SKILL_LEVELUP_SUCCESS, this.UpdateSkillLevelInfo);
        EventDispatcher.GameWorld.UnRegist<RuneInfo>(EventDefine.EVENT_UPLOAD_RUNE, this.ShowUploadRuneOpt);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_RUNE_SC_SUCC, this.UpdateRuneSCSuccsss);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_RuneInfo, this.UpdateRuneList);// TODO 符石
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ADDUNLOCK_RUNE_POS, this.RefreshUpLoadRunes);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_REDPOINT_UPDATE, this.RefreshBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_TALENT_UPDATE, UpdateTalentInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HOLY_LIST, UpdateHolySoltInfo);//圣物槽
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPLOAD_HOLY_UPDATE, UpdateAndSortHolyItemList);//圣物
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_HolyInfo, UpdateAndSortHolyItemList);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HOLY_LIST, HolyRedPointInTable);//圣物红点
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, HolyRedPointInTable);//圣物红点
    }

    private void OnClickPetToShowAni()
    {
        if(_spine == null) return;
        _curRoleAnimIndex++;
        if (_curRoleAnimIndex >= _RoleAniName.Length)
            _curRoleAnimIndex = 0;
        string aniName = _RoleAniName[_curRoleAnimIndex];
        _spine.skeleton.SetToSetupPose();
        _spine.state.ClearTracks();
        Animation animation = _spine.skeleton.Data.FindAnimation(aniName);
        while (animation == null)
        {
            _curRoleAnimIndex++;
            if (_curRoleAnimIndex >= _RoleAniName.Length)
                _curRoleAnimIndex = 0;
            aniName = _RoleAniName[_curRoleAnimIndex];
            animation = _spine.skeleton.Data.FindAnimation(aniName);
        }
        
        _spine.state.SetAnimation(0, aniName, false).Complete += (Spine.TrackEntry track) =>
        {
            _spine.state.SetAnimation(0, "idle", true);
        };
    }

    protected override void OnShow()
    {
        base.OnShow();
        _isGuiding = false;
        if (_openIndex != -1)
        {
            ChangeIndex(_openIndex);;
        }
        else
        {
            if (this.roleUI.tabList.selectedIndex == -1)
                this.roleUI.tabList.selectedIndex = 0;
            ChangeIndex(this.roleUI.tabList.selectedIndex);
        }
        OnRoleUpdate();
        this.roleUI.tabList.EnsureBoundsCorrect();
        GameManager.Instance.TimerManager.SetTimer(0.3f, RefreshBottomRedDot);


        //引导-点击技能标签页
        if (RoleManager.Instance.GetSkillSlotInfoCount() <= 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_OpenHeroSys,
            bid = GuideID.Click_SummonSkill,
            gid = GuideID.Click_SkillBtn,
            tui = this.roleUI.tabList.GetChildAt(1),
            isForce = true,
            isSend = true,
            isLucency = false
        }))
        { }
        else
        {
            GuideManager.Instance.SendToCompleteGuide((int)GuideID.Click_OpenHeroSys);
            GuideManager.Instance.HideGuide();
        }

        //if (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_SkillBtn))
        //{
        //    GuideManager.Instance.HideGuide();
        //    //技能标签
        //    GuideManager.Instance.StartGuide(this.roleUI.tabList.GetChildAt(1), GuideID.Click_SkillBtn, PosType.Left, true, false);
        //}
        //else if (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_PetBtn))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.roleUI.tabList.GetChildAt(2), GuideID.Click_PetBtn, PosType.Left, true, false);
        //}

        //var petMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonPetPos2);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_PetBtn) && petMap.Item1)
        //{
        //    ChangeIndex(0);
        //    _isGuiding = true;
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.roleUI.tabList.GetChildAt(2), GuideID.Trigger_Click_PetBtn, PosType.Left, true, true);
        //}

        //var roleMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroLevelUp);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_OpenHeroLvBtn) && roleMap.Item1)
        //{
        //    ChangeIndex(0);
        //    _isGuiding = true;
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.roleUI.roleInfo.upLevelBtn, GuideID.Trigger_Click_OpenHeroLvBtn, PosType.Left, true, false);
        //}

        // 符石
        // var runeMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Rune);
        // if(!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_RuneBtn) && runeMap.Item1)
        // {
        //     ChangeIndex(0);
        //     _isGuiding = true;
        //     GuideManager.Instance.HideGuide();
        //     GuideManager.Instance.StartGuide(this.roleUI.tabList.GetChildAt(3), GuideID.Trigger_Click_RuneBtn, PosType.Left, true, true);
        // }

        if (_talentInfoUI.talentSelect.btnList.selectedIndex == -1)
        {
            _talentInfoUI.talentSelect.btnList.selectedIndex = 0;
        }
        ChangeTalentIndex(_talentInfoUI.talentSelect.btnList.selectedIndex);
    }

    private void OnClickBottomItem(EventContext context)
    {
        GButton item = context.data as GButton;
        var index = this.roleUI.tabList.GetChildIndex(item);
        ChangeIndex(index);
    }
    
    private void ChangeIndex(int index)
    {
        bool isChange = true;
        switch (index)
        {
            case 0:
                UpdateRoleInfo();
                _preSelectIndex = index;
                break;
            case 1:
                var skillMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
                if (skillMap.Item1)
                {
                    UpdateSkillInfo();
                    _preSelectIndex = index;

                    if(RoleManager.Instance.GetSkillSlotInfoCount() <= 0)
                    {
                        GuideManager.Instance.StarGuideByData(new GuideData()
                        {
                            giding = GuideID.Click_SkillBtn,
                            bid = GuideID.Click_SummonSkill,
                            gid = GuideID.Click_SkillIntensify,
                            tui = _skillInfoUI.allStrengthBtn,
                            isForce = true,
                            isSend = true,
                            npcTxt = "Beginner_Doc_010",
                            npcPosType = PosType.Down,
                            isLucency = false
                        });
                    }

                    //if (!GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_SkillIntensify))
                    //{
                    //    GuideManager.Instance.HideGuide();
                    //    //批量强化按钮
                    //    GuideManager.Instance.StartGuide(_skillInfoUI.allStrengthBtn, GuideID.Click_SkillIntensify, PosType.Left, true, false);
                    //}
                }
                else
                {
                    UIManager.Instance.Toast(skillMap.Item2);
                    ((UI_TabBtn) this.roleUI.tabList.GetChildAt(index)).selected = false;
                    isChange = false;
                }
                break;
            case 2: // 天赋
                var heroTalentMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroTalent);
                if (heroTalentMap.Item1)
                {
                    UpdateTalentInfo();
                    _preSelectIndex = index;
                }
                else
                {
                    UIManager.Instance.Toast(heroTalentMap.Item2);
                    ((UI_TabBtn) this.roleUI.tabList.GetChildAt(index)).selected = false;
                    isChange = false;
                }
                break;
            case 3: // 圣物
                var heroHolyMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroHoly);
                if (heroHolyMap.Item1)
                {
                    UpdateHolyInfo();
                    _preSelectIndex = index;
                }
                else
                {
                    UIManager.Instance.Toast(heroHolyMap.Item2);
                    ((UI_TabBtn) this.roleUI.tabList.GetChildAt(index)).selected = false;
                    isChange = false;
                }
                break;
        }

        if (isChange)
        {
            _skillInfoUI.visible = index == 1;
            // _petInfoUI.visible = index == 2;
            // _runeInfoUI.visible = index == 3;
            _talentInfoUI.visible = index == 2;
            _holyInfoUI.visible = index == 3;
        }

        for (int i = 0; i < 4; i++)
        {
            if(i == _preSelectIndex)
                ((UI_TabBtn) this.roleUI.tabList.GetChildAt(_preSelectIndex)).selected = true;
            else
            {
                ((UI_TabBtn) this.roleUI.tabList.GetChildAt(i)).selected = false;
            }
        }

        this.roleUI.typeCtrl.selectedIndex = _preSelectIndex;
        if(_isGuiding)
            GuideManager.Instance.HideGuide();
    }
    
    protected override void OnHide()
    {
        base.OnHide();
        _openIndex = 0;
        // _petInfoUI.petBg.url = null;
        // _skillInfoUI.skillBg.url = null;
        Utils.ClearSpineModelOnFGUI(this.roleUI.roleInfo.spine);

        if (GuideManager.Instance.GuideId == (int)GuideID.Click_UIRoleClose)
        {
            GuideManager.Instance.SendToCompleteGuide((int)GuideID.Click_UIRoleClose);
            GuideManager.Instance.HideGuide();
        }
    }

    public void RefreshBottomRedDot()
    {
        ((UI_TabBtn) this.roleUI.tabList.GetChildAt(0)).redCtrl.selectedIndex = HeroInfoManager.Instance.IsShowRedDot() ? 1 : 0;
        this.roleUI.roleInfo.redDot.visible = HeroInfoManager.Instance.IsShowRedDot();
        
        var skillMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
        if (!skillMap.Item1)
        {
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(1)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(1)).lockCtrl.selectedIndex = 0;
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(1)).redCtrl.selectedIndex = (SkillInfoManager.Instance.IsCanUpLevel() || SkillInfoManager.Instance.IsCanUpLoadSkill()) ? 1 : 0; 
        }
        
        var heroTalentMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroTalent);
        if (!heroTalentMap.Item1)
        {
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(2)).lockCtrl.selectedIndex = 0;
        }
        
        var heroHolyMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroHoly);
        if (!heroHolyMap.Item1)
        {
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(3)).lockCtrl.selectedIndex = 1;
        }
        else
        {
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(3)).lockCtrl.selectedIndex = 0;
            ((UI_TabBtn) this.roleUI.tabList.GetChildAt(3)).redCtrl.selectedIndex = HolyManager.Instance.RedPointHandle() ? 1 : 0;// 圣物红点
        }
        
    }

    private void OnClickHelp()
    {
        UIManager.Instance.ShowUIPanel("Help",HelpType.Helo_RoleHelp);
    }

    #region 角色

    private void UpdateRoleInfo()
    {
        _heroInfo = HeroInfoManager.Instance.GetMyHero();
        GameManager.Instance.TimerManager.SetTimer(0.2f, () =>
        {
            string strResName = ConfigUtils.GetHeroModelUIPathByID(_heroInfo.HeroUnit.Id);
            // float scale = 180f * Math.Min(1.2f,GRoot.contentScaleFactor);
            float scale = 120f * Math.Min(1f,GRoot.contentScaleFactor);
            if (_heroInfo.HeroUnit.Id == 20036 || _heroInfo.HeroUnit.Id == 20046)
            {
                scale = 145f;
            }
            Utils.SetSpineModelOnFGUI(this.roleUI.roleInfo.spine, strResName, scale, "idle", (o) =>
            {
                if (o is SkeletonAnimation animation)
                    _spine = animation;
            });
        });
        
        this.roleUI.roleInfo.roleName.text = ConfigUtils.GetTextById(_heroInfo.HeroUnit.Name);
        this.roleUI.roleInfo.roleLv.SetVar("value",_heroInfo.Level.ToString()).FlushVars();

        this.roleUI.roleInfo.occupationLb.url = UIResource.GetRoleOccupationImg(_heroInfo.HeroUnit.Vocation.ToString());//职业
        this.roleUI.roleInfo.attrLb.url = UIResource.GetRoleAttrImgImg(_heroInfo.HeroUnit.VocationAttr.ToString());//属系
        
        this.roleUI.roleInfo.expBar.min = 0;
        ((UI_HeroBarExp) this.roleUI.roleInfo.expBar).maxCtrl.selectedIndex = 0;
        ConfigHeroLevelUnit levelUnit = ConfigUtils.GetHeroLevel(_heroInfo.HeroUnit.Id,_heroInfo.Level+1);
        if (levelUnit != null)
        {
            this.roleUI.roleInfo.expBar.max =  levelUnit.Exp;
        }
        if (levelUnit != null && levelUnit.Item != "0" && levelUnit.Level > _heroInfo.BreakLevel)
        {
            ((UI_HeroBarExp) this.roleUI.roleInfo.expBar).maxCtrl.selectedIndex = 1;
        }
        this.roleUI.roleInfo.expBar.value = _heroInfo.Exp;
        
        ((UI_qualityLabel) this.roleUI.roleInfo.roleName).qualityCtrl.selectedIndex = _heroInfo.HeroUnit.HeroQuality-1;
        ((UI_roleQualityItem) this.roleUI.roleInfo.qIcon).quality.selectedIndex = _heroInfo.HeroUnit.HeroQuality-1;
        
        // 主动技能
        ConfigSkillUnit activeSkillCfg = ConfigUtils.GetSkillById(_heroInfo.HeroUnit.ActiveSkill);
        // this.roleUI.roleInfo.skillItem.icon = UIResource.GetItemUrl(activeSkillCfg.SkillIcon);
        this.roleUI.roleInfo.skillItem.icon = UIResource.GetHeroSkillIcon(activeSkillCfg.SkillIcon);//暂时注释
        // this.roleUI.roleInfo.skillItem.icon = UIResource.GetHeroSkillIcon(2004602.ToString());//美术要求暂时写死
        this.roleUI.roleInfo.skillItem.status.selectedIndex = 1;//主动技能默认解锁
        this.roleUI.roleInfo.skillItem.data = activeSkillCfg;
        
        // 普攻
        ConfigSkillUnit atkSkillUnit = ConfigUtils.GetSkillById(_heroInfo.HeroUnit.AtkSkill);
        // this.roleUI.roleInfo.skillItem2.icon = UIResource.GetItemUrl(atkSkillUnit.SkillIcon);
        this.roleUI.roleInfo.skillItem2.icon = UIResource.GetHeroSkillIcon(atkSkillUnit.SkillIcon);//暂时注释
        this.roleUI.roleInfo.skillItem2.icon = UIResource.GetHeroSkillIcon(2004601.ToString());//美术要求暂时写死
        this.roleUI.roleInfo.skillItem2.status.selectedIndex = 1;//普攻技能默认解锁
        this.roleUI.roleInfo.skillItem2.data = atkSkillUnit;
        
        // 被动技能
        _pasvIdList.Clear();
        // string[] buffIdStrings = activeSkillCfg.BuffId.ToString().Split('|');//被动技能id数组
        string[] buffIdStrings = _heroInfo.HeroUnit.Passive.Split('|');//被动技能id数组
        if (buffIdStrings.Length > 1)
        {
            foreach (string idStr in buffIdStrings)
            {
                if (!_pasvIdList.Contains(int.Parse(idStr)))
                {
                    _pasvIdList.Add(int.Parse(idStr));
                }
            }
        }
        this.roleUI.roleInfo.list.numItems = _pasvIdList.Count;
        
        
        // 技能描述
        this.roleUI.roleInfo.desc.text = StringUtils.Format(ConfigUtils.GetTextById(activeSkillCfg.SkillDes), (_heroInfo.SkillDamageRate*ConstDefine.CONFIG_PLACE).ToString("f2"));
        this.roleUI.roleInfo.skillItem.onClick.Add(OnClickActiveOrAtkSkill);
        this.roleUI.roleInfo.skillItem2.onClick.Add(OnClickActiveOrAtkSkill);
        
        // this.roleUI.roleInfo.cdLb.SetVar("value", (activeSkillCfg.Cd * ConstDefine.CONFIG_PLACE_EX).ToString("")).FlushVars();//修改
        // this.roleUI.roleInfo.roleSkillIcon.onClick.Set(this.OnClickSkillIconToShowEff);//修改

        _heroAttrUnits = ConfigUtils.GetHeroAttrsByHeroId(_heroInfo.HeroUnit.Id);
        // if (_heroInfo.HeroUnit.Id == HeroInfoManager.Instance.GetHeroFirstID())
        // {
        //     this.roleUI.roleInfo.starList.numItems = 0;
        //     this.roleUI.roleInfo.levelAttrList.numItems = _heroAttrUnits.Count;//初始角色没有突破属性
        // }
        // else
        // {
        //     this.roleUI.roleInfo.starList.numItems = _heroInfo.BreakLevelLayer > 0 ? 3 : 0;
        //     this.roleUI.roleInfo.levelAttrList.numItems = _heroAttrUnits.Count + 1;//有一个是大于100级的突破属性
        // }
        
        this.roleUI.roleInfo.starList.numItems = _heroInfo.BreakLevelLayer > 0 ? 3 : 0;
        this.roleUI.roleInfo.levelAttrList.numItems = _heroAttrUnits.Count + 1;//有一个是大于100级的突破属性
        
        // 默认选中主动技能
        this.roleUI.roleInfo.skillItem.selected = true;
        this.roleUI.roleInfo.skillItem2.selected = false;
    
        // 确保被动技能列表初始无选中项
        foreach (GObject item in this.roleUI.roleInfo.list.GetChildren())
        {
            ((UI_SkillItemCom)item).selected = false;
        }
  
    }

    private void UpdateRoleLevelInfo(HeroInfo heroInfo, int preLevel)
    {
        _heroInfo = heroInfo;
        this.roleUI.roleInfo.roleLv.SetVar("value",_heroInfo.Level.ToString()).FlushVars();
        
        this.roleUI.roleInfo.expBar.min = 0;
        ((UI_HeroBarExp) this.roleUI.roleInfo.expBar).maxCtrl.selectedIndex = 0;
        ConfigHeroLevelUnit levelUnit = ConfigUtils.GetHeroLevel(_heroInfo.HeroUnit.Id,_heroInfo.Level+1);
        if (levelUnit != null)
        {
            this.roleUI.roleInfo.expBar.max =  levelUnit.Exp;
        }
        if (levelUnit != null && levelUnit.Item != "0" && levelUnit.Level > _heroInfo.BreakLevel)
        {
            ((UI_HeroBarExp) this.roleUI.roleInfo.expBar).maxCtrl.selectedIndex = 1;
        }
        this.roleUI.roleInfo.expBar.value = _heroInfo.Exp;
        
        _heroAttrUnits = ConfigUtils.GetHeroAttrsByHeroId(_heroInfo.HeroUnit.Id);
        // if (_heroInfo.HeroUnit.Id == HeroInfoManager.Instance.GetHeroFirstID())
        // {
        //     this.roleUI.roleInfo.levelAttrList.numItems = _heroAttrUnits.Count;//初始角色没有突破属性
        // }
        // else
        // {
        //     this.roleUI.roleInfo.levelAttrList.numItems = _heroAttrUnits.Count + 1;//有一个是大于100级的突破属性
        // }
        this.roleUI.roleInfo.levelAttrList.numItems = _heroAttrUnits.Count + 1;//有一个是大于100级的突破属性
        ConfigSkillUnit activeSkillCfg = ConfigUtils.GetSkillById(_heroInfo.HeroUnit.ActiveSkill);
        this.roleUI.roleInfo.desc.text = StringUtils.Format(ConfigUtils.GetTextById(activeSkillCfg.SkillDes), (_heroInfo.SkillDamageRate*ConstDefine.CONFIG_PLACE).ToString("f2"));
        
        this.roleUI.roleInfo.list.numItems = _pasvIdList.Count;
    }
    
    private void OnClickSkillIconToShowEff(EventContext context)
    {
        int skillLevelId = _heroInfo.HeroUnit.ActiveSkill;
        ConfigSkillUnit skillLevel = ConfigUtils.GetSkillById(skillLevelId);
        UIManager.Instance.ShowUIPanel("SkillEffectPre", skillLevel.AttackEffect);
    }
    
    private void OnRoleUpdate()
    {
        ((RoleMain.UI_Currency)(this.roleUI.btnGold)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        ((RoleMain.UI_Currency)(this.roleUI.btnDia)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia);
        ((UI_RoleCurrency)(this.roleUI.btnGold2)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        ((UI_RoleCurrency)(this.roleUI.btnDia2)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia);
    }

    private void OnShowRoleSelect()
    {
        UIManager.Instance.ShowUIPanel("RoleSelect", HeroInfoManager.Instance.GetMyHero());
    }

    private void HeroLevelAttrItemRender(int index, GObject item)
    {
        if (index != _heroAttrUnits.Count)
        {
            ((UI_RoleAttrItem) item).typeCtrl.selectedIndex = 0;
            ConfigHeroAttrUnit heroAttr = _heroAttrUnits[index];
            ((UI_RoleAttrItem) item).lockCtrl.selectedIndex = _heroInfo.Level >= heroAttr.Level ? 0 : 1;
            // ((UI_RoleAttrItem) item).attrCtrl.selectedIndex = heroAttr.AttrId - 1;
            ((UI_RoleAttrItem)item).pIcon.url = UIResource.GetAttrIconById(heroAttr.AttrId.ToString());
            if (((UI_RoleAttrItem) item).lockCtrl.selectedIndex == 1)
            {
                ((UI_RoleAttrItem) item).lvLb.SetVar("value", heroAttr.Level.ToString()).FlushVars();
            }
        }
        else
        {
            var starParam = Utils.GetHeroStar(_heroInfo.BreakLevelLayer);
            for (int i = 0; i < 3; i++)
            {
                ((UI_RoleStarItem) ((UI_RoleAttrItem) item).GetChild("star" + i)).lockCtrl.selectedIndex= starParam.Item2 > i ? 0 : 1;
                ((UI_RoleStarItem) ((UI_RoleAttrItem) item).GetChild("star" + i)).type.selectedIndex = starParam.Item1;
            }
            (double, int) attrParam = ConfigUtils.GetHeroAttrsByBreakLevel(_heroInfo.HeroUnit.Id, _heroInfo.BreakLevel);
            // ((UI_RoleAttrItem) item).attrCtrl.selectedIndex = attrParam.Item2-1;
            ((UI_RoleAttrItem)item).pIcon.url = UIResource.GetAttrIconById(attrParam.Item2.ToString());
            ((UI_RoleAttrItem) item).starCount.selectedIndex = starParam.Item2;
            ((UI_RoleAttrItem) item).typeCtrl.selectedIndex = 1;
            ((UI_RoleAttrItem) item).lockCtrl.selectedIndex = starParam.Item2 == 0 ? 1 : 0;
            ((UI_RoleAttrItem) item).starLb.text = _heroInfo.BreakLevelLayer.ToString();
        }
        
        ((UI_RoleAttrItem) item).data = index;
        ((UI_RoleAttrItem) item).onClick.Set(OnHeroLevelAttrTips);
    }
    
    private void OnHeroLevelAttrTips(EventContext context)
    {
        int index = (int)((UI_RoleAttrItem)context.sender).data;
        if (index >= 0 && index < _heroAttrUnits .Count)
        {
            ConfigHeroAttrUnit heroAttr = _heroAttrUnits[index];
            // TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, StringUtils.ConvertToAttributeValue(heroAttr.AttrId, heroAttr.Value), _heroInfo.Level>=heroAttr.Level, heroAttr.Level,heroAttr.AttrId );
            
            int attrId = heroAttr.AttrId;
            double value = double.Parse(heroAttr.Value.ToString());
            string lastValue = EquipManager.Instance.SetAttributeValue(attrId, value,  true);
            TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, attrId, lastValue, _heroInfo.Level>=heroAttr.Level, heroAttr.Level);
        }
        else
        {
            int breakLevel = _heroInfo.BreakLevel;
            if (breakLevel == 0)
            {
                breakLevel = 100;
            }
            (double, int) attrParam = ConfigUtils.GetHeroAttrsByBreakLevel(_heroInfo.HeroUnit.Id, breakLevel);
            int attrId = attrParam.Item2;
            double value = attrParam.Item1;
            string lastValue = EquipManager.Instance.SetAttributeValue(attrId, value,  true);
            TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, attrId, lastValue, true, 0);
        }
    }

    private void HeroStarListRender(int index, GObject item)
    {
        var starParam = Utils.GetHeroStar(_heroInfo.BreakLevelLayer);
        ((UI_RoleStarItem) item).type.selectedIndex = starParam.Item1;
        ((UI_RoleStarItem) item).lockCtrl.selectedIndex = starParam.Item2 > index ? 0 : 1;
    }
    
    // 被动技能列表
    private void PasvItemRender(int index, GObject item)
    {
        ConfigSkillBuffUnit skillBuffUnit = ConfigUtils.GetSkillBuffByBuffId(_pasvIdList[index]);
        // ((UI_SkillItemCom)item).icon = UIResource.GetItemUrl(skillBuffUnit.Item.ToString());
        // ((UI_SkillItemCom)item).icon = UIResource.GetHeroSkillIcon(skillBuffUnit.Item.ToString());
        ((UI_SkillItemCom)item).icon = UIResource.GetHeroSkillIcon(skillBuffUnit.BuffId.ToString());//美术资源命名是按照BuffId命名的
        // skillBuffUnit.Common;//buff参数

        // ConfigSkillAchieveUnit skillAchieveUnit = ConfigUtils.GetSkillAchieveById(_pasvIdList[index]);
        // ((UI_SkillItemCom)item).icon = UIResource.GetItemUrl(skillAchieveUnit.Item.ToString());
        
        if (_heroInfo.Level >= _skillUnlockLvList[index])
        {
            ((UI_SkillItemCom)item).status.selectedIndex = 1;
        }
        else
        {
            ((UI_SkillItemCom)item).status.selectedIndex = 0;
        }

        ((UI_SkillItemCom)item).data = skillBuffUnit;
        // ((UI_SkillItemCom)item).data = skillAchieveUnit;
        ((UI_SkillItemCom)item).onClick.Set(OnClickPasvSkill);
    }

    private void OnClickPasvSkill(EventContext context)
    {
        UI_SkillItemCom clickedPasv = (UI_SkillItemCom)context.sender;
        
        // 清除主动和普攻按钮的选中状态
        this.roleUI.roleInfo.skillItem.selected = false;
        this.roleUI.roleInfo.skillItem2.selected = false;

        // 清除所有被动技能项的选中状态，并设置当前点击项
        foreach (GObject item in this.roleUI.roleInfo.list.GetChildren())
        {
            UI_SkillItemCom skillItem = (UI_SkillItemCom)item;
            skillItem.selected = (item == clickedPasv);
        }

        // 更新技能描述
        ConfigSkillBuffUnit skillBuffUnit = clickedPasv.data as ConfigSkillBuffUnit;
        string time = skillBuffUnit.Time.ToString();
        this.roleUI.roleInfo.desc.text = StringUtils.Format(ConfigUtils.GetTextById(skillBuffUnit.Doc),time);
        // ConfigSkillAchieveUnit skillAchieveUnit = clickedPasv.data as ConfigSkillAchieveUnit;
        // this.roleUI.roleInfo.desc.text = ConfigUtils.GetTextById(skillAchieveUnit.Doc);
        
        if (clickedPasv.status.selectedIndex == 0)
        {
            // 获取点击项在列表中的索引
            GList list = this.roleUI.roleInfo.list;
            int index = list.GetChildIndex(clickedPasv);
            int requiredLevel = _skillUnlockLvList[index];
            
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8042,requiredLevel));
        }
    }

    private void OnClickActiveOrAtkSkill(EventContext context)
    {
        UI_SkillItemCom clickedSkill = (UI_SkillItemCom)context.sender;
        // 清除所有技能按钮的选中状态
        this.roleUI.roleInfo.skillItem.selected = false;
        this.roleUI.roleInfo.skillItem2.selected = false;
        
        // 设置当前点击的按钮为选中
        clickedSkill.selected = true;
        
        // 清除被动技能项的选中状态
        foreach (GObject item in this.roleUI.roleInfo.list.GetChildren())
        {
            ((UI_SkillItemCom)item).selected = false;
        }
        
        // 更新技能描述
        ConfigSkillUnit skillUnit = clickedSkill.data as ConfigSkillUnit;
        this.roleUI.roleInfo.desc.text = StringUtils.Format(
            ConfigUtils.GetTextById(skillUnit.SkillDes), 
            (_heroInfo.SkillDamageRate * ConstDefine.CONFIG_PLACE).ToString("f2")
        );
        
    }

    #endregion
    

    #region 宠物
    /*
    private void UpdatePetInfo()
    {
        _petInfoUI.petBg.url = UIResource.GetRoleBg("nn15");
        _petInfoUI.petBg.priority = true;
        HideUploadPetView();
        _petItemInfos = PetInfoManager.Instance.GetAllPetInfoList();
        _petItemInfos.Sort((a, b)=>
        {
            int result = a.petCfg.Quality > b.petCfg.Quality ? 1 : (a.petCfg.Quality==b.petCfg.Quality ? 0 : -1);
            if (result == 0)
                result = a.petCfg.Id > b.petCfg.Id ? 1 : -1;
            return result;
        });
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        UpdatePetLevelInfo();
        UpdateUnlockPetPos();
        RefrehUpLoadItem();
    }
    
    private void PetAllItemRender(int index, GObject item)
    {
        ConfigPetBasisUnit petBasisUnit = _petItemInfos[index].petCfg;
        ((UI_PetItemEx) item).icon = UIResource.GetPetIcon(petBasisUnit.IconPath);
        ((UI_PetItemEx) item).ctrlQuality.selectedIndex = petBasisUnit.Quality-1;
        ((UI_PetItemEx) item).type.selectedIndex = 0;
        ((UI_PetItemEx) item).img.url = UIResource.GetImageUrlWithLang("ysz", "RoleMain");
        PetItemInfo hasPetInfo = PetInfoManager.Instance.GetPet(_petItemInfos[index].petCfg.Id);
        if (hasPetInfo != null)
        {
            ((UI_PetItemEx) item).disableCtrl.selectedIndex = 0;
            bool isUpload = PetInfoManager.Instance.IsInUpload(hasPetInfo);
            ((UI_PetItemEx) item).uploadCtrl.selectedIndex = isUpload ? 0 : 1;
            (ConfigPetLevelUnit, bool) petItem =
                ConfigUtils.GetPetLevelByLevel(hasPetInfo.petCfg.Id, hasPetInfo.PetLv+1);
            ((UI_PetItemEx) item).lvLb.text = hasPetInfo.PetLv.ToString();
            if (petItem.Item1 != null && !petItem.Item2)
            {
                if (petItem.Item1.CardNumber <= hasPetInfo.CardNumber)
                {
                    ((UI_PetItemEx) item).levelUp.selectedIndex = 1;
                }
                else
                {
                    ((UI_PetItemEx) item).levelUp.selectedIndex = 0;
                }
    
                ((UI_PetItemEx) item).numBar.maxCtrl.selectedIndex = 0;
                ((UI_PetItemEx) item).numBar.min = 0;
                ((UI_PetItemEx) item).numBar.max = petItem.Item1.CardNumber;
                ((UI_PetItemEx) item).numBar.value = hasPetInfo.CardNumber;
            }
            else
            {
                ((UI_PetItemEx) item).levelUp.selectedIndex = 0;
                ((UI_PetItemEx) item).numBar.maxCtrl.selectedIndex = 1;
            }
        }
        else
        {
            (ConfigPetLevelUnit, bool) noPetItem =
                ConfigUtils.GetPetLevelByLevel(_petItemInfos[index].petCfg.Id, _petItemInfos[index].PetLv+1);
            if (noPetItem.Item1 != null)
            {
                ((UI_PetItemEx) item).numBar.min = 0;
                ((UI_PetItemEx) item).numBar.max = noPetItem.Item1.CardNumber;
                ((UI_PetItemEx) item).numBar.value = _petItemInfos[index].CardNumber;
            }
            ((UI_PetItemEx) item).disableCtrl.selectedIndex = 1;
            ((UI_PetItemEx) item).levelUp.selectedIndex = 0;
            ((UI_PetItemEx) item).numBar.maxCtrl.selectedIndex = 0;
        }
        
        ((UI_PetItemEx) item).data = _petItemInfos[index];
        ((UI_PetItemEx) item).onClick.Set(this.OnClickPetItem);
    }
    
    private void OnClickPetItem(EventContext context)
    {
        PetItemInfo pet = (context.sender as UI_PetItemEx)?.data as PetItemInfo;
        if(pet != null)
            UIManager.Instance.ShowUIPanel("PetDetail", pet);
    }
    
    private void OnClickUpLoadPetListItem(EventContext context)
    {
        UI_PetUpLoadItem item = context.sender as UI_PetUpLoadItem;
        int index = int.Parse(item.name.Substring(9, 1));
        if (item.showHandCtrl.selectedIndex == 1)
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop(18);
            var builder = PetInSlot_CS.CreateBuilder();
            builder.PetId = (uint) _uploadPet.petCfg.Id;
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
    private void RefrehUpLoadItem()
    {
        GameManager.Instance.TimerManager.ClearTimer(RefreshUpLoadPets);
        GameManager.Instance.TimerManager.SetTimer(0.1f, RefreshUpLoadPets);
        double atkadd = 0;
        double hpadd = 0;
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
        _petInfoUI.hpLb.SetVar("value", StringUtils.FormatCurrency(hpadd * ConstDefine.CONFIG_PLACE)).FlushVars();
        _petInfoUI.atkLb.SetVar("value", StringUtils.FormatCurrency(atkadd * ConstDefine.CONFIG_PLACE)).FlushVars();
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
        _petInfoUI.redDot.visible = PetInfoManager.Instance.IsCanUpLevel();
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
        ((UI_qualityLabel)item.pName).qualityCtrl.selectedIndex = pet.petCfg.Quality - 1;
        // ((UI_qualityLabel) item.pName).text = pet.petCfg.Name;
        ((UI_qualityLabel) item.pName).text = ConfigUtils.GetTextById(pet.petCfg.Name);
        Utils.SetSpineModelOnFGUI(item.spine, pet.petCfg.PetModel, index>2 ?75 : 90, "idle");
    }
    
    private void UpdateUnlockPetPos()
    {
        RefreshUpLoadPets();
    }
    
    private void HideUploadPetView()
    {
        this.roleUI.showPetHand.selectedIndex = 0;
        foreach (var item in _petUpLoadItemDict)
        {
            item.Value.showHandCtrl.selectedIndex = 0;
        }
    }
    
    private void ShowUploadPetOpt(PetItemInfo pet)
    {
        _uploadPet = pet;
        this.roleUI.showPetHand.selectedIndex = 1;
        this.roleUI.hideUpload1.y = _petInfoUI.rlLoader.y;
        this.roleUI.hideUpload2.height = _petInfoUI.upGroup.y;
        foreach (var item in _petUpLoadItemDict)
        {
            item.Value.showHandCtrl.selectedIndex = 1;
        }
    }
    
    private void UpdatePetSCSuccsss()
    {
        HideUploadPetView();
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        _petInfoUI.petAllList.numItems = _petItemInfos.Count;
        RefrehUpLoadItem();
    }
    
    private void OnClickAllPetStrengthBtn()
    {
        var builder = AllPetLevelUp_CS.CreateBuilder();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AllPetLevelUp_CS, builder.Build());
    }
    
    private void UpdatePetLevelInfo()
    {
        _upLoadPetDict = PetInfoManager.Instance.GetUpLoadPet();
        _petInfoUI.petAllList.numItems = _petItemInfos.Count;
    
        bool isCanLevelUp = false;
        foreach (var item in PetInfoManager.Instance.GetAllHavePetList())
        {
            (ConfigPetLevelUnit, bool) petItem =
                ConfigUtils.GetPetLevelByLevel(item.petCfg.Id, item.PetLv+1);
            if (petItem.Item1 != null)
            {
                if (petItem.Item1.CardNumber <= item.CardNumber)
                {
                    isCanLevelUp = true;
                    break;
                }
            }
        }
    
        _petInfoUI.allStrengthBtn.enabled = isCanLevelUp;
    
        RefrehUpLoadItem();
        RefreshBottomRedDot();
    }
    */
    #endregion
    
    #region 技能
    
    private void UpdateSkillInfo()
    {
        HideUploadSkillView();
        _skillInfos = SkillInfoManager.Instance.GetAllSkillInfoList();
        _skillInfos.Sort((a, b)=>
        {
            int result = a.SkillUnit.SkillQuality > b.SkillUnit.SkillQuality ? 1 : (a.SkillUnit.SkillQuality==b.SkillUnit.SkillQuality ? 0 : -1);
            if (result == 0)
                result = a.SkillUnit.Id > b.SkillUnit.Id ? 1 : -1;
            return result;
        });
        _upLoadSkillDict = SkillInfoManager.Instance.GetUpLoadSkill(true);
        _skillInfoUI.SkillAllList.numItems = _skillInfos.Count;
        UpdateUnlockSkillPos();
        UpdateSkillLevelInfo();
        RefrehUpLoadSkillItem();
    }
    
    private void SkillAllItemRender(int index, GObject item)
    {
        ConfigSkillUnit skillUnit = _skillInfos[index].SkillUnit;
        ((UI_PetItemEx) item).icon = UIResource.GetItemUrl(skillUnit.SkillIcon);
        ((UI_PetItemEx) item).ctrlQuality.selectedIndex = skillUnit.SkillQuality-1;
        ((UI_PetItemEx) item).type.selectedIndex = 1;
        // ((UI_PetItemEx) item).img.url = UIResource.GetImageUrlWithLang("yizhuangbei","RoleMain");
        SkillInfo hasSkillInfo = SkillInfoManager.Instance.GetSkill(skillUnit.Id);
        if (hasSkillInfo != null)
        {
            ((UI_PetItemEx) item).disableCtrl.selectedIndex = 0;
            bool isUpload = SkillInfoManager.Instance.IsInUpload(hasSkillInfo);
            ((UI_PetItemEx) item).uploadCtrl.selectedIndex = isUpload ? 0 : 1;
            ConfigSkillLevelUnit skillLevelUnit = ConfigUtils.GetSkillLevelUnit(hasSkillInfo.SkillUnit.Id, hasSkillInfo.Level+1);
            ((UI_PetItemEx) item).lvLb.text = hasSkillInfo.Level.ToString();
            if (skillLevelUnit != null)
            {
                if (skillLevelUnit.CardNumber <= hasSkillInfo.CardNumber)
                {
                    ((UI_PetItemEx) item).levelUp.selectedIndex = 1;
                }
                else
                {
                    ((UI_PetItemEx) item).levelUp.selectedIndex = 0;
                }
    
                // ((UI_PetItemEx) item).numBar.maxCtrl.selectedIndex = 0;
                // ((UI_PetItemEx) item).numBar.min = 0;
                // ((UI_PetItemEx) item).numBar.max = skillLevelUnit.CardNumber;
                // ((UI_PetItemEx) item).numBar.value = hasSkillInfo.CardNumber;
                ((UI_PetItemEx) item).numBar2.maxCtrl.selectedIndex = 0;
                ((UI_PetItemEx) item).numBar2.min = 0;
                ((UI_PetItemEx) item).numBar2.max = skillLevelUnit.CardNumber;
                ((UI_PetItemEx) item).numBar2.value = hasSkillInfo.CardNumber;
            }
            else
            {
                ((UI_PetItemEx) item).levelUp.selectedIndex = 0;
                // ((UI_PetItemEx) item).numBar.maxCtrl.selectedIndex = 1;
                ((UI_PetItemEx) item).numBar2.maxCtrl.selectedIndex = 1;
            }
        }
        else
        {
            ConfigSkillLevelUnit noSkillLevelUnit = ConfigUtils.GetSkillLevelUnit(_skillInfos[index].SkillUnit.Id, 2);
            if (noSkillLevelUnit != null)
            {
                // ((UI_PetItemEx) item).numBar.min = 0;
                // ((UI_PetItemEx) item).numBar.max = noSkillLevelUnit.CardNumber;
                // ((UI_PetItemEx) item).numBar.value = _skillInfos[index].CardNumber;
                ((UI_PetItemEx) item).numBar2.min = 0;
                ((UI_PetItemEx) item).numBar2.max = noSkillLevelUnit.CardNumber;
                ((UI_PetItemEx) item).numBar2.value = _skillInfos[index].CardNumber;
            }
            ((UI_PetItemEx) item).disableCtrl.selectedIndex = 1;
            ((UI_PetItemEx) item).levelUp.selectedIndex = 0;
            // ((UI_PetItemEx) item).numBar.maxCtrl.selectedIndex = 0;
            ((UI_PetItemEx) item).numBar2.maxCtrl.selectedIndex = 0;
        }
        
        ((UI_PetItemEx) item).data = _skillInfos[index];
        ((UI_PetItemEx) item).onClick.Set(this.OnClickSkillItem);
    }
    
    private void OnClickSkillItem(EventContext context)
    {
        SkillInfo skill = (context.sender as UI_PetItemEx)?.data as SkillInfo;
        if(skill != null)
            UIManager.Instance.ShowUIPanel("SkillDetail", skill);
    }
    
    private void OnClickUpLoadSkillListItem(EventContext context)
    {
        UI_SkillItem item = context.sender as UI_SkillItem;
        int index = int.Parse(item.name.Substring(9, 1));
        if (item.showHandCtrl.selectedIndex == 1)
        {
            // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(23);
            var builder = SkillInSlot_CS.CreateBuilder();
            builder.SkillId = (uint) _uploadSkill.SkillUnit.Id;
            builder.SkillSlotId = (uint)index;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_SkillInSlot_CS,builder.Build());
        }
        else
        {
            if (item != null && item.hasCtrl.selectedIndex == 0)
            {
                if (this._upLoadSkillDict.TryGetValue(index, out var skill))
                {
                    //跳转技能详细界面
                    UIManager.Instance.ShowUIPanel("SkillDetail", skill);
                }
            } 
            else
            {
                //功能开启枚举 = 索引+1901
                var stateMap =  FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType) (index+1901));
                if(!stateMap.Item1)
                    UIManager.Instance.Toast(stateMap.Item2);
                else
                    OnClickToAddSkill();
            }
        }
        
    }
    
    private void OnClickToAddSkill()
    {
        UIManager.Instance.ToastByKey(10181);
    }
    private void RefrehUpLoadSkillItem()
    {
        GameManager.Instance.TimerManager.ClearTimer(RefreshUpLoadSkills);
        GameManager.Instance.TimerManager.SetTimer(0.1f, RefreshUpLoadSkills);
        
        double skilladd = 0;
    
        foreach (var item in SkillInfoManager.Instance.GetAllHaveSkillList())
        {
            skilladd += item.OwnerAtkValue;
        }
        
        foreach (var item in SkillInfoManager.Instance.GetBattleSkillList())
        {
            skilladd += item.CarryAtkValue;
        }
        
        _skillInfoUI.atkLb.SetVar("value", StringUtils.FormatCurrency(skilladd * ConstDefine.CONFIG_PLACE)).FlushVars();
        
    }
    
    private void RefreshUpLoadSkills()
    {
        foreach (var item in _skillUpLoadItemDict)
        {
            if (_upLoadSkillDict.TryGetValue(item.Key, out var skill))
            {
                this.SetUpLoadSkillItemData(item.Value, skill, item.Key);
            }
            else
            {
                var stateMap = SkillInfoManager.Instance.UnLockSkillPos > item.Key;
                if (!stateMap)
                {
                    item.Value.hasCtrl.selectedIndex = 2;
                }
                else
                {
                    item.Value.hasCtrl.selectedIndex = 1;
                }
            }
    
        }
        
        //红点
        _skillInfoUI.redDot.visible = SkillInfoManager.Instance.IsCanUpLevel();
        RefreshBottomRedDot();
    }
    private void SkillUpListRender(int index, GObject item)
    {
        UI_SkillItem btn = (UI_SkillItem) item;
        if (this._upLoadSkillDict.TryGetValue(index, out var skill))
        {
            SetUpLoadSkillItemData(btn, skill, index);
        }
    }
    
    private void SetUpLoadSkillItemData(UI_SkillItem item,SkillInfo skill, int index)
    {
        skill.BattleIndex = index;
        item.icon.priority = true;
        item.icon.url = UIResource.GetItemUrl(skill.SkillUnit.SkillIcon);
        item.hasCtrl.selectedIndex = 0;
        item.ctrlQuality.selectedIndex = skill.SkillUnit.SkillQuality - 1;
    }
    
    private void UpdateUnlockSkillPos()
    {
        RefreshUpLoadSkills();
    }
    
    private void HideUploadSkillView()
    {
        this.roleUI.showPetHand.selectedIndex = 0;
        foreach (var item in _skillUpLoadItemDict)
        {
            item.Value.showHandCtrl.selectedIndex = 0;
        }
    }
    
    private void ShowUploadSkillOpt(SkillInfo skill)
    {
        _uploadSkill = skill;
        this.roleUI.showPetHand.selectedIndex = 1;
        this.roleUI.hideUpload1.y = _skillInfoUI.skillItemBg.y;
        this.roleUI.hideUpload2.height = _skillInfoUI.rlLoader.y;
        foreach (var item in _skillUpLoadItemDict)
        {
            item.Value.showHandCtrl.selectedIndex = 1;
        }
    }
    
    private void UpdateSkillSCSuccsss()
    {
        HideUploadSkillView();
        _upLoadSkillDict = SkillInfoManager.Instance.GetUpLoadSkill();
        _skillInfoUI.SkillAllList.numItems = _skillInfos.Count;
        RefrehUpLoadSkillItem();
    }
    
    private void OnClickAllSkillStrengthBtn()
    {
        var builder = AllSkillLevelUp_CS.CreateBuilder();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AllSkillLevelUp_CS, builder.Build());
        
    }
    
    private void UpdateSkillLevelInfo()
    {
        _upLoadSkillDict = SkillInfoManager.Instance.GetUpLoadSkill(true);
        _skillInfoUI.SkillAllList.numItems = _skillInfos.Count;
    
        bool isCanLevelUp = false;
        foreach (var item in SkillInfoManager.Instance.GetAllHaveSkillList())
        {
            ConfigSkillLevelUnit skillLevelUnit =
                ConfigUtils.GetSkillLevelUnit(item.SkillUnit.Id, item.Level+1);
            if (skillLevelUnit != null)
            {
                if (skillLevelUnit.CardNumber <= item.CardNumber)
                {
                    isCanLevelUp = true;
                    break;
                }
            }
        }
    
        _skillInfoUI.allStrengthBtn.enabled = isCanLevelUp;
        
        RefrehUpLoadSkillItem();
        RefreshBottomRedDot();
        //引导-点击第一个技能
        if(RoleManager.Instance.GetSkillSlotInfoCount() <= 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_UIEquipIntensifyClose,
            bid = GuideID.Click_SkillIntensify,
            gid = GuideID.Click_FirstSkillItem,
            tui = _skillInfoUI.SkillAllList.GetChildAt(0),
            isForce = true,
            isSend = true,
            npcTxt = "Beginner_Doc_011",
            npcPosType = PosType.Down,
            isLucency = false
        })){ _skillInfoUI.SkillAllList.EnsureBoundsCorrect(); }
    }

    #endregion

    #region 符石

    private void UpdateRuneInfo()
    {
        HideUploadRuneView();
        _runeInfos = RuneInfoManager.Instance.GetAllRuneInfoList();
        _runeInfos.Sort((a, b)=>
        {
            ConfigSkillUnit skillUnitA = ConfigUtils.GetSkillById(a.SkillId);
            ConfigSkillUnit skillUnitB = ConfigUtils.GetSkillById(b.SkillId);
            int result = skillUnitA.Id > skillUnitB.Id ? -1 : (skillUnitA.Id == skillUnitB.Id ? 0 : 1);
            if(result == 0)
                result = skillUnitA.SkillQuality > skillUnitB.SkillQuality ? 1 : (skillUnitA.SkillQuality==skillUnitB.SkillQuality ? 0 : -1);
            if(result == 0)
                result = a.MagicTimes > b.MagicTimes ? -1 : (a.MagicTimes == b.MagicTimes ? 0 :1);
            if (result == 0)
                result = a.Guid > b.Guid ? 1 : -1;
            return result;
        });
        _upLoadRuneDict = RuneInfoManager.Instance.GetUpLoadRune();
        //_runeInfoUI.runeAllList.numItems = _runeInfos.Count;
        UpdateUnlockRunePos();
        RefrehUpLoadRuneItem();
    }
    
    private void UpdateRuneList()
    {
        if (IsShow() && IsOnStage())
        {
            _runeInfos = RuneInfoManager.Instance.GetAllRuneInfoList();
            _runeInfos.Sort((a, b)=>
            {
                ConfigSkillUnit skillUnitA = ConfigUtils.GetSkillById(a.SkillId);
                ConfigSkillUnit skillUnitB = ConfigUtils.GetSkillById(b.SkillId);
                int result = skillUnitA.Id > skillUnitB.Id ? -1 : (skillUnitA.Id == skillUnitB.Id ? 0 : 1);
                if(result == 0)
                    result = skillUnitA.SkillQuality > skillUnitB.SkillQuality ? 1 : (skillUnitA.SkillQuality==skillUnitB.SkillQuality ? 0 : -1);
                if(result == 0)
                    result = a.MagicTimes > b.MagicTimes ? -1 : (a.MagicTimes == b.MagicTimes ? 0 :1);
                if (result == 0)
                    result = a.Guid > b.Guid ? 1 : -1;
                return result;
            });
            //_runeInfoUI.runeAllList.numItems = _runeInfos.Count; 
        }

    }
    
    private void RuneDetailItemRender(int index, GObject item)
    {
        if (_upLoadRuneDict.TryGetValue(index, out var rune))
        {
            ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(rune.SkillId);
            ((UI_RuneDetailItem) item).ctrl.selectedIndex = 0;
            ((UI_RuneDetailItem) item).quality.selectedIndex = rune.Quality - 1;
            // ((UI_RuneDetailItem) item).runeName.text = rune.ItemTypeUnit.Name;
            ((UI_RuneDetailItem) item).runeName.text = ConfigUtils.GetTextById(rune.ItemTypeUnit.Name);
            // ((UI_RuneDetailItem) item).runeDesc.SetVar("skillName", skillUnit.Name).SetVar("value", rune.MagicTimes.ToString()).FlushVars();
            ((UI_RuneDetailItem) item).runeDesc.SetVar("skillName", ConfigUtils.GetTextById(skillUnit.Name)).SetVar("value", rune.MagicTimes.ToString()).FlushVars();
        }
        else
        {
            ((UI_RuneDetailItem) item).ctrl.selectedIndex = 1;
        }
    
    }
    
    private void RuneAllItemRender(int index, GObject item)
    {
        RuneInfo runeInfo = _runeInfos[index];
        ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(runeInfo.SkillId);
        ((UI_RuneItem) item).countLb.text = runeInfo.MagicTimes.ToString();
        ((UI_RuneItem) item).countCtrl.selectedIndex = 0;
        ((UI_RuneItem) item).ctrlQuality.selectedIndex = skillUnit.SkillQuality - 1;
        ((UI_RuneItem) item).bgiCtrl.selectedIndex = 0;
        ((UI_RuneItem) item).icon.url = UIResource.GetItemUrl(runeInfo.ItemTypeUnit.Icon);
        bool isUpload = RuneInfoManager.Instance.IsInUpload(runeInfo);
        ((UI_RuneItem) item).uploadCtrl.selectedIndex = isUpload ? 0 : 1;
        ((UI_RuneItem) item).redDot.visible = RuneInfoManager.Instance.OneHigherSkillTimes(runeInfo) && !isUpload;
        ((UI_RuneItem) item).data = _runeInfos[index];
        ((UI_RuneItem) item).onClick.Set(this.OnClickRuneItem);
    }
    
    private void OnClickRuneItem(EventContext context)
    {
        RuneInfo rune = (context.sender as UI_RuneItem)?.data as RuneInfo;
        if(rune != null)
            UIManager.Instance.ShowUIPanel("RuneDetail", rune);
    }
    
    private void OnClickUpLoadRuneListItem(EventContext context)
    {
        UI_RuneItem item = context.sender as UI_RuneItem;
        int index = int.Parse(item.name.Substring(8, 1));
        if (item.showHandCtrl.selectedIndex == 1)
        {
            RuneSlotInfo slotInfo = RoleManager.Instance.GetRuneSlotInfoBySlotId(index);
            if (RuneInfoManager.Instance.HasSameRuneInfo(_uploadRune) && slotInfo.ItemId != _uploadRune.ItemId)
            {
                HideUploadRuneView();
                UIManager.Instance.ToastByKey(10190);
                return;
            }
            // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(18);
            var builder = RuneInSlot_CS.CreateBuilder();
            builder.RuneGuid = _uploadRune.Guid;
            builder.SlotId = (uint)index;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RuneInSlot_CS, builder.Build());
        }
        else
        {
            if (item != null && item.hasCtrl.selectedIndex == 0)
            {
                if (this._upLoadRuneDict.TryGetValue(index, out var rune))
                {
                    //跳转符石详细界面
                    UIManager.Instance.ShowUIPanel("RuneDetail", rune);
                }
            } 
            else
            {
                //功能开启枚举 = 索引+2401
                var stateMap =  FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)(index+2401));
                if(!stateMap.Item1)
                    UIManager.Instance.Toast(stateMap.Item2);
                else
                    OnClickToAddRune();
            }
        }
        
    }
    
    private void OnClickToAddRune()
    {
        UIManager.Instance.ToastByKey(10182);
    }
    private void RefrehUpLoadRuneItem()
    {
        GameManager.Instance.TimerManager.ClearTimer(RefreshUpLoadRunes);
        GameManager.Instance.TimerManager.SetTimer(0.1f, RefreshUpLoadRunes);
    }
    
    private void RefreshUpLoadRunes()
    {
        foreach (var item in _runeUpLoadItemDict)
        {
            if (_upLoadRuneDict.TryGetValue(item.Key, out var rune))
            {
                this.SetUpLoadRuneItemData(item.Value, rune, item.Key);
            }
            else
            {
                var stateMap = RuneInfoManager.Instance.UnLockRunePos > item.Key;
                if (!stateMap)
                {
                    item.Value.hasCtrl.selectedIndex = 2;
                    item.Value.redDot.visible = false;
                }
                else
                {
                    item.Value.hasCtrl.selectedIndex = 1;
                    item.Value.redDot.visible = true;
                }
            }
    
        }
    
        //_runeInfoUI.runDetailList.numItems = 6;
        RefreshBottomRedDot();
    }
    private void RuneUpListRender(int index, GObject item)
    {
        UI_RuneItem btn = (UI_RuneItem) item;
        if (this._upLoadRuneDict.TryGetValue(index, out var rune))
        {
            SetUpLoadRuneItemData(btn, rune, index);
        }
    }
    
    private void SetUpLoadRuneItemData(UI_RuneItem item,RuneInfo rune, int index)
    {
        ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(rune.SkillId);
        rune.BattleIndex = index;
        item.hasCtrl.selectedIndex = 0;
        item.countLb.text = rune.MagicTimes.ToString();
        item.redDot.visible = false;
        item.icon.priority = true;
        item.icon.url = UIResource.GetItemUrl(rune.ItemTypeUnit.Icon);
        item.ctrlQuality.selectedIndex = skillUnit.SkillQuality - 1;
    }
    
    private void UpdateUnlockRunePos()
    {
        for (int i = 0; i < _runeUpLoadItemDict.Count; i++)
        {
            RuneUpListRender(i, _runeUpLoadItemDict[i]);
        }
    }
    
    private void HideUploadRuneView()
    {
        this.roleUI.showPetHand.selectedIndex = 0;
        foreach (var item in _runeUpLoadItemDict)
        {
            item.Value.showHandCtrl.selectedIndex = 0;
        }
    }
    
    private void ShowUploadRuneOpt(RuneInfo runeInfo)
    {
        _uploadRune = runeInfo;
        this.roleUI.showPetHand.selectedIndex = 1;
        this.roleUI.hideUpload1.y = _runeInfoUI.runeItemBg.y;
        this.roleUI.hideUpload2.height = _runeInfoUI.upGroup.y;
        bool hasRune = RuneInfoManager.Instance.HasSameRuneInfo(runeInfo);
        if (hasRune)
        {
            foreach (var item in _runeUpLoadItemDict)
            {
                if (_upLoadRuneDict.TryGetValue(item.Key, out var rune) && runeInfo.ItemId == rune.ItemId)
                {
                    item.Value.showHandCtrl.selectedIndex = 1;
                    break;
                }
            }
        }
        else
        {
            foreach (var item in _runeUpLoadItemDict)
            {
            
                item.Value.showHandCtrl.selectedIndex = 1;
            }
        }

    }
    
    private void UpdateRuneSCSuccsss()
    {
        HideUploadRuneView();
        _upLoadRuneDict = RuneInfoManager.Instance.GetUpLoadRune();
        //_runeInfoUI.runeAllList.numItems = _runeInfos.Count;
        RefrehUpLoadRuneItem();
    }
    
    private void OnClickAllRuneRecycleBtn()
    {
        UIManager.Instance.ShowUIPanel("RuneRecycle");
    }
    
    #endregion

    #region 天赋

    private void UpdateTalentInfo()
    {
        _unlockTalentDict = TalentInfoManager.Instance.GetTalentDict();

        _talentInfoUI.talentPoint.icon = UIResource.GetItemUrl(1010014.ToString());
        _talentInfoUI.talentPoint.txtValue.text = TalentInfoManager.Instance.GetTalentPoints().ToString();//玩家天赋点数量
        _talentInfoUI.upLvBtn.itemIcon.url = UIResource.GetItemUrl(1010014.ToString());
        _talentInfoUI.talentToken.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_common100006.Param1)).Icon);
        int talentToken = ItemInfoManager.Instance.GetItemCount(int.Parse(_common100006.Param1));//拥有的天赋令的数量,配置表更改后放开
        // long talentToken = DataManager.Instance.mRoleData.dia;// TODO 目前配置表配置的是砖石
        _talentInfoUI.talentToken.txtValue.text = talentToken.ToString();
        _talentInfoUI.resetBtn.itemIcon.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_common100006.Param1)).Icon);
        _talentInfoUI.resetBtn.num.text = _common100006.Param2;
        _talentInfoUI.resetBtn.grayed = !(talentToken >= int.Parse(_common100006.Param2));
        
        OnAllTypeTalentUpdate(_phyTalentUnits, _talentInfoUI.phyTalent.talent);//物理
        OnAllTypeTalentUpdate(_spellTalentUnits, _talentInfoUI.spellTalent.talent);//法术
        OnAllTypeTalentUpdate(_defTalentUnits, _talentInfoUI.defTalent.talent);//防御
        // OnAllTypeTalentUpdate(_assistTalentUnits, _talentInfoUI.assistTalent);//辅助
        
        // 刷新当前选中的天赋项信息
        if (_currentTalentContainer != null)
        {
            foreach (var child in _currentTalentContainer.GetChildren())
            {
                if (child is UI_TalentItem item && (item.talentSmallItem.asButton.selected || item.talentMiddleItem.asButton.selected || item.talentBigItem.asButton.selected))
                {
                    // UpdateSelectedTalentDisplay(item.data as ConfigAptitudeUnlockUnit);
                    // break;
                    
                    ConfigAptitudeUnlockUnit unit = null;
                    if (item.talentSmallItem.asButton.selected)
                        unit = item.talentSmallItem.data as ConfigAptitudeUnlockUnit;
                    else if (item.talentMiddleItem.asButton.selected)
                        unit = item.talentMiddleItem.data as ConfigAptitudeUnlockUnit;
                    else if (item.talentBigItem.asButton.selected)
                        unit = item.talentBigItem.data as ConfigAptitudeUnlockUnit;

                    if (unit != null)
                        UpdateSelectedTalentDisplay(unit);
                    else
                        Debug.LogWarning("Selected talent item has no data bound.");

                    break;
                }
            }
        }
        
        // 平滑滚动到该天赋UI位置
        ScrollToTalentItem();
    }

    private void ScrollToTalentItem()
    {
        // 平滑滚动到该天赋UI位置
        if (_lastUpgradedTalentId != -1)
        {
            GameManager.Instance.TimerManager.SetTimer(0.1f, () =>
            {
                UI_TalentItem targetItem = FindTalentItemById(_lastUpgradedTalentId);
                if (targetItem != null)
                {
                    if (_currentTalentContainer.parent.scrollPane != null)
                    {
                        _currentTalentContainer.parent.scrollPane?.ScrollToView(targetItem, true, false);
                        
                        float viewHeight = _currentTalentContainer.parent.scrollPane.viewHeight;
                        float itemY = targetItem.y;
                        float targetPosY = itemY - (viewHeight - targetItem.height) / 2f;
                        
                        targetPosY = Mathf.Clamp(targetPosY, 0, _currentTalentContainer.parent.scrollPane.contentHeight - viewHeight);

                        // 设置 ScrollPane 的滚动位置
                        _currentTalentContainer.parent.scrollPane.posY = targetPosY;
                    }
                }
                
                _lastUpgradedTalentId = -1; // 用完清空
            });
        }
    }

    private UI_TalentItem FindTalentItemById(int talentId)
    {
        foreach (var child in _currentTalentContainer.GetChildren())
        {
            if (child is UI_TalentItem item)
            {
                if (item.data is ConfigAptitudeUnlockUnit unit && unit.BuffId == talentId)
                {
                    return item;
                }
            }
        }
        return null;
    }

    // 点击不同的天赋类型按钮
    private void OnClickTalentTypeItem(EventContext context)
    {
        GButton item = context.data as GButton;
        var index = _talentInfoUI.talentSelect.btnList.GetChildIndex(item);
        _talentInfoUI.talentSelect.btnList.selectedIndex = index;
        ChangeTalentIndex(index);
    }

    private void ChangeTalentIndex(int index)
    {
        
        bool isChanged = true;
        List<ConfigAptitudeUnlockUnit> currentUnits = null;
        switch (index)
        {
            case 0://物理天赋
                _talentInfoUI.phyTalent.scrollPane.ScrollBottom();
                _talentInfoUI.type.selectedIndex = 0;
                currentUnits = _phyTalentUnits;
                _currentTalentContainer = _talentInfoUI.phyTalent.talent;
                break;
            case 1://法术天赋
                _talentInfoUI.spellTalent.scrollPane.ScrollBottom();
                _talentInfoUI.type.selectedIndex = 1;
                currentUnits = _spellTalentUnits;
                _currentTalentContainer = _talentInfoUI.spellTalent.talent;
                break;
            case 2://防御天赋
                _talentInfoUI.defTalent.scrollPane.ScrollBottom();
                _talentInfoUI.type.selectedIndex = 2;
                currentUnits = _defTalentUnits;
                _currentTalentContainer = _talentInfoUI.defTalent.talent;
                break;
            // case 3://辅助天赋
            //     _talentInfoUI.type.selectedIndex = 3;
            //     currentUnits = _assistTalentUnits;
            //     _currentTalentContainer = _talentInfoUI.assistTalent;
            //     break;
        }
        
        // 默认选中当前类型的第一个天赋
        if (currentUnits?.Count > 0)
        {
            // 清除当前容器内所有项的选中状态
            ClearAllSelections();
            
            OnAllTypeTalentUpdate(currentUnits, _currentTalentContainer as UI_Talent);
            // 获取第一个天赋项的UI组件（假设命名规则为"talentItem1"）
            UI_TalentItem firstTalentItem = _currentTalentContainer.GetChild("talentItem1") as UI_TalentItem;
            if (firstTalentItem != null)
            {
                if (firstTalentItem != null)
                {
                    UpdateSelectedTalentDisplay(currentUnits[0]);
                    firstTalentItem.talentSmallItem.asButton.selected = true;
                }
            }
        }
    }
    
    private void UpdateSelectedTalentDisplay(ConfigAptitudeUnlockUnit unit)
    {
        if (_talentInfoUI.talent != null)
        {
            // _talentInfoUI.talent.icon = UIResource.GetTalentUrl(unit.Icon.ToString());
            // _talentInfoUI.talent.status.selectedIndex = 2;
            // _talentInfoUI.talent.Lv.visible = false;
            
            int maxLv = ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(unit.BuffId).Lv;//当前天赋满级等级
            int lv = _unlockTalentDict.ContainsKey(unit.BuffId) ? _unlockTalentDict[unit.BuffId] : 0;
            
            int index = unit.BuffId % 100;
            if (index == 1 || index == 2 || index == 3 || index == 6 || index == 7 || index == 8 || index == 9 || index == 15 || index == 16 || index == 17 || index == 18)
            {
                _talentInfoUI.talent.typeCtrl.selectedIndex = 0;
                _talentInfoUI.talent.talentSmallItem.icon = UIResource.GetTalentUrl(unit.Icon.ToString());
                _talentInfoUI.talent.talentSmallItem.status.selectedIndex = 2;
                _talentInfoUI.talent.talentSmallItem.Lv.SetVar("cur", lv.ToString()).SetVar("max", maxLv.ToString()).FlushVars();
            }
            
            if (index == 4 || index == 5 || index == 10 || index == 11 || index == 13 || index == 14 || index == 19 || index == 20)
            {
                _talentInfoUI.talent.typeCtrl.selectedIndex = 1;
                _talentInfoUI.talent.talentMiddleItem.icon = UIResource.GetTalentUrl(unit.Icon.ToString());
                _talentInfoUI.talent.talentMiddleItem.status.selectedIndex = 2;
                _talentInfoUI.talent.talentMiddleItem.Lv.SetVar("cur", lv.ToString()).SetVar("max", maxLv.ToString()).FlushVars();
            }
            
            if (index == 12 || index == 21 || index == 22)
            {
                _talentInfoUI.talent.typeCtrl.selectedIndex = 2;
                _talentInfoUI.talent.talentBigItem.icon = UIResource.GetTalentUrl(unit.Icon.ToString());
                _talentInfoUI.talent.talentBigItem.status.selectedIndex = 2;
                _talentInfoUI.talent.talentBigItem.Lv.SetVar("cur", lv.ToString()).SetVar("max", maxLv.ToString()).FlushVars();
            }
            
            _talentInfoUI.resetBtn.itemIcon.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_common100006.Param1)).Icon);
            _talentInfoUI.resetBtn.num.text = _common100006.Param2;
            
            _talentInfoUI.activateBtn.data = unit;
            _talentInfoUI.activateBtn.onClick.Set(this.OnClickUnlockTalent);
            _talentInfoUI.upLvBtn.data = unit;
            
            _unlockTalentDict = TalentInfoManager.Instance.GetTalentDict();
            
            _talentInfoUI.name.text = ConfigUtils.GetTextById(unit.Name);
            if (_unlockTalentDict.ContainsKey(unit.BuffId) &&  _unlockTalentDict[unit.BuffId] != 0)
            {
                var talent = ConfigUtils.GetAptitudeUnitByAptitudeIdAndLv(unit.BuffId,_unlockTalentDict[unit.BuffId]);
                // var skillAchieveUnit = ConfigUtils.GetSkillAchieveById(talent.Id);
                // _talentInfoUI.desc.text = skillAchieveUnit.Doc;
                if (talent.Param == 1)
                {
                    string value = talent.CommonNotes1;
                    _talentInfoUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(talent.Doc), value);
                }

                if (talent.Param == 2)
                {
                    string value = (double.Parse(talent.CommonNotes1)/100).ToString("f2") + "%";
                    _talentInfoUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(talent.Doc), value);
                }
            }
            else
            {
                var talent = ConfigUtils.GetAptitudeUnitByAptitudeIdAndLv(unit.BuffId,1);
                // var skillAchieveUnit = ConfigUtils.GetSkillAchieveById(talent.Id);
                // _talentInfoUI.desc.text = skillAchieveUnit.Doc;
                if (talent.Param == 1)
                {
                    string value = talent.CommonNotes1;
                    _talentInfoUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(talent.Doc), value);
                }

                if (talent.Param == 2)
                {
                    string value = (double.Parse(talent.CommonNotes1)/100).ToString("f2") + "%";
                    _talentInfoUI.desc.text = StringUtils.Format(ConfigUtils.GetTextById(talent.Doc), value);
                }
            }
            
            string[] preIds = unit.PreId.Split('|');//前置天赋
            string[] preTalentLv = unit.Lv.Split('|');//前置天赋达到指定等级
            List<ItemData> preTalentInfo = new List<ItemData>();
            for (int i = 0; i < preIds.Length; i++)
            {
                ItemData itemData = new ItemData();
                itemData.id = int.Parse(preIds[i]);
                itemData.count = double.Parse(preTalentLv[i]); // 该前置天赋需要达到的等级
                preTalentInfo.Add(itemData);
            }
            
            bool allPreconditionsMet = true;
            // 检查所有前置天赋是否满足等级要求
            foreach (var preTalent in preTalentInfo)
            {
                // 1.已解锁该前置天赋 2.解锁等级达到要求
                if (!_unlockTalentDict.ContainsKey(preTalent.id) || 
                    _unlockTalentDict[preTalent.id] < preTalent.count)
                {
                    allPreconditionsMet = false;
                    break;
                }
            }
            
            // 可激活时按钮状态
            // string[] preIds = unit.PreId.Split('|');
            if (allPreconditionsMet)//preIds.All(id => _unlockTalentDict.ContainsKey(int.Parse(id))) && preIds.All(id => _unlockTalentDict[int.Parse(id)] == ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(int.Parse(id)).Lv)
            {
                _talentInfoUI.upCtrl.selectedIndex = 1;
                _talentInfoUI.activateBtn.grayed = false;
            }
            else
            {
                _talentInfoUI.upCtrl.selectedIndex = 1;
                _talentInfoUI.activateBtn.grayed = true;
            }
            
            // 激活未满级对应的升级按钮状态
            if (_unlockTalentDict.ContainsKey(unit.BuffId) && _unlockTalentDict[unit.BuffId] < ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(unit.BuffId).Lv)
            {
                int talentPoints = TalentInfoManager.Instance.GetTalentPoints();
                ConfigAptitudeUnit nextUnit = ConfigUtils.GetNextAptitudeUnitByAptitudeIdAndLv(unit.BuffId,_unlockTalentDict[unit.BuffId]);//下一等级天赋的unit
                int costTalentPoints = nextUnit == null ? 0 : nextUnit.Count.ToInt32();
                _talentInfoUI.upLvBtn.status.selectedIndex = talentPoints < costTalentPoints ? 1 : 0;
                _talentInfoUI.upLvBtn.num.text = costTalentPoints.ToString();
                _talentInfoUI.upCtrl.selectedIndex = 0;
                _talentInfoUI.upLvBtn.onClick.Set(this.OnClickUpLvBtn);
            }
            
            // 激活已满级按钮状态
            if (_unlockTalentDict.ContainsKey(unit.BuffId) && _unlockTalentDict[unit.BuffId] >= ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(unit.BuffId).Lv)
            {
                _talentInfoUI.upCtrl.selectedIndex = 2;
                _talentInfoUI.maxBtn.grayed = true;
            }
        }
    }
    
    // 全部类型的天赋 
    // 暂时注释掉，等测试完成后，恢复，删除另一个OnAllTypeTalentUpdate方法
    private void OnAllTypeTalentUpdate(List<ConfigAptitudeUnlockUnit> aptitudeUnlockUnits, UI_Talent talent)
    {
        // 最高等级消耗的天赋点为0，天赋等级从0开始
        int index = 1;
        foreach (var item in aptitudeUnlockUnits)
        {
            _unlockTalentDict = TalentInfoManager.Instance.GetTalentDict();
            UI_TalentItem talentItem = talent.GetChild("talentItem" + index) as UI_TalentItem;
            // talentItem.data = item; // 绑定数据
            // talentItem.onClick.Add(OnClickTalentItem); // 注册点击事件
            // talentItem.icon = UIResource.GetTalentUrl(item.Icon.ToString());
            // talentItem.Lv.text = _unlockTalentDict.ContainsKey(item.BuffId) ? _unlockTalentDict[item.BuffId].ToString() : "0";
            int maxLv = ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(item.BuffId).Lv;//当前天赋满级等级
            int lv = _unlockTalentDict.ContainsKey(item.BuffId) ? _unlockTalentDict[item.BuffId] : 0;
            
            talentItem.data = item;
            
            talentItem.talentSmallItem.data = item; // 绑定数据
            talentItem.talentSmallItem.onClick.Set(OnClickTalentItem); // 注册点击事件
            talentItem.talentSmallItem.icon = UIResource.GetTalentUrl(item.Icon.ToString());
            // talentItem.talentSmallItem.Lv.text = _unlockTalentDict.ContainsKey(item.BuffId) ? _unlockTalentDict[item.BuffId].ToString() : "0";
            talentItem.talentSmallItem.Lv.SetVar("cur", lv.ToString()).SetVar("max", maxLv.ToString()).FlushVars();
            
            talentItem.talentMiddleItem.data = item; // 绑定数据
            talentItem.talentMiddleItem.onClick.Set(OnClickTalentItem); // 注册点击事件
            talentItem.talentMiddleItem.icon = UIResource.GetTalentUrl(item.Icon.ToString());
            // talentItem.talentMiddleItem.Lv.text = _unlockTalentDict.ContainsKey(item.BuffId) ? _unlockTalentDict[item.BuffId].ToString() : "0";
            talentItem.talentMiddleItem.Lv.SetVar("cur", lv.ToString()).SetVar("max", maxLv.ToString()).FlushVars();
            
            talentItem.talentBigItem.data = item; // 绑定数据
            talentItem.talentBigItem.onClick.Set(OnClickTalentItem); // 注册点击事件
            talentItem.talentBigItem.icon = UIResource.GetTalentUrl(item.Icon.ToString());
            // talentItem.talentBigItem.Lv.text = _unlockTalentDict.ContainsKey(item.BuffId) ? _unlockTalentDict[item.BuffId].ToString() : "0";
            talentItem.talentBigItem.Lv.SetVar("cur", lv.ToString()).SetVar("max", maxLv.ToString()).FlushVars();
            
            string[] preIds = item.PreId.Split('|');//前置天赋
            string[] preTalentLv = item.Lv.Split('|');//前置天赋达到指定等级
            List<ItemData> preTalentInfo = new List<ItemData>();
            for (int i = 0; i < preIds.Length; i++)
            {
                ItemData itemData = new ItemData();
                itemData.id = int.Parse(preIds[i]);
                itemData.count = double.Parse(preTalentLv[i]); // 该前置天赋需要达到的等级
                preTalentInfo.Add(itemData);
            }
            
            bool allPreconditionsMet = true;
            // 检查所有前置天赋是否满足等级要求
            foreach (var preTalent in preTalentInfo)
            {
                // 1.已解锁该前置天赋 2.解锁等级达到要求
                if (!_unlockTalentDict.ContainsKey(preTalent.id) || 
                    _unlockTalentDict[preTalent.id] < preTalent.count)
                {
                    allPreconditionsMet = false;
                    break;
                }
            }
    
            // 天赋状态：未激活，可激活，已激活未满级，已激活已满级
            // 默认为未激活状态：第一个天赋默认激活，其他都未激活
            if (item.PreId == "0")
            {
                // talentItem.status.selectedIndex = 2;
                
                talentItem.talentSmallItem.status.selectedIndex = 2;
                
                // UI_talentBar4 talentBar4 = talent.GetChild("bar1") as UI_talentBar4;
                // talentBar4.status.selectedIndex = (_unlockTalentDict.ContainsKey(item.BuffId) && _unlockTalentDict[item.BuffId] == maxLv) ? 1 : 0;
            }
    
            // 未激活
            if (!_unlockTalentDict.ContainsKey(item.BuffId))
            {
                // talentItem.status.selectedIndex = 0;
                
                talentItem.talentSmallItem.status.selectedIndex = 0;
                talentItem.talentMiddleItem.status.selectedIndex = 0;
                talentItem.talentBigItem.status.selectedIndex = 0;
            }
            
            // if (item.PreId != "0" && preIds.All(id => _unlockTalentDict.ContainsKey(int.Parse(id))) && preIds.All(id => _unlockTalentDict[int.Parse(id)] == ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(int.Parse(id)).Lv))//新：前置天赋已解锁且达到指定等级   //旧：前置天赋已解锁且为满级:_unlockTalentDict[int.Parse(id)] == ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(int.Parse(id)).Lv)
            // {
            //     talentItem.status.selectedIndex = 1;
            // }
            if (item.PreId != "0" && preTalentInfo.All(talent => _unlockTalentDict.ContainsKey(talent.id)) && preTalentInfo.All(talent => _unlockTalentDict[talent.id] >= talent.count))//新：前置天赋已解锁且达到指定等级   //旧：前置天赋已解锁且为满级:_unlockTalentDict[int.Parse(id)] == ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(int.Parse(id)).Lv)
            {
                // talentItem.status.selectedIndex = 1;
                
                talentItem.talentSmallItem.status.selectedIndex = 1;
                talentItem.talentMiddleItem.status.selectedIndex = 1;
                talentItem.talentBigItem.status.selectedIndex = 1;
            }
            
            // 已激活未满级
            if (_unlockTalentDict.ContainsKey(item.BuffId) && _unlockTalentDict[item.BuffId] < maxLv)
            {
                // talentItem.status.selectedIndex = 2;
                
                talentItem.talentSmallItem.status.selectedIndex = 2;
                talentItem.talentMiddleItem.status.selectedIndex = 2;
                talentItem.talentBigItem.status.selectedIndex = 2;
            }
            
            // 已激活已满级
            if (_unlockTalentDict.ContainsKey(item.BuffId) && _unlockTalentDict[item.BuffId] >= maxLv)
            {
                // talentItem.status.selectedIndex = 3;
                
                talentItem.talentSmallItem.status.selectedIndex = 3;
                talentItem.talentMiddleItem.status.selectedIndex = 3;
                talentItem.talentBigItem.status.selectedIndex = 3;
            }
            
            // index++;
            
            // 设置进度条状态
            if (index == 2 || index == 3)
            {
                UI_talentBar4 talentBar4 = talent.GetChild("bar1") as UI_talentBar4;
                // talentBar5.status.selectedIndex = (_unlockTalentDict.ContainsKey(item.BuffId) && _unlockTalentDict[item.BuffId] >= preTalentInfo[0].count) ? 1 : 0;//达到指定等级  //_unlockTalentDict[item.BuffId] == maxLv
                talentBar4.status.selectedIndex = allPreconditionsMet ? 1 : 0;
            }
    
            if (index == 4 || index == 5)
            {
                UI_talentBar5 talentBar5 = talent.GetChild("bar2") as UI_talentBar5;
                // talentBar6.status.selectedIndex = (_unlockTalentDict.ContainsKey(item.BuffId) && _unlockTalentDict[item.BuffId] == maxLv) ? 1 : 0;
                talentBar5.status.selectedIndex = allPreconditionsMet ? 1 : 0;
            }
    
            if (index == 13 || index == 14)
            {
                UI_talentBar5 talentBar5 = talent.GetChild("bar3") as UI_talentBar5;
                talentBar5.status.selectedIndex = allPreconditionsMet ? 1 : 0;
            }
    
            if (index == 6 || index == 7)
            {
                UI_talentBar6 talentBar6 = talent.GetChild("bar4") as UI_talentBar6;
                talentBar6.status.selectedIndex = allPreconditionsMet ? 1 : 0;
            }
    
            if (index == 8 || index == 9)
            {
                UI_talentBar7 talentBar6 = talent.GetChild("bar5") as UI_talentBar7;
                talentBar6.status.selectedIndex = allPreconditionsMet ? 1 : 0;
            }
    
            if (index == 15 || index == 16)
            {
                UI_talentBar7 talentBar6 = talent.GetChild("bar13") as UI_talentBar7;
                talentBar6.status.selectedIndex = allPreconditionsMet ? 1 : 0;
            }
            
            if (index == 17 || index == 18)
            {
                UI_talentBar6 talentBar6 = talent.GetChild("bar14") as UI_talentBar6;
                talentBar6.status.selectedIndex = allPreconditionsMet ? 1 : 0;
            }
    
            if (index == 10)
            {
                bool flag1 = false;
                bool flag2 = false;
                if (preTalentInfo.Count < 2)  continue;//后续可删除
                
                var info1 = preTalentInfo[0];
                if (_unlockTalentDict.ContainsKey(info1.id) && _unlockTalentDict[info1.id] >= info1.count)
                {
                    flag1 = true;
                }
                var info2 = preTalentInfo[1];
                if (_unlockTalentDict.ContainsKey(info2.id) && _unlockTalentDict[info2.id] >= info2.count)
                {
                    flag2 = true;
                }
                
                UI_talentBar8 talentBar3 = talent.GetChild("bar6") as UI_talentBar8;
                talentBar3.status.selectedIndex = flag1 ? 1 : 0;
                UI_talentBar3 talentBar = talent.GetChild("bar7") as UI_talentBar3;
                talentBar.status.selectedIndex = flag2 ? 1 : 0;

                if (allPreconditionsMet)
                {
                    UI_talentBar9 talentBar9 = talent.GetChild("barx1") as UI_talentBar9;
                    talentBar9.status.selectedIndex = allPreconditionsMet ? 1 : 0;
                }
            }
            
            if (index == 11)
            {
                bool flag1 = false;
                bool flag2 = false;
                if (preTalentInfo.Count < 2)  continue;//后续可删除
                
                var info1 = preTalentInfo[0];
                if (_unlockTalentDict.ContainsKey(info1.id) && _unlockTalentDict[info1.id] >= info1.count)
                {
                    flag1 = true;
                }
                var info2 = preTalentInfo[1];
                if (_unlockTalentDict.ContainsKey(info2.id) && _unlockTalentDict[info2.id] >= info2.count)
                {
                    flag2 = true;
                }
                
                UI_talentBar3 talentBar3 = talent.GetChild("bar8") as UI_talentBar3;
                talentBar3.status.selectedIndex = flag1 ? 1 : 0;
                UI_talentBar8 talentBar = talent.GetChild("bar9") as UI_talentBar8;
                talentBar.status.selectedIndex = flag2 ? 1 : 0;
                
                if (allPreconditionsMet)
                {
                    UI_talentBar8 talentBar9 = talent.GetChild("bary1") as UI_talentBar8;
                    talentBar9.status.selectedIndex = allPreconditionsMet ? 1 : 0;
                }
            }
            
            if (index == 19)
            {
                bool flag1 = false;
                bool flag2 = false;
                if (preTalentInfo.Count < 2)  continue;//后续可删除
                
                var info1 = preTalentInfo[0];
                if (_unlockTalentDict.ContainsKey(info1.id) && _unlockTalentDict[info1.id] >= info1.count)
                {
                    flag1 = true;
                }
                var info2 = preTalentInfo[1];
                if (_unlockTalentDict.ContainsKey(info2.id) && _unlockTalentDict[info2.id] >= info2.count)
                {
                    flag2 = true;
                }
                
                UI_talentBar8 talentBar3 = talent.GetChild("bar15") as UI_talentBar8;
                talentBar3.status.selectedIndex = flag1 ? 1 : 0;
                UI_talentBar3 talentBar = talent.GetChild("bar16") as UI_talentBar3;
                talentBar.status.selectedIndex = flag2 ? 1 : 0;
                
                if (allPreconditionsMet)
                {
                    UI_talentBar8 talentBar9 = talent.GetChild("bary2") as UI_talentBar8;
                    talentBar9.status.selectedIndex = allPreconditionsMet ? 1 : 0;
                }
            }
            
            if (index == 20)
            {
                bool flag1 = false;
                bool flag2 = false;
                if (preTalentInfo.Count < 2)  continue;//后续可删除
                
                var info1 = preTalentInfo[0];
                if (_unlockTalentDict.ContainsKey(info1.id) && _unlockTalentDict[info1.id] >= info1.count)
                {
                    flag1 = true;
                }
                var info2 = preTalentInfo[1];
                if (_unlockTalentDict.ContainsKey(info2.id) && _unlockTalentDict[info2.id] >= info2.count)
                {
                    flag2 = true;
                }
                
                UI_talentBar3 talentBar3 = talent.GetChild("bar17") as UI_talentBar3;
                talentBar3.status.selectedIndex = flag1 ? 1 : 0;
                UI_talentBar8 talentBar = talent.GetChild("bar18") as UI_talentBar8;
                talentBar.status.selectedIndex = flag2 ? 1 : 0;
                
                if (allPreconditionsMet)
                {
                    UI_talentBar9 talentBar9 = talent.GetChild("barx2") as UI_talentBar9;
                    talentBar9.status.selectedIndex = allPreconditionsMet ? 1 : 0;
                }
            }
            
            if (index == 12)
            {
                bool flag1 = false;
                bool flag2 = false;
                if (preTalentInfo.Count < 2)  continue;//后续可删除
                
                var info1 = preTalentInfo[0];
                if (_unlockTalentDict.ContainsKey(info1.id) && _unlockTalentDict[info1.id] >= info1.count)
                {
                    flag1 = true;
                }
                var info2 = preTalentInfo[1];
                if (_unlockTalentDict.ContainsKey(info2.id) && _unlockTalentDict[info2.id] >= info2.count)
                {
                    flag2 = true;
                }
                
                UI_talentBar2 talentBar2 = talent.GetChild("bar10") as UI_talentBar2;
                talentBar2.status.selectedIndex = flag1 ? 1 : 0;
                UI_talentBar2 talentBar = talent.GetChild("bar11") as UI_talentBar2;
                talentBar.status.selectedIndex = flag2 ? 1 : 0;
            }
            
            if (index == 21)
            {
                bool flag1 = false;
                bool flag2 = false;
                if (preTalentInfo.Count < 2)  continue;//后续可删除
                
                var info1 = preTalentInfo[0];
                if (_unlockTalentDict.ContainsKey(info1.id) && _unlockTalentDict[info1.id] >= info1.count)
                {
                    flag1 = true;
                }
                var info2 = preTalentInfo[1];
                if (_unlockTalentDict.ContainsKey(info2.id) && _unlockTalentDict[info2.id] >= info2.count)
                {
                    flag2 = true;
                }
                
                UI_talentBar2 talentBar2 = talent.GetChild("bar19") as UI_talentBar2;
                talentBar2.status.selectedIndex = flag1 ? 1 : 0;
                UI_talentBar2 talentBar = talent.GetChild("bar20") as UI_talentBar2;
                talentBar.status.selectedIndex = flag2 ? 1 : 0;
            }
            
            if (index == 22)
            {
                bool flag1 = false;
                bool flag2 = false;
                if (preTalentInfo.Count < 2)  continue;//后续可删除
                
                var info1 = preTalentInfo[0];
                if (_unlockTalentDict.ContainsKey(info1.id) && _unlockTalentDict[info1.id] >= info1.count)
                {
                    flag1 = true;
                }
                var info2 = preTalentInfo[1];
                if (_unlockTalentDict.ContainsKey(info2.id) && _unlockTalentDict[info2.id] >= info2.count)
                {
                    flag2 = true;
                }
                
                UI_talentBar1 talentBar1 = talent.GetChild("bar12") as UI_talentBar1;
                talentBar1.status.selectedIndex = flag1 ? 1 : 0;
                UI_talentBar1 talentBar = talent.GetChild("bar21") as UI_talentBar1;
                talentBar.status.selectedIndex = flag2 ? 1 : 0;
            }
            
            // // 已激活未满级
            // if (_unlockTalentDict.ContainsKey(item.BuffId) && _unlockTalentDict[item.BuffId] < maxLv)
            // {
            //     talentItem.status.selectedIndex = 2;
            // }
            //
            // // 已激活已满级
            // if (_unlockTalentDict.ContainsKey(item.BuffId) && _unlockTalentDict[item.BuffId] == maxLv)
            // {
            //     talentItem.status.selectedIndex = 3;
            // }
            
            // if (index == aptitudeUnlockUnits.Count)
            // {
            //     break;
            // }
            index++;
        }
        
    }
    
    private void OnClickTalentItem(EventContext context)
    {
        GButton btn = context.sender as GButton;
        if (btn?.data is ConfigAptitudeUnlockUnit unit)
        {
            UpdateSelectedTalentDisplay(unit);
        
            // 清除旧选中状态
            ClearAllSelections();
            btn.selected = true;
        }
    }
    
    private void ClearAllSelections()
    {
        // 清理当前容器内所有选中状态
        foreach (var child in _currentTalentContainer.GetChildren())
        {
            if (child is UI_TalentItem item)
            {
                // item.asButton.selected = false;
                
                item.talentSmallItem.asButton.selected = false;
                item.talentMiddleItem.asButton.selected = false;
                item.talentBigItem.asButton.selected = false;
            }
        }
    }

    // 升级选中的天赋
    private void OnClickUpLvBtn(EventContext context)
    {
        ConfigAptitudeUnlockUnit unit = (context.sender as GButton).data as ConfigAptitudeUnlockUnit;
        _lastUpgradedTalentId = unit.BuffId;
        int buffId = unit.BuffId;
        int curLv = _unlockTalentDict[buffId];
        ConfigAptitudeUnit nextUnit = ConfigUtils.GetNextAptitudeUnitByAptitudeIdAndLv(buffId, curLv);
        int costTalentPoints = nextUnit == null ? 0 : nextUnit.Count.ToInt32();
        if (TalentInfoManager.Instance.GetTalentPoints() >= costTalentPoints)
        {
            // 创建主 Builder
            var mainBuilder = BatchTalentsLevelUp_CS.CreateBuilder();
    
            // 创建 TalentLevelUp.Builder
            var talentBuilder = TalentLevelUp.CreateBuilder();
            talentBuilder.SetTalentId((uint)unit.BuffId);
            talentBuilder.SetLevelUpValue(1);
    
            // 添加条目到列表
            mainBuilder.AddTalentLevelUp(talentBuilder.Build());
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_BatchTalentsLevelUp_CS, mainBuilder.Build());
        }
        else
        {
            // UIManager.Instance.ToastByKey(8037);
            UIManager.Instance.Toast("天赋令不足");
        }
    }

    public void OnClickResetTalent()
    {
        // if (DataManager.Instance.mRoleData.dia < int.Parse(_common100006.Param2))
        // {
        //     UIManager.Instance.ToastByKey(8041);
        //     return;
        // }
        
        if (ItemInfoManager.Instance.GetItemCount(int.Parse(_common100006.Param1)) < int.Parse(_common100006.Param2))
        {
            // UIManager.Instance.ToastByKey(8041);
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_common100006.Param1)).Name)));
            return;
        }
        
        _lastUpgradedTalentId = 0; // 重置
        
        // 提示框
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        {
            OkCallBack = () =>
            {
                if (ItemInfoManager.Instance.GetItemCount(int.Parse(_common100006.Param1)) >=
                    int.Parse(_common100006.Param2))
                {
                    var builder = ResetTalents_CS.CreateBuilder();
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ResetTalents_CS, builder.Build());
                }
                else
                {
                    UIManager.Instance.ToastByKey(5001);
                }
                
                // TODO 目前消耗的是钻石，上面消耗的是天赋令
                // if (DataManager.Instance.mRoleData.dia >=
                //     int.Parse(_common100006.Param2))
                // {
                //     var builder = ResetTalents_CS.CreateBuilder();
                //     GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ResetTalents_CS, builder.Build());
                // }
                // else
                // {
                //     UIManager.Instance.ToastByKey(8041);
                // }
            }
        };
        UIManager.Instance.ShowUIPanel("MessageBox",ConfigUtils.GetStringByKey(8036),param,true);
    }

    // 激活选中的天赋
    public void OnClickUnlockTalent(EventContext context)
    {
        ConfigAptitudeUnlockUnit unit = (context.sender as GButton).data as ConfigAptitudeUnlockUnit;
        _lastUpgradedTalentId = unit.BuffId;
        string[] preIds = unit.PreId.Split('|');
        string[] preLvs = unit.Lv.Split('|');
        List<ItemData> preTalentInfo = new List<ItemData>();
        for (int i = 0; i < preIds.Length; i++)
        {
            ItemData itemData = new ItemData();
            itemData.id = int.Parse(preIds[i]);
            itemData.count = double.Parse(preLvs[i]);// 该前置天赋需要达到的等级
            preTalentInfo.Add(itemData);
        }
        bool allPreconditionsMet = true;
        // 检查所有前置天赋是否满足等级要求
        foreach (var preTalent in preTalentInfo)
        {
            // 1.已解锁该前置天赋 2.解锁等级达到要求
            if (!_unlockTalentDict.ContainsKey(preTalent.id) || 
                _unlockTalentDict[preTalent.id] < preTalent.count)
            {
                allPreconditionsMet = false;
                break;
            }
        }

        if (allPreconditionsMet)
        {
            var builder = UnlockTalent_CS.CreateBuilder();
            builder.AddTalentIds((uint)unit.BuffId);
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_UnlockTalent_CS, builder.Build());
        }
        else
        {
            UIManager.Instance.ToastByKey(8043);
            ScrollToTalentItem();
        }
        
        // if (preIds.All(id => _unlockTalentDict.ContainsKey(int.Parse(id))) && preIds.All(id => _unlockTalentDict[int.Parse(id)] >= ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(int.Parse(id)).Lv))
        // {
        //     var builder = UnlockTalent_CS.CreateBuilder();
        //     builder.AddTalentIds((uint)unit.BuffId);
        //     GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_UnlockTalent_CS, builder.Build());
        // }
        // else
        // {
        //     UIManager.Instance.ToastByKey(8043);
        // }
        
    }
    
    #endregion

    #region 圣物
    
    private void UpdateHolyInfo()
    {
        _holyInfoUI.currency1.icon = UIResource.GetItemUrl(1010015.ToString());//碎片
        _holyInfoUI.currency1.txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(1010015));
        _holyInfoUI.currency2.icon = UIResource.GetItemUrl(1000.ToString());//砖石
        _holyInfoUI.currency2.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia);

        _selectedHolySoltIndex = -1;
        _isReplaceMode = false; // 重置替换模式
        _isReplace2 = false;
        isUIPanelOpen = true;
        UpdateAndSortHolyItemList();
        
        // 默认选中圣物列表中的第一个圣物
        holyIndex = 0;
        _holyInfoUI.holyList.selectedIndex = holyIndex;
        _selectedHolyUnit = _allHolyUnits.Count > 0 ? _allHolyUnits[holyIndex] : null;
        ShowSelectedHolyInfo();
        
        _battleHolyList = HolyManager.Instance.GetHolyItemBattleList();
        UpdateHolySoltInfo();

        
        foreach (var i in _holyUploadItemDict.Values)
        {
            i.isSelect.selectedIndex = 0; // 重置所有项为未选中
            _holyInfoUI.SetChildIndex(i, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
        }
        foreach (var item in _holyUploadItemDict)
        {
            HolySlotInfo slotInfo = new HolySlotInfo();
            slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(item.Key);
            if (slotInfo.ItemId > 0 && slotInfo.Status == eSlotStatus.eSlotStatus_Normal)
            {
                item.Value.isSelect.selectedIndex = 1;
                _holyInfoUI.selectBg.visible = true;//选中背景
                _holyInfoUI.selectBg.touchable = false; // 设置点击穿透
                int bgIndex = _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg);
                // // 设置item在selectBg的上一层
                _holyInfoUI.SetChildIndex(item.Value, bgIndex + 1);
                break;
            }
        }
    }

    private void UpdateHolySoltInfo()
    {
        foreach (var item in _holyUploadItemDict)
        {
            HolySlotInfo slotInfo = new HolySlotInfo();
            item.Value.isSelect.selectedIndex = 0;
            _holyInfoUI.selectBg.visible = false;//选中背景
            slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(item.Key);
            // 解锁且上阵
            if (slotInfo.ItemId > 0 && slotInfo.Status == eSlotStatus.eSlotStatus_Normal)
            {
                item.Value.status.selectedIndex = 1;
                item.Value.redPoint.visible = false;
                HolyItemInfo holyItemInfo = new HolyItemInfo();
                holyItemInfo = HolyManager.Instance.GetHolyItem(slotInfo.ItemId);
                var holyUnit = ConfigUtils.GetHolyUnitByHolyIdAndLevel(holyItemInfo.HolyId, holyItemInfo.Level);
                item.Value.icon.url = UIResource.GetItemUrl(holyUnit.ItemID.ToString());
            }
            
            // 解锁未上阵
            if (slotInfo.ItemId == 0 && slotInfo.Status == eSlotStatus.eSlotStatus_Normal)
            {
                item.Value.status.selectedIndex = 0;
                item.Value.redPoint.visible = true;
            }

            // 未解锁
            if (slotInfo.Status == eSlotStatus.eSlotStatus_Locked)
            {
                item.Value.status.selectedIndex = 0;
                item.Value.redPoint.visible = false;
            }
        }
    }
    
    /// <summary>
    /// 点击上阵圣物栏
    /// </summary>
    /// <param name="context"></param>
    private void OnClickUploadHolyListItem(EventContext context)
    {
        UI_HolyUploadItem item = context.sender as UI_HolyUploadItem;
        int index = int.Parse(item.name.Substring(14,1));
        
        // 重置替换模式
        _isReplaceMode = false;
        _isReplace2 = false;
        
        // 该栏位有圣物
        HolySlotInfo slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(index);
        if (slotInfo.ItemId > 0 && slotInfo.Status == eSlotStatus.eSlotStatus_Normal)
        {
            foreach (var i in _holyUploadItemDict.Values)
            {
                i.isSelect.selectedIndex = 0; // 重置所有项为未选中
                _holyInfoUI.SetChildIndex(i, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
            }
            item.isSelect.selectedIndex = 1;
            _holyInfoUI.selectBg.visible = true;
            _holyInfoUI.selectBg.touchable = false; // 设置点击穿透
            int bgIndex = _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg);
            // // 设置item在selectBg的上一层
            _holyInfoUI.SetChildIndex(item, bgIndex + 1);
            
            _selectedHolySoltIndex = index;
            HolyItemInfo _selectedHolyItem = HolyManager.Instance.GetHolyItem(slotInfo.ItemId);
            _selectedHolyUnit = ConfigUtils.GetHolyUnitByHolyIdAndLevel(_selectedHolyItem.HolyId, _selectedHolyItem.Level);
            ShowSelectedHolyInfo();
            _showFlagInHolyList = false; // 不显示标记
        }
    
        // 该栏位没有圣物,栏位已解锁
        if (slotInfo.ItemId == 0 && slotInfo.Status == eSlotStatus.eSlotStatus_Normal)
        {
            List<HolyItemInfo> noUploadHoly = HolyManager.Instance.GetNoUploadHoly();//没有上阵的圣物
            List<HolyItemInfo> hasHolys = HolyManager.Instance.GetHolyItemList();// 拥有的圣物
    
            // 没有任何圣物
            if (hasHolys == null || hasHolys.Count == 0)
            {
                // UIManager.Instance.ToastByKey(8043);//没有圣物提示，后续需要再加
                item.isSelect.selectedIndex = 0; // 取消选中
                _holyInfoUI.SetChildIndex(item, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
                _holyInfoUI.selectBg.visible = false;
                _showFlagInHolyList = false;
                return;
            }
            
            // 没有未装备的圣物
            if (noUploadHoly == null || noUploadHoly.Count == 0)
            {
                // UIManager.Instance.ToastByKey(8043);//没有未装备的圣物提示，后续需要再加
                item.isSelect.selectedIndex = 0; // 取消选中
                _holyInfoUI.SetChildIndex(item, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
                _holyInfoUI.selectBg.visible = false;
                _showFlagInHolyList = false;
                return;
            }
            
            // 进入引导状态
            foreach (var i in _holyUploadItemDict.Values)
            {
                i.isSelect.selectedIndex = 0; // 重置所有项为未选中
                _holyInfoUI.SetChildIndex(i, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
            }
            item.isSelect.selectedIndex = 1;
            _holyInfoUI.selectBg.visible = true;
            _holyInfoUI.selectBg.touchable = false; // 设置点击穿透
            _holyInfoUI.SetChildIndex(item, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) + 1);
            
            
            _selectedHolySoltIndex = index;
            _showFlagInHolyList = true;// 需要显示标记
            // 排序实现
            HolyItemInfo selectedHoly = noUploadHoly
                .Select(item => new {
                    Item = item,
                    MaxLevel = ConfigUtils.GetMaxHolyUnitByHolyId(item.HolyId).Lv  // 直接获取满级等级
                })
                .OrderByDescending(x => x.Item.Level >= x.MaxLevel)  // 1. 已满级优先
                .ThenByDescending(x => x.Item.Level)                 // 2. 等级降序
                .ThenBy(x => x.Item.HolyId)                          // 3. HolyId升序
                .Select(x => x.Item)
                .FirstOrDefault();
            _selectedHolyUnit = ConfigUtils.GetHolyUnitByHolyIdAndLevel(selectedHoly.HolyId, selectedHoly.Level);
            
            // 找到该圣物在列表中的位置
            holyIndex = _allHolyUnits.FindIndex(u => u.HolyId == selectedHoly.HolyId);
            if (holyIndex >= 0) {
                _holyInfoUI.holyList.selectedIndex = holyIndex;
            }
            
            ShowSelectedHolyInfo();
        }
        
        // 该栏位没有解锁
        if (slotInfo.Status == eSlotStatus.eSlotStatus_Locked)
        {
            // 没有解锁提示
            UIManager.Instance.ToastByKey(8043);
            _showFlagInHolyList = false; // 不显示标记
            return;
        }
        _holyInfoUI.holyList.numItems = _allHolyUnits.Count;
    
    }

    
    private void UpdateAndSortHolyItemList()
    {
        if (!isUIPanelOpen)  //回调更新圣物
        {
            BuffInfoManager.Instance.UpdateHolyData();
        }
        
        _holyInfoUI.currency1.txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(1010015));
        _holyInfoUI.currency2.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia);
        
        List<HolyItemInfo> holyItemList = new List<HolyItemInfo>();
        holyItemList = HolyManager.Instance.GetHolyItemList();// 已拥有的圣物列表
        
        Dictionary<int, ConfigHolyUnit> holyUnitDict = new Dictionary<int, ConfigHolyUnit>();
        foreach (var unit in _allHolyUnits)
        {
            holyUnitDict[unit.HolyId] = unit;
        }
        
        // 遍历已拥有的圣物
        foreach (var item in holyItemList)
        {
            // 是否有相同HolyId的项
            if (holyUnitDict.TryGetValue(item.HolyId, out var targetUnit))
            {
                ConfigHolyUnit unit = ConfigUtils.GetHolyUnitByHolyIdAndLevel(item.HolyId, item.Level);
                targetUnit = unit;
                holyUnitDict[unit.HolyId] = targetUnit;
            }
        }
        
        // 获取所有圣物ID的最大等级配置
        var maxLevels = holyUnitDict.Keys.ToDictionary(
            holyId => holyId,
            holyId => ConfigUtils.GetMaxHolyUnitByHolyId(holyId).Lv
        );
        
        // 上阵圣物
        var battleHolyIds = HolyManager.Instance.GetHolyItemBattleList()
            .Select(info => info.HolyId)
            .ToHashSet();
        
        // 构建圣物槽位映射：HolyId -> SlotId
        Dictionary<int, int> holyIdToSlotId = new Dictionary<int, int>();
        for (int slotId = 0; slotId < 3; slotId++)
        {
            HolySlotInfo slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(slotId);
            if (slotInfo.ItemId != 0) // 检查槽位是否有圣物
            {
                holyIdToSlotId[slotInfo.ItemId] = slotId;
            }
        }

        // 拥有且未上阵圣物
        var noUploadHolyItems = HolyManager.Instance.GetNoUploadHoly()
            .ToDictionary(info => info.HolyId, info => info.Level);
        
        // 排序实现
        var sortedItems = holyUnitDict
            .Select(item => {
                var holyId = item.Key;
        
                // 状态优先级: 0=已装备, 1=已拥有未装备, 2=未拥有
                int statusPriority = battleHolyIds.Contains(holyId) ? 0 : 
                    noUploadHolyItems.ContainsKey(holyId) ? 1 : 2;
                
                // 槽位ID（仅对上阵圣物有效）
                int slotId = statusPriority == 0 && holyIdToSlotId.TryGetValue(holyId, out int sid) 
                    ? sid 
                    : -1; // 非上阵圣物用-1表示
        
                // 等级优先级: 满级=0, 其他按等级倒序
                int levelPriority = 0;
                if (statusPriority == 1) // 仅处理已拥有未装备的圣物
                {
                    int currentLevel = noUploadHolyItems[holyId];
                    int maxLevel = maxLevels[holyId];
            
                    levelPriority = currentLevel >= maxLevel 
                        ? 0  // 满级最高优先级
                        : int.MaxValue - currentLevel; // 等级越高，优先级数值越大
                }
        
                return (Item: item, StatusPriority: statusPriority, SlotId: slotId, LevelPriority: levelPriority, HolyId: holyId);
            })
            .OrderBy(x => x.StatusPriority)  // 先按状态排序
            .ThenBy(x => x.StatusPriority == 0 ? x.SlotId : int.MaxValue) // 已上阵的按槽位排序
            .ThenBy(x => x.LevelPriority)    // 再按等级排序
            .ThenBy(x => x.HolyId)           // 最后按holyId升序（对状态1有效）
            .Select(x => x.Item.Value)
            .ToList();

        _allHolyUnits = sortedItems;
        _holyInfoUI.holyList.numItems = _allHolyUnits.Count;
        
        ShowSelectedHolyInfo();
        
        isUIPanelOpen = false;
    }
    
    // 点击列表触发事件：点击列表空白区域退出引导
    private void OnClickHolyList(EventContext context)
    {
        Vector2 globalPos = new Vector2(context.inputEvent.x, context.inputEvent.y);
        Vector2 localPos = _holyInfoUI.holyList.GlobalToLocal(globalPos);
    
        if (!IsPointInRect(localPos, _holyInfoUI.holyList)) 
            return;
    
        bool hitItem = false;
    
        // 处理虚拟列表：只检测实际渲染的项
        int firstIndex = _holyInfoUI.holyList.GetFirstChildInView();
        int selectedIndex = _holyInfoUI.holyList.selectedIndex;
        int visibleItemCount = _holyInfoUI.holyList.numChildren;
    
        for (int i = 0; i < visibleItemCount; i++)
        {
            GObject item = _holyInfoUI.holyList.GetChildAt(i);
            Vector2 itemLocalPos = item.GlobalToLocal(globalPos);
        
            if (IsPointInRect(itemLocalPos, item))
            {
                hitItem = true;
                break;
            }
        }
    
        if (!hitItem)
        {
            // 检查是否在滚动中
            if (_holyInfoUI.holyList.scrollPane != null && _holyInfoUI.holyList.scrollPane.isDragged)
            {
                return; // 滚动中不触发穿透
            }
        
            if (_showFlagInHolyList)
            {
                ExitGuideState1();
            }
            
            if (_isReplaceMode)
            {
                ExitGuideState1();
            }
    
            if (_isReplace2)
            {
                _selectedHolySoltIndex = -1;
                _holyInfoUI.ctrl.selectedIndex = 1;
                _isReplace2 = false;
                ShowSelectedHolyInfo();
            }
        }
    }
    
    // 边界检测方法
    private bool IsPointInRect(Vector2 point, GObject obj)
    {
        // 检查点是否在组件边界内
        return point.x >= 0 && 
               point.y >= 0 && 
               point.x <= obj.width && 
               point.y <= obj.height;
    }
    
    private void HolyItemRender(int index, GObject item)
    {
        var holyUnitInAll = _allHolyUnits[index];
        // 设置选中状态
        ((UI_HolyItem)item).selected = (_selectedHolyUnit != null && holyUnitInAll.HolyId == _selectedHolyUnit.HolyId);;//(index == holyIndex);
        ((UI_HolyItem)item).icon = UIResource.GetItemUrl(holyUnitInAll.ItemID.ToString());
        ((UI_HolyItem)item).level.SetVar("value",  holyUnitInAll.Lv.ToString()).FlushVars();
        
        // 未装备
        bool isUploaded = HolyManager.Instance.GetNoUploadHoly()
            .Any(info => info.HolyId == holyUnitInAll.HolyId);
    
        // 已装备
        bool isUnlocked = HolyManager.Instance.GetHolyItemBattleList()
            .Any(info => info.HolyId == holyUnitInAll.HolyId);
        
        if (isUploaded)
        {
            // 解锁未装备
            ((UI_HolyItem)item).status.selectedIndex = 0;
            var nextUnit1 = ConfigUtils.GetNextHolyUnitByHolyIdAndLevel(holyUnitInAll.HolyId, holyUnitInAll.Lv);
            if (nextUnit1 != null && DataManager.Instance.mRoleData.dia > nextUnit1.Consume2 && ItemInfoManager.Instance.GetItemCount(1010015) > nextUnit1.Consume1)
            {
                ((UI_HolyItem)item).redPoint.visible = true;
            }
            else
            {
                ((UI_HolyItem)item).redPoint.visible = false;
            }
        } 
        else if (isUnlocked)
        {
            // 解锁已装备
            ((UI_HolyItem)item).status.selectedIndex = 1;
            var nextUnit2 = ConfigUtils.GetNextHolyUnitByHolyIdAndLevel(holyUnitInAll.HolyId, holyUnitInAll.Lv);
            if (nextUnit2 != null && DataManager.Instance.mRoleData.dia > nextUnit2.Consume2 && ItemInfoManager.Instance.GetItemCount(1010015) > nextUnit2.Consume1)
            {
                ((UI_HolyItem)item).redPoint.visible = true;
            }
            else
            {
                ((UI_HolyItem)item).redPoint.visible = false;
            }
        }
        else
        {
            // 未解锁
            ((UI_HolyItem)item).status.selectedIndex = 2;
        }
    
        ((UI_HolyItem)item).data = holyUnitInAll;
        ((UI_HolyItem)item).onClick.Set(this.OnClickHolyItemInList);
        
        // 箭头显示逻辑：仅在引导状态且是未装备圣物时显示
        bool shouldShowFlag = _showFlagInHolyList && isUploaded;
        ((UI_HolyItem)item).flag.visible = shouldShowFlag;
        
    }
    
    private void OnClickHolyItemInList(EventContext context)
    {
        // ConfigHolyUnit holy = (context.sender as UI_HolyItem)?.data as ConfigHolyUnit;
        
        UI_HolyItem clickedItem = context.sender as UI_HolyItem;
        holyIndex = _holyInfoUI.holyList.GetChildIndex(clickedItem); // 更新选中索引
        // int firstIndex = _holyInfoUI.holyList.GetFirstChildInView();//设置为虚拟列表时使用
        // _holyInfoUI.holyList.selectedIndex = holyIndex + firstIndex;//设置为虚拟列表时使用
        _holyInfoUI.holyList.selectedIndex = holyIndex;

        _selectedHolyUnit = clickedItem?.data as ConfigHolyUnit;

        HolySlotInfo slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(_selectedHolySoltIndex);

        // 选中的为已上阵圣物
        if (HolyManager.Instance.GetHolyItemBattleList().Any(info => info.HolyId == _selectedHolyUnit.HolyId))
        {
            // _selectedHolySoltIndex = RoleManager.Instance.GetHolySlotInfoByHolyId(_selectedHolyUnit.HolyId).SlotId;
            int slotId = RoleManager.Instance.GetHolySlotInfoByHolyId(_selectedHolyUnit.HolyId).SlotId;
            foreach (var i in _holyUploadItemDict.Values)
            {
                i.isSelect.selectedIndex = 0; // 重置所有项为未选中
                _holyInfoUI.SetChildIndex(i, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
            }
            // ((UI_HolyUploadItem)_holyInfoUI.GetChild("holyUploadItem" + _selectedHolySoltIndex)).isSelect.selectedIndex = 1;
            ((UI_HolyUploadItem)_holyInfoUI.GetChild("holyUploadItem" + slotId)).isSelect.selectedIndex = 1;
            _holyInfoUI.selectBg.visible = true;
            _holyInfoUI.selectBg.touchable = false; // 设置点击穿透
            _holyInfoUI.SetChildIndex(_holyInfoUI.GetChild("holyUploadItem" + slotId), _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) + 1);
        }
        
        // 引导状态下点击处理
        if (_showFlagInHolyList)
        {
            // 是否属于没有上阵
            bool isNoUpload = HolyManager.Instance.GetNoUploadHoly()
                .Any(info => info.HolyId == _selectedHolyUnit.HolyId);
            
            // 是否属于已上阵
            bool isUnlocked = HolyManager.Instance.GetHolyItemBattleList()
                .Any(info => info.HolyId == _selectedHolyUnit.HolyId);

            // 是否解锁
            // bool isHas = HolyManager.Instance.GetHolyItem(_selectedHolyUnit.HolyId) == null;
            bool isHas = isNoUpload || isUnlocked;
            
            // 点击未装备圣物：保持引导状态。点击已装备：退出引导状态。点击未解锁圣物：退出引导状态
            if (isUnlocked) // 点击已装备：退出引导状态
            {
                ExitGuideState1();
            }
            else if (!isHas) // 点击未解锁圣物：退出引导状态
            {
                ExitGuideState2();
            }
            
        }

        if (_isReplace2)
        {
            bool isUnlocked = HolyManager.Instance.GetHolyItemBattleList()
                .Any(info => info.HolyId == _selectedHolyUnit.HolyId);
            
            // 是否解锁
            bool isHas = HolyManager.Instance.GetHolyItem(_selectedHolyUnit.HolyId) == null;

            if (isUnlocked || isHas)
            {
                _selectedHolySoltIndex = -1;
                _holyInfoUI.ctrl.selectedIndex = 1;
                _isReplace2 = false;
            }
        }

        bool isHasHoly = HolyManager.Instance.GetHolyItem(_selectedHolyUnit.HolyId) == null;
        // 是否属于没有上阵
        bool isNoUploadHoly = HolyManager.Instance.GetNoUploadHoly()
            .Any(info => info.HolyId == _selectedHolyUnit.HolyId);
        if (isHasHoly || isNoUploadHoly)
        {
            // 重置所有槽位选中状态
            foreach (var item in _holyUploadItemDict.Values)
            {
                item.isSelect.selectedIndex = 0;
                _holyInfoUI.SetChildIndex(item, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
                _holyInfoUI.selectBg.visible = false;
            }
        }
        
        // 退出替换模式（如果用户点击了圣物列表）
        _isReplaceMode = false;
        UpdateReplaceButtons(false);
        
        ShowSelectedHolyInfo();
        
    }
    
    // 更新替换按钮显示状态
    private void UpdateReplaceButtons(bool show)
    {
        _holyInfoUI.replaceBtn0.visible = show;
        _holyInfoUI.replaceBtn1.visible = show;
        _holyInfoUI.replaceBtn2.visible = show;
        _holyInfoUI.holyUploadItem0.guide.visible = show;
        _holyInfoUI.holyUploadItem1.guide.visible = show;
        _holyInfoUI.holyUploadItem2.guide.visible = show;
        // _holyInfoUI.hasMask.selectedIndex = show ? 1 : 0;
        
        //调高层级
        _holyInfoUI.SetChildIndex(_holyInfoUI.replaceBtn0,_holyInfoUI.GetChildIndex(_holyInfoUI.holyUploadItem0) + 1);
        _holyInfoUI.SetChildIndex(_holyInfoUI.replaceBtn1,_holyInfoUI.GetChildIndex(_holyInfoUI.holyUploadItem1) + 1);
        _holyInfoUI.SetChildIndex(_holyInfoUI.replaceBtn2,_holyInfoUI.GetChildIndex(_holyInfoUI.holyUploadItem2) + 1);
    }
    
    // 引导状态退出:点击已装备
    private void ExitGuideState1()
    {
        _showFlagInHolyList = false;
        _selectedHolySoltIndex = -1;
    
        // 重置所有槽位选中状态
        foreach (var item in _holyUploadItemDict.Values)
        {
            item.isSelect.selectedIndex = 0;
            _holyInfoUI.SetChildIndex(item, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
            _holyInfoUI.selectBg.visible = false;
        }
    
        // 刷新列表隐藏箭头
        _holyInfoUI.holyList.numItems = _allHolyUnits.Count;
    
        // 默认选中第一个圣物
        holyIndex = 0;
        _holyInfoUI.holyList.selectedIndex = holyIndex;
        _selectedHolyUnit = _allHolyUnits.Count > 0 ? _allHolyUnits[0] : null;
        ShowSelectedHolyInfo();
    }

    // 引导状态退出:点击未解锁圣物
    private void ExitGuideState2()
    {
        _showFlagInHolyList = false;
        _selectedHolySoltIndex = -1;
        
        // 重置所有槽位选中状态
        foreach (var item in _holyUploadItemDict.Values)
        {
            item.isSelect.selectedIndex = 0;
            _holyInfoUI.SetChildIndex(item, _holyInfoUI.GetChildIndex(_holyInfoUI.selectBg) - 1);
            _holyInfoUI.selectBg.visible = false;
        }
        
        // 刷新列表隐藏箭头
        _holyInfoUI.holyList.numItems = _allHolyUnits.Count;
        ShowSelectedHolyInfo();
    }

    private void ShowSelectedHolyInfo()
    {
        if (_selectedHolyUnit == null) return;
        
        var selectHoly = HolyManager.Instance.GetHolyItem(_selectedHolyUnit.HolyId);
        if (selectHoly != null)
        {
            _selectedHolyUnit = ConfigUtils.GetHolyUnitByHolyIdAndLevel(selectHoly.HolyId, selectHoly.Level);
        }
        
        // 解锁未装备
        bool isUploaded = HolyManager.Instance.GetNoUploadHoly()
            .Any(info => info.HolyId == _selectedHolyUnit.HolyId);
    
        // 解锁已装备
        bool isUnlocked = HolyManager.Instance.GetHolyItemBattleList()
            .Any(info => info.HolyId == _selectedHolyUnit.HolyId);
        
        // 是否拥有（解锁）
        bool isOwned = isUploaded || isUnlocked;
        
        // 是否满级
        bool isMaxLevel = _selectedHolyUnit.Equals(ConfigUtils.GetMaxHolyUnitByHolyId(_selectedHolyUnit.HolyId));
        
        // 上阵圣物的数量
        int battleHolyCount = HolyManager.Instance.GetHolyItemBattleList().Count;
        
        // 重置替换按钮
        UpdateReplaceButtons(false);
        
        _holyInfoUI.holyName.text = ConfigUtils.GetTextById(_selectedHolyUnit.Name);

        if (isOwned)
        {
            // 已拥有
            if (isMaxLevel)
            {
                _holyInfoUI.maxLv.SetVar("value", _selectedHolyUnit.Lv.ToString()).FlushVars();
                _holyInfoUI.maxDesc.text = StringUtils.Format(ConfigUtils.GetTextById(_selectedHolyUnit.Doc), 
                    (int.Parse(_selectedHolyUnit.Common)*ConstDefine.CONFIG_PLACE).ToString("f2"));
                _holyInfoUI.isMax.selectedIndex = 1;
            }
            else
            {
                var nextUnit = ConfigUtils.GetNextHolyUnitByHolyIdAndLevel(_selectedHolyUnit.HolyId, _selectedHolyUnit.Lv);//下一等级的圣物
                _holyInfoUI.curLv.SetVar("value", _selectedHolyUnit.Lv.ToString()).FlushVars();
                _holyInfoUI.curDesc.text = StringUtils.Format(ConfigUtils.GetTextById(_selectedHolyUnit.Doc), 
                    (int.Parse(_selectedHolyUnit.Common)*ConstDefine.CONFIG_PLACE).ToString("f2"));
                _holyInfoUI.nextLv.SetVar("value", nextUnit.Lv.ToString()).FlushVars();
                _holyInfoUI.nextDesc.text = StringUtils.Format(ConfigUtils.GetTextById(nextUnit.Doc), 
                    (int.Parse(nextUnit.Common)*ConstDefine.CONFIG_PLACE).ToString("f2"));
                _holyInfoUI.isMax.selectedIndex = 0;
            
                // 强化材料显示
                _holyInfoUI.cost1.icon.url = UIResource.GetItemUrl(1010015.ToString());
                _holyInfoUI.cost1.num.text = nextUnit.Consume1.ToString();
                _holyInfoUI.cost2.icon.url = UIResource.GetItemUrl(1000.ToString());
                _holyInfoUI.cost2.num.text = nextUnit.Consume2.ToString();
            
                bool flag1 = ItemInfoManager.Instance.GetItemCount(1010015) < nextUnit.Consume1;//碎片是否充足
                bool flag2 = DataManager.Instance.mRoleData.dia < nextUnit.Consume2;//砖石是否充足
                _holyInfoUI.cost1.status.selectedIndex = flag1 ? 1 : 0;
                _holyInfoUI.cost2.status.selectedIndex = flag2 ? 1 : 0;
                _holyInfoUI.strongBtnCtrl.selectedIndex = (flag1 || flag2) ? 1 : 0;
            }
            
        }
        else
        {
            // 未拥有
            _holyInfoUI.maxLv.SetVar("value", _selectedHolyUnit.Lv.ToString()).FlushVars();
            _holyInfoUI.maxDesc.text = StringUtils.Format(ConfigUtils.GetTextById(_selectedHolyUnit.Doc), 
                (int.Parse(_selectedHolyUnit.Common)*ConstDefine.CONFIG_PLACE).ToString("f2"));
            _holyInfoUI.isMax.selectedIndex = 1;
        }
        
        // ---状态控制---
        if (!isOwned) {
            // 未拥有状态
            _holyInfoUI.ctrl.selectedIndex = 1; // 装备/替换控制器
            _holyInfoUI.upBtnCtrl.selectedIndex = 1; // 按钮置灰
            _holyInfoUI.hasCtrl.selectedIndex = 1; // 未拥有状态
            _holyInfoUI.downBtnCtrl.selectedIndex = 1; // 卸下按钮置灰
        }
        else if (isUnlocked) {
            // 已装备状态
            _holyInfoUI.ctrl.selectedIndex = 0; // 卸下控制器
            _holyInfoUI.hasCtrl.selectedIndex = 0; // 拥有状态
            // _holyInfoUI.downBtnCtrl.selectedIndex = _selectedHolySoltIndex >= 0 ? 0 : 1; // 根据槽位选中状态
            _holyInfoUI.downBtnCtrl.selectedIndex = 0;
        }
        else if (isUploaded) {
            // 未装备状态
            _holyInfoUI.ctrl.selectedIndex = 1; // 装备控制器
            _holyInfoUI.hasCtrl.selectedIndex = 0; // 拥有状态
            
            _holyInfoUI.upBtnCtrl.selectedIndex = 0;
            if (_selectedHolySoltIndex >= 0 && RoleManager.Instance.GetHolySlotInfoBySlotId(_selectedHolySoltIndex).ItemId > 0)
            {
                _holyInfoUI.ctrl.selectedIndex = 2;
                _isReplace2 = true;
            }
        
            // 如果穿戴栏满，进入替换模式
            if (battleHolyCount >= 3 && !_isReplaceMode) {
                _holyInfoUI.ctrl.selectedIndex = 2;
                // _isReplaceMode = true;
                // UpdateReplaceButtons(true);
            }
        }
        
    }

    
    // 圣物升级
    private void UpLvToHolyItem()
    {
        // 选中的圣物信息：_selectedHolyUnit
        
        // 获取下一级圣物信息
        var nextHoly = ConfigUtils.GetNextHolyUnitByHolyIdAndLevel(_selectedHolyUnit.HolyId, _selectedHolyUnit.Lv);
        
        // 判断材料是否充足
        if (ItemInfoManager.Instance.GetItemCount(1010015) < nextHoly.Consume1)
        {
            //碎片不足
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038,ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(1010015).Name)));
            return;
        }
        if (DataManager.Instance.mRoleData.dia < nextHoly.Consume2)
        {
            //砖石不足
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038,ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(1000).Name)));
            return;
        }
        
        var builder = HolyItemLevelUp_CS.CreateBuilder();
        builder.HolyId = (uint)_selectedHolyUnit.HolyId;
        builder.LevelupTimes = 1;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HolyItemLevelUp_CS, builder.Build());
    }
    
    // 圣物上阵或替换
    private void UpLoadOrReplace()
    {
        // 替换模式处理
        // if (_isReplaceMode) {
        //     // 在替换模式下，这个按钮不可用
        //     return;
        // }

        // 是否解锁
        bool isHas = HolyManager.Instance.GetHolyItem(_selectedHolyUnit.HolyId) == null;
        if (isHas)
        {
            UIManager.Instance.Toast("还未获得此圣物！");
            return;
        }
        
        if (HolyManager.Instance.GetHolyItemBattleList().Count >= 3 && !_isReplaceMode)
        {
            _isReplaceMode = true;
            ShowSelectedHolyInfo();
            UpdateReplaceButtons(true);
            return;
        }
    
        // 没有选择槽位时,装备至上方空位，按照顺序填充
        if (_selectedHolySoltIndex < 0)
        {
            for (int i = 0; i < 3; i++)
            {
                HolySlotInfo slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(i);
                if (slotInfo.ItemId == 0)
                {
                    _selectedHolySoltIndex = slotInfo.SlotId;
                    break;
                }
            }
        }

        if (_selectedHolySoltIndex >= 0)
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralUploadSE);
            
            var builder = ReplaceHolyItemInSlot_CS.CreateBuilder();
            builder.HolyId = (uint)_selectedHolyUnit.HolyId;
            builder.HolySlotId = (uint)_selectedHolySoltIndex;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ReplaceHolyItemInSlot_CS, builder.Build());
        }
        
        _selectedHolySoltIndex = -1;
        _showFlagInHolyList = false;
        _isReplace2 = false;
        _holyInfoUI.holyList.numItems = _allHolyUnits.Count;
    }
    
    // 圣物批量上阵或替换
    private void BatchUpLoadOrReplace()
    {
        var builder = BatchReplaceHolyItemInSlot_CS.CreateBuilder();
        // foreach (var id in )
        // {
        //     builder.AddHolyId(id);
        // }
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_BatchReplaceHolyItemInSlot_CS, builder.Build());
    }

    // 圣物下阵
    public void DownHolyItemInSlot()
    {
        // 选择槽位提示,没有选择槽位时
        if (_selectedHolySoltIndex < 0)
        {
            HolySlotInfo slotInfo = RoleManager.Instance.GetHolySlotInfoByHolyId(_selectedHolyUnit.HolyId);
            bool isUpload = slotInfo != null;
            if (_selectedHolyUnit != null && isUpload)
            {
                _selectedHolySoltIndex = slotInfo.SlotId;
            }
            
        }
        
        var builder = RemoveHolyItemFromSlot_CS.CreateBuilder();
        builder.SlotId = (uint)_selectedHolySoltIndex;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RemoveHolyItemFromSlot_CS, builder.Build());
        
        _selectedHolySoltIndex = -1;
    }

    private void OnClickRepaceBtn0()
    {
        ReplaceHolyItem(0);
    }

    private void OnClickRepaceBtn1()
    {
        ReplaceHolyItem(1);
    }

    private void OnClickRepaceBtn2()
    {
        ReplaceHolyItem(2);
    }
    
    private void ReplaceHolyItem(int slotId)
    {
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralUploadSE);
        
        var builder = ReplaceHolyItemInSlot_CS.CreateBuilder();
        builder.HolyId = (uint)_selectedHolyUnit.HolyId;
        builder.HolySlotId = (uint)slotId;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ReplaceHolyItemInSlot_CS, builder.Build());
    
        // 退出替换模式
        _isReplaceMode = false;
        _selectedHolySoltIndex = -1;
        _showFlagInHolyList = false;
        UpdateReplaceButtons(false);
        // _holyInfoUI.holyList.RefreshVirtualList();
    }

    private void OnClickBlankAreaToExitGuide()
    {
        if (_showFlagInHolyList)
        {
            ExitGuideState1();
        }

        if (_isReplaceMode)
        {
            ExitGuideState1();
        }

        if (_isReplace2)
        {
            _selectedHolySoltIndex = -1;
            _holyInfoUI.ctrl.selectedIndex = 1;
            _isReplace2 = false;
            ShowSelectedHolyInfo();
        }
    }

    private void OnClickMaxBtn()
    {
        if (_showFlagInHolyList)
        {
            ExitGuideState1();
        }
    }

    private void OnClickHolyHelp()
    {
        if (_showFlagInHolyList)
        {
            ExitGuideState1();
        }
        
        if (_isReplaceMode)
        {
            ExitGuideState1();
        }

        if (_isReplace2)
        {
            _selectedHolySoltIndex = -1;
            _holyInfoUI.ctrl.selectedIndex = 1;
            _isReplace2 = false;
            ShowSelectedHolyInfo();
        }
        
        UIManager.Instance.ShowUIPanel("Help", HelpType.Help_HolyHelp);
    }


    private void HolyRedPointInTable()
    {
        RefreshBottomRedDot();
    }

    #endregion
}
