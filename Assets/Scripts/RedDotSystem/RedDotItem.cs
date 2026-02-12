using CommonEx;
using FairyGUI;
using FairyGUI.Utils;

namespace RedDotSystem
{
    public class RedDotItem : UI_RedDotComponent
    {
        private RedDotDefine _redKey;

        public void SetRedDotDefine(RedDotDefine redKey)
        {
            this._redKey = redKey;
            RedDotSystem.Instance.RegisterRedDotChangeEvent(_redKey, OnRedDotStateChangeEvent);
            RedDotSystem.Instance.UpdateRedDotState(_redKey);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            RedDotSystem.Instance.UnRegisterRedDotChangeEvent(_redKey, OnRedDotStateChangeEvent);
        }
        
        /// <summary>
        /// 红点状态改变事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="active"></param>
        /// <param name="count"></param>
        public void OnRedDotStateChangeEvent(RedDotType type, bool active, int count)
        {
            redDotObj.visible = active;
            if (type != RedDotType.Normal)
            {
                countText.text = count.ToString();
            }

            countText.visible = (type != RedDotType.Normal);
        }
    }
}