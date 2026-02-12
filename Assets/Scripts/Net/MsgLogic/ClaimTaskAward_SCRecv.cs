using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ClaimTaskAward_SCRecv : IReceiver
    {
        public ClaimTaskAward_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimTaskAward_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                TaskVo taskVo = new TaskVo();
                taskVo.MainTaskId = (int) msg.NextTaskId;
                taskVo.MainTaskProgress = (int) msg.MainTaskProcess;

                TaskInfoManager.Instance.SetCurMainTask(taskVo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_GET_REWARD);
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.AwardsList.ToList(), ref ret);
        
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                if (ret.Count > 0)
                {
                    // foreach (var item in ret)
                    // {
                    //     ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(item.id);
                    //     if(itemTypeUnit != null)
                    //         TipsManger.Instance.ShowTip(UIResource.GetItemUrl(itemTypeUnit.Icon),itemTypeUnit.Name + "x"+StringUtils.FormatCurrency(item.count));
                    // }
                    // UIManager.Instance.ShowUIPanel("GetReward", ret);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimTaskAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
