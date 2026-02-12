using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace ThirdParty
{
    public class CBaseTimer
    {
        float m_fInterval = 0.0f;
        float m_tBaseTime = 0.0f;
        bool m_bRealTime = true;

        public CBaseTimer()
        {
            m_tBaseTime = 0;
            m_fInterval = 0;
        }

        void Update()
        {
            if (m_bRealTime)
            {
                m_tBaseTime = RealTime.time;
            }
            else
            {
                m_tBaseTime = Time.time;
            }

            // 容错
            if (m_tBaseTime <= 0)
            {
                m_tBaseTime = 1;
            }
        }

        public bool IsTimeOut()
        {
            if (m_bRealTime)
            {
                return RealTime.time >= m_tBaseTime + m_fInterval;
            }
            else
            {
                return Time.time >= m_tBaseTime + m_fInterval;
            }
        }

        public bool ToNextTime()
        {
            if (IsActive() && IsTimeOut())
            {
                Update();
                return true;
            }

            return false;
        }

        public void Startup(float fSecond)
        {
            m_fInterval = fSecond;
            Update();
        }

        public void Startup(float fSecond, bool bRealTime)
        {
            m_fInterval = fSecond;
            m_bRealTime = bRealTime;
            Update();
        }

        public bool TimeOver()
        {
            if (IsActive() && IsTimeOut())
            {
                Clear();
                return true;
            }

            return false;
        }

        public void ToTimeOver()
        {
            if (IsActive())
            {
                if (m_bRealTime)
                {
                    m_tBaseTime = RealTime.time - m_fInterval;
                }
                else
                {
                    m_tBaseTime = Time.time - m_fInterval;
                }
            }
        }

        public void Clear()
        {
            m_fInterval = 0;
            m_tBaseTime = 0;
        }

        public bool IsActive()
        {
            return m_tBaseTime != 0;
        }

        public float GetInterval()
        {
            return m_fInterval;
        }

        public float GetRemain()
        {
            float fResult = 0;

            if (IsActive())
            {
                if (m_bRealTime)
                {
                    fResult = m_fInterval - (RealTime.time - m_tBaseTime);
                }
                else
                {
                    fResult = m_fInterval - (Time.time - m_tBaseTime);
                }

                if (fResult < 0)
                {
                    fResult = 0;
                }
                else if (fResult > m_fInterval)
                {
                    fResult = m_fInterval;
                }
            }

            return fResult;
        }

        public float GetPass()
        {
            float fResult = 0;

            if (IsActive())
            {
                if (m_bRealTime)
                {
                    fResult = RealTime.time - m_tBaseTime;
                }
                else
                {
                    fResult = Time.time - m_tBaseTime;
                }

                if (fResult < 0)
                {
                    fResult = 0;
                }
                else if (fResult > m_fInterval)
                {
                    fResult = m_fInterval;
                }
            }

            return fResult;
        }

        public float GetPassPrecent()
        {
            if (IsActive() && m_fInterval != 0)
            {
                return GetPass() / m_fInterval;
            }

            return 1.0f;
        }

        public float GetRemainPrecent()
        {
            if (IsActive() && m_fInterval != 0)
            {
                return GetRemain() / m_fInterval;
            }

            return 0.0f;
        }
    }
}