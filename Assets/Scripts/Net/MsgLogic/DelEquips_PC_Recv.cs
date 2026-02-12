using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DelEquips_PC_Recv : IReceiver
    {
        public DelEquips_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DelEquips_PC;
        }

        public void Process()
        {
            EquipManager.Instance.DelEquip(msg.EquipGuid);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DelEquips_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
