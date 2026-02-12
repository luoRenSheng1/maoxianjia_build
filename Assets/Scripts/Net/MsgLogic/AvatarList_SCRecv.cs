using System;
using EngineBase;
using msg;

namespace Engine
{
    public class AvatarList_SCRecv : IReceiver
    {
        public AvatarList_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_AvatarList_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.AvatarsList)
                {
                    RoleManager.Instance.UpdateAvatarListInfo((int) item.Id);
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_AVATARS_UPDATE);
                }
                
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_AVATARS_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AvatarList_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}