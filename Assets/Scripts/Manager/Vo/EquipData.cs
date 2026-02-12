using System;
using System.Collections.Generic;
using UnityEngine;
using EngineBase;
using msg;

namespace Engine
{
   [System.Serializable]
    public class SkillMultipleData
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public int skillId;
        /// <summary>
        /// 技能伤害次数
        /// </summary>
        public int times;
    }
    
    [System.Serializable]
    public class EquipData
    {
        public ulong guid;//装备唯一id
        public int id;
        public int lv;
        public List<int> lstAttrsID = new List<int>();  //装备属性ID（和角色属性一样）
        public List<double> lstAttrsValue = new List<double>(); //装备对应属性值（和角色属性一样）

        public List<int> lstEntryCfgsID = new List<int>();//装备词条配置表ID
        public List<int> lstEntrysID = new List<int>(); //装备词条属性ID（和角色属性一样）
        public List<double> lstEntrysValue = new List<double>();

        // public double recallGold;
        public int partType;
        public int quality;
        public bool isWear;
        public bool isNew = false;

        public int recallGold;  //出售的金币 客户端显示要乘以折扣
        public List<SkillMultipleData> skillMultiplesList = new List<SkillMultipleData>();
        
        public EquipData()
        {
            id = 0;
            lv = 0;
            recallGold = 0;

            lstAttrsID.Clear();
            lstAttrsValue.Clear();
            lstEntrysID.Clear();
            lstEntrysValue.Clear();
        }

        public bool IsNull()
        {
            return id == 0 && lv == 0;
        }

        public int GetAttrCount()
        {
            return lstAttrsID.Count;
        }
        
        public int GetEntryCount()
        {
            return lstEntrysID.Count;
        }

        public int GetSourceId()
        {
            return (int)(id / ConstDefine.ItemLevLen) * ConstDefine.ItemLevLen;
        }
    }
}