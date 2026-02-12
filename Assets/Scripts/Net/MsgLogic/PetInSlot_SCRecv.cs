using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PetInSlot_SCRecv : IReceiver
    {
        public PetInSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetInSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if (msg.PetId > 0)
                {
                    PetItemInfo petItemInfo =  PetInfoManager.Instance.GetPet(msg.PetSlot.PetGuid);
                    petItemInfo.BattleIndex = -1;
                }

                PetSlotInfo slotInfo = new PetSlotInfo();
                slotInfo.SlotId = (int)msg.PetSlot.SlotId;
                slotInfo.PetId = (int)msg.PetSlot.PetId;
                slotInfo.Status = msg.PetSlot.SlotStatus;
                slotInfo.PetGuid = msg.PetSlot.PetGuid;
                RoleManager.Instance.AddPetSlotInfo(slotInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PET_SC_SUCC);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_PET_LIST);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_SKILL_LIST);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ATTR_CHANGE_UPATE_PET);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetInSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
