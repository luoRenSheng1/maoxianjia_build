using System.Collections.Generic;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class Stage_Failed_SCRecv : IReceiver
    {
        public Stage_Failed_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_Failed_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                DataManager.Instance.GetRoleData().subStageId = (int) msg.LatestNode;
                DataManager.Instance.GetRoleData().stageId = (int) msg.LatestPassedStage;
                DataManager.Instance.GetRoleData().battleStatus = (int) msg.Status;

                ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIdAndNode((int)msg.LatestPassedStage, (int)msg.LatestNode -1);
                if (stageUnit != null)
                {
                    ConfigChapterUnit chapterUnit = ConfigUtils.GetChapterUnitById(stageUnit.Chapter);
                    if (chapterUnit != null)
                    {
                        var stageId = (int) msg.LatestPassedStage;
                        List<ConfigStageUnit> stageUnits = ConfigUtils.GetStageUnitById(stageId);
                        MapObjectManager.Instance.MaxGuanKaMonsterIndex = stageUnits.Count-1;
                        EventDispatcher.GameWorld.DispatchEvent(EventDefine.STAGE_FIGHT_LOSE, true);
                    }
                }
                
                LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                lobbyView?.HideNextStageButton();
                
                MapObjectManager.Instance.InitGuanKaFSM();
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("Stage_Begin_SCRecv Read " + mRecv.Length);
            
            msg = Stage_Failed_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
