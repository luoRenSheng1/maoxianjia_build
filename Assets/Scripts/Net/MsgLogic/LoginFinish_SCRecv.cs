using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class LoginFinish_SCRecv : IReceiver
    {
        public LoginFinish_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LoginFinish_SC;
        }

        public void Process()
        {
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = LoginFinish_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
