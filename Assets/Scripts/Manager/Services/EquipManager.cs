using System.Collections.Generic;
using EngineBase;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;

namespace Engine
{
    public struct AutoOpenEquipStruct
    {
        public int equipQuality;
        public bool isEntry1;
        public int entryId1;
        public int entryId2;
        public bool isEntry2;
        public int entryId3;
        public int entryId4;
        public int openKeyNumIndex;
        public int openKeyNum;
        public bool isTZQFull;
        
        
        public JsonObject ToJsonObject()
        {
            JsonObject jsObj = new JsonObject();
            jsObj["equipQuality"] = equipQuality;
            jsObj["isEntry1"] = isEntry1;
            jsObj["entryId1"] = entryId1;
            jsObj["entryId2"] = entryId2;
            jsObj["isEntry2"] = isEntry2;
            jsObj["entryId3"] = entryId3;
            jsObj["entryId4"] = entryId4;
            jsObj["openKeyNumIndex"] = openKeyNumIndex;
            jsObj["openKeyNum"] = openKeyNum;
            jsObj["isTZQFull"] = isTZQFull;
            return jsObj;
        }

        public void ReadJson(JsonObject jo)
        {
            equipQuality = jo.GetInt("equipQuality");
            isEntry1 = jo.GetBool("isEntry1");
            entryId1 = jo.GetInt("entryId1");
            entryId2 = jo.GetInt("entryId2");
            isEntry2 = jo.GetBool("isEntry2");
            entryId3 = jo.GetInt("entryId3");
            entryId4 = jo.GetInt("entryId4");
            openKeyNumIndex = jo.GetInt("openKeyNumIndex");
            openKeyNum = jo.GetInt("openKeyNum");
            isTZQFull = jo.GetBool("isTZQFull");
        }
    }

    public class EquipManager : TSingleton<EquipManager>
    {
        public AutoOpenEquipStruct OpenEquipStruct { get; set; } = default;
        public bool AutoUnpack { get; set; }
        public bool HasNewEquipToStopAutopack { get; set; }
        public ulong curNoEquipGuid { get; set; }
        public bool isNewEquip { get; set; }

        public int newEquipItemId { get; set; } = 0;//新装备的物品ID，开箱子砍树时使用
        
        /// <summary>
        /// 是传承装备
        /// </summary>
        public bool isInheritEquip { get; set; }
        
        List<EquipData> lstEquipTemp = new List<EquipData>(); //装备的信息详情 包含属性，装备等级等

        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PART_DECOMPOSE_RES, this.OnPartDecomposeSucc);
        }

        public override void Dispose()
        {
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PART_DECOMPOSE_RES, this.OnPartDecomposeSucc);
            base.Dispose();
        }

        public void ItemConsume(int consume)
        {
            var count = DataManager.Instance.GetMagicKeys();
            if (count >= consume)
            {
                var resCount = count - consume;
                var itemData = DataManager.Instance.lstItemData.Find(m => m.id == ConstDefine.CONST_MAGIC_KEY);
                if (null != itemData)
                {
                    itemData.count = resCount;
                }
            }
        }

        public void AddEquipItem(EquipData equipData)
        {
            lstEquipTemp.Add(equipData);
        }

        public EquipData GetEquipById(ulong guid)
        {
            foreach (var item in lstEquipTemp)
            {
                if (guid == item.guid)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 获得所有装备列表
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetAllEquip()
        {
            return lstEquipTemp;
        }

        /// <summary>
        /// 没有穿戴的装备  没用，之前都是获得直接装备上
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetNoWearEquip()
        {
            List<EquipData> lstEq = new List<EquipData>();
            foreach (var item in lstEquipTemp)
            {
                if (item.isWear == false)
                {
                    if (item.partType == (int)eEquipType.eEquipType_Helmet || item.partType == (int)eEquipType.eEquipType_Cuirass || item.partType == (int)eEquipType.eEquipType_Weapon || item.partType == (int)eEquipType.eEquipType_ShoulderAmor)
                    {
                        lstEq.Add(item);
                    }
                }
            }

            return lstEq;
        }
        
        /// <summary>
        /// 没有穿戴的传承装备
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetNoWearLoreEquip()
        {
            List<EquipData> lstEq = new List<EquipData>();
            foreach (var item in lstEquipTemp)
            {
                if (item.isWear == false)
                {
                    if (item.partType == (int)eLoreEquipType.eLoreEquipType_Bracers || item.partType == (int)eLoreEquipType.eLoreEquipType_Cloak || item.partType == (int)eLoreEquipType.eLoreEquipType_Gloves || item.partType == (int)eLoreEquipType.eLoreEquipType_Sash)
                    {
                        lstEq.Add(item);
                    }
                }
            }

            return lstEq;
        }

        /// <summary>
        /// 根据guid获取没有穿戴的传承装备
        /// </summary>
        /// <returns></returns>
        public EquipData GetNoWearLoreEquipByGuid(ulong guid)
        {
            foreach (var item in lstEquipTemp)
            {
                if (item.isWear == false)
                {
                    if (item.partType == (int)eLoreEquipType.eLoreEquipType_Bracers || item.partType == (int)eLoreEquipType.eLoreEquipType_Cloak || item.partType == (int)eLoreEquipType.eLoreEquipType_Gloves || item.partType == (int)eLoreEquipType.eLoreEquipType_Sash)
                    {
                        if (item.guid == guid)
                        {
                            return item;
                        }
                    }
                }
            }
            
            return null;
        }

        List<EquipData> loreEquipList = new List<EquipData>();
        /// <summary>
        /// 获取已经穿戴的传承装备的列表
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetInstallLoreEquipsList()
        {
            loreEquipList.Clear();
            foreach (var item in DataManager.Instance.dictPartEquipData)
            {
                if (item.Value.partType == (int)eLoreEquipType.eLoreEquipType_Cloak ||
                    item.Value.partType == (int)eLoreEquipType.eLoreEquipType_Bracers ||
                    item.Value.partType == (int)eLoreEquipType.eLoreEquipType_Gloves ||
                    item.Value.partType == (int)eLoreEquipType.eLoreEquipType_Sash)
                {
                    loreEquipList.Add(item.Value);
                }
            }
            return loreEquipList;
        }
        
        /// <summary>
        /// 抽卡技能关联符石技能的次数
        /// </summary>
        /// <returns></returns>
        public int GetSkillTimesWithEquip(int skillID)
        {
            int maxMagicTimes = 0;
            foreach (var equipData in GetInstallLoreEquipsList())
            {
                if (equipData.skillMultiplesList != null && equipData.skillMultiplesList.Count > 0)
                {
                    foreach (var skillMultipleData in equipData.skillMultiplesList)
                    {
                        if (skillMultipleData.skillId == skillID)
                        {
                            maxMagicTimes += skillMultipleData.times;
                        }
                    }
                }
            }
            return maxMagicTimes;
        }
        
        public void AddItem(int itemId, int num)
        {
            DataManager.Instance.SetItemData(itemId, num);
            LogUtils.LogWarning($"AddItem itemId:{itemId}   num:{num}");
        }
        
        /// <summary>
        /// 是否为新装备，之前对应的部位没有装备
        /// </summary>
        /// <param name="partId">部分id</param>
        /// <returns></returns>
        public bool IsNewPartEquip(int partId)
        {
            foreach (var item in DataManager.Instance.dictPartEquipData)
            {
                if (item.Key == partId && item.Value.guid > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void AddPartEquip(int partId, EquipData equipData)
        {
            if (DataManager.Instance.dictPartEquipData.ContainsKey(partId))
            {
                DataManager.Instance.dictPartEquipData[partId] = equipData;
            }
            else
            {
                DataManager.Instance.dictPartEquipData.Add(partId, equipData);
            }
        }

        // todo 移除装备部位信息
        public void RemovePartEquip(int partId)
        {
            if (DataManager.Instance.dictPartEquipData.ContainsKey(partId))
            {
                DataManager.Instance.dictPartEquipData[partId] = null;
            }
        }

        public void DelEquip(ulong equipGuid)
        {
            foreach (var item in lstEquipTemp)
            {
                if (item.guid == equipGuid)
                {
                    lstEquipTemp.Remove(item);
                    break;
                }
            }

        }

        public EquipData GetObtainEquip(ulong guid)
        {
            var equipData = lstEquipTemp.Find(m => m.guid == guid);
            return equipData;
        }

        public void DecomposeEquip(ulong guid)
        {
            var builder = SellEquip_CS.CreateBuilder();
            builder.EquipGuid = guid;
            SellEquip_CS sellEquipCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_SellEquip_CS, sellEquipCs);
        }

        private void OnPartDecomposeSucc()
        {
            // UIManager.Instance.ToastByKey(StringDefine.STRING_USE_DECOM_SUCC);
            EquipManager.Instance.curNoEquipGuid = 0;
            EquipManager.Instance.isNewEquip = false;
        }
        
        public SortedDictionary<int,string> GetProbabilityDict(string qualityProbability)
        {
            SortedDictionary<int, string > probabilities = new SortedDictionary<int, string>();
            string[] qualityPairs = qualityProbability.Split('|');
            
            foreach (string pair in qualityPairs)
            {
                string[] values = pair.Split(',');
                float val = (int.Parse(values[1]) / 100f);
                probabilities[int.Parse(values[0])] = val.ToString("f2");
            }

            return probabilities;
        }

        public void Clear()
        {
            lstEquipTemp.Clear();
            DataManager.Instance.dictPartEquipData.Clear();
            DataManager.Instance.lstItemData.Clear();
        }

        #region 传承装备红点

        public bool LoreRedPoint()
        {
            var loreEquipMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Inherit);
            if (loreEquipMap.Item1)
            {
                bool flag1 = HasNowearEquipRedPoint();
                bool flag2 = EquipManager.Instance.GetHighQualityEquipInBag().Count > 0;
                bool highSkillCount = EquipManager.Instance.GetHighSKillCountInLoreBag().Count > 0;//更高技能次数
                bool highDamageMultipler = EquipManager.Instance.GetHighDamageMultipler().Count > 0;//更高伤害倍率
                bool highHPMultipler = EquipManager.Instance.GetHighHPMultipler().Count > 0;//更高生命倍率
                
                return flag1 || flag2 || highSkillCount || highDamageMultipler || highHPMultipler;
            }
            
            return false;
        }

        /// <summary>
        /// 有空栏位，背包有该类型装备时展示红点
        /// </summary>
        /// <returns></returns>
        public bool HasNowearEquipRedPoint()
        {
            List<EquipData> noWearLoreEquipList = new List<EquipData>();
            noWearLoreEquipList = EquipManager.Instance.GetNoWearLoreEquip();//没有穿戴的传承装备列表

            List<EquipData> installLoreEquipList = new List<EquipData>();
            installLoreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();//已装备的传承装备列表

            // 获取已装备的所有partType
            HashSet<int> installedPartTypes = new HashSet<int>();
            foreach (var equip in installLoreEquipList)
            {
                installedPartTypes.Add(equip.partType);
            }

            // 检查未穿戴装备中是否存在新类型
            foreach (var equip in noWearLoreEquipList)
            {
                if (!installedPartTypes.Contains(equip.partType))
                {
                    return true; // 发现未穿戴的新类型
                }
            }
            
            return false;
        }

        /// <summary>
        /// 背包中有比穿戴的品质更高的传承装备
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetHighQualityEquipInBag()
        {
            List<EquipData> result = new List<EquipData>();
            
            List<EquipData> noWearLoreEquipList = EquipManager.Instance.GetNoWearLoreEquip();//没有穿戴的传承装备列表
            List<EquipData> installLoreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();//已装备的传承装备列表

            // 按部位分组已装备的传承装备，并记录每个部位的最高品质
            Dictionary<int, int> installedMaxQuality = new Dictionary<int, int>();
            if (installLoreEquipList != null)
            {
                foreach (var equip in installLoreEquipList)
                {
                    if (!installedMaxQuality.ContainsKey(equip.partType))
                    {
                        installedMaxQuality[equip.partType] = equip.quality;
                    }
                    else if (equip.quality > installedMaxQuality[equip.partType])
                    {
                        installedMaxQuality[equip.partType] = equip.quality;
                    }
                }
            }
            
            // 遍历未穿戴的传承装备
            if (noWearLoreEquipList != null)
            {
                foreach (var equip in noWearLoreEquipList)
                {
                    // 如果该部位没有已装备的传承装备，或者当前装备品质更高
                    if (!installedMaxQuality.ContainsKey(equip.partType) || 
                        equip.quality > installedMaxQuality[equip.partType])
                    {
                        result.Add(equip);
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// 背包中有更高的技能次数装备
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetHighSKillCountInLoreBag()
        {
            List<EquipData> result = new List<EquipData>();
            
            List<EquipData> noWearLoreEquipList = EquipManager.Instance.GetNoWearLoreEquip();//没有穿戴的传承装备列表
            List<EquipData> installLoreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();//已装备的传承装备列表

            // 按部位分组已装备的传承装备，并记录每个部位的最高技能次数
            Dictionary<int, int> installedMaxSkillCount = new Dictionary<int, int>();
            if (installLoreEquipList != null)
            {
                foreach (var equip in installLoreEquipList)
                {
                    int totalSkillCount = CalculateTotalSkillCount(equip);
            
                    if (!installedMaxSkillCount.ContainsKey(equip.partType))
                    {
                        installedMaxSkillCount[equip.partType] = totalSkillCount;
                    }
                    else if (totalSkillCount > installedMaxSkillCount[equip.partType])
                    {
                        installedMaxSkillCount[equip.partType] = totalSkillCount;
                    }
                }
            }
            
            // 按部位分组未穿戴的传承装备，并记录每个部位中技能次数最高的装备
            Dictionary<int, EquipData> bestNoWearEquips = new Dictionary<int, EquipData>();
            Dictionary<int, int> bestNoWearSkillCounts = new Dictionary<int, int>();
            
            if (noWearLoreEquipList != null)
            {
                foreach (var equip in noWearLoreEquipList)
                {
                    // 只有当该部位有已装备的传承装备时才处理
                    if (installedMaxSkillCount.ContainsKey(equip.partType))
                    {
                        int currentSkillCount = CalculateTotalSkillCount(equip);
                
                        // 是否满足条件：技能次数高于已装备的最高技能次数
                        if (currentSkillCount > installedMaxSkillCount[equip.partType])
                        {
                            // 当前装备技能次数更高
                            if (!bestNoWearSkillCounts.ContainsKey(equip.partType) || 
                                currentSkillCount > bestNoWearSkillCounts[equip.partType])
                            {
                                bestNoWearEquips[equip.partType] = equip;
                                bestNoWearSkillCounts[equip.partType] = currentSkillCount;
                            }
                        }
                    }
                }
            }
            
            // 将每个部位的最佳装备添加到结果列表
            foreach (var kvp in bestNoWearEquips)
            {
                result.Add(kvp.Value);
            }
            
            return result;
        }
        
        // 计算装备的总技能次数
        private int CalculateTotalSkillCount(EquipData equip)
        {
            int totalSkillCount = 0;
            if (equip.skillMultiplesList != null)
            {
                foreach (var skillData in equip.skillMultiplesList)
                {
                    totalSkillCount += skillData.times;
                }
            }
            return totalSkillCount;
        }

        /// <summary>
        /// 更高伤害倍率
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetHighDamageMultipler()
        {
            List<EquipData> result = new List<EquipData>();
            
            List<EquipData> noWearLoreEquipList = EquipManager.Instance.GetNoWearLoreEquip();//没有穿戴的传承装备列表
            List<EquipData> installLoreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();//已装备的传承装备列表

            // 按部位分组已装备的传承装备，并记录每个部位的最高伤害倍率
            Dictionary<int, double> installedMaxDamageMultiplier = new Dictionary<int, double>();
            if (installLoreEquipList != null)
            {
                foreach (var equip in installLoreEquipList)
                {
                    double damageMultiplier = GetAttributeValue(equip, 23); // 23表示伤害倍率
            
                    if (!installedMaxDamageMultiplier.ContainsKey(equip.partType))
                    {
                        installedMaxDamageMultiplier[equip.partType] = damageMultiplier;
                    }
                    else if (damageMultiplier > installedMaxDamageMultiplier[equip.partType])
                    {
                        installedMaxDamageMultiplier[equip.partType] = damageMultiplier;
                    }
                }
            }
            
            // 按部位分组未穿戴的传承装备，并记录每个部位中伤害倍率最高的装备
            Dictionary<int, EquipData> bestNoWearEquips = new Dictionary<int, EquipData>();
            Dictionary<int, double> bestNoWearDamageMultipliers = new Dictionary<int, double>();
            
            if (noWearLoreEquipList != null)
            {
                foreach (var equip in noWearLoreEquipList)
                {
                    // 只有当该部位有已装备的传承装备时才处理
                    if (installedMaxDamageMultiplier.ContainsKey(equip.partType))
                    {
                        double currentDamageMultiplier = GetAttributeValue(equip, 23); // 23表示伤害倍率
                
                        // 检查是否满足条件：伤害倍率高于已装备的最高伤害倍率
                        if (currentDamageMultiplier > installedMaxDamageMultiplier[equip.partType])
                        {
                            // 如果这个部位还没有记录，或者当前装备伤害倍率更高
                            if (!bestNoWearDamageMultipliers.ContainsKey(equip.partType) || 
                                currentDamageMultiplier > bestNoWearDamageMultipliers[equip.partType])
                            {
                                bestNoWearEquips[equip.partType] = equip;
                                bestNoWearDamageMultipliers[equip.partType] = currentDamageMultiplier;
                            }
                        }
                    }
                }
            }
            
            // 将每个部位的最佳装备添加到结果列表
            foreach (var kvp in bestNoWearEquips)
            {
                result.Add(kvp.Value);
            }

            return result;
        }

        /// <summary>
        /// 更高生命倍率
        /// </summary>
        /// <returns></returns>
        public List<EquipData> GetHighHPMultipler()
        {
            List<EquipData> result = new List<EquipData>();
            
            List<EquipData> noWearLoreEquipList = EquipManager.Instance.GetNoWearLoreEquip();//没有穿戴的传承装备列表
            List<EquipData> installLoreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();//已装备的传承装备列表
            
            // 首先按部位分组已装备的传承装备，并记录每个部位的最高生命倍率
            Dictionary<int, double> installedMaxHPMultiplier = new Dictionary<int, double>();
            if (installLoreEquipList != null)
            {
                foreach (var equip in installLoreEquipList)
                {
                    double hpMultiplier = GetAttributeValue(equip, 22); // 22表示生命倍率
            
                    if (!installedMaxHPMultiplier.ContainsKey(equip.partType))
                    {
                        installedMaxHPMultiplier[equip.partType] = hpMultiplier;
                    }
                    else if (hpMultiplier > installedMaxHPMultiplier[equip.partType])
                    {
                        installedMaxHPMultiplier[equip.partType] = hpMultiplier;
                    }
                }
            }
            
            // 按部位分组未穿戴的传承装备，并记录每个部位中生命倍率最高的装备
            Dictionary<int, EquipData> bestNoWearEquips = new Dictionary<int, EquipData>();
            Dictionary<int, double> bestNoWearHPMultipliers = new Dictionary<int, double>();
            
            if (noWearLoreEquipList != null)
            {
                foreach (var equip in noWearLoreEquipList)
                {
                    // 只有当该部位有已装备的传承装备时才处理
                    if (installedMaxHPMultiplier.ContainsKey(equip.partType))
                    {
                        double currentHPMultiplier = GetAttributeValue(equip, 22); // 22表示生命倍率
                
                        // 检查是否满足条件：生命倍率高于已装备的最高生命倍率
                        if (currentHPMultiplier > installedMaxHPMultiplier[equip.partType])
                        {
                            // 如果这个部位还没有记录，或者当前装备生命倍率更高
                            if (!bestNoWearHPMultipliers.ContainsKey(equip.partType) || 
                                currentHPMultiplier > bestNoWearHPMultipliers[equip.partType])
                            {
                                bestNoWearEquips[equip.partType] = equip;
                                bestNoWearHPMultipliers[equip.partType] = currentHPMultiplier;
                            }
                        }
                    }
                }
            }
    
            // 将每个部位的最佳装备添加到结果列表
            foreach (var kvp in bestNoWearEquips)
            {
                result.Add(kvp.Value);
            }
    
            return result;
        }
        
        // 从装备属性中获取指定属性的值
        private double GetAttributeValue(EquipData equip, int attributeId)
        {
            if (equip.lstAttrsID == null || equip.lstAttrsValue == null)
                return 0;
    
            for (int i = 0; i < equip.lstAttrsID.Count; i++)
            {
                if (equip.lstAttrsID[i] == attributeId)
                {
                    return equip.lstAttrsValue[i];
                }
            }
    
            return 0;
        }

        #endregion

        #region 神器

        private Dictionary<int,int> _artifactDict = new Dictionary<int,int>();
        
        public void UpdateArtifact(int artifactId, int lv)
        {
            // _artifactDict[artifactId] = lv;
            
            switch (artifactId)
            {
                case (int)ArtifactType.SHIELD:
                    DataManager.Instance.mRoleData.shield = lv;
                    break;
                case (int)ArtifactType.MEDAL:
                    DataManager.Instance.mRoleData.emblem = lv;
                    break;
                case (int)ArtifactType.STONE:
                    DataManager.Instance.mRoleData.rune = lv;
                    break;
                case (int)ArtifactType.JOB:
                    DataManager.Instance.mRoleData.sigil = lv;
                    break;
            }
            
        }

        public Dictionary<int, int> GetArtifactDict()
        {
            _artifactDict[(int)ArtifactType.SHIELD] = DataManager.Instance.mRoleData.shield;
            _artifactDict[(int)ArtifactType.MEDAL] = DataManager.Instance.mRoleData.emblem;
            _artifactDict[(int)ArtifactType.STONE] = DataManager.Instance.mRoleData.rune;
            _artifactDict[(int)ArtifactType.JOB] = DataManager.Instance.mRoleData.sigil;
            return _artifactDict;
        }
        
        public bool ArtifactRedPointHandler()
        {
            _artifactDict = GetArtifactDict();

            bool canUpgrade = false;
            foreach (var item in _artifactDict)
            {
                // var curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv(item.Key, item.Value);
                var nextUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(item.Key, item.Value);

                string[] costItems = nextUnit.ItemCost.Split(',');
                int costDiamond = nextUnit.DiamondsCost;
                int costItemId = int.Parse(costItems[0]);
                int costItemAmount = int.Parse(costItems[1]);

                // 升级条件
                canUpgrade = DataManager.Instance.mRoleData.dia >= costDiamond
                             && ItemInfoManager.Instance.GetItemCount(costItemId) >= costItemAmount;
                // && curUnit.ArtifactLevel < GetMaxHeroLv();

                if (canUpgrade)
                {
                    break;
                }

            }
            return canUpgrade;
        }

        #endregion
        
        /// <summary>
        /// 属性值处理
        /// </summary>
        /// <param name="attrId">属性id</param>
        /// <param name="value">数值</param>
        /// <param name="isBaifen">为false时表示服务器发下，为true表示读取配置表</param>
        /// <returns></returns>
        public string SetAttributeValue(int attrId, double value, bool isBaifen = false)
        {
            eBattleAttr attrIdType = (eBattleAttr)attrId;
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
                return StringUtils.FormatCurrency(value);
            }
            else
            {
                if (isBaifen)
                {
                    value = value * ConstDefine.CONFIG_PLACE;
                    return $"{StringUtils.FormatCurrency(value)}%";
                }
                value = value * 100;
                return $"{StringUtils.FormatCurrency(value)}%";
            }
        }

        public string GetQualityName(QualityType qualityType)
        {
            string qualityName = "";
            switch (qualityType)
            {
                case QualityType.COMMON:
                    qualityName = ConfigUtils.GetStringByKey(10026);
                    break;
                case QualityType.GOOD:
                    qualityName = ConfigUtils.GetStringByKey(10027);
                    break;
                case QualityType.UNCOMMON:
                    qualityName = ConfigUtils.GetStringByKey(10028);
                    break;
                case QualityType.EPIC:
                    qualityName = ConfigUtils.GetStringByKey(10029);
                    break;
                case QualityType.LEGEND:
                    qualityName = ConfigUtils.GetStringByKey(10030);
                    break;
                case QualityType.MYTH:
                    qualityName = ConfigUtils.GetStringByKey(10031);
                    break;
                case QualityType.IMMORTAL:
                    qualityName = ConfigUtils.GetStringByKey(10032);
                    break;
            }
            
            return qualityName;
        }


    }
}