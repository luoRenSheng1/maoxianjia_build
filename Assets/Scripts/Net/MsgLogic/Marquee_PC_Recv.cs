using msg;

namespace Engine
{
    using EngineBase;
    
    public class Marquee_PC_Recv : IReceiver
    {
        // public Marquee_PC msg;

        public int MsgID()
        {
            // return (int) eMsgID.eMsg_Marquee_PC;
            return 0;
        }

        public void Process()
        {
            // MarqueeManager.Instance.UpdateMarqueeInfo(msg.Marquee);
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_MARQUEE_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            // msg = Marquee_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}