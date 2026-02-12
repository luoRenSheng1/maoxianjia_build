using System.Numerics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using EngineBase;

namespace Engine
{
    /// <summary>
    /// 服务器下发客户端数据结构
    /// </summary>
    public class HeroVo
    {
        public int unitID { get; set; }
        public string userID { get; set; }
        public int generalsType { get; set; }
        public Vector3 pos { get; set; }
        public Quaternion rot { get; set; }

        public EN_CAMP_TYPE CampType = EN_CAMP_TYPE.HERO;
    }
}
