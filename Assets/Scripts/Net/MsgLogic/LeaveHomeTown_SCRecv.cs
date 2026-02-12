using msg;

namespace Engine
{
    using EngineBase;

    public class LeaveHomeTown_SCRecv : IReceiver
    {
        public LeaveHomeTown_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LeaveHomeTown_SC;
        }

        public void Process()
        {

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = LeaveHomeTown_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
