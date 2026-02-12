using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class AchievementUpdate_PC_Recv : IReceiver
    {
        public AchievementUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AchievementUpdate_PC;
        }

        public void Process()
        {
            AchievementInfo achievementInfo = new AchievementInfo();
            achievementInfo.AId = (int)msg.AchievementInfo.Aid;
            achievementInfo.Process = (int)msg.AchievementInfo.Process;
            achievementInfo.Status = (int)msg.AchievementInfo.Status;
            achievementInfo.NextId = (int)msg.AchievementInfo.NextAid;
            
            AchievementManager.Instance.UpdateAchievement(achievementInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ACHIEVEMENT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AchievementUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}