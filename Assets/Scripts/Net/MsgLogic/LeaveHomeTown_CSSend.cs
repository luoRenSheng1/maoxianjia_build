using msg;

namespace Engine
{
    using EngineBase;
    
    public class LeaveHomeTown_CSSend : ISender
    {
        public LeaveHomeTown_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LeaveHomeTown_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as LeaveHomeTown_CS;
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
