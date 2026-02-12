using System;
using EngineBase;

namespace Engine
{
    public static class SimpleJsonEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns></returns>
        public static JsonObject TryGetJsonObject(this JsonObject json, string key)
        {
            object jsObj;
            if (json.TryGetValue(key, out jsObj))
            {
                try
                {
                    return (JsonObject)jsObj;
                }
                catch (Exception e)
                {
                    LogUtils.LogException(e);
                }
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static JsonArray TryGetJsonArray(this JsonObject json, string key)
        {
            object jsObj;
            if (json.TryGetValue(key,out jsObj))
            {
                try
                {
                    return (JsonArray)jsObj;
                }
                catch (Exception e)
                {
                  LogUtils.LogException(e);
                }
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ToJson(this JsonObject json)
        {
            return SimpleJson.SerializeObjectInHeap(json);
        }
    }
}