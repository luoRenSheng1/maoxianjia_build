using System.Collections.Generic;
using System.Linq;
using Engine;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PassTaskInfo_PC_Recv : IReceiver
    {
        public PassTaskInfo_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PassTaskInfo_PC;
        }

        public void Process()
        {
            PassPortInfo passPortInfo = PlayerAttrUtils.GetPassPartInfoBySever(msg.PassTask);
            ActivityManager.Instance.SetPassPortInfo(passPortInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PASSPORT_GetSubTask);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PassTaskInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
