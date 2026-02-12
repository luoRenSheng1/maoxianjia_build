using msg;

namespace Engine
{
    public class AchievementList_CSSend : ISender
    {
        public AchievementList_CS msg;
        
        public int MsgID()
        {
            return (int) eMsgID.eMsg_AchievementList_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AchievementList_CS;
            return true;
        }
        
        public bool Send(BaseStructSend send)
        {
            send.obj = msg.ToByteArray();
            return true;
        }
        
        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}