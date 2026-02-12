using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerPetSlotUpdate_PC_Recv : IReceiver
    {
        public PlayerPetSlotUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerPetSlotUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.PetSlotsList)
            {
                PetSlotInfo slotInfo = new PetSlotInfo();
                slotInfo.SlotId = (int)item.SlotId;
                slotInfo.PetId = (int)item.PetId;
                slotInfo.Status = item.SlotStatus;
                slotInfo.PetGuid = item.PetGuid;
                RoleManager.Instance.AddPetSlotInfo(slotInfo);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PET_UNLOCK);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerPetSlotUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
