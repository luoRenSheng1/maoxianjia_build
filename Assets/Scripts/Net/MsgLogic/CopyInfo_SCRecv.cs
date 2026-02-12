using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class CopyInfo_SCRecv : IReceiver
    {
        public CopyInfo_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_CopyInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.CopyInfoList)
                {
                    ConfigDungeonStageUnit stageUnit = ConfigUtils.GetDungeonStageById((int) item.CopyStageId, (int) item.CopyType);
                    if(stageUnit != null)
                        DataManager.Instance.GetRoleData().SetDungeonStage((int) item.CopyType, stageUnit.Stage);
                    AdManager.Instance.SetAdFreeTime(1000+(int)item.CopyType, (int) item.FreeTimesAd);
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_OPEN_STAGE_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = CopyInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
