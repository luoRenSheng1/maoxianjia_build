using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetSwitchBetweenBuilds_CSSend : ISender
    {
        public PetSwitchBetweenBuilds_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetSwitchBetweenBuilds_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetSwitchBetweenBuilds_CS;
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
