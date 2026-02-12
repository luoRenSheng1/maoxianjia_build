using System;
using Config;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EngineBase;
using Google.Protobuf.Collections;
using msg;

namespace Engine
{
    /// <summary>
    /// 全局配置ID枚举
    /// </summary>
    public enum EN_GLOBAL
    {
        LOD_ARRAY = 10000001,               // LOD等级
        NEWBIE_GUIDE_ID_START = 900000000,  // 新手引导活物id
        NEWBIE_GUIDE_ID_END = 900001000,
        DEFAULT_MOVE_SPEED = 20000000,
        DEFAULT_SIGNPOSTS_EFFECT_ID = 20000001,
        DEFAULT_CAMERA_ROT = 20000003,
        DEFAULT_BATTLE = 20000004,
        DEFAULT_FOG = 20000007,
        MAIN_TASK_ARROW_GUIDE_DIS = 900040,//主线任务引导箭头距离判断
        WARRIO_RATTACT_DIS = 401090,
        ARCHER_ATTACT_DIS = 401100,
    }
    
    public static class ConfigUtils
    {
        public static void ClearAll()
        {
        }

        // 初始化配置参数
        public static void InitConfigParam()
        {
            var cfg = GetGlobalById((int)EN_GLOBAL.LOD_ARRAY);
            ConstDefine.InitConfigParam();
        }

        public static string GetStringByKey(int nKey)
        {
            string name = GameManager.Instance.GetStringCsLanguageById(nKey);
            if (name != null)
            {
                string strValue = name;//cfg.Text;
                strValue = strValue.Replace("\\n", "\n");
                //Debug.Log(nKey);
                //Debug.Log(strValue);
                return strValue;
            }

#if UNITY_EDITOR
            return string.Format("key{0}", nKey);
#else
            return "";
#endif
        }

        public static string FormatStringByKey(int nKey, params object[] args)
        {
            string name = GameManager.Instance.GetStringCsLanguageById(nKey);
            if (name != null)
            {
                string strValue = name;//cfg.Text;
                strValue = strValue.Replace("\\n", "\n");
                return string.Format(strValue, args);
            }

#if UNITY_EDITOR
            return string.Format("key{0}", nKey);
#else
            return "";
#endif
        }

        public static string GetPetQuotesByKey(int nKey)
        {
            string name = GameManager.Instance.GetPetQuotesCsLanguageById(nKey);
            if (name != null)
            {
                string strValue = name;
                strValue = strValue.Replace("\\n", "\n");
                return strValue;
            }
            
#if UNITY_EDITOR
            return string.Format("key{0}", nKey);
#else
            return "";
#endif
        }

        public static List<Vector3> GetVector3ByConfigString(string strValue)
        {
            if (!string.IsNullOrEmpty(strValue))
            {
                string[] strLines = strValue.Split(';');

                if (strLines != null)
                {
                    List<Vector3> lstVector = new List<Vector3>();

                    foreach (var line in strLines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] strLineItems = line.Split(',');

                            if (strLineItems != null)
                            {
                                if (strLineItems.Length == 2)
                                {
                                    lstVector.Add(new Vector3(Utils.GetFloat(strLineItems[0]), 0, Utils.GetFloat(strLineItems[1])));
                                }
                                else if (strLineItems.Length == 3)
                                {
                                    lstVector.Add(new Vector3(Utils.GetFloat(strLineItems[0]), Utils.GetFloat(strLineItems[1]), Utils.GetFloat(strLineItems[2])));
                                }
                            }
                        }
                    }

                    return lstVector;
                }
            }

            return null;
        }

        public static List<Vector2Int> GetVector2ByConfigString(string strValue)
        {
            if (!string.IsNullOrEmpty(strValue))
            {
                string[] strLines = strValue.Split('|');

                if (strLines != null)
                {
                    List<Vector2Int> lstVector = new List<Vector2Int>();

                    foreach (var line in strLines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] strLineItems = line.Split(':');

                            if (strLineItems != null)
                            {
                                if (strLineItems.Length >= 2)
                                {
                                    lstVector.Add(
                                        new Vector2Int(Utils.GetInt(strLineItems[0]), Utils.GetInt(strLineItems[1])));
                                }
                            }
                        }
                    }

                    return lstVector;
                }
            }

            return null;
        }
        
        /// <summary>
        /// 是否技能循环播放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsSkillEffectLoop(int id)
        {
            ConfigSkillEffectUnit cfgSkillEffect = GetSkillEffectById(id);

            if (cfgSkillEffect != null)
            {
                return cfgSkillEffect.Time == 0;
            }

            return false;
        }

        public static string GetHeroModelPathByID(int id)
        {
            var hero = GetHeroById(id);
            return hero != null ? hero.Model : "";
        }
        
        public static string GetHeroModelUIPathByID(int id)
        {
            var hero = GetHeroById(id);
            return hero != null ? hero.Model1 : "";
        }
        
        public static string GetPetModelPathByID(int id)
        {
            var pet = GetPetById(id);
            return pet != null ? pet.PetModel : "";
        }
        
        public static string GetMonsterModelPathByID(int id)
        {
            var hero = GetMonsterById(id);
            return hero != null ? hero.Model : "";
        }
        
        // 改成这个 GetStringCsLanguageById
        // public static ConfigStringUnit GetStringCsById(int id)
        // {
        //     return ConfigDataGroup.GetInstance<ConfigString>().Get(id);
        // }

        public static ConfigGlobalUnit GetGlobalById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigGlobal>().Get(id);
        }

        public static ConfigHeroUnit GetHeroById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigHero>().Get(id);
        }
        
        public static ConfigHeroUnit GetHeroBySkillId(int skillId)
        {
            foreach (var cfg in ConfigDataGroup.GetInstance<ConfigHero>().Data)
            {
                if((int)cfg.Value.ActiveSkill == (int)skillId)
                {
                    return cfg.Value;
                }
            }
            return null;
        }
        
        public static ConfigPetBasisUnit GetPetById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigPetBasis>().Get(id);
        }

        public static string GePetModelPathByID(int id)
        {
            var pet = GetPetById(id);
            return pet != null ? pet.PetModel : "";
        }
        
        public static ConfigMonsterUnit GetMonsterById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigMonster>().Get(id);
        }

        /// <summary>
        /// 存在多个groupId相同
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static List<ConfigMonsterGroupUnit> GetMonsterGroupById(int groupId)
        {
            List<ConfigMonsterGroupUnit> list = new List<ConfigMonsterGroupUnit>();
            MapField<int, ConfigMonsterGroupUnit> arr = ConfigDataGroup.GetInstance<ConfigMonsterGroup>().Data;
            foreach (var cfg in arr)
            {
                if(cfg.Value.GroupId == groupId)
                {
                    list.Add(cfg.Value);
                }
            }
            return list;
        }
        
        public static ConfigBubbleUnit GetBubbleById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigBubble>().Get(id);
        }
        
        public static ConfigEventUnit GetEventDataById(int eventId)
        {
            return ConfigDataGroup.GetInstance<ConfigEvent>().Get(eventId);
        }

        public static ConfigEventStageUnit GetEventStageUnitById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigEventStage>().Get(id);
        }
        
        public static ConfigEventUnit GetEventUnitByType(int eventType)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigEvent>().Data.Values)
            {
                if (item.Type == eventType)
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取奇遇商店表配置数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ConfigEventTraderUnit GetEventTraderById(int id)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigEventTrader>().Data.Values)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        public static ConfigRuinsBuffUnit GetRuinsBuffDataById(int ruinsId)
        {
            foreach (var cfg in ConfigDataGroup.GetInstance<ConfigRuinsBuff>().Data)
            {
                if((int)cfg.Value.Id == (int)ruinsId)
                {
                    return cfg.Value;
                }
            }
            return null;
        }
        
        public static ConfigBossTraitUnit GetBossTraitById(int id)
        {
            foreach (var cfg in ConfigDataGroup.GetInstance<ConfigBossTrait>().Data)
            {
                if((int)cfg.Value.Id == (int)id)
                {
                    return cfg.Value;
                }
            }
            return null;
        }
        
        public static ConfigSkillUnit GetSkillById(int id)
        {
            ConfigSkillUnit skillUnit = ConfigDataGroup.GetInstance<ConfigSkill>().Get(id);
            if (skillUnit == null)
            {
                LogUtils.LogErrorFormat("Skill找不到技能信息id={0}",id);
            }

            return skillUnit;
        }

        public static ConfigSkillBuffUnit GetSkillBuffById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigSkillBuff>().Get(id);
        }

        public static ConfigSkillBuffUnit GetSkillBuffByBuffId(int buffId)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigSkillBuff>().Data)
            {
                if (item.Value.BuffId == buffId)
                    return item.Value;
            }
            return null;
        }
        
        public static ConfigSkillAchieveUnit GetSkillAchieveById(int Id)
        {
            return ConfigDataGroup.GetInstance<ConfigSkillAchieve>().Get(Id);;
        }
        
        public static ConfigBuffActionTemplateUnit GetBuffActionByBuffId(int Id)
        {
            return ConfigDataGroup.GetInstance<ConfigBuffActionTemplate>().Get(Id);;
        }

        public static ConfigSkillEffectUnit GetSkillEffectById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigSkillEffect>().Get(id);
        }
        
        public static string GetSkillEffectPathById(int id)
        {
            ConfigSkillEffectUnit skillEffectUnit = ConfigDataGroup.GetInstance<ConfigSkillEffect>().Get(id);
            if (skillEffectUnit != null)
            {
                return skillEffectUnit.Path;
            }
            LogUtils.LogErrorFormat("SkillEffect找不到技能特效信息id={0}",id);
            return "";
        }
        
        public static ConfigSkillTargetUnit GetSkillTargetById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigSkillTarget>().Get(id);
        }

        public static ConfigTreasureChestUnit GetTreasureChestUnitById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigTreasureChest>().Get(id);
        }
        
        public static MapField<int,ConfigTreasureChestUnit> GetConfigTreasureChestUnits()
        {
            return ConfigDataGroup.GetInstance<ConfigTreasureChest>().Data;
        }
        
        public static MapField<int,ConfigItemTypeUnit> GetConfigItemTypeUnits()
        {
            return ConfigDataGroup.GetInstance<ConfigItemType>().Data;
        }
        
        public static ConfigItemTypeUnit GetConfigItemTypeUnitById(int id)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigDataGroup.GetInstance<ConfigItemType>().Get(id);
            if (itemTypeUnit == null)
            {
                LogUtils.LogErrorFormat("该道具在ItemType配置表里找不到,id={0}", id);
            }
            return itemTypeUnit;
        }
        
        public static ConfigItemTypeUnit GetConfigItemTypeByParam(int param)
        {
            // List<ConfigItemTypeUnit> itemTypeUnits = new List<ConfigItemTypeUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigItemType>().Data)
            {
                if (item.Value.Param == param)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static ConfigLoreEntryUnit GetConfigLoreEntryUnitById(int id)
        {
            ConfigLoreEntryUnit loreEntryUnit = ConfigDataGroup.GetInstance<ConfigLoreEntry>().Get(id);
            if (loreEntryUnit == null)
            {
                LogUtils.LogErrorFormat("在LoreEntry配置表里找不到,id={0}", id);
            }
            return loreEntryUnit;
        }
        
        public static List<ConfigLoreEntryQualityUnit> GetLoreEntryQualityUnitsList()
        {
            List<ConfigLoreEntryQualityUnit> tempList = new List<ConfigLoreEntryQualityUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigLoreEntryQuality>().Data)
            {
                // if (item.Value.LevelId == id)
                // {
                    tempList.Add(item.Value);
                // }
            }

            return tempList;
        }
        
        /// <summary>
        /// 通过表ID 获得数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ConfigStageMonsterAttrUnit GetStageMonsterAttrUnitByIndexId(int id)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigStageMonsterAttr>().Data)
            {
                if (item.Value.Id == id)
                {
                    return item.Value;
                }
            }

            return null;
        }
        
        /// <summary>
        /// 怪物buff词条
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static List<ConfigMonsterEntryUnit> GetMonsterEntryByGroup(int group)
        {
            List<ConfigMonsterEntryUnit> tempList = new List<ConfigMonsterEntryUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigMonsterEntry>().Data)
            {
                if (item.Value.EntryGroup == group)
                {
                    tempList.Add(item.Value);
                }
            }

            return tempList;
        }
        
        public static List<ConfigStageUnit> GetStageUnitById(int id)
        {
            List<ConfigStageUnit> tempList = new List<ConfigStageUnit>();
            var config = ConfigDataGroup.GetInstance<ConfigStage>();
            if (config != null)
            {
                foreach (var item in config.Data)
                {
                    if (item.Value.LevelId == id)
                    {
                        tempList.Add(item.Value);
                    }
                }
            }

            return tempList;
        }
        
        public static ConfigStageUnit GetStageUnitByIdAndNode(int id, int node)
        {
            var config = ConfigDataGroup.GetInstance<ConfigStage>();
            if(config != null)
            {
                foreach (var item in ConfigDataGroup.GetInstance<ConfigStage>().Data)
                {
                    if (item.Value.LevelId == id && item.Value.Node == node+1)
                    {
                        return item.Value;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// 通过表ID 获得数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ConfigStageUnit GetStageUnitByIndexId(int id)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigStage>().Data)
            {
                if (item.Value.Id == id)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static List<ConfigStageUnit> GetStageUnitByChapterId(int chapterId)
        {
            List<ConfigStageUnit> tempList = new List<ConfigStageUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigStage>().Data)
            {
                if (item.Value.Chapter == chapterId)
                {
                    tempList.Add(item.Value);
                }
            }

            return tempList;
        }
        
        public static ConfigChapterUnit GetChapterUnitById(int chaperId)
        {
            var chapterUnit = ConfigDataGroup.GetInstance<ConfigChapter>().Get(chaperId);
            if (chapterUnit == null)
            {
                LogUtils.LogErrorFormat("chapter配置表找不到id:{0}", chaperId);
                return null;
            }
            return chapterUnit;
        }

        public static ConfigAttrEnumerationUnit GetAttrEnumerationById(int id)
        {
            var attrEnumerationUnit = ConfigDataGroup.GetInstance<ConfigAttrEnumeration>().Get(id);
            if (attrEnumerationUnit == null)
            {
                LogUtils.LogErrorFormat("AttrEnumeration配置表找不到id:{0}", id);
                return null;
            }
            return attrEnumerationUnit;
        }

        public static ConfigPetLevelUnit GetPetLevelByQualityWithLevel(int quality, int level)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPetLevel>().Data)
            {
                if (item.Value.Quality == quality)
                {
                    if (item.Value.PetLevel == level)
                    {
                        return item.Value;
                    }
                }
            }
            return null;
        }

        public static ConfigPetSkillLevelUnit GetPetSkillLevelUnitBySkillIdWithLevel(int skillId, int level)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPetSkillLevel>().Data)
            {
                if (item.Value.SkillId == skillId && item.Value.Lv == level)
                {
                        return item.Value;
                }
            }
            LogUtils.LogWarningFormat("找不到SkillLevel数据:skillId={0}  level={1}  可能是达到最大值了", skillId, level);
            return null;
        }
        
        //TODO 多语言  后期要添加其它语种
        public static string GetTextById(string textId,string Params = "")
        {
            string name = GameManager.Instance.GetTextNameByIdWithIndex(textId, GameManager.Instance.languageIndex);
            if (Params != "")
            {
                string[] paramArr = Params.Split("|");
                name = string.Format(name, paramArr.Cast<object>().ToArray());
            }
            
            return name;
            // return ConfigDataGroup.GetInstance<ConfigMultiLang>().Get(curTaskId);
        }
        //TODO 气泡多语言  后期要添加其它语种
        public static string GetBubbleTextById(int textId, string Params = "")
        {
            ConfigBubbleUnit bubble = ConfigUtils.GetBubbleById(textId);
            return ConfigUtils.GetTextById(bubble.Doc, Params);
        }

        public static string GetTextById(string textId,List<string> Params)
        {
            string name = GameManager.Instance.GetTextNameByIdWithIndex(textId, GameManager.Instance.languageIndex);
            if (Params.Count > 0)
            {
                string str = "";
                foreach (var item in Params)
                {
                    str += item + "|";
                }
                if (str != "")
                {
                    str = str.TrimEnd('|');
                }
                string[] paramArr = str.Split("|");
                name = string.Format(name, paramArr.Cast<object>().ToArray());
            }
            return name;
        }
        
        public static ConfigTaskUnit GetTaskById(int curTaskId)
        {
            return ConfigDataGroup.GetInstance<ConfigTask>().Get(curTaskId);
        }
        
        public static ConfigTaskGuideUnit GetTaskGuideById(int Conduct)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigTaskGuide>().Data)
            {
                if (item.Value.Conduct == Conduct)
                {
                    return item.Value;
                }
            }
        
            return null;
        }
        
        public static ConfigDungeonStageUnit GetDungeonStageById(int id, int dungeonType)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigDungeonStage>().Data)
            {
                if (item.Value.Id == id && item.Value.Type == dungeonType)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static ConfigDungeonStageUnit GetDungeonStageByNandu(int stage, int dungeonType)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigDungeonStage>().Data)
            {
                if (item.Value.Stage == stage && item.Value.Type == dungeonType)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static int GetDungeonStageMaxByType(int type)
        {
            List<ConfigDungeonStageUnit> tempList = new List<ConfigDungeonStageUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigDungeonStage>().Data)
            {
                if (item.Value.Type == type)
                {
                    tempList.Add(item.Value);
                }
            }

            return tempList.Count;
        }
        
        public static ConfigDungeonChapterUnit GetDungeonChapterById(DungeonType type)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigDungeonChapter>().Data)
            {
                if (item.Value.Type ==(int) type)
                {
                    return item.Value;
                }
            }

            return null;
        }

        private static Dictionary<int, int> _dungeonCntDict;
        public static int GetDungeonTotalCount(int type)
        {
            if (_dungeonCntDict == null)
            {
                _dungeonCntDict = new Dictionary<int, int>(8);
                ConfigCommonUnit commonUnit = ConfigDataGroup.GetInstance<ConfigCommon>().Get(20);
                string[] dungeonIdArr = commonUnit.Param1.Split('|');
                string[] dungeonCntArr = commonUnit.Param2.Split('|');
                for (int i = 0; i < dungeonIdArr.Length; i++)
                {
                    _dungeonCntDict.Add(int.Parse(dungeonIdArr[i]), int.Parse(dungeonCntArr[i]));
                }
            }
            
            return _dungeonCntDict[type];
        }
        
        private static Dictionary<int, int> _dungeonCntAdDict;
        public static int GetDungeonAdTotalCount(int type)
        {
            if (_dungeonCntAdDict == null)
            {
                _dungeonCntAdDict = new Dictionary<int, int>(8);
                ConfigCommonUnit commonUnit = ConfigDataGroup.GetInstance<ConfigCommon>().Get(21);
                string[] dungeonIdArr = commonUnit.Param1.Split('|');
                string[] dungeonCntArr = commonUnit.Param2.Split('|');
                for (int i = 0; i < dungeonIdArr.Length; i++)
                {
                    _dungeonCntAdDict.Add(int.Parse(dungeonIdArr[i]), int.Parse(dungeonCntArr[i]));
                }
            }
            
            return _dungeonCntAdDict[type];
        }

        public static ConfigBuildUnit GetBuildUnitByType(VillageBuildType type)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigBuild>().Data)
            {
                if (item.Value.BuildType ==(int) type)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static ConfigSystemUnit GetFunPreInfo(int funcId)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigSystem>().Data)
            {
                if (item.Value.Id == funcId)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static List<ConfigSystemUnit> GetFunPreUnitsByPosition(int position)
        {
            List<ConfigSystemUnit> systemUnitList = new List<ConfigSystemUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigSystem>().Data.Values)
            {
                if (item.Position == position)
                {
                    systemUnitList.Add(item);
                }
            }
            
            return systemUnitList;
        }

        public static ConfigPetQuotesUnit GetPetQuotesUnitById(int quotesId)
        {
            return ConfigDataGroup.GetInstance<ConfigPetQuotes>().Get(quotesId);
        }

        public static ConfigHomePetAttrContrastUnit GetHomePetAttrContrastUnit(int type, int vigour)
        {
            vigour = Mathf.Max(1, vigour);
            vigour = Mathf.Min(100, vigour);
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHomePetAttrContrast>().Data)
            {
                if (item.Value.Type == type && item.Value.Min <= vigour && item.Value.Max >= vigour)
                {
                    return item.Value;
                }
            }

            return null;
        }
        
        public static List<ConfigFailurePromptUnit> GetFailurePromptUnits()
        {
            return ConfigDataGroup.GetInstance<ConfigFailurePrompt>().Data.Values.ToList();
        }

        public static ConfigDailyTaskUnit GetDailyTaskUnit(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigDailyTask>().Get(id);
        }

        public static ConfigDailyCheckRewardUnit GetSevenDayUnit(int count)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigDailyCheckReward>().Data)
            {
                if (item.Count == count)
                    return item;
            }

            return null;
        }

        public static double GetRingStrengthCost(int lv, ShuxingType type)
        {
            ConfigRingUnit ringUnit = ConfigDataGroup.GetInstance<ConfigRing>().Get(lv);
            double cost = 0;
            int maxLevel = ConfigDataGroup.GetInstance<ConfigRing>().Data.Count;
            if(lv <= maxLevel)
                cost = ringUnit.Consume;
            else
            {
                ConfigCommonUnit common200006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(200006);
                ConfigRingUnit ringMaxUnit = ConfigDataGroup.GetInstance<ConfigRing>().Get(maxLevel);
                cost = Math.Ceiling(ringMaxUnit.Consume * Math.Pow(1+int.Parse(common200006.Param2)*ConstDefine.CONFIG_PLACE_EX, lv-maxLevel));
            }

            return cost;
        }

        public static List<ConfigHeroAttrUnit> GetHeroAttrsByHeroId(int heroId)
        {
            List<ConfigHeroAttrUnit> heroAttrUnits = new List<ConfigHeroAttrUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroAttr>().Data)
            {
                if (item.Value.HeroId == heroId && item.Value.Level <= 100 && item.Value.Type == 1)
                {
                    heroAttrUnits.Add(item.Value);
                }
            }

            return heroAttrUnits;
        }
        
        public static List<ConfigHeroAttrUnit> GetHeroBreakAttrsByHeroId(int heroId)
        {
            List<ConfigHeroAttrUnit> heroAttrUnits = new List<ConfigHeroAttrUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroAttr>().Data)
            {
                if (item.Value.HeroId == heroId && item.Value.Level >= 100 && item.Value.Type == 2)
                {
                    heroAttrUnits.Add(item.Value);
                }
            }

            return heroAttrUnits;
        }
        
        public static (double,int) GetHeroAttrsByBreakLevel(int heroId, int breakLevel)
        {
            double attrValue = 0;
            int attrId = 0;
            List<ConfigHeroAttrUnit> heroAttrUnits = new List<ConfigHeroAttrUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroAttr>().Data)
            {
                if (item.Value.HeroId == heroId && item.Value.Type == 2 && item.Value.Level <= breakLevel)
                {
                    attrValue += item.Value.Value;
                }

                if (item.Value.HeroId == heroId && item.Value.Type == 2)
                {
                    attrId = item.Value.AttrId;
                }
            }

            return (attrValue, attrId);
        }

        public static ConfigHeroLevelUnit GetHeroLevel(int heroId, int lv)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroLevel>().Data)
            {
                if (item.Value.HeroId == heroId && item.Value.Level == lv)
                    return item.Value;
            }

            return null;
        }
        
        public static double GetHeroSkillDamage(int heroId, int level)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroSkill>().Data)
            {
                if (item.Value.Hero == heroId && item.Value.SkillLevel == level)
                    return item.Value.SkillDamage;
            }
        
            return 0;
        }

        public static ConfigHeroSkillUnit GetHeroSkillByHeroIdAndSkillLv(int heroId, int lv)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroSkill>().Data)
            {
                if (item.Value.Hero == heroId && item.Value.SkillLevel == lv)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static ConfigHeroSkillUnit GetHeroNextSKillUnitByHeroIdAndSkillLv(int heroId, int skillLv)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroSkill>().Data)
            {
                if (item.Value.Hero == heroId && (item.Value.SkillLevel == skillLv + 1))
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static ConfigHeroSkillUnit GetHeroSKillMaxLvByHeroId(int heroId)
        {
            var skillData = ConfigDataGroup.GetInstance<ConfigHeroSkill>()?.Data?.Values;
            if (skillData == null)
                return null;
            
            ConfigHeroSkillUnit maxSkillUnit = null;
            foreach (var skill in skillData)
            {
                if (skill.Hero == heroId && (maxSkillUnit == null || skill.SkillLevel > maxSkillUnit.SkillLevel))
                {
                    maxSkillUnit = skill;
                }
            }
            return maxSkillUnit;
        }

        public static ConfigSkillLevelUnit GetSkillLevelUnit(int skillId, int level)
        {
            //SkillLevel表的ID规则=skillId（宠物组）*1000+Level（等级）- 25000000000
            int key = (int) ((skillId * 1000 + level) - 25000000000);
            ConfigSkillLevelUnit skillLevelUnit = ConfigDataGroup.GetInstance<ConfigSkillLevel>().Get(key);
            if(skillLevelUnit == null)
                LogUtils.LogWarningFormat("找不到SkillLevel数据:skillId={0}  level={1}  可能是达到最大值了", skillId, level);
            return skillLevelUnit;
        }

        public static ConfigRaffleLevelUnit GetRaffleUnit(int lv, int type)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigRaffleLevel>().Data)
            {
                if (item.Value.Level == lv && item.Value.Type == type)
                {
                    return item.Value;
                }
            }

            return null;
        }
        
        public static List<ConfigRaffleLevelUnit> GetRaffleUnits(int type)
        {
            List<ConfigRaffleLevelUnit> units = new List<ConfigRaffleLevelUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigRaffleLevel>().Data)
            {
                if (item.Value.Type == type)
                {
                    units.Add(item.Value);
                }
            }

            return units;
        }

        public static ConfigMonthlyUnit GetMonthlyUnit(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigMonthly>().Get(id);
        }

        public static List<ConfigGiftUnit> GetGiftUnitsByType(GiftType type)
        {
            List<ConfigGiftUnit> units = new List<ConfigGiftUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigGift>().Data)
            {
                if (item.Value.GiftType == (int) type)
                {
                    units.Add(item.Value);
                }
            }

            return units;
        }
        
        public static ConfigGiftUnit GetGiftUnitsById(int giftId)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigGift>().Data)
            {
                if (item.Value.Id == (int) giftId)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static List<ConfigPayListUnit> GetPayListUnitsByType(int type=1)
        {
            List<ConfigPayListUnit> units = new List<ConfigPayListUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPayList>().Data)
            {
                if (item.Value.Type == (int) type)
                {
                    units.Add(item.Value);
                }
            }

            return units;
        }

        public static List<ConfigPassGiftUnit> GetPassGiftUnitsByType(int stageLevelId)
        {
            List<ConfigPassGiftUnit> units = new List<ConfigPassGiftUnit>();
            
            //加一个空的unit：//适配UI效果
            ConfigPassGiftUnit nullUnit = new ConfigPassGiftUnit();
            nullUnit.Id = 0;
            nullUnit.Level = 0;
            nullUnit.Score = 0;
            nullUnit.Gift = "";
            nullUnit.FancyGift = "";
            nullUnit.StageLevelId = 0;
            units.Add(nullUnit);
            
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPassGift>().Data)
            {
                if (item.Value.StageLevelId == (int) stageLevelId)
                {
                    units.Add(item.Value);
                }
            }

            return units;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">0=默认开  1=高级开</param>
        /// <returns></returns>
        public static List<ConfigPassTaskUnit> GetPassTaskByType(int type)
        {
            List<ConfigPassTaskUnit> units = new List<ConfigPassTaskUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPassTask>().Data)
            {
                if (item.Value.PayType == (int) type)
                {
                    units.Add(item.Value);
                }
            }

            return units;
        }
        
        public static ConfigPassTaskUnit GetPassTaskById(int taskId)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPassTask>().Data)
            {
                if (item.Value.Id == (int) taskId)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static ConfigLoginGiftUnit GetLoginGiftById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigLoginGift>().Get(id);
        }

        public static float GetHeroOutTime()
        {
            return int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(200).Param1)/1000f;
        }

        public static List<ConfigHelpUnit> GetHelpUnitsByType(HelpType type)
        {
            List<ConfigHelpUnit> units = new List<ConfigHelpUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHelp>().Data)
            {
                if (item.Value.Type == (int) type)
                {
                    units.Add(item.Value);
                }
            }

            return units;
        }

        public static ConfigFirstRewardUnit GetPvpRewardUnitByRank(int rank)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigFirstReward>().Data)
            {
                if (item.Value.Ranking == (int) rank)
                {
                    return item.Value;
                }
            }

            return null;
        }
        
        public static List<ConfigFirstRewardUnit> GetPvpRewardUnits()
        {
           return ConfigDataGroup.GetInstance<ConfigFirstReward>().Data.Values.ToList();
        }

        public static ConfigClassicsUnit GetCalssicsById(int id)
        {
            return ConfigDataGroup.GetInstance<ConfigClassics>().Get(id);
        }

        //获取满级秘典
        public static ConfigClassicsUnit GetClassicsByClassicsID(int classicsID)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigClassics>().Data.Values)
            {
                if (item.ClassicsID == classicsID && !item.Attr2.Equals('0'))
                {
                    return item;
                }
            }

            return null;
        }

        public static ConfigAptitudeUnit GetMaxLvAptitudeUnitByAptitUdeId(int aptitudeId)
        {
            var aptitudeData = ConfigDataGroup.GetInstance<ConfigAptitude>()?.Data?.Values;
            if (aptitudeData == null)
                return null;
            
            ConfigAptitudeUnit maxAptitudeUnit = null;
            foreach (var aptitude in aptitudeData)
            {
                if (aptitude.AptitudeId == aptitudeId && (maxAptitudeUnit == null || aptitude.Lv >= maxAptitudeUnit.Lv))
                {
                    maxAptitudeUnit = aptitude;
                }
            }
            return maxAptitudeUnit;
        }

        public static List<ConfigAptitudeUnit> GetAptitudeUnits()
        {
            return ConfigDataGroup.GetInstance<ConfigAptitude>().Data.Values.ToList();
        }

        public static ConfigAptitudeUnit GetNextAptitudeUnitByAptitudeIdAndLv(int aptitudeId,int lv)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAptitude>().Data)
            {
                if (item.Value.AptitudeId == aptitudeId && item.Value.Lv == (lv + 1))
                {
                    return item.Value;
                }
            }

            return null;
        }
        
        public static ConfigAptitudeUnit GetAptitudeUnitByAptitudeIdAndLv(int aptitudeId,int lv)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAptitude>().Data)
            {
                if (item.Value.AptitudeId == aptitudeId && item.Value.Lv == lv)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public static List<ConfigAptitudeUnlockUnit> GetAptitudeUnlockUnits(int type)
        {
            List<ConfigAptitudeUnlockUnit> aptitudeUnlockUnits = new List<ConfigAptitudeUnlockUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAptitudeUnlock>().Data)
            {
                if (item.Type == type)
                { 
                    aptitudeUnlockUnits.Add(item);
                }
            }
            return aptitudeUnlockUnits;
        }

        public static ConfigAptitudeUnlockUnit GetNextAptitudeUnitUnlockUnitByIdAndType(int id, int type)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAptitudeUnlock>().Data)
            {
                if ((item.BuffId == id + 1) && item.Type == type)
                { 
                    return item;
                }
            }
            
            return null;
        }

        public static ConfigArtifactEquipUnit GetArtifactEquipUnitByTypeAndLv(int type, int lv)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigArtifactEquip>().Data)
            {
                if (item.Value.Type == (int) type && item.Value.ArtifactLevel == lv)
                {
                    return item.Value;
                }
            }
            
            return null;
        }

        public static ConfigArtifactEquipUnit GetNextArtifactEquipUnitByTypeAndLv(int type, int lv)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigArtifactEquip>().Data)
            {
                if (item.Value.Type == type && item.Value.ArtifactLevel == (lv + 1))
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static ConfigArtifactEquipUnit GetMaxLvArtifactEquipUnitByType(int type)
        {
            var artifactData = ConfigDataGroup.GetInstance<ConfigArtifactEquip>()?.Data?.Values;
            if (artifactData == null)
                return null;
            ConfigArtifactEquipUnit maxLvArtifact = null;
            foreach (var item in artifactData)
            {
                if (item.Type == type && (maxLvArtifact == null || item.ArtifactLevel >= maxLvArtifact.ArtifactLevel))
                {
                    maxLvArtifact = item;
                }
            }
            return maxLvArtifact;
        }

        public static ConfigAttrEnumerationUnit GetAttrEnumerationUnitByAttrId(int attrId)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAttrEnumeration>().Data)
            {
                if (item.Value.AttrId == attrId)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static ConfigPetAptitudeUnit GetPetAptitudeUnitById(int id)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPetAptitude>().Data)
            {
                if (item.Value.Id == id)
                {
                    return item.Value;
                }
            }
            return null;
        }
        
        public static ConfigPetSkillBookUnit GetPetSkillBookUnitById(int id)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigPetSkillBook>().Data)
            {
                if (item.Value.Id == id)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static List<ConfigAchievementUnit> GetAchievementUnitByType(int type)
        {
            List<ConfigAchievementUnit> achievementUnits = new List<ConfigAchievementUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAchievement>().Data.Values)
            {
                if (item.AchievementType == type)
                {
                    achievementUnits.Add(item);
                }
            }

            return achievementUnits;
        }

        public static ConfigAchievementUnit GetAchievementUnitByAid(int aid)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAchievement>().Data.Values)
            {
                if (item.AID == aid)
                {
                    return item;
                }
            }
            return null;
        }

        // 该成就是否为前置成就
        public static bool GetNextAchievementUnitByAId(int aid)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAchievement>().Data.Values)
            {
                if (item.PreAchievementID == aid)
                {
                    return true;
                }
            }
            return false;
        }
        
        // 该成就是否为前置成就
        public static ConfigAchievementUnit GetNextAchievementUnitByAId2(int aid)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigAchievement>().Data.Values)
            {
                if (item.PreAchievementID == aid)
                {
                    return item;
                }
            }
            return null;
        }

        public static List<ConfigDropUnit> GetDropUnitByDropGroup(int dropGroup)
        {
            List<ConfigDropUnit> dropUnits = new List<ConfigDropUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigDrop>().Data.Values)
            {
                if (item.DropGroup == dropGroup)
                {
                    dropUnits.Add(item);
                }
            }
            
            return dropUnits;
        }

        public static ConfigHolyUnit GetHolyUnitByHolyIdAndLevel(int holyId, int level)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHoly>().Data.Values)
            {
                if (item.HolyId == holyId && item.Lv == level)
                {
                    return item;
                }
            }
            return null;
        }

        public static List<ConfigHolyUnit> GetAllHolyUnits()
        {
            List<ConfigHolyUnit> holyUnits = new List<ConfigHolyUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHoly>().Data.Values)
            {
                if (item.Lv == 1)
                {
                    holyUnits.Add(item);
                }
            }
            return holyUnits;
        }

        public static ConfigHolyUnit GetMaxHolyUnitByHolyId(int holyId)
        {
            ConfigHolyUnit maxUnit = null;
            
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHoly>().Data.Values)
            {
                if (item.HolyId == holyId)
                {
                    if (maxUnit == null || item.Lv > maxUnit.Lv)
                    {
                        maxUnit = item;
                    }
                }
            }
            return maxUnit;
        }

        public static ConfigHolyUnit GetNextHolyUnitByHolyIdAndLevel(int holyId, int level)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHoly>().Data.Values)
            {
                if (item.HolyId == holyId && item.Lv == level + 1)
                {
                    return item;
                }
            }
            return null;
        }

        public static List<ConfigHeroAttributesUnit> GetAllHeroAttributesUnitByObjTypeAndAttrType(int objType, int attrType)
        {
            List<ConfigHeroAttributesUnit> heroAttributesUnits = new List<ConfigHeroAttributesUnit>();
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroAttributes>().Data.Values)
            {
                if (item.ObjectType == objType && item.AttributesType == attrType)
                {
                    heroAttributesUnits.Add(item);
                }
            }
            
            return heroAttributesUnits;
        }

        /// <summary>
        /// 狩猎商城物品
        /// </summary>
        /// <returns></returns>
        public static List<ConfigEventStoreUnit> GetEventStoreUnitList()
        {
            List<ConfigEventStoreUnit> eventStoreUnits = new List<ConfigEventStoreUnit>();
            eventStoreUnits = ConfigDataGroup.GetInstance<ConfigEventStore>().Data.Values.ToList();
            eventStoreUnits = eventStoreUnits
                .OrderBy(unit => unit.Sort)
                .ToList();
            return eventStoreUnits;
        }

        /// <summary>
        /// 根据id获取狩猎任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ConfigEventTaskUnit GetEventTaskUnitById(int id)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigEventTask>().Data.Values)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        //获取所有的推送数据
        public static List<ConfigPushUnit> GetAllConfigPushUnit()
        {
            return ConfigDataGroup.GetInstance<ConfigPush>().Data.Values.ToList();
        }
    }
}
