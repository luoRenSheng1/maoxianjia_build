using msg;
using UnityEngine;

namespace Engine
{
    public class PM_SCRecv : IReceiver
    {
        public Pm_SC msg;
        public int MsgID()
        {
            return (int)eMsgID.eMsg_Pm_SC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("PM_CS_Proto2_SCRecv Process");
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                LogUtils.Log("PM命令使用成功");
                LogUtils.Log(msg.StrParam);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = Pm_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}