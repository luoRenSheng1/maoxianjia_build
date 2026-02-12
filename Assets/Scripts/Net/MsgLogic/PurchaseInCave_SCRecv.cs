using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    /// <summary>
    /// 购买奇遇山洞道具（接收回包）
    /// </summary>
    public class PurchaseInCave_SCRecv : IReceiver
    {
        public PurchaseInCave_SC msg;
        public int MsgID()
        {
            return (int)eMsgID.eMsg_PurchaseInCave_SC;
        }

        public void Process()
        {
            Debug.Log($"PurchaseInCave_SC Result = {msg.Result}");
            if (msg == null || msg.Result != eErrCode.eErrCode_Success)
            {
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ADVENTURE_CAVE_SHOP, msg.Result);
                return;
            }

            var data = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
            if(data != null)
            {
                foreach (var item in data.batchStuffList)
                {
                    if (item.id == msg.RemainItemsInStore.Id)
                    {
                        item.amount = (int)msg.RemainItemsInStore.Amount;
                        break;
                    }
                }
            }

            //获得道具
            RewardLogic.GiveReward(msg.RewardItems, msg.Finance, 1);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ADVENTURE_CAVE_SHOP, msg.Result);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("PurchaseInCave_SC_Recv Read " + mRecv.Length);
            msg = PurchaseInCave_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}