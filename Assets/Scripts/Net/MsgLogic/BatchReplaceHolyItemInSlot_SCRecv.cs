using msg;

namespace Engine
{
    using EngineBase;
    
    public class BatchReplaceHolyItemInSlot_SCRecv : IReceiver
    {
        public BatchReplaceHolyItemInSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchReplaceHolyItemInSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // 需要获取到已上阵的圣物的id列表，将其BattleIndex设置为-1
                
                foreach (var item in msg.HolyItemSlotList)
                {
                    HolySlotInfo slotInfo = new HolySlotInfo();
                    slotInfo.SlotId = (int)item.SlotId;
                    slotInfo.ItemId = (int)item.ItemId;
                    slotInfo.Status = item.SlotStatus;
                    
                    RoleManager.Instance.AddHolySlotInfo(slotInfo);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPLOAD_HOLY_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BatchReplaceHolyItemInSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}