using UnityEngine;
using EngineBase;

namespace Engine
{
    public class ItemData
    {
        public int id;
        public double count;
        /// <summary>
        /// 最早一个要到期道具的到期时间戳，如果是0表示普通道具
        /// </summary>
        public ulong expiredTime;

        public long gold;//金币
        public long diamond;//钻石
        public ulong ItemGuid;//目前就符石需要

        public ItemData(int id = 0, double count = 0)
        {
            this.id = id;
            this.count = count;
            expiredTime = 0;
        }
    }
}