using System.Collections.Generic;
using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class BatchRecyclePets_SCRecv : IReceiver
    {
        public BatchRecyclePets_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchRecyclePets_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);

                foreach (var guid in msg.PetGuidList)
                {
                    ulong petGuid = guid;
                    PetInfoManager.Instance.DelPet(petGuid);
                }

                List<ItemData> rewardList = new List<ItemData>();
                foreach (var item in msg.ItemsList)
                {
                    rewardList.Add(new ItemData()
                    {
                        id = (int)item.Id,
                        count = item.Num
                    });
                }

                foreach (var item in rewardList)
                {
                    ItemInfoManager.Instance.AddItemData(item);
                }

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RECYCLE_PET);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BatchRecyclePets_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}