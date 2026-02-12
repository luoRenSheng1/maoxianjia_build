using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerGuideID4Login_PC_Recv : IReceiver
    {
        public PlayerGuideID4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerGuideID4Login_PC;
        }

        public void Process()
        {
            foreach (var guideId in msg.GuideIdList)
            {
                GuideManager.Instance.PushCompleteGuide((int) guideId);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_COMPLETE_GUIDE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerGuideID4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
