using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;

namespace Engine
{
    public class SkillStrengthVo
    {
        public ConfigSkillUnit SkillUnit;
        public int PreLv;
        public int CurLv;
    }
    
    public class SkillInfo
    {
        public ConfigSkillUnit SkillUnit;
        public int SkillId;
        public int Level = 1;
        public double SkillAtkValue;
        public double CarryAtkValue;
        public double OwnerAtkValue;
        public int BattleIndex = -1;
        public int CardNumber;
    }
    
    public class SkillInfoManager : TSingleton<SkillInfoManager>
    {
        private List<SkillInfo> _skillInfos = new List<SkillInfo>();
        
        private List<SkillInfo> _allSkillInfoList;
        private List<SkillInfo> _battleSkillInfoList = new List<SkillInfo>();

        private static readonly int SKILL_TOTAL_CNT = 8;
        public int UnLockSkillPos { get; set; } = 0;

        public void OnInit()
        {
            InitAllPetList();
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.OnSkillUnlockUpdate);
        }
        
        private void OnSkillUnlockUpdate()
        {
            int tempUnlock = 0;
            for (int i = 0; i < SKILL_TOTAL_CNT; i++)
            {
                var skillMap = FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)(i + 1901));
                if (i >= ConstDefine.PetSkillIndex) //插入宠物技能位置  前五个抽卡技能  后三个宠物技能
                {
                    skillMap = FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)(i - ConstDefine.PetSkillIndex + 2001));
                }
                else
                {
                    if (skillMap.Item1)
                        tempUnlock++;
                }
            }

            if (UnLockSkillPos < tempUnlock)
            {
                UnLockSkillPos = tempUnlock;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ADDUNLOCK_SKILL_POS);
            }
        }
        /// <summary>
        /// 配置表初始
        /// </summary>
        private void InitAllPetList()
        {
            if (_allSkillInfoList == null)
            {
                _allSkillInfoList = new List<SkillInfo>();
                foreach (var itemCfg in ConfigDataGroup.GetInstance<ConfigSkill>().Data)
                {
                    if(itemCfg.Value.SkillType != 5)//5=抽卡技能
                        continue;
                    SkillInfo skillInfo = new SkillInfo();
                    skillInfo.Level = 1;
                    skillInfo.SkillId = itemCfg.Value.Id;
                    skillInfo.SkillUnit = itemCfg.Value;
                     
                    _allSkillInfoList.Add(skillInfo);
                }
            }
        }

        public List<SkillInfo> GetAllSkillInfoList()
        {
            return _allSkillInfoList;
        }

        public override void Dispose()
        {
            base.Dispose();
            _skillInfos.Clear();
            _battleSkillInfoList.Clear();
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.OnSkillUnlockUpdate);
        }

        public void UpdateSkillInfo(SkillInfo skillInfo)
        {
            bool isHas = false;
            foreach (var item in _skillInfos)
            {
                if (item.SkillId == skillInfo.SkillId)
                {
                    item.Level = skillInfo.Level;
                    item.SkillId = (int) skillInfo.SkillId;
                    item.Level = (int) skillInfo.Level;
                    item.CardNumber = (int) skillInfo.CardNumber;
                    item.SkillAtkValue = skillInfo.SkillAtkValue;
                    item.CarryAtkValue = skillInfo.CarryAtkValue;
                    item.OwnerAtkValue = skillInfo.OwnerAtkValue;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
            {
                _skillInfos.Add(skillInfo);
            }
        }

        public bool IsHasNoUpLoadSkill()
        {
            foreach (var item in _skillInfos)
            {
                if (item.BattleIndex == -1)
                    return true;
            }

            return false;
        }

        public double GetSkillDamageById(int skillId)
        {
            foreach (var item in _skillInfos)
            {
                if (item.SkillId == skillId)
                {
                    return item.SkillAtkValue;
                }
            }

            return 0;
        }
        
        
        public Dictionary<int, SkillInfo> GetUpLoadSkill(bool isInit = false)
        {
            Dictionary<int, SkillInfo> upLoadSkillDict = new Dictionary<int, SkillInfo>();
            for (int i = 0; i < UnLockSkillPos; i++)
            {
                SkillSlotInfo slotInfo = RoleManager.Instance.GetSkillSlotInfoBySlotId(i);
                if (slotInfo != null && slotInfo.SkillId > 0)
                {
                    SkillInfo skill = GetSkill(slotInfo.SkillId);
                    if (skill != null)
                    {
                        skill.BattleIndex = slotInfo.SlotId;
                        upLoadSkillDict.Add(i, skill);
                    }

                }
            }

            UpdateBattleSkillList(upLoadSkillDict, isInit);

            return upLoadSkillDict;
        }
        
        private void UpdateBattleSkillList(Dictionary<int, SkillInfo> upLoadSkillDict, bool isInit = false)
        {
            _battleSkillInfoList.Clear();
            foreach (var item in upLoadSkillDict)
            {
                _battleSkillInfoList.Add(item.Value);
            }

            if (!isInit)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_SKILL_LIST);
        }

        public bool IsInUpload(SkillInfo skillInfo)
        {
            foreach (var item in _battleSkillInfoList)
            {
                if (item.SkillId == skillInfo.SkillId)
                    return true;
            }

            return false;
        }
        
        public SkillInfo GetSkill(int skillId)
        {
            for (int i = 0; i < _skillInfos.Count; i++)
            {
                if (_skillInfos[i].SkillId == skillId)
                {
                    return _skillInfos[i];
                }
            }

            return null;
        }

        public SkillInfo GetSkillInAll(int skillId)
        {
            for (int i = 0; i < _allSkillInfoList.Count; i++)
            {
                if (_allSkillInfoList[i].SkillId == skillId)
                {
                    return _allSkillInfoList[i];
                }
            }

            return null;
        }

        public List<SkillInfo> GetBattleSkillList()
        {
            return _battleSkillInfoList;
        }
        
        public int GetNoUploadIndex()
        {
            for (int i = 0; i < ConstDefine.PetSkillIndex; i++)//i < UnLockSkillPos
            {
                SkillSlotInfo slotInfo = RoleManager.Instance.GetSkillSlotInfoBySlotId(i);
                if (slotInfo == null || slotInfo.SkillId == 0)
                {
                    //Status的状态目前服务器发下的都是eSlotStatus_Normal，服务器说后期会正常下发
                    if (slotInfo.Status == eSlotStatus.eSlotStatus_Normal)
                    {
                        // 客户端做拦截：功能是否解锁
                        var skillMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkillPos1 + i);
                        if (skillMap.Item1)
                        {
                            return i;
                        }
                    }
                    // return i;
                }
            }

            return -1;
        }

        public List<SkillInfo> GetAllHaveSkillList()
        {
            return _skillInfos;
        }

        public bool IsCanUpLevel()
        {
            bool isCanLevelUp = false;
            foreach (var item in _skillInfos)
            {
                ConfigSkillLevelUnit skillItem = ConfigUtils.GetSkillLevelUnit(item.SkillUnit.Id, item.Level+1);
                if (skillItem != null)
                {
                    if (skillItem.CardNumber <= item.CardNumber)
                    {
                        isCanLevelUp = true;
                        break;
                    }
                }
            }

            return isCanLevelUp;
        }

        public bool IsCanUpLoadSkill()
        {
            return GetNoUploadIndex() != -1;
        }
    }
}