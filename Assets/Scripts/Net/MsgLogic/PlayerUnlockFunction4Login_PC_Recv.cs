using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerUnlockFunction4Login_PC_Recv : IReceiver
    {
        public PlayerUnlockFunction4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerUnlockFunction4Login_PC;
        }

        public void Process()
        {
            foreach (var item in msg.UnlockFunctionsList)
            {
                FuncPreviewManger.Instance.UpdateFunInfo((int) item.FuncId, item.IsClaimAward ? 2 : 0);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_FUN_PREVIEW_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerUnlockFunction4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
