using System.Collections.Generic;
using EngineBase;
using msg;

namespace Engine
{
    public class AchievementInfo
    {
        public int AId;
        public int Process;
        public int Status;
        public int NextId;
    }

    public class AchievementManager : TSingleton<AchievementManager>
    {
        private List<AchievementInfo> _achievementList = new List<AchievementInfo>();

        public void SendToGetAchievement()
        {
            var builder = AchievementList_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AchievementList_CS, builder.Build());
        }

        public void AddAchievementList(AchievementInfo achievementInfo)
        {
            _achievementList.Add(achievementInfo);
        }

        public void UpdateAchievement(AchievementInfo achievementInfo)
        {
            bool isAdd = true;
            for (int i = 0; i < _achievementList.Count; i++)
            {
                if (_achievementList[i].AId == achievementInfo.AId)
                {
                    _achievementList[i] = achievementInfo;
                    isAdd = false;
                    break;
                }
            }

            if (isAdd)
            {
                _achievementList.Add(achievementInfo);
            }
        }

        public List<AchievementInfo> GetAchievementList()
        {
            return _achievementList;
        }
    }
}