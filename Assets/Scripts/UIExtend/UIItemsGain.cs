using FairyGUI;
using System;
using UnityEngine;

namespace Engine
{
    /// <summary>
    /// 用例://UIItemsGain itemsGain = new UIItemsGain();
    /// itemsGain.ApplyItemSourceToDestination(this.btnTarget, 0);
    /// itemsGain.StartItemFly(this.btnStart.LocalToGlobal(Vector2.one), 100);
    /// </summary>
    public class UIItemsGain : UIGainBase
    {
        public Action flyComplete;


        protected override void OnCtor()
        {
            base.OnCtor();

            OnScroll(OnScrollSync);
            OnScrolling(OnScrollingSync);
            OnScrollEnd(OnScrollEndSync);
        }

        private void OnScrollSync(GObject context)
        {
            //LogUtils.LogWarning("OnScrollSync");

            if (null != context && null != context.asCom)
            {
                var transition = context.asCom.GetTransition("reacts");
                if (null != transition)
                {
                    transition.Play(() =>
                    {
                        flyComplete?.Invoke();
                    });
                }
            }
        }

        private void OnScrollingSync(GObject pocket, double number)
        {
            // LogUtils.LogWarning($"OnScrollingSync number:{number}");

            if (null != pocket && !pocket.isDisposed)
            {
            }
        }

        private void OnScrollEndSync(GObject context)
        {
            // LogUtils.LogWarning("OnScrollEnd");
            // flyComplete?.Invoke();
            // flyComplete = null;
            UIGainBasePool.ReycleUIGainBase(this);
        }

        private Vector2 GetFlyPos()
        {
            if (receiver != null && !receiver.isDisposed)
            {
                GObject icon = receiver.GetChild("icon");
                if (icon != null)
                    return icon.LocalToGlobal(new Vector2(icon.width / 2, icon.height / 2));
                else
                    return receiver.LocalToGlobal(new Vector2(receiver.width / 2, receiver.height / 2));
            }

            return Vector2.zero;
        }

        private double GetCurrentCurrency()
        {
            return DataManager.Instance.GetRoleData().gold;
        }

        /// <summary>
        /// 应用目标点
        /// </summary>
        /// <param name="pocket"></param>
        /// <param name="type"></param>
        public void ApplyItemSourceToDestination(GComponent pocket, string itemIcon)
        {
            autoSizeFrag = false;
            fragWidth = 30;
            fragHeight = 30;

            SetIcon(UIResource.GetItemUrl(itemIcon));
            SetReceiver(pocket);
        }
        
        /// <summary>
        /// 应用目标点
        /// </summary>
        /// <param name="pocket"></param>
        /// <param name="type"></param>
        public void ApplyItemSourceToDestinationEx(GComponent pocket, string itemIcon)
        {
            autoSizeFrag = false;
            fragWidth = 30;
            fragHeight = 30;

            SetIcon(itemIcon);
            SetReceiver(pocket);
        }

        /// <summary>
        /// 启动道具飞
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="adds"></param>
        /// <param name="flag">是否需要第一阶段移动动画</param>
        public void StartItemFly(Vector2 pos, double adds, bool flag = true)
        {
            //UIInterface.PlayUISound(3024);
            //UIInterface.PlayUISound(3013);
            double value = GetCurrentCurrency();
            SetValue(value, value+adds);
            Start(pos, GetFlyPos(), flag);
        }
    }
}