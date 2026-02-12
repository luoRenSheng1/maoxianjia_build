using System;
using EngineBase;
using UnityEngine;

namespace Engine
{
    public class TreasureData
    {
        public int id => DataManager.Instance.GetRoleData().equipBoxLv;
        public ulong lastTargetTime => DataManager.Instance.GetRoleData().equipBoxLvUpTime;//秒

        public TreasureData()
        {
        }

        public string ToJson()
        {
            return this.ToJsonObject().ToJson();
        }

        public JsonObject ToJsonObject()
        {
            JsonObject jsObj = new JsonObject();
            jsObj["id"] = id;
            jsObj["lastTargetTime"] = lastTargetTime;
            return jsObj;
        }
    }
}