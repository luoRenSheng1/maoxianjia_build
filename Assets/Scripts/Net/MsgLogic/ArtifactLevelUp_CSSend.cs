using msg;

namespace Engine
{
    public class ArtifactLevelUp_CSSend : ISender
    {
        public ArtifactLevelUp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ArtifactLevelUp_CS;
        }

        public bool Build(object data)
        {
            msg = data as ArtifactLevelUp_CS;
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