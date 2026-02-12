
using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Engine;
using EngineBase;
using msg;
using UnityEngine;
using Enum = System.Enum;
using Random = UnityEngine.Random;

public class CityInfo
{
    public VillageBuildType BuildType;
    /// <summary>
    /// true=解锁 false=未解锁
    /// </summary>
    public bool IsUnLock;

    public ulong StartProduceTime;
    public ulong NextProduceTime;
    public List<ulong> PetIdList = new List<ulong>(3);

    public double ItemNum;
}

public enum VillageBuildType
{
    // 1=粮食工坊
    // 2=加工厂
    // 3=探索营地
    // 4=窝棚
    // 5=训练场
    // 6=石头矿区
    None,
    FoodWorkshop = 1,
    Factory = 2,
    Explore = 3,
    Shack = 4,
    Training = 5,
    Stone = 6,
}

public class VillageInfoManager : TSingleton<VillageInfoManager>
{
    public bool IsInVillageHome { get; set; }
    
    public readonly int BUILD_PET_COUNT = 3;
    public bool IsGetServer { get; set; } = false;
    private Dictionary<VillageBuildType, CityInfo> _cityInfos = new Dictionary<VillageBuildType, CityInfo>();

    public void SetBuildInfo(VillageBuildType buildType, CityInfo cityInfo)
    {
        if (_cityInfos.ContainsKey(buildType))
        {
            _cityInfos[buildType] = cityInfo;
        }
        else
        {
            _cityInfos.Add(buildType, cityInfo);
        }
        
    }

    public CityInfo GetBuildInfoByType(VillageBuildType type)
    {
        if (_cityInfos.TryGetValue(type, out CityInfo cityInfo))
        {
            return cityInfo;
        }

        ConfigBuildUnit buildUnit = ConfigUtils.GetBuildUnitByType(type);
        CityInfo city = new CityInfo();
        city.IsUnLock = false;
        city.BuildType = type;
        city.StartProduceTime = 0;
        city.NextProduceTime = 0;
        SetBuildInfo(type, city);
        return city;
    }

    public List<PetItemInfo> GetAllIdleVillagePet()
    {
        List<PetItemInfo> villagePetInfos = new List<PetItemInfo>();
        List<PetItemInfo> allList = PetInfoManager.Instance.GetAllHavePetList();
        foreach (var item in allList)
        {
            villagePetInfos.Add(item);
        }

        villagePetInfos.Sort((a, b) =>
        {
            int aD = a.DispatchBuild != VillageBuildType.None ? 1 : 0;
            int bD = b.DispatchBuild != VillageBuildType.None ? 1 : 0;
            int result = aD > bD ? 1 : (aD == bD ? 0 : -1);
            if(result == 0)
                result = a.petCfg.Quality > b.petCfg.Quality ? -1 : (a.petCfg.Quality == b.petCfg.Quality ? 0 : 1);
            if (result == 0)
                result = a.petCfg.Id > b.petCfg.Id ? -1 : 1;
            return result;
        });
        return villagePetInfos;
    }

    public bool HasIdlePet()
    {
        List<PetItemInfo> allList = PetInfoManager.Instance.GetAllHavePetList();
        for (int i = 0; i < allList.Count; i++)
        {
            if (allList[i].DispatchBuild == (int) eBuildType.eBuildType_None)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// *100算的
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public int GetVillagePetAdd(int quality)
    {
        int commonId = 600001 + quality;
        ConfigCommonUnit common = ConfigDataGroup.GetInstance<ConfigCommon>().Get(commonId);
        return Mathf.RoundToInt(int.Parse(common.Param1) * ConstDefine.CONFIG_PLACE);
    }
    
    public List<PetItemInfo> GetVillagePetByBuild(VillageBuildType buildType)
    {
        List<PetItemInfo> allList = PetInfoManager.Instance.GetAllHavePetList();
        List<PetItemInfo> petItemInfos = new List<PetItemInfo>();
        for (int i = 0; i < allList.Count; i++)
        {
            if (allList[i].DispatchBuild ==  buildType)
            {
                petItemInfos.Add(allList[i]);
            }
        }
        return petItemInfos;
    }

    public void SetVillagePetByBuild(PetItemInfo petItemInfo, VillageBuildType buildType)
    {
        List<PetItemInfo> allList = PetInfoManager.Instance.GetAllHavePetList();
        for (int i = 0; i < allList.Count; i++)
        {
            if (allList[i].PetGuid ==  petItemInfo.PetGuid)
            {
                allList[i].BuildInnerIndex = -1;
                allList[i].DispatchBuild = buildType;
                break;
            }
        }
    }
    
    public int GetNoUploadIndex(VillageBuildType buildType)
    {
       CityInfo cityInfo = GetBuildInfoByType(buildType);
       for (int i = 0; i < cityInfo.PetIdList.Count; i++)
       {
           if (cityInfo.PetIdList[i] == 0)
               return i;
       }
       return -1;
    }

    /// <summary>
    /// 获取对应建筑类型宠物栏位为空的数量
    /// </summary>
    /// <param name="buildType"></param>
    /// <returns></returns>
    public int GetNoUploadCountByBuild(VillageBuildType buildType)
    {
        int count = 0;
        CityInfo cityInfo = GetBuildInfoByType(buildType);
        for (int i = 0; i < cityInfo.PetIdList.Count; i++)
        {
            if (cityInfo.PetIdList[i] == 0)
                count++;
        }
        return count;
    }

    public FuncOpenType GetFuncIdByBuildType(VillageBuildType buildType)
    {
        FuncOpenType funcId = 0;
        switch (buildType)
        {
            case VillageBuildType.Explore:
                funcId = FuncOpenType.BuildExplore;
                break;
            case VillageBuildType.Factory:
                funcId = FuncOpenType.BuildFactory;
                break;
            case VillageBuildType.Shack:
                funcId = FuncOpenType.BuildPet;
                break;
            case VillageBuildType.Stone:
                funcId = FuncOpenType.BuildStone;
                break;
            case VillageBuildType.Training:
                funcId = FuncOpenType.BuildTrain;
                break;
            case VillageBuildType.FoodWorkshop:
                funcId = FuncOpenType.BuildFood;
                break;
        }

        return funcId;
    }

    public bool RewardLimit(VillageBuildType buildType)
    {
        foreach (var item in _cityInfos)
        {
            ConfigBuildUnit buildUnit = ConfigUtils.GetBuildUnitByType(item.Value.BuildType);
            if (item.Value.BuildType == buildType)
            {
                if ((item.Value.StartProduceTime + (ulong) buildUnit.PeakTime)  <= ServerTimeManager.Instance.CurServerTime)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool VillageRedDot()
    {
        bool isRedDot = false;
        foreach (var item in _cityInfos)
        {
            if (FuncPreviewManger.Instance.GetFuncOpenState(GetFuncIdByBuildType(item.Key)).Item1)
            {
                isRedDot = RewardLimit(item.Value.BuildType);
                if (isRedDot)
                    return true;
                isRedDot = DispatchPetToProduct(item.Value.BuildType);
                if (isRedDot)
                    return true;
            }

        }

        return false;
    }

    private bool DispatchPetToProduct(VillageBuildType buildType)
    {
        foreach (var item in _cityInfos)
        {
            if (item.Value.BuildType == buildType)
            {
                if (item.Value.PetIdList.Contains(0) && HasIdlePet())
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public bool BuildRedDot(VillageBuildType buildType)
    {
        bool isRedDot = false;
      
        isRedDot = RewardLimit(buildType);
        if (isRedDot)
            return true;
        isRedDot = DispatchPetToProduct(buildType);
        if (isRedDot)
            return true;

        return false;
    }
    
}
