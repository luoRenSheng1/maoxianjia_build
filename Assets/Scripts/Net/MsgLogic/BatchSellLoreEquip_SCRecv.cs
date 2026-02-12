using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class BatchSellLoreEquip_SCRecv : IReceiver
    {
        public BatchSellLoreEquip_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchSellLoreEquip_SC;
        }

        public void Process()
        {//卖的装备-成功的话，客户端可以删除
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                
                foreach (var guid in msg.EquipGuidList)
                {
                    ulong equipGuid = guid; //这个装备 移除
                    EquipManager.Instance.DelEquip(equipGuid);
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOREEQUIP_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BatchSellLoreEquip_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}