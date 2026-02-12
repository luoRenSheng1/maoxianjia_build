using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetLottery_CSSend : ISender
    {
        public PetLottery_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetLottery_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetLottery_CS;
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
