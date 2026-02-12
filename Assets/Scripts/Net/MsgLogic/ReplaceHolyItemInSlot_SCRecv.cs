using msg;

namespace Engine
{
    using EngineBase;
    
    public class ReplaceHolyItemInSlot_SCRecv : IReceiver
    {
        public ReplaceHolyItemInSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceHolyItemInSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if (msg.HolyId > 0)
                {
                    HolyItemInfo holyItemInfo = HolyManager.Instance.GetHolyItem((int)msg.HolyId);
                    holyItemInfo.BattleIndex = -1;
                }

                HolySlotInfo slotInfo = new HolySlotInfo();
                slotInfo.SlotId = (int)msg.HolyItemSlot.SlotId;
                slotInfo.ItemId = (int)msg.HolyItemSlot.ItemId;
                slotInfo.Status = msg.HolyItemSlot.SlotStatus;
                
                RoleManager.Instance.AddHolySlotInfo(slotInfo);

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPLOAD_HOLY_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_HOLY_LIST);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ReplaceHolyItemInSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}