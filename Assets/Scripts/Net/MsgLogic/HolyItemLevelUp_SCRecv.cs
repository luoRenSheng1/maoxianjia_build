using msg;

namespace Engine
{
    using EngineBase;
    
    public class HolyItemLevelUp_SCRecv : IReceiver
    {
        public HolyItemLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HolyItemLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                HolyManager.Instance.UpdateHolyLvInfo((int)msg.HolyId, (int)msg.Level);
                
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                
                foreach (var item in msg.ItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int)item.Id,(int)item.Num);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HolyInfo);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HolyItemLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}