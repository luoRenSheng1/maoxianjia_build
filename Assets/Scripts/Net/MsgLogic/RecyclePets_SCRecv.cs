using System.Collections.Generic;
using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class RecyclePets_SCRecv : IReceiver
    {
        public RecyclePets_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RecyclePets_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {//单个宠物回收
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                
                PetInfoManager.Instance.DelPet(msg.PetId);
                
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
            msg = RecyclePets_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}