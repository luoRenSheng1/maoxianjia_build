using msg;

namespace Engine
{
    using EngineBase;
    
    public class AcceptWildPet_CSSend : ISender
    {
        public AcceptWildPet_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AcceptWildPet_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AcceptWildPet_CS;
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
