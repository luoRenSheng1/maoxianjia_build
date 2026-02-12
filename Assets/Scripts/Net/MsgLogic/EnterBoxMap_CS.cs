using msg;

namespace Engine
{
    /// <summary>
    /// 进入深埋宝藏小地图（发送）
    /// </summary>
    public class EnterBoxMap_CSSend : ISender
    {
        public EnterBoxMap_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterBoxMap_CS;
        }

        public bool Build(object data)
        {
            msg = data as EnterBoxMap_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            //Debug.Log("======EnterBoxMap_CS Send======");
            send.obj = msg.ToByteArray();
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}