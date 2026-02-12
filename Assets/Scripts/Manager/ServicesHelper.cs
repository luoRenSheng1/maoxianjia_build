using System;
using System.Collections.Generic;
using EngineBase;

namespace Engine
{
    public delegate void EventDelegate(JsonObject data, JsonObject parama);

    public class ServicesHelper : TSingleton<ServicesHelper>
    {
        private Dictionary<string, EventDelegate> m_eventMap;

        public void OnInit()
        {
            m_eventMap = new Dictionary<string, EventDelegate>();
        }

        public override void Dispose()
        {
            m_eventMap?.Clear();
            base.Dispose();
        }
        
        public void AddEvent(string key, EventDelegate listener)
        {
            if (m_eventMap.ContainsKey(key))
            {
                m_eventMap[key] += listener;
            }
            else
            {
                m_eventMap[key] = listener;
            }
        }

        public void RemoveEvent(string key, EventDelegate listener)
        {
            if (m_eventMap.ContainsKey(key))
            {
                m_eventMap[key] -= listener;
                if (m_eventMap[key] == null)
                {
                    m_eventMap.Remove(key);
                }
            }
        }

        public void ExecuteEvent(string key, JsonObject data, JsonObject param)
        {
            if (m_eventMap.ContainsKey(key))
            {
                m_eventMap[key]?.Invoke(data, param);
            }
        }
        
        public  bool HasEvent(string key)
        {
            return m_eventMap.ContainsKey(key);
        }
    }
}