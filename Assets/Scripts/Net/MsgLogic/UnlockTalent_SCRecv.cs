using System.Collections.Generic;
using System.Linq;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class UnlockTalent_SCRecv : IReceiver
    {
        public UnlockTalent_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_UnlockTalent_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // List<ItemData> ret = new List<ItemData>();
                // PlayerAttrUtils.GetItemData(msg.ItemsList.ToList(),ref ret);
                foreach (var item in msg.ItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int)item.Id,(int)item.Num);
                }
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
                foreach (var item in msg.TalentsList)
                {
                    TalentInfoManager.Instance.UpdateTalentDict((int)item.TalentId,(int)item.Level);
                }
                
                TalentInfoManager.Instance.UpdateTalentPoints((int)msg.TalentPoints);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TALENT_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = UnlockTalent_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}