using msg;

namespace Engine
{
    using EngineBase;
    using UnityEngine;

    //using System.Diagnostics;

    /// <summary>
    /// 随机宝箱奖励领取
    /// </summary>
    public class ClaimRandomBox_SCRecv : IReceiver
    {
        public ClaimRandomBox_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimRandomBox_SC;
        }

        public void Process()
        {   // 领取随机宝箱
            Debug.Log($"领取随机宝箱消息包 {msg.Result}，{msg.EventGuid}");
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int type = MapChapterManager.Instance.GetEventTypeByGuid(msg.EventGuid);
                if(type == (int)eRandomEventType.eRandomEventType_RandomBox)
                {//深埋宝藏
                    var data = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                    if (data != null)
                    {
                        //通知刷新界面
                        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ENTER_RANDOM_BOX_RESULT);
                        RewardLogic.GiveRewardMini(msg.RewardItems, msg.Finance);//给奖励
                    }
                    else
                    {
                        Debug.LogError("领取箱子后没有数据了");
                    }
                }
                else
                {
                    RewardLogic.GiveReward(msg.RewardItems, msg.Finance);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimRandomBox_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
