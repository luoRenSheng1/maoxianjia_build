using System;
using System.Collections.Generic;
using Config;
using EngineBase;
using msg;

namespace Engine
{
    public class RuneInfo
    {
        public ulong Guid;
        public int ItemId;
        public int Level;
        public int Quality;
        public int SkillId;
        public int MagicTimes;

        public int BattleIndex;
        
        public ConfigItemTypeUnit ItemTypeUnit => ConfigUtils.GetConfigItemTypeUnitById(ItemId);
    }
    
    public class RuneInfoManager : TSingleton<RuneInfoManager>
    {
        private List<RuneInfo> _battleRuneInfoList = new List<RuneInfo>();
        
        private List<RuneInfo> _runeInfos = new List<RuneInfo>();

        private readonly RuneInfo runeInfo = new RuneInfo();
        private const int RuneCount = 6;
        public int UnLockRunePos { get; set; } = 0;

        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, OnRuneUnlockUpdate);
        }

        public override void Dispose()
        {
            base.Dispose();
            _runeInfos.Clear();
            _battleRuneInfoList.Clear();
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, OnRuneUnlockUpdate);
        }

        private void OnRuneUnlockUpdate()
        {
            int tempUnlock = 0;
            for (int i = 0; i < RuneCount; i++)
            {
                var runeMap = FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType) (i + 2401));
                if (runeMap.Item1)
                    tempUnlock++;
            }

            if (UnLockRunePos < tempUnlock)
            {
                UnLockRunePos = tempUnlock;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ADDUNLOCK_RUNE_POS);
            }
        }

        public void UpdateRuneInfo(RuneInfo runeInfo)
        {
            bool isHas = false;
            foreach (var item in _runeInfos)
            {
                if (item.Guid == runeInfo.Guid)
                {
                    item.Level = runeInfo.Level;
                    item.Quality = runeInfo.Quality;
                    item.SkillId = runeInfo.SkillId;
                    item.ItemId = runeInfo.ItemId;
                    item.MagicTimes = runeInfo.MagicTimes;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
            {
                _runeInfos.Add(runeInfo);
            }
        }

        public List<RuneInfo> GetAllRuneInfoList()
        {
            return _runeInfos;
        }

        // 在所有的符石中获取某一个符石
        public RuneInfo GetRuneInAll(ulong runeId)
        {
            runeInfo.ItemId = (int)runeId;
            runeInfo.Level = 1;
            int skillId = 0;
            // foreach (var item in ConfigDataGroup.GetInstance<ConfigRunePond>().Data)
            // {
            //     if (item.Value.RuneId == (int)runeId)
            //     {
            //         skillId = item.Value.Skill;
            //         break;
            //     }
            // }
            runeInfo.SkillId = skillId;
            ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(skillId);
            runeInfo.Quality = skillUnit.SkillQuality;
            runeInfo.MagicTimes = 1;
            
            return runeInfo;
        }

        public Dictionary<int, RuneInfo> GetUpLoadRune(bool isInit = false)
        {
            Dictionary<int, RuneInfo> upLoadRuneDict = new Dictionary<int, RuneInfo>();
            for (int i = 0; i < UnLockRunePos; i++)
            {
                RuneSlotInfo slotInfo = RoleManager.Instance.GetRuneSlotInfoBySlotId(i);
                if (slotInfo != null && slotInfo.RuneGuid > 0)
                {
                    RuneInfo rune = GetRune(slotInfo.RuneGuid);
                    if (rune != null)
                    {
                        rune.BattleIndex = slotInfo.SlotId;
                        upLoadRuneDict.Add(i, rune);
                    }

                }
            }

            UpdateBattleRuneList(upLoadRuneDict, isInit);

            return upLoadRuneDict;
            
        }
        
        private void UpdateBattleRuneList(Dictionary<int, RuneInfo> upLoadRuneDict, bool isInit = false)
        {
            _battleRuneInfoList.Clear();
            foreach (var item in upLoadRuneDict)
            {
                _battleRuneInfoList.Add(item.Value);
            }
        }
        
        public bool IsInUpload(RuneInfo runeInfo)
        {
            foreach (var item in _battleRuneInfoList)
            {
                if (item.Guid == runeInfo.Guid)
                    return true;
            }

            return false;
        }
        
        public RuneInfo GetRune(ulong guid)
        {
            for (int i = 0; i < _runeInfos.Count; i++)
            {
                if (_runeInfos[i].Guid == guid)
                {
                    return _runeInfos[i];
                }
            }

            return null;
        }

        public List<RuneInfo> GetBattleRuneList()
        {
            return _battleRuneInfoList;
        }

        public int GetSkillTimes(int skillId)
        {
            int skillTimes = 0;
            foreach (var item in _battleRuneInfoList)
            {
                if (item.SkillId == skillId)
                    skillTimes += item.MagicTimes;
            }

            return skillTimes;
        }

        public void DeleteRuneInfo(ulong runeGuid)
        {
            foreach (var item in _runeInfos)
            {
                if (item.Guid == runeGuid)
                {
                    _runeInfos.Remove(item);
                    break;
                }
            }
        }

        public int GetNoUploadIndex()
        {
            for (int i = 0; i < UnLockRunePos; i++)
            {
                RuneSlotInfo slotInfo = RoleManager.Instance.GetRuneSlotInfoBySlotId(i);
                if (slotInfo == null || slotInfo.RuneGuid == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool HigherSkillTimes()
        {
            foreach (var item in _battleRuneInfoList)
            {
                foreach (var rune in _runeInfos)
                {
                    if (NotInBattleRune(rune) && item.SkillId == rune.SkillId && item.Guid != rune.Guid && item.MagicTimes < rune.MagicTimes)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool NotInBattleRune(RuneInfo item)
        {
            foreach (var runeInfo in _battleRuneInfoList)
            {
                if (runeInfo.Guid == item.Guid)
                    return false;
            }

            return true;
        }
        
        public bool OneHigherSkillTimes(RuneInfo rune)
        {
            foreach (var item in _battleRuneInfoList)
            {
                if (NotInBattleRune(rune) && item.SkillId == rune.SkillId && item.Guid != rune.Guid && item.MagicTimes < rune.MagicTimes)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasSameRuneInfo(RuneInfo runeInfo)
        {
            foreach (var item in _battleRuneInfoList)
            {
                if (item.SkillId == runeInfo.SkillId)
                    return true;
            }

            return false;
        }
        
    }
}