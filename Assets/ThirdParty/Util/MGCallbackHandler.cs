using System;
using System.Collections;
using ThirdParty.Wrapper.ExtensionsDefs;
using UnityEngine;

namespace ThirdParty.Util
{
    class MGCallbackHandler
    {
        private static readonly Hashtable responseHT = new Hashtable();

        public static string Add<T>(T t1) where T : MGBaseOption<MGBaseCallbackResult>
        {
            try
            {
                int count = responseHT.Count;
                string key;
                for (key = ((float)count + UnityEngine.Random.value).ToString(); responseHT.ContainsKey(key); key = ((float)count + UnityEngine.Random.value).ToString())
                    ++count;

                responseHT.Add(key, t1);

                return key;
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }

            return Guid.NewGuid().ToString();
        }

        public static void InvokeResponseCallback<T>(string str) where T : MGBaseCallbackResult
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    T res = JsonUtility.FromJson<T>(str);
                    var id = res.callbackId;
                    if (responseHT.ContainsKey(id))
                    {
                        var callback = (MGBaseOption<T>)responseHT[id];
                        if (res.errCode == 1)
                        {
                            callback.success(res);
                        }
                        else
                        {
                            callback.fail(res);
                        }

                        responseHT.Remove(id);
                    }
                    else
                    {
                        LogUtils.LogError($"callback id not found, id: {id}");
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }
    }
}