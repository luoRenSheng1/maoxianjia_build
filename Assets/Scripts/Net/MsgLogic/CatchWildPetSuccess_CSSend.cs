using msg;

namespace Engine
{
    using EngineBase;
    
    public class CatchWildPetSuccess_CSSend : ISender
    {
        public CatchWildPetSuccess_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_CatchWildPetSuccess_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as CatchWildPetSuccess_CS;
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
