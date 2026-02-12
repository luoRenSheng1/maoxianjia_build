using System;
using FairyGUI;
using EngineBase;

namespace Engine
{
    // 初始化的时候
    // 预先设置好红点的层级关系
    // UIRedsManager.PreAddNode
    //
    // UI创建的时候，对对应的UIItem
    // UIRedsManager.SetNodeItem
    //
    // 刷新红点
    // UIRedsManager.SetNodeNumber

    public class UIRedsManager : Singleton<UIRedsManager>
    {
        private UIRedsRoot _redsRoot = new UIRedsRoot();

        private UIRedsRoot _uiRedsRoot
        {
            get
            {
                if (null == _redsRoot)
                {
                    _redsRoot = new UIRedsRoot();
                }

                return _redsRoot;
            }
        }

        public void OnInit()
        {
            try
            {
                InitPreAddNodes();
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
                throw;
            }
        }

        #region 制作流程开放接口

        /// <summary>
        /// 预添加节点
        /// </summary>
        /// <param name="gName">组名</param>
        /// <param name="id">当前功能ID（需要红点点的）</param>
        /// <param name="pid">父级功能ID</param>
        /// <param name="number">数字</param>
        /// <param name="format">默认大于0展示数字</param>
        public void PreAddNode(string gName, string id, string pid, int number, int format = -1)
        {
            format = format != -1 ? format : RED_FORMAT.Normal;
            this._uiRedsRoot.PreAddNode(gName, id, pid, number, format);
        }

        /// <summary>
        /// 更新节点UI对象
        /// </summary>
        /// <param name="gName">组名</param>
        /// <param name="id">ID</param>
        /// <param name="item">UI目标</param>
        public void SetNodeItem(string gName, string id, GComponent item, Layout dotLayout)
        {
            this._uiRedsRoot.SetNodeItem(gName, id, item, dotLayout);
        }

        /// <summary>
        /// 更新红点数
        /// </summary>
        /// <param name="gName">组名</param>
        /// <param name="id">ID</param>
        /// <param name="number">数字</param>
        /// <param name="format">默认大于0展示数字</param>
        public void SetNodeNumber(string gName, string id, int number, int format = -1)
        {
            this._uiRedsRoot.SetNodeNumber(gName, id, number, format);
        }

        #endregion

        #region 特殊制作

        /// <summary>
        /// 添加或更新节点
        /// </summary>
        /// <param name="gName">组名</param>
        /// <param name="id">ID</param>
        /// <param name="item">UI目标</param>
        /// <param name="pId">父节点ID</param>
        /// <param name="dotLayout">点布局</param>
        /// <param name="number">数字</param>
        /// <returns>添加的节点</returns>
        public Node AddNodeWithPID(string gName, string id, GComponent item, string pId, Layout dotLayout, int number = -1)
        {
            return this._uiRedsRoot.AddNodeWithPID(gName, id, item, pId, dotLayout, number);
        }

        /// <summary>
        /// 设置本地数字
        /// </summary>
        public void SetLocalNum(Node node, int num)
        {
            this._uiRedsRoot.SetLocalNum(node, num);
        }

        /// <summary>
        /// 清除所有红点
        /// </summary>
        public void ClearAll()
        {
            this._uiRedsRoot.ClearAll();
        }

        /// <summary>
        /// 按组名清除红点
        /// </summary>
        /// <param name="gName">组名</param>
        public void ClearGroupByName(string gName)
        {
            this._uiRedsRoot.ClearGroupByName(gName);
        }

        /// <summary>
        /// 清除红点组
        /// </summary>
        /// <param name="group">组</param>
        public void ClearGroup(Group group)
        {
            this._uiRedsRoot.ClearGroup(group);
        }

        /// <summary>
        /// 清除红点数值
        /// </summary>
        /// <param name="gName">组名</param>
        public void ClearGroupNumber(string gName)
        {
            this._uiRedsRoot.ClearGroupNumber(gName);
        }

        #endregion

        private void InitPreAddNodes()
        {
            //this.PreAddNode("login", "login", null, 0, RED_FORMAT.Numbers);
            //this.PreAddNode("login", "login2", "login", 0, RED_FORMAT.Numbers);
            //this.PreAddNode("login", "login3", "login", 0, RED_FORMAT.WarningMark);
        }
    }
    
    public static class RedsGroup
    {
        //public const string login = "login";
    }
    
    public static class RedsID
    {
        //public const string login = "login";
        //public const string login2 = "login2";
        //public const string login3 = "login3";
    }
}