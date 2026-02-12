using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class Copy_End_SCRecv : IReceiver
    {
        public Copy_End_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Copy_End_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RoleData roleData = DataManager.Instance.mRoleData;
                ConfigDungeonStageUnit stageUnit = ConfigUtils.GetDungeonStageById((int) msg.CopyInfo.CopyStageId, (int) msg.CopyInfo.CopyType);
                roleData.SetDungeonStage((int) msg.CopyInfo.CopyType, stageUnit.Stage);
                if(!msg.IsWin) return;
                ItemInfoManager.Instance.GetItemData(msg.CopyInfo.TicketItemId).count = msg.CopyInfo.Tickets;
                List<ItemData> ret = new List<ItemData>();
                
                PlayerAttrUtils.GetItemDataWithGuid(msg.RewardItemsList.ToList(), ref ret);
        
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                UIManager.Instance.ShowUIPanel("GetDungeonReward", ret, (int) msg.CopyInfo.CopyType,stageUnit.Stage);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_STAGE_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = Copy_End_SC.ParseFrom(mRecv.obj);
            return true;
        }
        
        
    }
}
