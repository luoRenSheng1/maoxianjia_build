using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapContext
{
    public bool useSSL = false;
    public string host = "";
    public int port;
    public bool autoLogin;
    public int tipCode;
}

public class LoopCheck
{
    private const int maxLoopIndex = 10000000;

    private int curLoopIndex = 1;

    public LoopCheck()
    {
        curLoopIndex = 1;
    }

    public void Clear()
    {
        curLoopIndex = 1;
    }

    public bool CheckLoop()
    {
        ++curLoopIndex;
        return curLoopIndex > maxLoopIndex;
    }
}

//消息错误code
public static class MsgCode
{
    public const int SUCCESS = 0; //成功
    public const int FAIL = 1; //失败
    public const int DATA_UNDEFINED = 2; //数据未定义
    public const int SYSTEM_FAIL = 3; //系统错误

    public const int LOGIN_SERVER_FAIL = 10001; // 连接服务器失败
    public const int LOGIN_SDK_FAIL = 10002; // 登录SDK失败

    public const int TOO_MANY_PEOPLE_VISIT = 100006; // 短时间内太多人访问
    public const int THE_SERVER_IS_BUSY = 100012; // 服务器繁忙
    public const int THE_SERVER_IS_FULL = 101044; // 服务器已满

    public const int VERIFY_FAILED = 101012; // 登录校验失败
}

// 基础常量
public static class ConstDefine
{
    public const float MIN_ARRIVE_DINSTANCE = 0.5f;

    public const float VELOCITY_RUN_LIMIT = 2.0f;
    public const float VELOCITY_RUN_LIMIT_PLACE = 0.1f; // 速度的基准值
    public const float DEFAULT_SKILL_TIME = 5.0f;
    public const float MODEL_SCALE = 60.0f;
    public const float MODEL_FX_SCALE = 90.0f;
    public const float MODEL_SCALE_MIN = 30.0f;

    public const float MIN_DISTANCE = 10.0f;
    public const float CONFIG_PLACE = 0.01f; // 配置中使用的百分比
    public const float CONFIG_PLACE_EX = 0.0001f; // 配置中使用的万分比
    public const float CONFIG_PLACE_TIME = 0.001f; // 配置中使用的千分比，时间
    public const float DEFAULT_ERROR_TIME = 3.0f; // 容错时间
    public const float DEFAULT_DOUBLE_CLICK_TIME = 1.0f; // 双击间隔时间
    public const int CLUB_ICON_SIZE = 44; // 游戏圈大小
    
    public const string FILING_TEXT = "备案号  闽ICP备2025092450号-3A"; // 备案号
    public const string URL_FILING = "https://beian.miit.gov.cn/#/Integrated/recordQuery"; // 备案链接
    public const string URL_USER = "http://download.fkpd.cc/public/url/userTips.html"; // 用户协议
    public const string URL_PRIVACY = "http://download.fkpd.cc/public/url/PrivacyTips.html"; // 隐私协议

    public static float DEFAULT_MOVE_SPEED { get; private set; } = 240.0f;

    public static int TINT_COLOR_ID = Shader.PropertyToID("_TintColor"); //颜色相关
    public static int COLOR_ID = Shader.PropertyToID("_Color");
    public static readonly Color COLOR_HIDE = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    public const int CONST_MAGIC_KEY = 10000001; //钥匙
    public const int Item_SpeedCardId = 3000;//加速卷轴道具ID
    public const int Item_GoldId = 2000;//金币轴道具ID
    public const int Item_Diamond = 1000;//钻石道具ID
    public const int Item_LotteryCardId = 10000002;//加速卷轴道具ID
    public const int Item_TaskTurnCardId = 10000008;//加速卷轴道具ID
    
    public const int Item_HeroLevelId = 1010001;//角色升级道具ID
    
    public const int Item_HeroBreakId = 1010002;//角色突破道具ID
    
    public const int Item_HeroSkillUp = 1010004;//角色技能升级道具Id

    public const int ItemLevLen = 100;//道具等级最后3位数
    
    public const int PetPieceId = 1010013;//宠物碎片道具ID
    
    public const int PetSkillIndex = 5;//宠物技能开始索引位置
    
    public static void Init()
    {
    }

    public static void InitConfigParam()
    {
    }
}

public class ServerInfo
{
    public string name = "";
    public string url = "";
}

/// <summary>
/// 更友好提供给策划配置
/// </summary>
public class AppConstData
{
    public List<ServerInfo> LocalServerList = new List<ServerInfo>();
}

public class MinigamePlatform
{
    public static string ios = "ios"; //ios': iOS微信（包含 iPhone、iPad）;
    public static string android = "android"; //'android': Android微信;
    public static string windows = "windows"; //'windows': Windows微信;
    public static string mac = "mac"; //'mac': macOS微信;
    public static string devtools = "devtools"; //"devtools"
}

public enum EN_SKILL_FX
{
    FX_NORMAL,
    FX_LINE,
    FX_PARABOLA,
    FX_FOLLOW = 7, // 跟随特效
    FX_BEZIER = 21, // 光明之光
    FX_CHAIN_LIGHT = 22, // 连锁闪电
    FX_DELAY_LIGHT = 25, // 延迟激光
}

public enum EN_BUFF_ADD_TYPE
{
    // ATK_ADD = 5,     // 攻击加成
    Begin = 0,  //开始
    PhysicAtk = 1, // 物理伤害
    MagicAtk = 2,  // 魔法伤害
    SorceryAtk = 3,  // 道术伤害
    HP = 4,    //生命值
    PhysicDef = 5,  // 物理防御
    MagicDef = 6, // 魔法防御
    SorceryDef = 7,  // 道术防御
    Recovery = 8,    // 生命恢复
    PetAtk = 9,   // 附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
    PhysicAtkADD = 10,  // 物理伤害加成
    MagicAtkADD = 11,   // 魔法伤害加成
    SorceryAtkADD = 12,  // 道术伤害加成
    HP_ADD = 13,           // 生命加成比率
    PhysicDefADD = 14,    // 物理防御加成
    MagicDefADD = 15,    // 魔法防御加成
    SorceryDefADD = 16,    // 道术防御加成
    EarthAtkADD = 17,    // 地系伤害加成
    WaterAtkADD = 18,    // 水系伤害加成
    FireAtkADD = 19,    // 火系伤害加成
    AirAtkADD = 20,    // 气系伤害加成
    PetAtkADD = 21,    // 宠物伤害加成（角色各功能加给宠物身上生效的伤害）
    HPMultiple = 22,    // 生命倍率
    AtkMultiple = 23,    // 伤害倍率
    JoukRate = 24,    // 英雄 闪避率
    AtkHitRate = 25,    // 英雄 普通攻击命中率
    ParryRate = 26,    // 格挡率
    ParryValue = 27,    // 格挡值
    IgnoreDef = 28,    // 无视防御   英雄无视防御百分比
    CriticalStrike = 29,           // 暴击
    CriticalInjury = 30,         // 爆伤
    BossDamageAdd = 31,           // BOSS伤害加成
    MonsterDamageAdd = 32,         //小怪伤害加成
    Mitigation = 33,//减伤
    Bloodsucking = 34,     // 吸血
    AtkHPRecovery = 35,     //攻击回复   普攻回复的生命具体值
    SkillDamage = 36,//技能伤害
    MagicTimesAdd = 37, //技能次数加成
    MagicTimes = 38,//技能次数
    GoldAdd = 39, //金币加成
    SkillCd = 40, //技能冷却
    AtkSpeed = 41,//攻速
    ComboAtk = 42,//连击
    CounterAtk = 43, //反击
    LoreEquipRate = 44, //稀有传承概率
    DropLoreEquipRate = 45, //传承掉落概率
    OnlineAwardTimes = 46, //关卡挂机奖励次数
    HomeLimitMaxTime = 47, //家园挂机时间上限
    BattleFinalAttack = 48, //战斗最终伤害 ---- 还未投放，暂时为0
    KilledRecovery = 49, //消灭对象后HP回复
    
    ATK = 101,     // 攻击力    总攻击力  eBattleAttr_FinalAttack = 101 用作Attack伤害
    Def = 102,   // 防御值  总防御力   需取服务器端下发的eBattleAttr_FinalDefence = 102 用作防御值
}

/// <summary>
/// 受击buff 特效类型
/// </summary>
public enum EN_BUFF_TYPE
{
    /// <summary>
    /// 普通受击
    /// </summary>
    Normal = 1000,
    /// <summary>
    /// 眩晕
    /// </summary>
    Dizzy = 1001,
    /// <summary>
    /// 灼烧
    /// </summary>
    Burn = 1002,
    /// <summary>
    /// 流血上层
    /// </summary>
    Bleed = 1003,
    /// <summary>
    /// 流血下层
    /// </summary>
    Bleed1 = 10031,
    /// <summary>
    /// 回春
    /// </summary>
    AddBlood = 1004,
    /// <summary>
    /// 无敌
    /// </summary>
    Invincible = 1005,
    /// <summary>
    /// 反弹
    /// </summary>
    //Bounce = 1006,
    /// <summary>
    /// 复活上层
    /// </summary>
    ReBirth = 1007,
    /// <summary>
    /// 复活下层
    /// </summary>
    ReBirth1 = 10071,
    /// <summary>
    /// 重置CD
    /// </summary>
    //ResetSkillCD = 1008,
    /// <summary>
    /// 生命护盾
    /// </summary>
    LifeShield = 1009,
    /// <summary>
    /// 伤害转生命护盾
    /// </summary>
    LifeShield1 = 1010,
    /// <summary>
    /// 防御生命护盾
    /// </summary>
    LifeShield2 = 1011,
    /// <summary>
    /// 减少目标攻击
    /// </summary>
    AtkReduce = 1014,
    /// <summary>
    /// 减少目标攻速  上层
    /// </summary>
    SpeedReduce = 1015,
    /// <summary>
    /// 减少目标攻速 下层
    /// </summary>
    SpeedReduce1 = 10151,
    /// <summary>
    /// 闪电
    /// </summary>
    Lightning = 2001,
    /// <summary>
    /// 爆炸
    /// </summary>
    Bomb = 2002,
    
    PetAtk =10201007,   //宠物释放技能后为角色提供等同自身宠物伤害{0}%的主属性，持续3秒
    PetAtkBottom =102010071,
    PetAtkAdd =10201008,   // 宠物释放技能后为角色提供等同自身宠物等级{0}%的伤害加成，持续3秒
    PetAtkAddBottom =102010081,
    ElementAdd =10201009,   // 宠物释放技能后为角色提供等同自身宠物等级{0}%的元素伤害加成，持续3秒
    ElementAddBottom =102010091,
    BaoJiAdd =10201010,    // 宠物释放技能后为角色提供等同自身宠物等级{0}%的暴击加成，持续3秒
    BaoJiAddBottom =102010101,
    BaoShangAdd =10201011,   // 宠物释放技能后为角色提供等同自身宠物等级{0}%的爆伤加成，持续3秒
    BaoShangAddBottom =102010111,
    
    PetRecovery =10301012,  // 宠物释放技能后回复角色等同自身宠物伤害{0}%的血量
    PetRecoveryTime =10301013,  // 宠物释放技能后回复角色等同自身宠物伤害{0}%的血量，持续3秒
    PetRecoveryTimeBottom =103010131,
    PetLifeShield =10301014,  // 宠物释放技能后为角色提供等同自身宠物伤害{0}%的生命护盾，持续3秒
    PetGongJiRecovery =10301015,  // 宠物释放技能后提供角色等同自身宠物伤害{0}%的攻击回复，持续3秒
    PetGongJiRecoveryBottom =103010151,
    PetShouJiRecovery =10301016,  // 角色受到攻击时，回复等同自身宠物伤害{0}%的血量，持续3秒
    
    PetDef =10401017,  // 宠物释放技能后为角色提供等同自身宠物伤害{0}%的防御属性，持续3秒
    PetParry =10401018,  // 宠物释放技能后为角色提供等同自身宠物伤害{0}%的格挡值属性，持续3秒
    PetParryBottom =104010181,
    PetDefAdd =10401019,  // 宠物释放技能后为角色提供等同自身宠物等级{0}%的防御加成，持续3秒
    PetDefAddBottom =104010191,
    PetJouk =10401020,  // 宠物释放技能后为角色提供等同自身宠物等级{0}%的闪避率，持续3秒
    PetJoukBottom =104010201,
    PetParryRate =10401021,  // 宠物释放技能后为角色提供等同自身宠物等级{0}%的格挡率，持续3秒
}

/// <summary>
/// 受击buff 特效类型
/// </summary>
public enum EN_BUFF_ACHIEVE_TYPE
{
    /// <summary>
    /// 反弹
    /// </summary>
    Bounce = 1006,
    /// <summary>
    /// 重置CD
    /// </summary>
    ResetSkillCD = 1008,
    /// <summary>
    /// 减少怪物眩晕概率10%
    /// </summary>
    DizaayRate = 1012,
    /// <summary>
    /// 怪物回复效果比例减少1%
    /// </summary>
    RecoveryRate = 1013,
    /// <summary>
    /// 追加防御值伤害
    /// </summary>
    DefHurt = 2003,
    /// <summary>
    /// 狂战
    /// </summary>
    HurtAdd = 2004,
    /// <summary>
    /// 追加生命值伤害
    /// </summary>
    HpHurt = 2005,
    /// <summary>
    /// 追加主属性
    /// </summary>
    AttrHurt = 2006,
    /// <summary>
    /// 受击回复攻击者伤害值比例的生命
    /// </summary>
    HurtRecovery = 2007,
    /// <summary>
    /// 造成目标当前生命值d%的伤害
    /// </summary>
    HpPercentHurt = 2008,
    /// <summary>
    /// 为角色提供等同自身宠物等级比例的属性加成
    /// </summary>
    PetLevelHurt = 2009,
    /// <summary>
    /// 为角色提供等同宠物伤害比例的的属性加成
    /// </summary>
    PetHurtRatio = 2010,
    /// <summary>
    /// 回复角色等同自身宠物伤害比例的血量
    /// </summary>
    PetHurtRecovery = 2011,
    /// <summary>
    /// 释放技能时，法术概率再释放1次
    /// </summary>
    DoubleCastSkill = 2012,
    /// <summary>
    /// 普攻对同目标伤害持续提升，最多提升至X%
    /// </summary>
    UpgradeHurt = 2013,
    /// <summary>
    /// 减少怪物对玩家的属性降低
    /// </summary>
    RoleAttr = 2014,
    /// <summary>
    /// 伤害提升
    /// </summary>
    DamageUp = 2015,
    /// <summary>
    /// 受到的伤害减少
    /// </summary>
    DamageReduce = 2016,
    /// <summary>
    /// 造成目标自身生命值d%的伤害
    /// </summary>
    TargetLifeDamage = 2017,
    /// <summary>
    /// 再释放一次宠物技能
    /// </summary>
    CastPetSkill = 2018,
}

/// <summary>
/// 职业类型
/// </summary>
public enum EN_HARM_TYPE
{
    Normal = 0,
    /// <summary>
    /// 物理伤害
    /// </summary>
    PhysicAtk,
    /// <summary>
    /// 魔法伤害
    /// </summary>
    MagicAtk,
    /// <summary>
    /// 道术伤害
    /// </summary>
    SorceryAtk,
}

/// <summary>
/// 职业类型
/// </summary>
public enum EN_HARM_ATTR_TYPE
{
    Normal = 0,
    /// <summary>
    /// 地系伤害
    /// </summary>
    Earth,
    /// <summary>
    /// 水系伤害
    /// </summary>
    Water,
    /// <summary>
    /// 火系伤害
    /// </summary>
    Fire,
    /// <summary>
    /// 气系伤害
    /// </summary>
    Air,
}

///---伤害类型
public enum EN_DAMAGE_TYPE
{
    /// <summary>
    /// 普通
    /// </summary>
    NORMAL = 0, //--普通
    /// <summary>
    /// 闪避
    /// </summary>
    DODGE = 1, //--闪避
    /// <summary>
    /// 暴击
    /// </summary>
    STRIKE = 2, //--暴击
    /// <summary>
    /// 连击
    /// </summary>
    COMBO = 3, //--连击
    /// <summary>
    /// 反击
    /// </summary>
    COUNTER = 4, //--反击
    /// <summary>
    /// 反弹
    /// </summary>
    Bounce = 5,
    /// <summary>
    /// 回血
    /// </summary>
    RECOVERY = 11, //--回血
    /// <summary>
    /// 闪避率
    /// </summary>
    Jouk = 20,
    /// <summary>
    /// 护盾值
    /// </summary>
    LifeShield = 21,
    /// <summary>
    /// 格挡
    /// </summary>
    Parry = 22,
}

//---字色
public class FORNT_COLOR
{
    // public const string WRITE = "#FFFFFF"; //-- 白色（默认）
    // public const string RED = "#E12727"; //-- 红色（默认）
    // public const string RED1 = "#FF6060"; //--红色
    public const string ORANGE = "#F47E34"; // -- 橘色(嘲讽)
    public const string BLUE1 = "#4472C4"; //-- 蓝色（防御）
    // public const string BLUE2 = "#4472C4"; //-- 蓝色（闪避）
    // public const string BLUE3 = "#80FBFF"; //-- 蓝色（冰冻）
    // public const string GREEN = "#00B04F"; //-- 绿色,
    // public const string GREEN1 = "#1AE832"; //--绿色,   
    public const string GREEN2 = "#76DC2A"; //--绿色
    public const string YELLOW = "#C3AEFF"; //--紫色（反弹）
    public const string GRAY = "#D1D1D1"; //--灰色
    public const string YELLOW1 = "#FFEC43"; //--黄色
//新的
    public const string New_Red = "#ff0000";
    public const string New_PINZI1 = "#bdaba2";//品质1
    public const string New_PINZI2 = "#7ed2a6";//品质2 
    public const string New_PINZI3 = "#82b6d6";//品质3 
    public const string New_PINZI4 = "#ad68c8";//品质4 
    public const string New_PINZI5 = "#cbd16b";//品质5
    public const string New_PINZI6 = "#f3c95c";//品质6 
    public const string New_PINZI7 = "#f38277";//品质7 
    public const string New_PINZI8 = "#f081c3";//品质8 
    public const string New_PINZI9 = "#dc6fee";//品质9 
    public const string New_PINZI10 = "#ff7200";//品质10
    public const string New_Fight = "#fde38f";//战力 
    public const string New_Equip_Entry = "#835c25";//装备词条 
    public const string New_Equip_Att = "#eac042";//装备属性 
    public const string New_Equip_Entry_Desc = "#7b7b7b";//词条说明 
    public const string New_Skill_Entry = "#63f0e4";//技能词条 
    public const string New_Black = "#948d84";//黑色字
    public const string New_White = "#ffffff";//白色字
}

//-飘字动效索引
public class FLYFORNT_TRANTYPE
{
    /// <summary>
    /// 飘字一般动效
    /// </summary>
    public const string NORMAL = "nomral"; // -- 一般动效
    /// <summary>
    /// 飘字暴击动效
    /// </summary>
    public const string BAOJI = "baoji"; // -- 暴击动效
    /// <summary>
    /// 飘字BUFF效果
    /// </summary>
    public const string BUFF = "buff"; //--BUFF效果
    /// <summary>
    /// 飘字暴击动效
    /// </summary>
    public const string SKILL = "skill"; // -- 技能动效
}

public enum EN_SKILL_TYPE
{
    /// <summary>
    /// 普攻，普通技能攻击
    /// </summary>
    NORMAL = 1,
    /// <summary>
    /// 技能攻击
    /// </summary>
    XP = 2,
    /// <summary>
    /// BUFF属性 被动技能
    /// </summary>
    BUFF = 3,
    /// <summary>
    /// 抽卡技能
    /// </summary>
    ChoukaSkill = 5
}

public enum EN_TARGET_RANGE_TYPE
{
    NONE = 0,
    SHIFA = 1,
    PARAM = 2,
}

public enum EN_TARGET_RANGEREFER_TYPE
{
    ZISHEN = 0,                     // 自身
    MUBIAO = 1,                     // 目标
    ZISHENQIANFANG = 2,             // 自身前方
    ZISHENQIANFANGZHONGPAI = 3,     // 自身前方-中排(我方使用Hero位置，敌方使用Hero对应的敌方中心位置)
}

public enum EN_TARGET_TARGETPRIORITY
{
    // 0 =所有，1 =随机，10 =距离最近，11 =距离最远；20 =生命最高，21 =生命最低，30=攻击力最高，31攻击力最低）
    ALL = 0,
    RANDOM = 1,
    ZUIJIN = 10,
    ZUIYUAN = 11,
    HPZUIGAO = 20,
    HPZUIDI = 21,
    ATKZUIGAO = 30,
    ATKZUIDI = 31,
}

public enum EN_CAMP_TYPE
{
    HERO,
    FRIEND,
    ENEMY,
    NEUTRALITY
}

public enum EN_GUANKA_STEP
{
    INIT, // 初始
    SHOW, // 进场
    FIGHTING, // 战斗
    RESULT, // 结算
};

// TODO 暂时注释
// public enum EN_EQUIP_PARTS
// {
//     None,
//     Weapon, //武器
//     Helmet, //头盔
//     ShoulderArmor, // 肩甲
//     Breastplate, //胸甲
//     Cloak, //披风
//     BracerGuards, //护腕
//     Gloves, //手套
//     Belt, //腰带
//     Pants, //裤子
//     Shoes, //鞋子
//     Count
// }

public enum EN_EQUIP_PARTS
{
    None,
    Weapon, //武器
    Helmet, //头盔
    Breastplate, //胸甲
    Shoes, //鞋子
    Cloak, //披风   5  传承装备
    BracerGuards, //护腕   6  传承装备
    Gloves, //手套    7  传承装备
    Belt, //腰带    8  传承装备
    ShoulderArmor, // 肩甲  不要了
    Pants, //裤子   不要了
    Count
}

public enum EN_Other_PARTS
{
    None,
    Weapon, //武器
    Helmet, //头盔
    ShoulderArmor, // 肩甲
    Breastplate, //胸甲
    Cloak, //披风
    Count
}

public class SCREEN
{
    public const int WIDTH = 720;
    public const int HEIGHT = 1280;
}

public static class Hash
{
    public static int animKey_finish = Animator.StringToHash("finish");
    public static int animKey_start = Animator.StringToHash("start");

    public static int animKey_workIndex = Animator.StringToHash("workIndex");
    public static int animKey_impact = Animator.StringToHash("impact");

    public static int animKey_idle = Animator.StringToHash("idle0");
    public static int animKey_run = Animator.StringToHash("run");
    public static int animKey_jumpStart = Animator.StringToHash("jump_start");
    public static int animKey_jumpFinish = Animator.StringToHash("jump_finish");

    public static int animKey_offset = Animator.StringToHash("offset");
    public static int animKey_petSpecial = Animator.StringToHash("petSpecial");
    public static int animKey_moveType = Animator.StringToHash("moveType");
    public static int animKey_justRun = Animator.StringToHash("justRun");
    public static int animKey_cloudRide = Animator.StringToHash("cloud_ride");
    public static int animKey_workSpeed = Animator.StringToHash("speed_Work");
    public static int animKey_rollOver = Animator.StringToHash("rollOver");

    public static int animKey_butterfly = Animator.StringToHash("butterfly");

    public static int shader_tileColor = Shader.PropertyToID("_TintColor");

    public static int shader_mainColor = Shader.PropertyToID("_MainColor");
    public static int shader_ambient = Shader.PropertyToID("_AmbientFactor");
    public static int shader_color = Shader.PropertyToID("_Color");
    public static int shader_alpha = Shader.PropertyToID("_Alpha");
    public static int shader_activeAlpha = Shader.PropertyToID("_isToggled_Alpha");
    public static int shader_matcapColor = Shader.PropertyToID("_MatCap");
    public static int shader_planeHeight = Shader.PropertyToID("_PlaneHeight");
    public static int shader_outlineColor = Shader.PropertyToID("_OutlineColor");
    public static int shader_outlineWidth = Shader.PropertyToID("_OutlineWidth");
    public static int shader_gray = Shader.PropertyToID("_Gray");
    public static int shader_effectAmount = Shader.PropertyToID("_EffectAmount");
    public static int shader_edges = Shader.PropertyToID("_Edges");
    public static int shader_posX = Shader.PropertyToID("_PosX");
    public static int shader_posY = Shader.PropertyToID("_PosY");
    public static int shader_rimPower = Shader.PropertyToID("_RimPower");
    public static int shader_slice = Shader.PropertyToID("_SliceAmount");
    public static int shader_color2 = Shader.PropertyToID("_RimColor");
    public static int shader_center = Shader.PropertyToID("_CenterPos");
    public static int shader_luminosity = Shader.PropertyToID("_Luminosity");
    public static int shader_fat = Shader.PropertyToID("_Fat");
    public static int shader_scale = Shader.PropertyToID("_Scale");
    public static int shader_offset = Shader.PropertyToID("_Offset");
    public static int shader_grass = Shader.PropertyToID("_GrassFactor");
    public static int shader_specular = Shader.PropertyToID("_Specular");
    public static int shader_activeGlow = Shader.PropertyToID("_isToggled_Emission");
    public static int shader_glowFactor = Shader.PropertyToID("_EmissionMultiplier");
    public static int shader_glowColor = Shader.PropertyToID("_EmissionColor");
    public static int shader_activeRimLight = Shader.PropertyToID("_isToggled_RimLight");
    public static int shader_rimLightFactor = Shader.PropertyToID("_RimIntensity");
    public static int shader_rimLightColor = Shader.PropertyToID("_RimColor");

    public static int shader_subTexture = Shader.PropertyToID("_SubTex");
    public static int shader_oddEye = Shader.PropertyToID("_Odd_Eye");
}


public static class defLanguage
{
    public const int English = 0;    //英语
    public const int Korean = 1;     //韩语
    public const int Japanese = 2;   //日语
    public const int ChineseSimplified = 3;  // 简体中文
    public const int ChineseTraditional = 4; // 繁体中文
    public const int German = 5;
    public const int French = 6;
    public const int Spanish = 7;
    public const int Portuguese = 8;
    public const int Russian = 9;
    public const int Indonesian = 10;
    public const int Italian = 11;
    public const int Thai = 12;
    public const int Vietnamese = 13;
    public const int Turkish = 14;
    public const int MAX = 15;

    public const int Brazil = 100; // @note: 정식으로 사용하려면 Max 안의 값으로 넣어야 한다
    public const int Arabic = 101;
    public const int Spanish_Latam = 102;
    public const int Polish = 103;
}


// 多语言key常量
public static class StringDefine
{
    public const int STRING_GOLD = 1; //金币
    public const int STRING_PROPS_ENOUGH = 43; //道具不足
    public const int STRING_Speed = 2020; //移动速度
    public const int STRING_ATTACK = 2022; //攻击
    public const int STRING_LIFE = 2021;//生命
    public const int STRING_DEFENSE = 2023; //防御
    public const int STRING_SUDU = 2024; //速度
    public const int STRING_BJL = 2031;//	暴击率
    public const int STRING_LJL = 2032;//	连击率
    public const int STRING_FJL = 2033;//	反击率
    public const int STRING_HF = 2034;//	恢复
    public const int STRING_BS = 2035;//	爆伤
    public const int STRING_KBL = 2036;//	抗暴率
    public const int STRING_CTL = 2037;//	穿透率
    public const int STRING_GJXS = 2038;//	攻击系数
    public const int STRING_LJXS = 2039;//连击系数
    public const int STRING_FJXS = 2040;//反击系数

    public const int STRING_MAGIC_KEY_NOT_ENOUGH = 10001; //钥匙不足
    public const int STRING_LEVEL_ERROR = 10002;//当前条件未满足升级
    public const int STRING_USE_EQUIP_SUCC = 10003;//装备成功
    public const int STRING_USE_DECOM_SUCC = 10004;//分解成功
    public const int STRING_USE_REPLACE_SUCC = 10005;//替换成功
    public const int STRING_USE_EQUIP_FAILD = 10006;//装备失败
    public const int STRING_USE_DECOM_FAILD = 10007;//分解失败
    public const int STRING_USE_REPLACE_FAILD = 10008;//替换失败
    public const int STRING_USE_UNKNOWN_ERROR_FAILD = 10009;//未知错误
    public const int STRING_OPEN_ERROR_FAILD = 10010;//开启成功
    public const int STRING_UPGRADE_CD_ERROR = 10011;//升级CD中
    public const int STRING_UPGRADE_CURRENCY_ERROR = 10012;//货币不足
    public const int STRING_UPGRADE_LEVEL_ERROR = 10013;//等级条件不足
    public const int STRING_UPGRADE_LIMITE_LEVEL_ERROR = 10014;//等级上限
}

public enum HelpType
{
    Help_stone = 10006,//石头矿区
    Help_facotry = 10002,//加工厂
    Help_food = 10001,//粮食工坊
    Help_train = 10005,//训练场
    Help_wopeng = 10004,//窝棚
    Help_explore = 10003,//探索营地
    Help_meirith = 20001,//每日特惠
    Help_tthl = 20002,//天天好礼
    Help_fkzn = 20003,//疯狂指南
    Help_txdy = 30001,//天下第一
    Help_PetDetails = 40001,//宠物详情
    Help_PetTalent = 40002,//宠物天赋
    Help_PetSkillBook = 40003,//宠物技能书
    Helo_RoleHelp = 50001,//角色帮助
    Help_HolyHelp = 60001,//圣物帮助
    Help_SummonHeroHelp = 70001,
}

public enum ArtifactType
{
    SHIELD = 53,//盾牌
    MEDAL = 54,//勋章
    STONE = 55,//符石
    JOB = 56//职印
}

public enum QualityType
{
    COMMON = 1,//普通
    GOOD = 2,//良好
    UNCOMMON = 3,//稀有
    EPIC = 4,//史诗
    LEGEND = 5,//传说
    MYTH = 6,//神话
    IMMORTAL = 7//不朽
}

public enum FunctionPosition
{
    /// <summary>
    /// 其他
    /// </summary>
    OTHER = 0,
    /// <summary>
    /// 左边栏位
    /// </summary>
    LEFT = 1,
    /// <summary>
    /// 右边栏位
    /// </summary>
    RIGHT = 2,
    /// <summary>
    /// 折叠栏位
    /// </summary>
    FOLD = 3
}

public enum SkillBookType
{
    /// <summary>
    /// 稀有技能书
    /// </summary>
    UNCOMMON = 1050001,
    /// <summary>
    /// 史诗技能书
    /// </summary>
    EPIC= 1050002,
    /// <summary>
    /// 传说技能书
    /// </summary>
    LEGEND = 1050003,
    /// <summary>
    /// 神话技能书
    /// </summary>
    MYTH = 1050004,
    /// <summary>
    /// 不朽技能书
    /// </summary>
    IMMORTAL = 1050005,
    /// <summary>
    /// 随机技能书
    /// </summary>
    RANDOM = 1050006,
}

public enum DropItemType
{
    /// <summary>
    /// 金币
    /// </summary>
    GOLD = 2000,
    /// <summary>
    /// 秘典
    /// </summary>
    CLASSIC = 5001,
    /// <summary>
    /// 圣物
    /// </summary>
    HOLY = 5002,
    /// <summary>
    /// 传承Inherit
    /// </summary>
    INHERIT = 5003,
}

public enum SoundType
{
    /// <summary>
    /// 登录BGM
    /// </summary>
    LoginBGM = 1,
    /// <summary>
    /// 第一章BGM
    /// </summary>
    ChapterBGM1,
    /// <summary>
    /// 第二章BGM
    /// </summary>
    ChapterBGM2,
    /// <summary>
    /// 第三章BGM
    /// </summary>
    ChapterBGM3,
    /// <summary>
    /// 第四章BGM
    /// </summary>
    ChapterBGM4,
    /// <summary>
    /// 第五章BGM
    /// </summary>
    ChapterBGM5,
    /// <summary>
    /// 第六章BGM
    /// </summary>
    ChapterBGM6,
    /// <summary>
    /// 营地BGM
    /// </summary>
    CampBGM,
    /// <summary>
    /// 副本BGM
    /// </summary>
    CopyBGM,
    /// <summary>
    /// PVPBGM
    /// </summary>
    PVPBGM,
    /// <summary>
    /// 家园BGM
    /// </summary>
    HOMEBGM,
    /// <summary>
    /// 事件地图BGM
    /// </summary>
    EventMapBGM,
    /// <summary>
    /// 通用点击音效
    /// </summary>
    GeneralClickSE,
    /// <summary>
    /// 通用关闭按钮音效
    /// </summary>
    GeneralCloseSE,
    /// <summary>
    /// 通用获得奖励音效
    /// </summary>
    GeneralGainRewardSE,
    /// <summary>
    /// 通用点开弹窗界面音效
    /// </summary>
    GeneralClickPopupSE,
    /// <summary>
    /// 通用挑战BOSS点击音效
    /// </summary>
    GeneralChallengeBossClickSE,
    /// <summary>
    /// 通用胜利结算音效
    /// </summary>
    GeneralWinSE,
    /// <summary>
    /// 通用失败结算音效
    /// </summary>
    GeneralLoseSE,
    /// <summary>
    /// 通用普攻受击音效
    /// </summary>
    GeneralAttackSE,
    /// <summary>
    /// 通用挂机获得金币音效
    /// </summary>
    GeneralOnlineGainGoldSE,
    /// <summary>
    /// 购买成功音效
    /// </summary>
    BuySuccessSE,
    /// <summary>
    /// 铸造装备音效
    /// </summary>
    MakeEquipSE,
    /// <summary>
    /// 升级音效
    /// </summary>
    UpLvSE,
    /// <summary>
    /// 战力飙升音效
    /// </summary>
    MilitaryPowerUpSE,
    /// <summary>
    /// 角色合成音效
    /// </summary>
    HeroCompoundSE,
    /// <summary>
    /// 通用上阵音效
    /// </summary>
    GeneralUploadSE,
    /// <summary>
    /// 通用抽卡抽取普通道具的音效
    /// </summary>
    GeneralLotteryCommonItemSE,
    /// <summary>
    /// 金币堆弹出音效
    /// </summary>
    GoldPilePopupSE,
    /// <summary>
    /// 金币拾取音效
    /// </summary>
    GoldPickupSE,
    /// <summary>
    /// 钻石拾取音效
    /// </summary>
    DiamondPickupSE,
    /// <summary>
    /// 通用挖宝箱音效
    /// </summary>
    GeneralScoopTreasureSE,
    /// <summary>
    /// 通用挖矿音效
    /// </summary>
    GeneralScoopDiamondSE,
    /// <summary>
    /// 装备拾取音效
    /// </summary>
    EquipPickupSE,
    /// <summary>
    /// 功能解锁音效
    /// </summary>
    FunctionUnlockSE,
    /// <summary>
    /// 宠物书打孔音效
    /// </summary>
    PetBookPunchingSE,
    /// <summary>
    /// 传送音效
    /// </summary>
    TransferSE,
    /// <summary>
    /// 箱子移动音效
    /// </summary>
    BoxMoveSE,
    /// <summary>
    /// 箱子进入插槽音效
    /// </summary>
    BoxInTrenchSE,
    /// <summary>
    /// 道路生成音效
    /// </summary>
    WayGenerateSE,
    /// <summary>
    /// 天女散花音效
    /// </summary>
    FairySendingFlowersSE,
    /// <summary>
    /// 钓鱼抛竿音效
    /// </summary>
    FishingRollCastSE,

    /// <summary>
    /// 引导
    /// </summary>
    guide1 = 43,
    guide2,
    guide3,
    guide4,
    guide5,
    guide6,
    guide7,
    guide8,
    guide9,
    guide10,
    guide11,
    guide12,
    guide13,
    guide14,
    guide15,
    guide16,
    guide17,
    guide18,
    guide19,
    guide20,
    guide21,
    guide22,
    guide23,
    guide24,
    guide25,
    guide26,
    guide27,
    guide28,
    guide29,
    guide30,
    guide31,
    guide32,

    /// <summary>
    /// 宠物初始技能BUFF类音效
    /// </summary>
    PetInitSkillBuffSE = 10201007,
}