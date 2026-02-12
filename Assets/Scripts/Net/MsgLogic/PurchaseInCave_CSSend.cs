using msg;

namespace Engine
{
    /// <summary>
    /// 购买奇遇山洞道具（发送）
    /// </summary>
    public class PurchaseInCave_CSSend : ISender
    {
        public PurchaseInCave_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PurchaseInCave_CS;
        }

        public bool Build(object data)
        {
            msg = data as PurchaseInCave_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            send.obj = msg.ToByteArray(); ;
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}