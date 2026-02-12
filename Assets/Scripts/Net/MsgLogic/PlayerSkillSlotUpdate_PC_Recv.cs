using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerSkillSlotUpdate_PC_Recv : IReceiver
    {
        public PlayerSkillSlotUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerSkillSlotUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.SkillSlotsList)
            {
                SkillSlotInfo slotInfo = new SkillSlotInfo();
                slotInfo.SlotId = (int)item.SlotId;
                slotInfo.SkillId = (int)item.SkillId;
                slotInfo.Status = item.SlotStatus;
                RoleManager.Instance.AddSkillSlotInfo(slotInfo);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SkillSlotInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerSkillSlotUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
