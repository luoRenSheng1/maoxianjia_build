using System.Collections.Generic;
using UnityEngine;
using EngineBase;

namespace Engine
{
    /// <summary>
    /// 服务器下发客户端数据结构
    /// </summary>
    public class CastSkillVo
    {
        public static int BATTLEACTION_BEGIN = 1;
        
        public int battleActionID { get; set; }
        public int unitID { get; set; }
        public int skillID { get; set; }
        public List<UnitDamageVo> lstTarget { get; set; }

        public CastSkillVo()
        {
            battleActionID = BATTLEACTION_BEGIN++;
        }
        
        public void DeserializeMsg(JsonObject jo)
        {
            this.unitID = jo.GetInt("unitID");
            this.skillID = jo.GetInt("skillID");
            this.lstTarget = new List<UnitDamageVo>();
        }

        public void Clear()
        {
            this.unitID = 0;
            this.skillID = 0;
            
            if (this.lstTarget != null)
            {
                this.lstTarget.Clear();
            }
        }
    }
}
