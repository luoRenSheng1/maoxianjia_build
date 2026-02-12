using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MailList_SCRecv : IReceiver
    {
        public MailList_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailList_SC;
        }

        public void Process()
        {
            foreach (var item in msg.MailsList)
            {
                MailInfo mailInfo = new MailInfo();
                mailInfo.Read(item);
                MailManager.Instance.UpdateMailInfo(mailInfo);
            }
            if(msg.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_MAIL_LIST_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MailList_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
