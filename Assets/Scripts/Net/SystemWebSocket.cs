using System;
using UnityWebSocket;
using EngineBase;

namespace Engine
{
    public class SystemWebSocket : Socket
    {
        private const float TIMEOUT = 15.0f;
        
        private SOCKET_STATUS mState = SOCKET_STATUS.NONE;
        private WebSocket mSocket;

        public override bool Init()
        {
            return true;
        }

        public override bool Connect(string host, int port, bool useSSL)
        {
            if (this.Isconnecting())
            {
                LogUtils.LogWarning("SystemWebSocket Connecting!!!");
                return false;
            }

            if (this.Isconnected(true))
            {
                LogUtils.LogWarning("SystemWebSocket Already Connected!!!");
                return false;
            }

            if (string.IsNullOrEmpty(host))
            {
                LogUtils.LogWarning("SystemWebSocket Host Is IsNullOrEmpty!!!");
                return false;
            }

            string address;

            if (useSSL)
            {
                address = string.Format("wss://{0}:{1}", host, port);
            }
            else
            {
                address = string.Format("ws://{0}:{1}", host, port);
            }

            this.ClearSocket();
            this.mSocket = new WebSocket(address);
            this.mSocket.OnOpen += Socket_OnOpen;
            this.mSocket.OnMessage += Socket_OnMessage;
            this.mSocket.OnClose += Socket_OnClose;
            this.mSocket.OnError += Socket_OnError;
            this.mState = SOCKET_STATUS.CONNECTING;
            this.mSocket.ConnectAsync();

            StartTimeOutTimer();
            
            LogUtils.LogWarning("SystemWebSocket Connect:" + address);

            return true;
        }

        private void Socket_OnOpen(object sender, OpenEventArgs e)
        {
            LogUtils.LogWarning("SystemWebSocket OnOpen");
            
            this.CloseTimeOutTimer();
            
            this.mState = SOCKET_STATUS.CONNECTED;
            base.Context.EventHandler.OnConnected();
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            LogUtils.LogWarningFormat("Lobbywebsocket Socket_OnClose StatusCode: {0}, Reason: {1}", e.StatusCode, e.Reason);

            this.mState = SOCKET_STATUS.CLOSED;
            base.Context.EventHandler.OnDisconnect(SocketErrorType.RemoteClose);
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            LogUtils.LogWarningFormat("Lobbywebsocket Socket_OnError {0}", e.Message);

            if (this.mState == SOCKET_STATUS.CONNECTING)
            {
                if (base.Context != null)
                {
                    base.Context.EventHandler.OnConnectFail(SocketErrorType.ConnectionUnknow);
                }
                
                this.Clear();
                this.mState = SOCKET_STATUS.NONE;
            }
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            this.ProcessReceive(e);
        }

        private void ProcessReceive(MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                this.OnDataReceived(e.RawData, 0, e.RawData.Length);
            }
        }

        private void OnDataReceived(byte[] data, int offset, int length)
        {
            base.Context.ReceiveBytes(length);
            base.Context.EventHandler.OnRead(data, offset, length);
        }

        public override void Disconnect()
        {
            this.Clear();
        }

        public override void Send(byte[] bytes, int length, PacketReliability packageMode = PacketReliability.RELIABLE_ORDERED)
        {
            if (this.mSocket != null && Isconnected(true) && length > 0)
            {
                if (length > bufferLength)
                {
                    LogUtils.LogError("BeginSend is out of range");
                    return;
                }

                base.Context.SendBytes(length);

                try
                {
                    this.mSocket.SendAsync(bytes);
                }
                catch (Exception exception)
                {
                    LogUtils.LogException(exception.Message);

                    if (base.Context != null)
                    {
                        base.Context.EventHandler.OnSocketException();
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
            if (this.mSocket == null)
            {
                return false;
            }

            if (mState == SOCKET_STATUS.CONNECTED)
            {
                return true;
            }
            else
            {
                return false;
            }
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

            if (this.mSocket != null)
            {
                if (this.mState == SOCKET_STATUS.CONNECTED)
                {
                    this.mSocket.CloseAsync();
                }

                this.mSocket.OnOpen -= Socket_OnOpen;
                this.mSocket.OnMessage -= Socket_OnMessage;
                this.mSocket.OnClose -= Socket_OnClose;
                this.mSocket.OnError -= Socket_OnError;
                this.mSocket = null;
            }
        }

        public override void Dispose()
        {
            this.Clear();

            this.Context = null;
        }

        public override void OnUpdate(float fElpaseTime)
        {
        }

        public override void SetPacketReliability(PacketReliability mode)
        {
        }
        
        private void StartTimeOutTimer()
        {
            TimerManagerEx.Instance.ClearTimer(this.ConnectTimeOutCallBack);
            TimerManagerEx.Instance.SetTimer(TIMEOUT, this.ConnectTimeOutCallBack);
        }

        private void ConnectTimeOutCallBack()
        {
            LogUtils.LogWarning("ConnectTimeOutCallBack");

            if (!Isconnected(false))
            {
                LogUtils.LogWarning("ConnectTimeOutCallBack Disconnect");
                
                if (base.Context != null)
                {
                    base.Context.EventHandler.OnTimeout(SocketErrorType.ConnectTimeout);
                }
                this.ClearSocket();
            }
        }

        private void CloseTimeOutTimer()
        {
            TimerManagerEx.Instance.ClearTimer(this.ConnectTimeOutCallBack);
        }
    }
}