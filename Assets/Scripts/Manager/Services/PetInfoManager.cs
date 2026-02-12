using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class PetStrengthVo
    {
        public ConfigPetBasisUnit PetBasisUnit;
        public int PreLv;
        public int CurLv;
    }

    public class PetBattleAttr
    {
        public int AttrId;
        public double AttrVal;
    }

    // 宠物家园加成属性类
    public class PetUpAttr
    {
        public int AttrId;
        public double AddVal;
    }
    
    /// <summary>
    /// 宠物书插槽
    /// </summary>
    public class PetSkillBookSlot
    {
        public int SlotId;   //从0开始
        public eSlotStatus SlotStatus;  //状态 目前就锁定未锁定
        public int BookId;   //PetSkillBook.xlsx中的id  ，为0表示没有装备
        public List<PetBattleAttr> battleAttrList = new List<PetBattleAttr>();
    }

    /// <summary>
    /// 宠物技能书
    /// </summary>
    public class PetSkillBook
    {
        public int BookId;   //PetSkillBook.xlsx中的id
        public int Amount;  //数量
        public int Gold;   //回收价格--回收用
        public List<PetBattleAttr> battleAttrList = new List<PetBattleAttr>();//加的属性--具体给加的对象需查表再加给英雄或宠物 参照SkillAchieve表作用对象
        public long Guid;
        public int Quality;
    }

    public class PetTalent
    {
        public int talentId;//宠物天赋表中的id
        public List<PetBattleAttr> BattleAttrs = new List<PetBattleAttr>();//加的属性--具体给加的对象需查表再加给英雄或宠物
        public int ObjType;//作用对象，参照SkillAchieve表作用对象，只有1--英雄 和 2--宠物  (或许未来有4--己方全部)
    }

    public class PetItemInfo
    {
        // 宠物属性
        public Dictionary<int,PetBattleAttr> PetAttrs = new Dictionary<int,PetBattleAttr>();
        public FightAttrVo FightAttrVo = new FightAttrVo();
        
        /// <summary>
        /// 宠物表PetBasis.xlsx中item id
        /// </summary>
        public int PetId;//UniqueId;
        
        /// <summary>
        /// 宠物唯一ID
        /// </summary>
        public ulong PetGuid;
        
        /// <summary>
        /// 等级
        /// </summary>
        public int PetLv = 0;
        
        /// <summary>
        /// 回收碎片数量
        /// </summary>
        public int recallDebris = 0;
        
        /// <summary>
        /// PetBasis.xlsx中  SkillId 字段 对应技能的 等级
        /// </summary>
        public int skillLevel = 0;
        
        /// <summary>
        /// 初始生成的随机技能  Skill.xlsx中  id 字段 对应技能的 SkillId
        /// </summary>
        public int skillId = 0;

        /// <summary>
        /// 天赋ID -- 宠物天赋表中的ID
        /// </summary>
        // public List<int> talentsList = new List<int>();
        public List<PetTalent> talentsList= new List<PetTalent>();
        
        /// <summary>
        /// 技能书插槽
        /// </summary>
        public List<PetSkillBookSlot> bookSlotsList = new List<PetSkillBookSlot>();
        
        /// <summary>
        /// 生长率 使用时候需乘以万分比
        /// </summary>
        public double growRate = 0;
        
        /// <summary>
        /// 品质 enum ePetQuality 
        /// </summary>
        public int quality = 0;
        
        // 携带战斗属性  不用了
        // public List<PetBattleAttr> CarryAttrs = new List<PetBattleAttr>();
        // 拥有战斗属性  不用了
        // public List<PetBattleAttr> OwnerAttrs = new List<PetBattleAttr>();
        
        /// <summary>
        /// 等级经验  经验进度条的值  不用了
        /// </summary>
        public int CardNumber = 0;
        
        /// <summary>
        /// 宠物配置表信息
        /// </summary>
        public ConfigPetBasisUnit petCfg;
        
        public bool unLock = true; //是否拥有
        public int BattleIndex = -1; //上阵位置
        
        /// <summary>
        /// 宠物家园加成属性列表
        /// </summary>
        public List<PetUpAttr> UpAttrs = new List<PetUpAttr>();
        
        /// <summary>
        /// 派遣的建筑类型
        /// </summary>
        public VillageBuildType DispatchBuild;
        /// <summary>
        /// 派遣中 部分建筑物内部的 id，比如床位id，工位id之类
        /// </summary>
        public int BuildInnerIndex;
    }
    
    public class PetInfoManager : TSingleton<PetInfoManager>
    {
        private List<int> _onceHavePetInfoList = new List<int>(); //曾经拥有的宠物，由服务器下发，不能重复
        private List<PetItemInfo> _havePetInfoList = new List<PetItemInfo>();
        private List<PetItemInfo> _allPetInfoList;
        private List<PetItemInfo> _battlePetInfoList = new List<PetItemInfo>();

        private static readonly int PET_TOTAL_CNT = 3;//5
        public int UnLockPetPos { get; set; } = 0;

        public List<PetSkillBook> _havePetSkillBookList = new List<PetSkillBook>();//拥有的宠物技能书列表,技能书背包
        public List<PetSkillBook> _battlePetSkillBookList = new List<PetSkillBook>();//上阵的宠物技能书列表
        
        public List<int> _lockTalentIds = new List<int>();//锁定的天赋id列表
        
        private ConfigCommonUnit _common300006;
        private ConfigCommonUnit _common300010;
        private ConfigCommonUnit _common3003;
        private ConfigCommonUnit _common3004;
        private ConfigCommonUnit _common3005;
        private bool GetPetBookPackageInLogin = true;

        public void OnInit()
        {
            _common300006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300006);
            _common300010 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300010);
            _common3003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3003);
            _common3004 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3004);
            _common3005 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3005);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.OnPetUnlockUpdate);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ATTR_CHANGE_UPATE_PET, this.OnUpdatePetAttr);
            InitAllPetList();
        }

        public override void Dispose()
        {
            _battlePetInfoList.Clear();
            _havePetInfoList.Clear();
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.OnPetUnlockUpdate);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ATTR_CHANGE_UPATE_PET, this.OnUpdatePetAttr);
            base.Dispose();
        }

        private void OnPetUnlockUpdate()
        {
            ConfigCommonUnit common = ConfigDataGroup.GetInstance<ConfigCommon>().Get(1); //1=获取宠物解锁数据
            int tempUnlock = 0;
            for (int i = 0; i < PET_TOTAL_CNT; i++)
            {
                var petMap = FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType) (i + 2001));
                if (petMap.Item1)
                    tempUnlock++;
            }

            if (UnLockPetPos < tempUnlock)
            {
                UnLockPetPos = tempUnlock;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ADDUNLOCK_PET_POS);
            }
        }

        /// <summary>
        /// 配置表初始
        /// </summary>
        private void InitAllPetList()
        {
            if (_allPetInfoList == null)
            {
                _allPetInfoList = new List<PetItemInfo>();
                foreach (var itemCfg in ConfigDataGroup.GetInstance<ConfigPetBasis>().Data)
                {
                     if(itemCfg.Value.Type == 2) continue;
                     var pet = new PetItemInfo();
                     pet.PetId = itemCfg.Value.Id;
                     pet.petCfg = itemCfg.Value;
                     pet.PetLv = 1;
                     _allPetInfoList.Add(pet);
                }
            }
        }

        public List<PetItemInfo> GetAllPetInfoList()
        {
            return _allPetInfoList;
        }

        public List<PetItemInfo> GetAllHavePetList()
        {
            List<PetItemInfo> tempPetList = new List<PetItemInfo>();
            foreach (var item in _havePetInfoList)
            {
                tempPetList.Add(item);
            }
            return tempPetList;
        }

        public void UpdateBattlePetList(Dictionary<int, PetItemInfo> upLoadPetDict, bool isInit = false)
        {
            _battlePetInfoList.Clear();
            foreach (var item in upLoadPetDict)
            {
                _battlePetInfoList.Add(item.Value);
            }

            if (!isInit)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_PET_LIST);
        }

        public List<PetItemInfo> GetBattlePetList()
        {
            return _battlePetInfoList;
        }

        public Dictionary<int, PetItemInfo> GetUpLoadPet(bool isInit = false)
        {
            Dictionary<int, PetItemInfo> upLoadPetDict = new Dictionary<int, PetItemInfo>();
            for (int i = 0; i < UnLockPetPos; i++)
            {
                PetSlotInfo slotInfo = RoleManager.Instance.GetPetSlotInfoBySlotId(i);
                if (slotInfo != null && slotInfo.PetId > 0)
                {
                    PetItemInfo pet = GetPet(slotInfo.PetGuid);
                    if (pet != null)
                    {
                        pet.BattleIndex = slotInfo.SlotId;
                        upLoadPetDict.Add(i, pet);
                    }

                }
            }

            UpdateBattlePetList(upLoadPetDict, isInit);

            return upLoadPetDict;
        }

        public bool IsInUpload(PetItemInfo petItemInfo)
        {
            // foreach (var item in _battlePetInfoList)
            // {
            //     if (item.petCfg.Id == petItemInfo.petCfg.Id)
            //         return true;
            // }
            
            foreach (var item in _battlePetInfoList)
            {
                if (item.PetGuid == petItemInfo.PetGuid)
                    return true;
            }

            return false;
        }

        public int GetNoUploadIndex()
        {
            for (int i = 0; i < UnLockPetPos; i++)
            {
                PetSlotInfo slotInfo = RoleManager.Instance.GetPetSlotInfoBySlotId(i);
                if (slotInfo == null || slotInfo.PetId == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 没有上阵槽位数量
        /// </summary>
        /// <returns></returns>
        public int GetNoUploadSlotCount()
        {
            int count = 0;
            for (int i = 0; i < UnLockPetPos; i++)
            {
                PetSlotInfo slotInfo = RoleManager.Instance.GetPetSlotInfoBySlotId(i);
                if (slotInfo == null || slotInfo.PetId == 0)
                { 
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 更新宠物信息
        /// </summary>
        /// <param name="newPet"></param>
        public void AddPet(PetItemInfo newPet)
        {
            bool isAdd = true;
            for (int i = 0; i < _havePetInfoList.Count; i++)
            {
                if (_havePetInfoList[i].PetGuid == newPet.PetGuid)
                {
                    _havePetInfoList[i] = newPet;
                    isAdd = false;
                    break;
                }
            }

            if (isAdd)
            {
                _havePetInfoList.Add(newPet);
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PET_INFO, newPet);
        }

        public void DeletePet(PetItemInfo pet)
        {
            if (pet.petCfg.Type == 2)
            {
                return;
            }
            
            for (int i = _havePetInfoList.Count - 1; i >= 0; i--)
            {
                if (_havePetInfoList[i].PetGuid == pet.PetGuid)
                {
                    _havePetInfoList.RemoveAt(i);
                    break;
                }
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELETE_PET_INFO, pet);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_PET_LIST);
        }
        

        public PetItemInfo GetPetInAll(int petId)
        {
            for (int i = 0; i < _allPetInfoList.Count; i++)
            {
                if (_allPetInfoList[i].petCfg.Id == petId)
                {
                    return _allPetInfoList[i];
                }
            }

            return null;
        }

        public PetItemInfo GetPet(ulong petGuid)
        {
            if (petGuid == 0) return null;
            for (int i = 0; i < _havePetInfoList.Count; i++)
            {
                if (_havePetInfoList[i].PetGuid == petGuid)
                {
                    return _havePetInfoList[i];
                }
            }

            return null;
        }
        
        public PetItemInfo GetPetByPetId(int petId)
        {
            if (petId == 0) return null;
            for (int i = 0; i < _havePetInfoList.Count; i++)
            {
                if (_havePetInfoList[i].PetId == petId)
                {
                    return _havePetInfoList[i];
                }
            }

            return null;
        }
        
        public void SetAllReadyPetIdList(int petId)
        {
            if (!_onceHavePetInfoList.Contains(petId))
                _onceHavePetInfoList.Add(petId);
        }

        public List<int> GetAllReadyPetIdList()
        {
            return _onceHavePetInfoList;
        }

        public bool HasReadyPet(int petId)
        {
            foreach (var item in _onceHavePetInfoList)
            {
                if (item == petId)
                    return true;
            }

            return false;
        }

        public bool IsCanUpLevel()
        {
            bool isCanLevelUp = false;
            foreach (var item in _havePetInfoList)
            {
                ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(item.quality, item.PetLv+1);
                if (petItem != null )
                {
                    // if (petItem.Item1.CardNumber <= item.CardNumber)
                    // {
                        isCanLevelUp = true;
                        break;
                    // }
                }
            }

            return isCanLevelUp;
        }

        public bool IsCanUpLoadPet()
        {
            return GetNoUploadIndex() != -1;
        }

        /// <summary>
        /// 没有上阵的宠物列表
        /// </summary>
        /// <returns></returns>
        public List<PetItemInfo> GetNoUpLoadPet()
        {
            //上阵的
            var battleIds = _battlePetInfoList.AsParallel()
                .Select(pet => pet.PetGuid)
                .ToHashSet();

            return _havePetInfoList.AsParallel()
                .Where(pet => !battleIds.Contains(pet.PetGuid) && pet.DispatchBuild == VillageBuildType.None)
                .ToList();
        }
        
        public List<PetItemInfo> GetNoUpLoadPet2()
        {
            //上阵的
            var battleIds = _battlePetInfoList.AsParallel()
                .Select(pet => pet.PetGuid)
                .ToHashSet();

            return _havePetInfoList.AsParallel()
                .Where(pet => !battleIds.Contains(pet.PetGuid))
                .ToList();
        }

        /// <summary>
        /// 获取没有上阵宠物中品质最高的宠物列表
        /// </summary>
        /// <param name="count">获取的数量</param>
        /// <returns></returns>
        public List<PetItemInfo> GetNoUploadPetHighQuality(int count)
        {
            List<PetItemInfo> noUploadPets = GetNoUpLoadPet2();
            if (noUploadPets == null || noUploadPets.Count == 0)
                return new List<PetItemInfo>();
            
            var sortedPets = noUploadPets.OrderByDescending(pet => pet.quality)
                .ThenByDescending(pet => pet.PetLv)
                .ThenBy(pet => pet.PetGuid)
                .ToList();
            
            // 获取最高品质的值
            int maxQuality = sortedPets.First().quality;
            // 筛选所有品质等于最高品质的项
            List<PetItemInfo> highestQualityPets = sortedPets
                .Where(pet => pet.quality == maxQuality)
                .ToList();
            
            return highestQualityPets;
        }

        /// <summary>
        /// 背包中有更高品质的宠物未上阵
        /// </summary>
        /// <returns></returns>
        public List<PetItemInfo> GetNoUploadPetHighQuality2()
        {
            List<PetItemInfo> result = new List<PetItemInfo>();
            
            List<PetItemInfo> battlePetList = GetBattlePetList();//已上阵
            if (battlePetList == null || battlePetList.Count == 0)
                return new List<PetItemInfo>();
            
            List<PetItemInfo> noUploadPetList = GetNoUpLoadPet2();//未上阵
            if (noUploadPetList == null || noUploadPetList.Count == 0)
                return new List<PetItemInfo>();
            
            var sortedPets = noUploadPetList.OrderByDescending(pet => pet.quality)
                .ThenByDescending(pet => pet.PetLv)
                .ThenBy(pet => pet.PetGuid)
                .ToList();

            // 找到已上阵列表中的最小品质
            int minQualityInBattle = battlePetList.Min(p => p.quality);
            
            // 检查未上阵列表中最高品质是否大于已上阵最小品质
            if (sortedPets[0].quality > minQualityInBattle)
            {
                int maxQuality = sortedPets[0].quality;
                return sortedPets.Where(p => p.quality == maxQuality).ToList();
            }
            else
            {
                return new List<PetItemInfo>();
            }
            
        }

        /// <summary>
        /// 宠物天赋洗练时，重置天赋id
        /// </summary>
        public void UpdatePetTalentIds(ulong petId,List<PetTalent> talentIds)
        {
            PetItemInfo pet = GetPet(petId);
            pet.talentsList = talentIds;
            
            // 天赋修改，重新设置宠物属性
            SetPetBattleAttr(pet);
        }

        /// <summary>
        /// 锁定的天赋id
        /// </summary>
        public void AddLockTalentIds(List<int> talentIds)
        {
            _lockTalentIds =  talentIds;
        }

        public List<int> GetLockTalentIds()
        {
            return _lockTalentIds;
        }

        /// <summary>
        /// 回收删除对应宠物
        /// </summary>
        /// <param name="petGuid"></param>
        public void DelPet(ulong petGuid)
        {
            foreach (var item in _havePetInfoList)
            {
                if (item.PetGuid == petGuid)
                {
                    _havePetInfoList.Remove(item);
                    break;
                }
            }
        }

        /// <summary>
        /// 获取背包中书的数量
        /// </summary>
        /// <returns></returns>
        public int GetSkillBookCount()
        {
            double count1 = 0;
            ItemData skillbookItemData1 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.UNCOMMON);
            ItemData skillbookItemData2 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.EPIC);
            ItemData skillbookItemData3 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.LEGEND);
            ItemData skillbookItemData4 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.MYTH);
            ItemData skillbookItemData5 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.IMMORTAL);
            ItemData skillbookItemData6 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.RANDOM);
        
            if (skillbookItemData6.count > 0)
            {
                count1 += skillbookItemData6.count;
            }
            if (skillbookItemData5.count > 0)
            {
                count1 += skillbookItemData5.count;
            }
            if (skillbookItemData4.count > 0)
            {
                count1 += skillbookItemData4.count;
            }
            if (skillbookItemData3.count > 0)
            {
                count1 += skillbookItemData3.count;
            }
            if (skillbookItemData2.count > 0)
            {
                count1 += skillbookItemData2.count;
            }
            if (skillbookItemData1.count > 0)
            {
                count1 += skillbookItemData1.count;
            }

            int count2 = 0;
            List<PetSkillBook> skillBookInPackageList = GetPetSkillBookList();
            if (skillBookInPackageList != null && skillBookInPackageList.Count > 0)
            {
                count2 = skillBookInPackageList.Count;
            }
            
            return (int)count1 + count2;
        }

        /// <summary>
        /// 是否有空的技能书槽位,背包中有技能书
        /// </summary>
        /// <param name="petItemInfo"></param>
        /// <returns></returns>
        public bool HasEmptySkillBookSlotAndBook(PetItemInfo petItemInfo)
        {
            bool hasBook = GetSkillBookCount() > 0;

            bool hasEmptySlot = false;
            List<PetSkillBookSlot> bookSlots = petItemInfo.bookSlotsList;
            foreach (var slot in bookSlots)
            {
                if (slot.BookId == 0 && slot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                {
                    hasEmptySlot = true;
                    break;
                }
            }
            
            return hasBook && hasEmptySlot;
        }

        /// <summary>
        /// 有空的技能书槽位,背包中有技能书
        /// </summary>
        /// <returns></returns>
        public bool PetSkillBookEmptySlotAndBookRed()
        {
            return GetBattlePetList().Any(pet => HasEmptySkillBookSlotAndBook(pet));
        }

        /// <summary>
        /// 是否有未解锁的技能书槽位
        /// </summary>
        /// <param name="petItemInfo"></param>
        /// <returns></returns>
        public bool HasLockedSkillBookSlot(PetItemInfo petItemInfo)
        {
            List<PetSkillBookSlot> bookSlots = petItemInfo.bookSlotsList;
            foreach (var slot in bookSlots)
            {
                if (slot.BookId == 0 && slot.SlotStatus == eSlotStatus.eSlotStatus_Locked)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取背包中品质最高的技能书
        /// </summary>
        /// <returns></returns>
        public int GetPetSkillBookHighQualityList()
        {
            int maxQuality = 0;
            
            List<PetSkillBook> skillBookInPackageList = GetPetSkillBookList();
            if (skillBookInPackageList == null || skillBookInPackageList.Count == 0)
                return maxQuality;
            
            maxQuality = skillBookInPackageList.Max(book => book.Quality);
            
            return maxQuality;
        }

        public int GetCommonPetBookHighQuality()
        {
            // 按品质从高到低
            SkillBookType[] qualityOrder = {
                SkillBookType.IMMORTAL,
                SkillBookType.MYTH,
                SkillBookType.LEGEND,
                SkillBookType.EPIC,
                SkillBookType.UNCOMMON,
                SkillBookType.RANDOM
            };
            
            foreach (var bookType in qualityOrder)
            {
                ItemData itemData = ItemInfoManager.Instance.GetItemData((int)bookType);
                if (itemData.count > 0)
                {
                    return ConfigUtils.GetConfigItemTypeUnitById((int)bookType).Quality;
                }
            }
            
            return 0;
        }

        /// <summary>
        /// 此上阵宠物的技能书中品质最低的书的品质
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        public int GetPetLowQualityBook(PetItemInfo pet)
        {
            int lowQuality = 100;
            bool found = false;
            
            foreach (var slot in pet.bookSlotsList)
            {
                if (slot.BookId != 0 && slot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                {
                    ConfigPetSkillBookUnit item = ConfigUtils.GetPetSkillBookUnitById(slot.BookId);
                    if (item != null)
                    {
                        int quality = item.Quality;
                        if (quality < lowQuality)
                        {
                            lowQuality = quality;
                        }
                        found = true;
                    }
                }
                    
            }
            
            return found ? lowQuality : 0;
        }
        
        /// <summary>
        /// 获取背包中最高品质技能书的品质
        /// </summary>
        /// <returns></returns>
        public int GetPackageHighPetBookQuality()
        {
            int qualityInpackage1 = GetPetSkillBookHighQualityList();
            int qualityInpackage2 = GetCommonPetBookHighQuality();
            int maxQualityInpackage = Mathf.Max(qualityInpackage1, qualityInpackage2);
            
            return maxQualityInpackage;
        }

        /// <summary>
        /// 背包中的书品质有高于宠物携带的书品质
        /// </summary>
        /// <returns></returns>
        public bool BookPackageHighPetBookQuality()
        {
            int qualityInpackage1 = GetPetSkillBookHighQualityList();
            int qualityInpackage2 = GetCommonPetBookHighQuality();
            int maxQualityInpackage = Mathf.Max(qualityInpackage1, qualityInpackage2);

            int petBookQuality = BattlePetLowBookQuality();
            
            return maxQualityInpackage > petBookQuality;
        }

        /// <summary>
        /// 获取所有上阵宠物携带的技能书中品质最低的书的品质
        /// </summary>
        /// <returns></returns>
        public int BattlePetLowBookQuality()
        {
            int lowQuality = 100;
            List<PetItemInfo> battlePetList = GetBattlePetList();
            bool found = false;
            
            foreach (var pet in battlePetList)
            {
                foreach (var slot in pet.bookSlotsList)
                {
                    if (slot.BookId != 0 && slot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                    {
                        ConfigPetSkillBookUnit item = ConfigUtils.GetPetSkillBookUnitById(slot.BookId);
                        if (item != null)
                        {
                            int quality = item.Quality;
                            if (quality < lowQuality)
                            {
                                lowQuality = quality;
                            }
                            found = true;
                        }
                    }
                    
                }
            }
            
            return found ? lowQuality : 0;
        }

        /// <summary>
        /// 回收删除对应宠物技能书
        /// </summary>
        /// <param name="bookGuid"></param>
        public void DelSkillBook(long bookGuid)
        {
            foreach (var item in _havePetSkillBookList)
            {
                if (item.Guid == bookGuid)
                {
                    _havePetSkillBookList.Remove(item);
                    break;
                }
            }
        }

        /// <summary>
        /// 获取宠物技能书背包信息    第一次打开任何一只宠物技能页面时请求，之后本地保存再打开无需请求
        /// </summary>
        public void SendPetSkillBookInfo_CS()
        {
            _havePetSkillBookList.Clear();
            var builder = PetSkillBookInfo_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_PetSkillBookInfo_CS, builder.Build());
        }

        public void PetSkillBooksInPackage(PetSkillBook petSkillBook)
        {
            _havePetSkillBookList.Add(petSkillBook);
        }

        /// <summary>
        /// 更新宠物技能书信息
        /// </summary>
        /// <param name="petSkillBook"></param>
        public void UpdatePetSkillBookInfo(PetSkillBook petSkillBook)
        {
            bool isAdd = true;
            for (int i = 0; i < _havePetSkillBookList.Count; i++)
            {
                // if (_havePetSkillBookList[i].BookId == petSkillBook.BookId)
                // {
                //     _havePetSkillBookList[i] = petSkillBook;
                //     isAdd = false;
                //     break;
                // }
                
                if (_havePetSkillBookList[i].Guid == petSkillBook.Guid)
                {
                    _havePetSkillBookList[i] = petSkillBook;
                    isAdd = false;
                    break;
                }
            }

            if (isAdd)
            {
                _havePetSkillBookList.Add(petSkillBook);
            }
            
        }

        /// <summary>
        /// 获取宠物技能书列表，背包技能书列表
        /// </summary>
        public List<PetSkillBook> GetPetSkillBookList()
        {
            return _havePetSkillBookList;
        }

        /// <summary>
        /// 根据guid获取背包内的技能书
        /// </summary>
        /// <param name="petGuid"></param>
        /// <returns></returns>
        public PetSkillBook GetSkillBookByGuidInPackage(long petGuid)
        {
            foreach (var petSkillBook in _havePetSkillBookList)
            {
                if (petSkillBook.Guid == petGuid)
                {
                    return petSkillBook;
                }
            }

            return null;
        }
        
        public bool ShowPetDetailBattlePetRedPointInPetList(PetItemInfo petItemInfo)
        {
            ItemData itemData = ItemInfoManager.Instance.GetItemData(ConstDefine.PetPieceId);
            var petLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetLvUp);
            if (petLvUpMap.Item1)
            {
                ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(petItemInfo.quality, petItemInfo.PetLv+1);
                if (petItem != null && itemData.count >= petItem.PetDebris)
                {
                    return true;
                }
            }
        
            var petSkillLvUpMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetSkillLvUp);
            if (petSkillLvUpMap.Item1)
            {
                ConfigPetSkillLevelUnit petSkillData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(petItemInfo.skillId, petItemInfo.skillLevel+1);
                if (petSkillData != null && itemData.count >= petSkillData.PetDebris)
                {
                    return true;
                }
            }
        
            return false;
        }
        
        public bool PetDetailRedPointHandle()
        {
            List<PetItemInfo> battlePetInfos = GetBattlePetList();
            foreach (var battlePet in battlePetInfos)
            {
                return ShowPetDetailBattlePetRedPointInPetList(battlePet);
            }
        
            return false;
        }
        
        public bool PetBattleRedPointHandle()
        {
            // 回收
            List<PetItemInfo> havePetInfoList = GetAllHavePetList();
            if (havePetInfoList != null && havePetInfoList.Count >= int.Parse(_common3004.Param1))
            {
                return true;
            }
        
            // 有空位栏且有未上阵宠物
            bool hasNoUploadSlot = IsCanUpLoadPet();//有空栏位
            int noUploadCount = GetNoUpLoadPet2().Count;//没有上阵宠物的数量
            if (hasNoUploadSlot && noUploadCount > 0)
            {
                return true;
            }
        
            // 背包中有更高品质的宠物未上阵
            List<PetItemInfo> pets = GetNoUploadPetHighQuality2();
            if (pets != null && pets.Count > 0)
            {
                return true;
            }
        
            return false;
        }
        
        public bool PetTalentRedPointHandle()
        {
            return ItemInfoManager.Instance.GetItemCount(int.Parse(_common300010.Param1.Split(',')[0])) >= int.Parse(_common3003.Param1);
        }
        
        public bool PetSkillBookRedPointHandle()
        {
            if (GetPetBookPackageInLogin)
            {
                SendPetSkillBookInfo_CS();
                GetPetBookPackageInLogin = false;
            }
            
            // 是否可开槽
            bool makeItem = ItemInfoManager.Instance.GetItemCount(int.Parse(_common300006.Param2.Split(',')[0])) >= int.Parse(_common300006.Param2.Split(',')[1]);
            bool hasLockedSkillBookSlot = GetBattlePetList().Any(pet => HasLockedSkillBookSlot(pet));
            bool canMake = hasLockedSkillBookSlot && makeItem;
        
            // 有空槽位且背包有技能书
            bool hasEmptyAndBook = PetSkillBookEmptySlotAndBookRed();
        
            // 背包中存在比槽位更高品质的技能书
            bool hasHighQualityBook = BookPackageHighPetBookQuality();
        
            // 背包中技能书数量大于参数时
            bool canRecycle = GetSkillBookCount() >= int.Parse(_common3005.Param1);
        
            return canMake || canRecycle || hasEmptyAndBook || hasHighQualityBook;
        }
        
        public bool PetRedPointHandle()
        {
            // 详情
            bool petDetailRed = PetDetailRedPointHandle();
            
            // 上阵
            bool petBattleRed = PetBattleRedPointHandle();
            
            // 天赋
            bool petTalentRed = PetTalentRedPointHandle();
            
            // 详情
            bool petBookRed = PetSkillBookRedPointHandle();
        
            return petDetailRed || petBattleRed || petTalentRed || petBookRed;
        }

        private void OnUpdatePetAttr()
        {
            foreach (var petItemInfo in _battlePetInfoList)
            {
                // 重新设置宠物属性
                SetPetBattleAttr(petItemInfo);
            }
        }
        
        private static List<string> attrsParams = null;
        private static List<string> attrs = null;
        public PetItemInfo GetPetInfoBySever(PetInfo item)
        {
            PetItemInfo pet = new PetItemInfo();
            pet.PetId = (int)item.PetId;
            pet.PetGuid = item.PetGuid;
            pet.PetLv = item.Level;
            pet.skillLevel = item.SkillLevel;
            pet.skillId = (int)item.SkillId;
            pet.growRate = item.GrowRate * ConstDefine.CONFIG_PLACE_EX;
            pet.quality = (int)item.Quality;
            
            pet.petCfg = ConfigUtils.GetPetById((int)item.PetId);
            pet.unLock = true;
            
            pet.talentsList.Clear();
            foreach (var petTalent in item.TalentsList) //宠物天赋
            {
                PetTalent talent = new PetTalent();
                talent.talentId = (int)petTalent.TalentId;
                talent.BattleAttrs.Clear();
                foreach (var i in petTalent.BattleAttrList)
                {
                    PetBattleAttr battleAttr = new PetBattleAttr();
                    battleAttr.AttrId = (int)i.AttrId;
                    battleAttr.AttrVal = i.AttrValue;
                    talent.BattleAttrs.Add(battleAttr);
                }
                talent.ObjType = (int)petTalent.ObjType;
                
                pet.talentsList.Add(talent);
            }

            pet.bookSlotsList.Clear();
            foreach (var attr in item.BookSlotsList)  //宠物技能书
            {
                PetSkillBookSlot bookSlot = new PetSkillBookSlot();

                bookSlot.SlotId = (int)attr.SlotId;
                bookSlot.SlotStatus = attr.SlotStatus;
                bookSlot.BookId = (int)attr.BookId;
                bookSlot.battleAttrList.Clear();
                foreach (var info in attr.BattleAttrList)
                {
                    PetBattleAttr battleAttr = new PetBattleAttr();
                    battleAttr.AttrId = (int)info.AttrId;
                    battleAttr.AttrVal = info.AttrValue;
                    bookSlot.battleAttrList.Add(battleAttr);
                }
                
                pet.bookSlotsList.Add(bookSlot);
            }
            
            SetPetBattleAttr(pet);
            
            return pet;
        }

        private static ConfigPetSkillLevelUnit skillData = null;
        private static ConfigPetLevelUnit petLevel = null;
        
        //todo 角色属性变化 重置宠物攻击力 
        /// <summary>
        /// 设置宠物攻击力
        /// </summary>
        public void SetPetBattleAttr(PetItemInfo petItemInfo)
        {
            petItemInfo.PetAttrs.Clear();
            foreach (var attr in petItemInfo.talentsList)
            {
                ConfigSkillAchieveUnit skillAchieveUnit = ConfigUtils.GetSkillAchieveById(attr.talentId);
                if (skillAchieveUnit != null && ( skillAchieveUnit.HandleObject == 2 || skillAchieveUnit.HandleObject == 4 ) ) //作用对象，参照SkillAchieve表作用对象， 2=己方宠物的基础属性 4=已方所有的基础属性
                {
                    foreach (var item in attr.BattleAttrs)
                    {
                        if (petItemInfo.PetAttrs.TryGetValue(item.AttrId, out PetBattleAttr Attr))
                        {
                            Attr.AttrVal += item.AttrVal;
                        }
                        else
                        {
                            PetBattleAttr battleAttr = new PetBattleAttr()
                            {
                                AttrId = item.AttrId,
                                AttrVal = item.AttrVal
                            };
                            petItemInfo.PetAttrs.Add(battleAttr.AttrId, battleAttr);
                        }
                    }
                }
            }
            
            foreach (var bookSlot in petItemInfo.bookSlotsList)
            {
                if (bookSlot.BookId != 0)
                {
                    ConfigPetSkillBookUnit petBook = ConfigUtils.GetPetSkillBookUnitById(bookSlot.BookId);
                    ConfigSkillAchieveUnit skillAchieveUnit = ConfigUtils.GetSkillAchieveById(bookSlot.BookId);
                    if (skillAchieveUnit != null && skillAchieveUnit.DevSide == 0 && skillAchieveUnit.Trigger == 0 && skillAchieveUnit.HandleObject == 2)
                    {
                        foreach (var item in bookSlot.battleAttrList)
                        {
                            if (petItemInfo.PetAttrs.TryGetValue(item.AttrId, out PetBattleAttr Attr))
                            {
                                Attr.AttrVal += item.AttrVal;
                            }
                            else
                            {
                                PetBattleAttr battleAttr = new PetBattleAttr()
                                {
                                    AttrId = item.AttrId,
                                    AttrVal = item.AttrVal
                                };
                                petItemInfo.PetAttrs.Add(battleAttr.AttrId, battleAttr);
                            }
                        }
                    }
                }
            }
            
            petItemInfo.FightAttrVo = FightUtils.GetPetFightAttrVo(petItemInfo.PetAttrs);
            
            //爆伤概率初始值
            ConfigCommonUnit common100007 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(100007);
            petItemInfo.FightAttrVo.CriticalInjury += int.Parse(common100007.Param1) * ConstDefine.CONFIG_PLACE_EX;
            
            //攻速
            petItemInfo.FightAttrVo.AtkSpeed += petItemInfo.petCfg.AtkSpeed * ConstDefine.CONFIG_PLACE_EX; //攻速 默认读配置
            
            // 战斗最终伤害 ---- 还未投放，暂时为角色的值
            petItemInfo.FightAttrVo.BattleFinalAttack += DataManager.Instance.GetRoleData().FightAttrVo.BattleFinalAttack;
            
            skillData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(petItemInfo.skillId, petItemInfo.skillLevel);
            if (skillData != null)
                petItemInfo.FightAttrVo.SkillDamage += skillData.SkillHurt * ConstDefine.CONFIG_PLACE_EX;

            petLevel = ConfigUtils.GetPetLevelByQualityWithLevel(petItemInfo.quality, petItemInfo.PetLv);
            if (petLevel != null)
            {
                petItemInfo.FightAttrVo.Atk +=
                    DataManager.Instance.GetRoleData().FightAttrVo.Atk * (petItemInfo.petCfg.Inherit * ConstDefine.CONFIG_PLACE_EX) * (1 + petLevel.PetHurt * ConstDefine.CONFIG_PLACE_EX) *
                    petItemInfo.growRate * (1 + petItemInfo.FightAttrVo.PetAtkADD + DataManager.Instance.GetRoleData().FightAttrVo.PetAtkADD) +
                    DataManager.Instance.GetRoleData().FightAttrVo.PetAtk + petItemInfo.FightAttrVo.PetAtk;
            }
        }
        
    }
}
