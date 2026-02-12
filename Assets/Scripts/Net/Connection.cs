using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using EngineBase;
using msg;

namespace Engine
{
    public enum ConnectionState
    {
        Idle,
        Connecting,
        Connected,
        Disconnect,
        Reconnect,
    }

    //Network connection
    public class Connection : IDisposable
    {
        /// <summary>
        /// 断线次数上线
        /// </summary>
        public static int MAX_RECONNECT_TIMES { get; private set; } = 60; //原先15次，改成60次
        /// <summary>
        /// 断线重连间隔
        /// </summary>
        public static float RECONNECT_INTERVAL { get; private set; } = 1.0f;
        /// <summary>
        /// 心跳包超时次数，超过次数主动断线
        /// </summary>
        public static int MAX_TIMEOUT_TIMES { get; private set; } = 3;
        /// <summary>
        /// 断线重连,心跳包最大时间差
        /// </summary>
        public static float MAX_RECONNECT_HEARTBEAT_TIME { get; private set; } = 600.0f;
        
        public static void SetTimeoutTimes(int value)
        {
            MAX_TIMEOUT_TIMES = value;
        }

        public static void SetReconnectTimes(int value)
        {
            MAX_RECONNECT_TIMES = value;
        }
        
        public void InitHeartBeat(float interval)
        {
            if (EventHandler != null)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.ios))
                {
                    // IOS onclose事件有点延迟，减少心跳间隔
                    EventHandler.InitHeartBeat(3.9f, 1.0f);
                }
                else
                {
                    EventHandler.InitHeartBeat(10f, 1.0f);
                }
            }
        }

        public void SendHeartBeatTimely()
        {
            if (EventHandler != null)
            {
                EventHandler.SendHeartBeatTimely();
            }
        }
        
        protected bool useSSL = false;
        protected string host;
        protected int port = 0;

        protected Type socketType;
        protected Type eventHandlerType;
        protected Type recvFactoryType;
        protected Type sendFactoryType;

        protected Socket socket;
        protected SocketEventHandler eventHandler;
        protected SocketReceiverFactory receiverFactory;
        protected SocketSenderFactory senderFactory;

        protected ConnectionState state = ConnectionState.Idle;

        protected Queue<BaseStructSend> sendCache_queue = new Queue<BaseStructSend>();

        protected int reconnectCount = 0;
        protected float timeToReconnect = RECONNECT_INTERVAL;
        protected bool isReconnect = false;
        
        protected long bytesSend = 0;
        protected long bytesReceive = 0;
        
        public void ResetBytesStatistics()
        {
            this.bytesSend = this.bytesReceive = 0;
        }
        
        public void SendBytes(long len)
        {
            this.bytesSend += len;
        }

        public void ReceiveBytes(long len)
        {
            this.bytesReceive += len;
        }

        public long GetTotalBytesTransfered()
        {
            return this.bytesSend + this.bytesReceive;
        }

        public SocketEventHandler EventHandler
        {
            get
            {
                return eventHandler;
            }
        }

        public SocketReceiverFactory ReceiverFactory
        {
            get
            {
                return receiverFactory;
            }
        }
        public SocketSenderFactory SenderFactory
        {
            get
            {
                return senderFactory;
            }
        }

        private ProtocolMessage _protocolMessage;
        private object threadSendLock = new object();

        public ProtocolMessage ProtocolMessage
        {
            get { return _protocolMessage; }
        }
        
        public Connection(MapContext context)
        {
            useSSL = context.useSSL;
            host = context.host;
            port = context.port;
            sendCache_queue.Enqueue(new BaseStructSend());
            _protocolMessage = new ProtocolMessage();
        }
        
        public void Init()
        {
            this.Init(
#if UNITY_WEBGL
                typeof(SystemWebSocket),
#else
                typeof(SystemTCPEx),
#endif
                typeof(SocketEventHandler),
                typeof(SocketReceiverFactory),
                typeof(SocketSenderFactory));
        }

        protected virtual void Init(Type socketType, Type eventHandlerType, Type recvFactoryType, Type sendFactoryType)
        {
#if UNITY_WEBGL
            SetReconnectTimes(10);
#endif
            
            this.socketType = socketType;
            this.eventHandlerType = eventHandlerType;
            this.recvFactoryType = recvFactoryType;
            this.sendFactoryType = sendFactoryType;

            this.socket = (Socket)Activator.CreateInstance(socketType);
            this.socket.Context = this;
            this.socket.Init();

            this.receiverFactory = (SocketReceiverFactory)Activator.CreateInstance(recvFactoryType);
            this.senderFactory = (SocketSenderFactory)Activator.CreateInstance(sendFactoryType);
        }

        protected void ChangeSocketType(Type socketType)
        {
            this.socketType = socketType;

            if (this.socket != null)
            {
                this.socket.Dispose();
                this.socket = null;
            }

            if (this.eventHandler != null)
            {
                this.eventHandler.Dispose();
                this.eventHandler = null;
            }

            this.socket = (Socket)Activator.CreateInstance(socketType);
            this.socket.Context = this;
            this.socket.Init();
        }

        public void ChangeAddr(MapContext context)
        {
            useSSL = context.useSSL;
            host = context.host;
            port = context.port;
        }

        public void ActiveClose()
        {
            state = ConnectionState.Idle;

            if (this.socket != null)
            {
                this.socket.Dispose();
                this.socket = null;
            }

            if (this.eventHandler != null)
            {
                this.eventHandler.StopHeartBeat(false);
            }
        }

        protected virtual void OnConnected()
        {
            if (reconnectCount > 0)
            {
                if (this.eventHandler != null)
                {
                    this.eventHandler.StartHeartBeat();
                }
            }

            reconnectCount = 0;
        }

        public virtual void SetReconnectFlag(bool value)
        {
            isReconnect = value;
        }

        public bool IsReconnectFlag()
        {
            return isReconnect;
        }

        public void Dispose()
        {
            if (socket != null)
                socket.Dispose();
            if (eventHandler != null)
                eventHandler.Dispose();
            if (receiverFactory != null)
                receiverFactory.Dispose();

            socket = null;
            eventHandler = null;
            receiverFactory = null;
        }

        //connect server
        public virtual bool Connect()
        {
            if (this.socket == null)
            {
                this.socket = (Socket)Activator.CreateInstance(socketType);
                this.socket.Context = this;
                this.socket.Init();
                this.state = ConnectionState.Idle;
            }

            if (this.eventHandler == null)
            {
                this.eventHandler = (SocketEventHandler)Activator.CreateInstance(eventHandlerType, this);
                this.eventHandler.Initialize();
            }

            if (String.IsNullOrEmpty(host) || port == 0)
            {
                state = ConnectionState.Idle;
                return false;
            }

            host = host.Trim();

            if ((state != ConnectionState.Connecting && state != ConnectionState.Connected) || (state == ConnectionState.Connected && !IsConnected()))
            {
                state = ConnectionState.Connecting;
                LogUtils.LogWarningFormat("Connection Connect {0} {1}", host, port);
                return socket.Connect(host, port, useSSL);
            }
            else
            {
                return true;
            }
        }

        public virtual void Reconnect()
        {
            if (eventHandler != null)
            {
                // var LastHeartBeatTime = eventHandler.GetLastHeartBeatTime();
                // if (!Mathf.Approximately(LastHeartBeatTime, 0)
                //     && Time.unscaledTime - LastHeartBeatTime > MAX_RECONNECT_HEARTBEAT_TIME)
                // {
                //     LogUtils.LogWarning("Disconnect To Long, Return Login");
                //     
                //     // 超时很久了，不断线重连了，直接回登录
                //     GameManager.Instance.Connection.ActiveClose();
                //     MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
                //     {
                //         OkCallBack = () =>
                //         {
                //             //
                //             ItemInfoManager.Instance.Clear();
                //             EquipManager.Instance.Clear();
                //             UIManager.Instance.CloseAllUIPanel();
                //             UIManager.Instance.ShowUIPanel("Login");
                //         },
                //         CancelBack = () =>
                //         {
                //             EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_RECONNECT_SUCCESS);
                //         }
                //     };
                //     UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(10164), param, true);
                //     return;
                // }
            }
            
            state = ConnectionState.Reconnect;
            
            if (reconnectCount <= 0)
            {
                //timeToReconnect = RECONNECT_INTERVAL;
                // 立即显示Loading
                UIManager.Instance.ShowLoading(RECONNECT_INTERVAL);  //显示断线重连弹窗
            }
        }

        protected virtual void ReconnectInner()
        {
            // keep this in main thread
            if (this.eventHandler != null)
            {
                this.eventHandler.StopHeartBeat(false);
            }
            
            timeToReconnect = RECONNECT_INTERVAL;

            if (reconnectCount >= MAX_RECONNECT_TIMES)
            {
                LogUtils.LogWarning("Lobby Connection Reconnect Fail.");

                this.Disconnect();
                this.DisconnectInner();
                
                UIManager.Instance.ShowUIPanel("Login");
            }
            else
            {
                LogUtils.LogWarning("Lobby Connection Try Reconnect.");

                UIManager.Instance.ShowLoading(90.0f);

                reconnectCount++;
                this.SetReconnectFlag(true);
                this.Connect();
            }
        }

        public virtual void Disconnect()
        {
            state = ConnectionState.Disconnect;
            
            LogUtils.LogWarning("Lobby Disconnect.");

            GameManager.Instance.DisconnectLobbyServer();
        }

        protected virtual void DisconnectInner()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }

            if (eventHandler != null)
            {
                eventHandler.Clear();
            }

            if (this.eventHandler != null)
            {
                this.eventHandler.StopHeartBeat(false);
            }
        }

        public virtual void Tick(float deltaSeconds)
        {
            switch (state)
            {
                case ConnectionState.Connecting:
                    if (IsConnected())
                    {
                        state = ConnectionState.Connected;
                        this.OnConnected();
                    }
                    break;
                case ConnectionState.Disconnect:
                    {
                        DisconnectInner();
                        state = ConnectionState.Idle;
                        return;
                    }
                case ConnectionState.Reconnect:
                    {
                        if (IsConnected())
                        {
                            DisconnectInner();
                            timeToReconnect = RECONNECT_INTERVAL;
                        }

                        if ((timeToReconnect -= deltaSeconds) < 0)
                        {
                            timeToReconnect = RECONNECT_INTERVAL;
                            this.ReconnectInner();
                        }
                        return;
                    }
                case ConnectionState.Connected:
                    break;
                
                default:
                    break;
            }

            if (socket != null)
            {
                socket.OnUpdate(deltaSeconds);
            }
            if (eventHandler != null)
            {
                eventHandler.OnUpdate(deltaSeconds);
            }
        }

        public bool SendMessage(int msgID, object data, object context = null)
        {
            ISender sender = this.senderFactory.CreateSender(msgID);
            // LogUtils.LogFormat("SendMessage:"+sender);
            if (sender == null)
                return false;

            lock (this.threadSendLock)
            {
                if (sender == null)
                {
                    LogUtils.LogError("Sender is NULL!!");
                    return false;
                }

                int id = sender.MsgID();
                if (!sender.Build(data)) // 这部将Data转为不同的protobuf定义的结构
                {
                    LogUtils.LogErrorFormat("Build msg error id = {0}", id);
                    return false;
                }
                BaseStructSend sendCache = null;
                if (!sendCache_queue.TryDequeue(out sendCache))
                {
                    sendCache = new BaseStructSend();
                }
                sendCache.Reset(id);
                if (!sender.Send(sendCache)) // 使用Protobuf将上一部的设置的数据
                {
                    LogUtils.LogErrorFormat("Send msg error id = {0}", id);
                    return false;
                }
                
                if (socket != null && socket.Isconnected(false))
                {
                    sendCache.lobbyNormal(_protocolMessage, context);
#if UNITY_EDITOR
                    LogUtils.LogWarningFormat("Send msg id = {0}  data= {1}", id, data.ToString());   
#endif
                    socket.Send(sendCache.buffer, sendCache.buffer.Length);

                    if(id != (int) eMsgID.eMsg_ServerPing_Apply && id != (int) eMsgID.eMsg_Stage_Begin_CS && id != (int) eMsgID.eMsg_Stage_AwardCommit_CS && id != (int) eMsgID.eMsg_ClientPing_CS)
                        UIManager.Instance.ShowSCLoading(id);
                    //RecordHelper.RecLobbyMsg(id, sendCache.obj.ToString(), MsgType.Send, Context.RemoteType, (int)sendData.type);//录制 发送消息
                    // LogUtils.LogWarningFormat("Send msg id = {0}  data= {1}", id, data.ToString());  
                    sendCache_queue.Enqueue(sendCache);
                    return true;
                }
                else
                {
                    LogUtils.LogWarningFormat("Send msg error id = {0}, Lobby NOT connected", id);
                    sendCache_queue.Enqueue(sendCache);

                    if (this.eventHandler != null)
                    {
                        this.eventHandler.OnConnectFail(SocketErrorType.ConnectionNotConnected);
                    }
                }

                return false;
            }
        }
        
        public bool IsConnected(bool bPrecise = false)
        {
            return socket != null && socket.Isconnected(bPrecise);
        }
        
        public void InitProtocol(JsonObject serverProtos, JsonObject clientProtos)
        {
            if (_protocolMessage == null)
            {
                LogUtils.LogError("DUPLICATED INIT PROTOCOL!!!");
                return;
            }
            
            _protocolMessage.InitProtocol(serverProtos, clientProtos);
        }

        /*
        /// <summary>
        /// 检测是否断线，提高平滑表现，需要延迟1s，1s过程中如果网络层已经率先发现断线，清除定时器
        /// </summary>
        public void CheckConnected()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowLoading(5.0f);
            }

            if (TimerManagerEx.Instance != null)
            {
                TimerManagerEx.Instance.ClearTimer(this.OnCheckConnected);
                TimerManagerEx.Instance.SetTimer(1.0f, this.OnCheckConnected);
            }
        }

        private void ClearCheckConnected()
        {
            if (TimerManagerEx.Instance != null)
            {
                TimerManagerEx.Instance.ClearTimer(this.OnCheckConnected);
            }
        }
        
        private void OnCheckConnected()
        {
            LogUtils.LogWarning("OnCheckConnected");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideLoading();
            }
            
            if (IsConnected(true))
            {
                // 连接中
                LogUtils.LogWarning("CheckConnected Connected");
            }
            else
            {
                // 断线
                LogUtils.LogWarning("CheckConnected Disconnect");

                if (eventHandler != null)
                {
                    eventHandler.OnTimeout(SocketErrorType.CheckTimeout);
                }
            }
        }*/
    }
}
