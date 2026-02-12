using msg;

namespace Engine
{
    public class EndRuinMosterFight_CSSend : ISender
    {
        public EndRuinMosterFight_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EndRuinMosterFight_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as EndRuinMosterFight_CS;
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