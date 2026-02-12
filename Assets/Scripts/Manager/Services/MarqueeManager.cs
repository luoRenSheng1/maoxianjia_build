using System.Collections.Generic;
using EngineBase;
using FairyGUI;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

namespace Engine
{

    public class MarqueeInfo
    {
        public ulong Id;
        public int Type;
        public string Content;
    }

    public class MarqueeManager : TSingleton<MarqueeManager>
    {
        private Queue<MarqueeInfo> _marqueeInfos = new Queue<MarqueeInfo>();

        private float scrollSpeed = 200f;    // 跑马灯移动速度 
        private float currentX = 0;    // 当前消息的坐标
        private bool isMoving = false;    // 是否正在移动
        private GTextField marqueeContent;
        private GImage marqueeBg;
        private float extraPadding = 80f; // 背景图额外的填充长度

        public void OnInit()
        {
            // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_MARQUEE_UPDATE, this.OnUpdateMarquee);
        }

        public override void Dispose()
        {
            base.Dispose();
            // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_MARQUEE_UPDATE, this.OnUpdateMarquee);
        }

        private void OnUpdateMarquee()
        {
        }

        public void Initialize(GTextField contentField, GImage bg)
        {
            marqueeContent = contentField;
            marqueeBg = bg;
            // InitializeTestData();// 初始化测试数据
        }
        
        // private void InitializeTestData()
        // {
        //     UpdateMarqueeInfo(new MarqueeInfo { Id = 1, Type = 1, Content = "欢迎来到啪嘟游戏世界！" });
        //     UpdateMarqueeInfo(new MarqueeInfo { Id = 2, Type = 2, Content = "祝您游戏愉快！" });
        //     UpdateMarqueeInfo(new MarqueeInfo { Id = 3, Type = 3, Content = "更多活动敬请期待！" });
        // }

        public void MarqueeUpdate()
        {
            if (_marqueeInfos.Count <= 0)
            {
                // marqueeBg.visible = false;// 隐藏背景图
                return;
            }

            if(isMoving) return;
            
            if (_marqueeInfos.Count > 0)
            {
                currentX = GRoot.inst.width;
                marqueeContent.x = currentX;
                marqueeBg.x = currentX;
                
                var queInfo = _marqueeInfos.Dequeue();// 从队列中取出一个消息
                marqueeContent.text = queInfo.Content;
                
                // 设置背景图的宽度与消息内容的宽度匹配
                float textWidth = marqueeContent.width;
                marqueeBg.width = textWidth + extraPadding;
                
                // 设置消息内容在背景图中居中
                marqueeContent.x = currentX + (extraPadding / 2);
                
                marqueeBg.visible = true;// 显示背景图
                
                isMoving = true;
            }
        }
        
        public void Marquee()
        {
            if (isMoving && marqueeContent != null)
            {
                // 计算消息的移动距离
                float moveDistance = Time.deltaTime * scrollSpeed;
            
                // 更新消息的x坐标
                currentX -= moveDistance;
                // 更新背景图的位置
                marqueeBg.x = currentX;
                
                // 更新消息内容的位置，使其在背景图中居中
                marqueeContent.x = currentX + (extraPadding / 2);
            
                // 确保消息和背景图已经完全移出屏幕
                if (currentX < -marqueeBg.width)
                {
                    // 停止移动
                    isMoving = false;
                    
                    if (_marqueeInfos.Count > 0)
                    {
                        MarqueeUpdate();
                    }
                    else
                    {
                        // 所有消息都展示完毕，停止移动
                        isMoving = false;
                        // MarqueeUpdate();
                        marqueeBg.visible = false;// 隐藏背景图
                    }
                }
            
                // 更新消息的位置
                marqueeContent.x = currentX + (extraPadding / 2);
            }
        }

        public void UpdateMarqueeInfo(MarqueeInfo marqueeInfo)
        {
            _marqueeInfos.Enqueue(marqueeInfo);
            MarqueeUpdate();
        }

        public MarqueeInfo GetMarqueeInfo(ulong id)
        {
            foreach (var item in _marqueeInfos)
            {
                if (item.Id == id)
                    return item;
            }

            return null;
        }

        public Queue<MarqueeInfo> GetMarqueeList()
        {
            // 添加处理逻辑，如：按照什么规则排序、类型筛选等等
            
            return _marqueeInfos;
        }

    }
}