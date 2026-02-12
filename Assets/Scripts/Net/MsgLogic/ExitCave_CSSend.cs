using msg;

namespace Engine
{
    /// <summary>
    /// 离开奇遇山洞小地图（发送）
    /// </summary>
    public class ExitCave_CSSend : ISender
    {
        public ExitCave_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExitCave_CS;
        }

        public bool Build(object data)
        {
            msg = data as ExitCave_CS;
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