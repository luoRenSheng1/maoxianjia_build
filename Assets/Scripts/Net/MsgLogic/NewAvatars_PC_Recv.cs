using EngineBase;
using msg;

namespace Engine
{
    public class NewAvatars_PC_Recv : IReceiver
    {
        public NewAvatars_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewAvatars_PC;
        }

        public void Process()
        {
            foreach (var item in msg.AvatarsList)
            {
                RoleManager.Instance.UpdateAvatarListInfo((int) item.Id);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_AVATARS_UPDATE);
            }
                
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_AVATARS_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewAvatars_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}