using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerEquipSlotUpdate_PC_Recv : IReceiver
    {
        public PlayerEquipSlotUpdate_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_PlayerEquipSlotUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.EquipSlotsList)
            {
                // item.SlotType;
                // item.EquipGuid;
                // item
                EquipSlotInfo slotInfo = new EquipSlotInfo();
                slotInfo.SlotType = (int)item.SlotType;
                slotInfo.equipGuid = item.EquipId;
                slotInfo.Status = item.SlotStatus;
                slotInfo.EquipCfgId = (int) item.BaseItemId;
                RoleManager.Instance.AddEquipSlotInfo(slotInfo);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SIGLE_EQUIP_WEAR);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerEquipSlotUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}