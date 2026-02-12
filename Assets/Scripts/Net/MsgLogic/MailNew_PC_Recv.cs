using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MailNew_PC_Recv : IReceiver
    {
        public MailNew_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailNew_PC;
        }

        public void Process()
        {
            int mailType = (int) msg.MailType;
            int total = (int) msg.Total;
            MailInfo mailInfo = new MailInfo();
            mailInfo.Read(msg.Mail);
            MailManager.Instance.UpdateMailInfo(mailInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MailNew_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
