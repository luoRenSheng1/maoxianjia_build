using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class FinishedGuideID_SCRecv : IReceiver
    {
        public FinishedGuideID_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FinishedGuideID_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int guide = 0;
                foreach (var guideId in msg.GuideIdList)
                {
                    guide = guideId;
                    if (msg.IsHardcore)
                    {
                        GuideManager.Instance.PushCompleteGuide(guideId);
                    }
                    else
                    {
                        GuideManager.Instance.RemoveCompleteGuide(guideId);
                    }
                }

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_COMPLETE_GUIDE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = FinishedGuideID_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
