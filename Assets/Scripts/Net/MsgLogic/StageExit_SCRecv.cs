using msg;

namespace Engine
{
    public class StageExit_SCRecv : IReceiver
    {
        public StageExit_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_Exit_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                DataManager.Instance.GetRoleData().battleStatus = (int) msg.Status;
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("StageExit_SCRecv Read " + mRecv.Length);
            
            msg = StageExit_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
