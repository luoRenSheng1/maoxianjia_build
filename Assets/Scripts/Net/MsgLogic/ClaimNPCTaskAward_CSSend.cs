using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimNPCTaskAward_CSSend : ISender
    {
        public ClaimNPCTaskAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimNPCTaskAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimNPCTaskAward_CS;
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
