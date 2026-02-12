using System;
using EngineBase;
#if !UNITY_WEBGL
using System.Net.NetworkInformation;
using System.Net.Sockets;
#endif

namespace Engine
{
    public class SystemTCPEx : Socket
    {
#if !UNITY_WEBGL
        private const float TIMEOUT = 10.0f;
        static public int MAX_MSG_SIZE = 256 * 1024;

        private SOCKET_STATUS mState = SOCKET_STATUS.NONE;

        private string m_strHost;
        private int m_nPort;
        private TcpClient m_client;
        private NetworkStream m_clientStream;
        private byte[] m_clientBuffer = new byte[MAX_MSG_SIZE];

        public override bool Init()
        {
            return true;
        }

        public override bool Connect(string host, int port, bool useSSL)
        {
            if (string.IsNullOrEmpty(host))
            {
                LogUtils.LogError("SystemTCPEx Connect Host Is IsNullOrEmpty!!!");
                return false;
            }

            if (this.Isconnecting())
            {
                LogUtils.LogWarning("SystemTCPEx Connecting!!!");
                return false;
            }

            if (this.Isconnected(true) && (m_strHost == host && m_nPort == port))
            {
                LogUtils.LogWarning("SystemTCPEx Already Connected.");
                return true;
            }

            if (m_client != null)
            {
                Clear();
            }

            try
            {
                LogUtils.LogWarningFormat("SystemTCPEx Connect {0} {1}", host, port);

                m_strHost = host;
                m_nPort = port;

                base.Context.EventHandler.OnStartConnect();
                mState = SOCKET_STATUS.CONNECTING;

                m_client = new TcpClient();
                m_client.NoDelay = true;
                m_client.BeginConnect(m_strHost, m_nPort, new AsyncCallback(OnConnect), null);

                this.StartTimeOutTimer();

                return true;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                base.Context.EventHandler.OnSocketException();
                mState = SOCKET_STATUS.NONE;
            }

            return false;
        }

        private void OnConnect(IAsyncResult asr)
        {
            try
            {
                this.CloseTimeOutTimer();

                // 结束异步连接
                m_client?.EndConnect(asr);

                if (m_client != null && m_client.Connected)
                {
                    LogUtils.LogWarning("SystemTCPEx Connected");

                    this.mState = SOCKET_STATUS.CONNECTED;

                    m_clientStream = m_client.GetStream();
                    m_clientStream.BeginRead(m_clientBuffer, 0, MAX_MSG_SIZE, new AsyncCallback(OnRead), null);

                    base.Context.EventHandler.OnConnected();
                }
                else
                {
                    LogUtils.LogError("SystemTCPEx Connect failed");
                    base.Context?.EventHandler.OnConnectFail(SocketErrorType.None);
                }
            }
            catch (SocketException ex)
            {
                LogUtils.LogException(ex);
                OnSocketError(ex.SocketErrorCode);
            }
        }

        public override void Disconnect()
        {
            this.Clear();
        }

        public override void Send(byte[] bytes, int length, PacketReliability packageMode = PacketReliability.RELIABLE_ORDERED)
        {
            if (length > 0 && this.m_clientStream != null && this.m_clientStream.CanWrite)
            {
                if (length > bufferLength)
                {
                    LogUtils.LogError("SystemTCPEx BeginSend is out of range");
                    return;
                }

                base.Context.SendBytes(length);

                try
                {
                    this.m_clientStream.BeginWrite(bytes, 0, length, new AsyncCallback(OnWrite), null);
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                    base.Context.EventHandler.OnSocketException();
                }
            }
        }

        void OnWrite(IAsyncResult r)
        {
            try
            {
                if (this.m_clientStream == null || this.mState == SOCKET_STATUS.CLOSED)
                {
                    return;
                }

                this.m_clientStream.EndWrite(r);
            }
            catch (SocketException ex)
            {
                LogUtils.LogException(ex);
                OnSocketError(ex.SocketErrorCode);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                
                if (ex.InnerException != null && ex.InnerException is SocketException)
                {
                    OnSocketError(((SocketException)ex.InnerException).SocketErrorCode);
                }
            }
        }

        /// 读取消息
        /// </summary>
        void OnRead(IAsyncResult asr)
        {
            try
            {
                if (this.m_clientStream == null || this.mState == SOCKET_STATUS.CLOSED)
                {
                    return;
                }

                var bytesRead = this.m_clientStream.EndRead(asr);

                if (bytesRead <= 0)
                {
                    base.Context.EventHandler.OnDisconnect(SocketErrorType.RemoteClose);
                    return;
                }

                base.Context.ReceiveBytes(bytesRead);
                base.Context.EventHandler.OnRead(m_clientBuffer, 0, bytesRead);

                Array.Clear(m_clientBuffer, 0, MAX_MSG_SIZE);   //清空数组
                m_clientStream?.BeginRead(m_clientBuffer, 0, MAX_MSG_SIZE, new AsyncCallback(OnRead), null);
            }
            catch (SocketException ex)
            {
                LogUtils.LogException(ex);
                OnSocketError(ex.SocketErrorCode);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);

                if (ex.InnerException != null && ex.InnerException is SocketException)
                {
                    OnSocketError(((SocketException)ex.InnerException).SocketErrorCode);
                }
            }
        }

        private void OnSocketError(SocketError e)
        {
            LogUtils.LogWarningFormat("OnSocketError {0}", e);

            if (e == SocketError.WouldBlock)
            {
                LogUtils.Log("OnSocketError WouldBlock");
                return;
            }

            if (base.Context == null)
            {
                return;
            }

            if (this.mState == SOCKET_STATUS.CONNECTING)
            {
                if (e == SocketError.TimedOut)
                {
                    base.Context.EventHandler.OnTimeout(SocketErrorType.ConnectTimeout);
                }
                else
                {
                    SocketErrorType error = GetErrorType(e);
                    base.Context.EventHandler.OnConnectFail(error);
                }

                this.mState = SOCKET_STATUS.NONE;
            }
            else if (this.mState == SOCKET_STATUS.CLOSED)
            {
                if (e == SocketError.TimedOut)
                {
                    base.Context.EventHandler.OnTimeout(SocketErrorType.ConnectTimeout);
                }
            }
            else
            {
                if (e == SocketError.TimedOut)
                {
                    base.Context.EventHandler.OnTimeout(SocketErrorType.ReceriveTimeout);
                }
                else
                {
                    SocketErrorType error = GetErrorType(e);

                    switch (error)
                    {
                        case SocketErrorType.SocketException:
                            {
                                base.Context.EventHandler.OnSocketException();
                            }
                            break;

                        default:
                            {
                                base.Context.EventHandler.OnConnectFail(error);
                            }
                            break;
                    }
                }
            }
        }

        public override bool Isconnecting()
        {
            return this.mState == SOCKET_STATUS.CONNECTING;
        }

        public override bool Isconnected(bool bPrecise)
        {
            // 测试失败，IOS断线事件关wifi没法知道
            /*if (!bPrecise)*/
            {
                if (m_client == null)
                {
                    return false;
                }
                
                if (SOCKET_STATUS.CONNECTED != mState)
                {
                    return false;
                }
                
                return m_client.Connected;
            }

            /*if (m_client == null || m_client.Client == null)
            {
                return false;
            }
            
            if (SOCKET_STATUS.CONNECTED != mState)
            {
                return false;
            }

            if (!m_client.Connected)
            {
                return false;
            }
            
            LogUtils.LogWarning("Isconnected 1");
            
            // 尝试发送以非阻塞模式发送一个消息 注意这个非阻塞模式不会影响异步发送
            bool blockingState = m_client.Client.Blocking;
            bool bSocketError = false;
            
            try
            {
                LogUtils.LogWarning("Isconnected 2");
                // 组装一次心跳包
                byte[] bufTmp = new byte[6];
                bufTmp[0] = 0x3;
                bufTmp[1] = 0x0;
                bufTmp[2] = 0x0;
                bufTmp[3] = 0x2;
                bufTmp[4] = 0x7B;
                bufTmp[5] = 0x7D;
                m_client.Client.Blocking = false;
                LogUtils.LogWarning("Isconnected 3-1");
                m_client.Client.Send(bufTmp, 6, 0);
                LogUtils.LogWarning("Isconnected 3");
                return true;
            }
            catch (SocketException e)
            {
                LogUtils.LogWarning("Isconnected e");
                
                if (!e.NativeErrorCode.Equals(10035))
                {
                    LogUtils.LogWarning("Isconnected e 10035");
                    
                    // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                    // 这个错误是说发送缓冲区已满或者客户端的接收缓冲区已满
                    bSocketError = true;
                }
            }
            finally
            {
                LogUtils.LogWarning("Isconnected Reset");
                m_client.Client.Blocking = blockingState; //恢复状态
            }

            if (bSocketError)
            {
                LogUtils.LogWarning("Isconnected bSocketError");
                return false;
            }

            return true;*/
        }

        public override void Clear()
        {
            // TIP ： 必须提前设置为关闭状态，避免处理OnSocketError时Context为null
            this.mState = SOCKET_STATUS.CLOSED;

            base.Clear();

            this.ClearSocket();
        }

        private void ClearSocket()
        {
            this.mState = SOCKET_STATUS.CLOSED;

            if (this.m_clientStream != null)
            {
                this.m_clientStream.Close();
                this.m_clientStream = null;
            }

            if (this.m_client != null)
            {
                this.m_client.Close();
                this.m_client = null;
            }
        }

        public override void Dispose()
        {
            this.Clear();

            base.Context = null;
        }

        private SocketErrorType GetErrorType(SocketError e)
        {
            SocketErrorType error = SocketErrorType.ConnectionUnknow;

            if (e == SocketError.ConnectionRefused)
            {
                error = SocketErrorType.ConnectionRefused;
            }
            else if (e == SocketError.ConnectionReset)
            {
                error = SocketErrorType.ConnectionReset;
            }
            else if (e == SocketError.ConnectionAborted)
            {
                error = SocketErrorType.ConnectionAborted;
            }
            else if (e == SocketError.HostUnreachable || e == SocketError.NetworkUnreachable)
            {
                error = SocketErrorType.ConnectionNetworkUnreachable;
            }
            else if (e == SocketError.TimedOut)
            {
                error = SocketErrorType.ConnectTimeout;
            }
            else if (e == SocketError.NotSocket)
            {
                // 底层资源被释放后，会返回NotSocket错误
                error = SocketErrorType.SocketException;
            }
            else
            {
                error = SocketErrorType.ConnectionUnknow;
            }

            return error;
        }

        private void StartTimeOutTimer()
        {
            TimerManagerEx.Instance.ClearTimer(this.ConnectTimeOutCallBack);
            TimerManagerEx.Instance.SetTimer(TIMEOUT, this.ConnectTimeOutCallBack);
        }

        private void ConnectTimeOutCallBack()
        {
            LogUtils.LogWarning("ConnectTimeOutCallBack");

            if (null != m_client && !m_client.Connected)
            {
                LogUtils.LogWarning("ConnectTimeOutCallBack Disconnect");
                this.OnSocketError(SocketError.TimedOut);
                this.ClearSocket();
            }
        }

        private void CloseTimeOutTimer()
        {
            TimerManagerEx.Instance.ClearTimer(this.ConnectTimeOutCallBack);
        }

        public override void OnUpdate(float fElpaseTime)
        {
        }

        public override void SetPacketReliability(PacketReliability mode)
        {
        }
#else
        public override bool Connect(string host, int port, bool useSSL)
        {
            return false;
        }

        public override void Disconnect()
        {
        }

        public override void Dispose()
        {
        }

        public override bool Init()
        {
            return false;
        }

        public override bool Isconnected(bool bPrecise)
        {
            return false;
        }

        public override bool Isconnecting()
        {
            return false;
        }

        public override void OnUpdate(float fElpaseTime)
        {
        }

        public override void Send(byte[] bytes, int length, PacketReliability packageMode = PacketReliability.RELIABLE_ORDERED)
        {
        }

        public override void SetPacketReliability(PacketReliability mode)
        {
        }
#endif
    }
}
