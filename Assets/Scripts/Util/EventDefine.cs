using System;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************/
/* 一些约定：
 *  1）编号 10000 以下为特殊消息，留待底层工作时备用
 *  
 *  2）100000 （10万）段编号段为消息专用，每个消息占用100个消息段
 *      例如：100100 - 100199 为玩家信息相关消息
 *  
 *  3）200000 （20万）段编号段为战斗专用
 *      例如：
/************************************************************************/

namespace Engine
{
    public class EventDefine
    {
        public const int EVENT_DEFINE_NONE = 0;

        #region 底层事件

        public const int EVT_UI_DESTROY = 81;
        public const int EVT_UI_SHOW = 82;
        public const int EVT_UI_HIDE = 83;
        public const int STR_SOUND_SETTING_CHANGE = 84;

        // 登录
        public const int STR_SDK_LOGIN_INIT_SDK_SUCCESS = 101; // 初始化SDK完成
        public const int STR_SDK_LOGIN_INIT_SDK_FAIL = 102; // 初始化SDK失败
        public const int STR_SDK_LOGIN_LOAD_CONF_COMPLETE = 103; // 加载控制文件
        public const int STR_SDK_LOGIN_LOAD_CONF_FAIL = 104; // 加载控制文件失败
        public const int STR_SDK_LOGIN_SUCCESS = 105; // SDK登录成功
        public const int STR_SDK_LOGIN_FAIL = 106; // SDK登录失败
        public const int STR_RECONNECT_SUCCESS = 107;//重登
        public const int STR_TAP_SDK_LOGIN_SUCCESS = 108; // TAPSDK登录成功

        // 资源更新
        public const int STR_NEW_VERSION_CHECK_NONE = 304; // 增量更新-版本无需更新
        public const int STR_NEW_VERSION_BIG_VERSION_UPDATE = 305; //增量更新-大版本需要更新
        public const int STR_NEW_VERSION_CHECK_UPDATE = 306; //增量更新-版本检查需要更新
        public const int STR_NEW_VERSION_CHECK_SUCCESS = 307; //增量更新-版本成功
        public const int STR_NEW_VERSION_CHECK_FAIL = 308; //增量更新-版本失败
        public const int STR_NEW_VERSION_POINT_DOWN_SYNC = 309; //增量更新-版本下载增量包进度同步
        public const int STR_NEW_VERSION_POINT_DOWN_FAIL = 310; //增量更新-下载失败
        public const int STR_NEW_VERSION_COMPRESS_SYNC = 311; //增量更新-版本解压增量包进度同步
        public const int STR_NEW_VERSION_COMPRESS_FAIL = 312; //增量更新-版本解压增量包失败

        // 加载
        public const int STR_LOAD_COMMON_RES_SUCCESS = 501; //加载公有资源完成

        // LoginRes
        public const int STR_LOGIN_RES_PANEL_SHOW_LOADING = 601; // 显示Loading
        public const int STR_LOGIN_RES_PANEL_HIDE_LOADING = 602; // 显示Loading
        public const int STR_LOGIN_RES_PANEL_DECOMPRESS_FILE_SUCCESS = 603; // 解压文件成功-表现
        public const int STR_LOGIN_RES_PANEL_DECOMPRESS_FILE_FAIL = 604; // 解压文件失败-表现
        public const int STR_LOGIN_RES_PANEL_RESVERSION_UPDATE = 605; // 资源更新确定更新
        public const int STR_LOGIN_RES_PANEL_NEW_VERSION_SUCCESS = 606; // 更新包更新成功-表现
        public const int STR_LOGIN_RES_SERVER_FAIL = 607; // 重新登录
        public const int STR_LOGIN_RES_RESHOW = 608; // 重新登录
        public const int STR_LOGIN_RES_STOPALL = 609; // 暂停加载
        public const int STR_LOGIN_RES_STARTLOGIN = 610; // 开始登录
        public const int STR_LOGIN_RES_SERVER_FAIL_RETRY = 611; // 登录重试

        public const int STR_ON_TOUCH_BEGIN_ = 700;
        public const int STR_ON_TOUCH_MOVED_ = 701;
        public const int STR_ON_TOUCH_ENDED_ = 702;

        public const int DRAGDROP_CANCLE = 800;//拖拽取消，没有接收者

        public const int STR_LOGIN_CHECK_SUCCESS = 900; // 登录结果
        public const int STR_LOGIN_CHECK_FAIL = 901; // 登录结果
        public const int STR_LOGIN_PLAYER_SUCCESS = 902; // 登录结果
        public const int STR_LOGIN_PLAYER_FAIL = 903; // 登录结果
        public const int STR_LOGIN_PLAYER_User_check = 904; // 用户协议
        public const int EVENT_DAILY_RESET_UPDATE = 905;//每日重置
        #endregion

        public const int FIGHT_DATA_HERO_CASTSKILL = 1001;
        public const int FIGHT_DATA_MONSTER_DEAD = 1101;
        public const int FIGHT_DATA_MONSTER_BOSS_INFO = 1102;
        public const int STAGE_DATA_INFO = 1103;
        public const int MONSTER_WAVE_DATA_INFO = 1104;
        public const int STAGE_COMPLETE_INFO = 1105;
        public const int STAGE_COMPLETE_RWARD = 1106;
        public const int STAGE_FIGHT_LOSE = 1107;//关卡失败
        public const int EVENT_HERO_HP_CHANGE = 1108;//角色血条变化

        public const int EVENT_ROLE_UPDATE = 100000; //角色信息更新
        public const int EVENT_USE_TREASURE_CHES_RES = 100001; //开箱结果
        public const int EVENT_PART_DECOMPOSE_RES = 100002; //装备分解
        public const int EVENT_PART_REPLACE_RES = 100003; //装备替换
        public const int EVENT_TREASURE_LEVELUP_RES = 100004; //宝箱升级
        public const int EVENT_EQUIP_CHANGE = 100005;//装备更换
        public const int EVENT_TREASURE_PROGRESS_UPSUCC = 100006;//宝箱进度购买
        public const int EVENT_SIGLE_EQUIP_WEAR = 100007;//装备穿上
        public const int EVENT_ROLE_Level_UPDATE = 100008; //角色等级更新
        public const int EVENT_ROLE_PART_UPDATE = 100009;//橘色身上部件替换更新
        public const int EVENT_TREASURE_CD_UPDATE = 100010;//宝箱升级cd
        public const int EVENT_ROLE_level_up = 100011; //角色升级
        public const int EVENT_LORE_EQUIP_REMOVE = 100012;//装备卸下
        public const int EVENT_LORE_EQUIP_EQUIP = 100013;//装备传承装备
        public const int EVENT_LOREEQUIP_UPDATE = 100014;
        public const int EVENT_GAIN_LORE_EQUIP = 100015; //获得新的传承装备
        
        //宠物相关
        public const int EVENT_ADDUNLOCK_PET_POS = 200000;//宠物位置解锁
        public const int EVENT_UPDATE_PET_INFO = 200001; //宠物数据更新
        public const int EVENT_UPLOAD_PET = 200002;//宠物上阵
        public const int EVENT_UPLOAD_SKILL = 200003;//技能上阵
        public const int EVENT_ADDUNLOCK_SKILL_POS = 200004;//技能位置解锁
        public const int EVENT_DELETE_PET_INFO = 200006; //宠物数据删除
        public const int EVENT_UPDATE_BATTLE_PET_LIST = 200010;//上阵宠物修改
        public const int EVENT_UPDATE_BATTLE_Skill_LIST_Lobby = 200011;//上阵技能UI修改
        public const int EVENT_UPDATE_PET_UNLOCK = 200012;//宠物位置服务器解锁
        public const int EVENT_UPDATE_PET_SC_SUCC = 200013;//宠物上阵服务器回包
        public const int EVENT_SWITCH_PET_SC_SUCC = 200014;//宠物阵上切换服务器回包
        public const int EVENT_PET_LEVELUP_SUCCESS = 200015;//宠物升级服务器回包
        public const int EVENT_PET_PIECECOMPOUND_SUCCESS = 200028;//宠物图鉴合成服务器回包
        public const int EVENT_UPDATE_SKILL_SC_SUCC = 200029;//上阵技能切换
        public const int EVENT_SKILL_LEVELUP_SUCCESS = 200030;//技能升级回包
        public const int EVENT_UPLOAD_RUNE = 200031;//符石上阵
        public const int EVENT_UPDATE_RUNE_SC_SUCC = 200032;//上阵符石切换
        public const int EVENT_ADDUNLOCK_RUNE_POS = 200033;//符石位置解锁
        public const int EVENT_CHANGE_HERO_TO_BATTLE = 200034;//切换英雄
        public const int EVENT_RECYCLE_SKILL_BOOK  = 200035;//回收宠物技能书
        public const int EVENT_RESET_PET_TELENT_INFO = 200036;//重置宠物天赋
        public const int EVENT_GET_PET_SKILLBOOK = 200037;//获取宠物技能书
        public const int EVENT_PETBOOKSLOT_UNLOCK = 200038;//解锁技能书槽位
        public const int EVENT_PETBOOK_UPDATE = 200039;//宠物技能书替换
        public const int EVENT_RECYCLE_PET = 200040;//回收宠物
        public const int EVENT_ATTR_CHANGE_UPATE_PET = 200041;//角色属性变化，更新宠物

        //角色相关
        public const int EVENT_ROLE_TRANSFER_SUCCESS = 300000;//角色转职
        public const int EVENT_UPDATE_BATTLE_HERO = 300001;//战场角色修改
        public const int EVENT_UPDATE_BATTLE_HERO_Lobby = 300002;//战场角色UI修改
        public const int EVENT_UPDATE_BATTLE_HERO_Attr = 300004;//修改角色属性
        
        //战场相关
        public const int EVENT_STAGE_COMPLETE_RECV_SUCCESS = 400000;//关卡stage通过回包
        public const int EVENT_STAGE_AWARD_RECV_SUCCESS = 400001;//关卡stage奖励回包
        
        public const int EVENT_CHOOSE_STAGE_RECV_CALLBACK = 400006;//选择关卡stage回调
        
        //商城
        public const int EVENT_PET_LOTTERY_SUCCESS = 500000;//宠物抽奖返回
        public const int EVENT_SKILL_LOTTERY_SUCCESS = 500001;//技能抽奖返回
        public const int EVENT_HERO_LOTTERY_SUCCESS = 500002;//英雄抽奖返回
        
        //任务
        public const int EVENT_TASK_UPDATE = 600000;//任务更新
        public const int EVENT_FUN_PREVIEW_UPDATE = 600001;//功能预告
        public const int EVENT_POWER_CHANGE = 600002;//战力变化
        public const int EVENT_POWER_CHANGE_Update_lobby = 600003;//战力变化更新
        public const int EVENT_DAILY_TASK_UPDATE = 600004;//每日任务刷新
        public const int EVENT_LOTTERY_START = 600005;//任务转盘开始转动
        public const int EVENT_TASK_GET_REWARD = 600006;//任务领取成功
        
        //道具相关
        public const int EVENT_ITEM_UPDATE = 700000;//道具刷新
        public const int EVENT_VILLAGE_ITEM_UPDATE = 700001;//家园道具刷新
        public const int EVENT_USE_ITEM_UPDATE = 700002;//使用道具刷新
        
        //副本
        public const int EVENT_GOTO_DUNGEON_MAP_STATE = 800000;//进入副本状态
        public const int EVENT_DUNGEON_STAGE_DATA_INFO = 800001;//副本信息
        public const int EVENT_DUNGEON_MONSTER_WAVE_DATA_INFO = 800002;//副本怪物信息
        public const int EVENT_DUNGEON_STAGE_UPDATE = 800003;//副本信息刷新
        public const int EVENT_DUNGEON_OPEN_STAGE_UPDATE = 800004;//副本界面打开
        public const int EVENT_DUNGEON_RANDOM_STAGE_DATA_INFO = 800005;//随机副本信息
        public const int EVENT_DUNGEON_FISHING_STAGE_DATA_INFO = 800006;//钓鱼副本信息
        
        //服务器红点
        public const int EVENT_REDPOINT_UPDATE = 900000;//红点更新
        //在线奖励刷新
        public const int EVENT_UPDATE_ONLINEAWARD = 900001;
        public const int EVENT_UPDATE_ONLINEAWARD_lobby = 900002;
        public const int EVENT_MAIL_LIST_UPDATE = 900003;//邮件更新
        public const int EVENT_CHANGENAMECOUNTER = 900004;//改名次数
        public const int EVENT_GO_TO_Lobby = 900005;//登录到大厅
        
        //家园
        public const int VILLAGE_CITY_UPLEVELSUCCESS = 1000000;//家园建筑升级
        public const int EVENT_VILLAGE_ALL_BUILD = 1000001;//所有家园建筑信息
        public const int EVENT_VILLAGE_ONE_BUILD = 1000002;//单个建筑信息更新
        public const int EVENT_VILLAGE_PET_REPLACE_PET = 1000003;//宠物家园建筑上阵替换

        public const int EVENT_VILLAGE_DISPATCH_PET_PC = 1000007;//宠物派遣服务器主动push
        public const int EVENT_VILLAGE_PET_ONCLICK = 1000009;//宠物点击
        public const int EVENT_UPDATE_VILLAGE_PET_SC_SUCC = 1000010;//家园宠物上阵服务器回包
        public const int EVENT_VILLAGE_PRODUCT_FULL = 1000011;//宠物家园建筑生产上限
        public const int EVENT_VILLAGE_GUIDE_PRODUCE_GETREWARD = 1000012;//家园建筑奖励领取新手引导

        //vip
        public const int EVENT_VIP_INFO_UPDATE = 1100000;//vip更新
        public const int EVENT_GIFTPACK_INFO_UPDATE = 1100001;//限时礼包
        public const int EVENT_RECHARGE_SUCCESS = 1100002;//充值成功

        //新手引导
        public const int EVENT_COMPLETE_GUIDE = 1200000;//新手引导消息接收
        public const int EVENT_COMPLETE_GUIDE_UPLOAD_PET = 1200001;//新手引导上阵宠物
        public const int EVENT_SHOW_FORCE_GUIDE = 1200002;//显示强制新手引导
        
        //聊天
        public const int EVENT_CHAT_UPDATE = 1300000;//聊天更新
        public const int EVENT_CHAT_UPDATE_Lobby = 1300001;//更新UI
        public const int EVENT_CHAT_UPDATE_Lobby_Red = 1300002;//更新UI
        
        //支付
        public const int EVENT_PAY_START = 1400000;//支付开始
        public const int EVNET_PAY_SUCCESS = 1400001;//支付成功
        public const int EVENT_WATCHAD_START = 1400010;//看广告开始
        public const int EVNET_WATCHAD_SUCCESS = 1400011;//看广告成功
        public const int EVENT_ACTIVITY_UPDATE = 1400002;//活动更新
        public const int EVENT_SEVENDAY_UPDATE = 1400003;//七日签到更新

        public const int EVENT_Refresh_xpskill = 1400004;//更新xpskill

        public const int EVENT_UPDATE_HEROInfo = 1500000;
        public const int EVENT_UPDATE_RuneSlotInfo = 1500001;//符石槽更新
        public const int EVENT_UPDATE_RuneInfo = 1500002;//符石更新
        public const int EVENT_UPDATE_HEROInfo_LevelUp = 1500003;//英雄升级更新
        public const int EVENT_UPDATE_SKILLInfo = 1500004;//技能更新
        public const int EVENT_UPDATE_SkillSlotInfo = 1500005;//英雄更新
        public const int EVENT_UPDATE_RingInfo = 1500006;//英雄更新
        public const int EVENT_UPDATE_FIGHT_INFO = 1500007;//战斗角色属性变更
        public const int EVENT_UPDATE_HEROInfo_Break = 1500008;//英雄突破更新
        public const int EVENT_WATCHADTOGET_HEROLEVELUP_Item = 1500009;//看广告拿升级道具
        public const int EVENT_UPDATE_HEROInfo_Merge = 1500010;//合成英雄
        public const int EVENT_UPDATE_BATTLE_SKILL_LIST = 1500011;//更新战场技能
        public const int EVENT_UPDATE_BATTLE_SKILL_CD = 1500012;//技能CD更新
        public const int EVENT_UPDATE_HeroSkillLV_INFO = 1500013;//技能等级更新
        
        public const int EVENT_HERO_MONTHACTIVITY_SUCCESS = 1600001;//英雄特权卡刷新
        public const int EVENT_CARD_ACTIVITY_SUCCESS = 1600002;//特权卡购买

        public const int EVENT_FIRST_PAY_UPDATE = 1700001;//首充礼包
        public const int EVENT_FIRST_PAY_UI_UPDATE = 1700002;//首充礼包领取刷新
        
        //通行证
        public const int EVENT_UPDATE_PASSPORT = 1800000;//通行证刷新
        public const int EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE = 1800001;//通行证解锁高级

        public const int EVENT_UPDATE_SHOP_REDDOT= 1900001;//商城红点刷新
        public const int EVENT_PASSPORT_GetSubTask = 1900002;//领取通行证子任务经验
        public const int EVENT_CRAZY_GUIDE_UPDATE = 1900003;//疯狂指南更新
        public const int EVENT_GUIDE_AWARD_GET = 1900004;//疯狂指南奖励领取
        public const int EVENT_CRAZY_GUIDE_PAY_SUCCESS = 1900005;//疯狂指南支付成功
        public const int EVENT_UPDATE_FREE_GETITEM = 1900007;//免费领取奖励刷新
        
        //登录礼包
        public const int EVENT_LOGINGIFT_UPDATE = 2000000;//登录礼包
        public const int EVENT_RECHARGE_GIFT_UPDATE = 2000001;//登录礼包
        
        //pvp
        public const int EVENT_PVP_UPDATE = 2100000;//pvp更新
        public const int EVENT_PVP_REPORT_UPDATE = 2100001;//战报
        public const int EVENT_PVP_FIGHT_COUNT_UPDATE = 2100002;//挑战次数
        public const int EVENT_PVP_GET_REWARD_UPDATE = 2100003;//领取pvp奖励

        public const int EVENT_AVATARS_UPDATE = 2200000;//头像
        public const int EVENT_CLASSIC_UPDATE = 2200001;//秘典
        
        //地图
        public const int EVENT_CHAPTER_MAP_DATA_UPDATE = 2300000;  //章节地图信息更新
        public const int EVENT_RANDOM_EVENTS_UPDATE = 2300001;  //随机事件更新
        public const int EVENT_NEXT_CHAPTER_UPDATE = 2300002;  //更新进入下一章地图
        public const int EVENT_DELEGATE_TASK_UPDATE = 2300003;  //委托任务更新
        public const int EVENT_Dorp_NPC_TASK = 2300004;  //放弃委托任务
        public const int EVENT_CLAIM_BUFF_UPDATE = 2300005;  //buff更新
        public const int EVENT_GAIN_BUFF_ANIMATION = 2300006;  //buff更新动画

        public const int EVENT_RANDOM_GOLD_EVENTS_UPDATE = 2300007;  //有新的天女散花金币堆
        public const int EVENT_RANDOM_GOLD_EVENTS_END = 2300008;  //天女散花金币堆消失
        public const int EVENT_CHAPTER_MAP_MOVE_STOP = 2300009;  // 宠物和BOSS打开挑战界面
		public const int EVENT_RANDOM_TREASURE_UPDATE = 2300010; //开箱子砍树更新
        public const int EVENT_RANDOM_TREASURE_END = 2300011; //开箱子砍树消失
        public const int EVENT_RANDOM_DIAMOND_EVENTS_UPDATE = 2300012;  //有新的钻石矿事件
        public const int EVENT_RANDOM_DIAMOND_EVENTS_END = 2300013;  //钻石矿事件消失
        public const int EVENT_RANDOM_EVENTS_DATA_UPDATE = 2300014;  //金币随机事件刷新
		public const int EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE = 2300015;  //大地图宠物和BOSS战斗客户端结束更新
        public const int EVENT_BOSS_PET_BATTLE_RESULT_UPDATE = 2300016;  //大地图宠物和BOSS战斗服务器结束更新
        public const int EVENT_BOSS_PET_AUTO_FIND_EVENT = 2300017;  // 点击自动寻找BOSS
        public const int EVENT_DELEGATE_TASK_POINTS_UPDATE = 2300018;//兑换积分更新
        public const int EVENT_DELEGATE_TASK_END = 2300019;//委托任务结束更新

        public const int EVENT_ADVENTURE_CAVE_SHOP = 2300020;//奇遇商店购买刷新
        public const int EVENT_PLAY_CATCH_PET_EVENTS = 2300021;  //大地图抓捕宠物
        public const int EVENT_BOSS_PET_CLICK_EVENTS = 2300022;  //大地图传承boss、宠物被点击
        
		public const int EVENT_RUIN_UPDATE = 2300022;//遗迹事件更新
        public const int EVENT_RUIN_END = 2300023;//遗迹事件结束
        public const int EVENT_AUTOEXIT_RUIN = 2300024;//自动退出遗迹地图
        public const int EVENT_END_RUINBOSS_FIGHT = 2300025;//挑战遗迹boss战斗结束
        public const int EVENT_RUIN_CLAIMBUFF_UPDATE = 2300026;//遗迹buff
        public const int EVENT_ENTER_AND_EXIT_RUIN =  2300027;

        public const int EVENT_ENTER_RANDOM_BOX_RESULT =  2300027;//深埋宝藏挖宝后刷新
        
        
        // 天赋
        public const int EVENT_TALENT_UPDATE = 2400001;
        public const int EVENT_ARTIFACT_UPDATE = 2400002;//神器
        
        //成就
        public const int EVENT_ACHIEVEMENT_UPDATE = 2500001;//成就更新
        public const int EVENT_GET_ACHIEVEMENT_INFO = 2500002;//获取成就信息

        public const int EVENT_UPDATE_HolyInfo = 2600001;//圣物更新
        public const int EVENT_UPLOAD_HOLY_UPDATE = 2600002;//圣物上阵
        public const int EVENT_UPDATE_BATTLE_HOLY_LIST = 2600003;// 圣物上阵列表更新
        public const int EVENT_ADDUNLOCK_HOLY_POS  = 2600004;// 圣物位置解锁
    }
}