using msg;

namespace Engine
{
    using EngineBase;
    
    public class OfferItem4Task_SCRecv : IReceiver
    {
        public OfferItem4Task_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_OfferItem4Task_SC;
        }

        public void Process()
        {   
            //上供完成任务反馈
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
                foreach (var item in msg.CostItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int)item.Id,(int)item.Num);
                }
                
                //上供完毕后，关闭上供界面
                UIManager.Instance.CloseUIPanel("SubmitGoldAndDiaMain");
            
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = OfferItem4Task_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}