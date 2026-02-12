using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class AchievementList_SCRecv : IReceiver
    {
        public AchievementList_SC msg;
        
        public int MsgID()
        {
            return (int)eMsgID.eMsg_AchievementList_SC;
        }
        
        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                AchievementManager.Instance.GetAchievementList().Clear();
                foreach (var item in msg.AchievementInfoList)
                {
                    AchievementInfo achievementInfo = new AchievementInfo();
                    achievementInfo.AId = (int)item.Aid;
                    achievementInfo.Process = (int)item.Process;
                    achievementInfo.Status = (int)item.Status;
                    achievementInfo.NextId = (int)item.NextAid;
                    
                    AchievementManager.Instance.AddAchievementList(achievementInfo);
                    // AchievementManager.Instance.UpdateAchievement(achievementInfo);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ACHIEVEMENT_UPDATE);
                
                UIManager.Instance.ShowUIPanel("Achievement");
                
                // 红点处理
                RedPointInfo redPointInfo = ReddotSysManager.Instance.GetRedPointByType(eRedPointType.eRedPointType_AchievementNotClaim);
                if (redPointInfo != null && !redPointInfo.IsRead)
                {
                    redPointInfo.IsRead = true;
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }
        
        public bool Read(BaseStructRecv mRecv)
        {
            msg = AchievementList_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}