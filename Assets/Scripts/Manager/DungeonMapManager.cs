using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Config;
using DungeonMap;
using UnityEngine;
using Engine;
using EngineBase;
using FairyGUI;
using Lobby;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;
using RandomEventMonster = Engine.RandomEventMonster;


public enum DungeonType
{
    NONE = 0,
    Gold,
    Zhuzhao,
    Diamond,
    Exp,
    PetMaterial,
    PetSkillBook,
    GodEquip,
    Holy,
    //Rune,
    BossWithPet = 100,  //挑战游荡的boss和宠物
    FishingBoss = 101,//钓鱼boss：普通boss、传承boss
}

public class DungeonMapManager : TSingleton<DungeonMapManager>
{
    public bool IsInCopy;
    public ulong EndTime;
    private bool isShowBossFight = false;

    private static int MonsterUnitId = 10000;//怪物自增ID 从10000开始

    // 4      1
    // 3     主角
    // 5      2
    private static List<int> petPosY => MapObjectManager.petPosY;

    private static List<int> petPosX => MapObjectManager.petPosX;

    public void Tick(float deltaSeconds)
    {
        if(!IsInCopy) return;
        MapObjectManager.Instance.TickByMapObject(deltaSeconds * MapObjectManager.Instance.Speed);
        LateTickByMapObject(deltaSeconds);
        UpdateState(GuanKaStep);
    }
    
    public void LateTickByMapObject(float deltaSeconds)
    {
        MapObjectManager.Instance.LateTickByMapObject(deltaSeconds * MapObjectManager.Instance.Speed);
    }
    
    public void PlayCommonTimeSpine()
    {
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            lobbyView.PlayCommonTimeSpine(false);
        }
    }
    
    public void StopCommonTimeSpine()
    {
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            lobbyView.StopCommonTimeSpine();
        }
    }

    /// <summary>
    /// 副本入口按钮红点
    /// </summary>
    public bool DungeonRedPoint()
    {
        List<ConfigDungeonChapterUnit> list = new List<ConfigDungeonChapterUnit>();
        list = ConfigDataGroup.GetInstance<ConfigDungeonChapter>().Data.Values.ToList();
        var filteredList = list.Where(item => item.Id <= 100000).ToList();//移除随机事件副本
        foreach (var item in filteredList)
        {
            var funcType = GetDungeonFuncType((DungeonType) item.Type);
            var dungeonMap = FuncPreviewManger.Instance.GetFuncOpenState(funcType);
            if (dungeonMap.Item1)
            {
                if (ItemInfoManager.Instance.GetItemCount(item.ItemID) > 0 || AdManager.Instance.GetAdFreeTimes(1000+(int) item.Type) > 0)
                {
                    return true;
                }
                
            }
        }

        return false;
    }
    
    public FuncOpenType GetDungeonFuncType(DungeonType type)
    {
        FuncOpenType openType = 0;
        switch (type)
        {
            case DungeonType.Diamond:
                openType = FuncOpenType.Dungeon_dimoand;
                break;
            case DungeonType.Gold:
                openType = FuncOpenType.Dungeon_gold;
                break;
            case DungeonType.Zhuzhao:
                openType = FuncOpenType.Dungeon_zhuzhao;
                break;
            case DungeonType.Exp:
                openType = FuncOpenType.Dungeon_exp;
                break;
            case DungeonType.PetMaterial:
                openType = FuncOpenType.Dungeon_petMatial;
                break;
            case DungeonType.PetSkillBook:
                openType = FuncOpenType.Dungeon_petSkillBook;
                break;
            case DungeonType.GodEquip:
                openType = FuncOpenType.Dungeon_godEquip;
                break;
            case DungeonType.Holy:
                openType = FuncOpenType.Dungeon_holy;
                break;
        }

        return openType;
    }


    #region 关卡有限状态机

    public EN_GUANKA_STEP GuanKaStep { get; private set; } = EN_GUANKA_STEP.INIT;

    public DungeonType dungeonType;
    public int GuanKaMonsterIndex { get; private set; }

    private CTimer timerShow1 = new CTimer();
    private CTimer timerShow2 = new CTimer();
    private CTimer timreFightResult = new CTimer();
    
    public int GuanKaStage;//难度
    public bool IsMonsterBoss { get; private set; } = true;
    public int MonsterBossTime { get; private set; }
    public CTimer timerMonsterBoss { get; private set; } = new CTimer();
    public bool fightLose { get; private set; } = false;
    public int MaxGuanKaMonsterIndex { get; private set; }

    private ConfigDungeonChapterUnit dungeonChapterUnit;

    public int normalStageId;
    public RandomEventData mapEventData { get; set; }
    public int batchStuffId;
    
    // 遗迹
    public RandomEventMonster eventMonsterData { get; set; }
    // public int eventMonsterIndexId;

    public void InitGuanKaFSM(DungeonType dungeonType)
    {
        RoleManager.Instance.EndSkillProxyDict();
        // GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.MAP);
        // GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.DUNGEON);

        MapObjectManager.Instance.isHeroPlayNormalSkill = false;
        MapObjectManager.Instance.isHeroPlayXPSkill = false;
        
        // 通关
        this.dungeonType = dungeonType;
        var hero = MapObjectManager.Instance.GetLocalHero();
        
        if (hero != null)
        {
            hero.IsAutoAttack = false;
            hero.ClearMovePath();
            hero.EndCurSkill();
        }
        
        foreach (var t in MapObjectManager.Instance.lstPetObj)
        {
            t.IsAutoAttack = false;
            t.ClearMovePath();
            t.EndCurSkill();
        }

        isShowBossFight = false;
        GuanKaMonsterIndex = 0;
        GuanKaStep = EN_GUANKA_STEP.INIT;
        EnterState(GuanKaStep);
    }

    public void ChangeState(EN_GUANKA_STEP newState)
    {
        if (GuanKaStep == newState)
        {
            return;
        }
        
        ExitState(GuanKaStep, newState);

        GuanKaStep = newState;

        EnterState(GuanKaStep);
    }

    private void InitHeroAndPets()
    {
        if (MapObjectManager.Instance.GetLocalHero() == null)
        {
            // 初始形态，刷英雄，刷第一波怪
            var hero = new HeroVo();
            hero.unitID = 1;
            hero.generalsType = HeroInfoManager.Instance.GetMyHero().HeroUnit.Id;
            hero.pos = new Vector3(0 + 540 * Math.Max(GuanKaMonsterIndex, 0), MapObjectManager.HERO_Y, 0);
            hero.rot = Quaternion.Euler(0, 90, 0);
            MapObjectManager.Instance.UpdateHero(hero);
            
            List<PetItemInfo> petInfoList = PetInfoManager.Instance.GetBattlePetList();
            for (int i=petInfoList.Count-1; i >=0; i--)
            {
                PetVo pet = new PetVo();
                pet.unitID = 100 + i;
                pet.generalsType = petInfoList[i].petCfg.Id;
                pet.BattleIndex = petInfoList[i].BattleIndex;
                pet.pos = new Vector3(0 + MapObjectManager.Instance.GetLocalHero().Position.x - MapObjectManager.FrontX2 + petPosX[petInfoList[i].BattleIndex], petPosY[petInfoList[i].BattleIndex], 0);
                pet.rot = Quaternion.Euler(0, 90, 0);
                MapObjectManager.Instance.UpdatePet(pet, petInfoList[i]);
                for (int j = MapObjectManager.Instance.lstPetObj.Count-1; j >=0; j--)
                {
                    MapObjectManager.Instance.lstPetObj[j].UpdatePetTotalAttribute();
                }
            }
        }
        else
        {
            MapObjectManager.Instance.GetLocalHero().UpdateHeroTotalAttr();
            MapObjectManager.Instance.GetLocalHero().UpdateHpBar();
        }
        RoleManager.Instance.SetHeroSkillProxy();
    }

    protected void EnterState(EN_GUANKA_STEP state)
    {
        switch (state)
        {
            case EN_GUANKA_STEP.INIT:
                {
                    MapObjectManager.Instance.DestroyMapObjectById(1);
                    foreach (var pet in MapObjectManager.Instance.lstPetObj)
                    {
                        MapObjectManager.Instance.DestroyMapObjectById(pet.Attr.unitID);
                    }
                    MapObjectManager.Instance.lstPetObj.Clear();
                    for (int i = MapObjectManager.Instance.lstMonsterID.Count-1; i >=0; i--)
                    {
                        int monsterId = MapObjectManager.Instance.lstMonsterID[i];
                        MapObjectManager.Instance.DestroyMapObjectById(monsterId);
                        MapObjectManager.Instance.lstMonsterID.Remove(monsterId);
                    }
                    
                    fightLose = false;

                    dungeonChapterUnit = ConfigUtils.GetDungeonChapterById(dungeonType);
                    if (dungeonChapterUnit != null)
                    {
                        var stageNandu = GuanKaStage;

                        int nandu = 0;
                        int mapBgNum = 0;
                        if(dungeonChapterUnit.Type ==  (int)DungeonType.BossWithPet)
                        {
                            int cfgId = 0;

                            if (DungeonMapManager.Instance.mapEventData.eventType == (int)eRandomEventType.eRandomEventType_RandomBox)
                            {
                                foreach (var stuffData in DungeonMapManager.Instance.mapEventData.batchStuffList)
                                {
                                    if (stuffData.id == DungeonMapManager.Instance.batchStuffId)
                                    {
                                        cfgId = stuffData.extraCfgId;
                                        break;
                                    }
                                }
                            }
                            else if (DungeonMapManager.Instance.mapEventData.eventType == (int)StageEventType.Relic)
                            {
                                cfgId = DungeonMapManager.Instance.eventMonsterData.eventStageId;
                            }
                            else
                            {
                                foreach (var stuffData in DungeonMapManager.Instance.mapEventData.batchStuffList)
                                {
                                    if (stuffData.id == DungeonMapManager.Instance.batchStuffId)
                                    {
                                        cfgId = stuffData.cfgId;
                                        break;
                                    }
                                }
                            }
                            ConfigEventStageUnit stageUnit = ConfigUtils.GetEventStageUnitById(cfgId);
                            nandu = 1;
                            mapBgNum = 1;//多一块地图，防止穿帮
                            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_RANDOM_STAGE_DATA_INFO, stageUnit, GuanKaMonsterIndex);
                        }else if (dungeonChapterUnit.Type == (int)DungeonType.FishingBoss)
                        {
                            int cfgId = 0;
                            foreach (var stuffData in DungeonMapManager.Instance.mapEventData.batchStuffList)
                            {
                                if (stuffData.id == DungeonMapManager.Instance.batchStuffId)
                                {
                                    cfgId = stuffData.cfgId;
                                    break;
                                }
                            }
                            
                            if (DungeonMapManager.Instance.batchStuffId == 0)
                            {
                                // 普通boss
                                ConfigStageMonsterAttrUnit stageMonsterAttrUnit = ConfigUtils.GetStageMonsterAttrUnitByIndexId(cfgId);
                                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_FISHING_STAGE_DATA_INFO, stageMonsterAttrUnit, GuanKaMonsterIndex);
                            }
                            
                            if (DungeonMapManager.Instance.batchStuffId == 1)
                            {
                                // 传承boss
                                ConfigEventStageUnit stageUnit = ConfigUtils.GetEventStageUnitById(cfgId);
                                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_RANDOM_STAGE_DATA_INFO, stageUnit, GuanKaMonsterIndex);
                            }
                            
                            nandu = 1;
                            mapBgNum = 1;//多一块地图，防止穿帮
                        }
                        else
                        {
                            ConfigDungeonStageUnit stageUnit = ConfigUtils.GetDungeonStageByNandu(stageNandu, dungeonChapterUnit.Type);
                            nandu = stageUnit.Stage;
                            mapBgNum = stageUnit.MonsterData.Count;//多一块地图，防止穿帮
                            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_STAGE_DATA_INFO, stageUnit, GuanKaMonsterIndex);
                        }
                        
                        var battleScene = MapObjectManager.Instance.GetMapObjectSceneRootTrans();

                        if (battleScene != null)
                        {
                            battleScene.x = Mathf.Max(GuanKaMonsterIndex, 0) * -540;                     
                            var mapList = battleScene.GetChild("map").asCom.GetChild("mapList").asList;
                            mapList.itemRenderer = (index, item) =>
                            {
                                int mapIndex = (index) % 3 + 1;
                                ((UI_MapLoadItem) item).mapLoader.fixMode = true;
                                ((UI_MapLoadItem) item).mapLoader.url = UIResource.GetMapItemByMapId(dungeonChapterUnit.Scenes,mapIndex);
                            };
                            mapList.numItems = mapBgNum;//多一块地图，防止穿帮
                            mapList.ResizeToFit();
                        }
                        
                        var battleBg = MapObjectManager.Instance.GetMapObjectBGRootTrans();

                        if (battleBg != null)
                        {
                            battleBg.x = 0;
                            
                            // var map_bg0 = battleBg.GetChild("map_bg0").asLoader;
                            // map_bg0.fixMode = true;
                            // map_bg0.icon = UIResource.GetMapBgColorByMapId(1001);
                            // var map_bg = battleBg.GetChild("map_bg").asLoader;
                            // map_bg.fixMode = true;
                            // map_bg.icon = UIResource.GetMapBgByMapId(dungeonChapterUnit.Scenes);
                            //var map_middle = battleBg.GetChild("map_middle").asLoader;
                            //map_middle.fixMode = true;
                            //map_middle.icon =  UIResource.GetMapMiddleByMapId(dungeonChapterUnit.Scenes);
                            
                            var mapList2 = battleBg.GetChild("mapList2").asList;
                            mapList2.itemRenderer = (index, item) =>
                            {
                                int mapIndex = (index) % 3 + 1;
                                ((UI_MapLoadItem2) item).bg2.fixMode = true;
                                ((UI_MapLoadItem2)item).bg2.url = UIResource.GetMapMiddleByMapId(dungeonChapterUnit.Scenes);
                            };

                            mapList2.numItems = mapBgNum;

                            // var mapList3 = battleBg.GetChild("mapList3").asList;
                            // mapList3.itemRenderer = (index, item) =>
                            // {
                            //     int mapIndex = (index) % 2 + 1;
                            //     ((UI_MapLoadItem3) item).bg3.fixMode = true;
                            //     ((UI_MapLoadItem3)item).bg3.url = UIResource.GetMapBgColorByMapId(dungeonChapterUnit.Scenes);
                            // };
                            var cloud0 = battleBg.GetChild("cloud0").asLoader;
                            cloud0.url = UIResource.GetMapBgColorByMapId(dungeonChapterUnit.Scenes);
                            var cloud1 = battleBg.GetChild("cloud1").asLoader;
                            cloud1.url = UIResource.GetMapBgColorByMapId(dungeonChapterUnit.Scenes);
                        }

                        InitHeroAndPets();
                        
                        SpawnMonster(nandu, 0.01f);
                    }
                    
                }
                break;

            case EN_GUANKA_STEP.SHOW:
                {
                    PlayCommonTimeSpine();
                    var hero = MapObjectManager.Instance.GetLocalHero();

                    if (hero != null)
                    {
                        hero.EndCurSkill();
                        var lstPos = new List<Vector3>();
                        lstPos.Add(hero.Position);
                        lstPos.Add(new Vector3(MapObjectManager.FrontX2 + 540 * GuanKaMonsterIndex, MapObjectManager.HERO_Y, 0));
                        var lstRot = new List<Quaternion>();
                        hero.UpdateMoveByPath(lstPos, null, null);
                    }

                    for (int i = 0; i < MapObjectManager.Instance.lstPetObj.Count; i++)
                    {
                        MapPetObject t = MapObjectManager.Instance.lstPetObj[i];
                        var lstPos = new List<Vector3>();
                        lstPos.Add(t.Position);
                        lstPos.Add(new Vector3(petPosX[t.PetAttr.PetVo.BattleIndex] + 540 * GuanKaMonsterIndex, petPosY[t.PetAttr.PetVo.BattleIndex], 0));
                        t.UpdateMoveByPath(lstPos, null, null);
                    }

                    foreach (var monsterID in MapObjectManager.Instance.lstMonsterID)
                    {
                        var monster = MapObjectManager.Instance.GetMonsterObjectById(monsterID);

                        if (monster != null)
                        {
                            var lstPos = new List<Vector3>();
                            lstPos.Add(monster.Position);
                            lstPos.Add(new Vector3(monster.Position.x,  monster.Position.y));
                            monster.UpdateMoveByPath(lstPos, null, null);
                        }
                    }
                    
                    timerShow1.Clear();
                    timerShow2.Clear();
                    timerShow1.Startup(0.5f/MapObjectManager.Instance.Speed, false);
                    MapObjectManager.Instance.battleSign = false;
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_MONSTER_WAVE_DATA_INFO,GuanKaMonsterIndex);
                break;

            case EN_GUANKA_STEP.FIGHTING:
                {
                    StopCommonTimeSpine();
                    if(fightLose) return;

                    GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,1,1.0f, () =>
                    {
                        var hero = MapObjectManager.Instance.GetLocalHero();

                        if (hero != null)
                        {
                            hero.UpdateHeroTotalAttr();
                            hero.IsAutoAttack = true;
                            hero.ClearMovePath();
                        }

                        foreach (var t in MapObjectManager.Instance.lstPetObj)
                        {
                            t.UpdatePetTotalAttribute();
                            t.IsAutoAttack = true;
                            t.ClearMovePath();
                        }

                        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP, 1, MapObjectManager.Instance._battlePauseTimer, () =>
                        {
                            MapObjectManager.Instance.battleSign = true;
                        });
                
                        timreFightResult.Startup(1f/MapObjectManager.Instance.Speed);
                    });

                }
                break;
            
             case EN_GUANKA_STEP.RESULT:
                {
                    if (fightLose)
                    {
                        if(dungeonChapterUnit.Type ==  (int)DungeonType.BossWithPet)
                        {
                            if (mapEventData != null)
                            {
                                if (mapEventData.eventType == (int)StageEventType.Boss)
                                {
                                    var builder = AttackWildBossEnd_CS.CreateBuilder();
                                    builder.EventGuid = (ulong)mapEventData.guid;
                                    builder.BatchStuffId = (uint)batchStuffId;
                                    builder.IsWin = false;
                                    AttackWildBossEnd_CS copyEndCs = builder.Build();
                                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackWildBossEnd_CS, copyEndCs);
                                    
                                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                                }else if (mapEventData.eventType == (int)StageEventType.Pet)
                                {
                                    var builder = AttackWildPetEnd_CS.CreateBuilder();
                                    builder.EventGuid = (ulong)mapEventData.guid;
                                    builder.BatchStuffId = (uint)batchStuffId;
                                    builder.IsWin = false;
                                    AttackWildPetEnd_CS copyEndCs = builder.Build();
                                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackWildPetEnd_CS, copyEndCs);
                                    
                                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                                }
                                else if (mapEventData.eventType == (int)StageEventType.Relic)
                                {
                                    //遗迹boss
                                    if (eventMonsterData != null)
                                    {
                                        var builder = EndRuinMosterFight_CS.CreateBuilder();
                                        builder.EventGuid = eventMonsterData.guid;
                                        builder.MonsterIndex = (uint)eventMonsterData.indexId;
                                        builder.IsWin = false;
                                        EndRuinMosterFight_CS copyEndCs = builder.Build();
                                        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_EndRuinMosterFight_CS, copyEndCs);
                                
                                        eventMonsterData = null;
                                    }
                                }
                                else if (mapEventData.eventType == (int)eRandomEventType.eRandomEventType_RandomBox)
                                {//深埋宝藏
                                    var builder = AttackBoxBossEnd_CS.CreateBuilder();
                                    builder.EventGuid = mapEventData.guid;
                                    builder.StuffId = (uint)batchStuffId;
                                    builder.IsWin = false;
                                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackBoxBossEnd_CS, builder.Build());
                                }
                                mapEventData = null;
                            }
                            
                            DungeonMapManager.Instance.IsInCopy = false;
                            MapObjectManager.Instance.IsStopMapBattle = false;
                            UIManager.Instance.CloseUIPanel("DungeonMap");
                            //失败界面
                            UIManager.Instance.ShowUIPanel("FightLose");
                            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                            // UIManager.Instance.ShowUIPanel("DungeonFailed");
                        }else if (dungeonChapterUnit.Type ==  (int)DungeonType.FishingBoss)
                        {
                            if (mapEventData != null)
                            {
                                if (mapEventData.eventType == (int)StageEventType.Fishing)
                                {
                                    var builder = AttackFishingBossEnd_CS.CreateBuilder();
                                    builder.IsWin = false;
                                    AttackFishingBossEnd_CS copyEndCs = builder.Build();
                                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackFishingBossEnd_CS, copyEndCs);
                                }
                                
                                mapEventData = null;
                                
                            }
                            
                            DungeonMapManager.Instance.IsInCopy = false;
                            MapObjectManager.Instance.IsStopMapBattle = false;
                            UIManager.Instance.CloseUIPanel("DungeonMap");
                            //失败界面
                            UIManager.Instance.ShowUIPanel("FishingFail");
                        }
                        else
                        {
                            DungeonMapManager.Instance.IsInCopy = false;
                            MapObjectManager.Instance.IsStopMapBattle = false;
                            UIManager.Instance.CloseUIPanel("DungeonMap");
                            //失败界面
                            UIManager.Instance.ShowUIPanel("DungeonFailed");
                        }
                        return;
                    }

                    UpdateStageMonster();
                }
                break;
             
            default:
                break;
        }
    }

    private void UpdateStageMonster()
    {
        var hero = MapObjectManager.Instance.GetLocalHero();

        if (hero != null)
        {
            // 恢复生命
            hero.PlayRecovery();
        }
        // 准备下一波怪物
        ++GuanKaMonsterIndex;

        if (MaxGuanKaMonsterIndex != 0 && GuanKaMonsterIndex >= MaxGuanKaMonsterIndex)
        {
            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,7,0.5f, () =>
            {
                DungeonMapManager.Instance.IsInCopy = false;
                MapObjectManager.Instance.IsStopMapBattle = false;
                UIManager.Instance.CloseUIPanel("DungeonMap");
                if (!fightLose)
                {
                    if (dungeonChapterUnit.Type ==  (int)DungeonType.BossWithPet)
                    {
                        if (mapEventData != null)
                        {
                            if (mapEventData.eventType == (int)StageEventType.Boss)
                            {
                                var builder = AttackWildBossEnd_CS.CreateBuilder();
                                builder.EventGuid = (ulong)mapEventData.guid;
                                builder.BatchStuffId = (uint)batchStuffId;
                                builder.IsWin = true;
                                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackWildBossEnd_CS, builder.Build());
                                
                                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                            }else if (mapEventData.eventType == (int)StageEventType.Pet)
                            {
                                var builder = AttackWildPetEnd_CS.CreateBuilder();
                                builder.EventGuid = (ulong)mapEventData.guid;
                                builder.BatchStuffId = (uint)batchStuffId;
                                builder.IsWin = true;
                                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackWildPetEnd_CS, builder.Build());
                                
                                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                            }
                            else if (mapEventData.eventType == (int)StageEventType.Relic)
                            {
                                //遗迹boss
                                if (eventMonsterData != null)
                                {
                                    var builder = EndRuinMosterFight_CS.CreateBuilder();
                                    builder.EventGuid = (ulong)eventMonsterData.guid;
                                    builder.MonsterIndex = (uint)eventMonsterData.indexId;
                                    builder.IsWin = true;
                                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_EndRuinMosterFight_CS, builder.Build());
                            
                                    eventMonsterData = null;
                                    // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                                }
                            }
                            else if (mapEventData.eventType == (int)eRandomEventType.eRandomEventType_RandomBox)
                            {
                                //深埋宝藏
                                var builder = AttackBoxBossEnd_CS.CreateBuilder();
                                builder.EventGuid = mapEventData.guid;
                                builder.StuffId = (uint)batchStuffId;
                                builder.IsWin = true;
                                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackBoxBossEnd_CS, builder.Build());
                            }
                            mapEventData = null;
                            
                        }
                        // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                        
                    }else if (dungeonChapterUnit.Type ==  (int)DungeonType.FishingBoss)
                    {
                        if (mapEventData != null)
                        {
                            if (mapEventData.eventType == (int)StageEventType.Fishing)
                            {
                                var builder = AttackFishingBossEnd_CS.CreateBuilder();
                                builder.IsWin = true;
                                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackFishingBossEnd_CS, builder.Build());
                            }
                            
                            mapEventData = null;
                        }
                    }
                    else
                    {
                        var builder = Copy_End_CS.CreateBuilder();
                        builder.CopyType = (eCopyType) dungeonType;
                        builder.StartStageId = (uint) ConfigUtils.GetDungeonStageByNandu(GuanKaStage,(int)dungeonType).Id;
                        builder.IsWin = true;
                        builder.IsDirectFinish = false;
                        Copy_End_CS copyEndCs = builder.Build();
                        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Copy_End_CS, copyEndCs);
                    }
                }

            });
            return;
        }
        fightLose = false;

        EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_MONSTER_BOSS_INFO);

        if (hero != null)
        {
            hero.IsAutoAttack = false;
        }

        InitHeroAndPets();
        foreach (var t in MapObjectManager.Instance.lstPetObj)
        {
            t.IsAutoAttack = false;
        }
        //new Vector3(540 + 540 * GuanKaMonsterIndex + ConstDefine.DEFAULT_MOVE_SPEED * 4.0f, 280 + 60 * i, 0);
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,4,0.2f, () =>
        {
            var stageNandu = GuanKaStage;
            SpawnMonster(stageNandu, 0.10f);
        });
    }

    private void SpawnMonster(int stageNandu, float timer)
    {
        MapObjectManager.Instance.lstMonsterID.Clear();
        if (dungeonChapterUnit.Type ==  (int)DungeonType.BossWithPet)
        {
            
            //ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIndexId(normalStageId);
            // if (stageUnit == null)
            // {
            //     LogUtils.LogErrorFormat("Stage数据为空:stageId:{0}", stageNandu);
            //     return;
            // }
            // foreach (var stuffData in mapEventData.batchStuffList)
            // {
            //     if (stuffData.id == batchStuffId)
            //     {
            //         ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(stuffData.cfgId);
            //         if (eventStageUnit == null)
            //         {
            //             LogUtils.LogErrorFormat("EventStage数据为空:stageId:{0}", stageNandu);
            //             return;
            //         }
            //         SetMonsterData(eventStageUnit);
            //         return;
            //     }
            // }

            if (mapEventData.eventType == (int)eRandomEventType.eRandomEventType_RandomBox)
            {
                // 深埋宝藏
                foreach (var stuffData in mapEventData.batchStuffList)
                {
                    if (stuffData.id == batchStuffId)
                    {
                        ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(stuffData.extraCfgId);
                        if (eventStageUnit == null)
                        {
                            LogUtils.LogErrorFormat("EventStage数据为空:stageId:{0}", stageNandu);
                            return;
                        }
                        SetMonsterData(eventStageUnit);
                        return;
                    }
                }
            }
            else if (mapEventData.eventType == (int)StageEventType.Relic)
            {
                // 遗迹boss
                ConfigEventStageUnit ruinEventStageUnit = ConfigUtils.GetEventStageUnitById(eventMonsterData.eventStageId);
                if (ruinEventStageUnit == null)
                {
                    LogUtils.LogErrorFormat("EventStage数据为空:stageId:{0}", stageNandu);
                    return;
                }
                SetMonsterData(ruinEventStageUnit);
            }
            else
            {
                foreach (var stuffData in mapEventData.batchStuffList)
                {
                    if (stuffData.id == batchStuffId)
                    {
                        ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(stuffData.cfgId);
                        if (eventStageUnit == null)
                        {
                            LogUtils.LogErrorFormat("EventStage数据为空:stageId:{0}", stageNandu);
                            return;
                        }
                        SetMonsterData(eventStageUnit);
                        return;
                    }
                }
            }
            
        }else if (dungeonChapterUnit.Type ==  (int)DungeonType.FishingBoss)
        {
            if (mapEventData.eventType == (int)StageEventType.Fishing)
            {
                if (batchStuffId == 0)
                {
                    // 普通boss
                    foreach (var stuffData in mapEventData.batchStuffList)
                    {
                        if (stuffData.id == batchStuffId)
                        {
                            ConfigStageMonsterAttrUnit stageMonsterAttrUnit = ConfigUtils.GetStageMonsterAttrUnitByIndexId(stuffData.cfgId);
                            if (stageMonsterAttrUnit == null)
                            {
                                LogUtils.LogErrorFormat("StageMonsterAttr数据为空:stageId:{0}", stageNandu);
                                return;
                            }
                            SetMonsterData(stageMonsterAttrUnit);
                            return;
                        }
                    }
                }

                if (batchStuffId == 1)
                {
                    // 传承boss
                    foreach (var stuffData in mapEventData.batchStuffList)
                    {
                        if (stuffData.id == batchStuffId)
                        {
                            ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(stuffData.cfgId);
                            if (eventStageUnit == null)
                            {
                                LogUtils.LogErrorFormat("EventStage数据为空:stageId:{0}", stageNandu);
                                return;
                            }
                            SetMonsterData(eventStageUnit);
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            ConfigDungeonStageUnit stageUnit = ConfigUtils.GetDungeonStageByNandu(stageNandu, dungeonChapterUnit.Type);
            if (stageUnit == null)
            {
                LogUtils.LogErrorFormat("Stage数据为空:stageId:{0}", stageNandu);
                return;
            }
            SetMonsterData(stageUnit);
        }
    }
    
    /// <summary>
    /// 野外BOSS，逃跑宠物
    /// </summary>
    /// <param name="stageUnit"></param>
    private void SetMonsterData(ConfigEventStageUnit stageUnit)
    {
            int groupId = stageUnit.MonsterData;  
            var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
            for (int i = 0; i < monsterGroupArr.Count; i++)
            {
                var monsterGroup = monsterGroupArr[i];
                if (monsterGroup != null)
                {
                    for (int j = 0; j < monsterGroup.Num; j++)
                    {
                        var monster = new MonsterVo();
                        monster.generalsType = monsterGroup.MonsterId;
                        monster.unitID = MonsterUnitId ++;
                        ConfigCommonUnit common = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2);//2=获取怪物出生随机位置
                        if (!IsMonsterBoss)
                        {
                            monster.pos = new Vector3(540 + 540 * GuanKaMonsterIndex +  Random.Range(int.Parse(common.Param1), int.Parse(common.Param2)),  Random.Range(int.Parse(common.Param3),int.Parse(common.Param4)));
                        }
                        else
                        {
                            monster.pos = new Vector3(540 + 540 * GuanKaMonsterIndex +   Random.Range(200, 220),  MapObjectManager.HERO_Y);
                        }
                        monster.rot = Quaternion.Euler(0, 270, 0);
                        monster.IsBoss = IsMonsterBoss;
                        MapMonsterObject monsterObject = MapObjectManager.Instance.UpdateMonster(monster);
                        //副本怪物加成
                        ConfigCommonUnit common2003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2003);
                        ConfigMonsterUnit MonsterUnit = ConfigUtils.GetMonsterById(monster.generalsType);
                        
                        //减少攻击速度
                        // monsterObject.MonsterAttr.InitByAttrID(MonsterUnit.AtkSpeed - int.Parse(common2003.Param3), MonsterUnit.Speed);
                        monsterObject.SetMoveAttrValue(int.Parse(common2003.Param2), int.Parse(common2003.Param3));
                        
                        monsterObject.AddStageAttr(stageUnit, monsterGroup.MonsterId);
                        monsterObject.UpdateObject();
                        monsterObject.UpdateHpBar();
                        
                        MapObjectManager.Instance.lstMonsterID.Add(monster.unitID);
                    }
                    
                }
                else
                {
                    timerMonsterBoss.Clear();
                }
            }

            if (!isShowBossFight)
            {
                isShowBossFight = true;
                MaxGuanKaMonsterIndex = 1;//stageUnit.MonsterData;
                int bossId = 0;
                // for (int i = 0; i < stageUnit.MonsterData.Count; ++i)
                // {
                    groupId = stageUnit.MonsterData;
                    monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
                    foreach (var monsterGroup in monsterGroupArr)
                    {
                        if (monsterGroup != null)
                        {
                            var isBossArr = monsterGroup.IsBoss.Split(",");
                            if (isBossArr.Length == 2)
                            {
                                bossId = monsterGroup.MonsterId;
                                break;
                            }
                        }
                    }
                // }

                UI_BossFightWindow fightBoss = MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("fightBoss") as UI_BossFightWindow;
                fightBoss.visible = true;
                // GLoader fightBossBg = MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("fightBossBg") as GLoader;
                // fightBossBg.visible = true;
                var heroInfo = HeroInfoManager.Instance.GetMyHero();
                string strResName = ConfigUtils.GetHeroModelPathByID(heroInfo.HeroUnit.Id);
                Utils.SetSpineModelOnFGUI(fightBoss.hero.asGraph, strResName, 140);
                fightBoss.hero.asGraph.visible = false;
                ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(bossId);
                var monsterModelPath = monsterUnit.Model;
                Utils.SetSpineModelOnFGUI(fightBoss.monster.asGraph, monsterModelPath, monsterUnit.BossSize, "idle", null, true);
                fightBoss.monster.asGraph.visible = false;
                Transition t = fightBoss.GetTransition("vs");
                
                t.Play(() =>
                {
                    fightBoss.monster.asGraph.visible = true;
                    fightBoss.hero.asGraph.visible = true;
                });

                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,5,2f, () =>
                {
                    fightBoss.visible = false;
                    // fightBossBg.visible = false;
                    EndTime = ServerTimeManager.Instance.CurServerTime + (ulong)stageUnit.Time;
                    MonsterBossTime = (int) (EndTime - ServerTimeManager.Instance.CurServerTime);
                    timerMonsterBoss.Clear();
                    timerMonsterBoss.Startup(MonsterBossTime);
                });
                
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON, 8,0.01f, () =>
                {
                    GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,6,2f, () =>
                    {
                        ChangeState(EN_GUANKA_STEP.SHOW);
                    });
                });

            }
            else
            {
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,9,0.01f, () =>
                {
                    ChangeState(EN_GUANKA_STEP.SHOW);
                }); 
            }
    }

    /// <summary>
    /// 普通boss
    /// </summary>
    private void SetMonsterData(ConfigStageMonsterAttrUnit stageUnit)
    {
        int groupId = stageUnit.MonsterData;
        var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
        for (int i = 0; i < monsterGroupArr.Count; i++)
        {
            var monsterGroup = monsterGroupArr[i];
            if (monsterGroup != null)
            {
                for (int j = 0; j < monsterGroup.Num; j++)
                {
                    var monster = new MonsterVo();
                    monster.generalsType = monsterGroup.MonsterId;
                    monster.unitID = MonsterUnitId ++;
                    ConfigCommonUnit common = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2);//2=获取怪物出生随机位置
                    if (!IsMonsterBoss)
                    {
                        monster.pos = new Vector3(540 + 540 * GuanKaMonsterIndex +  Random.Range(int.Parse(common.Param1), int.Parse(common.Param2)),  Random.Range(int.Parse(common.Param3),int.Parse(common.Param4)));
                    }
                    else
                    {
                        monster.pos = new Vector3(540 + 540 * GuanKaMonsterIndex +   Random.Range(200, 220),  MapObjectManager.HERO_Y);
                    }
                    monster.rot = Quaternion.Euler(0, 270, 0);
                    monster.IsBoss = IsMonsterBoss;
                    MapMonsterObject monsterObject = MapObjectManager.Instance.UpdateMonster(monster);
                    //副本怪物加成
                    ConfigCommonUnit common2003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2003);
                    ConfigMonsterUnit MonsterUnit = ConfigUtils.GetMonsterById(monster.generalsType);
                        
                    //减少攻击速度
                    // monsterObject.MonsterAttr.InitByAttrID(MonsterUnit.AtkSpeed - int.Parse(common2003.Param3), MonsterUnit.Speed);
                    monsterObject.SetMoveAttrValue(int.Parse(common2003.Param2), int.Parse(common2003.Param3));
                        
                    monsterObject.AddStageAttr(stageUnit, monsterGroup.MonsterId);
                    monsterObject.UpdateObject();
                    monsterObject.UpdateHpBar();
                        
                    MapObjectManager.Instance.lstMonsterID.Add(monster.unitID);
                }
                    
            }
            else
            {
                timerMonsterBoss.Clear();
            }
        }
        
        if (!isShowBossFight)
        {
            isShowBossFight = true;
            MaxGuanKaMonsterIndex = 1;//stageUnit.MonsterData;
            int bossId = 0;
            // for (int i = 0; i < stageUnit.MonsterData.Count; ++i)
            // {
            groupId = stageUnit.MonsterData;
            monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
            foreach (var monsterGroup in monsterGroupArr)
            {
                if (monsterGroup != null)
                {
                    var isBossArr = monsterGroup.IsBoss.Split(",");
                    if (isBossArr.Length == 2)
                    {
                        bossId = monsterGroup.MonsterId;
                        break;
                    }
                }
            }
            // }

            UI_BossFightWindow fightBoss = MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("fightBoss") as UI_BossFightWindow;
            fightBoss.visible = true;
            // GLoader fightBossBg = MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("fightBossBg") as GLoader;
            // fightBossBg.visible = true;
            var heroInfo = HeroInfoManager.Instance.GetMyHero();
            string strResName = ConfigUtils.GetHeroModelPathByID(heroInfo.HeroUnit.Id);
            Utils.SetSpineModelOnFGUI(fightBoss.hero.asGraph, strResName, 140);
            fightBoss.hero.asGraph.visible = false;
            ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(bossId);
            var monsterModelPath = monsterUnit.Model;
            Utils.SetSpineModelOnFGUI(fightBoss.monster.asGraph, monsterModelPath, monsterUnit.BossSize, "idle", null, true);
            fightBoss.monster.asGraph.visible = false;
            Transition t = fightBoss.GetTransition("vs");
                
            t.Play(() =>
            {
                fightBoss.monster.asGraph.visible = true;
                fightBoss.hero.asGraph.visible = true;
            });

            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,5,2f, () =>
            {
                fightBoss.visible = false;
                // fightBossBg.visible = false;
                // EndTime = ServerTimeManager.Instance.CurServerTime + (ulong)stageUnit.Time;
                EndTime = ServerTimeManager.Instance.CurServerTime + 60;
                MonsterBossTime = (int) (EndTime - ServerTimeManager.Instance.CurServerTime);
                timerMonsterBoss.Clear();
                timerMonsterBoss.Startup(MonsterBossTime);
            });
                
            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON, 8,0.01f, () =>
            {
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,6,2f, () =>
                {
                    ChangeState(EN_GUANKA_STEP.SHOW);
                });
            });

        }
        else
        {
            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,9,0.01f, () =>
            {
                ChangeState(EN_GUANKA_STEP.SHOW);
            }); 
        }
        
    }

    /// <summary>
    /// 副本怪物
    /// </summary>
    /// <param name="stageUnit"></param>
    private void SetMonsterData(ConfigDungeonStageUnit stageUnit)
    {
        if (GuanKaMonsterIndex < stageUnit.MonsterData.Count)
        {
            int groupId = stageUnit.MonsterData[GuanKaMonsterIndex];  
            var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
            for (int i = 0; i < monsterGroupArr.Count; i++)
            {
                var monsterGroup = monsterGroupArr[i];
                if (monsterGroup != null)
                {
                    for (int j = 0; j < monsterGroup.Num; j++)
                    {
                        var monster = new MonsterVo();
                        monster.generalsType = monsterGroup.MonsterId;
                        monster.unitID = MonsterUnitId ++;
                        ConfigCommonUnit common = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2);//2=获取怪物出生随机位置
                        if (!IsMonsterBoss)
                        {
                            monster.pos = new Vector3(540 + 540 * GuanKaMonsterIndex +  Random.Range(int.Parse(common.Param1), int.Parse(common.Param2)),  Random.Range(int.Parse(common.Param3),int.Parse(common.Param4)));
                        }
                        else
                        {
                            monster.pos = new Vector3(540 + 540 * GuanKaMonsterIndex +   Random.Range(200, 220),  MapObjectManager.HERO_Y);
                        }
                        monster.rot = Quaternion.Euler(0, 270, 0);
                        monster.IsBoss = IsMonsterBoss;
                        MapMonsterObject monsterObject = MapObjectManager.Instance.UpdateMonster(monster);
                        //副本怪物加成
                        monsterObject.AddStageAttr(stageUnit);
                        monsterObject.UpdateObject();
                        monsterObject.UpdateHpBar();
                        
                        MapObjectManager.Instance.lstMonsterID.Add(monster.unitID);
                    }
                    
                }
                else
                {
                    timerMonsterBoss.Clear();
                }
            }

            if (!isShowBossFight)
            {
                isShowBossFight = true;
                MaxGuanKaMonsterIndex = stageUnit.MonsterData.Count;
                int bossId = 0;
                for (int i = 0; i < stageUnit.MonsterData.Count; ++i)
                {
                    groupId = stageUnit.MonsterData[i];
                    monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
                    foreach (var monsterGroup in monsterGroupArr)
                    {
                        if (monsterGroup != null)
                        {
                            var isBossArr = monsterGroup.IsBoss.Split(",");
                            if (isBossArr.Length == 2)
                            {
                                bossId = monsterGroup.MonsterId;
                                break;
                            }
                        }
                    }
                }

                UI_BossFightWindow fightBoss = MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("fightBoss") as UI_BossFightWindow;
                fightBoss.visible = true;
                // GLoader fightBossBg = MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("fightBossBg") as GLoader;
                // fightBossBg.visible = true;
                var heroInfo = HeroInfoManager.Instance.GetMyHero();
                string strResName = ConfigUtils.GetHeroModelPathByID(heroInfo.HeroUnit.Id);
                Utils.SetSpineModelOnFGUI(fightBoss.hero.asGraph, strResName, 140);
                fightBoss.hero.asGraph.visible = false;
                ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(bossId);
                var monsterModelPath = monsterUnit.Model;
                Utils.SetSpineModelOnFGUI(fightBoss.monster.asGraph, monsterModelPath, monsterUnit.BossSize, "idle", null, true);
                fightBoss.monster.asGraph.visible = false;
                Transition t = fightBoss.GetTransition("vs");
                
                t.Play(() =>
                {
                    fightBoss.monster.asGraph.visible = true;
                    fightBoss.hero.asGraph.visible = true;
                });

                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,5,2f, () =>
                {
                    fightBoss.visible = false;
                    // fightBossBg.visible = false;
                    MonsterBossTime = (int) (EndTime - ServerTimeManager.Instance.CurServerTime);
                    timerMonsterBoss.Clear();
                    timerMonsterBoss.Startup(MonsterBossTime);
                });
                
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON, 8,0.01f, () =>
                {
                    GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,6,2f, () =>
                    {
                        ChangeState(EN_GUANKA_STEP.SHOW);
                    });
                });

            }
            else
            {
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.DUNGEON,9,0.01f, () =>
                {
                    ChangeState(EN_GUANKA_STEP.SHOW);
                }); 
            }
        }
    }

    protected void ExitState(EN_GUANKA_STEP state, EN_GUANKA_STEP stateNext)
    {
        switch (state)
        {
            case EN_GUANKA_STEP.FIGHTING:
                {
                    timreFightResult.Clear();
                }
                break;

            default:
                break;
        }
    }

    protected void UpdateState(EN_GUANKA_STEP state)
    {
        switch (state)
        {
            case EN_GUANKA_STEP.SHOW:
                {
                    if (timerShow1.TimeOver())
                    {
                        if (GuanKaMonsterIndex == 0)
                        {
                            ChangeState(EN_GUANKA_STEP.FIGHTING);
                        }
                        else
                        {
                            timerShow2.Startup(1.5f/MapObjectManager.Instance.Speed, false);
                        }
                    }

                    if (timerShow2.IsActive())
                    {   
                        // 主节点晚1s移动，一共移动3s，根据主角速度移动
                        var battleScene = MapObjectManager.Instance.GetMapObjectSceneRootTrans();

                        if (battleScene != null)
                        {
                            battleScene.x = Mathf.Max(GuanKaMonsterIndex - 1, 0) * -540 + timerShow2.GetPassPrecent() * -540;
                        }
                        
                        var battleBg = MapObjectManager.Instance.GetMapObjectBGRootTrans();
                        
                        if (battleBg != null)
                        {
                            battleBg.x = Mathf.Max(GuanKaMonsterIndex - 1, 0) * -270 + timerShow2.GetPassPrecent() * -270;
                        }

                        if (timerShow2.TimeOver())
                        {
                            ChangeState(EN_GUANKA_STEP.FIGHTING);
                        }
                    }
                }
                break;
            
            case EN_GUANKA_STEP.FIGHTING:
                {
                    if (timreFightResult.ToNextTime())
                    {
                        if (!MapObjectManager.Instance.IsExistMonster())
                        {
                            fightLose = false;
                            ChangeState(EN_GUANKA_STEP.RESULT);
                        }
                        else if (!MapObjectManager.Instance.IsExistHero())
                        {
                            fightLose = true;
                            ChangeState(EN_GUANKA_STEP.RESULT);
                        }
                    }

                    if (timerMonsterBoss.IsActive() && timerMonsterBoss.TimeOver())
                    {
                        fightLose = true;
                        ChangeState(EN_GUANKA_STEP.RESULT);
                    }
                }
                break;

            default:
                break;
        }
    }

    #endregion
}
