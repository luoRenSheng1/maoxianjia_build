using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerAttrUtils
    {
        private static  Dictionary<ePlayerAttrID, double> playerAttr = new Dictionary<ePlayerAttrID, double>();
        /// <summary>
        //玩家基础数值
        // ePlayerAttrID_Level = 1;			// 等级	
        // ePlayerAttrID_Exp	= 2;			// 经验
        // ePlayerAttrID_VipLevel = 3;			// vip等级	
        // ePlayerAttrID_VipExp	= 4;			// vip经验
        // ePlayerAttrID_ChapterID	= 5;			// 章节 最后通过关卡所在章节---
        // ePlayerAttrID_StageID	= 6;			// 关卡 最后通过的关卡 ---
        // ePlayerAttrID_SubStageID	= 7;			// 关卡内第几波小怪 -关卡内第几波小怪 --有记录，挂机状态时客户端用
        // ePlayerAttrID_LatestPassedStageID	= 8;			// 最后通过的 stage_id  //判断首通奖励用
        // ePlayerAttrID_HeroID	= 9;			// 英雄ID，即职业ID
        // ePlayerAttrID_Gold = 10;			// 金币
        // ePlayerAttrID_Diamond = 11;		// 钻石
        // ePlayerAttrID_FreeDiamond = 12;		// 免费钻石--官方赠送
        // ePlayerAttrID_pet_lottery_level = 13;		// 宠物老虎机等级
        // ePlayerAttrID_pet_lottery_exp = 14;		// 宠物老虎机经验
        // ePlayerAttrID_equip_box_level = 15;		// 装备箱子等级
        // ePlayerAttrID_equip_box_exp = 16;		// 装备箱子经验
        // ePlayerAttrID_equip_box_lvlup_time = 17;		// 装备箱子下一次允许升级时间戳
        // ePlayerAttrID_pet_reborn_time = 18;		// 宠物下一次重生允许时间戳
        //ePlayerAttrID_OnlineAwardCountBegin = 20;     //在线奖励计时开始时间戳(以第一次登录时间做账户初始化，每次领取奖励重置)
       // ePlayerAttrID_ChangeNameCounter = 21;     //改名次数
       // ePlayerAttrID_OfflineAwardFreeTimes = 22;     //剩余在线免费观看广告奖励次数
       // ePlayerAttrID_OnlineAwardFreeTimes = 23;     //剩余离线线免费观看广告奖励次数
       // ePlayerAttrID_BoxAccFreeTimes = 24;     //装备宝箱剩余免费广告加速次数
        // ePlayerAttrID_hero_lottery_level = 25;		// 角色老虎机等级
        // ePlayerAttrID_hero_lottery_exp = 26;		// 角色老虎机经验
        // ePlayerAttrID_skill_lottery_level = 27;		// 技能老虎机等级
        // ePlayerAttrID_skill_lottery_exp = 28;		// 技能老虎机经验
        // ePlayerAttrID_Max = 29;		// 当前字段上限 -1就是字段数
        // ePlayerAttrID_StageStatus = 31; // 关卡状态 ---参见 eBattleStatus 枚举 为0表示正常战斗过关中、为1表示进入循环--挂机 、 -1 没有战斗--默认大地图、  -2;  //没有战斗--在营地
        // ePlayerAttrID_DailyFinishedRandomEvents = 49;   //今日完成随机任务数
        // ePlayerAttrID_PlayerMapStatus = 50;   // 不用舍弃了 参见 eChapterMapStatus  // 是挂机，还是过关  -1:没有战斗   0:正常过关  1:失败进入循环
        // ePlayerAttrID_ChapterIDOfMap = 51;   //最后所在地图对应的章节ID
        
        // ePlayerAttrID_npc_task_pints = 57;  //npc任务积分 
        /// </summary>
        /// <param name="playerAttrs"></param>
        public static void UpdatePlayerAttr(List<PlayerAttr> playerAttrs)
        {
            playerAttr.Clear();
            foreach (var t in playerAttrs)
            {
                playerAttr.Add(t.AttrTypeId, t.AttrValue);
            }

            RoleData roleData = DataManager.Instance.GetRoleData();
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_Level))
                roleData.lv = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_Level];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_Exp))
                roleData.exp = (long) playerAttr[ePlayerAttrID.ePlayerAttrID_Exp];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_VipLevel))
                roleData.vipLv = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_VipLevel];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_VipExp))
                roleData.vipExp = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_VipExp];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_ChapterID))
                roleData.chapterId = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_ChapterID];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_StageID))
                roleData.stageId = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_StageID];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_SubStageID))
                roleData.subStageId = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_SubStageID];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_LatestPassedStageID))
                roleData.latestPassedStageId = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_LatestPassedStageID];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_HeroID))
                roleData.heroId = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_HeroID];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_Gold))
                roleData.gold = playerAttr[ePlayerAttrID.ePlayerAttrID_Gold];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_Diamond))
                roleData.dia = (long) playerAttr[ePlayerAttrID.ePlayerAttrID_Diamond];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_FreeDiamond))
                roleData.dia2 = (long) playerAttr[ePlayerAttrID.ePlayerAttrID_FreeDiamond];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_UsedAvatar))
                roleData.avatarID = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_UsedAvatar];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_TalentPoints))
                roleData.talentPoints = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_TalentPoints];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_Shield))
                roleData.shield = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_Shield];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_Emblem))
                roleData.emblem = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_Emblem];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_Rune))
                roleData.rune = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_Rune];
            if (playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_sigil))
                roleData.sigil = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_sigil];
            
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_pet_lottery_level))
                roleData.petLotteryLv = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_pet_lottery_level];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_pet_lottery_exp))
                roleData.petLotteryExp = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_pet_lottery_exp];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_equip_box_level))
                roleData.equipBoxLv = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_equip_box_level];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_equip_box_exp))
                roleData.equipBoxExp = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_equip_box_exp];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_equip_box_lvlup_time))
                roleData.equipBoxLvUpTime = (ulong) playerAttr[ePlayerAttrID.ePlayerAttrID_equip_box_lvlup_time];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_rune_sold_condition))
                roleData.runeRecycleNum = (ulong) playerAttr[ePlayerAttrID.ePlayerAttrID_rune_sold_condition];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_OnlineAwardCountTotal))
                roleData.OnlineAwardCdTime = (ulong) playerAttr[ePlayerAttrID.ePlayerAttrID_OnlineAwardCountTotal];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_ChangeNameCounter))
                roleData.ChangeNameCounter = (uint) playerAttr[ePlayerAttrID.ePlayerAttrID_ChangeNameCounter];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_OfflineAwardFreeTimes))
                AdManager.Instance.SetAdFreeTime((int)ePlayerAttrID.ePlayerAttrID_OfflineAwardFreeTimes, (int) playerAttr[ePlayerAttrID.ePlayerAttrID_OfflineAwardFreeTimes]);
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes))
                AdManager.Instance.SetAdFreeTime((int)ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes, (int) playerAttr[ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes]);
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_BoxAccFreeTimes))
                AdManager.Instance.SetAdFreeTime((int)ePlayerAttrID.ePlayerAttrID_BoxAccFreeTimes, (int) playerAttr[ePlayerAttrID.ePlayerAttrID_BoxAccFreeTimes]);
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_HeroExpItemFreeTimes))
                AdManager.Instance.SetAdFreeTime((int)ePlayerAttrID.ePlayerAttrID_HeroExpItemFreeTimes, (int) playerAttr[ePlayerAttrID.ePlayerAttrID_HeroExpItemFreeTimes]);
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_skill_lottery_level))
                roleData.SkillLotteryLv = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_skill_lottery_level];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_skill_lottery_exp))
                roleData.SkillLotteryExp = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_skill_lottery_exp];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_FirstHeroLottoFree))
                roleData.HeroFreeLottery = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_FirstHeroLottoFree];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_PetLottoTimesByAd))
                AdManager.Instance.SetAdFreeTime((int)ePlayerAttrID.ePlayerAttrID_PetLottoTimesByAd,(int) playerAttr[ePlayerAttrID.ePlayerAttrID_PetLottoTimesByAd]);
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_SkillLottoTimesByAd))
                AdManager.Instance.SetAdFreeTime((int)ePlayerAttrID.ePlayerAttrID_SkillLottoTimesByAd, (int) playerAttr[ePlayerAttrID.ePlayerAttrID_SkillLottoTimesByAd]);
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_StageStatus))
                roleData.battleStatus =  (int) playerAttr[ePlayerAttrID.ePlayerAttrID_StageStatus];// > 0;
            
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_NewPlayerChargePack))
                roleData.IsFirstCharge =  (int) playerAttr[ePlayerAttrID.ePlayerAttrID_NewPlayerChargePack] > 0;
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_LoginGiftPackID))
                ActivityManager.Instance.LoginGiftPackID = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_LoginGiftPackID];
            
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_FreeMallDailyPack))
                ActivityManager.Instance.Shop_FreeDailyPack = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_FreeMallDailyPack];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_FreeMallSignPack))
                ActivityManager.Instance.Shop_FreeTTHLPack = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_FreeMallSignPack];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_FreeMallHeroCardPack))
                ActivityManager.Instance.Shop_FreeTQKPack = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_FreeMallHeroCardPack];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_HaveClaimedTodayPernamentAward))
                ActivityManager.Instance.HasGetPermanentTodayReward = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_HaveClaimedTodayPernamentAward];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_ClaimPernamentDays))
                ActivityManager.Instance.GetPermanentDays = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_ClaimPernamentDays];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_IsPernament))
                ActivityManager.Instance.IsBuyPermanent = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_IsPernament];
            
            //pvp
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_DailyGFPvpChallengerOpportunity))
                PvpRankDataManager.Instance.FightLeftCount = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_DailyGFPvpChallengerOpportunity];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_YesterdayGFRank))
                PvpRankDataManager.Instance.YestdayMyRank =  (int) playerAttr[ePlayerAttrID.ePlayerAttrID_YesterdayGFRank];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_YesterdayGFAwardHasClaimed))
                PvpRankDataManager.Instance.YestdayMyRankAwardGet =  (int) playerAttr[ePlayerAttrID.ePlayerAttrID_YesterdayGFAwardHasClaimed] >= 1;  
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_YesterdayGFPlayCounter))
                PvpRankDataManager.Instance.YesterdayGFPlayCounter =  (int) playerAttr[ePlayerAttrID.ePlayerAttrID_YesterdayGFPlayCounter];  
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_TodayGFPlayCounter))
                PvpRankDataManager.Instance.TodayGFPlayCounter =  (int) playerAttr[ePlayerAttrID.ePlayerAttrID_TodayGFPlayCounter];  
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_TodayPurchaseCounter))
                PvpRankDataManager.Instance.FightBuyCnt =  (int) playerAttr[ePlayerAttrID.ePlayerAttrID_TodayPurchaseCounter];

            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_DailyFinishedRandomEvents))
                roleData.DoneEventTaskNum = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_DailyFinishedRandomEvents];
            // if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_PlayerMapStatus))
            //     roleData.PlayerMapStatus = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_PlayerMapStatus];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_ChapterIDOfMap))
                roleData.lastStayMapChapterId = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_ChapterIDOfMap];
            if(playerAttr.ContainsKey(ePlayerAttrID.ePlayerAttrID_npc_task_points))
                roleData.npcTaskPoints = (int) playerAttr[ePlayerAttrID.ePlayerAttrID_npc_task_points];
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_ONLINEAWARD);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHANGENAMECOUNTER);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOGINGIFT_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CARD_ACTIVITY_SUCCESS);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_FIRST_PAY_UI_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_GET_REWARD_UPDATE);
            JsonObject jsObj = new JsonObject();
            jsObj["errCode"] = MsgCode.SUCCESS;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_LEVELUP_RES, MsgCode.SUCCESS, jsObj.ToString());
        }

        public static (List<int>, List<double>) GetAttrID_Value(List<EquipAttr> equipAttrs)
        {
            List<int> attrId = new List<int>(equipAttrs.Count);
            List<double> attrValue = new List<double>(equipAttrs.Count);
            foreach (var item in equipAttrs)
            {
                attrId.Add((int)item.AttrId);
                // attrValue.Add(item.AttrVal);
                
                eBattleAttr attrIdType = item.AttrId;
                if (attrIdType == eBattleAttr.eBattleAttr_PhysicAttack  // 物理伤害
                     || attrIdType == eBattleAttr.eBattleAttr_MagicAttack  // 魔法伤害
                     || attrIdType == eBattleAttr.eBattleAttr_SorceryAttack  // 道术伤害
                     || attrIdType == eBattleAttr.eBattleAttr_HP // 生命
                     || attrIdType == eBattleAttr.eBattleAttr_PhysicDefence  // 物理防御
                     || attrIdType == eBattleAttr.eBattleAttr_MagicDefence  // 魔法防御
                     || attrIdType == eBattleAttr.eBattleAttr_SorceryDefence  // 道术防御
                     || attrIdType == eBattleAttr.eBattleAttr_HP_Recovery // 生命恢复
                     || attrIdType == eBattleAttr.eBattleAttr_PetAtk  // //附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
                     || attrIdType == eBattleAttr.eBattleAttr_Parry_Value // 格挡值
                     || attrIdType == eBattleAttr.eBattleAttr_Hurt_HPRecovery // 普攻回复的生命具体值
                     || attrIdType == eBattleAttr.eBattleAttr_SkillAtkMultiple  // 技能伤害次数(倍数)
                     || attrIdType == eBattleAttr.eBattleAttr_OnlineStageAwaardMultiple  // 关卡挂机奖励次数
                     || attrIdType == eBattleAttr.eBattleAttr_HomeTownProduce_ExtraLimit  // 家园挂机时间上限
                     || attrIdType == eBattleAttr.eBattleAttr_KilledRecovery  // 消灭对象后HP回复
                     || attrIdType == eBattleAttr.eBattleAttr_FinalAttack // 攻击力
                     || attrIdType == eBattleAttr.eBattleAttr_FinalDefence) // 防御
                {
                    attrValue.Add(item.AttrVal);
                }
                else
                {
                    attrValue.Add(item.AttrVal * ConstDefine.CONFIG_PLACE_EX);
                }
            }
            return (attrId,attrValue);
        }
        
        public static (List<int>, List<int>, List<double>) GetEntryAttrID_Value(List<EntryAttr> equipAttrs)
        {
            List<int> entryIdList = new List<int>(equipAttrs.Count);
            List<int> attrIdList = new List<int>(equipAttrs.Count);
            List<double> attrValueList = new List<double>(equipAttrs.Count);
            foreach (var item in equipAttrs)
            {
                if (item.EntryId > 0)
                {
                    entryIdList.Add((int)item.EntryId);
                    eBattleAttr attrIdType = item.EquipAttrList[0].AttrId;
                    attrIdList.Add((int)attrIdType);
                    if (attrIdType == eBattleAttr.eBattleAttr_AtkSpeed_Rate)  // 攻速比率
                    {
                        float atkSpeed = (float)(item.EquipAttrList[0].AttrVal * ConstDefine.CONFIG_PLACE_EX);//(int) (1.0f / (item.EquipAttrList[0].AttrVal * ConstDefine.CONFIG_PLACE_EX) * 100) / 100f;
                        attrValueList.Add(atkSpeed);
                    }
                    // else if (attrIdType == eBattleAttr.eBattleAttr_Atk || attrIdType == eBattleAttr.eBattleAttr_HP || attrIdType == eBattleAttr.eBattleAttr_HP_Recovery || attrIdType == eBattleAttr.eBattleAttr_SkillAtkMultiple)
                    else if (attrIdType == eBattleAttr.eBattleAttr_PhysicAttack  // 物理伤害
                             || attrIdType == eBattleAttr.eBattleAttr_MagicAttack  // 魔法伤害
                             || attrIdType == eBattleAttr.eBattleAttr_SorceryAttack  // 道术伤害
                             || attrIdType == eBattleAttr.eBattleAttr_HP // 生命
                             || attrIdType == eBattleAttr.eBattleAttr_PhysicDefence  // 物理防御
                             || attrIdType == eBattleAttr.eBattleAttr_MagicDefence  // 魔法防御
                             || attrIdType == eBattleAttr.eBattleAttr_SorceryDefence  // 道术防御
                             || attrIdType == eBattleAttr.eBattleAttr_HP_Recovery // 生命恢复
                             || attrIdType == eBattleAttr.eBattleAttr_PetAtk  // //附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
                             || attrIdType == eBattleAttr.eBattleAttr_Parry_Value // 格挡值
                             || attrIdType == eBattleAttr.eBattleAttr_Hurt_HPRecovery // 普攻回复的生命具体值
                             || attrIdType == eBattleAttr.eBattleAttr_SkillAtkMultiple  // 技能伤害次数(倍数)
                             || attrIdType == eBattleAttr.eBattleAttr_OnlineStageAwaardMultiple  // 关卡挂机奖励次数
                             || attrIdType == eBattleAttr.eBattleAttr_HomeTownProduce_ExtraLimit  // 家园挂机时间上限
                             || attrIdType == eBattleAttr.eBattleAttr_KilledRecovery  // 消灭对象后HP回复
                             || attrIdType == eBattleAttr.eBattleAttr_FinalAttack // 攻击力
                             || attrIdType == eBattleAttr.eBattleAttr_FinalDefence) // 防御
                    {
                        attrValueList.Add(item.EquipAttrList[0].AttrVal);
                    }
                    else
                    {
                        attrValueList.Add(item.EquipAttrList[0].AttrVal * ConstDefine.CONFIG_PLACE_EX);
                    }
                }

            }
            return (entryIdList,attrIdList,attrValueList);
        }

        /// <summary>
        /// 统一调用，获取服务器道具
        /// </summary>
        /// <param name="ItemsList"></param>
        /// <param name="ret"></param>
        /// <param name="isVillage">是否家园道具</param>
        public static void GetItemDataWithGuid(List<ItemInfoWithGuid> ItemsList, ref List<ItemData> ret, bool isVillage = false)
        {
            foreach (var item in ItemsList)
            {
                ItemData itemData = new ItemData();
                itemData.id = (int) item.Id;
                itemData.count = (double) item.Num;
                itemData.expiredTime = item.ExpiredTime;
                itemData.ItemGuid = item.ItemGuid;
                ret?.Add(itemData);
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById((int) item.Id);
                if(itemTypeUnit == null)
                    continue;
                if (itemTypeUnit.Type == (int) eItemType.eItemType_Pet) //如果是整只宠物，则通过其他协议下发
                    continue;
                if(!isVillage)
                    ItemInfoManager.Instance.AddItemData(itemData);
                else
                {
                    itemData.count = item.Num;
                    ItemInfoManager.Instance.AddItemData(itemData);
                }
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
        }
        
        /// <summary>
        /// 统一调用，获取服务器道具
        /// </summary>
        /// <param name="ItemsList"></param>
        /// <param name="ret"></param>
        /// <param name="isVillage">是否家园道具</param>
        public static void GetItemData(List<ItemInfo> ItemsList, ref List<ItemData> ret, bool isVillage = false)
        {
            foreach (var item in ItemsList)
            {
                ItemData itemData = new ItemData();
                itemData.id = (int) item.Id;
                itemData.count = (double)Math.Floor(item.Num);
                itemData.expiredTime = item.ExpiredTime;
                itemData.ItemGuid = item.Guid;
                ret?.Add(itemData);
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById((int) item.Id);
                if(itemTypeUnit == null)
                    continue;
                if (itemTypeUnit.Type == (int) eItemType.eItemType_Pet) //如果是整只宠物，则通过其他协议下发
                    continue;
                if(!isVillage)
                    ItemInfoManager.Instance.AddItemData(itemData);
                else
                {
                    itemData.count = item.Num;
                    ItemInfoManager.Instance.AddItemData(itemData);
                }
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
        }
        
        public static CityInfo GetBuildInfoByServer(BuildInfo buildInfo)
        {
            CityInfo cityInfo = new CityInfo();
            cityInfo.BuildType = (VillageBuildType) buildInfo.Build;
            cityInfo.StartProduceTime = buildInfo.StartProduceTime;
            cityInfo.NextProduceTime = buildInfo.NextProduceTime;
            cityInfo.ItemNum = (double) buildInfo.ItemNum;
            cityInfo.PetIdList = buildInfo.PetsList.ToList();
            for (int i = 0; i < cityInfo.PetIdList.Count; i++)
            {
                ulong petGuid = cityInfo.PetIdList[i];
                if (petGuid > 0)
                {
                    PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet(petGuid);
                    if (petItemInfo != null)
                    {
                        petItemInfo.DispatchBuild = (VillageBuildType) buildInfo.Build;
                        petItemInfo.BuildInnerIndex = i;
                    }
                }

            }
            cityInfo.IsUnLock = buildInfo.StartProduceTime > 0;
            return cityInfo;
        }

        public static void UpdateFinance(Finance finance, bool isTips = false)
        {
            double gold = DataManager.Instance.mRoleData.gold;
            DataManager.Instance.mRoleData.gold = finance.Golds;
            if (DataManager.Instance.mRoleData.gold - gold > 0 && isTips)
            {
                TipsManger.Instance.ShowTip(ConfigUtils.FormatStringByKey(10151, StringUtils.FormatCurrency(DataManager.Instance.mRoleData.gold - gold)));
            }
            else if(DataManager.Instance.mRoleData.gold - gold < 0)
            {
                //TipsManger.Instance.ShowTip(ConfigUtils.FormatStringByKey(10152, Mathf.Abs(DataManager.Instance.mRoleData.gold - gold)));
            }

            long dia = DataManager.Instance.mRoleData.dia;
            DataManager.Instance.mRoleData.dia = finance.Diamonds;

            if (DataManager.Instance.mRoleData.dia - dia > 0 && isTips)
            {
                TipsManger.Instance.ShowTip(ConfigUtils.FormatStringByKey(10153, StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia - dia)));
            }
            else if(DataManager.Instance.mRoleData.dia - dia < 0)
            {
                //TipsManger.Instance.ShowTip(ConfigUtils.FormatStringByKey(10154, Mathf.Abs(DataManager.Instance.mRoleData.dia - dia)));
            }
            DataManager.Instance.mRoleData.dia2 = finance.FreeDiamonds;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);

            //Debug.Log($"旧金币={gold}，新金币={DataManager.Instance.mRoleData.gold}，旧钻石={dia}，新钻石={DataManager.Instance.mRoleData.dia}");
        }
        
        
        //缩短时间道具使用
        //     message ItemUse4Acceleration_CS { 			//eMsg_ItemUse4Acceleration_CS = 804 ; 
        //         optional ItemInfo used_items = 1;  //使用道具
        //         optional eAccelerationOp operation = 2;
        //         optional uint32 object_id = 3;  //如果是宝箱升级不用填；如果是建筑升级填eBuildType;如果是宠物派遣，填petID
	       //
        //         optional bool is_use_diamond = 4;  //如果使用钻石，以上参数不用填，钻石服务器端直接结束倒计时并扣钱
        // }
        /// <summary>
        //eAccelerationOp_EquipBoxLevelUp = 1;		// 装备宝箱升级
        // eAccelerationOp_BuildLevelUp = 2;		// 建筑物升级
        // eAccelerationOp_PetDispathching = 3;		// 宠物派遣
        // eAccelerationOp_CampExploring = 4;		// 探险营地
        // eAccelerationOp_FinalFactoryProducing = 5;		// 加工厂制造
        /// </summary>
        /// <param name="totalTime"></param>
        /// <param name="eAccelerationOp"></param>
        /// <param name="buildIdType"></param>
        public static void UseSpeedItem(int totalTime, eAccelerationOp eAccelerationOp, int buildIdType)
        {
            totalTime = Mathf.Max(0, totalTime);
            if (ItemInfoManager.Instance.GetItemData(ConstDefine.Item_SpeedCardId).count > 0)
            {
                UIManager.Instance.ShowUIPanel("UseSpeedItem",eAccelerationOp, totalTime, buildIdType);
            }
            else
            {
                int diamond = Mathf.CeilToInt(totalTime/300f) * 10;//5分钟10钻石
                MessageBoxView.MessageParam param = new MessageBoxView.MessageParam();
                param.OkCallBack = () =>
                {
                    if (DataManager.Instance.GetRoleData().dia < diamond)
                    {
                        UIManager.Instance.ToastByKey(10099);
                        return;
                    }
                    var builder = ItemUse4Acceleration_CS.CreateBuilder();
                    builder.IsUseDiamond = true;
                    builder.Operation = eAccelerationOp;
                    if (eAccelerationOp == eAccelerationOp.eAccelerationOp_BuildLevelUp)
                        builder.ObjectId = (uint) buildIdType;
                    else if (eAccelerationOp == eAccelerationOp.eAccelerationOp_CampExploring)
                        builder.ObjectId = (uint) buildIdType;
                    else if (eAccelerationOp == eAccelerationOp.eAccelerationOp_FinalFactoryProducing)
                        builder.ObjectId = (uint) buildIdType;
                    else if (eAccelerationOp == eAccelerationOp.eAccelerationOp_HomeTalentLevelUp)
                        builder.ObjectId = (uint) buildIdType;
                    else if (eAccelerationOp == eAccelerationOp.eAccelerationOp_PetReproducing)
                        builder.ObjectId = (uint) buildIdType;
                    ItemUse4Acceleration_CS use4AccelerationCs = builder.Build();
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ItemUse4Acceleration_CS, use4AccelerationCs);
                };
                UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.FormatStringByKey(10098, diamond), param, true);
            }
        }

        public static PassPortInfo GetPassPartInfoBySever(PassTask msgPassTask)
        {
            PassPortInfo passPortInfo = new PassPortInfo();
            passPortInfo.PassGiftUnits = ConfigUtils.GetPassGiftUnitsByType((int) msgPassTask.CfgId);
            passPortInfo.UnlockTier = msgPassTask.UnlockTier;
            passPortInfo.StartTime = msgPassTask.StartTime;
            passPortInfo.EndTime = msgPassTask.EndTime;
            passPortInfo.DailySubTasks.Clear();
            foreach (var item in msgPassTask.DailyTasksList)
            {
                passPortInfo.DailySubTasks.Add(new PassPortSubTask()
                {
                    TaskId = (int) item.TaskId,
                    PassSubTaskUnit = ConfigUtils.GetPassTaskById((int) item.TaskId),
                    Progress = (ulong) item.Process,
                    HasGetReward = item.HasClaimed
                });
            }
            passPortInfo.WeekSubTasks.Clear();
            foreach (var item in msgPassTask.WeeklyTasksList)
            {
                passPortInfo.WeekSubTasks.Add(new PassPortSubTask()
                {
                    TaskId = (int) item.TaskId,
                    PassSubTaskUnit = ConfigUtils.GetPassTaskById((int) item.TaskId),
                    Progress = (ulong) item.Process,
                    HasGetReward = item.HasClaimed
                });
            }

            passPortInfo.PassPortLv = msgPassTask.Level;
            passPortInfo.PassPortExp = msgPassTask.Exp;
            passPortInfo.NormalRewardGetLvs = msgPassTask.ClaimedPlainAwardsList.ToList();
            passPortInfo.SuperRewardGetLvs = msgPassTask.ClaimedAdvancedAwardsList.ToList();
            passPortInfo.CounterAfterTopLevel = msgPassTask.CounterAfterToplevel;
            passPortInfo.ClaimedCounterAfterTopLevel = msgPassTask.ClaimedCounterAfterToplevel;
            
            return passPortInfo;
        }
    }
}