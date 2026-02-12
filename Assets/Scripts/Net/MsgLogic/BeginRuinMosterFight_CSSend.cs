using msg;

namespace Engine
{
    public class BeginRuinMosterFight_CSSend : ISender
    {
        public BeginRuinMosterFight_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BeginRuinMosterFight_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BeginRuinMosterFight_CS;
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