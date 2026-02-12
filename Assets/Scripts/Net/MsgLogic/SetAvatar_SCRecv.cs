using EngineBase;
using msg;

namespace Engine
{
    public class SetAvatar_SCRecv : IReceiver
    {
        public SetAvatar_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_SetAvatar_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RoleData roleData = DataManager.Instance.GetRoleData();
                roleData.avatarID = (int)msg.Id;
                RoleManager.Instance.SetAvatarId((int) msg.Id);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_AVATARS_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SetAvatar_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}