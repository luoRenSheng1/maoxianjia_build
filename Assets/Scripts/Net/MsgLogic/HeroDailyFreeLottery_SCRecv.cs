using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class HeroDailyFreeLottery_SCRecv : IReceiver
    {
        public HeroDailyFreeLottery_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroDailyFreeLottery_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.ItemsList.ToList(), ref ret);

                DataManager.Instance.mRoleData.HeroFreeLottery = msg.UsedFreeplay ? 1 : 0;
                if(ret.Count > 0)
                    UIManager.Instance.ShowUIPanel("SummonHeroReward", ret);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_HERO_LOTTERY_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroDailyFreeLottery_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
