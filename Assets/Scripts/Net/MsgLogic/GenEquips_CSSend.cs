using msg;

namespace Engine
{
    using EngineBase;
    
    public class GenEquips_CSSend : ISender
    {
        public GenEquips_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GenEquips_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GenEquips_CS;
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
