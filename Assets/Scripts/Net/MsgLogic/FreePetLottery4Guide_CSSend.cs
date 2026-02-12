using msg;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 引导免费十次抽宠物
    /// </summary>
    public class FreePetLottery4Guide_CSSend : ISender
    {
        public FreePetLottery4Guide_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FreePetLottery4Guide_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as FreePetLottery4Guide_CS;
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
