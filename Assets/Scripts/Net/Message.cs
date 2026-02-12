using System;
using EngineBase;

namespace Engine
{
    public class Message
    {
        public int routeId;
        public uint id;
        public JsonObject data;
        public string rawData;
        public object reqData;

        public Message(uint id, int routeId, JsonObject data, string rawData, object reqData)
        {
            this.id = id;
            this.routeId = routeId;
            this.data = data;
            this.rawData = rawData;
            this.reqData = reqData;
        }
    }
}