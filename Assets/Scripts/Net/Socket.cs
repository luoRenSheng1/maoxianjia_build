using System.Net;
using System;

namespace Engine
{
    public enum PacketReliability
    {
        /// Same as regular UDP, except that it will also discard duplicate datagrams.  RakNet adds (6 to 17) + 21 bits of overhead, 16 of which is used to detect duplicate packets and 6 to 17 of which is used for message length.
        UNRELIABLE,

        /// Regular UDP with a sequence counter.  Out of order messages will be discarded.
        /// Sequenced and ordered messages sent on the same channel will arrive in the order sent.
        UNRELIABLE_SEQUENCED,

        /// The message is sent reliably, but not necessarily in any order.  Same overhead as UNRELIABLE.
        RELIABLE,

        /// This message is reliable and will arrive in the order you sent it.  Messages will be delayed while waiting for out of order messages.  Same overhead as UNRELIABLE_SEQUENCED.
        /// Sequenced and ordered messages sent on the same channel will arrive in the order sent.
        RELIABLE_ORDERED,
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public enum SOCKET_STATUS
    {
        NONE,
        CONNECTING,
        CONNECTED,
        CLOSED
    }

    public enum ConnectStatus
    {
        None,
        StartConnect,
        Connected,
        ConnectFail,
        Disconnect,
        Timeout,
        Receive,
        TryReconnect,
    }

    public abstract class Socket
    {
        protected const int bufferLength = 0x1000000;

        public Connection Context { set; get; }

        public abstract bool Init();

        public abstract bool Connect(string host, int port, bool useSSL);

        public abstract void Disconnect();

        public abstract void Send(byte[] bytes, int length, PacketReliability packageMode = PacketReliability.RELIABLE_ORDERED);

        public abstract void OnUpdate(float fElpaseTime);

        public abstract bool Isconnecting();

        public abstract bool Isconnected(bool bPrecise);

        public abstract void Dispose();

        public abstract void SetPacketReliability(PacketReliability mode);

        public static bool IsStatus(ConnectStatus statuses, ConnectStatus status)
        {
            return (statuses & status) > 0;
        }

        public virtual void Clear()
        {
        }

        public bool Enable { get; set; }
    }
}
