using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class TriggerStageBuff_SCRecv : IReceiver
    {
        public TriggerStageBuff_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_TriggerStageBuff_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //提交收集的buff数据  返回成功  回包只会告诉客户端触发成功或失败--调试用，客户端可以忽略
            }
            else
            {
                Debug.LogError("=====TriggerStageBuff_SC=== msg.Result is fails ");
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = TriggerStageBuff_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
