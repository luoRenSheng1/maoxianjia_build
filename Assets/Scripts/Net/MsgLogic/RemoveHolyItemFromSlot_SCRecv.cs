using msg;

namespace Engine
{
    using EngineBase;
    
    public class RemoveHolyItemFromSlot_SCRecv : IReceiver
    {
        public RemoveHolyItemFromSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RemoveHolyItemFromSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                HolyItemInfo holyItemInfo = HolyManager.Instance.GetHolyItem((int)msg.HolyId);
                HolySlotInfo holySlotInfo = new HolySlotInfo();
                holySlotInfo.SlotId = (int)msg.HolyItemSlot.SlotId;
                holySlotInfo.ItemId = (int)msg.HolyItemSlot.ItemId;
                holySlotInfo.Status = msg.HolyItemSlot.SlotStatus;
                RoleManager.Instance.AddHolySlotInfo(holySlotInfo);

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPLOAD_HOLY_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_HOLY_LIST);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = RemoveHolyItemFromSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}