using msg;

namespace Engine
{
    using EngineBase;
    
    public class PlayerLogin_CSSend : ISender
    {
        public PlayerLogin_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerLogin_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PlayerLogin_CS;
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
