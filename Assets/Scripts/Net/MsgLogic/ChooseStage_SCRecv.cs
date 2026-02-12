using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class ChooseStage_SCRecv : IReceiver
    {
        public ChooseStage_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_ChooseStage_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int preChapterId = DataManager.Instance.mRoleData.chapterId;
                int preStageId = DataManager.Instance.mRoleData.stageId;
                DataManager.Instance.mRoleData.chapterId = (int) msg.StartChapterId;
                DataManager.Instance.mRoleData.stageId = (int) msg.StartStageId;

                DataManager.Instance.mRoleData.battleStatus = (int)eBattleStatus.eBattleStatus_Normal;
                Debug.Log("=ChooseStage_SCRecv  =msg.StartStageId="+msg.StartStageId+"=msg.StartChapterId="+msg.StartChapterId);
                
                //TODO 选关卡后，再次挑战按钮隐藏
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.STAGE_FIGHT_LOSE, false);
                
                // UIManager.Instance.CloseUIPanel("StageBigMap");
                
                LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                lobbyView?.OpenBottomMapPanel();
                EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
            }
            else
            {
                Debug.Log("ChooseStage_SCRecv msg.Result = fail,move chooseStage");
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHOOSE_STAGE_RECV_CALLBACK);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ChooseStage_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}