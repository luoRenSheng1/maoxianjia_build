using EngineBase;
using msg;

namespace Engine
{
    public class NewPlayerRechargeInfo_PC_Recv : IReceiver
    {
        public NewPlayerRechargeInfo_PC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_NewPlayerRechargeInfo_PC;
        }

        public void Process()
        {
            foreach (var item in msg.SignInfoList)
            {
                ActivityManager.Instance.UpdateFirstPay((int) item.Id, (int) item.ClaimStatus, item.SigninTime);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_FIRST_PAY_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewPlayerRechargeInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}