using msg;

namespace Engine
{
    using EngineBase;
    
    public class NewPlayerSignInClaimAward_CSSend : ISender
    {
        public NewPlayerSignInClaimAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewPlayerSignInClaimAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as NewPlayerSignInClaimAward_CS;
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
