using msg;

namespace Engine
{
    /// <summary>
    /// 进入奇遇山洞小地图（发送）
    /// </summary>
    public class EnterCave_CSSend : ISender
    {
        public EnterCave_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterCave_CS;
        }

        public bool Build(object data)
        {
            msg = data as EnterCave_CS;
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