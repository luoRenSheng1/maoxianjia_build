using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class Copy_Begin_SCRecv : IReceiver
    {
        public Copy_Begin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Copy_Begin_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int dungeonType = (int) msg.CopyType;
                int stage = (int) msg.StartStageId;
                var stageUnit = ConfigUtils.GetDungeonStageById(stage, dungeonType);
                DungeonMapManager.Instance.EndTime = msg.RequiredEndTime;
                UIManager.Instance.CloseUIPanel("DungeonStageDetail");
                UIManager.Instance.ShowUIPanel("DungeonMap", stageUnit);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = Copy_Begin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
