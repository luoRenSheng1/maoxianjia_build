using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MailRemove_SCRecv : IReceiver
    {
        public MailRemove_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailRemove_SC;
        }

        public void Process()
        {
            foreach (var item in msg.MailGuidList)
            {
                MailManager.Instance.DelMailInfo(item);
            }

            if (msg.IsEnd)
            {
                UIManager.Instance.ToastByKey(10129);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_MAIL_LIST_UPDATE);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MailRemove_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
