using msg;

namespace Engine
{
    /// <summary>
    /// 离开深埋宝藏小地图（发送）
    /// </summary>
    public class ExitBoxMap_CSSend : ISender
    {
        public ExitBoxMap_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExitBoxMap_CS;
        }

        public bool Build(object data)
        {
            msg = data as ExitBoxMap_CS;
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