using msg;

namespace Engine
{
    using EngineBase;
    
    public class TestMsg_Proto2_CSSend : ISender
    {
        public TestMsg_Proto2_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eTestMsg_Proto2_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as TestMsg_Proto2_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            send.obj = msg.ToByteArray();;
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}
