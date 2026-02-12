using msg;

namespace Engine
{
    using EngineBase;
    
    public class TalentUpdate_PC_Recv : IReceiver
    {
        public TalentUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_TalentUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.TalentsList)
            {
                TalentInfoManager.Instance.UpdateTalentDict((int)item.TalentId,(int)item.Level);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TALENT_UPDATE);
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SKILLInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = TalentUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}