using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class NewLoreEquips_PC_Recv : IReceiver
    {
        public NewLoreEquips_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_NewLoreEquips_PC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("NewLoreEquips_PC_Recv Process");
            EquipData tmpEquipData = null;
            foreach (var item in msg.LoreEquipsList)
            {
                EquipData equipData = new EquipData();
                equipData.guid = item.Guid;
                equipData.id = (int)item.BaseItemId;

                foreach (var item2 in item.EntryInfoList)
                {
                    Debug.Log(item2.EquipAttrList[0].AttrId);
                }
                    
                (List<int>, List<double>) basicRet = PlayerAttrUtils.GetAttrID_Value(item.EquipAttrList.ToList());
                equipData.lstAttrsID = basicRet.Item1;
                equipData.lstAttrsValue = basicRet.Item2;
                (List<int>,List<int>, List<double>) entryRet = PlayerAttrUtils.GetEntryAttrID_Value(item.EntryInfoList.ToList());
                equipData.lstEntryCfgsID = entryRet.Item1;
                equipData.lstEntrysID = entryRet.Item2;
                equipData.lstEntrysValue = entryRet.Item3;
                equipData.partType = (int)item.EquipType;
                equipData.quality = (int)item.Quality;
                equipData.lv = item.Level;
                equipData.recallGold = (int)item.RecallGold;
                foreach (var data in item.EntrySkillsList.ToList())
                {
                    SkillMultipleData multipleData = new SkillMultipleData();
                    multipleData.skillId = (int)data.SkillId;
                    multipleData.times = (int)data.SkillMultiple_;
                    equipData.skillMultiplesList.Add(multipleData);
                    
                    equipData.lstEntryCfgsID.Add((int)data.SkillId);
                    equipData.lstEntrysID.Add((int)data.SkillId);
                    equipData.lstEntrysValue.Add((int)data.SkillMultiple_);
                }
                EquipManager.Instance.AddEquipItem(equipData);
                tmpEquipData = equipData;
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GAIN_LORE_EQUIP, (int)item.BaseItemId);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOREEQUIP_UPDATE);
            
            // if (tmpEquipData != null)
            // {
            //     bool isNew = EquipManager.Instance.IsNewPartEquip((int)tmpEquipData.partType);
            //     EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_USE_TREASURE_CHES_RES, tmpEquipData, isNew);
            // }
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("NewLoreEquips_PC_Recv Read " + mRecv.Length);
            msg = NewLoreEquips_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}