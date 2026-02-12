using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MailMark_SCRecv : IReceiver
    {
        public MailMark_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailMark_SC;
        }

        public void Process()
        {
            foreach (var item in msg.MailGuidList)
            {
                MailInfo mailInfo = MailManager.Instance.GetMailInfo(item);
                mailInfo.Status = msg.Status;
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_MAIL_LIST_UPDATE);

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MailMark_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
