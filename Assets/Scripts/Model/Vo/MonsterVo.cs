using UnityEngine;
using EngineBase;

namespace Engine
{
    /// <summary>
    /// 服务器下发客户端数据结构
    /// </summary>
    public class MonsterVo
    {
        /// <summary>
        /// 怪物自动编号
        /// </summary>
        public int unitID { get; set; }
        /// <summary>
        /// 怪物配置ID
        /// </summary>
        public int generalsType { get; set; }
        public Vector3 pos { get; set; }
        public Quaternion rot { get; set; }
        
        public bool IsBoss { get; set; }
        public double MonsterGold;
        public int GroupId;
        public int MonsterIndex;

        public void DeserializeMsg(JsonObject jo)
        {
            this.unitID = jo.GetInt("unitId");
            this.generalsType = jo.GetInt("generalsType");
            this.pos = Vector3.zero;
            this.rot = Quaternion.identity;
        }
    }
}
