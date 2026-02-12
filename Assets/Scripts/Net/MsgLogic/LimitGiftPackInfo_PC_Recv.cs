using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 限制礼包购买信息请求
    /// </summary>
    public class LimitGiftPackInfo_PC_Recv : IReceiver
    {
        public LimitGiftPackInfo_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LimitGiftPackInfo_PC;
        }

        public void Process()
        {
            foreach (var item in msg.DailyLimitPacksList)
            {
                ShopInfoManager.Instance.SetDailyPackFlag((int) item.PackId, (int) item.PurchaseCounter, item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
                
            foreach (var item in msg.WeeklyLimitPacksList)
            {
                ShopInfoManager.Instance.SetWeekPackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
            
            foreach (var item in msg.DoubleWeeklyLimitPacksList)
            {
                ShopInfoManager.Instance.SetDoublePackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
            
            foreach (var item in msg.MonthlyLimitPacksList)
            {
                ShopInfoManager.Instance.SetMonthlyPackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VIP_INFO_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GIFTPACK_INFO_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = LimitGiftPackInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
