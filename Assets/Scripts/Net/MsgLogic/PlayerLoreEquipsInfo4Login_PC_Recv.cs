using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerLoreEquipsInfo4Login_PC_Recv : IReceiver
    {
        public PlayerLoreEquipsInfo4Login_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_PlayerLoreEquipsInfo4Login_PC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("PlayerLoreEquipsInfo4Login_PC_Recv Process");
            foreach (var item in msg.EquipsList)
            {
                EquipData equipData = new EquipData();
                equipData.guid = item.Guid;
                equipData.id = (int) item.BaseItemId;
                (List<int>, List<double>) basicRet = PlayerAttrUtils.GetAttrID_Value(item.EquipAttrList.ToList());
                equipData.lstAttrsID = basicRet.Item1;
                equipData.lstAttrsValue = basicRet.Item2;
                (List<int>,List<int>, List<double>) entryRet = PlayerAttrUtils.GetEntryAttrID_Value(item.EntryInfoList.ToList());
                equipData.lstEntryCfgsID = entryRet.Item1;
                equipData.lstEntrysID = entryRet.Item2;
                equipData.lstEntrysValue = entryRet.Item3;
                equipData.lv = item.Level;
                equipData.partType = (int)item.EquipType;
                equipData.quality = (int)item.Quality;
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
            }
            
            // 接到的数据才是完整的
            // if(msg.SplitInfo.IsEnd)
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("PlayerLoreEquipsInfo4Login_PC_Recv Read " + mRecv.Length);
            msg = PlayerLoreEquipsInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}