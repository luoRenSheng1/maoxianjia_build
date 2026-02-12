using System;
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using UnityEngine;
using Village;
using EventDispatcher = EngineBase.EventDispatcher;

public class BuildInfoView : UIViewBase
{ 
    private UI_BuildInfo BuildInfo => this.main as UI_BuildInfo;

    private CityInfo _buildInfo;
    private ConfigBuildUnit _buildUnit;
    private HelpType _helpType;

    private List<PetItemInfo> _petItemInfos;    // 存储所有宠物信息的列表
    private Dictionary<int, PetItemInfo> _upPetDict = new Dictionary<int, PetItemInfo>();    // 存储已上阵的宠物信息的字典
    private Dictionary<int, UI_PetShow> _petShowDict = new Dictionary<int, UI_PetShow>(3);    // 存储宠物上阵项的字典
    private PetItemInfo _uploadPet;    // 当前选择上阵的宠物信息
    private bool _isGuiding;
    public BuildInfoView()
    {
        this.name = "BuildInfo"; 
        this.package = "Village";
        this.component = "BuildInfo";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        VillageBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _buildInfo = values[0] as CityInfo;
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.BuildInfo.closeBtn.onClick.Add(this.Hide);
        this.BuildInfo.closeBtn.onClick.Add(this.HideWithSoundEffect);
        
        this.BuildInfo.titleCtrl.onChanged.Add(this.OnTitleCtrlChange);
        
        // 渲染宠物列表
        this.BuildInfo.petList2.itemRenderer = PetList2Render;
        this.BuildInfo.petList2.SetVirtual();
        
        this.BuildInfo.hideUpload1.onClick.Add(this.HideUploadPetView);
        this.BuildInfo.hideUpload2.onClick.Add(this.HideUploadPetView);
        
        // 处理查看奖励信息点击事件
        this.BuildInfo.itemBtn.onClick.Add(this.OnClickItemBtn);
        // 处理查看帮助点击事件
        this.BuildInfo.helpBtn.onClick.Add(this.OnClickHelpBtn);
        // 处理查看奖励信息点击事件
        this.BuildInfo.rewardBtn.onClick.Add(this.OnClickRewardBtn);
        // 处理领取奖励点击事件
        this.BuildInfo.getBtn.onClick.Add(this.OnClickGetBtn);
        
        for (int i = 0; i < 3; i++)
        {
            // 获取名为petUp0到petUp2的宠物上阵项
            UI_PetShow upPetShow = this.BuildInfo.GetChild("petUp" +i) as UI_PetShow;
            // 将获取到的宠物上阵项添加到_petShowDict字典中
            _petShowDict.Add(i, upPetShow);
            // 用于处理宠物上阵项的点击事件
            upPetShow?.onClick.Add(this.OnClickUpPetList);
            // 渲染当前宠物上阵项
            UpPetListRender(i, upPetShow);
        }
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC, this.UpPetSuccess);
        EventDispatcher.GameWorld.Regist<CityInfo>(EventDefine.EVENT_VILLAGE_ONE_BUILD, this.UpdateBuildInfo);
        EventDispatcher.GameWorld.Regist<PetItemInfo>(EventDefine.EVENT_VILLAGE_PET_REPLACE_PET, this.ShowUploadPetOpt);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC, this.UpPetSuccess);
        EventDispatcher.GameWorld.UnRegist<CityInfo>(EventDefine.EVENT_VILLAGE_ONE_BUILD, this.UpdateBuildInfo);
        EventDispatcher.GameWorld.UnRegist<PetItemInfo>(EventDefine.EVENT_VILLAGE_PET_REPLACE_PET, this.ShowUploadPetOpt);
    }

    protected override void OnShow()
    {
        base.OnShow();
        _buildUnit = ConfigUtils.GetBuildUnitByType(_buildInfo.BuildType);
        _helpType = (HelpType)_buildUnit.Id;
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_buildUnit.Item);
        this.BuildInfo.itemBtn.icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        this.BuildInfo.rewardBtn.icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        this.BuildInfo.titleCtrl.selectedIndex = (int) _buildInfo.BuildType - 1;
        this.BuildInfo.buildSpine.SetScale(1.06f, 0.9f);
        OnTitleCtrlChange();
        UpdateBuildView();

        //var stoneMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildStone);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageStoneAddPet) && stoneMap.Item1 && _buildInfo.BuildType == VillageBuildType.Stone)
        //{
        //    GuideManager.Instance.HideGuide();
        //    _isGuiding = true;
        //    GuideManager.Instance.StartGuide(this.BuildInfo.petList2.GetChildAt(0), GuideID.Trigger_Click_VillageStoneAddPet, PosType.Left, true, false);
        //}
        //引导-点击加工厂区首个宠物驻扎
        if (_buildInfo.BuildType == VillageBuildType.Factory && this.BuildInfo.petList2.numItems > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.BuildFactory,
            giding = GuideID.guideId_4201,
            gid = GuideID.guideId_4202,
            tui = this.BuildInfo.petList2.GetChildAt(0),
            isForce = true,
            isSend = false,
            pType = PosType.Left,
            npcTxt = "Beginner_Doc_032",
            npcPosType = PosType.Down,
            uiName = "BuildInfo"
        })) { _isGuiding = true; }

        //引导-点击石头矿区首个宠物驻扎
        if (!_isGuiding && _buildInfo.BuildType == VillageBuildType.Stone && this.BuildInfo.petList2.numItems > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.BuildStone,
            giding = GuideID.Trigger_Click_VillageStone,
            gid = GuideID.Trigger_Click_VillageStoneAddPet,
            tui = this.BuildInfo.petList2.GetChildAt(0),
            isForce = true,
            isSend = false,
            pType = PosType.Left,
            uiName = "BuildInfo",
            //npcTxt = "Beginner_Doc_002",
            //npcPosType = PosType.Down,
        })){ _isGuiding = true; }

        if (!_isGuiding && _buildInfo.BuildType == VillageBuildType.Factory)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Factory).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildFactory,
                giding = GuideID.Trigger_Click_VillageFactory,
                gid = GuideID.Trigger_Click_VillageFactoryGetReward,
                tui = this.BuildInfo.getBtn,
                isForce = true,
                isSend = true,
                pType = PosType.Left,
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { _isGuiding = true; }
        }

        if (!_isGuiding && _buildInfo.BuildType == VillageBuildType.FoodWorkshop)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.FoodWorkshop).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildFood,
                giding = GuideID.Trigger_Click_VillageFood,
                gid = GuideID.Trigger_Click_VillageFoodGetReward,
                tui = this.BuildInfo.getBtn,
                isForce = true,
                isSend = true,
                pType = PosType.Left,
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { _isGuiding = true; }
        }

        if (!_isGuiding && _buildInfo.BuildType == VillageBuildType.Explore)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Explore).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildExplore,
                giding = GuideID.Trigger_Click_VillageExplore,
                gid = GuideID.Trigger_Click_VillageExploreGetReward,
                tui = this.BuildInfo.getBtn,
                isForce = true,
                isSend = true,
                pType = PosType.Left,
                //scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { _isGuiding = true; }
        }

        if (!_isGuiding && _buildInfo.BuildType == VillageBuildType.Shack)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Shack).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildPet,
                giding = GuideID.Trigger_Click_VillageStack,
                gid = GuideID.Trigger_Click_VillageStackGetReward,
                tui = this.BuildInfo.getBtn,
                isForce = true,
                isSend = true,
                pType = PosType.Left,
                //scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { _isGuiding = true; }
        }

        if (!_isGuiding && _buildInfo.BuildType == VillageBuildType.Training)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Training).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildTrain,
                giding = GuideID.Trigger_Click_VillageTrain,
                gid = GuideID.Trigger_Click_VillageTrainGetReward,
                tui = this.BuildInfo.getBtn,
                isForce = true,
                isSend = true,
                pType = PosType.Left,
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { _isGuiding = true; }
        }

        this.BuildInfo.redDot.visible = VillageInfoManager.Instance.RewardLimit(_buildInfo.BuildType);
    }

    private void UpdateBuildView()
    {
        _petItemInfos = VillageInfoManager.Instance.GetAllIdleVillagePet();
        this.BuildInfo.petList2.numItems = _petItemInfos.Count;
        this.BuildInfo.hasReward.selectedIndex = _buildInfo.ItemNum > 0 ? 1 : 0;
        this.BuildInfo.rewardValue.text = StringUtils.FormatCurrency(_buildInfo.ItemNum);
    }

    protected override void OnHide()
    {
        base.OnHide();
        if(_isGuiding)
            GuideManager.Instance.HideGuide();
        _isGuiding = false;
        
        foreach (var item in _petShowDict)
        {
            Utils.ClearSpineModelOnFGUI(item.Value.spine);
        }
    }

    private void OnTitleCtrlChange()
    {
        UpPetSuccess();
    }

    // 点击宠物上阵项时的处理，包括上阵宠物、显示宠物详情
    private void OnClickUpPetList(EventContext context)
    {
        UI_PetShow item = context.sender as UI_PetShow;
        int index = int.Parse(item.name.Substring(5, 1));
        if (item.showHandCtrl.selectedIndex == 1)
        {
            if (_uploadPet.DispatchBuild != VillageBuildType.None)
            {
                var builder = PetSwitchBetweenBuilds_CS.CreateBuilder();
                builder.SrcBuild = (eBuildType) _uploadPet.DispatchBuild;
                var buildPetSlot1 = BuildPetSlot.CreateBuilder();
                buildPetSlot1.Index = (uint) _uploadPet.BuildInnerIndex;
                // buildPetSlot1.PetId = (uint) _uploadPet.petCfg.Id;
                buildPetSlot1.PetId = (ulong) _uploadPet.PetGuid;
                builder.SrcPetIds = buildPetSlot1.Build();

                builder.DstBuild = (eBuildType) _buildInfo.BuildType;
                var buildPetSlot2 = BuildPetSlot.CreateBuilder();
                buildPetSlot2.Index = (uint) index;
                // buildPetSlot2.PetId = (uint) _uploadPet.petCfg.Id;
                buildPetSlot2.PetId = (ulong) _uploadPet.PetGuid;
                builder.DstPetIds = buildPetSlot2.Build();
                
                VillageInfoManager.Instance.SetVillagePetByBuild(_uploadPet,VillageBuildType.None);
                VillageInfoManager.Instance.SetVillagePetByBuild(_petItemInfos[index],VillageBuildType.None);
                
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetSwitchBetweenBuilds_CS, builder.Build());
            }
            else
            {
                var builder = PetDispatched_CS.CreateBuilder();
                builder.Build = (eBuildType) _buildInfo.BuildType;
                var buildPetSlot = BuildPetSlot.CreateBuilder();
                buildPetSlot.Index = (uint) index;
                // buildPetSlot.PetId = (uint) _uploadPet.petCfg.Id;
                buildPetSlot.PetId = (ulong) _uploadPet.PetGuid;
                builder.PetIdsList.Add(buildPetSlot.Build());
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetDispatched_CS, builder.Build());
            }

        }
        else
        {
            if (item.hasPet.selectedIndex == 0)
            {
                OnClickToAddPet();
            }
            else
            {
                // 有宠物上阵时，展示宠物详情
                if (item != null && item.hasPet.selectedIndex == 1)
                {
                    if (this._upPetDict.TryGetValue(index, out var pet))
                    {
                        // 跳转到宠物详情界面
                        UIManager.Instance.ShowUIPanel("VillagePetInfo", pet,  _buildInfo);
                    }
                }
            }
        }
        
    }
    
    private void OnClickToAddPet()
    {
        UIManager.Instance.ToastByKey(10183);
    }

    // 渲染宠物上阵项，设置状态和数据
    private void UpPetListRender(int index, GObject item)
    {
        UI_PetShow btn = (UI_PetShow) item;
        if (this._upPetDict.TryGetValue(index, out var pet))
        {
            SetUpShowData(btn, pet, index);
        }
    }

    // 设置宠物上阵项的数据
    private void SetUpShowData(UI_PetShow item, PetItemInfo pet, int index)
    {
        pet.BattleIndex = index;
        item.hasPet.selectedIndex = 1;
        ((UI_qualityLabel)item.petName).qualityCtrl.selectedIndex = pet.petCfg.Quality - 1;
        ((UI_roleQualityItem)item.qualityLb).quality.selectedIndex = pet.petCfg.Quality - 1;
        // item.petName.text = pet.petCfg.Name;
        item.petName.text = ConfigUtils.GetTextById(pet.petCfg.Name);
        
        // 设置item的spine控件模型
        Utils.SetSpineModelOnFGUI(item.spine, pet.petCfg.PetModel, 60,"idle");
    }

    // 渲染宠物列表
    private void PetList2Render(int index, GObject item)
    {
        // 从 _petItemInfos 列表中获取索引为 index 的宠物配置信息
        ConfigPetBasisUnit petBasisUnit = _petItemInfos[index].petCfg;
        // ((UI_PetCom) item).petIcon.url = UIResource.GetPetIcon(petBasisUnit.IconPath);
        ((UI_PetCom) item).petIcon.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petBasisUnit.IconPath)).Icon);
        ((UI_PetCom) item).qualityCtrl.selectedIndex = petBasisUnit.Quality - 1;
        
        // 是否已经上阵
        bool isUp = _petItemInfos[index].DispatchBuild != VillageBuildType.None;
        ((UI_PetCom) item).isStation.selectedIndex = isUp ? 0 : 1;
        // ((UI_PetCom) item).stationIcon.url = UIResource.GetImageUrlWithLang("yzz", "Village");
        
        ((UI_PetCom) item).lvLb.text = _petItemInfos[index].PetLv.ToString();
        if (isUp)
        {
            // 设置宠物上阵的建筑信息
            ((UI_PetCom) item).buildType.selectedIndex = (int) _petItemInfos[index].DispatchBuild - 1;
        }

        ((UI_PetCom) item).data = _petItemInfos[index];
        ((UI_PetCom) item).onClick.Set(this.OnClickPetItem);
        
        // 红点
        ((UI_PetCom) item).redPoint.visible = false;
        if (!isUp)
        {//没有上阵且品质较高的展示红点  _petItemInfos已经按照规则排序处理过了
            int count = VillageInfoManager.Instance.GetNoUploadCountByBuild(_buildInfo.BuildType);
            if (index < count)
            {
                ((UI_PetCom) item).redPoint.visible = true;
            }
        }
    }

    // 点击宠物展示宠物家园的信息
    private void OnClickPetItem(EventContext context)
    {
        PetItemInfo pet = (context.sender as UI_PetCom)?.data as PetItemInfo;
        if (pet != null)
        {
            if (pet.DispatchBuild != VillageBuildType.None && pet.DispatchBuild != _buildInfo.BuildType)
            {
                UIManager.Instance.ShowUIPanel("Hint", pet, _buildInfo);
            }
            else
            {
                UIManager.Instance.ShowUIPanel("VillagePetInfo", pet, _buildInfo);
            }

        }
    }
    

    // 更新宠物成功上阵后的处理
    private void UpPetSuccess()
    {
        List<PetItemInfo> petItemInfos = VillageInfoManager.Instance.GetVillagePetByBuild(_buildInfo.BuildType);
        _upPetDict.Clear();
        foreach (var item in petItemInfos)
        {
            _upPetDict.Add(item.BuildInnerIndex, item);
        }
        _petItemInfos = VillageInfoManager.Instance.GetAllIdleVillagePet();
        this.BuildInfo.petList2.numItems = _petItemInfos.Count;
        RefreshUpItem();
        HideUploadPetView();
    }

    // 刷新宠物上传项数据
    private void RefreshUpItem()
    {
        GameManager.Instance.TimerManager.ClearTimer(RefreshUpPets);
        GameManager.Instance.TimerManager.SetTimer(0.1f, RefreshUpPets);
        double addValue = 0;
        double capacityValue = 0;
        if (_buildInfo.BuildType != VillageBuildType.Stone)
            capacityValue = _buildUnit.Number; //buildUnit.Number 初始产量
        else if (_buildInfo.BuildType == VillageBuildType.Stone)
        {
            var stageId = MapObjectManager.Instance.GuanKaStageId;
            List<ConfigStageUnit> stageUnits =
                ConfigUtils.GetStageUnitById(stageId);
            double gold = 0;
            foreach (var item in stageUnits)
            {
                gold += double.Parse(item.Gold);
            }
            capacityValue = gold *
                            int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(600009).Param1) *
                            ConstDefine.CONFIG_PLACE_EX;
        }

        List<PetItemInfo> petItemInfos = VillageInfoManager.Instance.GetVillagePetByBuild(_buildInfo.BuildType);
        //遍历所有上阵宠物列表
        foreach (var item in petItemInfos)
        {
            addValue += VillageInfoManager.Instance.GetVillagePetAdd(item.petCfg.Quality) / 100f;
        }
        
        capacityValue *= (1 + addValue);
        if (_buildInfo.BuildType == VillageBuildType.Stone)
            capacityValue = Math.Ceiling(capacityValue);
        this.BuildInfo.addValue.SetVar("value", StringUtils.FormatCurrency(addValue * 100)).FlushVars();
        this.BuildInfo.capacityValue.text = StringUtils.FormatCurrency(capacityValue);
        this.BuildInfo.timeLb.SetVar("value", Mathf.RoundToInt(_buildUnit.ProduceTime / 60f).ToString()).FlushVars();

    }

    // 刷新宠物上阵项的状态，包括显示已上阵宠物的信息
    private void RefreshUpPets()
    {
        foreach (var item in _petShowDict)
        {
            if (_upPetDict.TryGetValue(item.Key, out var pet))
            {
                item.Value.hasPet.selectedIndex = 1;
                this.SetUpShowData(item.Value, pet, item.Key);
            }
            else
            {
                Utils.ClearSpineModelOnFGUI(item.Value.spine);
                item.Value.hasPet.selectedIndex = 0;
            }
        }
    }

    // 查看奖励物品的详细信息的点击事件
    private void OnClickItemBtn()
    {
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_buildUnit.Item);
        if(itemTypeUnit != null)
            TipsManger.Instance.ShowPopupTip(this.BuildInfo.itemBtn, Tipstype.Item, itemTypeUnit.Id);
    }

    // 查看帮助的点击事件
    private void OnClickHelpBtn()
    {
        UIManager.Instance.ShowUIPanel("Help", _helpType);
    }

    // 查看奖励物品的详细信息的点击事件
    private void OnClickRewardBtn()
    {
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_buildUnit.Item);
        if(itemTypeUnit != null)
            TipsManger.Instance.ShowPopupTip(this.BuildInfo.rewardBtn, Tipstype.Item, itemTypeUnit.Id);
    }

    // 领取奖励按钮的点击事件
    private void OnClickGetBtn()
    {
        if (this.BuildInfo.hasReward.selectedIndex == 1)
        {
            // 发送领取奖励请求
            var builder = ObtainProduct_CS.CreateBuilder();
            builder.Build = (eBuildType) _buildInfo.BuildType;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ObtainProduct_CS, builder.Build());
            
            SetVisible(false);
        }
    }

    private void UpdateBuildInfo(CityInfo cityInfo)
    {
        if (_buildInfo.BuildType == cityInfo.BuildType)
        {
            _buildInfo = cityInfo;
            UpdateBuildView();
        }
    }

    private void ShowUploadPetOpt(PetItemInfo pet)
    {
        this.BuildInfo.showPetHand.selectedIndex = 1;
        this.BuildInfo.hideUpload1.y = this.BuildInfo.rloader.y;
        this.BuildInfo.hideUpload2.height = this.BuildInfo.upGroup.y;
        _uploadPet = pet;
        foreach (var item in _petShowDict)
        {
            item.Value.showHandCtrl.selectedIndex = 1;
        }
    }
    
    private void HideUploadPetView()
    {
        this.BuildInfo.showPetHand.selectedIndex = 0;
        foreach (var item in _petShowDict)
        {
            item.Value.showHandCtrl.selectedIndex = 0;
        }
    }
}