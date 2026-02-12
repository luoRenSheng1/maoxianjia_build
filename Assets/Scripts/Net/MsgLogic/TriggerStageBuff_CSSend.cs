using msg;

namespace Engine
{
    using EngineBase;
    
    public class TriggerStageBuff_CSSend : ISender
    {
        public TriggerStageBuff_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_TriggerStageBuff_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as TriggerStageBuff_CS;
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
