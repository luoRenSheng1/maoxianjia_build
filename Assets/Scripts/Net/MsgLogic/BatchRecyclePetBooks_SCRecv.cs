using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class BatchRecyclePetBooks_SCRecv : IReceiver
    {
        public BatchRecyclePetBooks_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchRecyclePetBooks_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);

                foreach (var guid in msg.SoldBooksList)
                {
                    PetInfoManager.Instance.DelSkillBook((long)guid);
                }

                foreach (var item in msg.SoldBookItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int)item.Id, item.Num);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RECYCLE_SKILL_BOOK);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BatchRecyclePetBooks_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}