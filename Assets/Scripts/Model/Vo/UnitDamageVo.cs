using System;

namespace Engine
{
    /// <summary>
    /// 服务器下发客户端数据结构
    /// </summary>
    public class UnitDamageVo
    {
        public int unitID { get; set; }
        public int targetID { get; set; }
        public int skillID { get; set; }

        private double _damage;
        public double damage
        {
            get
            {
                _damage = Convert.ToDouble(_damage.ToString("0.00"));
                return Math.Ceiling(_damage);
            }

            set => _damage = value;
        }

        public double hp { get; set; }
        public EN_DAMAGE_TYPE damageType { get; set; }
    }
}
