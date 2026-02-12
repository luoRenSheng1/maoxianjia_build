using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class DelPetBooks_PC_Recv : IReceiver
    {
        public DelPetBooks_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DelPetBooks_PC;
        }

        public void Process()
        {

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DelPetBooks_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}