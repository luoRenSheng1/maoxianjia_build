using System.Collections.Generic;
using System.Linq;
using Engine;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewPassTaskInfo_PC_Recv : IReceiver
    {
        public NewPassTaskInfo_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewPassTaskInfo_PC;
        }

        public void Process()
        {
            PassPortInfo passPortInfo = PlayerAttrUtils.GetPassPartInfoBySever(msg.PassTask);
            ActivityManager.Instance.SetPassPortInfo(passPortInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewPassTaskInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
