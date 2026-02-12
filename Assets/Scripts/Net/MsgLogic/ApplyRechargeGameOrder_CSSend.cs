using msg;

namespace Engine
{
    using EngineBase;
    
    public class ApplyRechargeGameOrder_CSSend : ISender
    {
        public ApplyRechargeGameOrder_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ApplyRechargeGameOrder_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ApplyRechargeGameOrder_CS;
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
