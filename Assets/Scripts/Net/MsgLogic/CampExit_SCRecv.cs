using msg;

namespace Engine
{
    using EngineBase;
    
    public class CampExit_SCRecv : IReceiver
    {
        public CampExit_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Camp_Exit_SC;
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
            LogUtils.LogWarning("CampExit_SCRecv Read " + mRecv.Length);
            
            msg = CampExit_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
