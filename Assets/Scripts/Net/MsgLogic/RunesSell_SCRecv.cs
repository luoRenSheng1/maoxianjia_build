using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class RunesSell_SCRecv : IReceiver
    {
        public RunesSell_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RunesSell_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.RuneGuidList)
                {
                    RuneInfoManager.Instance.DeleteRuneInfo(item);
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_RuneInfo);
                PlayerAttrUtils.UpdateFinance(msg.Finance, true);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = RunesSell_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
