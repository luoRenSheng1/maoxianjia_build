using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class BatchReplacePetInSlot_SCRecv : IReceiver
    {
        public BatchReplacePetInSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchReplacePetInSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.PetSlotList)
                {
                    PetSlotInfo slotInfo = new PetSlotInfo();
                    slotInfo.SlotId = (int) item.SlotId;
                    slotInfo.PetId = (int) item.PetId;
                    slotInfo.Status = item.SlotStatus;
                    slotInfo.PetGuid = item.PetGuid;
                    RoleManager.Instance.AddPetSlotInfo(slotInfo);
                }

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PET_SC_SUCC);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_PET_LIST);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
                GameManager.Instance.TimerManager.SetTimer(.5f, () =>
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_COMPLETE_GUIDE_UPLOAD_PET);
                });
      
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BatchReplacePetInSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
