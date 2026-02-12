using System;

namespace RedDotSystem
{
    public class RedDotTreeNode
    {
        /// <summary>
        /// 红点类型
        /// </summary>
        public RedDotType RedDotType;
        /// <summary>
        /// 父节点
        /// </summary>
        public RedDotDefine parentNode;
        /// <summary>
        /// 当前节点
        /// </summary>
        public RedDotDefine node;

        public bool redDotActive;
        public int redDotCount;
        public Action<RedDotTreeNode> LogicHandler;
        public Action<RedDotType, bool, int> OnRedDotActiveChange;

        public virtual bool RefreshRedDotState()
        {
            redDotCount = 0;
            if (RedDotType == RedDotType.RedDotNodeNum)
            {
                //获取子节点显示的红点个数，去显示红点个数
                redDotCount = RedDotSystem.Instance.GetChildNodeRedDotCount(node);
                redDotActive = redDotCount > 0;
            }
            else
            {
                redDotCount = RefreshRedDotCount();
            }
            LogicHandler?.Invoke(this);

            if (RedDotType == RedDotType.RedDotDataNum)
                redDotActive = redDotCount > 0;
            
            OnRedDotActiveChange?.Invoke(RedDotType, redDotActive, redDotCount);

            return redDotActive;
        }
        
        /// <summary>
        /// 刷新红点个数
        /// </summary>
        /// <returns></returns>
        public virtual int RefreshRedDotCount()
        {
            return 1;
        }
    }
}