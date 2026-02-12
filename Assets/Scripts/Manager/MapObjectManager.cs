using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Config;
using UnityEngine;
using Engine;
using EngineBase;
using FairyGUI;
using Lobby;
using MonsterLove.StateMachine;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MapObjectManager : TSingleton<MapObjectManager>
{
    public GObjectPool Pool { private set; get; }
    
    public bool StageComplete = false;
    public float Speed { get; set; } = 1.0f;
    private bool _isCopyToBattle = false;
    private bool _isStopMapBattle = false;
    
    private ConfigCommonUnit _common18;
    private ConfigCommonUnit _common19;
    public float _battlePauseTimer = 0;
    
    public bool isHeroPlayNormalSkill = false;
    public bool isHeroPlayXPSkill = false;
    
    public bool IsStopMapBattle
    {
        set
        {

            if (value == _isStopMapBattle)
                return;
            _isStopMapBattle = value;
            if (_isStopMapBattle)
            {
                CleanUp();
                RoleManager.Instance.EndSkillProxyDict();
                UnloadUnusedAssets();
            }
            if (_isStopMapBattle == false)
            {
                CleanUp();
                UnloadUnusedAssets();
                StageComplete = false;
                Debug.LogWarningFormat("GuanKaMonsterIndex:{0}   MaxGuanKaMonsterIndex:{1}", GuanKaMonsterIndex, MaxGuanKaMonsterIndex);
                if (GuanKaMonsterIndex >= MaxGuanKaMonsterIndex && MaxGuanKaMonsterIndex != 0)
                    GuanKaMonsterIndex -= 1;
                else if(MaxGuanKaMonsterIndex == 0)
                {
                    GuanKaMonsterIndex = 0;
                }
                _isCopyToBattle = true;
                InitGuanKaFSM(MapObjectManager.Instance.GuanKaMonsterIndex);
            }
        }
        get => _isStopMapBattle;
    }

    public const int HERO_Y = 360;//360;  //340
    public const int PET1_Y = 260;//280;  // 240
    public const int PET2_Y = 460;//400;  //420

    private const int FrontX1 = 190;//290-70;  //220
    public const int FrontX2 = 140;//260-70;   //190
    private const int FrontX3 = 120;//230-70;  //160
    private const int BackX1 = 90;//140-40;  //100
    private const int BackX2 = 60;//110-40;  //70
    private const int BackX3 = 30;//80-40;   //40

    private static int MonsterUnitId = 1000;//怪物自增ID 从1000开始
    // public int CurMapId = 1001;
    // private static readonly int heroId = 1001;
    // private static readonly List<int> petIdList = new List<int>
    // {
    //     10012,10032,10060,10062,10042
    // };
    
    // 4      1
    // 3     主角
    // 5      2
    public static readonly List<int> petPosY = new List<int>
    {
        // PET1_Y,PET2_Y,HERO_Y,PET1_Y,PET2_Y
        PET1_Y,HERO_Y,PET2_Y,PET1_Y,PET2_Y
    };
    
    public static readonly List<int> petPosX = new List<int>
    {
        // FrontX1,FrontX3,BackX2,BackX1,BackX3
        BackX1,BackX2,BackX3,FrontX1,FrontX3
    };

    private const float DEFAULT_UNLOAD_RESOURCE_TIME = 80f;
    private const int DEFAULT_UNLOAD_RESOURCE_COUNT = 30;

    public Transform transGame { get; private set; }
    public Transform transRecycle { get; private set; }
    
    /// <summary>
    /// 延迟删除对象
    /// </summary>
    private bool IsInForeachObjectDoing { get; set; }

    /// <summary>
    /// 延迟删除对象id
    /// </summary>
    private List<int> lstDelayDestroyObjectID = new List<int>();
    
    /// <summary>
    /// 动态物件: 真实物件
    /// </summary>
    private Dictionary<int, MapObject> dictMapObject = new Dictionary<int, MapObject>();

    // 虚拟对象自增ID
    private Dictionary<int, int> dictMapVirtualObjectIDCur = new Dictionary<int, int>();
    
    // 需要Tick的光效 
    private List<MapFxObject> lstMapFxObject = new List<MapFxObject>();
    
    public List<MapPetObject> lstPetObj { get; private set; } = new List<MapPetObject>();
    
    public List<MapPetObject> pvplstPetObj { get; private set; } = new List<MapPetObject>();
    
    public double MonsterDeathGold { get; set; }
    
    #region 缓存策略

    protected Dictionary<int, Dictionary<string, GameObject>> dicPrefab =
        new Dictionary<int, Dictionary<string, GameObject>>();

    // 异步加载序号
    protected int m_nAsyncLoadIndex = 0;

    protected Dictionary<System.Type, Dictionary<string, List<MapObject>>>
        dicActorPool = new Dictionary<System.Type, Dictionary<string, List<MapObject>>>();

    #endregion

    #region 加载/缓存模块

    private ConfigCommonUnit _common1001;

    private GameObject GetCachePrefab(MapObject obj)
    {
        Dictionary<string, GameObject> dictGo;

        if (this.dicPrefab.TryGetValue((int)obj.ObjectType, out dictGo))
        {
            if (dictGo != null)
            {
                GameObject prefab = null;

                if (dictGo.TryGetValue(obj.Name, out prefab))
                {
                    return prefab;
                }
            }
        }

        return null;
    }

    private void AddCachePrefab(int objectType, string strAssetName, GameObject prefab)
    {
        if (this.dicPrefab != null)
        {
            Dictionary<string, GameObject> dictGo;

            if (!this.dicPrefab.TryGetValue(objectType, out dictGo))
            {
                dictGo = new Dictionary<string, GameObject>();
                this.dicPrefab[objectType] = dictGo;
            }

            if (dictGo != null)
            {
                if (!dictGo.ContainsKey(strAssetName))
                {
                    dictGo.Add(strAssetName, prefab);
                }
            }
        }
    }

    public void LoadGameObject(MapObject obj)
    {
        GameObject prefab = GetCachePrefab(obj);

        if (prefab != null)
        {
            obj.SetGameObject(GameObject.Instantiate(prefab));
        }
        else
        {
            obj.asyncLoadIndex = m_nAsyncLoadIndex++;
            if (obj is MapFxObject)
            {
                ModelManager.Instance.LoadNormalPrefab("Effect/" + obj.Name, 
                    (go) =>
                    {
                        OnLoadModelDone(obj, go);
                    });
            }
            else
            {
                ModelManager.Instance.LoadNormalPrefab("Role/" + obj.Name, 
                    (go) =>
                    {
                        OnLoadModelDone(obj, go);
                    }); 
            }

        }
    }

    public void OnLoadModelDone(MapObject obj, UnityEngine.Object prefab)
    {
        if (obj != null)
        {
            if (prefab != null)
            {
                AddCachePrefab((int)obj.ObjectType, obj.Name, prefab as GameObject);
            }

            if (prefab != null)
            {
                var goNew = GameObject.Instantiate(prefab as GameObject);
                obj.SetGameObject(goNew);
            }
        }
    }

    private MapObject FindActor<T>(string name) where T : MapObject
    {
        if (this.dicActorPool != null)
        {
            Dictionary<string, List<MapObject>> dicActor = null;
            if (dicActorPool.TryGetValue(typeof(T), out dicActor))
            {
                List<MapObject> list = null;
                if (dicActor.TryGetValue(name, out list) && list.Count > 0)
                {
                    var item = list[0];
                    list.RemoveAt(0);
                    return item;
                }
            }
        }

        return null;
    }

    public MapObject SpawnActor<T>(string name) where T : MapObject
    {
        MapObject actor = this.FindActor<T>(name);
        if (actor == null)
        {
            actor = System.Activator.CreateInstance(typeof(T)) as MapObject;
            actor.Name = name;
            actor.InitAttr();
        }

        return actor;
    }

    public void DestroyActor(MapObject actor)
    {
        if (actor != null && !actor.Recycled)
        {
            actor.Destroyed();

            if (this.dicActorPool != null)
            {
                System.Type t = actor.GetType();
                Dictionary<string, List<MapObject>> dicActor = null;
                if (!dicActorPool.TryGetValue(t, out dicActor))
                {
                    dicActor = new Dictionary<string, List<MapObject>>();
                    dicActorPool.Add(t, dicActor);
                }

                List<MapObject> list = null;
                if (!dicActor.TryGetValue(actor.Name, out list))
                {
                    list = new List<MapObject>();
                    dicActor.Add(actor.Name, list);
                }

                list.Add(actor);
            }
        }
    }

    public void CleanUp()
    {
        m_nAsyncLoadIndex = 0;
        RoleManager.Instance.DestroySkillProxyDict();
        GetLocalHero()?.EndCurSkill();
        MapObjectManager.Instance.DestroyMapObject(GetLocalHero());
        foreach (var pet in lstPetObj)
        {
            pet.EndCurSkill();
            MapObjectManager.Instance.DestroyMapObject(pet);
        }
        lstPetObj.Clear();
        for (int i = lstMonsterID.Count-1; i >=0; i--)
        {
            int monsterId = lstMonsterID[i];
            MapMonsterObject monsterObject = MapObjectManager.Instance.GetMonsterObjectById(monsterId);
            monsterObject?.EndCurSkill();
            MapObjectManager.Instance.DestroyMapObject(monsterObject);
            lstMonsterID.Remove(monsterId);
        }

        // GameManager.Instance.TimerManager?.ClearTimerBySource(EN_TIMER_SOURCE.MAP);
        // GameManager.Instance.TimerManager?.ClearTimerBySource(EN_TIMER_SOURCE.DUNGEON);
        // GameManager.Instance.TimerManager?.ClearTimerBySource(EN_TIMER_SOURCE.BATTLE);

        IsInForeachObjectDoing = true;
        
        
        for (int i = lstMapFxObject.Count - 1; i >= 0; i--)
        {
            var item = lstMapFxObject[i];
            MapObjectManager.Instance.DestroyActor(item);
        }

        foreach (var item in dictMapObject)
        {
            MapObjectManager.Instance.DestroyActor(item.Value);
        }
        
        foreach (var item in dicPrefab)
        {
            item.Value.Clear();
        }
        dicPrefab.Clear();

        foreach (var item in dicActorPool)
        {
            foreach (var itemType in item.Value)
            {
                List<MapObject> lstObj = itemType.Value;

                for (int i = lstObj.Count - 1; i >= 0; i--)
                {
                    lstObj[i].Dispose();
                    lstObj.RemoveAt(i);
                }
            }
        }
        GameManager.Instance?.TimerManager.ClearAll(true);
        IsInForeachObjectDoing = false;
       
        dictMapObject.Clear();
        dicActorPool.Clear();
        dictMapVirtualObjectIDCur.Clear();
        lstDelayDestroyObjectID.Clear();
        
        System.GC.Collect();
        // Resources.UnloadUnusedAssets();
    }

    public void UnloadUnusedAssets()
    {
        float fTime = Time.realtimeSinceStartup;
        int nDisposeCount = 0;

        foreach (var item in dicActorPool)
        {
            foreach (var itemType in item.Value)
            {
                List<MapObject> lstObj = itemType.Value;

                for (int i = lstObj.Count - 1; i >= 0; i--)
                {
                    var mapObj = lstObj[i];

                    if (mapObj != null && fTime - mapObj.RecycledTime >= DEFAULT_UNLOAD_RESOURCE_TIME)
                    {
                        lstObj.RemoveAt(i);
                        MapObjectManager.Instance.DestroyActor(mapObj);
                        nDisposeCount++;
                    }
                }

                if (nDisposeCount >= DEFAULT_UNLOAD_RESOURCE_COUNT)
                {
                    break;
                }
            }

            if (nDisposeCount >= DEFAULT_UNLOAD_RESOURCE_COUNT)
            {
                break;
            }
        }
    }

    #endregion

    public void InitWorldEnvironment()
    {
        if (this.transGame == null)
        {
            var goGameRoot = GameObject.Instantiate(Resources.Load("GameRoot") as GameObject);
            goGameRoot.name = "GameRoot";
            this.transGame = goGameRoot.transform;
        }

        if (this.transRecycle == null)
        {
            this.transRecycle = this.transGame.Find("recycleRoot");
        }

        if (Pool == null)
        {
            Pool = new GObjectPool(MapObjectManager.Instance.transRecycle);
        }
    }

    private float _recoveryTicker = 0;
    private float _totalRecoverTimer = 0;
    public void Tick(float deltaSeconds)
    {
        if(IsStopMapBattle) return;
        if(StageComplete) return;
        TickByMapObject(deltaSeconds * Speed);
        UpdateState(GuanKaStep);
        _recoveryTicker += deltaSeconds;
        if (_recoveryTicker > _totalRecoverTimer)
        {
            _recoveryTicker -=_totalRecoverTimer;
            var hero = MapObjectManager.Instance.GetLocalHero();
            if (hero != null)
            {
                //TODO 恢复生命
                hero.PlayRecovery();
                
                List<MapObject> lstEnemy = MapObjectManager.Instance.GetAllEnemy(hero, hero.Position, 0);
                foreach (var obj in lstEnemy)
                {
                    if (obj != null && obj.Attr.Recovery > 0)
                    {
                        (obj as MapMonsterObject).PlayRecovery();
                    }
                }
            }
        }

    }
    
    public void LateTick(float deltaSeconds)    
    {
        if(IsStopMapBattle) return;
        if(StageComplete) return;
        LateTickByMapObject(deltaSeconds * Speed);
        LateTickByDelayDestroyMapObject();
    }
    
    public void TickByMapObject(float deltaSeconds)
    {
        IsInForeachObjectDoing = true;
        foreach (var item in dictMapObject)
        {
            item.Value.Tick(deltaSeconds);
        }
        IsInForeachObjectDoing = false;
        
        for (int i = lstMapFxObject.Count - 1; i >= 0; i--)
        {
            var item = lstMapFxObject[i];
            
            if (item != null)
            {
                item.Tick(deltaSeconds);
            }
        }
    }
    
    public void LateTickByMapObject(float deltaSeconds)
    {
        IsInForeachObjectDoing = true;
        foreach (var item in dictMapObject)
        {
            item.Value.LateTick(deltaSeconds);
        }
        IsInForeachObjectDoing = false;

        var root = GetMapObjectSceneObjRootTrans();

        if (root != null)
        {
            var children = root.GetChildren();
            Array.Sort(children, (p1, p2) =>
            {
                // float sortA = p1.data != null ? (p1.y + (int)p1.data) : p1.y;
                // float sortB = p2.data != null ? (p2.y + (int)p2.data) : p2.y;
                // int result = sortA.CompareTo(sortB);
                // return result;
                int sort1 = Mathf.FloorToInt(p1.y);
                if (p1.data != null)
                {
                    sort1 += (int)p1.data;
                }
                int sort2 = Mathf.FloorToInt(p2.y);
                if (p2.data != null)
                {
                    sort2 += (int)p2.data;
                }

                if (sort1 == sort2)
                {
                    return p1.GID.CompareTo(p2.GID);
                }
                else
                {
                    return sort1.CompareTo(sort2);
                }
            });
            root.ChangeChildrenOrder(children);
        }
    }
    
    private void LateTickByDelayDestroyMapObject()
    {
        if (lstDelayDestroyObjectID.Count > 0)
        {
            /// 强行设置为false防止死循环
            IsInForeachObjectDoing = false;
            foreach (var item in lstDelayDestroyObjectID)
            {
                this.DestroyMapObjectById(item);
            }
            lstDelayDestroyObjectID.Clear();
        }
    }
    
    public static void PrintKeysAndValues( String[] myKeys, String[] myValues )  {
        for ( int i = 0; i < myKeys.Length; i++ )  {
            Console.WriteLine( "   {0,-10}: {1}", myKeys[i], myValues[i] );
        }
        Console.WriteLine();
    }
    
    public GComponent GetMapObjectRootTrans()
    {
        if (PVPMapManager.Instance.IsInPVP)
        {
            var pvpMapView = UIManager.Instance.FindByName("PVPMapMain") as PVPMapMainView;
            if (pvpMapView != null)
            {
                return pvpMapView.GetBattleRoot();
            }
        }
        
        if (DungeonMapManager.Instance.IsInCopy)
        {
            var dungeonMapView = UIManager.Instance.FindByName("DungeonMap") as DungeonMapView;
            if (dungeonMapView != null)
            {
                return dungeonMapView.GetBattleRoot();
            }
        }
        
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            return lobbyView.GetBattleRoot();
        }

        return null;
    }
    
    public GComponent GetMapObjectBGRootTrans()
    {
        if (PVPMapManager.Instance.IsInPVP)
        {
            var pvpMapView = UIManager.Instance.FindByName("PVPMapMain") as PVPMapMainView;
            if (pvpMapView != null)
            {
                return pvpMapView.GetBattleBGRoot();
            }
        }
        
        if (DungeonMapManager.Instance.IsInCopy)
        {
            var dungeonMapView = UIManager.Instance.FindByName("DungeonMap") as DungeonMapView;
            if (dungeonMapView != null)
            {
                return dungeonMapView.GetBattleBGRoot();
            }
        }
        
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            return lobbyView.GetBattleBGRoot();
        }

        return null;
    }
    
    public GComponent GetMapObjectSceneRootTrans()
    {
        if (PVPMapManager.Instance.IsInPVP)
        {
            var pvpMapView = UIManager.Instance.FindByName("PVPMapMain") as PVPMapMainView;
            if (pvpMapView != null)
            {
                return pvpMapView.GetBattleSceneRoot();
            }
        }
        
        if (DungeonMapManager.Instance.IsInCopy)
        {
            var dungeonMapView = UIManager.Instance.FindByName("DungeonMap") as DungeonMapView;
            if (dungeonMapView != null)
            {
                return dungeonMapView.GetBattleSceneRoot();
            }
        }
        
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            return lobbyView.GetBattleSceneRoot();
        }

        return null;
    }
    
    public GComponent GetMapObjectSceneObjRootTrans()
    {
        if (PVPMapManager.Instance.IsInPVP)
        {
            var pvpMapView = UIManager.Instance.FindByName("PVPMapMain") as PVPMapMainView;
            if (pvpMapView != null)
            {
                return pvpMapView.GetBattleSceneObjRoot();
            }
        }
        
        if (DungeonMapManager.Instance.IsInCopy)
        {
            var dungeonMapView = UIManager.Instance.FindByName("DungeonMap") as DungeonMapView;
            if (dungeonMapView != null)
            {
                return dungeonMapView.GetBattleSceneObjRoot();
            }
        }
        
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
        if (lobbyView != null)
        {
            return lobbyView.GetBattleSceneObjRoot();
        }

        return null;
    }

    public GComponent GetMapObjectSceneFlyRootTrans()
    {
        if (PVPMapManager.Instance.IsInPVP)
        {
            var pvpMapView = UIManager.Instance.FindByName("PVPMapMain") as PVPMapMainView;
            if (pvpMapView != null)
            {
                return pvpMapView.GetBattleSceneFlyRoot();
            }
        }
        
        if (DungeonMapManager.Instance.IsInCopy)
        {
            var dungeonMapView = UIManager.Instance.FindByName("DungeonMap") as DungeonMapView;
            if (dungeonMapView != null)
            {
                return dungeonMapView.GetBattleSceneFlyRoot();
            }
        }
        
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            return lobbyView.GetBattleSceneFlyRoot();
        }

        return null;
    }
    
    public GComponent GetMapObjectObjRootTrans()
    {
        if (PVPMapManager.Instance.IsInPVP)
        {
            var pvpMapView = UIManager.Instance.FindByName("PVPMapMain") as PVPMapMainView;
            if (pvpMapView != null)
            {
                return pvpMapView.GetRoot();
            }
        }
        
        if (DungeonMapManager.Instance.IsInCopy)
        {
            var dungeonMapView = UIManager.Instance.FindByName("DungeonMap") as DungeonMapView;
            if (dungeonMapView != null)
            {
                return dungeonMapView.GetRoot();
            }
        }
        
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            return lobbyView.GetRoot();
        }

        return null;
    }
    
    public void PlayCommonTimeSpine(bool isLose = false)
    {
        var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;

        if (lobbyView != null)
        {
            lobbyView.PlayCommonTimeSpine(isLose);
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
    
    public MapObject GetMapObjectById(int id)
    {
        MapObject obj;
        if (dictMapObject.TryGetValue(id, out obj))
        {
            return obj;
        }
        return null;
    }

    public MapMoveObject GetMapMoveObjectById(int id)
    {
        return GetMapObjectById(id) as MapMoveObject;
    }

    public void AddMapObject(MapObject obj)
    {
        if (obj == null)
        {
            return;
        }

        dictMapObject[obj.id] = obj;
    }

    public void DestroyMapObjectById(int id)
    {
        DestroyMapObject(GetMapObjectById(id));
    }

    public void DestroyMapObject(MapObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (IsInForeachObjectDoing)
        {
            // 遍历中不做删除操作
            if (!lstDelayDestroyObjectID.Contains(obj.id))
            {
                lstDelayDestroyObjectID.Add(obj.id);
            }
            return;
        }
        
        dictMapObject.Remove(obj.id);
        DestroyActor(obj);
    }

    public MapHeroObject GetHeroObjectById(int id)
    {
        return GetMapObjectById(id) as MapHeroObject;
    }

    public MapMonsterObject GetMonsterObjectById(int id)
    {
        return GetMapObjectById(id) as MapMonsterObject;
    }

    public MapPetObject GetPetObjectById(int id)
    {
        return GetMapObjectById(id) as MapPetObject;
    }
    
    public MapHeroObject GetLocalHero()
    {
        return GetHeroObjectById(1);
    }
    
    public bool IsExistHero()
    {
        return GetLocalHero() != null;
    }
    
    public bool IsExistMonster()
    {
        foreach (var item in dictMapObject)
        {
            if (item.Value != null && item.Value.ObjectType == MapObjectType.Monster)
            {
                return true;
            }
        }

        return false;
    }
    
    public void RegisterMapFxObject(MapFxObject obj)
    {
        if (!lstMapFxObject.Contains(obj))
        {
            lstMapFxObject.Add(obj);
        }
    }

    public void UnRegisterMapFxObject(MapFxObject obj)
    {
        lstMapFxObject.Remove(obj);
    }
    
    public int NewMapVirtualObjectID(MapObjectType type)
    {
        int virtualType = (int)type;
        if (!dictMapVirtualObjectIDCur.ContainsKey(virtualType))
        {
            dictMapVirtualObjectIDCur[virtualType] = virtualType;
        }
        int id = dictMapVirtualObjectIDCur[virtualType];
        dictMapVirtualObjectIDCur[virtualType] = id + 1;
        return id;
    }
    
    public void UpdateHero(HeroVo item)
    {
        if (item == null)
        {
            return;
        }

        string strResName = ConfigUtils.GetHeroModelPathByID(item.generalsType);

        var obj = GetHeroObjectById(item.unitID);
     
        if (obj != null && obj.Name.Equals(strResName))
        {
            // 没有变化
            obj.HeroAttr.Init(item);
            obj.SetTransform(item.pos, item.rot, false);
            obj.UpdateObject();
        }
        else
        {
            // 不存在或者已失效，回收资源
            if (obj != null)
            {
                DestroyMapObject(obj);
            }

            obj = SpawnActor<MapHeroObject>(strResName) as MapHeroObject;

            if (obj != null)
            {
                obj.HeroAttr.Init(item);
                obj.BeginPlay(item.unitID, item.userID, item.pos, item.rot);

                AddMapObject(obj);
            }
            else
            {
                LogUtils.LogErrorFormat("UpdateHero Create Error {0} {1}", item.unitID, item.userID);
            }
        }

        if (obj != null)
        {
            // 设置为不回收
            obj.IsTryRecycleSign = false;
        }
    }
    
    public MapMonsterObject UpdateMonster(MonsterVo item)
    {
        if (item == null)
        {
            return null;
        }

        string strResName = ConfigUtils.GetMonsterModelPathByID(item.generalsType);
        
        var obj = GetMonsterObjectById(item.unitID);

        if (obj != null && obj.Name.Equals(strResName))
        {
            // 没有变化
            obj.MonsterAttr.Init(item);
            obj.SetTransform(item.pos, item.rot, false);
            obj.UpdateObject();
            obj.PlayAlive();
        }
        else
        {
            // 不存在或者已失效，回收资源
            if (obj != null)
            {
                DestroyMapObject(obj);
            }

            obj = SpawnActor<MapMonsterObject>(strResName) as MapMonsterObject;

            if (obj != null)
            {
                obj.PlayAlive();
                obj.MonsterAttr.Init(item);
                obj.BeginPlay(item.unitID, "", item.pos, item.rot);

                AddMapObject(obj);
            }
            else
            {
                LogUtils.LogErrorFormat("UpdateMonster Create Error {0} {1}", item.unitID, "");
            }
        }

        if (obj != null)
        {
            // 设置为不回收
            obj.IsTryRecycleSign = false;
        }

        return obj;
    }
    
    public void UpdatePet(PetVo item,PetItemInfo petInfo)
    {
        if (item == null)
        {
            return;
        }

        string strResName = ConfigUtils.GePetModelPathByID(item.generalsType);

        var obj = GetPetObjectById(item.unitID);

        if (obj != null && obj.Name.Equals(strResName))
        {
            // 没有变化
            obj.PetAttr.Init(item, petInfo);
            obj.SetTransform(item.pos, item.rot, false);
            obj.UpdateObject();
        }
        else
        {
            // 不存在或者已失效，回收资源
            if (obj != null)
            {
                DestroyMapObject(obj);
            }

            obj = SpawnActor<MapPetObject>(strResName) as MapPetObject;

            if (obj != null)
            {
                obj.PetAttr.Init(item, petInfo);
                obj.BeginPlay(item.unitID, "", item.pos, item.rot);

                AddMapObject(obj);
            }
            else
            {
                LogUtils.LogErrorFormat("UpdatePet Create Error {0} {1}", item.unitID, "");
            }
        }

        if (obj != null)
        {
            // 设置为不回收
            obj.IsTryRecycleSign = false;
        }
        if(!lstPetObj.Contains(obj))
            lstPetObj.Add(obj);
    }
    
    public MapFxObject SpawnFxActor(int effectId, MapObject owner)
    {
        Vector3 pos = owner != null ? owner.Position : Vector3.zero;
        Quaternion rot = owner != null ? owner.Rotation : Quaternion.identity;
        return SpawnFxActor(effectId, pos, rot, owner, Vector3.zero, null, 0);
    }

    public MapFxObject SpawnFxActor(int effectId, Vector3 pos, Quaternion rot)
    {
        return SpawnFxActor(effectId, pos, rot, null, Vector3.zero, null, 0);
    }

    public MapFxObject SpawnFxActor(int effectId, Vector3 pos, Quaternion rot, MapObject owner)
    {
        return SpawnFxActor(effectId, pos, rot, owner, Vector3.zero, null, 0);
    }

    public MapFxObject SpawnFxActor(int effectId, Vector3 pos, Quaternion rot, MapObject owner,
        Vector3 targetPos, MapObject targetObj, float param = 0.0f, System.Action actEnd = null)
    {
        var cfgSkillEffect = ConfigUtils.GetSkillEffectById(effectId);

        if (cfgSkillEffect == null)
        {
            LogUtils.LogErrorFormat("SpawnFxActor Error, id {0}", effectId);
            return null;
        }

        // 绑定骨骼-GetBonePos
        if (owner != null)
        {
            if (!string.IsNullOrEmpty(cfgSkillEffect.Bone) && cfgSkillEffect.Attach == 0)
            {
                pos = owner.DummyPos(cfgSkillEffect.Bone);
            }
        }

        MapFxObject fx = null;

        switch (cfgSkillEffect.Type)
        {
            case (int)EN_SKILL_FX.FX_LINE:
                {
                    MapLineFxObject line = SpawnActor<MapLineFxObject>(cfgSkillEffect.Path) as MapLineFxObject;
                    line.InitLine(cfgSkillEffect, pos, rot, owner, targetObj, targetPos, param);
                    fx = line;
                }
                break;

            case (int)EN_SKILL_FX.FX_PARABOLA:
                {
                    MapParabolaFxObject proj =
                        SpawnActor<MapParabolaFxObject>(cfgSkillEffect.Path) as MapParabolaFxObject;
                    proj.InitParabola(cfgSkillEffect, pos, rot, owner, targetObj, targetPos, param);
                    fx = proj;
                }
                break;

            case (int)EN_SKILL_FX.FX_BEZIER:
                {
                    MapBezierFxObject bezier = SpawnActor<MapBezierFxObject>(cfgSkillEffect.Path) as MapBezierFxObject;
                    bezier.InitBezier(cfgSkillEffect, owner != null ? owner.Position : pos,
                        owner != null ? owner.Rotation : rot,
                        owner, targetObj, targetPos);
                    fx = bezier;
                }
                break;

            case (int)EN_SKILL_FX.FX_CHAIN_LIGHT:
                {
                    MapChainLightFxObject chainLight =
                        SpawnActor<MapChainLightFxObject>(cfgSkillEffect.Path) as MapChainLightFxObject;
                    chainLight.InitChainLight(cfgSkillEffect, owner != null ? owner.Position : pos,
                        owner != null ? owner.Rotation : rot,
                        owner, owner, targetObj, 0, param);
                    fx = chainLight;
                }
                break;

            case (int)EN_SKILL_FX.FX_FOLLOW:
                {
                    MapFollowFxObject fxFollow =
                        SpawnActor<MapFollowFxObject>(cfgSkillEffect.Path) as MapFollowFxObject;
                    fxFollow.InitFollow(cfgSkillEffect, owner != null ? owner.Position : pos,
                        owner != null ? owner.Rotation : rot, owner);
                    fx = fxFollow;
                }
                break;

            case (int)EN_SKILL_FX.FX_DELAY_LIGHT:
                {
                    MapDelayLightFxObject delayLight = SpawnActor<MapDelayLightFxObject>(cfgSkillEffect.Path) as MapDelayLightFxObject;
                    delayLight.InitDelayLight(cfgSkillEffect, owner != null ? owner.Position : pos,
                        owner != null ? owner.Rotation : rot,
                        owner, targetObj, targetPos);
                    fx = delayLight;
                }
                break;

            default:
                {
                    fx = SpawnActor<MapFxObject>(cfgSkillEffect.Path) as MapFxObject;
                    fx?.InitByConfig(cfgSkillEffect, pos, rot, owner);
                }
                break;
        }

        if (actEnd != null)
        {
            fx.EndAction = actEnd;
        }

        // 绑定骨骼-AttachBone
        if (owner != null && !string.IsNullOrEmpty(cfgSkillEffect.Bone) && cfgSkillEffect.Attach != 0)
        {
            if (fx != null)
            {
                fx.AttachBoneParent(owner, cfgSkillEffect.Bone);
            }
        }
        
        // BattleManager.Instance.TriggerSingleBlackdropFactor(fx);

        return fx;
    }
    
    public bool IsEnemy(MapObject partSrc, MapObject partTarget)
    {
        if (partSrc == null || partTarget == null)
        {
            return false;
        }

        if (partSrc.id == partTarget.id)
        {
            return false;
        }

        switch (partTarget.ObjectType)
        {
            case MapObjectType.VirtualFx:
            case MapObjectType.ITEM:
            case MapObjectType.Pet:
            {
                return false;
            }

            default:
                break;
        }

        if (partTarget.IsDead)
        {
            return false;
        }

        if (partSrc.Attr.Camp == EN_CAMP_TYPE.HERO)
        {
            return partTarget.Attr.Camp == EN_CAMP_TYPE.ENEMY;
        }
        else if (partSrc.Attr.Camp == EN_CAMP_TYPE.FRIEND)
        {
            return partTarget.Attr.Camp == EN_CAMP_TYPE.ENEMY;
        }
        else if (partSrc.Attr.Camp == EN_CAMP_TYPE.ENEMY)
        {
            return partTarget.Attr.Camp == EN_CAMP_TYPE.HERO
                   || partTarget.Attr.Camp == EN_CAMP_TYPE.FRIEND;
        }
        else if (partSrc.Attr.Camp == EN_CAMP_TYPE.NEUTRALITY)
        {
            return false;
        }

        return false;
    }
    
    public bool IsFriend(MapObject partSrc, MapObject partTarget)
    {
        if (partSrc == null || partTarget == null)
        {
            return false;
        }

        if (partSrc.id == partTarget.id)
        {
            return false;
        }

        switch (partTarget.ObjectType)
        {
            case MapObjectType.VirtualFx:
            case MapObjectType.ITEM:
            {
                return false;
            }

            default:
                break;
        }

        if (partTarget.IsDead)
        {
            return false;
        }

        if (partSrc.Attr.Camp == EN_CAMP_TYPE.HERO)
        {
            return partTarget.Attr.Camp == EN_CAMP_TYPE.FRIEND;
        }
        else if (partSrc.Attr.Camp == EN_CAMP_TYPE.FRIEND)
        {
            return partTarget.Attr.Camp == EN_CAMP_TYPE.HERO;
        }
        else if (partSrc.Attr.Camp == EN_CAMP_TYPE.ENEMY)
        {
            return partTarget.Attr.Camp == EN_CAMP_TYPE.ENEMY;
        }
        else if (partSrc.Attr.Camp == EN_CAMP_TYPE.NEUTRALITY)
        {
            return false;
        }

        return false;
    }
    
    /// <summary>
    /// 允许战斗
    /// </summary>
    /// <param name="partSrc"></param>
    /// <param name="partTarget"></param>
    /// <param name="bTip"></param>
    /// <returns></returns>
    public bool CanFight(MapObject partSrc, MapObject partTarget)
    {
        if (partSrc == null || partTarget == null)
        {
            return false;
        }

        /// 在安全区内？
        /*if (partSrc.PartType == CommonDefine.PART_TYPE.PART_TYPE_PLAYER && partTarget.PartType == CommonDefine.PART_TYPE.PART_TYPE_PLAYER)
        {
            // 安全区内
            bool bInSafeArea = IsInSafeArea(partTarget);

            if (bInSafeArea)
            {
                if (bTip)
                {
                    CUIPanelContral.ShowTipByKey(CStringDefine.FIGHT_DATA_SAVE_POS_ATTACK_TIP);
                }
                return false;
            }
        }*/

        return true;
    }
    
    /// <summary>
    /// 获取最近的敌人单位
    /// </summary>
    /// <param name="partSrc"></param>
    /// <param name="fDisVisual"></param>
    /// <returns></returns>
    public MapObject GetNearestEnemy(MapObject partSrc, float fDisVisual)
    {
        if (partSrc == null)
        {
            return null;
        }

        float fDisTarget = -1.0f;
        MapObject target = null;

        foreach (var item in dictMapObject)
        {
            var stage = item.Value;

            if (!IsEnemy(partSrc, stage))
            {
                continue;
            }

            if (!CanFight(partSrc, stage))
            {
                continue;
            }

            float fTmp = Utils.DistanceIgnoreZ(stage.Position, partSrc.Position);

            if (!Mathf.Approximately(fDisVisual, 0.0f))
            {
                if (fTmp > fDisVisual)
                {
                    continue;
                }
            }

            if (fDisTarget < 0 || fTmp < fDisTarget)
            {
                fDisTarget = fTmp;
                target = stage;
            }
        }

        return target;
    }
    
    public List<MapObject> GetAllEnemy(MapObject partSrc, Vector3 pos, float fDisVisual)
    {
        List<MapObject> lstEnemy = new List<MapObject>();
        
        if (partSrc == null)
        {
            return lstEnemy;
        }

        foreach (var item in dictMapObject)
        {
            var stage = item.Value;

            if (!IsEnemy(partSrc, stage))
            {
                continue;
            }

            if (!CanFight(partSrc, stage))
            {
                continue;
            }

            float fTmp = Utils.DistanceIgnoreZ(stage.Position, pos);

            if (!Mathf.Approximately(fDisVisual, 0.0f))
            {
                if (fTmp > fDisVisual)
                {
                    continue;
                }
            }
            
            lstEnemy.Add(stage);
        }

        return lstEnemy;
    }
    
    public void GetSkillTarget(MapObject partSrc, MapObject target, int skiilId, out List<MapObject> lstTarget, out Vector3 posAttack)
    {
        lstTarget = new List<MapObject>();
        posAttack = Vector3.zero;
        
        if (target == null)
        {
            return;
        }
        
        lstTarget.Add(target);
        posAttack = target.Position;

        var skill = ConfigUtils.GetSkillById(skiilId);

        if (skill == null)
        {
            return;
        }

        var skillTarget = ConfigUtils.GetSkillTargetById(skill.SkillTarget);

        if (skillTarget == null)
        {
            return;
        }

        if (skillTarget.RangeType == (int)EN_TARGET_RANGE_TYPE.NONE)
        {
            List<MapObject> lstEnemy = GetAllEnemy(partSrc, partSrc.Position, 0);
            lstEnemy.Remove(target);
            if (skillTarget.TargetNumber == 0)
            {
                lstTarget.AddRange(lstEnemy);
            }
            else
            {
                --skillTarget.TargetNumber;
                if (lstEnemy.Count > skillTarget.TargetNumber)
                {
                    if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ZUIJIN)
                    {
                        tmpSortBase = target;
                        lstEnemy.Sort(SortZUIJIN);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ZUIYUAN)
                    {
                        tmpSortBase = target;
                        lstEnemy.Sort(SortZUIYUAN);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.HPZUIDI)
                    {
                        lstEnemy.Sort(SortHPZUIDI);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.HPZUIGAO)
                    {
                        lstEnemy.Sort(SortHPZUIGAO);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ATKZUIDI)
                    {
                        lstEnemy.Sort(SortATKZUIDI);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ATKZUIGAO)
                    {
                        lstEnemy.Sort(SortATKZUIGAO);
                    }
                    lstEnemy.RemoveRange(skillTarget.TargetNumber, lstEnemy.Count - skillTarget.TargetNumber);
                }
                lstTarget.AddRange(lstEnemy);
            }
        }
        else if (skillTarget.RangeType == (int)EN_TARGET_RANGE_TYPE.PARAM)
        {
            var pos = partSrc.Position;

            if (skillTarget.RangeRefer == (int)EN_TARGET_RANGEREFER_TYPE.MUBIAO)
            {
                pos = target.Position;
            }
            else if (skillTarget.RangeRefer == (int)EN_TARGET_RANGEREFER_TYPE.ZISHENQIANFANG)
            {
                pos = partSrc.Position;
                pos.x += skillTarget.RangeData * (partSrc.Rotation.eulerAngles.y > 180.0f ? -1 : 1);
                
                posAttack = pos;
            }
            else if (skillTarget.RangeRefer == (int)EN_TARGET_RANGEREFER_TYPE.ZISHENQIANFANGZHONGPAI)
            {
                pos = partSrc.Position;

                if (partSrc.Attr.Camp == EN_CAMP_TYPE.HERO || partSrc.Attr.Camp == EN_CAMP_TYPE.FRIEND)
                {
                    // 我方，使用Hero位置
                    var localHero = MapObjectManager.Instance.GetLocalHero();

                    if (localHero != null)
                    {
                        pos.x = localHero.Position.x;
                        pos.y = localHero.Position.y;
                    }
                }
                else
                {
                    // 敌方，使用Hero对应的中心位置
                    var localHero = MapObjectManager.Instance.GetLocalHero();

                    if (localHero != null)
                    {
                        pos.x = localHero.Position.x;
                        pos.y = localHero.Position.y;
                    }
                    
                    pos.x += 260.0f;
                }
                
                pos.x += skillTarget.RangeData * (partSrc.Rotation.eulerAngles.y > 180.0f ? -1 : 1);
                
                posAttack = pos;
            }
            
            List<MapObject> lstEnemy = GetAllEnemy(partSrc, pos, skillTarget.RangeParam);

            if (skillTarget.TargetNumber == 0)
            {
                lstEnemy.Remove(target);
                lstTarget.AddRange(lstEnemy);
            }
            else
            {
                if (lstEnemy.Count <= 0)  //范围内没有敌人
                {
                    lstTarget.Clear();
                }
                lstEnemy.Remove(target);
                
                //--skillTarget.TargetNumber;
                if (lstEnemy.Count > (skillTarget.TargetNumber - 1) )
                {
                    if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ZUIJIN)
                    {
                        tmpSortBase = target;
                        lstEnemy.Sort(SortZUIJIN);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ZUIYUAN)
                    {
                        tmpSortBase = target;
                        lstEnemy.Sort(SortZUIYUAN);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.HPZUIDI)
                    {
                        lstEnemy.Sort(SortHPZUIDI);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.HPZUIGAO)
                    {
                        lstEnemy.Sort(SortHPZUIGAO);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ATKZUIDI)
                    {
                        lstEnemy.Sort(SortATKZUIDI);
                    }
                    else if (skillTarget.TargetPriority == (int)EN_TARGET_TARGETPRIORITY.ATKZUIGAO)
                    {
                        lstEnemy.Sort(SortATKZUIGAO);
                    }
                    lstEnemy.RemoveRange((skillTarget.TargetNumber - 1), lstEnemy.Count - (skillTarget.TargetNumber - 1));
                }
                lstTarget.AddRange(lstEnemy);
            }
        }
    }

    private static MapObject tmpSortBase;
    
    public static int SortZUIJIN(MapObject a, MapObject b)
    {
        var pos = Vector3.zero;
        if (tmpSortBase != null)
        {
            pos = tmpSortBase.Position;
        }
        float a1 = Utils.DistanceIgnoreZ(a.Position, pos);
        float b1 = Utils.DistanceIgnoreZ(b.Position, pos);
        return a1.CompareTo(b1);
    }
    
    public static int SortZUIYUAN(MapObject a, MapObject b)
    {
        var pos = Vector3.zero;
        if (tmpSortBase != null)
        {
            pos = tmpSortBase.Position;
        }
        float a1 = Utils.DistanceIgnoreZ(a.Position, pos);
        float b1 = Utils.DistanceIgnoreZ(b.Position, pos);
        return b1.CompareTo(a1);
    }
    
    public static int SortHPZUIGAO(MapObject a, MapObject b)
    {
        return b.Attr.HP.CompareTo(a.Attr.HP);
    }
    
    public static int SortHPZUIDI(MapObject a, MapObject b)
    {
        return a.Attr.HP.CompareTo(b.Attr.HP);
    }
    
    public static int SortATKZUIGAO(MapObject a, MapObject b)
    {
        return b.Attr.Atk.CompareTo(a.Attr.Atk);
    }
    
    public static int SortATKZUIDI(MapObject a, MapObject b)
    {
        return a.Attr.Atk.CompareTo(b.Attr.Atk);
    }

    /// <summary>
    /// 是否闪避
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsJoukDamage(MapObject attack, MapObject target)
    {
        if (attack == null || target == null)
        {
            return false;
        }

        if (target is MapMonsterObject)
        {
            float ret = Mathf.Clamp01((float)target.Attr.JoukRate);
            return Random.Range(0.0f, 1.0f) < ret;
        }
        else if (target is MapHeroObject)
        {
            FightAttrVo fightAttrVo = GetFightAttrVoByCamp(target);
            float ret = Mathf.Clamp01((float)fightAttrVo.JoukRate);
            return Random.Range(0.0f, 1.0f) < ret;
        }
        return false;
    }
    
    /// <summary>
    /// 是否暴击
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsCriticalDamage(int skillID, MapObject attack, MapObject target)
    {
        var skillType = ConfigUtils.GetSkillById(skillID);
        
        if (attack == null || target == null || skillType == null)
        {
            return false;
        }

        if (attack is MapMonsterObject)
        {
            var ret = Math.Max(0, attack.Attr.CriticalStrike);
            return Random.Range(0.0f, 1.0f) < ret;
        }
        else
        {
            double ret = 0;
            FightAttrVo fightAttrVo = GetFightAttrVoByCamp(attack);
            if (skillType.SkillType == (int)EN_SKILL_TYPE.XP)
            {
                ret = fightAttrVo.CriticalStrike;
            }
            else
            {
                ret = Math.Max(0, attack.Attr.CriticalStrike);
            }
        
            return Random.Range(0.0f, 1.0f) < ret; 
        }
    }
    
    /// <summary>
    /// 是否格挡
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsMonsterParryDamage(MapObject attack, MapObject target)
    {
        if (attack == null || target == null)
        {
            return false;
        }
        // if (attack is MapMonsterObject)
        //     return false;
        
        // FightAttrVo fightAttrVo = GetFightAttrVoByCamp(attack);
        var ret = Mathf.Clamp01((float)target.Attr.ParryRate);
        return Random.Range(0.0f, 1.0f) < ret;
    }
    
    /// <summary>
    /// 是否触发连击
    /// </summary>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsComboDamage(MapObject attack, MapObject target)
    {
        if (attack == null || target == null)
        {
            return false;
        }

        if (attack is MapMonsterObject)
        {
            var ret = Mathf.Clamp01(attack.Attr.ComboAtk);
            return Random.Range(0.0f, 1.0f) < ret;
        }
        else
        {
            FightAttrVo fightAttrVo = GetFightAttrVoByCamp(attack);
            var ret = Mathf.Clamp01(fightAttrVo.ComboAtk);
            return Random.Range(0.0f, 1.0f) < ret;
        }
    }
    
    /// <summary>
    /// 是否反击
    /// </summary>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsCounterDamage(MapObject attack, MapObject target)
    {
        if (attack == null || target == null)
        {
            return false;
        }

        if (attack is MapMonsterObject)
        {
            var ret = Mathf.Clamp01(attack.Attr.CounterAtk);
            return Random.Range(0.0f, 1.0f) < ret;
        }
        else
        {
            FightAttrVo fightAttrVo = GetFightAttrVoByCamp(attack);
            var ret = Mathf.Clamp01(fightAttrVo.CounterAtk);
            return Random.Range(0.0f, 1.0f) < ret;
        }
    }
    
    /// <summary>
    /// 是否触发吸血
    /// </summary>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsBloodDamage(MapObject attack, MapObject target)
    {
        if (attack == null || target == null)
        {
            return false;
        }
        if ( attack is MapPetObject)
            return false;
        if (attack is MapMonsterObject)
        {
            return attack.Attr.Bloodsucking > 0;
        }
        else
        {
            FightAttrVo fightAttrVo = GetFightAttrVoByCamp(attack);
            return fightAttrVo?.Bloodsucking > 0;
        }
    }

    private FightAttrVo GetFightAttrVoByCamp(MapObject attack)
    {
        FightAttrVo fightAttrVo = null;
       
        if (attack.Attr.Camp == EN_CAMP_TYPE.ENEMY)
        {
            // 需要改成对方的
            fightAttrVo = PvpRankDataManager.Instance.OtherFightAttrVo;
        }
        else
        {
            if (attack.Attr.Camp == EN_CAMP_TYPE.FRIEND) //宠物
                if (attack is MapSkillProxy && attack.Attr.PetGuid != 0)
                {
                    fightAttrVo = PetInfoManager.Instance.GetPet(attack.Attr.PetGuid).FightAttrVo;
                }
                else if ( attack is MapPetObject && (attack as MapPetObject).PetAttr != null)
                {
                    fightAttrVo = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo;
                }
                else
                {
                    fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
                }
            else
                fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
        }
        

        return fightAttrVo;
    }
    
    private double GetHeroSkillDamageRateByCamp(MapObject attack)
    {
        FightAttrVo fightAttrVo = null;
       
        if (attack.Attr.Camp == EN_CAMP_TYPE.ENEMY)
        {
            //需要改成对方的
            return PvpRankDataManager.Instance.OtherSkillDamageRate;
        }

        return HeroInfoManager.Instance.GetMyHero().SkillDamageRate;

    }
    
    private double GetMagicTimesByCamp(MapObject attack, int skillID)
    {
        double magicTimes = 0;
       
        if (attack.Attr.Camp == EN_CAMP_TYPE.ENEMY)
        {
            // 需要改成对方的  //TODO 需要改成对方安装传承装备携带的符石技能
            magicTimes = PvpRankDataManager.Instance.GetOtherSkillTimes(skillID);
        }
        else
        {
            // magicTimes = RuneInfoManager.Instance.GetSkillTimes(skillID);
            magicTimes = EquipManager.Instance.GetSkillTimesWithEquip(skillID) * (1 + attack.Attr.MagicTimesAdd);
        }
        

        return Math.Floor(magicTimes);
    }

    private Dictionary<int, int> magicTimeDict = new Dictionary<int, int>();
    private int maxMagicTimes = 0;
    private float skillTimesHurt = 1.0f;
    /// <summary>
    /// 装备技能里面，传承装备提供的最大的技能次数
    /// </summary>
    /// <returns></returns>
    private float RoleEquipSkillMaxTimes()
    {
        magicTimeDict.Clear();
        foreach (var equipData in EquipManager.Instance.GetInstallLoreEquipsList())
        {
            if (equipData.skillMultiplesList != null && equipData.skillMultiplesList.Count > 0)
            {
                foreach (var skillMultipleData in equipData.skillMultiplesList)
                {
                    foreach (var rune in SkillInfoManager.Instance.GetBattleSkillList())
                    {
                        if (rune.SkillId == skillMultipleData.skillId)
                        {
                            if (magicTimeDict.ContainsKey(rune.SkillId))
                            {
                                magicTimeDict[rune.SkillId] += skillMultipleData.times;
                            }
                            else
                            {
                                magicTimeDict.Add(rune.SkillId, skillMultipleData.times);
                            }
                        }
                    }
                }
            }
        }

        maxMagicTimes = 0;
        foreach (var item in magicTimeDict)
        {
            if (item.Value > maxMagicTimes)
                maxMagicTimes = item.Value;
        }
        
        skillTimesHurt = 1.0f;
        if (maxMagicTimes > 3) // 宠物伤害增加   （玩家最高技能次数/3，玩家技能次数大于3时启用）
        {
            skillTimesHurt = maxMagicTimes / 3.0f;
        }

        return skillTimesHurt;
    }
    
    private float baseMagicTimesHurt = 1.0f;
    /// <summary>
    /// 基础伤害
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public double GetBaseDamage(int skillID, MapObject attack, MapObject target, CastSkillType type)
    {
        if (attack == null || target == null)
        {
            return 0;
        }
        target.IsParry = false;
        
        FightAttrVo fightAttrVo = GetFightAttrVoByCamp(attack);

        baseMagicTimesHurt = 1.0f;
        if (GetLocalHero() != null && (attack is MapPetObject || (attack is MapSkillProxy && attack.Attr.Camp == EN_CAMP_TYPE.FRIEND))) // 宠物伤害增加   （玩家最高技能次数/3，玩家技能次数大于3时启用）
        {
            baseMagicTimesHurt = RoleEquipSkillMaxTimes();
        }

        double damage = 0;
        int targetType = 0;//0=角色 1=小怪 2=boss
        if (skillID > 0)
        {
            ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(skillID);
            if (skillUnit.SkillType == (int) EN_SKILL_TYPE.XP || skillUnit.SkillType == (int) EN_SKILL_TYPE.ChoukaSkill)//技能伤害
            {
                double skillDamage = skillUnit.SkillDamage;
                if (type == CastSkillType.Hero)
                {
                    skillDamage = GetHeroSkillDamageRateByCamp(attack);
                }
                else if(type == CastSkillType.SkillProxy)
                {
                    if (attack is MapSkillProxy && attack.Attr.PetGuid != 0)
                        skillDamage = 0; //宠物释放的技能 技能伤害读 宠物技能等级表的 fightAttrVo.SkillDamage
                    else
                        skillDamage = SkillInfoManager.Instance.GetSkillDamageById(skillUnit.Id);
                }

                skillDamage = skillDamage * ConstDefine.CONFIG_PLACE_EX;
                double magicTimes = 1;
                if (target is MapMonsterObject)
                {
                    // TODO 改成释放的技能和 传承装备的关联技能id一样的话，才使用传承装备的关联技能次数   
                    magicTimes = Mathf.Max(1, (int) attack.Attr.MagicTimes);
                    magicTimes += GetMagicTimesByCamp(attack, skillID);  //传承装备携带抽卡技能ID有关联符石技能的次数
                    targetType = 1;
                    bool isBoss = ((MapMonsterObject) target).MonsterAttr.MonsterVo.IsBoss;
                    if (isBoss)
                        targetType = 2;
                    if (targetType == 1)//小怪
                    {//（我方总物理伤害*战前总技能伤害*战前总技能次数*（1+BOSS加成or小怪加成）*（（暴击率判定：1,0）*战前总爆伤））*（1+伤害倍率）-（我方无视防御*敌方总物理防御）-（格挡判定：1,0）-总格挡值）*（1-战前减伤比例）* 战前总最终伤害
                         if (IsMonsterParryDamage(attack, target))
                         {
                             target.IsParry = true;
                             damage = (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.MonsterDamageAdd) * (1 + fightAttrVo.AtkMultiple) -
                                       ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) - target.Attr.ParryValue) *
                                      (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                         }
                         else
                         {
                             damage = (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.MonsterDamageAdd) * (1 + fightAttrVo.AtkMultiple) -
                                       ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def)) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) *
                                      fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                         }
                        //战前总攻击力*战前总技能伤害*战前总魔法次数*（1+BOSS加成or小怪加成）*（暴击判定：1,0）*战前总爆伤*（1-战前减伤比例）
                        //damage = fightAttrVo.Atk * (fightAttrVo.SkillDamage+skillDamage) * (1 + fightAttrVo.MonsterDamageAdd) * (1-((MapMonsterObject) target).MonsterAttr.Mitigation);
                    }
                    else
                    {
                        if (IsMonsterParryDamage(attack, target))
                        {
                            target.IsParry = true;
                            damage = (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.BossDamageAdd) *
                                      (1 + fightAttrVo.AtkMultiple)
                                      - ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) - target.Attr.ParryValue)
                                     * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                        }
                        else
                        {
                            damage = (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.BossDamageAdd) *
                                      (1 + fightAttrVo.AtkMultiple)
                                      - ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def))
                                     * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                        }
                        //damage = fightAttrVo.Atk * (fightAttrVo.SkillDamage+skillDamage) * (1 + fightAttrVo.BossDamageAdd) * (1-((MapMonsterObject) target).MonsterAttr.Mitigation);
                    }
 
                }
                else//角色
                {
                    if (attack is MapMonsterObject)
                    {
                        var monsterAttr = ((MapMonsterObject)attack).MonsterAttr;
                        if (IsMonsterParryDamage(attack, target))
                        {
                            target.IsParry = true;
                            damage = ( monsterAttr.Atk * (monsterAttr.SkillDamage + skillDamage) * (1 + monsterAttr.MonsterDamageAdd) * (1 + monsterAttr.AtkMultiple)
                                       - ((1 - monsterAttr.IgnoreDef) * target.Attr.Def) - target.Attr.ParryValue )
                                     * (1 - target.Attr.Mitigation) * monsterAttr.BattleFinalAttack;
                        }
                        else
                        {
                            damage = (monsterAttr.Atk * (monsterAttr.SkillDamage + skillDamage) * (1 + monsterAttr.MonsterDamageAdd) *
                                         (1 + monsterAttr.AtkMultiple) - ((1 - monsterAttr.IgnoreDef) * target.Attr.Def)) *
                                     (1 - target.Attr.Mitigation) * monsterAttr.BattleFinalAttack;
                        }
                        // Debug.LogWarningFormat("skillName={0}     damage={1}", skillUnit.Name,  damage);
                    }
                    else if(attack is MapHeroObject || attack is MapSkillProxy || attack is MapHeroSkillProxy)
                    {
                        magicTimes = Mathf.Max(1, (int) attack.Attr.MagicTimes);
                        magicTimes += GetMagicTimesByCamp(attack, skillID);
                        damage = fightAttrVo.Atk * (fightAttrVo.SkillDamage+skillDamage) * (1-((MapHeroObject) target).Attr.Mitigation);
                    }
                }
                
                if (attack is MapMonsterObject)
                {
                    damage = AddRangeDamage(damage, ((MapMonsterObject)attack).MonsterAttr.Atk);
                }
                else
                {
                    damage = AddRangeDamage(damage, fightAttrVo.Atk);
                    if (attack is MapSkillProxy && attack.Attr.PetGuid != 0 && fightAttrVo.SkillDamage == 0)  //宠物技能给角色加buff  技能伤害时0
                    {
                        damage = 0;
                    }
                }
                return Math.Ceiling(damage) * magicTimes;
            }
        }
        
        if (target is MapMonsterObject)
        {
            targetType = 1;
            bool isBoss = ((MapMonsterObject) target).MonsterAttr.MonsterVo.IsBoss;
            if (isBoss)
                targetType = 2;
        }
        
        if (targetType == 1)//小怪
        { //（我方总物理伤害*（1+BOSS加成or小怪加成）*（（暴击率判定：1,0）*战前总爆伤））*（1+伤害倍率）-（我方无视防御*敌方总物理防御）-（格挡判定：1,0）-总格挡值）*（1-战前减伤比例）
            if (IsMonsterParryDamage(attack, target))
            {
                target.IsParry = true;
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.MonsterDamageAdd) * (1 + fightAttrVo.AtkMultiple) - ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) -
                          target.Attr.ParryValue) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
            }
            else
            {
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.MonsterDamageAdd) * (1 + fightAttrVo.AtkMultiple) - ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def)) *
                         (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
            }
            // damage = fightAttrVo.Atk * (1 + fightAttrVo.MonsterDamageAdd) * (1-((MapMonsterObject) target).MonsterAttr.Mitigation);
        }
        else if (targetType == 2)//boss
        {
            // damage = fightAttrVo.Atk * (1 + fightAttrVo.BossDamageAdd) * (1-((MapMonsterObject) target).MonsterAttr.Mitigation);
            if (IsMonsterParryDamage(attack, target))
            {
                target.IsParry = true;
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.BossDamageAdd) * (1 + fightAttrVo.AtkMultiple) - ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) -
                          target.Attr.ParryValue) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
            }
            else
            {
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.BossDamageAdd) * (1 + fightAttrVo.AtkMultiple) - ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def)) *
                         (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
            }
        }
        else //角色  怪物打自己或者其它玩家到自己
        {
            if (attack is MapMonsterObject)
            {
                var monsterAttr = ((MapMonsterObject)attack).MonsterAttr;
                if (IsMonsterParryDamage(attack, target))
                {
                    target.IsParry = true;
                    damage = ( monsterAttr.Atk * (1 + monsterAttr.MonsterDamageAdd) * (1 + monsterAttr.AtkMultiple) - 
                               ((1 - monsterAttr.IgnoreDef) * target.Attr.Def) - target.Attr.ParryValue ) * 
                             (1 - target.Attr.Mitigation) * monsterAttr.BattleFinalAttack;
                }
                else
                {
                    damage = (monsterAttr.Atk * (1 + monsterAttr.MonsterDamageAdd) * (1 + monsterAttr.AtkMultiple) -
                              ((1 - monsterAttr.IgnoreDef) * target.Attr.Def)) * (1 - target.Attr.Mitigation) * monsterAttr.BattleFinalAttack;
                }
            }
            else if(attack is MapHeroObject && target is MapHeroObject def)
            {
                damage = fightAttrVo.Atk * (1-def.Attr.Mitigation);
            }
        }

        if (attack is MapMonsterObject)
        {
            damage = AddRangeDamage(damage, ((MapMonsterObject)attack).MonsterAttr.Atk);
        }
        else
        {
            damage = AddRangeDamage(damage, fightAttrVo.Atk);
        }
        return damage;
    }

    private double AddRangeDamage(double damage, double atk)
    {
        if (damage <= int.Parse(_common18.Param2.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX * atk)
        {
            var num = Random.Range(int.Parse(_common18.Param2.Split(",")[0]), int.Parse(_common18.Param2.Split(",")[1]));
            damage = num * ConstDefine.CONFIG_PLACE_EX * atk;
        }
        else
        {
            var num = Random.Range(int.Parse(_common18.Param1.Split(",")[0]), int.Parse(_common18.Param1.Split(",")[1]));
            damage = num * ConstDefine.CONFIG_PLACE_EX * damage;
        }
        return damage;
        /*
        if (attack is MapMonsterObject)
        {
            if (damage < int.Parse(_common18.Param2.Split(",")[0]) * ConstDefine.CONFIG_PLACE_EX * ((MapMonsterObject)attack).MonsterAttr.Atk)
            {
                var num = Random.Range(int.Parse(_common18.Param1.Split(",")[0]), int.Parse(_common18.Param1.Split(",")[1]));
                damage = num * ConstDefine.CONFIG_PLACE_EX * ((MapMonsterObject)attack).MonsterAttr.Atk;
            }
            else
            {
                var num = Random.Range(int.Parse(_common18.Param1.Split(",")[0]), int.Parse(_common18.Param1.Split(",")[1]));
                damage = num * ConstDefine.CONFIG_PLACE_EX * damage;
            }
        }
        else
        {
            
            if (damage < int.Parse(_common18.Param2.Split(",")[0]) * ConstDefine.CONFIG_PLACE_EX * fightAttrVo.Atk)
            {
                var num = Random.Range(int.Parse(_common18.Param1.Split(",")[0]), int.Parse(_common18.Param1.Split(",")[1]));
                damage = num * ConstDefine.CONFIG_PLACE_EX * fightAttrVo.Atk;
            }
            else
            {
                var num = Random.Range(int.Parse(_common18.Param1.Split(",")[0]), int.Parse(_common18.Param1.Split(",")[1]));
                damage = num * ConstDefine.CONFIG_PLACE_EX * damage;
            }
        }
        */
    }
    
    /// <summary>
    /// 连击伤害
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public double GetComboDamage(MapObject attack, MapObject target)
    {
        if (attack == null || target == null)
        {
            return 0;
        }

        return GetBaseDamage(0 , attack, target, CastSkillType.None);
    }
    
    /// <summary>
    /// 反击伤害
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public double GetCounterDamage(MapObject attack, MapObject target)
    {
        if (attack == null || target == null)
        {
            return 0;
        }
        
        return GetBaseDamage(0 , attack, target, CastSkillType.None);
    }
    
    /// <summary>
    /// 暴击伤害值计算
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="attack"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public double GetCriticalDamage(int skillID, MapObject attack, MapObject target, CastSkillType type)
    {
        if (attack == null || target == null)
        {
            return 0;
        }
        
        FightAttrVo fightAttrVo = GetFightAttrVoByCamp(attack);
        
        baseMagicTimesHurt = 1.0f;
        if (GetLocalHero() != null && (attack is MapPetObject || (attack is MapSkillProxy && attack.Attr.Camp == EN_CAMP_TYPE.FRIEND))) // 宠物伤害增加   （玩家最高技能次数/3，玩家技能次数大于3时启用）
        {
            baseMagicTimesHurt = RoleEquipSkillMaxTimes();
        }
        
        double damage = 0;
        int targetType = 0;//0=角色 1=小怪 2=boss
        if (skillID > 0)
        {
            ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(skillID);
            if (skillUnit.SkillType == (int) EN_SKILL_TYPE.XP || skillUnit.SkillType == (int) EN_SKILL_TYPE.ChoukaSkill)//技能伤害
            {
                double skillDamage = skillUnit.SkillDamage;
                if (type == CastSkillType.Hero)
                {
                    skillDamage = GetHeroSkillDamageRateByCamp(attack);
                }
                else if(type == CastSkillType.SkillProxy)
                {
                    if (attack is MapSkillProxy && attack.Attr.PetGuid != 0)
                        skillDamage = 0; //宠物释放的技能 技能伤害读 宠物技能等级表的 fightAttrVo.SkillDamage
                    else
                        skillDamage = SkillInfoManager.Instance.GetSkillDamageById(skillUnit.Id);
                }
                skillDamage = skillDamage * ConstDefine.CONFIG_PLACE_EX;
                double magicTimes = 1;
                if (target is MapMonsterObject)
                {
                    targetType = 1;
                    bool isBoss = ((MapMonsterObject) target).MonsterAttr.MonsterVo.IsBoss;
                    if (isBoss)
                        targetType = 2;
                    magicTimes = Mathf.Max(1, (int) attack.Attr.MagicTimes);
                    magicTimes += GetMagicTimesByCamp(attack, skillID);
                    if (targetType == 1)//小怪
                    {
                        //战前总攻击力*战前总技能伤害*战前总魔法次数*（1+BOSS加成or小怪加成）*（暴击判定：1,0）*战前总爆伤*（1-战前减伤比例）
                        // damage = fightAttrVo.Atk * (fightAttrVo.SkillDamage+skillDamage) * (1 + fightAttrVo.MonsterDamageAdd)  * fightAttrVo.CriticalInjury * (1-((MapMonsterObject) target).MonsterAttr.Mitigation);
                        
                        //（我方总物理伤害*战前总技能伤害*战前总技能次数*（1+BOSS加成or小怪加成）*（（暴击率判定：1,0）*战前总爆伤））*（1+伤害倍率）-（我方无视防御*敌方总物理防御）-（格挡判定：1,0）-总格挡值）*（1-战前减伤比例）*战前总最终伤害
                        if (IsMonsterParryDamage(attack, target))
                        {
                            target.IsParry = true;
                            damage =
                                (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.MonsterDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                                 ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) - target.Attr.ParryValue) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) *
                                fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                        }
                        else
                        {
                            damage =
                                (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.MonsterDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                                 ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def)) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) *
                                fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                        }
                    }
                    else if (targetType == 2)//boss
                    {
                        // damage = fightAttrVo.Atk * (fightAttrVo.SkillDamage+skillDamage) * (1 + fightAttrVo.BossDamageAdd) * fightAttrVo.CriticalInjury * (1-((MapMonsterObject) target).MonsterAttr.Mitigation);
                        if (IsMonsterParryDamage(attack, target))
                        {
                            target.IsParry = true;
                            damage = (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.BossDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                                      ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) - target.Attr.ParryValue) *
                                     (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                        }
                        else
                        {
                            damage = (fightAttrVo.Atk * (fightAttrVo.SkillDamage + skillDamage) * (1 + fightAttrVo.BossDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                                      ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def)) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) *
                                     fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
                        }
                    }
                }
                else //角色
                {
                    if (attack is MapMonsterObject)
                    {
                        if (IsMonsterParryDamage(attack, target))
                        {
                            target.IsParry = true;
                            damage = ((MapMonsterObject)attack).MonsterAttr.Atk * (((MapMonsterObject)attack).MonsterAttr.SkillDamage + skillDamage) *
                                     ((MapMonsterObject)attack).MonsterAttr.CriticalInjury * (1- target.Attr.Mitigation) - target.Attr.ParryValue;
                        }
                        else
                        {
                            damage = ((MapMonsterObject)attack).MonsterAttr.Atk * (((MapMonsterObject)attack).MonsterAttr.SkillDamage + skillDamage) * 
                                     ((MapMonsterObject)attack).MonsterAttr.CriticalInjury * (1- target.Attr.Mitigation);
                        }
                    }
                    else if(attack is MapHeroObject || attack is MapSkillProxy || attack is MapHeroSkillProxy)
                    {
                        magicTimes = Mathf.Max(1, (int) attack.Attr.MagicTimes);
                        magicTimes += GetMagicTimesByCamp(attack, skillID);
                        damage = fightAttrVo.Atk * (fightAttrVo.SkillDamage+skillDamage) * fightAttrVo.CriticalInjury * (1-((MapHeroObject) target).Attr.Mitigation);
                    }
                }
                
                if (attack is MapMonsterObject)
                {
                    damage = AddRangeDamage(damage, ((MapMonsterObject)attack).MonsterAttr.Atk);
                }
                else
                {
                    damage = AddRangeDamage(damage, fightAttrVo.Atk);
                    if (attack is MapSkillProxy && attack.Attr.PetGuid != 0 && fightAttrVo.SkillDamage == 0)  //宠物技能给角色加buff  技能伤害时0
                    {
                        damage = 0;
                    }
                }
                return Math.Ceiling(damage)*magicTimes;
            }
        }
        
        if (target is MapMonsterObject)
        {
            targetType = 1;
            bool isBoss = ((MapMonsterObject) target).MonsterAttr.MonsterVo.IsBoss;
            if (isBoss)
                targetType = 2;
        }
        
        if (targetType == 1)//小怪
        {
            // 战前总攻击力*（1+BOSS加成or小怪加成）*（（暴击率判定：1,0）*战前总爆伤））*（1-战前减伤比例）
            // damage = fightAttrVo.Atk *  (1 + fightAttrVo.MonsterDamageAdd) * fightAttrVo.CriticalInjury;
            
            //（我方总物理伤害*（1+BOSS加成or小怪加成）*（（暴击率判定：1,0）*战前总爆伤））*（1+伤害倍率）-（我方无视防御*敌方总物理防御）-（格挡判定：1,0）-总格挡值）*（1-战前减伤比例）
            if (IsMonsterParryDamage(attack, target))
            {
                target.IsParry = true;
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.MonsterDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                          ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) - target.Attr.ParryValue) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) *
                         fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
            }
            else
            {
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.MonsterDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                          ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def)) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack *
                         baseMagicTimesHurt;
            }
        }
        else if (targetType == 2)//boss
        {
            // damage = fightAttrVo.Atk * (1 + fightAttrVo.BossDamageAdd) * fightAttrVo.CriticalInjury;
            
            //（我方总物理伤害*（1+BOSS加成or小怪加成）*（（暴击率判定：1,0）*战前总爆伤））*（1+伤害倍率）-（我方无视防御*敌方总物理防御）-（格挡判定：1,0）-总格挡值）*（1-战前减伤比例）
            if (IsMonsterParryDamage(attack, target))
            {
                target.IsParry = true;
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.BossDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                          ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def) - target.Attr.ParryValue) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) *
                         fightAttrVo.BattleFinalAttack * baseMagicTimesHurt;
            }
            else
            {
                damage = (fightAttrVo.Atk * (1 + fightAttrVo.BossDamageAdd) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.AtkMultiple) -
                          ((1 - fightAttrVo.IgnoreDef) * ((MapMonsterObject)target).MonsterAttr.Def)) * (1 - ((MapMonsterObject)target).MonsterAttr.Mitigation) * fightAttrVo.BattleFinalAttack *
                         baseMagicTimesHurt;
            }
        }
        else //角色
        {
            if (attack is MapMonsterObject)
            {
                var monsterAttr = ((MapMonsterObject)attack).MonsterAttr;
                if (IsMonsterParryDamage(attack, target))
                {
                    target.IsParry = true;
                    damage = ( monsterAttr.Atk * (1 + monsterAttr.MonsterDamageAdd) * monsterAttr.CriticalInjury * (1 + monsterAttr.AtkMultiple) - 
                               ((1 - monsterAttr.IgnoreDef) * target.Attr.Def) - target.Attr.ParryValue ) * 
                             (1 - target.Attr.Mitigation) * monsterAttr.BattleFinalAttack;
                }
                else
                {
                    damage = (monsterAttr.Atk * (1 + monsterAttr.MonsterDamageAdd) * monsterAttr.CriticalInjury * (1 + monsterAttr.AtkMultiple) -
                              ((1 - monsterAttr.IgnoreDef) * target.Attr.Def)) * (1 - target.Attr.Mitigation) * monsterAttr.BattleFinalAttack;
                }
            }
            else if (attack is MapHeroObject && (target is MapHeroObject def))
            {
                damage = fightAttrVo.Atk * fightAttrVo.CriticalInjury * (1-def.Attr.Mitigation);
            }
        }

        if (attack is MapMonsterObject)
        {
            damage = AddRangeDamage(damage, ((MapMonsterObject)attack).MonsterAttr.Atk);
        }
        else
        {
            damage = AddRangeDamage(damage, fightAttrVo.Atk);
        }
        return damage;
    }
    
    #region 关卡有限状态机

    public EN_GUANKA_STEP GuanKaStep { get; private set; } = EN_GUANKA_STEP.INIT;

    public int GuanKaMonsterIndex { get; private set; }

    private CTimer timerShow1 = new CTimer();
    private CTimer timerShow2 = new CTimer();
    private CTimer timreFightResult = new CTimer();

    public int ChapterIndex => DataManager.Instance.GetRoleData().chapterId;
    public int GuanKaStageId => DataManager.Instance.mRoleData.stageId;
    public List<int> lstMonsterID { get; private set; } = new List<int>();
    public bool IsMonsterBoss { get; private set; } = false;
    public int MonsterBossTime { get; private set; }
    public CTimer timerMonsterBoss { get; private set; } = new CTimer();
    public bool fightLose { get; private set; } = false;
    public int MaxGuanKaMonsterIndex { get; set; }
    public ulong InvincibleEndTime { get; set; }
    public int LocakHeroId { get; set; }

    private bool isSkipToBoss;
    private ConfigChapterUnit chapterUnit;

    private List<MapPetObject> _deletePetList = new List<MapPetObject>();
    //怪物掉落
    private Dictionary<int, List<MonsterGroup>> _monsterGoldDict = new Dictionary<int, List<MonsterGroup>>();

    /// <summary>
    /// 开始战斗标识
    /// </summary>
    public bool battleSign { get; set; } = false;
    
    public void OnInit()
    {
        InitWorldEnvironment();
        _common1001 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(1001);
        _common18 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(18);
        _common19 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(19);
        _battlePauseTimer = float.Parse(_common19.Param1);
        _totalRecoverTimer = int.Parse(_common1001.Param1);
        EventDispatcher.GameWorld.Regist<PetItemInfo>(EventDefine.EVENT_UPDATE_PET_INFO, this.UpdatePetInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_PET_LIST, this.UpdateBattlePetList);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HERO_Attr, this.UpdateBattleHeroAttr);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS, this.ChangeToNextStage);
    }

    public override void Dispose()
    {
        CleanUp();
        if (this.transGame != null)
        {
            MapObjectManager.Instance.Pool.Clear();
            Pool = null;
            Object.Destroy(this.transGame.gameObject);
        }
        EventDispatcher.GameWorld.UnRegist<PetItemInfo>(EventDefine.EVENT_UPDATE_PET_INFO, this.UpdatePetInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_PET_LIST, this.UpdateBattlePetList);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HERO_Attr, this.UpdateBattleHeroAttr);
        
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS, this.ChangeToNextStage);
        base.Dispose();
    }

    private void UpdateBattleHeroAttr()
    {
        if (GetLocalHero() != null)
        {
            GetLocalHero().UpdateHeroTotalAttr();
            GetLocalHero().UpdateHpBar();
        }
    }

    private void UpdatePetInfo(PetItemInfo curPet)
    {
        if(curPet.BattleIndex == -1) return;
        FindPetByBattleIndex(curPet);
        UpdatePetListState();
    }

    private void UpdatePetListState()
    {
        if (GuanKaStep == EN_GUANKA_STEP.FIGHTING)
        {
            foreach (var item in lstPetObj)
            {
                item.ClearMovePath();
                item.EndCurSkill();
                if (!DungeonMapManager.Instance.IsInCopy)  // 上下阵宠物 切换时 需要宠物立即战斗，副本不需要没有切换宠物选项
                {
                    item.IsAutoAttack = true;
                }
                var position = Vector3.zero;
                if (GetLocalHero() != null)
                {
                    position = new Vector3(0 + GetLocalHero().Position.x - FrontX2 + petPosX[item.PetAttr.PetVo.BattleIndex], petPosY[item.PetAttr.PetVo.BattleIndex], 0);
                }else
                {
                    var heroPos = new Vector3(0 + 540 * Math.Max(GuanKaMonsterIndex, 0), HERO_Y, 0);
                    position = new Vector3(0 + heroPos.x - FrontX2 + petPosX[item.PetAttr.PetVo.BattleIndex], petPosY[item.PetAttr.PetVo.BattleIndex], 0);
                }
                item.UpdatePetTotalAttribute();
                item.SetPosition(position, true);
            }
        }
        else if(GuanKaStep == EN_GUANKA_STEP.SHOW)
        {
            for (int i = 0; i < lstPetObj.Count; i++)
            {
                MapPetObject t = lstPetObj[i];
                var lstPos = new List<Vector3>();
                lstPos.Add(t.Position);
                lstPos.Add(new Vector3(petPosX[t.PetAttr.PetVo.BattleIndex] + 540 * GuanKaMonsterIndex, petPosY[t.PetAttr.PetVo.BattleIndex], 0));
                t.UpdateMoveByPath(lstPos, null, null);
                t.UpdatePetTotalAttribute();
            }
        }
    }
    private void UpdateBattlePetList()
    {
        _deletePetList.Clear();
        List<PetItemInfo> petInfoList = PetInfoManager.Instance.GetBattlePetList();

        foreach (var item in lstPetObj)
        {
            FindRemovePetByBattleIndex(item);
        }
        
        foreach (var t in _deletePetList)
        {
            lstPetObj.Remove(t);
            MapObjectManager.Instance.DestroyMapObjectById(t.PetAttr.unitID);
        }
        
        foreach (var item in petInfoList)
        {
            FindPetByBattleIndex(item);
        }

        UpdatePetListState();
        
    }
    private void FindRemovePetByBattleIndex(MapPetObject mapPetObject)
    {
        List<PetItemInfo> petInfoList = PetInfoManager.Instance.GetBattlePetList();
        bool isHas = false;
        foreach (var item in petInfoList)
        {
            if (item.BattleIndex == mapPetObject.PetAttr.PetVo.BattleIndex)
            {
                isHas = true;
                if (item.petCfg.Id == mapPetObject.PetAttr.ObjectTypeID)
                {
                    break;
                }

                if (item.petCfg.Id != mapPetObject.PetAttr.ObjectTypeID)
                {
                    _deletePetList.Add(mapPetObject);
                    break;
                }
            }
        }

        if (!isHas)
        {
            _deletePetList.Add(mapPetObject);
        }
    }
    private void FindPetByBattleIndex(PetItemInfo petItem)
    {
        if(petItem.BattleIndex == -1) return;
        bool isHas = false;
        for (int i = lstPetObj.Count-1; i >=0; i--)
        {
            if (petItem.BattleIndex == lstPetObj[i].PetAttr.PetVo.BattleIndex && petItem.petCfg.Id == lstPetObj[i].PetAttr.ObjectTypeID)
            {
                isHas = true;
                break;
            }
            if (petItem.BattleIndex == lstPetObj[i].PetAttr.PetVo.BattleIndex && petItem.petCfg.Id != lstPetObj[i].PetAttr.ObjectTypeID)
            {
                Vector3 pos = lstPetObj[i].Position;
                lstPetObj[i].PetAttr.PetVo.generalsType = petItem.petCfg.Id;
                lstPetObj[i].PetAttr.UpdatePet(lstPetObj[i].PetAttr.PetVo);
                lstPetObj[i].PetAttr.PetVo.BattleIndex = petItem.BattleIndex;
                lstPetObj[i].PetAttr.PetVo.pos = pos;
                UpdatePet(lstPetObj[i].PetAttr.PetVo, petItem);
                isHas = true;
                break;
            }
        }

        if (!isHas)//新增的
        {
            PetVo pet = new PetVo();
            pet.unitID = 100 + petItem.BattleIndex;
            pet.generalsType = petItem.petCfg.Id;
            pet.BattleIndex = petItem.BattleIndex;
            if (GetLocalHero() != null)
            {
                pet.pos = new Vector3(0 + GetLocalHero().Position.x - FrontX2 + petPosX[petItem.BattleIndex], petPosY[petItem.BattleIndex], 0);
            }else
            {
                var heroPos = new Vector3(0 + 540 * Math.Max(GuanKaMonsterIndex, 0), HERO_Y, 0);
                pet.pos = new Vector3(0 + heroPos.x - FrontX2 + petPosX[petItem.BattleIndex], petPosY[petItem.BattleIndex], 0);
            }

            pet.rot = Quaternion.Euler(0, 90, 0);
            MapObjectManager.Instance.UpdatePet(pet, petItem);
        }
    }

    /// <summary>
    /// 获取上阵宠物 通过guid
    /// </summary>
    public MapPetObject GetMapPetObjectByGuid(ulong guid)
    {
        for (int i = lstPetObj.Count - 1; i >= 0; i--)
        {
            if (guid == lstPetObj[i].PetAttr.petItemInfo.PetGuid)
            {
                return lstPetObj[i];
            }
        }

        return null;
    }

    public void InitGuanKaFSM(int preBoCi = 0)
    {
        ClearBattleScreen(preBoCi);

        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_NoneInCamp || DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_None)
        {
            //只显示地图，后台不挂机
            LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            lobbyView?.OpenBottomMapPanel();
            return;
        }
        
        GuanKaStep = EN_GUANKA_STEP.INIT;
        GuanKaMonsterIndex = preBoCi;
        MonsterUnitId = 1000;
        EnterState(GuanKaStep);
        var builder = Stage_Begin_CS.CreateBuilder();
        builder.StartStageId = (ulong)DataManager.Instance.mRoleData.stageId;
        builder.StartChapterId = (ulong)DataManager.Instance.mRoleData.chapterId;
        builder.IsIdle = (eBattleStatus)DataManager.Instance.mRoleData.battleStatus;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Stage_Begin_CS, builder.Build());
    }

    /// <summary>
    /// 清除战斗场景
    /// </summary>
    /// <param name="preBoCi"></param>
    public void ClearBattleScreen(int preBoCi = 0)
    {
        if (MapObjectManager.Instance.isMeleeHero())
        {
            MapObjectManager.Instance.GetLocalHero().StopAllAnimation();
        }
        RoleManager.Instance.EndSkillProxyDict();
        // GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.MAP);
        // GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.DUNGEON);
        // GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.BATTLE);
        // 通关
        //UIManager.Instance.Toast("关卡" + GuanKaIndex);
        MapHeroObject heroObject = MapObjectManager.Instance.GetLocalHero();
        if (heroObject != null)
        {
            heroObject.EndCurSkill();
            heroObject.IsAutoAttack = false;
            heroObject.ResetBuff();
        }
        MapObjectManager.Instance.DestroyMapObjectById(1);
        foreach (var pet in lstPetObj)
        {
            pet.EndCurSkill();
            pet.IsAutoAttack = false;
            MapObjectManager.Instance.DestroyMapObjectById(pet.Attr.unitID);
        }
        lstPetObj.Clear();
        for (int i = lstMonsterID.Count-1; i >=0; i--)
        {
            int monsterId = lstMonsterID[i];
            MapObjectManager.Instance.DestroyMapObjectById(monsterId);
            lstMonsterID.Remove(monsterId);
        }
        isSkipToBoss = preBoCi > 0;
        
        timerShow1.Clear();
        timerShow2.Clear();
        timerMonsterBoss.Clear();
        timreFightResult.Clear();
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
        // Debug.LogErrorFormat("==========================================================InitHeroAndPets=============================================================");
        if (GetLocalHero() == null)
        {
            PetInfoManager.Instance.GetUpLoadPet(true);
            // 初始形态，刷英雄，刷第一波怪
            var hero = new HeroVo();
            hero.unitID = 1;
            hero.generalsType = HeroInfoManager.Instance.GetMyHero().HeroUnit.Id;
            hero.pos = new Vector3(0 + 540 * Math.Max(GuanKaMonsterIndex, 0), HERO_Y, 0);
            hero.rot = Quaternion.Euler(0, 90, 0);
            MapObjectManager.Instance.UpdateHero(hero);
            LocakHeroId = hero.unitID;
            List<PetItemInfo> petInfoList = PetInfoManager.Instance.GetBattlePetList();
            for (int i=petInfoList.Count-1; i >=0; i--)
            {
                PetVo pet = new PetVo();
                pet.unitID = 100 + i;
                pet.generalsType = petInfoList[i].petCfg.Id;
                pet.BattleIndex = petInfoList[i].BattleIndex;
                var heroPos = new Vector3(0 + 540 * Math.Max(GuanKaMonsterIndex, 0), HERO_Y, 0);
                pet.pos = new Vector3(0 + heroPos.x - FrontX2 + petPosX[petInfoList[i].BattleIndex], petPosY[petInfoList[i].BattleIndex], 0);
                pet.rot = Quaternion.Euler(0, 90, 0);
                MapObjectManager.Instance.UpdatePet(pet, petInfoList[i]);
                for (int j = lstPetObj.Count-1; j >=0; j--)
                {
                    lstPetObj[j]?.UpdatePetTotalAttribute();
                }
            }

            LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            lobbyView?.UpdateHeroSkillList();
        }
        else
        {
            GetLocalHero().UpdateHeroTotalAttr();
            GetLocalHero().UpdateHpBar();
        }
        
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_Refresh_xpskill);

    }

    protected void EnterState(EN_GUANKA_STEP state)
    {
        switch (state)
        {
            case EN_GUANKA_STEP.INIT:  //TODO 初始化地图和场景位置  英雄宠物位置  生产怪物
                {
                    // lstMonsterID.Clear();
                    fightLose = false;

                    chapterUnit = ConfigUtils.GetChapterUnitById(ChapterIndex);
                    if (chapterUnit != null)
                    {
                        var stageId = GuanKaStageId;
                        List<ConfigStageUnit> stageUnits = ConfigUtils.GetStageUnitById(stageId);
                        EventDispatcher.GameWorld.DispatchEvent(EventDefine.STAGE_DATA_INFO, stageUnits, GuanKaMonsterIndex);
                        var battleScene = GetMapObjectSceneRootTrans();

                        if (battleScene != null)
                        {
                            battleScene.x = Mathf.Max(GuanKaMonsterIndex, 0) * -540;                     
                            var mapList = battleScene.GetChild("map").asCom.GetChild("mapList").asList;
                            mapList.itemRenderer = (index, item) =>
                            {
                                // int mapIndex = (index) % 3 + 1;
                                int mapIndex = (index) % 2 + 1;
                                ((UI_MapLoadItem) item).mapLoader.fixMode = true;
                                // ((UI_MapLoadItem)item).mapLoader.url = UIResource.GetMapItemByMapId(chapterUnit.Scenes,mapIndex);
                                ((UI_MapLoadItem)item).mapLoader.url = UIResource.GetMapItemByMapId(GetScene(chapterUnit.Scenes),1);
                            };
                            mapList.numItems = stageUnits.Count;//多一块地图，防止穿帮
                            mapList.ResizeToFit();
                        }
                        
                        var battleBg = GetMapObjectBGRootTrans();

                        if (battleBg != null)
                        {
                         
                            battleBg.x = 0;

                            // var map_bg0 = battleBg.GetChild("map_bg0").asLoader;
                            // map_bg0.fixMode = true;
                            // map_bg0.icon = UIResource.GetMapBgColorByMapId(1001);
                            // var map_bg = battleBg.GetChild("map_bg").asLoader;
                            // map_bg.fixMode = true;
                            // map_bg.icon = UIResource.GetMapBgByMapId(chapterUnit.Scenes);
                            // var map_middle = battleBg.GetChild("map_middle").asLoader;
                            // map_middle.fixMode = true;
                            // map_middle.icon =  UIResource.GetMapMiddleByMapId(chapterUnit.Scenes);

                            var mapList2 = battleBg.GetChild("mapList2").asList;
                            mapList2.itemRenderer = (index, item) =>
                            {
                                int mapIndex = (index) % 3 + 1;
                                ((UI_MapLoadItem2) item).bg2.fixMode = true;
                                ((UI_MapLoadItem2)item).bg2.url = UIResource.GetMapMiddleByMapId(GetScene(chapterUnit.Scenes));
                            };

                            mapList2.numItems = stageUnits.Count;

                            // var mapList3 = battleBg.GetChild("mapList3").asList;
                            // mapList3.itemRenderer = (index, item) =>
                            // {
                            //     int mapIndex = (index) % 2 + 1;
                            //     ((UI_MapLoadItem3) item).bg3.fixMode = true;
                            //     ((UI_MapLoadItem3)item).bg3.url = UIResource.GetMapBgColorByMapId(chapterUnit.Scenes);
                            // };
                            var cloud0 = battleBg.GetChild("cloud0").asLoader;
                            cloud0.url = UIResource.GetMapBgColorByMapId(GetScene(chapterUnit.Scenes));
                            var cloud1 = battleBg.GetChild("cloud1").asLoader;
                            cloud1.url = UIResource.GetMapBgColorByMapId(GetScene(chapterUnit.Scenes));
                        }

                        InitHeroAndPets();
                        
                        SpawnMonster(stageUnits, 0.01f);
                    }
                    
                }
                break;

            case EN_GUANKA_STEP.SHOW:   //TODO 更新移动位置
                {
                    PlayCommonTimeSpine();
                    MapObjectManager.Instance.isHeroPlayXPSkill = false;
                    MapObjectManager.Instance.isHeroPlayNormalSkill = false;
                    var hero = MapObjectManager.Instance.GetLocalHero();
                    RoleManager.Instance.EndSkillProxyDict();
                    if (hero != null)
                    {
                        hero.SetSkeletonAnimationTimeScale(1);
                        hero.EndCurSkill();
                        var lstPos = new List<Vector3>();
                        lstPos.Add(hero.Position);
                        lstPos.Add(new Vector3(FrontX2 + 540 * GuanKaMonsterIndex, HERO_Y, 0));
                        var lstRot = new List<Quaternion>();
                        hero.UpdateMoveByPath(lstPos, null, null);
                    }

                    for (int i = 0; i < lstPetObj.Count; i++)
                    {
                        MapPetObject t = lstPetObj[i];
                        t.EndCurSkill();
                        var lstPos = new List<Vector3>();
                        lstPos.Add(t.Position);
                        lstPos.Add(new Vector3(petPosX[t.PetAttr.PetVo.BattleIndex] + 540 * GuanKaMonsterIndex, petPosY[t.PetAttr.PetVo.BattleIndex], 0));
                        t.UpdateMoveByPath(lstPos, null, null);
                    }

                    foreach (var monsterID in lstMonsterID)
                    {
                        var monster = MapObjectManager.Instance.GetMonsterObjectById(monsterID);

                        if (monster != null)
                        {
                            var lstPos = new List<Vector3>();
                            lstPos.Add(monster.Position);
                            lstPos.Add(new Vector3(monster.Position.x - 100,  monster.Position.y));
                            monster.UpdateMoveByPath(lstPos, null, null);
                        }
                    }
                    
                    timerShow1.Clear();
                    timerShow2.Clear();
                    timerShow1.Startup(0.5f/Speed, true);
                    battleSign = false;
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.MONSTER_WAVE_DATA_INFO,GuanKaMonsterIndex);
                break;

            case EN_GUANKA_STEP.FIGHTING:
                {
                    StopCommonTimeSpine();
                    if(fightLose) return;

                    GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP,1,.2f, () =>
                    {
                        var hero = MapObjectManager.Instance.GetLocalHero();

                        if (hero != null)
                        {
                            hero.UpdateHeroTotalAttr();
                            // hero.ClearMovePath();
                            hero.EndCurSkill();
                            hero.IsAutoAttack = true;
                        }

                        foreach (var t in lstPetObj)
                        {
                            t.UpdatePetTotalAttribute();
                            // t.ClearMovePath();
                            t.EndCurSkill();
                            t.IsAutoAttack = true;
                        }

                        timreFightResult.Clear();
                        timreFightResult.Startup(1f/Speed, true);

                        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP, 1, _battlePauseTimer, () =>
                        {
                            battleSign = true;
                        });
                        
                        if (IsMonsterBoss)
                        {
                            isSkipToBoss = false;
                            timerMonsterBoss.Startup(MonsterBossTime);
                        }
                        EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_MONSTER_BOSS_INFO);
                    });

                }
                break;
            
             case EN_GUANKA_STEP.RESULT:
                {
                    if (fightLose)
                    {
                        foreach (var t in lstPetObj)
                        {
                            t.ClearMovePath();
                            t.EndCurSkill();
                        }

                        if (IsMonsterBoss)
                        {
                            MaxGuanKaMonsterIndex = GuanKaMonsterIndex;
                            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.STAGE_FIGHT_LOSE, IsMonsterBoss);
                        }
                        UI_BossFightWindow fightBoss = GetMapObjectRootTrans().GetChild("fightBoss") as UI_BossFightWindow;
                        fightBoss.visible = false;
                        // GLoader fightBossBg = GetMapObjectRootTrans().GetChild("fightBossBg") as GLoader;
                        // fightBossBg.visible = false;
                        
                        GuanKaMonsterIndex = 0;
                        PlayCommonTimeSpine(true);
                        // if (!VillageInfoManager.Instance.IsInVillageHome)
                        //     GameManager.Instance.SoundManager.PlayEffectWithoutLoop(10);
                        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP, 11,1.5f/Speed, () =>
                        {
                            StopCommonTimeSpine();
                            if (UIManager.Instance.IsTopController("Lobby"))
                            {
                                UIManager.Instance.ShowUIPanel("FightLose");
                            }
          
                        });

                        if(!IsMonsterBoss)
                        {
                            GuanKaMonsterIndex = 0;
                            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP, 11, 1.5f / Speed, () =>
                            {
                                //TODO 回退到上一个关卡 服务器控制 是退回上个关卡，或只弹地图后台不挂机
                                var builder = Stage_Failed_CS.CreateBuilder();
                                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Stage_Failed_CS, builder.Build());
                            });
                            IsMonsterBoss = false;
                            return;
                        }
                        
                        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP,3,1.8f/Speed, () =>
                        {
                            InitGuanKaFSM();
                        });
                        return;
                    }
                    if(!IsMonsterBoss)
                        // 准备下一波怪物
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

        // if (hero != null)
        // {
        //     // 恢复生命
        //     hero.PlayRecovery();
        // }
        // 准备下一波怪物
        ++GuanKaMonsterIndex;
        if (MaxGuanKaMonsterIndex != 0 && GuanKaMonsterIndex >= MaxGuanKaMonsterIndex)
        {
            InitGuanKaFSM();
            return;
        }
        fightLose = false;
        IsMonsterBoss = false;
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_MONSTER_BOSS_INFO);

        if (hero != null)
        {
            hero.ClearMovePath();
            hero.IsAutoAttack = false;
            hero.EndCurSkill();
        }

        InitHeroAndPets();
        foreach (var t in lstPetObj)
        {
            t.ClearMovePath();
            t.IsAutoAttack = false;
            t.EndCurSkill();
        }
        //new Vector3(540 + 540 * GuanKaMonsterIndex + ConstDefine.DEFAULT_MOVE_SPEED * 4.0f, 280 + 60 * i, 0);
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP,4,0.2f, () =>
        {
            var stageId = GuanKaStageId;
            List<ConfigStageUnit> stageUnits = ConfigUtils.GetStageUnitById(stageId);
            SpawnMonster(stageUnits, 0.10f);
        });
    }

    private void ChangeToNextStage()
    {
        MaxGuanKaMonsterIndex = 0;
        GuanKaMonsterIndex = 0;
        // if(!_isStopMapBattle) return;
        if(DungeonMapManager.Instance.IsInCopy) return;
        InitGuanKaFSM();
        MapObjectManager.Instance.StageComplete = false;
    }

    private void SpawnMonster(List<ConfigStageUnit> stageUnitList, float timer)
    {
        lstMonsterID.Clear();
        if (GuanKaMonsterIndex < stageUnitList.Count)
        {
            //发送当前节点位置
            var builder = ReachNodeInStageChallenge_CS.CreateBuilder();
            builder.NodeId = (uint)stageUnitList[GuanKaMonsterIndex].Node;
            Debug.Log("============ReachNodeInStageChallenge_CS===builder.NodeId="+builder.NodeId);
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ReachNodeInStageChallenge_CS, builder.Build()); 
            
            int groupId = stageUnitList[GuanKaMonsterIndex].MonsterData;  
            var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
            int bossId = 0;
            
            ConfigCommonUnit common = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2);//2=获取怪物出生随机位置
            var margeNum = (int.Parse(common.Param4) - int.Parse(common.Param3)) / monsterGroupArr.Count;
            for (int i = 0; i < monsterGroupArr.Count; i++)
            {
                var monsterGroup = monsterGroupArr[i];
                if (monsterGroup != null)
                {
                    var isBossArr = monsterGroup.IsBoss.Split(",");
                    if (isBossArr.Length == 2)
                    {
                        IsMonsterBoss = isBossArr[0].Equals("1");
                        MonsterBossTime = int.Parse(isBossArr[1]);
                        bossId = monsterGroup.MonsterId;
                    }
                    else
                    {
                        IsMonsterBoss = false;
                        MonsterBossTime = 0;
                    }
                    for (int j = 0; j < monsterGroup.Num; j++)
                    {
                        var monster = new MonsterVo();
                        monster.generalsType = monsterGroup.MonsterId;
                        monster.unitID = MonsterUnitId ++;
                        
                        if (!IsMonsterBoss)
                        {    // 怪物的 Y 轴 位置先写死
                            // monster.pos = new Vector3(540 + 540 * GuanKaMonsterIndex + Random.Range(int.Parse(common.Param1), int.Parse(common.Param2)), Random.Range(int.Parse(common.Param3),int.Parse(common.Param4)));
                            monster.pos = new Vector3(495 + 540 * GuanKaMonsterIndex + Random.Range(int.Parse(common.Param1), int.Parse(common.Param2)), Random.Range( int.Parse(common.Param3) + margeNum * i , int.Parse(common.Param3) + margeNum * (i+1) ) );
                        }
                        else
                        {
                            monster.pos = new Vector3(435 + 540 * GuanKaMonsterIndex +   Random.Range(int.Parse(common.Param1), int.Parse(common.Param2)),  HERO_Y);
                        }
                        monster.rot = Quaternion.Euler(0, 270, 0);
                        monster.IsBoss = IsMonsterBoss;
                        monster.GroupId = groupId;
                        monster.MonsterIndex = j;
                        monster.MonsterGold = GetMonsterGold(stageUnitList[GuanKaMonsterIndex].LevelId,groupId, j, monsterGroup.MonsterId);
                        MapMonsterObject monsterObject = MapObjectManager.Instance.UpdateMonster(monster);
                        //怪物关卡加成
                        monsterObject.AddStageAttr(stageUnitList[GuanKaMonsterIndex], monsterGroup.MonsterId);
                        monsterObject.UpdateObject();
                        monsterObject.UpdateHpBar();
                        
                        lstMonsterID.Add(monster.unitID);
                    }
                    
                }
                else
                {
                    IsMonsterBoss = false;
                    timerMonsterBoss.Clear();
                }
            }
            
            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP,15,0.1f, () =>
            {
                ChangeState(EN_GUANKA_STEP.SHOW);
            }); 
            
            if(IsMonsterBoss)
            {
                UI_BossFightWindow fightBoss = GetMapObjectRootTrans().GetChild("fightBoss") as UI_BossFightWindow;
                fightBoss.visible = true;
                // GLoader fightBossBg = GetMapObjectRootTrans().GetChild("fightBossBg") as GLoader;
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
              
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP,5,2f, () =>
                {
                    fightBoss.visible = false;
                    // fightBossBg.visible = false;
                });

            }

        }
        else
        {
            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP,9,timer + 0.5f, () =>
            {
                ChangeState(EN_GUANKA_STEP.SHOW);
            });
        }
        
        if(MaxGuanKaMonsterIndex == stageUnitList.Count - 1)
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STAGE_FIGHT_LOSE, true);
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
                        if (_isCopyToBattle)
                        {
                            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.MAP,11,0.5f, () =>
                            {
                                _isCopyToBattle = false;
                                isSkipToBoss = false;
                                ChangeState(EN_GUANKA_STEP.FIGHTING);
                            });
                        }
                        else if (GuanKaMonsterIndex == 0)
                        {
                           ChangeState(EN_GUANKA_STEP.FIGHTING);
                        }
                        else
                        {
                            timerShow2.Startup(1.5f/Speed, true);
                        }
                    }

                    if (timerShow2.IsActive())  //TODO 背景 场景 移动
                    {
                        if (!isSkipToBoss)
                        {
                            // 主节点晚1s移动，一共移动3s，根据主角速度移动
                            var battleScene = GetMapObjectSceneRootTrans();

                            if (battleScene != null)
                            {
                                battleScene.x = Mathf.Max(GuanKaMonsterIndex - 1, 0) * -540 + timerShow2.GetPassPrecent() * -540;
                            }
                        
                            var battleBg = GetMapObjectBGRootTrans();
                        
                            if (battleBg != null)
                            {
                                battleBg.x = Mathf.Max(GuanKaMonsterIndex - 1, 0) * -270 + timerShow2.GetPassPrecent() * -270;
                            }
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
                        if (!IsExistHero())
                        {
                            fightLose = true;
                            ChangeState(EN_GUANKA_STEP.RESULT);
                        }
                        else if (!IsExistMonster())
                        {
                            fightLose = false;
                            if (IsMonsterBoss)
                            {
                                MaxGuanKaMonsterIndex = 0;
                                isSkipToBoss = false;
                            }
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

    public void SetMonsterGold(int stageId, List<MonsterGroup> monsterGroups)
    {
        _monsterGoldDict.Clear();
        _monsterGoldDict.Add(stageId, monsterGroups);
    }

    private double GetMonsterGold(int stageId,int groupId, int monsterIndex, int monsterId)
    {
        if (_monsterGoldDict.TryGetValue(stageId, out var monsterGroups))
        {
            foreach (var monsterGroup in monsterGroups)
            {
                if (monsterGroup.GroupId == groupId)
                {
                    foreach (var monsterAward in monsterGroup.MonsterAwardsList)
                    {
                        if (monsterAward.MonsterId == monsterId && monsterAward.MonsterIdIndex == monsterIndex)
                        {
                            return (double) monsterAward.AwardGold;
                        }
                    }
                }
            } 
        }

        return 0;
    }

    /// <summary>
    /// 是否近战英雄
    /// </summary>
    /// <returns></returns>
    public bool isMeleeHero()
    {                                                                                                                                   // 先写死只有白狼是近战 其它战士都不是
        if (MapObjectManager.Instance.GetLocalHero() != null && MapObjectManager.Instance.GetLocalHero().HeroAttr.HeroUnit.Vocation == 1 && MapObjectManager.Instance.GetLocalHero().HeroAttr.HeroUnit.Id == 20046) //近战英雄
        {
            return true;
        }
        return false;
    }

    public int GetScene(int sid)
    {
        if (sid > 3) { return 1; }
        return sid;
    }
}
