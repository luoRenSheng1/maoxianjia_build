using msg;

namespace Engine
{
    using EngineBase;
    
    public class RingAttrLevelUp_CSSend : ISender
    {
        public RingAttrLevelUp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RingAttrLevelUp_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RingAttrLevelUp_CS;
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
