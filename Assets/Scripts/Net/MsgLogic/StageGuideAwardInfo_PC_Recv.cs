using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;

namespace Engine
{
    public class StageGuideAwardInfo_PC_Recv : IReceiver
    {
        public StageGuideAwardInfo_PC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_StageGuideAwardInfo_PC;
        }

        public void Process()
        {
            ActivityManager.Instance.UpdateGuideBuyIdInfo(msg.PurchasedChapterList.ToList());
            
            ActivityManager.Instance.UpdateGuideGetIdInfo(msg.ClaimedStageAwardList.ToList());
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CRAZY_GUIDE_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = StageGuideAwardInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}