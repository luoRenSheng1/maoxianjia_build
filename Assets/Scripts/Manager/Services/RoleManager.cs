using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;
using UnityEngine;
using PlayerPrefsEx = EngineBase.PlayerPrefs;

namespace Engine
{
    public enum ShuxingType
    {
        ATK = 2,
        HP = 1,
        CriticalStrike = 6,
        CriticalInjury = 7,
        Recovery = 3
    }
    public class ShuxingInfo
    {
        public int Lv;
        public ShuxingType Type;
        public double Val;
        public int MaxTag;
    }
    
    public class EquipSlotInfo
    {
        public int SlotType;  //装备部位枚举
        public ulong equipGuid;
        public eSlotStatus Status;
        public int EquipCfgId;
    }
    
    public class RuneSlotInfo
    {
        public int SlotId;//位置
        public ulong RuneGuid;//装备的符石唯一id
        public eSlotStatus Status;
        public int ItemId;
    }
    
    
    public class PetSlotInfo
    {
        public int SlotId;
        public int PetId;//装备的宠物id--PetBasis表中的id ,为0表示 没装备，放空
        public eSlotStatus Status;
        public ulong PetGuid; //宠物guid  作为唯一标识 ,为0表示 没宠物，放空
    }
    
    public class SkillSlotInfo
    {
        public int SlotId;
        public int SkillId;
        public eSlotStatus Status;
    }
    
    public class HolySlotInfo
    {
        public int SlotId;
        public int ItemId;//item id-- 装备的id， Holy表中HolyId
        public eSlotStatus Status;//状态
    }

    // 头像
    public class AvatarInfo
    {
        public int Id;//itemtype表的item id
        public int ExpiredTime;//到期时间戳，如果是0表示普通道具，proto不占用数据包空间
    }
    
    // 头像框
    public class AvatarFrameInfo
    {
        public int Id;//itemtype表的item id
        public int ExpiredTime;//到期时间戳，如果是0表示普通道具，proto不占用数据包空间
    }

    public class RoleManager : TSingleton<RoleManager>
    {
        public double TotalFight = 0;
        
        private List<EquipSlotInfo> _equipSlotInfos = new List<EquipSlotInfo>(); // 部位已经装备的装备列表，部位没有装备的guid为0
        private List<RuneSlotInfo> _runeSlotInfos = new List<RuneSlotInfo>();
        private List<PetSlotInfo> _petSlotInfos = new List<PetSlotInfo>();
        private List<SkillSlotInfo> _skillSlotInfos = new List<SkillSlotInfo>();
        private List<ShuxingInfo> _shuxingInfos = new List<ShuxingInfo>();
        private List<HolySlotInfo> _holySlotInfos = new List<HolySlotInfo>();//圣物栏

        public void InitSkillInfo()
        {
            for (int i = 0; i < 8; i++)  //插入宠物技能
            {
                SetSkillIdByIndex(0, i);
            }

            RuneInfoManager.Instance.GetUpLoadRune(true);
            SkillInfoManager.Instance.GetUpLoadSkill(true);
            List<SkillInfo> skillInfos = SkillInfoManager.Instance.GetBattleSkillList();
            for (int i = 0; i < skillInfos.Count; i++)
            {
                SetSkillIdByIndex(skillInfos[i].SkillId, skillInfos[i].BattleIndex);
            }
            
            //插入宠物技能
            Dictionary<int, PetItemInfo> petDic = PetInfoManager.Instance.GetUpLoadPet();
            foreach (var pet in petDic)
            {
                if (petDic.TryGetValue(pet.Key, out var petItemInfo))
                {
                    SetSkillIdByIndex(petItemInfo.skillId, petItemInfo.BattleIndex + ConstDefine.PetSkillIndex);
                }
            }
            
            SetHeroSkillProxy();
            
            _common1 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(1);
        }

        public void AddEquipSlotInfo(EquipSlotInfo slotInfo)
        {
            bool isAdd = true;
            for (int i = 0; i < _equipSlotInfos.Count; i++)
            {
                if (_equipSlotInfos[i].SlotType == slotInfo.SlotType)
                {
                    _equipSlotInfos[i] = slotInfo;
                    isAdd = false;
                    if (slotInfo.equipGuid > 0)
                    {
                        EquipData equipData = EquipManager.Instance.GetEquipById(slotInfo.equipGuid);
                        equipData.isWear = true;
                        equipData.isNew = false;
                        EquipManager.Instance.AddPartEquip(slotInfo.SlotType, equipData);
                    }
                    else
                    {
                        EquipManager.Instance.AddPartEquip(slotInfo.SlotType, new EquipData());
                    }
                    
                    break;
                }
            }

            if (isAdd)
            {
                _equipSlotInfos.Add(slotInfo);
                if (slotInfo.equipGuid > 0)
                {
                    EquipData equipData = EquipManager.Instance.GetEquipById(slotInfo.equipGuid);
                    equipData.isWear = true;
                    EquipManager.Instance.AddPartEquip(slotInfo.SlotType, equipData);
                }
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_PART_UPDATE);
        }

        // todo 去除装备槽信息
        public void ReduceEquipSlotInfo(EquipSlotInfo slotInfo)
        {
            for (int i = 0; i < _equipSlotInfos.Count; i++)
            {
                if (_equipSlotInfos[i].SlotType == slotInfo.SlotType)
                {
                    _equipSlotInfos[i] = slotInfo;
                    // EquipData equipData = EquipManager.Instance.GetEquipById(slotInfo.equipGuid);
                    // // EquipManager.Instance.RemovePartEquip(slotInfo.SlotType, equipData);
                    // EquipManager.Instance.AddPartEquip(slotInfo.SlotType, equipData);
                    break;
                }
            }
        }

        public EquipSlotInfo GetEquipSlotInfoBySlotType(int slotType)
        {
            foreach (var item in _equipSlotInfos)
            {
                if (item.SlotType == slotType)
                    return item;
            }

            return null;
        }
        
        public EquipSlotInfo GetEquipSlotInfoByGuid(ulong guid)
        {
            foreach (var item in _equipSlotInfos)
            {
                if (item.equipGuid == guid)
                    return item;
            }

            return null;
        }
        
        /// <summary>
        /// 获取使用的部位传承装备
        /// </summary>
        /// <param name="slotType"></param>
        /// <returns></returns>
        public EquipSlotInfo GetLoreEquipSlotInfoBySlotType(int slotType)
        {
            if (slotType == (int)eLoreEquipType.eLoreEquipType_Cloak ||
                slotType == (int)eLoreEquipType.eLoreEquipType_Bracers ||
                slotType == (int)eLoreEquipType.eLoreEquipType_Gloves ||
                slotType == (int)eLoreEquipType.eLoreEquipType_Sash)
            {
                foreach (var item in _equipSlotInfos)
                {
                    if (item.SlotType == slotType && item.equipGuid > 0)
                        return item;
                }
            }
            return null;
        }
        
        public void AddRuneSlotInfo(RuneSlotInfo slotInfo)
        {
            bool isAdd = true;
            for (int i = 0; i < _runeSlotInfos.Count; i++)
            {
                if (_runeSlotInfos[i].SlotId == slotInfo.SlotId)
                {
                    _runeSlotInfos[i] = slotInfo;
                    isAdd = false;
                    break;
                }
            }
            if(isAdd)
                _runeSlotInfos.Add(slotInfo);
        }
        
        public RuneSlotInfo GetRuneSlotInfoBySlotId(int slotId)
        {
            foreach (var item in _runeSlotInfos)
            {
                if (item.SlotId == slotId)
                    return item;
            }

            return null;
        }

        public void AddPetSlotInfo(PetSlotInfo slotInfo)
        {
            bool isAdd = true;
            for (int i = 0; i < _petSlotInfos.Count; i++)
            {
                if (_petSlotInfos[i].SlotId == slotInfo.SlotId)
                {
                    _petSlotInfos[i] = slotInfo;
                    isAdd = false;
                    break;
                }
            }
            if(isAdd)
                _petSlotInfos.Add(slotInfo);
        }
        
        public PetSlotInfo GetPetSlotInfoBySlotId(int slotId)
        {
            foreach (var item in _petSlotInfos)
            {
                if (item.SlotId == slotId)
                    return item;
            }

            return null;
        }
        
        public void AddSkillSlotInfo(SkillSlotInfo slotInfo)
        {
            bool isAdd = true;
            for (int i = 0; i < _skillSlotInfos.Count; i++)
            {
                if (_skillSlotInfos[i].SlotId == slotInfo.SlotId)
                {
                    _skillSlotInfos[i] = slotInfo;
                    isAdd = false;
                    break;
                }
            }
            if(isAdd)
                _skillSlotInfos.Add(slotInfo);
        }
        
        public SkillSlotInfo GetSkillSlotInfoBySlotId(int slotId)
        {
            foreach (var item in _skillSlotInfos)
            {
                if (item.SlotId == slotId)
                    return item;
            }

            return null;
        }
        /// <summary>
        /// 获取已经装备的技能数量
        /// </summary>
        /// <returns></returns>
        public int GetSkillSlotInfoCount()
        {
            int count = 0;
            foreach(var slot in _skillSlotInfos)
            {
                if(slot.SkillId != 0)
                {
                    count++;
                }
            }
            return count;
        }

        // 圣物栏信息
        public void AddHolySlotInfo(HolySlotInfo holySlotInfo)
        {
            bool isAdd = true;
            for (int i = 0; i < _holySlotInfos.Count; i++)
            {
                if (_holySlotInfos[i].SlotId == holySlotInfo.SlotId)
                {
                    _holySlotInfos[i] = holySlotInfo;
                    isAdd = false;
                    break;
                }
            }
            if(isAdd)
                _holySlotInfos.Add(holySlotInfo);
        }

        public HolySlotInfo GetHolySlotInfoBySlotId(int slotId)
        {
            foreach (var item in _holySlotInfos)
            {
                if (item.SlotId == slotId)
                    return item;
            }

            return null;
        }
        
        public HolySlotInfo GetHolySlotInfoByHolyId(int holyId)
        {
            foreach (var item in _holySlotInfos)
            {
                if (item.ItemId == holyId)
                    return item;
            }

            return null;
        }

        //充值统一记录数据
        private SortedDictionary<int, bool> _rechargeDict = new SortedDictionary<int, bool>();
        public void SetRechargeFlag(int payListId, bool hasRecharged)
        {
            if (_rechargeDict.ContainsKey(payListId))
                _rechargeDict[payListId] = hasRecharged;
            else
            {
                _rechargeDict.Add(payListId, hasRecharged);
            }
        }

        public bool GetHasRechargeByPayListId(int payListId)
        {
            if (_rechargeDict.ContainsKey(payListId))
                return _rechargeDict[payListId];
            return false;
        }
        
        
        //vip
        private SortedDictionary<int, bool> _vipRewardDict = new SortedDictionary<int, bool>();
        public void SetVipRewardFlag(int lv, bool flag)
        {
            if (_vipRewardDict.ContainsKey(lv))
                _vipRewardDict[lv] = flag;
            else
            {
                _vipRewardDict.Add(lv, flag);
            }
        }

        public bool GetVipRewardFlag(int lv)
        {
            if (_vipRewardDict.ContainsKey(lv))
                return _vipRewardDict[lv];
            return false;
        }

        public bool IsCanGetVipReward()
        {
            var vipUnits = ConfigDataGroup.GetInstance<ConfigVip>().Data.Values.ToList();
            foreach (var vipUnit in vipUnits)
            {
                if (DataManager.Instance.GetRoleData().vipLv >= vipUnit.Level)
                {
                    int tagIndex = RoleManager.Instance.GetVipRewardFlag(vipUnit.Level) ? 2 : 1;
                    if (tagIndex == 1)
                        return true;
                }
            }

            return false;
        }
        
        //vip
        private SortedDictionary<int, bool> _vipDiscountDict = new SortedDictionary<int, bool>();
        public void SetVipDiscountFlag(int lv, bool flag)
        {
            if (_vipDiscountDict.ContainsKey(lv))
                _vipDiscountDict[lv] = flag;
            else
            {
                _vipDiscountDict.Add(lv, flag);
            }
        }

        public bool GetVipDiscountFlag(int lv)
        {
            if (_vipDiscountDict.ContainsKey(lv))
                return _vipDiscountDict[lv];
            return false;
        }
        
        
        //抽卡技能
        private Dictionary<int, int> _skillIdDict = new Dictionary<int, int>();

        public void SetSkillIdByIndex(int id, int index)
        {
            if (_skillIdDict.ContainsKey(index))
            {
                _skillIdDict[index] = id;
            }
            else
            {
                _skillIdDict.Add(index, id);
            }
        }

        private List<int> _skillDelayTimes = new List<int>(8) {0, 1, 2, 3, 4, 5, 6, 7}; //插入宠物技能时间位置

        private Dictionary<int, MapSkillProxy> _mapSkillProxyDict = new Dictionary<int, MapSkillProxy>();
        private MapHeroSkillProxy _heroSkillProxy;
        
        private Dictionary<int, MapSkillProxy> _enemySkillProxyDict = new Dictionary<int, MapSkillProxy>();
        private MapHeroSkillProxy _enemyHeroSkillProxy;
        
        public Dictionary<int, MapSkillProxy> GetEnemySkillProxyDict()
        {
            if (_enemySkillProxyDict.Count == 0)
            {
                foreach (var item in PvpRankDataManager.Instance.GetOtherBattleSkillInfoList())
                {
                    if(item.SkillId <= 0) continue;
                    MapSkillProxy mapSkillProxy = new MapSkillProxy();
                    mapSkillProxy.InitAttr();
                    mapSkillProxy.ProxyAttr.unitID = item.SkillId;
                    mapSkillProxy.ProxyAttr.PosIndex = item.BattleIndex;
                    mapSkillProxy.ProxyAttr.skillXPID = item.SkillId;
                    mapSkillProxy.ProxyAttr.Camp = EN_CAMP_TYPE.ENEMY;
                    _enemySkillProxyDict.Add(item.BattleIndex, mapSkillProxy);
                }

                UpdateEnemySkillProxyAttr(DataManager.Instance.GetRoleData().FightAttrVo);
            }

            return _enemySkillProxyDict;
        }
        
         /// <summary>
        /// 切换pvp对手主角技能
        /// </summary>
        public void SetEnemyHeroSkillProxy(FightAttrVo fightAttrVo, int heroId)
         {
             ConfigHeroUnit heroUnit = ConfigUtils.GetHeroById(heroId);
            _enemyHeroSkillProxy?.Destroyed();
            _enemyHeroSkillProxy = new MapHeroSkillProxy();
            _enemyHeroSkillProxy.InitAttr();
            _enemyHeroSkillProxy.IsAutoXPAttack = true;
            _enemyHeroSkillProxy.ProxyAttr.unitID = heroUnit.Id;
            _enemyHeroSkillProxy.ProxyAttr.PosIndex = -1;
            _enemyHeroSkillProxy.ProxyAttr.skillXPID = heroUnit.ActiveSkill;
            _enemyHeroSkillProxy.ProxyAttr.Camp = EN_CAMP_TYPE.ENEMY;
            _enemyHeroSkillProxy.Attr.Atk = fightAttrVo.Atk;
            _enemyHeroSkillProxy.Attr.HPMax = fightAttrVo.HP;
            _enemyHeroSkillProxy.Attr.SkillCd = fightAttrVo.SkillCd;
            _enemyHeroSkillProxy.Attr.SkillDamage = fightAttrVo.SkillDamage;
            _enemyHeroSkillProxy.Attr.BossDamageAdd = fightAttrVo.BossDamageAdd;
            _enemyHeroSkillProxy.Attr.MonsterDamageAdd = fightAttrVo.MonsterDamageAdd;
            _enemyHeroSkillProxy.Attr.CriticalStrike = fightAttrVo.CriticalStrike;
            _enemyHeroSkillProxy.Attr.CriticalInjury = fightAttrVo.CriticalInjury;
            _enemyHeroSkillProxy.Attr.Bloodsucking = fightAttrVo.Bloodsucking;
            _enemyHeroSkillProxy.Attr.MagicTimes = fightAttrVo.MagicTimes;
            _enemyHeroSkillProxy.Attr.MagicTimesAdd = fightAttrVo.MagicTimesAdd;
        }

        private void UpdateEnemySkillProxyAttr(FightAttrVo fightAttrVo)
        {
            foreach (var item in _enemySkillProxyDict)
            {
                item.Value.Attr.Atk = fightAttrVo.Atk;
                item.Value.Attr.HPMax = fightAttrVo.HP;
                item.Value.Attr.SkillCd = fightAttrVo.SkillCd;
                item.Value.Attr.SkillDamage = fightAttrVo.SkillDamage;
                item.Value.Attr.BossDamageAdd = fightAttrVo.BossDamageAdd;
                item.Value.Attr.MonsterDamageAdd = fightAttrVo.MonsterDamageAdd;
                item.Value.Attr.CriticalStrike = fightAttrVo.CriticalStrike;
                item.Value.Attr.CriticalInjury = fightAttrVo.CriticalInjury;
                item.Value.Attr.Bloodsucking = fightAttrVo.Bloodsucking;
                item.Value.Attr.MagicTimes = fightAttrVo.MagicTimes;
                item.Value.Attr.MagicTimesAdd = fightAttrVo.MagicTimesAdd;
            }
        }
        
        
        public Dictionary<int, MapSkillProxy> GetSkillProxyDict()
        {
            if (_mapSkillProxyDict.Count == 0)
            {
                foreach (var item in _skillIdDict)
                {
                    MapSkillProxy mapSkillProxy = new MapSkillProxy();
                    mapSkillProxy.InitAttr();
                    mapSkillProxy.ProxyAttr.unitID = item.Key;
                    mapSkillProxy.ProxyAttr.PosIndex = item.Key;
                    mapSkillProxy.ProxyAttr.skillXPID = item.Value;
                    mapSkillProxy.ProxyAttr.Camp = EN_CAMP_TYPE.HERO;
                    if (item.Key >= ConstDefine.PetSkillIndex)
                    {  //插入宠物技能
                        mapSkillProxy.ProxyAttr.Camp = EN_CAMP_TYPE.FRIEND;
                        Dictionary<int, PetItemInfo> petDic = PetInfoManager.Instance.GetUpLoadPet();
                        int key = item.Key - ConstDefine.PetSkillIndex;
                        foreach (var data in petDic)
                        {
                            if (data.Value.BattleIndex == key)
                            {
                                mapSkillProxy.ProxyAttr.PetGuid = data.Value.PetGuid;
                                break;
                            }
                        }
                    }
                    _mapSkillProxyDict.Add(item.Key, mapSkillProxy);
                }

                UpdateSkillProxyAttr();
            }

            return _mapSkillProxyDict;
        }

        public int GetSkillDelayTime(int pos)
        {
            int delayTime = _skillDelayTimes[pos];
            _skillDelayTimes[pos] = 0;
            return delayTime;
        }

        public void UpdateSkillIdList(List<SkillInfo> skillInfos)
        {
            foreach (var item in _mapSkillProxyDict)
            {
                item.Value.ProxyAttr.skillXPID = 0;
            }

            foreach (var item in skillInfos)
            {
                if(_mapSkillProxyDict.ContainsKey(item.BattleIndex))
                    _mapSkillProxyDict[item.BattleIndex].ProxyAttr.skillXPID = item.SkillId;
                
            }
            
            //插入宠物技能
            Dictionary<int, PetItemInfo> petDic = PetInfoManager.Instance.GetUpLoadPet();
            foreach (var pet in petDic)
            {
                if (petDic.TryGetValue(pet.Key, out var petItemInfo))
                {
                    SetSkillIdByIndex(petItemInfo.skillId, petItemInfo.BattleIndex + ConstDefine.PetSkillIndex);
                    _mapSkillProxyDict[petItemInfo.BattleIndex + ConstDefine.PetSkillIndex].ProxyAttr.skillXPID = petItemInfo.skillId;
                    _mapSkillProxyDict[petItemInfo.BattleIndex + ConstDefine.PetSkillIndex].ProxyAttr.PetGuid = petItemInfo.PetGuid;
                }
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_Skill_LIST_Lobby);
        }

        public void EndSkillProxyDict()
        {
            foreach (var item in _mapSkillProxyDict)
            {
                item.Value.EndCurSkill();
            }

            _heroSkillProxy?.EndCurSkill();

            foreach (var item in _enemySkillProxyDict)
            {
                item.Value.EndCurSkill();
            }

            _enemyHeroSkillProxy?.EndCurSkill();
        }
        
        public void DestroySkillProxyDict()
        {
            foreach (var item in _mapSkillProxyDict)
            {
                item.Value.Destroyed();
            }

            _heroSkillProxy?.Destroyed();
            _heroSkillProxy = null;
            foreach (var item in _enemySkillProxyDict)
            {
                item.Value.Destroyed();
            }

            _enemyHeroSkillProxy?.Destroyed();
            _enemyHeroSkillProxy = null;
            _mapSkillProxyDict.Clear();
            _enemySkillProxyDict.Clear();
        }

        /// <summary>
        /// 切换主角技能
        /// </summary>
        public void SetHeroSkillProxy()
        {
            FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
            HeroInfo heroInfo = HeroInfoManager.Instance.GetMyHero();
            _heroSkillProxy?.Destroyed();
            _heroSkillProxy = new MapHeroSkillProxy();
            _heroSkillProxy.InitAttr();
            _heroSkillProxy.IsAutoXPAttack = true;
            _heroSkillProxy.ProxyAttr.unitID = heroInfo.HeroUnit.Id;
            _heroSkillProxy.ProxyAttr.PosIndex = -1;
            _heroSkillProxy.ProxyAttr.skillXPID = heroInfo.HeroUnit.ActiveSkill;
            _heroSkillProxy.ProxyAttr.Camp = EN_CAMP_TYPE.HERO;
            _heroSkillProxy.Attr.Atk = fightAttrVo.Atk;
            _heroSkillProxy.Attr.HPMax = fightAttrVo.HP;
            _heroSkillProxy.Attr.SkillCd = fightAttrVo.SkillCd;
            _heroSkillProxy.Attr.SkillDamage = fightAttrVo.SkillDamage;
            _heroSkillProxy.Attr.BossDamageAdd = fightAttrVo.BossDamageAdd;
            _heroSkillProxy.Attr.MonsterDamageAdd = fightAttrVo.MonsterDamageAdd;
            _heroSkillProxy.Attr.CriticalStrike = fightAttrVo.CriticalStrike;
            _heroSkillProxy.Attr.CriticalInjury = fightAttrVo.CriticalInjury;
            _heroSkillProxy.Attr.Bloodsucking = fightAttrVo.Bloodsucking;
            _heroSkillProxy.Attr.MagicTimes = fightAttrVo.MagicTimes;
            _heroSkillProxy.Attr.MagicTimesAdd = fightAttrVo.MagicTimesAdd;
        }

        private void UpdateSkillProxyAttr(bool refreshAuto = false)
        {
            FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
            foreach (var item in _mapSkillProxyDict)
            {
                if (refreshAuto)
                {
                    item.Value.IsAutoXPAttack = true;
                }
                item.Value.Attr.Atk = fightAttrVo.Atk;
                item.Value.Attr.HPMax = fightAttrVo.HP;
                item.Value.Attr.SkillCd = fightAttrVo.SkillCd;
                item.Value.Attr.SkillDamage = fightAttrVo.SkillDamage;
                item.Value.Attr.BossDamageAdd = fightAttrVo.BossDamageAdd;
                item.Value.Attr.MonsterDamageAdd = fightAttrVo.MonsterDamageAdd;
                item.Value.Attr.CriticalStrike = fightAttrVo.CriticalStrike;
                item.Value.Attr.CriticalInjury = fightAttrVo.CriticalInjury;
                item.Value.Attr.Bloodsucking = fightAttrVo.Bloodsucking;
                item.Value.Attr.MagicTimes = fightAttrVo.MagicTimes;
                item.Value.Attr.MagicTimesAdd = fightAttrVo.MagicTimesAdd;
            }
        }

        public void Tick(float delta)
        {
            ResetMonsterEntry();
            if (MapObjectManager.Instance.GetLocalHero() == null)
                return;
            
            ReportClientBattleInfo(delta);
            
            foreach (var item in _mapSkillProxyDict)
            {
                item.Value?.Tick(delta);
                item.Value?.UpdatePos(MapObjectManager.Instance.GetLocalHero().Position, MapObjectManager.Instance.GetLocalHero().GetGoRotation());
            }
            
            _heroSkillProxy?.Tick(delta);
            _heroSkillProxy?.UpdatePos(MapObjectManager.Instance.GetLocalHero().Position, MapObjectManager.Instance.GetLocalHero().GetGoRotation());

            if(!PVPMapManager.Instance.IsInPVP) return;
            MapHeroObject enemyHero = MapObjectManager.Instance.GetHeroObjectById(2);
            if(enemyHero == null) return;
            foreach (var item in _enemySkillProxyDict)
            {
                item.Value?.Tick(delta);
                item.Value?.UpdatePos(enemyHero.Position, MapObjectManager.Instance.GetHeroObjectById(2).GetGoRotation()*Quaternion.Euler(0, -90, 0));
            }
            
            _enemyHeroSkillProxy?.Tick(delta);
            _enemyHeroSkillProxy?.UpdatePos(enemyHero.Position,MapObjectManager.Instance.GetHeroObjectById(2).GetGoRotation()*Quaternion.Euler(0, -90, 0));
        }

        public void UpdateShuxingInfo(ShuxingInfo info)
        {
            bool isHas = false;
            foreach (var item in _shuxingInfos)
            {
                if (item.Type == info.Type)
                {
                    item.Lv = info.Lv;
                    item.Val = info.Val;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
                _shuxingInfos.Add(info);
        }

        public List<ShuxingInfo> GetShuxingInfos()
        {
            return _shuxingInfos;
        }

        public void UpdateFightInfo(FightAttrVo fightAttrVo, bool isBuffChange = false)
        {
            RoleData roleData = DataManager.Instance.GetRoleData();
            roleData.FightAttrVo = fightAttrVo;
            UpdateSkillProxyAttr(isBuffChange);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_FIGHT_INFO);
        }

        #region 头像和头像框

        private List<AvatarInfo> _avatarList = new List<AvatarInfo>();//已解锁头像列表
        private List<AvatarFrameInfo> _avatarFrameList = new List<AvatarFrameInfo>();//头像框列表
        private int useAvatarId;// 设置的头像id

        // 获取头像列表请求
        public void SendToGetAvatarListCS()
        {
            var builder = AvatarList_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AvatarList_CS, builder.Build());
        }

        /// <summary>
        /// 获取已解锁的头像对应的id
        /// </summary>
        /// <param name="id">服务器下发已解锁的头像，对应itemType表中的id</param>
        public void UpdateAvatarListInfo(int id)
        {
            AvatarInfo avatarInfo = new AvatarInfo();
            avatarInfo.Id = id;
            _avatarList.Add(avatarInfo);
        }

        public List<AvatarInfo> GetAvatarList()
        {
            return _avatarList;
        }

        // 删除头像
        public void DelAvatars(int id)
        {
            _avatarList.RemoveAll(avatar => avatar.Id == id);
        }

        // 设置为头像的id
        public void SetAvatarId(int id)
        {
            useAvatarId = id;
        }

        public int GetAvatarId()
        {
            return useAvatarId;
        }

        #endregion

        //获得英雄主动技能对象
        public MapHeroSkillProxy GetMapHeroSkillProxy()
        {
            return _heroSkillProxy;
        }
        
        //获得英雄主动技能对象
        public Dictionary<int, MapSkillProxy> GetMapSkillProxy()
        {
            return _mapSkillProxyDict;
        }

        //设置怪物词条
        public void SetMonsterEntry(bool isNew = false)
        {
            Utils.InitMonsterEntry(isNew);
        }

        private ulong saveTime = 0;
        private void ResetMonsterEntry()
        {
            if (saveTime == 0)
            {
                SetMonsterEntry();
            }
            if (saveTime == 0 && Utils.GetMonsterEntryById(0) != null)
            {
                saveTime = (ulong)Utils.GetMonsterEntryById(0)[0];
            }

            if ( ServerTimeManager.Instance.CurServerTime != 0 && (ServerTimeManager.Instance.CurServerTime - saveTime) > (1*60*60 - 2) )
            {
                SetMonsterEntry(true);
                saveTime = (ulong)Utils.GetMonsterEntryById(0)[0];
            }
        }
        
        #region 上报战斗数据

        private ConfigCommonUnit _common1;
        private float timeSpace = 0;
        
        public class HeroAttackData
        {
            public int skillCount;  //英雄 释放技能次数
            public List<double> atkValueList = new List<double>();  //英雄 每次攻击的技能攻击值
        }
        public Dictionary<int,HeroAttackData> heroCastSkillInfo = new Dictionary<int,HeroAttackData>(); //本轮500毫秒内 英雄释放技能次数信息
        public int HeroAtkCounter = 0;    //本轮500毫秒内英雄累计普攻次数
        public int HeroHurtCounter = 0;   //本轮500毫秒内累计英雄受到伤害(有减HP) 的次数
        public int HeroBeAtkCounter = 0;  //本轮500毫秒内累计英雄受到普攻的次数

        public Dictionary<ulong,PetAttackData> petAtkInfoDIc = new Dictionary<ulong,PetAttackData>();  //本轮500毫秒内宠物攻击信息
        public class PetAttackData
        {
            public int atkCount;   //宠物 本轮累计普攻次数
            public int skillId;    //宠物 释放技能id
            public int skillCount;  //宠物 释放技能次数
            public double petHurt;  // 宠物攻击力
            public List<double> atkValueList = new List<double>();  // 每次攻击的技能攻击值
        }
        
        public List<double> heroHpChangeList = new List<double>(); //本轮500毫秒内 每次HP变动后的数值
        
        public Dictionary<int,SkillCD> heroSkillCDDic= new Dictionary<int,SkillCD>(); //本轮500毫秒 内角色绑定技能和上阵技能 CD重置
        public Dictionary<int,SkillCD> petCastSkillCDDic = new Dictionary<int,SkillCD>(); //本轮500毫秒内 宠物绑定技能与上阵技能(技能书可能有)  发生的 CD重置的  技能ID
        public class SkillCD
        {
            public int skillId;
            public ulong petGuid;
        }
        
        //上报战斗数据
        public void ReportClientBattleInfo(float delta)
        {
            timeSpace = timeSpace + delta * 1000;
            if (_common1 != null && timeSpace >= int.Parse(_common1.Param1))
            {
                var builder = TriggerStageBuff_CS.CreateBuilder();
                var battleInfo = ClientBattleInfo.CreateBuilder();  //本轮收集信息
                
                foreach (var itemData in heroCastSkillInfo)
                {
                    var heroSkillInfo = CastSkillInfo.CreateBuilder();  //本轮500毫秒内 英雄释放技能次数信息
                    heroSkillInfo.SkillId = (uint)itemData.Key;   // 释放的技能id 或者 组合技能ID
                    heroSkillInfo.Counter = (uint)itemData.Value.skillCount;      // 本轮累计次数
                    heroSkillInfo.AddRangeAtkValue(itemData.Value.atkValueList);      // 每次攻击的技能攻击值
                    battleInfo.AddHeroCastSkill(heroSkillInfo.Build());
                }
                
                battleInfo.HeroAtkCounter = (uint)HeroAtkCounter;   //本轮500毫秒内英雄累计普攻次数
                battleInfo.HeroHurtCounter = (uint)HeroHurtCounter;  //本轮500毫秒内累计英雄受到伤害(有减HP) 的次数
                battleInfo.HeroBeAtkCounter = (uint)HeroBeAtkCounter;  //本轮500毫秒内累计英雄受到普攻的次数

                foreach (var itemData in petAtkInfoDIc)
                {
                    var petAtkInfo = PetAttackInfo.CreateBuilder();   //本轮500毫秒内宠物攻击信息
                    petAtkInfo.PetGuid = itemData.Key;   //宠物 guid； 
                    petAtkInfo.AtkCounter = (uint)itemData.Value.atkCount;     //宠物   本轮累计普攻次数
                    petAtkInfo.PetHurt = (double)itemData.Value.petHurt;     //宠物   攻击力
                    
                    if (itemData.Value.skillId != 0)
                    {
                        var petSkillInfo = CastSkillInfo.CreateBuilder();  //宠物  释放技能次数信息
                        petSkillInfo.SkillId = (uint)itemData.Value.skillId;
                        petSkillInfo.Counter = (uint)itemData.Value.skillCount;
                        petSkillInfo.AddRangeAtkValue(itemData.Value.atkValueList);
                        petAtkInfo.AddCastSkills(petSkillInfo.Build());
                    }
                    
                    battleInfo.AddPetAtkInfo(petAtkInfo.Build());
                }

                for (int i = 0; i < heroHpChangeList.Count; i++)
                {
                    battleInfo.AddHeroHp(heroHpChangeList[i]);  //本轮500毫秒内 每次HP变动后的数值
                }

                foreach (var itemData in heroSkillCDDic)
                {
                    var heroShortenCDSkill = ShortenCDSkill.CreateBuilder();
                    heroShortenCDSkill.SkillId = (uint)itemData.Value.skillId;
                    heroShortenCDSkill.PetGuid = itemData.Value.petGuid;
                    battleInfo.AddHeroShortenSkill(heroShortenCDSkill.Build());
                }

                foreach (var itemData in petCastSkillCDDic)
                {
                    var petShortenCDSkill = ShortenCDSkill.CreateBuilder();   //本轮500毫秒内 宠物绑定技能与上阵技能(技能书可能有)  发生的 CD重置的  技能ID
                    petShortenCDSkill.SkillId = (uint)itemData.Value.skillId;
                    petShortenCDSkill.PetGuid = itemData.Value.petGuid;
                    battleInfo.AddHeroShortenSkill(petShortenCDSkill.Build());
                }
                
                builder.BattleFrameInfo = battleInfo.Build();
                builder.ClientSendTimestamp = ServerTimeManager.Instance.CurServerTime;
                if (!GameManager.Instance.isAppBackPause)
                {
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_TriggerStageBuff_CS, builder.Build());
                }
                
                timeSpace = 0;
                heroCastSkillInfo.Clear();
                HeroAtkCounter = 0;
                HeroHurtCounter = 0;
                HeroBeAtkCounter = 0;
                
                petAtkInfoDIc.Clear();
                heroHpChangeList.Clear();
                heroSkillCDDic.Clear();
                petCastSkillCDDic.Clear();
            }
        }
        
        #endregion
    }
}