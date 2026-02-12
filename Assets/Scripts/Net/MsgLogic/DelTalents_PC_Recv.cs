using msg;

namespace Engine
{
    using EngineBase;
    
    public class DelTalents_PC_Recv : IReceiver
    {
        public DelTalents_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DelTalents_PC;
        }

        public void Process()
        {
            foreach (var item in msg.TalentsList)
            {
                TalentInfoManager.Instance.DeleteTalent((int)item.TalentId);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TALENT_UPDATE);
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SKILLInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DelTalents_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}