using msg;

namespace Engine
{
    using EngineBase;
    
    public class AttackWildPetBegin_CSSend : ISender
    {
        public AttackWildPetBegin_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackWildPetBegin_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AttackWildPetBegin_CS;
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
