using System;

namespace Engine
{
    public enum ProtocolState
    {
        start = 1,          // Just open, need to send handshaking
        working = 2,		// can receive and send data 
        closed = 3,		    // on read body
    }

    public class PackageProtocol
    {
        public const int HEADER_LENGTH = 4;

        public static byte[] encode(int type, byte[] body)
        {
            int length = HEADER_LENGTH;
            int bodyLength = 0;

            if (body != null)
            {
                bodyLength = body.Length;
            }

            length += bodyLength;

            byte[] buf = new byte[length];

            byte[] dataType = BitConverter.GetBytes((ushort)type);
            Array.Copy(dataType, 0, buf, 0, 2);
            byte[] dataLength = BitConverter.GetBytes((ushort)bodyLength);
            Array.Copy(dataLength, 0, buf, 2, 2);
            if (body != null)
            {
                Array.Copy(body, 0, buf, HEADER_LENGTH, bodyLength);
            }
            
            return buf;
        }

        public static Package decode(byte[] buf)
        {
            if (buf == null || buf.Length < HEADER_LENGTH)
            {
                return null;
            }
            
            short type = BitConverter.ToInt16(buf, 0);
            short length = BitConverter.ToInt16(buf, 2);
            byte[] body = new byte[buf.Length - HEADER_LENGTH];
            Array.Copy(buf, HEADER_LENGTH, body, 0, body.Length);
            return new Package(type, body);
        }
    }
}