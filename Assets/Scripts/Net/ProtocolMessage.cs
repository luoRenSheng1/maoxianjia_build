namespace Engine
{
    using System;
    using System.Text;
    using EngineBase;
    using System.Collections.Generic;

    public class ProtocolMessage
    {
        private uint reqID = 0;

        private JsonObject encodeProtos = new JsonObject();
        private JsonObject decodeProtos = new JsonObject();
        private Dictionary<uint, ushort> reqMap;
        private Dictionary<uint, object> ctxMap;
        private Protobuf protobuf;

        public const int MSG_Route_Limit = 255;
        public const int MSG_Route_Mask = 0x01;
        public const int MSG_Type_Mask = 0x07;

        public ProtocolMessage()
        {
            this.reqMap = new Dictionary<uint, ushort>();
            this.ctxMap = new Dictionary<uint, object>();
        }

        public void InitProtocol(JsonObject serverProtos, JsonObject clientProtos)
        {
            protobuf = new Protobuf(clientProtos, serverProtos);
            this.encodeProtos = clientProtos;
            this.decodeProtos = serverProtos;
            reqID = 0;
        }

        private void writeInt(int offset, uint value, byte[] bytes)
        {
            bytes[offset] = (byte)(value >> 24 & 0xff);
            bytes[offset + 1] = (byte)(value >> 16 & 0xff);
            bytes[offset + 2] = (byte)(value >> 8 & 0xff);
            bytes[offset + 3] = (byte)(value & 0xff);
        }

        private void writeShort(int offset, ushort value, byte[] bytes)
        {
            bytes[offset] = (byte)(value >> 8 & 0xff);
            bytes[offset + 1] = (byte)(value & 0xff);
        }

        private ushort readShort(int offset, byte[] bytes)
        {
            ushort result = 0;

            result += (ushort)(bytes[offset] << 8);
            result += (ushort)(bytes[offset + 1]);

            return result;
        }

        private int byteLength(string msg)
        {
            return Encoding.UTF8.GetBytes(msg).Length;
        }

        private void writeBytes(byte[] source, int offset, byte[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[offset + i] = source[i];
            }
        }

        public uint GetNewReqID()
        {
            if (++reqID >= 1000000000)//renew reqID after long time
            {
                reqID = 1;
            }

            return reqID;
        }

        public void Clear()
        {
            reqID = 0;
            reqMap.Clear();
            ctxMap.Clear();
        }
    }
}

