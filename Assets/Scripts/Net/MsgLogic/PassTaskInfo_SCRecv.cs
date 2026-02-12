using System.Collections.Generic;
using System.Linq;
using Engine;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PassTaskInfo_SCRecv : IReceiver
    {
        public PassTaskInfo_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PassTaskInfo_SC;
        }

        public void Process()
        {
            PassPortInfo passPortInfo = PlayerAttrUtils.GetPassPartInfoBySever(msg.PassTask);
            ActivityManager.Instance.SetPassPortInfo(passPortInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT); 
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PassTaskInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
