using msg;

namespace Engine
{
    using EngineBase;
    
    public class BatchTalentsLevelUp_SCRecv : IReceiver
    {
        
        public BatchTalentsLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchTalentsLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.TalentsList)
                {
                    TalentInfoManager.Instance.UpdateTalentDict((int)item.TalentId,(int)item.Level);
                }

                foreach (var item in msg.ItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int)item.Id,(int)item.Num);
                }
                
                TalentInfoManager.Instance.UpdateTalentPoints((int)msg.TalentPoints);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TALENT_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(10002);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BatchTalentsLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}