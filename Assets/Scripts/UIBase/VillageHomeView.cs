
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
using Enum = System.Enum;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;
using Stage = FairyGUI.Stage;

public class VillageHomeView : UIViewBase
{
    private const int X = 50;
    private const int Y = 30;

    private UI_VillageHome VillageHome => this.main as UI_VillageHome;

    private List<Vector2> _pathList = new List<Vector2>();
    private const int RandomMax = 13;
    private bool _isStarPetPatrol;
    private float _startPetPartrolTime;
    private List<MapVillagePetObject> _idlePetLst = new List<MapVillagePetObject>();
    private DijkstraHelp _dijkstraHelp;

    private Dictionary<VillageBuildType, UI_VillageBuildItem> _villageBuildDict = new Dictionary<VillageBuildType, UI_VillageBuildItem>();

    private ConfigCommonUnit _weatherComm;
    private ConfigCommonUnit _commonUnit = ConfigDataGroup.GetInstance<ConfigCommon>().Get(32);
    // private PinchGesture _gesture3;
    private ConfigCommonUnit _commonUnit101 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(101);
    private bool _isGuiding;
    public VillageHomeView()
    {
        this.name = "VillageHome";
        this.package = "Village";
        this.component = "VillageHome";
        this.removePackage = true;
        this.safeAreaInset = false;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        VillageBinder.BindAll();
    }

    public GComponent GetPetRoot()
    {
        return VillageHome.panel.mapPanel;
    }

    public void GoToVillageBuild(VillageBuildType buildType, bool isClick = false)
    {
        if (_villageBuildDict.TryGetValue(buildType, out var buildItem))
        {
            if (isClick)
                buildItem.OnClickBuild();
            this.VillageHome.panel.mapPanel.scrollPane.posX = (buildItem.x - buildItem.width / 2);
            this.VillageHome.panel.mapPanel.scrollPane.posY = (buildItem.y - buildItem.height / 2);
        }
        this.VillageHome.panel.mapPanel.EnsureBoundsCorrect();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.VillageHome.userPanel.SetUISafeAreaOffset();
        this.VillageHome.panel.SetSize(GRoot.inst.width, GRoot.inst.height);

        this.VillageHome.panel.mapPanel.mapBg.data = -999999;
        for (int i = 0; i <= 13; i++)
        {
            GGraph p = this.VillageHome.panel.mapPanel.GetChild("p" + i).asGraph;
            _pathList.Add(new Vector2(p.x, p.y));
        }

        _dijkstraHelp = new DijkstraHelp();
        _dijkstraHelp.Init(RandomMax);

        _weatherComm = ConfigDataGroup.GetInstance<ConfigCommon>().Get(100);

        this.InitAllVillageBuild();

        // _gesture3 = new PinchGesture(this.VillageHome);
        // _gesture3.onAction.Add(OnPinch);

        Stage.inst.onTouchBegin.AddCapture(__stageTouchBegin);

        // this.VillageHome.panel.btnClose.onClick.Add(this.Hide);
        this.VillageHome.panel.btnClose.onClick.Add(this.HideWithSoundEffect);

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, UpdateRoleInfo);
        EventDispatcher.GameWorld.Regist<CityInfo>(EventDefine.EVENT_VILLAGE_ONE_BUILD, this.UpdateVillageCityLevel);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_VILLAGE_ALL_BUILD, this.UpdateAllVillageCityInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_VILLAGE_ITEM_UPDATE, this.UpdateAllVillageCityInfo);
        EventDispatcher.GameWorld.Regist<int, int, ulong, ulong>(EventDefine.EVENT_VILLAGE_DISPATCH_PET_PC, this.OnVillagePetDispatchPC);
        EventDispatcher.GameWorld.Regist<int, ulong>(EventDefine.EVENT_VILLAGE_PET_ONCLICK, this.OnVillagePetOnClick);

        MinScale = Mathf.Min(1, float.Parse(_commonUnit101.Param1));
        MaxScale = float.Parse(_commonUnit101.Param2);
    }

    private void __stageTouchBegin(EventContext context)
    {
        if (GuideManager.Instance.IsShowGuiding)
        {
            this.VillageHome.panel.mapPanel.scrollPane.touchEffect = false;
        }
        else
        {
            if (Stage.inst.touchCount >= 2)
            {
                this.VillageHome.panel.mapPanel.scrollPane.touchEffect = false;
            }
            else
            {
                this.VillageHome.panel.mapPanel.scrollPane.touchEffect = true;
            }
        }
    }
    private float MinScale = 1f;
    private float MaxScale = 8.0f;
    private float scale = 1.0f;

    void OnPinch(EventContext context)
    {
        if (_isGuiding) return;
        PinchGesture gesture = (PinchGesture)context.sender;
        if (gesture.delta != 0)
        {
            this.VillageHome.panel.mapPanel.scrollPane.touchEffect = true;
        }
        else
        {
            this.VillageHome.panel.mapPanel.scrollPane.touchEffect = false;
        }
        scale += gesture.delta;
        scale = Mathf.Clamp(scale, MinScale, MaxScale);
        float scaleX = Mathf.Lerp(this.VillageHome.panel.mapPanel.scale.x, scale, 0.5f);
        float scaleY = Mathf.Lerp(this.VillageHome.panel.mapPanel.scale.y, scale, 0.5f);
        this.VillageHome.panel.mapPanel.scale = new Vector2(scaleX, scaleY);
        this.VillageHome.panel.mapPanel.EnsureBoundsCorrect();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        // _gesture3.onAction.Remove(OnPinch);
        Stage.inst.onTouchBegin.RemoveCapture(__stageTouchBegin);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, UpdateRoleInfo);
        EventDispatcher.GameWorld.UnRegist<CityInfo>(EventDefine.EVENT_VILLAGE_ONE_BUILD, this.UpdateVillageCityLevel);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_VILLAGE_ALL_BUILD, this.UpdateAllVillageCityInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_VILLAGE_ITEM_UPDATE, this.UpdateAllVillageCityInfo);
        EventDispatcher.GameWorld.UnRegist<int, int, ulong, ulong>(EventDefine.EVENT_VILLAGE_DISPATCH_PET_PC, this.OnVillagePetDispatchPC);
        EventDispatcher.GameWorld.UnRegist<int, ulong>(EventDefine.EVENT_VILLAGE_PET_ONCLICK, this.OnVillagePetOnClick);
        
        GrimoireManager.Instance.isChangeUI = false;
    }

    private Vector2 GetRandomPos(int index)
    {
        return _pathList[index];
    }

    protected override void OnHide()
    {
        if (_isGuiding)
        {
            GuideManager.Instance.HideGuide();
        }
        _isStarPetPatrol = false;
        _isGuiding = false;

        ClearWeather();

        GList mapList = this.VillageHome.panel.mapPanel.mapBg.GetChild("mapList").asList;
        for (int i = 0; i < 4; i++)
        {
            mapList.GetChildAt(i).asLabel.icon = null;
        }

        for (int i = MapVillagePetManager.Instance.lstPetObj.Count - 1; i >= 0; i--)
        {
            MapVillagePetManager.Instance.DestroyMapObject(MapVillagePetManager.Instance.lstPetObj[i]);
        }
        MapVillagePetManager.Instance.lstPetObj.Clear();

        this.VillageHome.panel.mapPanel.scrollPane.posX = 400;
        this.VillageHome.panel.mapPanel.scrollPane.posY = 400;
        this.VillageHome.panel.mapPanel.scale = Vector2.one;

        var builder = LeaveHomeTown_CS.CreateBuilder();
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_LeaveHomeTown_CS, builder.Build());

        VillageInfoManager.Instance.IsInVillageHome = false;
        base.OnHide();
        
        GrimoireManager.Instance.isChangeUI = false;
    }

    private void UpdateIdlePetList()
    {
        _idlePetLst.Clear();

        List<PetItemInfo> petList = VillageInfoManager.Instance.GetAllIdleVillagePet();
        foreach (var item in petList)
        {
            PetVo pet = new PetVo();
            pet.unitID = item.petCfg.Id;
            pet.generalsType = item.petCfg.Id;
            int randomIndex = Random.Range(0, RandomMax - 1);
            pet.pos = new Vector3(this.GetRandomPos(randomIndex).x, GetRandomPos(randomIndex).y) +
                      new Vector3(Random.Range(-X, X), Random.Range(-Y, Y));
            pet.rot = Quaternion.Euler(0, 90, 0);
            MapVillagePetManager.Instance.AddVillagePet(pet, randomIndex, (ulong)item.PetGuid);
        }

        foreach (var pet in MapVillagePetManager.Instance.lstPetObj)
        {
            _idlePetLst.Add(pet);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        VillageInfoManager.Instance.IsInVillageHome = true;
        //加载地图
        var mapList = this.VillageHome.panel.mapPanel.mapBg.GetChild("mapList").asList;
        mapList.itemRenderer = (index, item) =>
        {
            ((GLabel)item).GetChild("icon").asLoader.fixMode = true;
            ((GLabel)item).icon = UIResource.GetVillageMapItem(index + 1);
        };
        mapList.numItems = 4;
        mapList.ResizeToFit();

        this.VillageHome.panel.mapPanel.scrollPane.posX = 400;
        this.VillageHome.panel.mapPanel.scrollPane.posY = 400;
        this.VillageHome.panel.mapPanel.scale = Vector2.one;

        UpdateIdlePetList();
        _isStarPetPatrol = true;

        if (!VillageInfoManager.Instance.IsGetServer)
        {
            var builder = GetHometown_CS.CreateBuilder();
            GetHometown_CS hometownCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_Hometown_CS, hometownCs);
        }
        else
        {
            UpdateAllVillageCityInfo();
        }

        UpdateRoleInfo();

        //随机天气系统
        RandomShowWeather();

        this.VillageHome.panel.mapPanel.EnsureBoundsCorrect();
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 30, 0.2f, () =>
        {
            GuideToVillageHome(false);
        });

        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.HOMEBGM);
    }

    private void GuideToVillageHome(bool isFirst)
    {
        if (!IsShow() || !IsOnStage()) return;
        _isGuiding = false;
        float lastPox = 0;
        float lastPoy = 0;
        if (!isFirst)
        {
            lastPox = this.VillageHome.panel.mapPanel.scrollPane.posX;
            lastPoy = this.VillageHome.panel.mapPanel.scrollPane.posY;
        }

        // var stoneMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildStone);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageStone) && stoneMap.Item1)
        //{
        //    GoToVillageBuild(VillageBuildType.Stone, false);
        //    GuideManager.Instance.HideGuide();
        //    _isGuiding = true;
        //    GuideManager.Instance.StartGuide(this.VillageHome.panel.mapPanel.villageBuild6, GuideID.Trigger_Click_VillageStone, PosType.Left, true, false, new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy));
        //}
        //引导-点击首个宠物，进行驻扎驻扎
        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.BuildStone,
            giding = GuideID.Trigger_Click_VillageBtn,
            gid = GuideID.Trigger_Click_VillageStone,
            tui = this.VillageHome.panel.mapPanel.villageBuild6,
            isForce = true,
            isSend = false,
            pType = PosType.Left,
            scale = new Vector2(0.6f, 0.6f),
            scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
            //npcTxt = "Beginner_Doc_002",
            //npcPosType = PosType.Down,
        }))
        {
            GoToVillageBuild(VillageBuildType.Stone, false);
            _isGuiding = true;
        }

        if (!_isGuiding)
        {
            if (VillageInfoManager.Instance.GetVillagePetByBuild(VillageBuildType.Factory).Count <= 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildFactory,
                giding = GuideID.guideId_4200,
                gid = GuideID.guideId_4201,
                tui = this.VillageHome.panel.mapPanel.villageBuild2,
                isForce = true,
                isSend = false,
                pType = PosType.Left,
                scale = new Vector2(0.6f, 0.6f),
                scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { GoToVillageBuild(VillageBuildType.Factory, false); _isGuiding = true; }
            else if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Factory).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildFactory,
                giding = GuideID.Trigger_Click_VillageBtn2,
                gid = GuideID.Trigger_Click_VillageFactory,
                tui = this.VillageHome.panel.mapPanel.villageBuild2,
                isForce = true,
                isSend = false,
                pType = PosType.Left,
                scale = new Vector2(0.6f, 0.6f),
                scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { GoToVillageBuild(VillageBuildType.Factory, false); _isGuiding = true; }
        }

        if (!_isGuiding)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.FoodWorkshop).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildFood,
                giding = GuideID.Trigger_Click_VillageBtn6,
                gid = GuideID.Trigger_Click_VillageFood,
                tui = this.VillageHome.panel.mapPanel.villageBuild1,
                isForce = true,
                isSend = false,
                pType = PosType.Left,
                scale = new Vector2(0.6f, 0.6f),
                scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { GoToVillageBuild(VillageBuildType.FoodWorkshop, false); _isGuiding = true; }
        }

        if (!_isGuiding)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Explore).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildExplore,
                giding = GuideID.Trigger_Click_VillageBtn5,
                gid = GuideID.Trigger_Click_VillageExplore,
                tui = this.VillageHome.panel.mapPanel.villageBuild3,
                isForce = true,
                isSend = false,
                pType = PosType.Left,
                scale = new Vector2(0.6f, 0.6f),
                scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { GoToVillageBuild(VillageBuildType.Explore, false); _isGuiding = true; }
        }

        if (!_isGuiding)
        {
            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Shack).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildPet,
                giding = GuideID.Trigger_Click_VillageBtn4,
                gid = GuideID.Trigger_Click_VillageStack,
                tui = this.VillageHome.panel.mapPanel.villageBuild4,
                isForce = true,
                isSend = false,
                pType = PosType.Left,
                scale = new Vector2(0.6f, 0.6f),
                scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { _isGuiding = true; }
        }

        if (!_isGuiding)
        {
            //var trainMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildTrain);
            //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageTrain) && trainMap.Item1)
            //{
            //    CityInfo cityInfo = VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Training);
            //    if (cityInfo.ItemNum > 0)
            //    {
            //        GoToVillageBuild(VillageBuildType.Training, false);
            //        GuideManager.Instance.HideGuide();
            //        _isGuiding = true;
            //        GuideManager.Instance.StartGuide(this.VillageHome.panel.mapPanel.villageBuild5, GuideID.Trigger_Click_VillageTrain, PosType.Left, true, false, new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy));
            //    }
            //}

            if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Training).ItemNum > 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildTrain,
                giding = GuideID.Trigger_Click_VillageBtn3,
                gid = GuideID.Trigger_Click_VillageTrain,
                tui = this.VillageHome.panel.mapPanel.villageBuild5,
                isForce = true,
                isSend = false,
                pType = PosType.Left,
                scale = new Vector2(0.6f, 0.6f),
                scrollPos = new Vector2(this.VillageHome.panel.mapPanel.scrollPane.posX - lastPox, this.VillageHome.panel.mapPanel.scrollPane.posY - lastPoy),
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })) { GoToVillageBuild(VillageBuildType.Training, false); _isGuiding = true; }
        }

        if (_isGuiding)
        {
            UIManager.Instance.CloseAllVillagePanel();
        }
    }

    private void RandomShowWeather()
    {
        ClearWeather();
        int time = int.Parse(_weatherComm.Param1);
        int stopTime = int.Parse(_weatherComm.Param3);
        string[] weatherArr = _weatherComm.Param2.Split('|');
        int index = Random.Range(0, weatherArr.Length);
        if (index == 0)//没有天气
            return;
        //雨 风 花 UI_Eff_weather1 UI_Eff_weather2 UI_Eff_weather3
        Utils.ShowUIPrefab(this.VillageHome.weatherPos, "UI_Eff_weather" + weatherArr[index], 100);
        GameManager.Instance.TimerManager.ClearTimer(ClearWeather);
        GameManager.Instance.TimerManager.SetTimer(stopTime, ClearWeather);
        GameManager.Instance.TimerManager.ClearTimer(RandomShowWeather);
        GameManager.Instance.TimerManager.SetTimer(time, this.RandomShowWeather);
    }

    private void ClearWeather()
    {
        if (this.VillageHome != null)
            Utils.HideUIPrefab(this.VillageHome.weatherPos);
        GameManager.Instance.TimerManager.ClearTimer(ClearWeather);
        GameManager.Instance.TimerManager.ClearTimer(RandomShowWeather);
    }

    private void UpdateAllVillageCityInfo()
    {
        UpdateAllBuild();
    }


    private void InitAllVillageBuild()
    {
        Array villageBuildTypes = Enum.GetValues(typeof(VillageBuildType));
        for (int i = 1; i < villageBuildTypes.Length; i++)
        {
            VillageBuildType key = (VillageBuildType)villageBuildTypes.GetValue(i);
            _villageBuildDict.Add(key, (UI_VillageBuildItem)this.VillageHome.panel.mapPanel.GetChild("villageBuild" + (int)key));
        }

        foreach (var item in _villageBuildDict)
        {
            item.Value.InitVillageBuild(item.Key);
        }
    }

    private void UpdateAllBuild()
    {
        foreach (var item in _villageBuildDict)
        {
            item.Value.UpdateBuildInfo();
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _startPetPartrolTime += Time.deltaTime;
        PetRandomPatrol();
    }

    private void PetRandomPatrol()
    {
        if (_isStarPetPatrol)
        {
            if (_startPetPartrolTime < 5f)
            {
                return;
            }

            _startPetPartrolTime = 0;
            for (int i = _idlePetLst.Count - 1; i >= 0; i--)
            {
                if (Random.value > 0.5f)
                {
                    var lstPos = new List<Vector3>();
                    lstPos.Add(_idlePetLst[i].Position);
                    int end = Random.Range(0, Random.Range(RandomMax - 3, RandomMax - 1));
                    int start = _idlePetLst[i].PetAttr.PosIndex;
                    var lstPath = _dijkstraHelp.GetShortedPath(start, end);
                    if (lstPath == null) return;
                    // LogUtils.LogWarning("Find: " +  start + " " + end);
                    foreach (var path in lstPath)
                    {
                        // LogUtils.LogWarning(path.ToString());
                        lstPos.Add(new Vector3(_pathList[path].x, _pathList[path].y, 0) + new Vector3(Random.Range(-X, X), Random.Range(-Y, Y)));
                    }

                    _idlePetLst[i].UpdateMoveByPath(lstPos, null, (id) =>
                    {
                        MapVillagePetObject petObject = GetVillagePetById(id);
                        Vector3 pos = petObject.Position;
                        petObject.SetPosition(pos, true);
                        petObject.PetAttr.PosIndex = end;
                    });
                }

            }
        }
    }

    private MapVillagePetObject GetVillagePetById(int unitId)
    {
        foreach (var item in _idlePetLst)
        {
            if (item.PetAttr.unitID == unitId)
            {
                return item;
            }
        }

        return null;
    }

    private void UpdateRoleInfo()
    {
        if (IsShow() && IsOnStage())
        {
            ((UI_ComUserInfo)this.VillageHome.userPanel.userInfo).UpdateUserInfo();
        }
    }

    private void UpdateVillageCityLevel(CityInfo cityInfo)
    {
        if (IsShow() && IsOnStage())
        {
            if (cityInfo.BuildType != VillageBuildType.None)
            {
                if (_villageBuildDict.TryGetValue(cityInfo.BuildType, out var item))
                {
                    item.UpdateBuildInfo();
                }
            }
            else
            {
                UpdateAllBuild();
            }

        }
    }

    private void OnVillagePetDispatchPC(int newPetId, int oldPetId, ulong oldPetGuid, ulong newPetGuid)
    {
        if (newPetId != oldPetId)
        {
            //宠物改变了

            MapVillagePetObject oldPet = MapVillagePetManager.Instance.GetMapObjectById(oldPetGuid);
            PetVo pet = new PetVo();
            pet.unitID = newPetId;
            pet.generalsType = newPetId;
            int randomIndex = oldPet.PetAttr.PosIndex;
            pet.pos = new Vector3(this.GetRandomPos(randomIndex).x, GetRandomPos(randomIndex).y) + new Vector3(Random.Range(-X, X), Random.Range(0, Y));
            pet.rot = Quaternion.Euler(0, 90, 0);
            MapVillagePetManager.Instance.AddVillagePet(pet, randomIndex, newPetGuid);
            MapVillagePetManager.Instance.DestroyMapObject(oldPet);
        }
    }

    private void OnVillagePetDispatchStart(List<PetItemInfo> lstPet)
    {
        foreach (var item in lstPet)
        {
            MapVillagePetObject villagePetObject = MapVillagePetManager.Instance.GetMapObjectById((ulong)item.petCfg.Id);
            if (villagePetObject != null)
            {
                // PetGoDispatchPath(villagePetObject, (VillageBuildType) item.DispatchBuild);
            }
        }

    }

    private void OnVillagePetOnClick(int petId, ulong petGuid)
    {
        PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet(petGuid);
        if(petItemInfo == null) { return; }
        MapVillagePetObject villagePet = MapVillagePetManager.Instance.GetMapObjectById(petGuid);
        if (villagePet != null)
        {
            if (petItemInfo.DispatchBuild == (int)eBuildType.eBuildType_None)
            {
                int index = Random.Range(0, petItemInfo.petCfg.PetQuotesId.Count - 1);
                int quotesId = petItemInfo.petCfg.PetQuotesId[index];
                // ConfigPetQuotesUnit quotesUnit = ConfigUtils.GetPetQuotesUnitById(quotesId);
                // villagePet.ShowTalk(quotesUnit.Desc);
                villagePet.ShowTalk(ConfigUtils.GetPetQuotesByKey(quotesId));
            }
        }
    }

    private void UpdateWildPetList()
    {
        UpdateIdlePetList();
    }

}
