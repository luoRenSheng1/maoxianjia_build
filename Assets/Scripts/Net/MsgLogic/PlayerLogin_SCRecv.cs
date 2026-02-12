using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerLogin_SCRecv : IReceiver
    {
        public PlayerLogin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerLogin_SC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("PlayerLogin_SCRecv Process {0}", msg.Result);
           
            GameManager.Instance.OnPlayerLogin(msg.Result);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerLogin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
