using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DiamondExchange4AccItem_SCRecv : IReceiver
    {
        public DiamondExchange4AccItem_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DiamondExchange4AccItem_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ItemInfoManager.Instance.AddItemData(new ItemData()
                {
                    id = (int) msg.Items.Id,
                    count = msg.Items.Num
                });
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                // UIManager.Instance.ToastByKey(10156);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DiamondExchange4AccItem_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
