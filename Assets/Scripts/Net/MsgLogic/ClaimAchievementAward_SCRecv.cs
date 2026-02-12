using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class ClaimAchievementAward_SCRecv : IReceiver
    {
        public ClaimAchievementAward_SC msg;
        
        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimAchievementAward_SC;
        }
        
        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
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

                foreach (var item in msg.AwardAttrList)
                {
                    
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ACHIEVEMENT_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }
        
        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimAchievementAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}