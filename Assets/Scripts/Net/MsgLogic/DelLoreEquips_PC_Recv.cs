using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    public class DelLoreEquips_PC_Recv : IReceiver
    {
        public DelLoreEquips_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_DelLoreEquips_PC;
        }

        public void Process()
        {  //删除传承装备
            LogUtils.LogWarningFormat("DelLoreEquips_PC_Recv Process");
            //1、uint64 equip_guid = 1; //客户端可以删除装备
            ulong equipGuid = msg.EquipGuid;   
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("DelLoreEquips_PC_Recv Read " + mRecv.Length);
            msg = DelLoreEquips_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}