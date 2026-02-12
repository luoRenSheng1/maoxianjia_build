using msg;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 随机宝箱奖励领取
    /// </summary>
    public class ClaimRandomBox_CSSend : ISender
    {
        public ClaimRandomBox_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimRandomBox_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimRandomBox_CS;
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
