using System.Collections.Generic;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class PlayerUnlockFunction_PC_Recv : IReceiver
    {
        public PlayerUnlockFunction_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerUnlockFunction_PC;
        }

        public void Process()
        {
            //解锁新功能的列表
            List<int> funcIds = new List<int>();
            foreach (var item in msg.UnlockFunctionList)
            {
                FunPrevInfo funPrevInfo = FuncPreviewManger.Instance.GetFunInfo((int) item.FuncId);
                if(funPrevInfo.SystemUnit.Show == 2)
                    funcIds.Add((int)item.FuncId);
                FuncPreviewManger.Instance.UpdateFunInfo((int) item.FuncId, item.IsClaimAward ? 2 : 0);
            }
            if(funcIds.Count > 0)
                UIManager.Instance.ShowUIPanel("FuncOpen", funcIds);//打开界面
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_FUN_PREVIEW_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerUnlockFunction_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
