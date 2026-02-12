using System;
using System.Collections.Generic;
using System.Linq;
using Achievement;
using Config;
using Engine;
using FairyGUI;
using msg;
using UnityEngine;
using AchievementInfo = Engine.AchievementInfo;
using EventDispatcher = EngineBase.EventDispatcher;

public enum AchievementType
{
    EQUIP = 1,
    PET = 2,
    SKILL = 3,
    PLAY = 4
}

public class AchievementView : UIViewBase
{
    private UI_AchievementMain Achievement => this.main as UI_AchievementMain;
    
    private List<ConfigAchievementUnit> _curUnit;//配置表
    private List<AchievementInfo> _curInfo = new List<AchievementInfo>();//服务器下发
    private List<ConfigAchievementUnit> _filterUnit;
    private int _scrollIndex;

    public AchievementView()
    {
        this.name = "Achievement";
        this.package = "Achievement";
        this.component = "AchievementMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        AchievementBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.Achievement.closeBtn.onClick.Add(this.Hide);
        this.Achievement.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.Achievement.list.itemRenderer = AchievementListRender;
        this.Achievement.list.SetVirtual();
        this.Achievement.tabList.onClickItem.Add(OnClickTab);
        this.Achievement.tabList.selectedIndex = 0;
        this.Achievement.tabList.itemRenderer = TabListRender;

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ACHIEVEMENT_UPDATE,this.UpdateAchievementInfo);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ACHIEVEMENT_UPDATE,this.UpdateAchievementInfo);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.Achievement.tabList.selectedIndex = 0;
        ChangeAchievementIndex(0);
        UpdateAchievementInfo();
    }

    private void OnClickTab(EventContext context)
    {
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralClickSE);
        
        GButton item = context.data as GButton;
        var index = this.Achievement.tabList.GetChildIndex(item);
        this.Achievement.tabList.selectedIndex = index;
        ChangeAchievementIndex(index);
        UpdateAchievementInfo();
    }

    private void ChangeAchievementIndex(int index)
    {
        switch (index)
        {
            case 0://装备
                _curUnit = ConfigUtils.GetAchievementUnitByType((int)AchievementType.EQUIP);
                FilterAndSortAchievementList(_curUnit);
                break;
            case 1://宠物
                _curUnit = ConfigUtils.GetAchievementUnitByType((int)AchievementType.PET);
                FilterAndSortAchievementList(_curUnit);
                break;
            case 2://技能
                _curUnit = ConfigUtils.GetAchievementUnitByType((int)AchievementType.SKILL);
                FilterAndSortAchievementList(_curUnit);
                break;
            case 3://玩法
                _curUnit = ConfigUtils.GetAchievementUnitByType((int)AchievementType.PLAY);
                FilterAndSortAchievementList(_curUnit);
                break;
        }
    }

    private void FilterAndSortAchievementList(List<ConfigAchievementUnit> achievementUnits)
    {
        _curInfo = AchievementManager.Instance.GetAchievementList();
        _filterUnit = new List<ConfigAchievementUnit>();
        foreach (var item in achievementUnits)
        {
            // 检查前置成就是否满足
            if (item.PreAchievementID != 0)
            {
                if (_curInfo.Count == 0)
                {
                    continue;
                }
                foreach (var info in _curInfo)
                {
                    // 如果_curInfo有当前item的Aid，说明前置成就已完成，移除前置成就，添加当前成就
                    if (info.AId == item.AID)
                    {
                        _filterUnit.Remove(ConfigUtils.GetAchievementUnitByAid(item.PreAchievementID));
                        _filterUnit.Add(item);
                        continue;
                    }
    
                    //前置成就完成时，添加当前成就
                    if (item.PreAchievementID == info.AId && info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
                    {
                        _filterUnit.Add(item);
                    }
                }
            }
            else
            {
                _filterUnit.Add(item);
                foreach (var info in _curInfo)
                {
                    //完成且为前置成就时，移除
                    if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed && ConfigUtils.GetNextAchievementUnitByAId(info.AId))
                    {
                        _filterUnit.Remove(ConfigUtils.GetAchievementUnitByAid(info.AId));
                    }

                    // 暂时这样处理,后续有时间再优化
                    if (item.Type == (int)eMainTaskType.eMainTaskType_CollectQualityEquips || item.Type == (int)eMainTaskType.eMainTaskType_CollectQualityPets || item.Type == (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips || item.Type == (int)eMainTaskType.eMainTaskType_QualitySkillReachLevels)
                    {
                        ConfigAchievementUnit unit = ConfigUtils.GetAchievementUnitByAid(info.AId);
                        bool flag = unit.Type == item.Type;
                        if (flag)
                        {
                            int curUnitQuality = int.Parse(unit.Param1.Split(',')[1]);
                            int initUnitQuality = int.Parse(item.Param1.Split(',')[1]);

                            if (item.Type == (int)eMainTaskType.eMainTaskType_QualitySkillReachLevels)
                            {
                                curUnitQuality = int.Parse(unit.Param1.Split(',')[0]);
                                initUnitQuality = int.Parse(item.Param1.Split(',')[0]);
                            }
                            
                            bool flag2 = curUnitQuality == initUnitQuality;
                            
                            if (unit.PreAchievementID != 0 && flag2)
                            {
                                _filterUnit.Remove(item);
                            }
                        }
                        
                    }
                    else
                    {
                        ConfigAchievementUnit unit = ConfigUtils.GetAchievementUnitByAid(info.AId);
                        bool flag = unit.Type == item.Type;
                        if (flag && unit.PreAchievementID != 0)
                        {
                            _filterUnit.Remove(item);
                        }
                    }
                }
            }
            
        }
    }

    private void SortAchievements()
    {
        _filterUnit.Sort((a, b) =>
        {
            // 获取成就状态
            var aInfo = _curInfo.Find(info => info.AId == a.AID);
            var bInfo = _curInfo.Find(info => info.AId == b.AID);

            bool aCompleted = aInfo?.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed;
            bool bCompleted = bInfo?.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed;

            // 已完成的成就排在最后
            if (aCompleted && !bCompleted)
                return 1;
            if (!aCompleted && bCompleted)
                return -1;

            // 按SortID和AID排序
            int sortCompare = a.SortID.CompareTo(b.SortID);
            return sortCompare != 0 ? sortCompare : a.AID.CompareTo(b.AID);
        });
    }

    private Dictionary<int, AchievementInfo> _curInfoDict;
    private void UpdateAchievementInfo()
    {
        _curInfo = AchievementManager.Instance.GetAchievementList();
        FilterAndSortAchievementList(_curUnit);
        SortAchievements();
        
        _curInfoDict = _curInfo.ToDictionary(x => x.AId); 
        
        this.Achievement.list.numItems = _filterUnit.Count;
        this.Achievement.list.RefreshVirtualList();
        
        // 自动定位到可领取位置
        _scrollIndex = -1;
        for (int i = 0; i < _filterUnit.Count; i++)
        {
            int targetAId = _filterUnit[i].AID; 
            if (_curInfoDict.TryGetValue(targetAId, out AchievementInfo info) && info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
            {
                _scrollIndex = i;
                break;
            }
        }
        _scrollIndex = _scrollIndex != -1 ? _scrollIndex : 0; // 无领取项则定位到顶部
        this.Achievement.list.ScrollToView(_scrollIndex, true, true);
        
        this.Achievement.tabList.numItems = 4;
    }

    // private void AchievementListRender(int index, GObject item)
    // {
    //     ((UI_AchievementItem)item).icon.url = UIResource.GetAchievementIcon(_curUnit[index].Item.ToString());
    //     int attrId = int.Parse(_filterUnit[index].Reward.Split(',')[0]);
    //     double value = double.Parse(_filterUnit[index].Reward.Split(',')[1]);
    //     ((UI_AchievementItem)item).itemBtn.icon = UIResource.GetAttrIconById(ConfigUtils.GetAttrEnumerationUnitByAttrId(attrId).Icon);//属性图标
    //     ((UI_AchievementItem)item).itemBtn.num.text = EquipManager.Instance.SetAttributeValue(attrId, value, true);//属性值
    //     
    //     ItemData itemData = new ItemData();
    //     itemData.id = attrId;
    //     itemData.count = value;
    //     ((UI_AchievementItem)item).itemBtn.data = itemData;
    //     ((UI_AchievementItem)item).itemBtn.onClick.Set(this.OnClickAchievementAttr);
    //
    //     ((UI_AchievementItem)item).desc.text = ConfigUtils.GetTextById(_filterUnit[index].Doc);
    //
    //     ((UI_AchievementItem)item).getBtn.data = _filterUnit[index].AID;
    //     
    //     ((UI_AchievementItem)item).isOk.selectedIndex = 0;
    //
    //     if (_filterUnit[index].Type == (int)eAchievementDetailType.eAchievementDetailType_PassStage)
    //     {
    //         ((UI_AchievementItem)item).bar.min = 0;
    //         ((UI_AchievementItem)item).bar.max = 1;
    //         ((UI_AchievementItem)item).bar.value = 0;
    //         
    //         if (_curInfo.Count == 0)
    //         {
    //             ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //             ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //             ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //             ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //         }
    //         foreach (var info in _curInfo)
    //         {
    //             if (_filterUnit[index].AID == info.AId)
    //             {
    //                 ((UI_AchievementItem)item).bar.value = Mathf.Min(1, info.Process);
    //                 if (info.Process >= 1)//已完成
    //                 {
    //                     // 已完成未领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtn);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                     }
    //                     
    //                     // 已完成已领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
    //                     {
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                         // ((UI_AchievementItem)item).getBtn.touchable = false;
    //                         ((UI_AchievementItem)item).isOk.selectedIndex = 1;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     // 进行中
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_InProcess)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //                     }
    //                 }
    //                 break;
    //             }
    //             else
    //             {
    //                 ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                 ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                 ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                 ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //             }
    //         }
    //     }
    //     else if (_filterUnit[index].Type == (int)eAchievementDetailType.eAchievementDetailType_PartEquipReachLevel || _filterUnit[index].Type == (int)eAchievementDetailType.eAchievementDetailType_ArtifactLevel)
    //     {
    //         ((UI_AchievementItem)item).bar.min = 0;
    //         ((UI_AchievementItem)item).bar.max = int.Parse(_filterUnit[index].Param1.Split(',')[1]);
    //         int process = 0;
    //
    //         if (_filterUnit[index].Type == (int)eAchievementDetailType.eAchievementDetailType_ArtifactLevel)
    //         {
    //             process = 1;
    //         }
    //         
    //         ((UI_AchievementItem)item).bar.value = process;
    //         
    //         if (_curInfo.Count == 0)
    //         {
    //             ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //             ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //             ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //             ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //         }
    //         foreach (var info in _curInfo)
    //         {
    //             if (info.AId == _filterUnit[index].AID)
    //             {
    //                 process = Mathf.Min(int.Parse(_filterUnit[index].Param1.Split(',')[1]), info.Process);
    //                 ((UI_AchievementItem)item).bar.value = process;
    //                 if (info.Process >= int.Parse(_filterUnit[index].Param1.Split(',')[1]))
    //                 {
    //                     // 已完成未领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtn);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                     }
    //                     
    //                     // 已完成已领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
    //                     {
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                         // ((UI_AchievementItem)item).getBtn.touchable = false;
    //                         ((UI_AchievementItem)item).isOk.selectedIndex = 1;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     // 进行中
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_InProcess)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //                     }
    //                 }
    //                 break;
    //             }
    //             else
    //             {
    //                 ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                 ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                 ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                 ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //             }
    //         }
    //     }else if (_filterUnit[index].Type == (int)eAchievementDetailType.eAchievementDetailType_SkillOfQualityReachLevel)
    //     {
    //         ((UI_AchievementItem)item).bar.min = 0;
    //         ((UI_AchievementItem)item).bar.max = int.Parse(_filterUnit[index].Param1.Split(',')[2]);
    //         int process = 0;
    //         ((UI_AchievementItem)item).bar.value = process;
    //
    //         if (_curInfo.Count == 0)
    //         {
    //             ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //             ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //             ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //             ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //         }
    //
    //         foreach (var info in _curInfo)
    //         {
    //             if (info.AId == _filterUnit[index].AID)
    //             {
    //                 process = Mathf.Min(int.Parse(_filterUnit[index].Param1.Split(',')[2]), info.Process);
    //                 ((UI_AchievementItem)item).bar.value = process;
    //                 if (info.Process >= int.Parse(_filterUnit[index].Param1.Split(',')[2]))
    //                 {
    //                     // 已完成未领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtn);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                     }
    //                     
    //                     // 已完成已领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
    //                     {
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                         // ((UI_AchievementItem)item).getBtn.touchable = false;
    //                         ((UI_AchievementItem)item).isOk.selectedIndex = 1;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     // 进行中
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_InProcess)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //                     }
    //                 }
    //                 break;
    //             }
    //             else
    //             {
    //                 ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                 ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                 ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                 ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         ((UI_AchievementItem)item).bar.min = 0;
    //         ((UI_AchievementItem)item).bar.max = int.Parse(_filterUnit[index].Param1.Split(',')[0]);
    //         int process = 0;
    //         ((UI_AchievementItem)item).bar.value = process;
    //         
    //         if (_curInfo.Count == 0)
    //         {
    //             ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //             ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //             ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //             ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //         }
    //
    //         foreach (var info in _curInfo)
    //         {
    //             if (info.AId == _filterUnit[index].AID)
    //             {
    //                 process = Mathf.Min(int.Parse(_filterUnit[index].Param1.Split(',')[0]), info.Process);
    //                 ((UI_AchievementItem)item).bar.value = process;
    //                 if (info.Process >= int.Parse(_filterUnit[index].Param1.Split(',')[0]))
    //                 {
    //                     // 已完成未领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtn);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                     }
    //                     
    //                     // 已完成已领取
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
    //                     {
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
    //                         // ((UI_AchievementItem)item).getBtn.touchable = false;
    //                         ((UI_AchievementItem)item).isOk.selectedIndex = 1;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     // 进行中
    //                     if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_InProcess)
    //                     {
    //                         ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                         ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                         ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                         ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //                     }
    //                 }
    //                 break;
    //             }
    //             else
    //             {
    //                 ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
    //                 ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
    //                 ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
    //                 ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
    //             }
    //         }
    //     }
    //     
    // }
    
    // 优化版
    private void AchievementListRender(int index, GObject item)
    {
        ((UI_AchievementItem)item).icon.url = UIResource.GetAchievementIcon(_curUnit[index].Item.ToString());
        int attrId = int.Parse(_filterUnit[index].Reward.Split(',')[0]);
        double value = double.Parse(_filterUnit[index].Reward.Split(',')[1]);
        ((UI_AchievementItem)item).itemBtn.icon = UIResource.GetAttrIconById(ConfigUtils.GetAttrEnumerationUnitByAttrId(attrId).Icon);//属性图标
        ((UI_AchievementItem)item).itemBtn.num.text = EquipManager.Instance.SetAttributeValue(attrId, value, true);//属性值
        
        ItemData itemData = new ItemData();
        itemData.id = attrId;
        itemData.count = value;
        ((UI_AchievementItem)item).itemBtn.data = itemData;
        ((UI_AchievementItem)item).itemBtn.onClick.Set(this.OnClickAchievementAttr);
    
        ((UI_AchievementItem)item).getBtn.data = _filterUnit[index].AID;
        
        ((UI_AchievementItem)item).isOk.selectedIndex = 0;
        
        // 后续参数格式有变化，需要再次修改
        switch (_filterUnit[index].Type)
        {
            case (int)eMainTaskType.eMainTaskType_KillMonster:
            case (int)eMainTaskType.eMainTaskType_GenEquipTimes:
            case (int)eMainTaskType.eMainTaskType_SoldEquipTimes:
            case (int)eMainTaskType.eMainTaskType_PlayPetLotto:
            case (int)eMainTaskType.eMainTaskType_PlaySkillLotto:
            case (int)eMainTaskType.eMainTaskType_PlayHeroLotto:
            case (int)eMainTaskType.eMainTaskType_EquipBoxLevelUp:
            case (int)eMainTaskType.eMainTaskType_ClassicAtkLevelUp:
            case (int)eMainTaskType.eMainTaskType_ClassicHPLevelUp:
            case (int)eMainTaskType.eMainTaskType_ClassicDefLevelUp:
            case (int)eMainTaskType.eMainTaskType_ClassicPetLevelUp:
            case (int)eMainTaskType.eMainTaskType_DailyTask:
            case (int)eMainTaskType.eMainTaskType_WatchADTimes:
            case (int)eMainTaskType.eMainTaskType_OnlineTime:
            case (int)eMainTaskType.eMainTaskType_ClaimHomeTownProduct:
            case (int)eMainTaskType.eMainTaskType_LoginDays:
            case (int)eMainTaskType.eMainTaskType_ArenaPlay:
            case (int)eMainTaskType.eMainTaskType_ClainOnlineAward:
            case (int)eMainTaskType.eMainTaskType_GoldCopyTimes:
            case (int)eMainTaskType.eMainTaskType_EquipCopyTimes:
            case (int)eMainTaskType.eMainTaskType_DiamondCopyTimes:
            case (int)eMainTaskType.eMainTaskType_HeroExpCopyTimes:
            case (int)eMainTaskType.eMainTaskType_RuneCopyTimes:
            case (int)eMainTaskType.eMainTaskType_AnyCopyTimes:
            case (int)eMainTaskType.eMainTaskType_CollectLoreEquips:
            case (int)eMainTaskType.eMainTaskType_PickUpGold:
            case (int)eMainTaskType.eMainTaskType_PickUpDiamond:
            case (int)eMainTaskType.eMainTaskType_FishingCounter:
            case (int)eMainTaskType.eMainTaskType_KillNormalBoss:
            case (int)eMainTaskType.eMainTaskType_KillLoreBoss:
            case (int)eMainTaskType.eMainTaskType_ArtifactLevels:
            case (int)eMainTaskType.eMainTaskType_FinishFixedEvents:
            case (int)eMainTaskType.eMainTaskType_ShufflePetTalents:
            case (int)eMainTaskType.eMainTaskType_PutOnPetSkillBook:
            case (int)eMainTaskType.eMainTaskType_CollectQualityEquips:
            case (int)eMainTaskType.eMainTaskType_CollectQualityPets:
            case (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips:
                ((UI_AchievementItem)item).desc.text = StringUtils.Format(ConfigUtils.GetTextById(_filterUnit[index].Doc), _filterUnit[index].TexParam);

                ((UI_AchievementItem)item).bar.min = 0;
                ((UI_AchievementItem)item).bar.max = int.Parse(_filterUnit[index].TexParam);
                int process1 = 0;
                ((UI_AchievementItem)item).bar.value = process1;
                
                if (_curInfo.Count == 0)
                {
                    ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                    ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                    ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                    ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                }
                
                foreach (var info in _curInfo)
                {
                    if (info.AId == _filterUnit[index].AID)
                    {
                        process1 = Mathf.Min(int.Parse(_filterUnit[index].TexParam), info.Process);
                        ((UI_AchievementItem)item).bar.value = process1;
                        if (info.Process >= int.Parse(_filterUnit[index].TexParam))
                        {
                            // 已完成未领取
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
                            {
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 0;
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
                                ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtn);
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
                            }
                            
                            // 已完成已领取
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
                            {
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 1;
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
                                // ((UI_AchievementItem)item).getBtn.touchable = false;
                                ((UI_AchievementItem)item).isOk.selectedIndex = 1;
                            }
                        }
                        else
                        {
                            // 进行中
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_InProcess)
                            {
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                                ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                            }
                        }
                        break;
                    }
                    else
                    {
                        ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                        ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                        ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                        ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                    }
                }
                break;
            case (int)eMainTaskType.eMainTaskType_QualitySkillReachLevels:
                string[] param2 = _filterUnit[index].TexParam.Split(',');
                int quality2 = int.Parse(param2[0]);
                int num2 = int.Parse(param2[1]);
                string qualityName2 = EquipManager.Instance.GetQualityName((QualityType)quality2);
                ((UI_AchievementItem)item).desc.text = StringUtils.Format(ConfigUtils.GetTextById(_filterUnit[index].Doc), qualityName2, num2.ToString());
                
                ((UI_AchievementItem)item).bar.min = 0;
                ((UI_AchievementItem)item).bar.max = int.Parse(_filterUnit[index].Param1.Split(',')[1]);
                int process2 = 0;
                ((UI_AchievementItem)item).bar.value = process2;
                
                if (_curInfo.Count == 0)
                {
                    ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                    ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                    ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                    ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                }
                
                foreach (var info in _curInfo)
                {
                    if (info.AId == _filterUnit[index].AID)
                    {
                        process2 = Mathf.Min(int.Parse(_filterUnit[index].Param1.Split(',')[1]), info.Process);
                        ((UI_AchievementItem)item).bar.value = process2;
                        if (info.Process >= int.Parse(_filterUnit[index].Param1.Split(',')[1]))
                        {
                            // 已完成未领取
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
                            {
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 0;
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
                                ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtn);
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
                            }
                            
                            // 已完成已领取
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
                            {
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 1;
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
                                // ((UI_AchievementItem)item).getBtn.touchable = false;
                                ((UI_AchievementItem)item).isOk.selectedIndex = 1;
                            }
                        }
                        else
                        {
                            // 进行中
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_InProcess)
                            {
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                                ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                            }
                        }
                        break;
                    }
                    else
                    {
                        ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                        ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                        ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                        ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                    }
                }
                break;
            case (int)eMainTaskType.eMainTaskType_PassStage:
                string[] stageParam = _filterUnit[index].TexParam.Split('|');
                string stagePatam1 = stageParam[0];
                string stagePatam2 = stageParam[1];
                ((UI_AchievementItem)item).desc.text = StringUtils.Format(ConfigUtils.GetTextById(_filterUnit[index].Doc), stagePatam1, stagePatam2);
                
                ((UI_AchievementItem)item).bar.min = 0;
                ((UI_AchievementItem)item).bar.max = 1;
                ((UI_AchievementItem)item).bar.value = 0;
                if (_curInfo.Count == 0)
                {
                    ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                    ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                    ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                    ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                }
                foreach (var info in _curInfo)
                {
                    if (_filterUnit[index].AID == info.AId)
                    {
                        ((UI_AchievementItem)item).bar.value = Mathf.Min(1, info.Process);
                        if (info.Process >= int.Parse(_filterUnit[index].Param1))//已完成
                        {
                            // 已完成未领取
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
                            {
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 0;
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
                                ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtn);
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
                            }
                            
                            // 已完成已领取
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_Claimed)
                            {
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 0;
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 1;
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 1;
                                // ((UI_AchievementItem)item).getBtn.touchable = false;
                                ((UI_AchievementItem)item).isOk.selectedIndex = 1;
                            }
                        }
                        else
                        {
                            // 进行中
                            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_InProcess)
                            {
                                ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                                ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                                ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                                ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                            }
                        }
                        break;
                    }
                    else
                    {
                        ((UI_GetBtn)((UI_AchievementItem)item).getBtn).status.selectedIndex = 2;
                        ((UI_AchievementItem)item).getBtnStatus.selectedIndex = 1;
                        ((UI_AchievementItem)item).getBtn.onClick.Set(this.OnClickGetBtnShowTips);
                        ((UI_AchievementItem)item).bar.status.selectedIndex = 0;
                    }
                }
                break;
            
        }
        
    }

    private void OnClickGetBtn(EventContext context)
    {
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralClickSE);
        
        int aId = (int)(context.sender as GButton).data;
        if (aId != null)
        {
            var builder = ClaimAchievementAward_CS.CreateBuilder();
            builder.Aid = (uint)aId;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimAchievementAward_CS, builder.Build());
            
            AchievementManager.Instance.SendToGetAchievement();
        }
    }

    private void OnClickAchievementAttr(EventContext context)
    {
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralClickSE);
        
        ItemData attrData = (context.sender as UI_ItemBtn).data as ItemData;
        string attrName = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationById(attrData.id).AttrName);
        string lastAttrValue = EquipManager.Instance.SetAttributeValue(attrData.id, attrData.count, true);
        TipsManger.Instance.ShowPopupTip((UI_ItemBtn)context.sender, Tipstype.Achievement, attrName, lastAttrValue, 110);
    }

    private void OnClickGetBtnShowTips()
    {
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralClickSE);
        UIManager.Instance.ToastByKey(10173);
    }

    private void TabListRender(int index, GObject item)
    {
        var uiItem = (UI_AchievementSelectBtn)item;
    
        AchievementType targetType = (AchievementType)(index + 1);
        
        // 检查当前类型是否存在可领取的成就
        uiItem.redPoint.visible = _curInfoDict != null && 
                                  _curInfoDict.Values.Any(info => 
                                  {
                                      var unit = ConfigUtils.GetAchievementUnitByAid(info.AId);
                                      //属于当前选项卡类型, 状态是可领取
                                      return unit != null && 
                                             unit.AchievementType == (int)targetType && 
                                             info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim;
                                  });
        
        bool x = uiItem.redPoint.visible;
    }

}