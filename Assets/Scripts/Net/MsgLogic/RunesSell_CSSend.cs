using msg;

namespace Engine
{
    using EngineBase;
    
    public class RunesSell_CSSend : ISender
    {
        public RunesSell_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RunesSell_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RunesSell_CS;
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
