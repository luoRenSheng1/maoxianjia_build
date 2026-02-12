//时间脉冲
using System;
using System.Collections;
using UnityEngine;
using EngineBase;

namespace Engine
{
    public class MinutePulseManager : TSingleton<MinutePulseManager>
    {
        private Coroutine _coroutine;
        private DateTime lastCheckedTime = new DateTime();
        CTimer timer = new CTimer();

        public void OnInit()
        {
            lastCheckedTime = UnbiasedTime.Instance.Now();
            
            if (null != _coroutine)
                GameManager.Instance.StopCoroutine(_coroutine);
            
            _coroutine = GameManager.Instance.StartCoroutine(MinutePulseCoroutine());
            
            timer.Startup(120.0f);
        }
        
        IEnumerator MinutePulseCoroutine()
        {
            lastCheckedTime = UnbiasedTime.Instance.Now();

            while (true)
            {
                DateTime currentTime = UnbiasedTime.Instance.Now();
                if (currentTime.Minute != lastCheckedTime.Minute)
                {
                    FullMinute(currentTime);
                }

                if (currentTime.Hour != lastCheckedTime.Hour)
                {
                    FullClock(currentTime);
                }

                if (UnbiasedTime.Instance.IsPassNewDay(currentTime, lastCheckedTime))
                {
                    FullDay(currentTime);
                }
                lastCheckedTime = currentTime;

                //模拟在线打点
                if (null != GameManager.Instance.Connection 
                    && GameManager.Instance.Connection.IsConnected())
                {
                    if (timer.ToNextTime())
                    {
                        ThinkingDataTrack();
                    }
                }

                yield return GameManager.Instance.waitSec1;
            }
        }
        
        /// <summary>
        /// 整分
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="lastTime"></param>
        private void FullMinute(DateTime currentTime)
        {
            //LogUtils.Log($"New minute reached: {currentTime.Minute}");
            
            //添加每5分钟执行一次操作的逻辑
            if (currentTime.Minute % 5 == 0)
            {
                FullFiveMinute(currentTime);
            }
        }
        
        /// <summary>
        /// 整点
        /// </summary>
        /// <param name="currentTime"></param>
        private void FullClock(DateTime currentTime)
        {
            //LogUtils.Log($"New hour reached: {currentTime.Hour}");
            
            //添加整点逻辑
            
        }
        
        /// <summary>
        /// 整天
        /// </summary>
        /// <param name="currentTime"></param>
        private void FullDay(DateTime currentTime)
        {
        }
        
        /// <summary>
        /// 扩展每五分钟
        /// </summary>
        /// <param name="currentTime"></param>
        private void FullFiveMinute(DateTime currentTime)
        {
            //LogUtils.Log($"New minute reached: {currentTime.Minute}");
            
        }

        /// <summary>
        /// 启动模拟在线打点 
        /// </summary>
        /// <param name="currentTime"></param>
        private void ThinkingDataTrack()
        {
            //LogUtils.LogWarning("OnHeartbeat");
        }
    }
}