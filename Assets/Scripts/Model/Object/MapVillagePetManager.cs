using System;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using EngineBase;
using FairyGUI;

public class MapVillagePetManager : TSingleton<MapVillagePetManager>
{

    /// <summary>
    /// 延迟删除对象
    /// </summary>
    private bool IsInForeachObjectDoing { get; set; }

    /// <summary>
    /// 延迟删除对象id
    /// </summary>
    private List<ulong> lstDelayDestroyObjectID = new List<ulong>();
    private List<ulong> _catchLstDelayDestroyObjectID = new List<ulong>();
    /// <summary>
    /// 动态物件: 真实物件
    /// </summary>
    private Dictionary<ulong, MapVillagePetObject> dictMapObject = new Dictionary<ulong, MapVillagePetObject>();
    
    private Dictionary<ulong, MapVillagePetObject> _dictCatchMapObject = new Dictionary<ulong, MapVillagePetObject>();
    
    public List<MapVillagePetObject> lstPetObj { get; private set; } = new List<MapVillagePetObject>();
    
    public List<MapVillagePetObject> catchLstPetObj { get; private set; } = new List<MapVillagePetObject>();
    #region 缓存策略

    protected Dictionary<int, Dictionary<string, GameObject>> dicPrefab =
        new Dictionary<int, Dictionary<string, GameObject>>();

    // 异步加载序号
    protected int m_nAsyncLoadIndex = 0;

    protected Dictionary<System.Type, Dictionary<string, List<MapVillagePetObject>>>
        dicActorPool = new Dictionary<System.Type, Dictionary<string, List<MapVillagePetObject>>>();

    #endregion

    #region 加载/缓存模块

    private GameObject GetCachePrefab(MapVillagePetObject obj)
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

    public void LoadGameObject(MapVillagePetObject obj)
    {
        GameObject prefab = GetCachePrefab(obj);

        if (prefab != null)
        {
            obj.SetGameObject(GameObject.Instantiate(prefab));
        }
        else
        {
            obj.asyncLoadIndex = m_nAsyncLoadIndex++;
 
            ModelManager.Instance.LoadNormalPrefab("Role/" + obj.Name, 
                (go) =>
                {
                    OnLoadModelDone(obj, go);
                });
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

    private MapVillagePetObject FindActor<T>(string name) where T : MapVillagePetObject
    {
        if (this.dicActorPool != null)
        {
            Dictionary<string, List<MapVillagePetObject>> dicActor = null;
            if (dicActorPool.TryGetValue(typeof(T), out dicActor))
            {
                List<MapVillagePetObject> list = null;
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

    public MapVillagePetObject SpawnActor<T>(string name) where T : MapVillagePetObject
    {
        MapVillagePetObject actor = this.FindActor<T>(name);
        if (actor == null)
        {
            actor = System.Activator.CreateInstance(typeof(T)) as MapVillagePetObject;
            actor.Name = name;
            actor.InitAttr();
        }

        return actor;
    }

    public void DestroyActor(MapVillagePetObject actor)
    {
        if (actor != null && !actor.Recycled)
        {
            actor.Destroyed();

            if (this.dicActorPool != null)
            {
                System.Type t = actor.GetType();
                Dictionary<string, List<MapVillagePetObject>> dicActor = null;
                if (!dicActorPool.TryGetValue(t, out dicActor))
                {
                    dicActor = new Dictionary<string, List<MapVillagePetObject>>();
                    dicActorPool.Add(t, dicActor);
                }

                List<MapVillagePetObject> list = null;
                if (!dicActor.TryGetValue(actor.Name, out list))
                {
                    list = new List<MapVillagePetObject>();
                    dicActor.Add(actor.Name, list);
                }

                list.Add(actor);
            }
        }
    }

    #endregion

    public void Tick(float deltaSeconds)
    {
        TickByMapObject(deltaSeconds);
    }
    
    public void LateTick(float deltaSeconds)    {
        LateTickByMapObject(deltaSeconds);
        LateTickByDelayDestroyMapObject();
    }
    
    public void TickByMapObject(float deltaSeconds)
    {
        IsInForeachObjectDoing = true;
        foreach (var item in dictMapObject)
        {
            item.Value.Tick(deltaSeconds);
        }
        
        foreach (var item in _dictCatchMapObject)
        {
            item.Value.Tick(deltaSeconds);
        }
        IsInForeachObjectDoing = false;
    }
    
    public void LateTickByMapObject(float deltaSeconds)
    {
        IsInForeachObjectDoing = true;
        foreach (var item in dictMapObject)
        {
            item.Value.LateTick(deltaSeconds);
        }
        foreach (var item in _dictCatchMapObject)
        {
            item.Value.LateTick(deltaSeconds);
        }
        
        IsInForeachObjectDoing = false;

        var root = GetMapObjectRootTrans(0);

        if (root != null)
        {
            var children = root.GetChildren();
            Array.Sort(children, (p1, p2) =>
            {
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
        
        var root1 = GetMapObjectRootTrans(1);

        if (root1 != null)
        {
            var children = root1.GetChildren();
            Array.Sort(children, (p1, p2) =>
            {
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
            root1.ChangeChildrenOrder(children);
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
                this.DestroyMapObjectById(item, 0);
            }
            lstDelayDestroyObjectID.Clear();
        }
        
        if (_catchLstDelayDestroyObjectID.Count > 0)
        {
            /// 强行设置为false防止死循环
            IsInForeachObjectDoing = false;
            foreach (var item in _catchLstDelayDestroyObjectID)
            {
                this.DestroyMapObjectById(item, 1);
            }
            _catchLstDelayDestroyObjectID.Clear();
        }
    }

    public GComponent GetMapObjectRootTrans(int villagePetType)
    {
        GComponent objRoot = null;
        if (villagePetType == 0)
        {
            VillageHomeView homeView = UIManager.Instance.FindByName("VillageHome") as VillageHomeView;
            objRoot  = homeView?.GetPetRoot();
        }

        return objRoot;
    }
    
    
    public MapVillagePetObject GetMapObjectById(ulong id)
    {
        MapVillagePetObject obj;
        if (dictMapObject.TryGetValue(id, out obj))
        {
            return obj;
        }
        return null;
    }

    public MapVillagePetObject GetCatchPetObjectById(ulong id)
    {
        MapVillagePetObject obj;
        if (_dictCatchMapObject.TryGetValue(id, out obj))
        {
            return obj;
        }
        return null;
    }
    
    private void AddMapObject(MapVillagePetObject obj)
    {
        if (obj == null)
        {
            return;
        }

        dictMapObject[obj.PetAttr.PetGuid] = obj;
    }
    
    private void AddCatchObject(MapVillagePetObject obj)
    {
        if (obj == null)
        {
            return;
        }

        _dictCatchMapObject[obj.PetAttr.PetGuid] = obj;
    }
    
    public void DestroyMapObjectById(ulong id, int villageType)
    {
        if(villageType == 0)
            DestroyMapObject(GetMapObjectById(id));
        else if (villageType == 1)
            DestroyCatchObject(GetCatchPetObjectById(id));
    }

    public void DestroyMapObject(MapVillagePetObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (IsInForeachObjectDoing)
        {
            // 遍历中不做删除操作
            if (!lstDelayDestroyObjectID.Contains(obj.PetAttr.PetGuid))
            {
                lstDelayDestroyObjectID.Add(obj.PetAttr.PetGuid);
            }
            return;
        }
        
        dictMapObject.Remove(obj.PetAttr.PetGuid);
        DestroyActor(obj);
    }
    
    public void DestroyCatchObject(MapVillagePetObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (IsInForeachObjectDoing)
        {
            // 遍历中不做删除操作
            if (!_catchLstDelayDestroyObjectID.Contains(obj.PetAttr.PetGuid))
            {
                _catchLstDelayDestroyObjectID.Add(obj.PetAttr.PetGuid);
            }
            return;
        }
        
        _dictCatchMapObject.Remove(obj.PetAttr.PetGuid);
        DestroyActor(obj);
    }
    
    public MapVillagePetObject GetPetObjectById(ulong id)
    {
        return GetMapObjectById(id) as MapVillagePetObject;
    }

    public MapVillagePetObject AddVillagePet(PetVo item, int posIndex, ulong petGuid)
    {
        if (item == null)
        {
            return null;
        }

        string strResName = ConfigUtils.GePetModelPathByID(item.generalsType);

        var obj = GetPetObjectById(petGuid);

        if (obj != null && obj.Name.Equals(strResName))
        {
            // 没有变化
            obj.PetAttr.Init(item.unitID, posIndex, petGuid, 0);
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

            obj = SpawnActor<MapVillagePetObject>(strResName) as MapVillagePetObject;

            if (obj != null)
            {
                obj.PetAttr.Init(item.unitID, posIndex, petGuid, 0);
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
        return obj;
    }
    
    public MapVillagePetObject AddCatchPet(PetVo item, ulong petGuid)
    {
        if (item == null)
        {
            return null;
        }

        string strResName = ConfigUtils.GePetModelPathByID(item.generalsType);

        var obj = GetCatchPetObjectById(petGuid);

        if (obj != null && obj.Name.Equals(strResName))
        {
            // 没有变化
            obj.PetAttr.Init(item.unitID, 0, petGuid, 1);
            obj.SetTransform(item.pos, item.rot, false);
            obj.UpdateObject();
        }
        else
        {
            // 不存在或者已失效，回收资源
            if (obj != null)
            {
                DestroyCatchObject(obj);
            }

            obj = SpawnActor<MapVillagePetObject>(strResName) as MapVillagePetObject;

            if (obj != null)
            {
                obj.PetAttr.Init(item.unitID, 0, petGuid, 1);
                obj.BeginPlay(item.unitID, "", item.pos, item.rot);

                AddCatchObject(obj);
            }
            else
            {
                LogUtils.LogErrorFormat("UpdatePet Create Error {0} {1}", item.unitID, "");
            }
        }

        if (obj != null)
        {
            // 设置为不回收
            obj.IsTryRecycleSign = true;
        }
        if(!catchLstPetObj.Contains(obj))
            catchLstPetObj.Add(obj);
        return obj;
    }
    
    public void CleanUp()
    {
        m_nAsyncLoadIndex = 0;
        
        foreach (var pet in lstPetObj)
        {
            pet.EndCurSkill();
            MapVillagePetManager.Instance.DestroyMapObject(pet);
        }
        lstPetObj.Clear();
        for (int i = catchLstPetObj.Count-1; i >=0; i--)
        {
            MapVillagePetManager.Instance.DestroyMapObject(catchLstPetObj[i]);
        }
        
        GetMapObjectRootTrans(0)?.RemoveChildren();
        GetMapObjectRootTrans(1)?.RemoveChildren();

        IsInForeachObjectDoing = true;
        foreach (var item in dictMapObject)
        {
            item.Value.Dispose();
        }
        
    
        foreach (var item in dicPrefab)
        {
            item.Value.Clear();
        }
        dicPrefab.Clear();
        
        IsInForeachObjectDoing = false;

        dictMapObject.Clear();
        dicActorPool.Clear();

        lstDelayDestroyObjectID.Clear();
        
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    public override void Dispose()
    {
        CleanUp();
        base.Dispose();
    }
}
