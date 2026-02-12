using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimPassSubTaskExp_SCRecv : IReceiver
    {
        public ClaimPassSubTaskExp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimPassSubTaskExp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ActivityManager.Instance.UpdatePassSubTask((int)msg.SubTask.TaskId, msg.SubTask.Process, msg.SubTask.HasClaimed);
                PassPortInfo passPortInfo = ActivityManager.Instance.GetPassPortInfo();
                passPortInfo.PassPortLv = msg.Level;
                passPortInfo.PassPortExp = msg.Exp;
                passPortInfo.CounterAfterTopLevel = msg.CounterAfterToplevel;
                passPortInfo.ClaimedCounterAfterTopLevel = msg.ClaimedCounterAfterToplevel;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PASSPORT_GetSubTask);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimPassSubTaskExp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
