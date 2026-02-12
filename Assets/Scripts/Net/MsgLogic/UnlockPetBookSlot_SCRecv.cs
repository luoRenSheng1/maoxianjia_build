using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class UnlockPetBookSlot_SCRecv : IReceiver
    {
        public UnlockPetBookSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_UnlockPetBookSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if (msg.IsSuccess == true)
                {
                    PetItemInfo pet = PetInfoManager.Instance.GetPet(msg.PetId);

                    foreach (var item in pet.bookSlotsList)
                    {
                        if (item.SlotId == msg.SlotIndex)
                        {
                            item.SlotStatus = eSlotStatus.eSlotStatus_Normal;
                        }
                    }
                    PetInfoManager.Instance.AddPet(pet);
                
                    // PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                
                    // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PETBOOKSLOT_UNLOCK);
                }
                else
                {
                    UIManager.Instance.ToastByKey(8052);
                }
                
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);

                foreach (var item in msg.ItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int)item.Id,item.Num);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PETBOOKSLOT_UNLOCK);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = UnlockPetBookSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}