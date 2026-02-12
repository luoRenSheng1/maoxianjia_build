using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;

namespace Engine
{

    public class HolyItemInfo
    {
        public int HolyId;//holy表中的holy id
        public int Level;
        public int TableKeyId;//holy表中的id--用于读取 itemid
        public int BattleIndex = -1;
    }

    public class HolyManager : TSingleton<HolyManager>
    {
        private List<HolyItemInfo> _holyItemInfoList = new List<HolyItemInfo>();//拥有的圣物列表
        private List<HolyItemInfo> _battleHolyItemInfoList = new List<HolyItemInfo>();//上阵的圣物列表
        
        private static readonly int Holy_TOTAL_CNT = 3;
        public int UnLockHolyPos { get; set; } = 0;

        public void UpdateHolyInfo(HolyItemInfo holyItemInfo)
        {
            bool isHas = true;
            for (int i = 0; i < _holyItemInfoList.Count; i++)
            {
                if (_holyItemInfoList[i].HolyId == holyItemInfo.HolyId)
                {
                    _holyItemInfoList[i] = holyItemInfo;
                    isHas = false;
                    break;
                }
            }

            if (isHas)
            {
                _holyItemInfoList.Add(holyItemInfo);
            }
        }

        public void DelHolyItemInfo(int holyId)
        {
            foreach (var item in _holyItemInfoList)
            {
                if (item.HolyId == holyId)
                {
                    _holyItemInfoList.Remove(item);
                } 
            }
        }

        public void UpdateHolyLvInfo(int holyId, int level)
        {
            for (int i = 0; i < _holyItemInfoList.Count; i++)
            {
                if (_holyItemInfoList[i].HolyId == holyId)
                {
                    _holyItemInfoList[i].Level = level;
                    break;
                }
            }
        }

        public HolyItemInfo GetHolyItem(int holyId)
        {
            foreach (var item in _holyItemInfoList)
            {
                if (item.HolyId == holyId)
                {
                    return item;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 获取已拥有的圣物列表
        /// </summary>
        /// <returns></returns>
        public List<HolyItemInfo> GetHolyItemList()
        {
            return _holyItemInfoList;
        }

        public void UpdateBattleHolyList()
        {
            _battleHolyItemInfoList.Clear();
            for (int i = 0; i < 3; i++)
            {
                HolySlotInfo slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(i);
                if (slotInfo != null && slotInfo.ItemId > 0)
                {
                    HolyItemInfo holy = GetHolyItem(slotInfo.ItemId);
                    if (holy != null)
                    {
                        // holy.BattleIndex = slotInfo.SlotId;
                        _battleHolyItemInfoList.Add(holy);
                    }
                }
            }
        }


        /// <summary>
        /// 获取已上阵圣物列表
        /// </summary>
        /// <returns></returns>
        public List<HolyItemInfo> GetHolyItemBattleList()
        {
            UpdateBattleHolyList();
            
            return _battleHolyItemInfoList;
        }

        public bool IsInUpload(HolyItemInfo holyItemInfo)
        {
            foreach (var item in _battleHolyItemInfoList)
            {
                if (item.HolyId == holyItemInfo.HolyId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 没有上阵的圣物列表
        /// </summary>
        /// <returns></returns>
        public List<HolyItemInfo> GetNoUploadHoly()
        {
            UpdateBattleHolyList();
            
            var battleIds = _battleHolyItemInfoList.AsParallel()
                .Select(holy => holy.HolyId)
                .ToHashSet();

            return _holyItemInfoList.AsParallel()
                .Where(holy => !battleIds.Contains(holy.HolyId))
                .ToList();
        }

        /// <summary>
        /// 圣物红点处理
        /// </summary>
        /// <returns></returns>
        public bool RedPointHandle()
        {
            List<HolyItemInfo> holyItemList = GetHolyItemList();//拥有的圣物列表
            int unlockedSlotCount = 0;
            int equippedCount = 0;
            
            // 栏位解锁且为空
            for (int i = 0; i < 3; i++)
            {
                HolySlotInfo slotInfo = RoleManager.Instance.GetHolySlotInfoBySlotId(i);
                if (slotInfo.Status == eSlotStatus.eSlotStatus_Normal)
                {
                    unlockedSlotCount++;
            
                    if (slotInfo.ItemId != 0)
                    {
                        equippedCount++;
                    }
                }
                
            }
            
            // 未装备的圣物数量
            int unequippedHolyCount = holyItemList.Count - equippedCount;
            if (unequippedHolyCount > 0 && unlockedSlotCount > equippedCount)
            {
                return true;
            }
            
            // 升级材料充足
            foreach (var item in holyItemList)
            {
                var nextUnit = ConfigUtils.GetNextHolyUnitByHolyIdAndLevel(item.HolyId, item.Level);
                if (ItemInfoManager.Instance.GetItemCount(1010015) >= nextUnit.Consume1 && DataManager.Instance.mRoleData.dia >= nextUnit.Consume2)
                {
                    return true;
                }
            }
            
            return false;
        }


    }
}