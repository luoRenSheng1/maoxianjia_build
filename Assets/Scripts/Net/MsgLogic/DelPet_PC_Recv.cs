using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class DelPet_PC_Recv : IReceiver
    {
        public DelPet_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DelPet_PC;
        }

        public void Process()
        {

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DelPet_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}