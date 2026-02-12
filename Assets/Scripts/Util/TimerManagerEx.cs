using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

namespace EngineBase
{
    //TODO: 
    //1. support loop
    //2. support arbitrary delegate
    public class TimerManagerEx
    {
        public bool Log { get; set; } = false;

        private float InternalTime = 0;
        private bool bHaveDoneTimer = false;
        private List<TimerData> timers = new List<TimerData>();

        private static TimerManagerEx _instance;

        public static TimerManagerEx Instance
        {
            get { return _instance; }
        }

        public TimerManagerEx()
        {
        }

        public void Init()
        {
            _instance = this;

            if (Log)
            {
                Debug.LogWarning("TimerManager::Init");
            }
        }

        private static bool DoneUIResLoad(TimerData timer)
        {
            if (null == timer || timer.bDone)
            {
                return true;
            }

            return false;
        }

        public void Tick(float deltaSeconds)
        {
            InternalTime += deltaSeconds;

            if (bHaveDoneTimer)
            {
                timers.RemoveAll(DoneUIResLoad);
                bHaveDoneTimer = false;
            }

            int count = timers.Count;

            for (int i = 0; i < count && i < timers.Count; ++i)
            {
                TimerData timer = timers[i];

                if (InternalTime > timer.expireTime && !timer.bDone)
                {
#if !UNITY_EDITOR
                    try
                    {
#endif
                        timer.bDone = true;
                        bHaveDoneTimer = true;

                        if (timer.param != null)
                        {
                            timer.callback1.DynamicInvoke(timer.param);
                        }
                        else
                        {
                            timer.callback.Invoke();
                        }
#if !UNITY_EDITOR
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
#endif
                }
            }
        }

        public TimerData SetTimer(float time, Action callback)
        {
            TimerData timer = new TimerData();
            timer.expireTime = InternalTime + time;
            timer.callback = callback;
            timer.callback1 = null;
            timer.param = null;
            timer.bDone = false;
            this.timers.Add(timer);
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::SetTimer cb({0}) source({1}) id({2})", callback != null ? callback.ToString() : "", EN_TIMER_SOURCE.NONE, 0);
            }
            return timer;
        }

        public TimerData SetTimer(float time, Action<object> callback, object param)
        {
            TimerData timer = new TimerData();
            timer.expireTime = InternalTime + time;
            timer.callback = null;
            timer.callback1 = callback;
            timer.param = param;
            timer.bDone = false;
            this.timers.Add(timer);
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::SetTimer cb({0}) source({1}) id({2})", callback != null ? callback.ToString() : "", EN_TIMER_SOURCE.NONE, 0);
            }
            return timer;
        }

        public TimerData SetTimer(EN_TIMER_SOURCE timerSource, int sourceid, float time, Action callback)
        {
            TimerData timer = new TimerData();
            timer.expireTime = InternalTime + time;
            timer.callback = callback;
            timer.callback1 = null;
            timer.param = null;
            timer.source = timerSource;
            timer.sourceid = sourceid;
            timer.bDone = false;
            this.timers.Add(timer);
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::SetTimer cb({0}) source({1}) id({2})", callback != null ? callback.ToString() : "", timerSource, sourceid);
            }
            return timer;
        }
        
        public TimerData SetTimer(EN_TIMER_SOURCE timerSource, int sourceid, float time, Action<object> callback, object param)
        {
            TimerData timer = new TimerData();
            timer.expireTime = InternalTime + time;
            timer.callback = null;
            timer.callback1 = callback;
            timer.param = param;
            timer.source = timerSource;
            timer.sourceid = sourceid;
            timer.bDone = false;
            this.timers.Add(timer);
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::SetTimer cb({0}) source({1}) id({2})", callback != null ? callback.ToString() : "", timerSource, sourceid);
            }
            return timer;
        }

        public void ClearTimer(Action callback)
        {
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::ClearTimer cb({0})", callback != null ? callback.ToString() : "");
            }

            if (callback == null)
            {
                return;
            }

            foreach (var timer in timers)
            {
                if (timer != null && timer.callback == callback)
                {
                    timer.bDone = true;
                    bHaveDoneTimer = true;
                    break;
                }
            }
        }
        
        public void ClearTimer(Action<object> callback)
        {
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::ClearTimer cb({0})", callback != null ? callback.ToString() : "");
            }

            if (callback == null)
            {
                return;
            }

            foreach (var timer in timers)
            {
                if (timer != null && timer.callback1 == callback)
                {
                    timer.bDone = true;
                    bHaveDoneTimer = true;
                    break;
                }
            }
        }
                
        public void ClearTimerBySourceID(EN_TIMER_SOURCE timerSource, int sourceid)
        {
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::ClearTimerBySourceID source({0}) id({1})", timerSource, sourceid);
            }

            foreach (var timer in timers)
            {
                if (timer != null && timer.source == timerSource && timer.sourceid == sourceid)
                {
                    timer.bDone = true;
                    bHaveDoneTimer = true;
                }
            }
        }

        public void ClearTimerBySource(EN_TIMER_SOURCE source)
        {
            if (Log)
            {
                Debug.LogWarningFormat("TimerManager::ClearTimerBySourceID source({0})", source);
            }

            foreach (var timer in timers)
            {
                if (timer != null && timer.source == source)
                {
                    timer.bDone = true;
                    bHaveDoneTimer = true;
                }
            }
        }

        public void ClearAll(bool isNotBack = false)
        {
            if(GameManager.Instance.OnAppquit) return;
            int count = timers.Count;

            for (int i = 0; i < count && i < timers.Count; ++i)
            {
                TimerData timer = timers[i];
                if (timer.param != null)
                {
                    if(!timer.bDone && !isNotBack)
                        timer.callback1.DynamicInvoke(timer.param);
                    ClearTimer(timer.callback1);
                }
                else
                {
                    if(!timer.bDone && !isNotBack)
                        timer.callback.Invoke();
                    ClearTimer(timer.callback);
                }
            }
            timers.Clear();
        }

        public void Dispose()
        {
            if (Log)
            {
                Debug.LogWarning("TimerManager::Dispose");
            }
        }
    }
}
