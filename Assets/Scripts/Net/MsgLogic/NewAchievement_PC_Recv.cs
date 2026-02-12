using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class NewAchievement_PC_Recv : IReceiver
    {
        public NewAchievement_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewAchievement_PC;
        }

        public void Process()
        {
            foreach (var item in msg.AchievementInfoList)
            {
                AchievementInfo achievementInfo = new AchievementInfo();
                achievementInfo.AId = (int)item.Aid;
                achievementInfo.Process = (int)item.Process;
                achievementInfo.Status = (int)item.Status;
                achievementInfo.NextId = (int)item.NextAid;
                
                AchievementManager.Instance.UpdateAchievement(achievementInfo);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ACHIEVEMENT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewAchievement_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}