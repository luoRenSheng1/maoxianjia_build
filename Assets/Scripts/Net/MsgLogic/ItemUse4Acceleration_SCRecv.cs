using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ItemUse4Acceleration_SCRecv : IReceiver
    {
        public ItemUse4Acceleration_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ItemUse4Acceleration_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if(msg.UsedItems.Id > 0)
                    ItemInfoManager.Instance.ReduceItem((int) msg.UsedItems.Id, (int) msg.UsedItems.Num);
            
                PlayerAttrUtils.UpdateFinance(msg.Finance);
            
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_USE_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ItemUse4Acceleration_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
