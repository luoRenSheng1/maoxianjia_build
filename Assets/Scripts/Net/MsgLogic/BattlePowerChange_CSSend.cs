using msg;

namespace Engine
{
    using EngineBase;
    
    public class BattlePowerChange_CSSend : ISender
    {
        public BattlePowerChange_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BattlePowerChange_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BattlePowerChange_CS;
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
