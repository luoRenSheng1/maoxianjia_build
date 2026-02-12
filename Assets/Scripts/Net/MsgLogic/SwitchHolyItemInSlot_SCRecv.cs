using msg;

namespace Engine
{
    using EngineBase;
    
    public class SwitchHolyItemInSlot_SCRecv : IReceiver
    {
        public SwitchHolyItemInSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SwitchHolyItemInSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_USE_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SwitchHolyItemInSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}