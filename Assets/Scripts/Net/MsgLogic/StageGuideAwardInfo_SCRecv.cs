using System.Collections.Generic;
using EngineBase;
using msg;

namespace Engine
{
    public class StageGuideAwardInfo_SCRecv : IReceiver
    {
        public StageGuideAwardInfo_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_StageGuideAwardInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<uint> buyListCopy = new List<uint>(msg.PurchasedChapterList);
                // buyListCopy.Add(3001);
                // buyListCopy.Add(4009);
                // buyListCopy.Add(6001);
                ActivityManager.Instance.UpdateGuideBuyIdInfo(buyListCopy);
                
                List<uint> getIdListCopy = new List<uint>(msg.ClaimedStageAwardList);
                // getIdListCopy.Add(2001);
                // getIdListCopy.Add(4001);
                ActivityManager.Instance.UpdateGuideGetIdInfo(getIdListCopy);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CRAZY_GUIDE_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = StageGuideAwardInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}