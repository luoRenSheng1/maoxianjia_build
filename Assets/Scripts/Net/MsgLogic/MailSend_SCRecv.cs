using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MailSend_SCRecv : IReceiver
    {
        public MailSend_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailSend_SC;
        }

        public void Process()
        {
            
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MailSend_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
