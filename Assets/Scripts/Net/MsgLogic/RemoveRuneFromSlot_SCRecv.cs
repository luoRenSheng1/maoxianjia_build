using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class RemoveRuneFromSlot_SCRecv : IReceiver
    {
        public RemoveRuneFromSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RemoveRuneFromSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RuneInfo runeInfo =  RuneInfoManager.Instance.GetRune(msg.RuneGuid);
                runeInfo.BattleIndex = -1;
                RuneSlotInfo slotInfo = new RuneSlotInfo();
                slotInfo.SlotId = (int)msg.RuneSlot.SlotId;
                slotInfo.RuneGuid = msg.RuneSlot.RuneGuid;
                slotInfo.ItemId = (int) msg.RuneSlot.ItemId;
                slotInfo.Status = msg.RuneSlot.SlotStatus;
                RoleManager.Instance.AddRuneSlotInfo(slotInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_RUNE_SC_SUCC);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = RemoveRuneFromSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
