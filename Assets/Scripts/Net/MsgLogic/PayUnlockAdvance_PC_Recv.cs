using msg;

namespace Engine
{
    using EngineBase;
    
    public class PayUnlockAdvance_PC_Recv : IReceiver
    {
        public PayUnlockAdvance_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PayUnlockAdvance_PC;
        }

        public void Process()
        {
            PassPortInfo passPortInfo = ActivityManager.Instance.GetPassPortInfo();
            passPortInfo.PassPortLv = msg.Level;
            passPortInfo.PassPortExp = msg.Exp;
            passPortInfo.CounterAfterTopLevel = msg.CounterAfterToplevel;
            passPortInfo.ClaimedCounterAfterTopLevel = msg.ClaimedCounterAfterToplevel;
            passPortInfo.UnlockTier = msg.UnlockTier;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PayUnlockAdvance_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
