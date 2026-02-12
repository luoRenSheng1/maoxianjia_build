using EngineBase;
using msg;

namespace Engine
{
    public class DelAvatars_PC_Recv : IReceiver
    {
        public DelAvatars_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DelAvatars_PC;
        }

        public void Process()
        {
            foreach (var item in msg.AvatarsList)
            {
                RoleManager.Instance.DelAvatars((int) item.Id);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_AVATARS_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DelAvatars_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}