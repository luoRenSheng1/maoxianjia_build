using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AccountCheck_SCRecv : IReceiver
    {
        public AccountCheck_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AccountCheck_SC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("AccountCheck_SCRecv Process {0} ip:{0}  port:{1}  CheckCode:{2}", msg.Result, msg.Ip, msg.Port, msg.CheckCode);

            GameManager.Instance.OnAccountCheck(msg.Result, msg.Ip, msg.Port, msg.CheckCode, msg.UserId);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AccountCheck_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
