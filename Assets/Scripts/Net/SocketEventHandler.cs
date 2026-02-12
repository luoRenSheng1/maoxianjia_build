using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public enum SocketErrorType
    {
        None,
        Connecting,
        Success,

        RemoteClose,

        //Fail
        ConnectionUnknow,
        ConnectionNotConnected,
        ConnectionAborted,
        ConnectionRefused,
        ConnectionReset,
        ConnectionNetworkUnreachable,
        ConnectionNetworkChanged,
        ConnectionAlreadyConnected,

        ConnectTimeout,
        SendTimeout,
        ReceriveTimeout,
        HeartBeatTimeout,

        ReceiveException,
        SocketException,
    }

    public class SocketEventHandler
    {
        /// <summary>
        /// 如果存在多线程，网络线程和主线程共享的消息包队列
        /// </summary>
        protected List<IReceiver> m_readPacketList = new List<IReceiver>();

        /// <summary>
        /// 主线程的消息队列
        /// </summary>
        protected List<IReceiver> m_readMessageList = new List<IReceiver>();

        protected Connection m_context;

        public enum TransportState
        {
            readHead = 1,       // on read head
            readBody = 2,       // on read body
            closed = 3          // connection closed, will ignore all the message and wait for clean up
        }

        public const int HEAD_LENGTH = 4;
        private byte[] headBuffer = new byte[4];
        private byte[] buffer;
        private int bufferOffset = 0;
        private int pkgLength = 0;
        private TransportState transportState;
        private ProtocolState state = ProtocolState.start;
        private bool IsInForeachObjectDoing { get; set; }
        
        public SocketEventHandler(Connection connection)
        {
            this.m_context = connection;
        }

        public Connection Context
        {
            get
            {
                return this.m_context;
            }
            set
            {
                this.m_context = value;
            }
        }

        public virtual bool Initialize()
        {
            transportState = TransportState.readHead;
            heartbeat = new SocketHeartBeat(this.m_context);

            return true;
        }

        public virtual void Dispose()
        {
            Clear();
            Context = null;
            StopHeartBeat(true);
        }

        public virtual void Clear()
        {
            buffer = null;
            bufferOffset = 0;
            pkgLength = 0;
            transportState = TransportState.readHead;
            state = ProtocolState.closed;
        }

        public virtual void OnRead(byte[] bytes, int index, int length)
        {
            this.ProcessBytes(bytes, index, length);
        }

        public virtual void OnStartConnect()
        {
            lock (m_readPacketList)
            {
                m_readPacketList.Clear();
            }

            IsInForeachObjectDoing = false;
            m_readMessageList.Clear();
        }

        public virtual void OnConnected()
        {
            LogUtils.LogWarning("Lobby Connect Success");

            this.state = ProtocolState.working;

            if (GameManager.Instance.Connection.IsReconnectFlag())
            {
              EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_RECONNECT_SUCCESS);
            }
            else
            {
                GameManager.Instance.LobbyServerConnectComplete();
            }

        }

        public virtual void OnConnectFail(SocketErrorType type)
        {
            LogUtils.LogWarningFormat("LobbyOnConnectFail {0}", type);

            this.m_context.ProtocolMessage.Clear();

            // 通知主线程连接失败
            TimerManagerEx.Instance.SetTimer(0.01f, OnRealConnectFail);
        }
        
        private void OnRealConnectFail()
        {
            if (m_context != null)
            {
                m_context.Reconnect();
            }
        }

        public virtual void OnSocketException()
        {
            GameManager.Instance.Connection.ActiveClose();
            MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
            {
                OkCallBack = () =>
                {
                    //断线
                    ItemInfoManager.Instance.Clear();
                    EquipManager.Instance.Clear();
                    UIManager.Instance.CloseAllUIPanel();
                    UIManager.Instance.ShowUIPanel("Login");
                },
                CancelBack = () =>
                {
                    //EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_RECONNECT_SUCCESS);
                }
            };
            UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(25), param, false);
            UIManager.Instance.CloseUIPanel("ReconnectWindow");
        }

        public virtual void OnTimeout(SocketErrorType type)
        {
            LogUtils.LogWarningFormat("SocketEventHandler OnTimeout {0}", type.ToString());

            if (m_context != null)
            {
                m_readMessageList.Clear();
                m_context.Reconnect();
            }
        }

        public virtual void OnDisconnect(SocketErrorType type)
        {
            LogUtils.LogWarningFormat("LobbyDisconnet {0}", type);

            m_context.ProtocolMessage.Clear();
            m_readMessageList.Clear();
            // 通知主线程断开连接
            TimerManagerEx.Instance.SetTimer(0.01f, OnRealDisconnect);
        }

        private void OnRealDisconnect()
        {
            if (m_context != null && state != ProtocolState.closed)
            {
                m_readMessageList.Clear();
                // close 状态不重连
                m_context.Reconnect();
            }
        }
        
        public virtual void OnUpdate(float fElpaseTime)
        {
            lock (m_readPacketList)
            {
                m_readMessageList.AddRange(m_readPacketList);
                m_readPacketList.Clear();
            }

            if (m_readMessageList.Count > 0)
            {
                m_readMessageList.Reverse();
                for (int i = m_readMessageList.Count-1; i >=0; i--)
                {
                    try
                    {
                        this.ProcessMsg(m_readMessageList[i]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarningFormat("消息处理有问题了，m_readMessageList.Count={0}   i={1}", m_readMessageList.Count, i);
                    }

                }
                m_readMessageList.Clear();
            }
            
            // if (m_readMessageList.Count > 0 && !IsInForeachObjectDoing)
            // {
            //     IsInForeachObjectDoing = true;
            //     foreach (var msg in m_readMessageList)
            //     {
            //         this.ProcessMsg(msg);
            //     }
            //
            //     m_readMessageList.Clear();
            //     IsInForeachObjectDoing = false;
            // }
            
            
            if (heartBeatUpdateByTick && heartbeat != null)
            {
                heartbeat.OnUpdate(fElpaseTime);
            }
        }

        protected virtual void ProcessMsg(IReceiver msg)
        {
            int nMsgID = msg.MsgID();

            try
            {
                msg.Process();
                if(nMsgID != (int) eMsgID.eMsg_ServerPing_Reply && nMsgID != (int) eMsgID.eMsg_Stage_Begin_SC && nMsgID != (int) eMsgID.eMsg_Stage_AwardCommit_SC && nMsgID != (int) eMsgID.eMsg_ClientPing_SC)
                    UIManager.Instance.HideSCLoading(nMsgID);
#if UNITY_EDITOR
                LogUtils.LogWarningFormat("ProcessMsg：msgID={0}", msg.MsgID());
#endif
            }
            catch (Exception ex)
            {
#if UNITY_WEBGL
                if (nMsgID != MsgDefine.HeartBeat && nMsgID != MsgDefine.PushBeKicked && nMsgID != MsgDefine.HandShakeSynAck)
                {
                    MsgPIPReceriver msgPIP = msg as MsgPIPReceriver;

                    if (msgPIP != null)
                    {
                        var data = msgPIP.GetData();

                        if (data != null)
                        {
                            string errorMsg = string.Format("[SocketEvent] ProcessMsg {0} {1} {2} {3}",
                                nMsgID, data.ToString(), ex.Message, ex.StackTrace);
                            LogUtils.LogException(errorMsg);
                            return;
                        }
                    }
                }
#endif

                string error = string.Format("[SocketEvent] ProcessMsg {0} {1} {2}", nMsgID, ex.Message, ex.StackTrace);
                LogUtils.LogException(error);
            }

            if (m_context != null && m_context.ReceiverFactory != null)
            {
                m_context.ReceiverFactory.RecycleReceiver(msg.MsgID(), msg);
            }
        }

        protected void ProcessNetMsg(BaseStructRecv mStruct, IReceiver receiver)
        {
            if (receiver != null)
            {
                try
                {
                    receiver.Read(mStruct);
       
                    lock (m_readPacketList)
                    {
                        m_readPacketList.Add(receiver);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogErrorFormat("ProcessNetMsg Error:{0} {1}", ex.Message, ex.StackTrace);
                }
            }
        }
        
        protected void ProcessHeartMsg(BaseStructRecv mStruct, IReceiver receiver)
        {
            if (receiver != null)
            {
                try
                {
                    receiver.Read(mStruct);

                    if (this.heartBeatUpdateByTick)
                    {
                        lock (m_readPacketList)
                        {
                            m_readPacketList.Add(receiver);
                        }
                    }
                    else
                    {
                        this.heartBeatUpdateRecv = true;
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogErrorFormat("ProcessNetMsg Error:{0} {1}", ex.Message, ex.StackTrace);
                }
            }
        }
        
        public bool isWorking()
        {
            return state == ProtocolState.working;
        }
        
        private void ProcessBytes(byte[] bytes, int offset, int limit)
        {
            if (this.transportState == TransportState.readHead)
            {
                readHead(bytes, offset, limit);
            }
            else if (this.transportState == TransportState.readBody)
            {
                readBody(bytes, offset, limit);
            }
        }

        private bool readHead(byte[] bytes, int offset, int limit)
        {
            int length = limit - offset; //数据长度
            int headNum = HEAD_LENGTH - bufferOffset;
            if (length >= headNum)  //保证头数据完整
            {
                //Write head buffer
                writeBytes(bytes, offset, headNum, bufferOffset, headBuffer);
                pkgLength = BitConverter.ToInt16(headBuffer, 2);
                //Init message buffer
                buffer = new byte[HEAD_LENGTH + pkgLength];
                writeBytes(headBuffer, 0, HEAD_LENGTH, buffer);
                offset += headNum;
                bufferOffset = HEAD_LENGTH;
                this.transportState = TransportState.readBody;

                if (offset <= limit) ProcessBytes(bytes, offset, limit);
                return true;
            }
            else
            {
                writeBytes(bytes, offset, length, bufferOffset, headBuffer);
                bufferOffset += length;
                return false;
            }
        }

        private void readBody(byte[] bytes, int offset, int limit)
        {
            int length = pkgLength + HEAD_LENGTH - bufferOffset;
            if ((offset + length) <= limit) //一条完整的消息
            {
                writeBytes(bytes, offset, length, bufferOffset, buffer);
                offset += length;

                this.ProcessMessage(buffer);

                this.bufferOffset = 0;
                this.pkgLength = 0;

                if (this.transportState != TransportState.closed)
                    this.transportState = TransportState.readHead;
                if (offset < limit)
                    ProcessBytes(bytes, offset, limit);
            }
            else
            {
                writeBytes(bytes, offset, limit - offset, bufferOffset, buffer);
                bufferOffset += limit - offset;
                this.transportState = TransportState.readBody;
            }
        }

        private void writeBytes(byte[] source, int start, int length, byte[] target)
        {
            writeBytes(source, start, length, 0, target);
        }

        private void writeBytes(byte[] source, int start, int length, int offset, byte[] target)
        {
            for (int i = 0; i < length; i++)
            {
                target[offset + i] = source[start + i];
            }
        }
        
        private void ProcessMessage(byte[] bytes)
        {
            Package pkg = PackageProtocol.decode(bytes);

            if (pkg != null)
            {
                BaseStructRecv mStruct = new BaseStructRecv(pkg.type);
                mStruct.obj = pkg.body;
                IReceiver receriver = Context.ReceiverFactory.CreateReceiver(pkg.type);

                this.ProcessNetMsg(mStruct, receriver);
            }
        }
        
        // 心跳包（大厅网络没有单独的线程，心跳包需要开线程发送）
        protected SocketHeartBeat heartbeat;
        protected bool heartBeatUpdateByTick = false;
        protected bool heartBeatUpdateRecv = false; // 简单处理，不需要加锁

        private Thread threadHeart;
        private bool threadHeartStop = false;
        private DateTime threadHeartTimeLast;
        
        public float GetLastHeartBeatTime()
        {
            if (heartbeat != null)
            {
                return heartbeat.LastHeartBeatTime;
            }

            return 0.0f;
        }
        
        public void InitHeartBeat(float interval, float timeOut)
        {
            if (heartbeat != null)
            {
                heartbeat.Init(interval, timeOut);
            }

            if (interval > 0)
            {
                this.StartHeartBeat();
            }
        }

        public void StartHeartBeat()
        {
#if UNITY_WEBGL || UNITY_EDITOR
            if (heartbeat != null)
            {
                heartbeat.Start();
            }

            heartBeatUpdateByTick = true;
#else
            if (heartbeat != null)
            {
                heartbeat.Start();
            }

            heartBeatUpdateByTick = false;

            if (threadHeart == null)
            {
                threadHeart = new Thread(new ThreadStart(this.RunThreadHeart));
                threadHeart.Start();
                threadHeartStop = false;
                threadHeartTimeLast = DateTime.Now;
            }
#endif
        }

        private void RunThreadHeart()
        {
            while (!threadHeartStop)
            {
                TimeSpan passTime = DateTime.Now - threadHeartTimeLast;
                
                if (heartbeat != null)
                {
                    heartbeat.OnUpdate((float)passTime.TotalSeconds);
                }

                threadHeartTimeLast = DateTime.Now;

                if (heartBeatUpdateRecv)
                {
                    if (heartbeat != null)
                    {
                        heartbeat.OnHeartBeatRecv(0);
                    }

                    heartBeatUpdateRecv = false;
                }
                
                try
                {
                    Thread.Sleep(900);
                }
                catch (ThreadInterruptedException e)
                {
                    LogUtils.LogWarning("Lobby Connection RunThreadHeart ThreadInterruptedException by " + e.Message);
                }
            }
        }
        
        public virtual void StopHeartBeat(bool bRelease)
        {
            if (heartbeat != null)
            {
                heartbeat.Stop();
            }

            if (bRelease)
            {
                heartbeat = null;

                heartBeatUpdateByTick = false;

                threadHeartStop = true;
                
                if (threadHeart != null)
                {
                    threadHeart.Interrupt();
                    threadHeart = null;
                }
            }
        }

        public virtual void OnHeartBeatRecv(int time)
        {
            if (heartbeat != null)
            {
                heartbeat.OnHeartBeatRecv(time);
            }
        }

        public void SendHeartBeatTimely()
        {
            if (heartbeat != null)
            {
                heartbeat.SendTimelyHeartBeat();
            }
        }
    }
}
