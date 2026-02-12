using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MailExtract_SCRecv : IReceiver
    {
        public MailExtract_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailExtract_SC;
        }

        public void Process()
        {
            List<ItemData> rewardList = new List<ItemData>();
            foreach (var item in msg.ExtractResultsList)
            {
                foreach (var attach in item.AttachsList)
                {
                    rewardList.Add(new ItemData()
                    {
                        id = (int) attach.AttachType,
                        count = attach.AttachNum
                    });
                }

                MailInfo mailInfo = MailManager.Instance.GetMailInfo(item.MailGuid);
                mailInfo.Status = (int) item.Status;
                MailManager.Instance.UpdateMailInfo(mailInfo);
               
            }
            
            DataManager.Instance.mRoleData.gold = msg.Finance.Golds;
            DataManager.Instance.mRoleData.dia = msg.Finance.Diamonds;
            DataManager.Instance.mRoleData.dia2 = msg.Finance.FreeDiamonds;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);

            if (rewardList.Count > 0)
            {
                foreach (var item in rewardList)
                {
                    ItemInfoManager.Instance.AddItemData(item);

                }
                UIManager.Instance.ShowUIPanel("GetReward", rewardList);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_MAIL_LIST_UPDATE);

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MailExtract_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
