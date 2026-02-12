using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GetCopyFreeTimesByAD_SCRecv : IReceiver
    {
        public GetCopyFreeTimesByAD_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetCopyFreeTimesByAD_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ItemInfoManager.Instance.GetItemData(msg.TicketItemId).count = msg.Tickets;
                AdManager.Instance.SetAdFreeTime(1000+(int)msg.CopyType, (int) msg.FreeTimesAd);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DUNGEON_STAGE_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GetCopyFreeTimesByAD_SC.ParseFrom(mRecv.obj);
            return true;
        }
        
        
    }
}
