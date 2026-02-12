using System.Collections.Generic;
using EngineBase;

namespace Engine
{
    
    public class TalentInfoManager : TSingleton<TalentInfoManager>
    {
        private Dictionary<int,int> _talentDict = new Dictionary<int,int>();
        private int _talentPoints;

        public void UpdateTalentDict(int talentId, int level)
        {
            _talentDict[talentId] = level;
        }

        public Dictionary<int,int> GetTalentDict()
        {
            return _talentDict;
        }

        public void DeleteTalent(int talentId)
        {
            _talentDict.Remove(talentId);
        }

        public void UpdateTalentPoints(int talentPoints)
        {
            _talentPoints = talentPoints;
        }

        public int GetTalentPoints()
        {
            return _talentPoints;
        }
    }
}