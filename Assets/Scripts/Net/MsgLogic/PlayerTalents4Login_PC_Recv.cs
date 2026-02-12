using msg;

namespace Engine
{
    using EngineBase;
    
    public class PlayerTalents4Login_PC_Recv : IReceiver
    {
        public PlayerTalents4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerTalents4Login_PC;
        }

        public void Process()
        {
            foreach (var item in msg.TalentsList)
            {
                TalentInfoManager.Instance.UpdateTalentDict((int)item.TalentId,(int)item.Level);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TALENT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerTalents4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}