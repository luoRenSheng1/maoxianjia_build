using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MailRemove_PC_Recv : IReceiver
    {
        public MailRemove_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailRemove_PC;
        }

        public void Process()
        {
            foreach (var item in msg.MailGuidList)
            {
                MailManager.Instance.DelMailInfo(item);
            }
            if(msg.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_MAIL_LIST_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MailRemove_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
