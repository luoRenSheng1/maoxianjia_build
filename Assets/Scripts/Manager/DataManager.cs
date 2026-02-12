using Engine;
using EngineBase;
using System;
using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

public class BuffData
{
    public eBattleAttr battleAttr;
    public double attrValue;
}

public class BuffInfo
{
    public List<BuffData> battleAttrList = new List<BuffData>(); //增加的buff 信息
    public ulong startTime;  // buff的生效时间
    public ulong endTime;   // buff的过期时间
    public int cfgId;   // 配置表中id，如没有就是自己维护的序列id，或无视，无用
}

public class ClaimBuff
{
    public int buffId;
    public BuffInfo buffInfo;
}

public class DataManager : Singleton<DataManager>
{
    public RoleData mRoleData = new RoleData(); //角色数据
    public TreasureData mTreasureData = new TreasureData(); //宝箱数据

    public Dictionary<int, EquipData> dictPartEquipData = new Dictionary<int, EquipData>(); //部位数据 part, (id, lv)  //只保存该部位对应的已经装备的数据
    public List<ItemData> lstItemData = new List<ItemData>(); //装备数据 id, count  //铸造台敲装备

    public ClaimBuff claimBuff = new ClaimBuff(); // 时效buff 数据  //List<BuffData> limitBuffList = new List<BuffData>(); 
    
    public List<BuffInfo> _buffInfoList = new List<BuffInfo>(); // 目前是用于遗迹buff
    
    public void ResetAllData()
    {
        mRoleData = new RoleData();
        mTreasureData = new TreasureData();

        dictPartEquipData = new Dictionary<int, EquipData>(); //part, (id, lv)
        lstItemData = new List<ItemData>(); //id, count
    }

    /// <summary>
    /// 角色数据
    /// </summary>
    /// <returns></returns>
    public RoleData GetRoleData()
    {
        return mRoleData;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public TreasureData GetTreasureData()
    {
        return mTreasureData;
    }

    /// <summary>
    /// 部位数据
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, EquipData> GetPartsData()
    {
        return dictPartEquipData;
    }

    /// <summary>
    /// 装备数据
    /// </summary>
    /// <returns></returns>
    public List<ItemData> GetEquipData()
    {
        return lstItemData;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public List<ItemData> SetItemData(int itemId, int num = 1)
    {
        var itemData = lstItemData.Find(m => m.id == itemId);
        if (null != itemData)
        {
            itemData.count += num;
        }
        else
        {
            itemData = new ItemData()
            {
                id = itemId,
                count = 1
            };
            lstItemData.Add(itemData);
        }

        return lstItemData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetEquipCount(int itemId)
    {
        var itemData = lstItemData.Find(m => m.id == itemId);
        return (int) (null != itemData ? itemData.count : 0);
    }

    /// <summary>
    /// 获取已经穿戴的装备
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public EquipData FindEquip(EN_EQUIP_PARTS part)
    {
        return dictPartEquipData.ContainsKey((int)part) ? dictPartEquipData[(int)part] : new EquipData();
    }

    /// <summary>
    /// 是否空槽
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public bool IsEmptySlot(EN_EQUIP_PARTS part)
    {
        return FindEquip(part).IsNull();
    }

    /// <summary>
    /// 神秘钥匙数
    /// </summary>
    /// <returns></returns>
    public int GetMagicKeys()
    {
        return ItemInfoManager.Instance.GetItemCount(ConstDefine.CONST_MAGIC_KEY);
    }
    
    /// <summary>
    /// 时限buff列表
    /// </summary>
    /// <returns></returns>
    public ClaimBuff GetLimitBuff()
    {
        return claimBuff;
    }

    /// <summary>
    /// 遗迹buff
    /// </summary>
    public void AddBuffInfos(BuffInfo buffInfo)
    {
        // bool isHas = false;
        //
        // foreach (var buff in _buffInfoList)
        // {
        //     if (buff.cfgId == buffInfo.cfgId)
        //     {
        //         buff.startTime = buffInfo.startTime;
        //         buff.endTime = buffInfo.endTime;
        //         buff.battleAttrList = buffInfo.battleAttrList;
        //         
        //         isHas = true;
        //         break;
        //     }
        // }
        //
        // if (!isHas)
        // {
        //     _buffInfoList.Add(buffInfo);
        // }
        
        _buffInfoList.Add(buffInfo);
        
    }

    /// <summary>
    /// 获取遗迹buff
    /// </summary>
    /// <returns></returns>
    public List<BuffInfo> GetBuffInfos()
    {
        return _buffInfoList;
    }
}