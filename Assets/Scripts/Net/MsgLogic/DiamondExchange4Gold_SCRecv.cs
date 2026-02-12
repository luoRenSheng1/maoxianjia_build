using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DiamondExchange4Gold_SCRecv : IReceiver
    {
        public DiamondExchange4Gold_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DiamondExchange4Gold_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                // UIManager.Instance.ToastByKey(10156);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DiamondExchange4Gold_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
