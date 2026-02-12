using System.Collections.Generic;
using System.Linq;
using Engine;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PassTaskEnd_PC_Recv : IReceiver
    {
        public PassTaskEnd_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PassTaskEnd_PC;
        }

        public void Process()
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PASSPORT_GetSubTask);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PassTaskEnd_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
