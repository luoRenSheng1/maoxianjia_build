
using System.Collections.Generic;
using System.Linq;
using Config;
using Engine;
using EngineBase;


public class FunPrevInfo
{
    public int FuncId;
    /// <summary>
    /// 0=能领取
    /// 1=不能领取
    /// 2=已领取
    /// </summary>
    public int CanGetRw;
    public ConfigSystemUnit SystemUnit;
}

public enum FuncOpenType
{
    None = 0,
    RoleInfo = 100,//个人信息
    MainTask = 200,//主线任务
    AttrLevelUp = 300,//属性升级
    AttrInfo = 400,//属性详情
    RedDot = 500,//红点
    NewGuide = 600,//新手引导
    // -------------------以上默认开放------------------
    SuperTXZ = 700,//通行证功能
    Shop = 701,//商城
    SummonGift = 702,//礼包商店
    LoginGift = 703,//登录礼包
    DiamondStore = 704,//钻石商店
    Chat = 800,//聊天
    TuJian = 1100,//图鉴
    Email = 1200,//邮箱
    FirstCharge = 1300,//首充
    SevenDay = 1400,//七日签到
    DailyTask = 1500,//日常任务
    OnlineReward = 1600,//挂机奖励
    Zhuzhao = 1700,//铸造
    AutoPack = 1701,//自动铸造
    Inherit =  1702,//传承装备
    Artifact = 1703,//神器
    MakeEquip = 1704,//普通装备铸造按钮
    HeroFunc = 1800,//角色-功能界面
    HeroLevelUp = 1801,//角色-英雄升级
    HeroTupo = 1802,//角色-突破
    HeroSkillLvUp = 1803,//角色-技能升级
    HeroTalent = 1804,//角色天赋
    HeroClassic = 1805,//角色秘典
    HeroHoly =  1806,//角色圣物
    SummonSkill = 1900,//组合技能
    //---------技能用索引（完成）-------
    SummonSkillPos1 = 1901,//组合技能栏1
    SummonSkillPos2 = 1902,//组合技能栏2
    SummonSkillPos3 = 1903,//组合技能栏3
    SummonSkillPos4 = 1904,//组合技能栏4
    SummonSkillPos5 = 1905,//组合技能栏5
    SummonPet = 2000,//宠物
    SummonPetPos1 = 2001,//召唤宠物栏1
    SummonPetPos2 = 2002,//召唤宠物栏2
    SummonPetPos3 = 2003,//召唤宠物栏3
    PetLvUp = 2004,//宠物升级
    PetSkillLvUp = 2005,//宠物初始技能升级
    PetTalent = 2006,//宠物天赋
    PetSkillBook = 2007,//宠物技能书
    //------------------------------------
    SummonHero = 2100,//召唤英雄
    Dungeon = 2200,//副本
    Dungeon_gold = 2201,//金币副本
    Dungeon_zhuzhao = 2202,//铸造锤副本
    Dungeon_dimoand = 2203,//钻石副本
    Dungeon_exp = 2204,//经验药水副本
    Dungeon_petMatial = 2205,//宠物材料副本
    Dungeon_petSkillBook = 2206,//宠物技能书副本
    Dungeon_godEquip = 2207,//神器副本
    Dungeon_holy = 2208,//圣物副本
    Village = 2300,//家园
    BuildStone = 2301,//石头矿区
    BuildFactory = 2302,//加工厂
    BuildFood = 2303,//粮食工坊
    BuildTrain = 2304,//训练场
    BuildPet = 2305,//窝棚
    BuildExplore = 2306,//探索营地

    Sokoban = 2400,//深埋宝藏
    Relic = 2401,//遗迹建筑
    Hunting = 2402,//狩猎任务
    SearchPets = 2403,//搜寻宠物
    InheritBOSS = 2404,//传承BOSS
    DiamondMine = 2405,//钻石矿
    GoldCoinRain = 2406,//天女散花
    OpenTreasureChest = 2407,//开箱子砍树
    AdventureBusiness = 2408,//奇遇商人
    Angling = 2409,//钓鱼 垂钓玩法
    
    PVP = 2500,//竞技
    Achievement = 2600,//成就
}

public class FuncPreviewManger : TSingleton<FuncPreviewManger>
{
    private SortedDictionary<int, FunPrevInfo> _funPrevDict = new SortedDictionary<int, FunPrevInfo>();
    public void OnInit()
    {
        foreach (var item in ConfigDataGroup.GetInstance<ConfigSystem>().Data)
        {
            AddFunInfo(new FunPrevInfo()
            {
                FuncId = item.Value.Id,
                CanGetRw = 1,
                SystemUnit = item.Value,
            });
        }
    }

    public override void Dispose()
    {
        _funPrevDict.Clear();
        base.Dispose();
    }

    public void AddFunInfo(FunPrevInfo info)
    {
        if (!_funPrevDict.ContainsKey(info.FuncId))
        {
            _funPrevDict.Add(info.FuncId, info);
        }
    }
    
    public bool FunIsOpened(int funId)
    {
        if (_funPrevDict.TryGetValue(funId, out  var funPrevInfo))
        {
            return funPrevInfo.CanGetRw != 1;
        }
        return false;
    }

    /// <summary>
    /// 更新功能预告
    /// </summary>
    /// <param name="id"></param>
    /// <param name="canGetRw"> 0=能领取 1=不能领取 2=已领取</param>
    public void UpdateFunInfo(int id, int canGetRw)
    {
        foreach (var item in _funPrevDict)
        {
            if (item.Key == id)
            {
                item.Value.CanGetRw = canGetRw;
            }
        }
    }

    public List<FunPrevInfo> GetFunPreList()
    {
        return _funPrevDict.Values.ToList();
    }

    public (bool, string) GetFuncOpenState(FuncOpenType type)
    {
        if(type == FuncOpenType.None) 
            return  (true, "");
        // return (true, "");
        bool isOpen = FunIsOpened((int) type);
        if (isOpen)
        {
            return (true, "");
        }

        if (_funPrevDict.TryGetValue((int) type, out FunPrevInfo prevInfo))
            return (false, ConfigUtils.GetTextById(prevInfo.SystemUnit.Desc,prevInfo.SystemUnit.DescParam));
        return (false, "");
    }
    
    public FunPrevInfo GetFunInfo(int funcId)
    {
        if (!_funPrevDict.ContainsKey(funcId))
        {
            return null;
        }

        return _funPrevDict[funcId];
    }
    
}
