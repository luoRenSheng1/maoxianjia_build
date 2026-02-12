using System;
using msg;

namespace Engine
{
    public class SocketHeartBeat
    {
        protected Connection context;
        protected ClientPing_CS msg;
        protected bool bStart = false;

        /// <summary>
        /// 心跳包的发送间隔，服务器下发
        /// </summary>
        protected float interval;
        /// <summary>
        /// 心跳包的超时时间，服务器下发，目前无用，客户端超时走不同平台差异
        /// </summary>
        protected float timeOut;
        /// <summary>
        /// 心跳包发送计时器
        /// </summary>
        protected float remainTime;
        /// <summary>
        /// 心跳包发送累计次数
        /// </summary>
        protected int curTimeOutTimes = 0;
        /// <summary>
        /// 心跳包延迟等待1s
        /// </summary>
        protected float disconnectRemainTime;
        /// <summary>
        /// 心跳包延迟等待1s
        /// </summary>
        protected bool willDisconnect = false;
        /// <summary>
        /// 心跳包日志
        /// </summary>
        protected bool logHeart = false;
        /// <summary>
        /// 最近一次心跳包收到时间，真实时间
        /// </summary>
        public float LastHeartBeatTime { get; private set; } = 0.0f;

        public SocketHeartBeat(Connection connection)
        {
            this.context = connection;
            var msgBuild = ClientPing_CS.CreateBuilder();
            this.msg = msgBuild.Build();
        }

        public void Init(float interval, float timeOut)
        {
            this.interval = interval;
            this.timeOut = timeOut;
        }
        
        public void Start()
        {
            this.bStart = true;
            this.remainTime = this.interval;
            this.willDisconnect = false;
            this.logHeart = false;
            this.LastHeartBeatTime = 0.0f;
        }
        
        public void Stop()
        {
            this.bStart = false;
            this.willDisconnect = false;
            this.logHeart = false;
        }
        
        public void OnUpdate(float deltaSeconds)
        {
            if (!this.bStart)
            {
                return;
            }
            
            this.remainTime -= deltaSeconds;

            if (this.willDisconnect)
            {
                this.disconnectRemainTime -= deltaSeconds;
            }

            if (this.remainTime <= 0)
            {
                SendHeartBeat();

                if (this.curTimeOutTimes >= Connection.MAX_TIMEOUT_TIMES)
                {
                    // LogUtils.LogWarning("LobbyHeartBeat Times Limit Will Disconnect");
                    
                    this.willDisconnect = true;
                    this.disconnectRemainTime = 1.0f;
                }
            }

            if (this.willDisconnect && this.disconnectRemainTime <= 0)
            {
                OnTimeout();
            }
        }

        protected void OnTimeout()
        {
            var handler = this.context.EventHandler;

            if (handler != null)
            {
                if (!handler.isWorking() || this.curTimeOutTimes >= Connection.MAX_TIMEOUT_TIMES)
                {
                    LogUtils.LogWarning("LobbyHeartBeat OnTimeout Now Disconnect");

                    this.curTimeOutTimes = 0;
                    this.logHeart = false;
                    GameManager.Instance.Connection.ActiveClose();
                    // 主动断线
                    handler.OnDisconnect(SocketErrorType.HeartBeatTimeout);
                }
            }
        }
        
        /// <summary>
        /// 发送及时心跳包，下一次触发心跳包发送若没回包将触发心跳包超时
        /// </summary>
        public void SendTimelyHeartBeat()
        {
            SendHeartBeat();

            this.curTimeOutTimes = Connection.MAX_TIMEOUT_TIMES;
        }
        
        protected void SendHeartBeat()
        {
            if (this.curTimeOutTimes > 0)
            {
                // 前一次心跳包没返回，开始记录心跳包日志
                this.logHeart = true;
                // LogUtils.LogWarningFormat("SendHeartBeat curTimeOutTimes({0})", curTimeOutTimes);
            }
            
            this.remainTime = interval;
            this.curTimeOutTimes++;
            this.context.SendMessage((int)eMsgID.eMsg_ClientPing_CS, this.msg);
        }
        
        public void OnHeartBeatRecv(int time)
        {
            if (this.logHeart)
            {
                // 心跳包正常返回，停止记录心跳包日志
                this.logHeart = false;
                LogUtils.LogWarningFormat("OnHeartBeatRecv curTimeOutTimes({0})", curTimeOutTimes);
            }
            
            this.remainTime = interval;
            this.curTimeOutTimes = 0;
            this.willDisconnect = false;
        }
    }
}
