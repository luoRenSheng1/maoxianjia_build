using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerRuneSlotUpdate_PC_Recv : IReceiver
    {
        public PlayerRuneSlotUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerRuneSlotUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.RuneSlotsList)
            {
                RuneSlotInfo slotInfo = new RuneSlotInfo();
                slotInfo.SlotId = (int)item.SlotId;
                slotInfo.RuneGuid = item.RuneGuid;
                slotInfo.Status = item.SlotStatus;
                RoleManager.Instance.AddRuneSlotInfo(slotInfo);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_RuneSlotInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerRuneSlotUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
