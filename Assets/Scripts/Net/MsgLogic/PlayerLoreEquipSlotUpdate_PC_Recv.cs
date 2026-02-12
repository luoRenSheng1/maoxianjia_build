using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerLoreEquipSlotUpdate_PC_Recv : IReceiver
    {
        public PlayerLoreEquipSlotUpdate_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_PlayerLoreEquipSlotUpdate_PC;
        }

        public void Process()
        {  //玩家传承装备栏发生变动会下发
            foreach (var item in msg.EquipSlotsList) //装备栏
            {
                EquipSlotInfo slotInfo = new EquipSlotInfo();
                slotInfo.SlotType = (int)item.SlotType;
                slotInfo.equipGuid = item.EquipId;
                slotInfo.Status = item.SlotStatus;
                slotInfo.EquipCfgId = (int) item.BaseItemId;
                //RoleManager.Instance.AddEquipSlotInfo(slotInfo);
                //TODO 添加到装备表中，不直接装备
                
            }
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PET_UNLOCK);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("PlayerLoreEquipSlotUpdate_PC_Recv Read " + mRecv.Length);
            msg = PlayerLoreEquipSlotUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}