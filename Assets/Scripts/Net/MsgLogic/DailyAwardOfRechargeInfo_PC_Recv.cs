using EngineBase;
using msg;

namespace Engine
{
    public class DailyAwardOfRechargeInfo_PC_Recv : IReceiver
    {
        public DailyAwardOfRechargeInfo_PC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_DailyAwardOfRechargeInfo_PC;
        }

        public void Process()
        {
            ActivityManager.Instance.UpdateDailyAwardInfo((int) msg.RechargeDaysOfDailyaward,1);
            ActivityManager.Instance.GetWaitingAssignDays(msg.WaitingAssignDays);
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.x);

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DailyAwardOfRechargeInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}