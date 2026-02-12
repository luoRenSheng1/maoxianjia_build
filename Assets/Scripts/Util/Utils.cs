using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CommonEx;
using Config;
using UnityEngine;
using EngineBase;
using FairyGUI;
using Spine;
using Spine.Unity;
using Object = UnityEngine.Object;
#if !UNITY_WEBGL
using System.Net.NetworkInformation;
#endif

namespace Engine
{
    public static class Utils
    {
        public static bool isDebug = true;
        private static int nRandomLast = 0;
        public static readonly Vector3 HALF_GRID_SIZE = new Vector3(1.5f, 0, 1.5f);
        
        public static bool IsLoadModelFromAssetBundle()
        {
#if UNITY_EDITOR
            return false;
#else
            return true;
#endif
        }

        public static bool IsLogSDK()
        {
#if LOG_SLG || LOGW_SLG
            return true;
#else
            return false;
#endif
        }

        public static bool IsPMFunc()
        {
#if LOG_SLG || LOGW_SLG
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        public static string GetLocalAddressIP()
        {
#if !UNITY_WEBGL
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || item.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }
#endif
            return "";
        }

        public static void XORBytes(byte key, ref byte[] bytes)
        {
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; ++i)
                {
                    bytes[i] ^= key;
                }
            }
        }

        public static string GetString(byte[] bytes)
        {
            return bytes != null ? Encoding.UTF8.GetString(bytes) : string.Empty;
        }

        public static Transform NewChildren(this Transform parent, string name)
        {
            if (parent != null)
            {
                GameObject go = new GameObject(name);
                go.transform.SetParent(parent, false);
                return go.transform;
            }

            return null;
        }

        public static Transform FindTransform(Transform tf, string strName)
        {
            Transform tfFind = tf.Find(strName);

            if (tfFind != null)
            {
                return tfFind;
            }

            foreach (Transform child in tf)
            {
                Transform tfChildFind = FindTransform(child, strName);

                if (tfChildFind != null)
                {
                    return tfChildFind;
                }
            }

            return null;
        }

        public static void DestroyChildren(Transform trans)
        {
            if (trans == null)
            {
                return;
            }

            for (int i = 0; i < trans.childCount; i++)
            {
                GameObject go = trans.GetChild(i).gameObject;
                GameObject.Destroy(go);
            }

            trans.transform.DetachChildren();
        }

        public static GameObject FindGameObject(this Transform tf, string strName)
        {
            Transform tfFind = tf.Find(strName);

            if (tfFind != null)
            {
                return tfFind.gameObject;
            }

            return null;
        }

        public static T FindComponent<T>(this Transform tf, string strName) where T : Component
        {
            Transform tfFind = tf.Find(strName);

            if (tfFind != null)
            {
                return tfFind.GetComponent<T>();
            }

            return null;
        }

        public static T GetComponentInChildrenByName<T>(this GameObject go, string name)
        {
            if (go != null && go.transform != null)
            {
                Transform tfChlid = go.transform.Find(name);

                if (tfChlid != null)
                {
                    return tfChlid.GetComponent<T>();
                }
            }

            return default(T);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go != null)
            {
                T t = go.GetComponent<T>();
                return t != null ? t : go.AddComponent<T>();
            }

            return default(T);
        }

        public static GameObject FindChild(this GameObject go, string name)
        {
            if (go != null && go.transform != null)
            {
                Transform tf = go.transform.Find(name);
                return tf != null ? tf.gameObject : null;
            }

            return null;
        }

        public static void SetActiveEx(this Camera camera, bool value)
        {
            if (camera != null)
            {
                SetActiveEx(camera.gameObject, value);
            }
        }

        public static void SetActiveEx(this Transform trans, bool value)
        {
            if (trans != null)
            {
                SetActiveEx(trans.gameObject, value);
            }
        }

        public static void SetActiveEx(this GameObject go, bool value)
        {
            if (go != null && go.activeSelf != value)
            {
                go.SetActive(value);
            }
        }

        public static void SetScaleEx(this GameObject go, Vector3 value)
        {
            if (go != null && go.transform != null)
            {
                go.transform.localScale = value;
            }
        }

        public static bool GetBool(string vaule)
        {
            if (vaule != null && !vaule.Equals("") && !vaule.Equals("0"))
            {
                return true;
            }

            return false;
        }

        public static uint GetUInt(string vaule)
        {
            uint i;
            return uint.TryParse(vaule, out i) ? i : 0;
        }

        public static int GetInt(string vaule)
        {
            int i;
            return int.TryParse(vaule, out i) ? i : 0;
        }

        public static long GetLong(string vaule)
        {
            long i;
            return long.TryParse(vaule, out i) ? i : 0;
        }

        public static float GetFloat(string vaule)
        {
            float i;
            return float.TryParse(vaule, out i) ? i : 0.0f;
        }

        public static string GetString(string[] arrVaule, int index)
        {
            if (arrVaule != null && index >= 0 && index < arrVaule.Length)
            {
                return arrVaule[index];
            }

            return "";
        }

        public static int GetInt(string[] arrVaule, int index)
        {
            if (arrVaule != null && index >= 0 && index < arrVaule.Length)
            {
                return GetInt(arrVaule[index]);
            }

            return 0;
        }

        public static float GetFloat(string[] arrVaule, int index)
        {
            if (arrVaule != null && index >= 0 && index < arrVaule.Length)
            {
                return GetFloat(arrVaule[index]);
            }

            return 0.0f;
        }

        public static bool GetBool(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetBool(vaule[index]);
            }

            return false;
        }

        public static string GetString(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return vaule[index];
            }

            return "";
        }

        public static int GetInt(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetInt(vaule[index]);
            }

            return 0;
        }

        public static long GetLong(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetLong(vaule[index]);
            }

            return 0;
        }

        public static float GetFloat(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetFloat(vaule[index]);
            }

            return 0.0f;
        }

        public static int GetListValue(List<int> value, int index)
        {
            if (value != null && index >= 0 && index < value.Count)
            {
                return value[index];
            }

            return 0;
        }

        public static bool GetListValueForbidZero(List<int> value, int index, out int outValue)
        {
            if (value != null && index >= 0 && index < value.Count)
            {
                if (value[index] != 0)
                {
                    outValue = value[index];
                    return true;
                }
            }

            outValue = 0;
            return false;
        }

        // 向量的夹角
        public static float AngleVector360(Vector3 from_, Vector3 to_)
        {
            Vector3 v3 = Vector3.Cross(from_, to_);

            if (v3.y > 0)
            {
                return Vector3.Angle(from_, to_);
            }
            else
            {
                return 360 - Vector3.Angle(from_, to_);
            }
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * ((float)Math.PI / 180f));
        }

        public static bool IsWIFI()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return true;
            }

            return false;
        }

        public static float GetBatteryLevel()
        {
            return UnityEngine.SystemInfo.batteryLevel;
        }

        /// <summary>
        /// 获取系统内存大小
        /// </summary>
        /// <returns></returns>
        public static int GetSystemMemorySize()
        {
            return UnityEngine.SystemInfo.systemMemorySize;
        }

        /// <summary>
        /// GPU内存大小
        /// </summary>
        /// <returns></returns>
        public static int GetGraphicsMemorySize()
        {
            return UnityEngine.SystemInfo.graphicsMemorySize;
        }

        /// <summary>
        /// 当前处理器的数量
        /// </summary>
        /// <returns></returns>
        public static int GetProcessorCount()
        {
            return UnityEngine.SystemInfo.processorCount;
        }

        /// <summary>
        /// GPU显卡名称
        /// </summary>
        /// <returns></returns>
        public static string GetGPUName()
        {
            return UnityEngine.SystemInfo.graphicsDeviceName;
        }

        public static bool GetSplitString(string strSrc, char splitChar, ref string strKey, ref string strValue)
        {
            int nPos = strSrc.IndexOf(splitChar);

            if (nPos < 0)
            {
                return false;
            }

            int nLength = strSrc.Length;

            if (nPos > 0)
            {
                strKey = strSrc.Substring(0, nPos);
            }

            if (nPos + 1 < nLength)
            {
                strValue = strSrc.Substring(nPos + 1, nLength - nPos - 1);
            }

            return true;
        }

        public static bool GetSplitStringByLast(string strSrc, char splitChar, ref string strKey, ref string strValue)
        {
            int nPos = strSrc.LastIndexOf(splitChar);

            if (nPos < 0)
            {
                return false;
            }

            int nLength = strSrc.Length;

            if (nPos > 0)
            {
                strKey = strSrc.Substring(0, nPos);
            }

            if (nPos + 1 < nLength)
            {
                strValue = strSrc.Substring(nPos + 1, nLength - nPos - 1);
            }

            return true;
        }

        public static string Md5Hash(string input)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var retVal = md5.ComputeHash(inputBytes);
            var sb = new System.Text.StringBuilder();
            for (var i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("X2"));
            }

            var md5Hash = sb.ToString();
            return md5Hash.ToLower();
        }

        public static string UrlEncode(string stringToEscape)
        {
            return Uri.EscapeDataString(stringToEscape);
        }

        /// <summary>
        /// 获取随机数，尽量不与上次相同
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandom(int min, int max)
        {
            int nRandom = UnityEngine.Random.Range(min, max);

            if (nRandom == nRandomLast)
            {
                nRandom = UnityEngine.Random.Range(min, max);
            }

            nRandomLast = nRandom;
            return nRandom;
        }

        public static List<Vector3> CopyListVector3(List<Vector3> lstPos)
        {
            List<Vector3> lstPosNew = new List<Vector3>();

            if (lstPos != null)
            {
                foreach (var item in lstPos)
                {
                    lstPosNew.Add(item);
                }
            }

            return lstPosNew;
        }

        public static Vector2[] SmoothTroopLine(Vector2[] path, float smooth_distance = 0.5f)
        {
            if (path.Length >= 3)
            {
                List<Vector2> list = new List<Vector2>();
                list.Add(path[0]);
                for (int i = 1; i < path.Length - 1; i++)
                {
                    Vector2 from = path[i] - path[i - 1];
                    Vector2 to = path[i + 1] - path[i];
                    if (Vector2.Angle(from, to) >= 10.0f && from.magnitude > smooth_distance * 2f &&
                        to.magnitude > smooth_distance * 2f)
                    {
                        from = from.normalized;
                        to = to.normalized;
                        Vector2 vector = path[i] - from * smooth_distance;
                        Vector2 vector2 = path[i] + to * smooth_distance;
                        list.Add(vector);
                        list.Add(((vector + vector2) / 2f + path[i]) / 2f);
                        list.Add(vector2);
                    }
                    else
                    {
                        list.Add(path[i]);
                    }
                }

                list.Add(path[path.Length - 1]);
                return list.ToArray();
            }

            return path;
        }

        public static Vector2[] SmoothLine(Vector2[] path, float smooth_distance, int iterate = 0,
            int iterated_times = 0)
        {
            if (path.Length >= 3)
            {
                List<Vector2> list = new List<Vector2>();
                list.Add(path[0]);
                for (int i = 1; i < path.Length - 1; i++)
                {
                    Vector2 vector = path[i] - path[i - 1];
                    Vector2 vector2 = path[i + 1] - path[i];
                    if (vector.magnitude > smooth_distance * 2f && vector2.magnitude > smooth_distance * 2f)
                    {
                        vector = vector.normalized;
                        vector2 = vector2.normalized;
                        Vector2 item = path[i] - vector * smooth_distance;
                        Vector2 item2 = path[i] + vector2 * smooth_distance;
                        list.Add(item);
                        list.Add(item2);
                    }
                    else
                    {
                        list.Add(path[i]);
                    }
                }

                list.Add(path[path.Length - 1]);
                if (iterate == 0)
                {
                    return list.ToArray();
                }

                iterated_times++;
                return SmoothLine(list.ToArray(), smooth_distance / (float)(iterated_times + 1), iterate - 1);
            }

            return path;
        }

        public static void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;

            Transform t = go.transform;

            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        public static float DistanceIgnoreY(Vector3 a, Vector3 b)
        {
            if (Mathf.Approximately(a.y, b.y))
            {
                return Vector3.Distance(a, b);
            }

            a.y = b.y;

            return Vector3.Distance(a, b);
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// 贝塞尔曲线 二次方
        /// </summary>
        /// <param name="start"></param>
        /// <param name="center"></param>
        /// <param name="end"></param>
        /// <param name="t">当前时间t(0.0~1.0)</param>
        /// <returns></returns>
        public static Vector3 GetBezierCurvePoint(Vector3 start, Vector3 center, Vector3 end, float t)
        {
            t = Mathf.Clamp(t, 0.0f, 1.0f);
            return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
        }

        public static int[] GetArray(string vaule)
        {
            if (!string.IsNullOrEmpty(vaule))
            {
                try
                {
                    return SimpleJson.DeserializeObject<int[]>(vaule);
                }
                catch (Exception e)
                {
                    LogUtils.LogException(e);
                }
            }

            return new int[] { };
        }

        public static T[] DeserializeJson<T>(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return SimpleJson.DeserializeObject<T[]>(value);
                }
                catch (Exception e)
                {
                    LogUtils.LogException(e);
                }
            }

            return new T[] { };
        }

        /// <summary>
        /// 生成一个GUID
        /// </summary>
        public static string NewGuid()
        {
            return System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetIDFA()
        {
#if UNITY_IOS
            if (UnityEngine.iOS.Device.advertisingTrackingEnabled) 
                return UnityEngine.iOS.Device.advertisingIdentifier;

            return "";
#else
            return "";
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetIDFV()
        {
#if UNITY_IOS
            return UnityEngine.iOS.Device.vendorIdentifier;
#else
            return "";
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceID()
        {
#if UNITY_IOS
            return UnityEngine.iOS.Device.vendorIdentifier;
#elif UNITY_ANDROID
            return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_WEBGL
            return "";//ThinkingSDK.PC.Utils.ThinkingSDKDeviceInfo.DeviceID();
#else
            return "";
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetFileName()
        {
            return $"catsoup{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsUrl(string path)
        {
            //System.Text.RegularExpressions.Regex urlRegex = new System.Text.RegularExpressions.Regex(@"^(http|https)://", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //return urlRegex.IsMatch(path);
            Uri uriResult;
            return Uri.TryCreate(path, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsLocalPath(string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return uri.IsFile;
            }
            return false;
        }

        private static string m_temporaryImagePath = null;

        public static string TemporaryImagePath
        {
            get
            {
                if (m_temporaryImagePath == null)
                {

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                    m_temporaryImagePath = System.IO.Path.Combine( Application.temporaryCachePath, "tmpImg" );
#else
                    m_temporaryImagePath = System.IO.Path.Combine(Application.persistentDataPath, "tmpImg");
#endif
                }

                if (!string.IsNullOrEmpty(m_temporaryImagePath))
                {
                    if (!System.IO.Directory.Exists(m_temporaryImagePath))
                    {
                        System.IO.Directory.CreateDirectory(m_temporaryImagePath);
                    }
                }

                return m_temporaryImagePath;
            }
        }

        /// <summary>
        /// 路径规范化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="systemPattern"></param>
        /// <returns></returns>
        public static string NormalizePath(this string path, bool systemPattern = false)
        {
            char seperator = '\\', repSeperator = '/';
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (systemPattern)
            {
                seperator = '/';
                repSeperator = '\\';
            }
#endif
            return path.Replace(seperator, repSeperator);

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int TryToInt32(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
                return 0;
            }
        }
        
        public static float DistanceIgnoreZ(Vector3 a, Vector3 b)
        {
            if (Mathf.Approximately(a.z, b.z))
            {
                return Vector3.Distance(a, b);
            }

            a.z = b.z;

            return Vector3.Distance(a, b);
        }
        
        public static float GetParticleLength(Transform transform)
        {
            ParticleSystem []particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0;
            foreach(ParticleSystem ps in particleSystems){
                // if(ps.enableEmission){
                    // if(ps.loop){
                    //     return -1f;
                    // }
                    float duration = 0f;
                    if(ps.emissionRate <=0){
                        duration = ps.startDelay + ps.startLifetime;
                    }else{
                        duration = ps.startDelay + Mathf.Max(ps.duration,ps.startLifetime);
                    }
                    if (duration > maxDuration) {
                        maxDuration = duration;
                    }
                // }
            }
            return maxDuration;
        }
        
        private static List<string> _pendingPrefabList = new List<string>();

        public static void SetSpineModelOnFGUI(GGraph graph, string spinePath, float scale, string aniName = "idle", Action<Object> action = null, bool isFlip = false, bool notoPending = false)
        {
            if(graph.name == ("Role/"+spinePath)) return;
            ClearSpineModelOnFGUI(graph);
            GameManager.Instance.TimerManager.SetTimer(0.001f, () =>
            {
                ShowUIPrefab(graph, spinePath, scale, "Role/", (o) =>
                {
                    SkeletonAnimation animation = (o as GameObject).GetComponent<SkeletonAnimation>();
                    {
                        animation.state.SetAnimation(0, aniName, true);
                        action?.Invoke(animation);
                    }
                }, isFlip, notoPending);
            });

        }

        public static void ClearSpineModelOnFGUI(GGraph graph)
        {
            HideUIPrefab(graph);
        }
        
        /// <summary>
        /// 显示预制体
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="effectPath"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static void ShowUIPrefab(GGraph graph, string resName, float scale = 100, string resPath="Effect/", Action<Object> action = null, bool isFlip = false,bool notoPending = false)
        {
            string path = Path.Combine(resPath, resName);
            if (_pendingPrefabList.Contains(path) && !notoPending)
            {
                Debug.LogErrorFormat("正在加载中的资源======>{0}", path);
                return;
            }
            // Debug.Log("播放的特效:"+path);
            graph.name = path;
            _pendingPrefabList.Add(path);
            GoWrapper wrapper = null;
            if (!(graph.displayObject is GoWrapper))
            {
                wrapper = new GoWrapper();
            }
            else
            {
                wrapper = graph.displayObject as GoWrapper;
            }

            if (wrapper.wrapTarget != null)
            {
                GameObject.Destroy(wrapper.wrapTarget);
            }

            // scale *= Math.Min(1.2f,GRoot.contentScaleFactor);
            // //同步加载
            object go = ModelManager.Instance.SyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_NORMAL_PREFAB, path, path);
            GameObject obj = GameObject.Instantiate(go as GameObject);
            Vector2 size = new Vector2(graph.width, graph.height);
            obj.transform.localScale = new Vector3(isFlip ? -scale : scale, scale, scale);
            obj.transform.localPosition = Vector3.zero;//new Vector3(size.x / 2f, -size.y, 1000);
            obj.transform.localEulerAngles = new Vector3(0, 0, 0);
            wrapper.SetWrapTarget(obj, true);
            obj.name = path;
            graph.SetNativeObject(wrapper);
            //回调
            action?.Invoke(obj);
            _pendingPrefabList.Remove(path);
            // ModelManager.Instance.LoadNormalPrefab(path, 
            //     (go) =>
            //     {
            //         // Debug.Log("回调UI:"+path);
            //         if(!_pendingPrefabList.Contains(path)) return;
            //         _pendingPrefabList.Remove(path);
            //         if (go != null)
            //         {
            //             GameObject obj = GameObject.Instantiate(go);
            //             Vector2 size = new Vector2(graph.width, graph.height);
            //             obj.transform.localScale = new Vector3(isFlip ? -scale : scale, scale, scale);
            //             obj.transform.localPosition = Vector3.zero;//new Vector3(size.x / 2f, -size.y, 1000);
            //             obj.transform.localEulerAngles = new Vector3(0, 0, 0);
            //             wrapper.wrapTarget = obj;
            //             obj.name = path;
            //             graph.SetNativeObject(wrapper);
            //             //回调
            //             action?.Invoke(obj);
            //         }
            //     });
        }

        public static void HideUIPrefab(GGraph graph)
        {
            string path = graph.name;
            if(_pendingPrefabList.Contains(path))
                _pendingPrefabList.Remove(path);
            if (graph.displayObject is GoWrapper)
            {
                GameObject.DestroyImmediate(((GoWrapper)graph.displayObject).wrapTarget);
                ((GoWrapper) graph.displayObject).Dispose();
                graph.SetNativeObject(null);
            }

            graph.name = "";
        }

        public static void SetItemDataWithGuid(this UI_ItemCom item, ItemData itemData, bool isShowNum = false)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
            int quality = itemTypeUnit.Quality;
            if (itemTypeUnit.Type == 6 && itemData.ItemGuid > 0)
            {
                RuneInfo runeInfo = RuneInfoManager.Instance.GetRune((ulong) itemData.ItemGuid);
                if (runeInfo != null)
                {
                    ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(runeInfo.SkillId);
                    quality = skillUnit.SkillQuality;
                }

            }
            
            // 是否为传承装备  类型14为传承装备
            if (itemData.ItemGuid != 0 && itemTypeUnit.Type == 14)
            {
                // 传承装备
                EquipData equipData = EquipManager.Instance.GetNoWearLoreEquipByGuid(itemData.ItemGuid);
                if (equipData != null)
                {
                    quality = equipData.quality;
                }
            }

            ((UI_ItemCom) item).data = itemData;
            ((UI_ItemCom) item).itemSpineEff.visible = false;
            ((UI_ItemCom) item).ctrlQuality.selectedIndex = quality - 1;
            if (itemData.count == 0)
                ((UI_ItemCom)item).txtLv.text = "";
            else
                ((UI_ItemCom) item).txtLv.text = StringUtils.FormatCurrency(itemData.count);

            ((UI_ItemCom) item).txtLv.visible = isShowNum;
            ((UI_ItemCom) item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);

    
            ((UI_ItemCom)item).onClick.Set(OnItemTips);
        }
        
        private static void OnItemTips(EventContext context)
        {
            ItemData itemData = (ItemData)((UI_ItemCom)context.sender).data;
            if (itemData != null)
            {
                TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemData.id, itemData.ItemGuid);
            }
        }

        public static void DiamondNotEnough()
        {
            OpenBuyDiamondView();
        }

        public static void PlayGetItemSpineEff(this UI_ItemCom itemCom, int quality)
        {
            // if (quality < 4)
            // {
            //     itemCom.itemSpineEff.visible = false;
            //     return;
            // }
            // // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(19);
            // itemCom.itemSpineEff.visible = true;
            // TrackEntry trackEntry = itemCom.itemSpineEff.spineAnimation.AnimationState.SetAnimation(0, "show_Q"+quality.ToString(), false);
            // trackEntry.AnimationStart = 0;
            // itemCom.itemSpineEff.spineAnimation.AnimationState.AddAnimation(0, "idle_Q"+quality.ToString(), true, 0);


            if (quality < 4)
            {
                itemCom.itemSpineEff.visible = false;
                return;
            }
            string effName = GetEffNameByItemQuality(quality);
            itemCom.itemSpineEff.visible = true;
            // itemCom.itemSpineEff.spineAnimation.AnimationState.SetAnimation(0, effName, true);
            Utils.PlaySpineAnim2(itemCom.itemSpineEff, effName, false,0.6f);
        }

        public static string GetEffNameByItemQuality(int quality)
        {
            string effName = "";
            switch (quality)
            {
                case 1:
                    effName = "CommonEx_tyjl_bai";
                    break;
                case 2:
                    effName = "CommonEx_tyjl_lv";
                    break;
                case 3:
                    effName = "CommonEx_tyjl_lan";
                    break;
                case 4:
                    effName = "CommonEx_tyjl_zi";
                    break;
                case 5:
                    effName = "CommonEx_tyjl_chen";
                    break;
                case 6:
                    effName = "CommonEx_tyjl_fen";
                    break;
                case 7:
                    effName = "CommonEx_tyjl_hong";
                    break;
            }

            return effName;
        }

        public static List<Vector2> GetIntervalPoint(bool hasStart, bool hasEnd, int count, Vector2 endPoint,
            Vector2 startPoint)
        {
            List<Vector2> dfPoint = new List<Vector2>();
            float dfXLength = Math.Abs(endPoint.x - startPoint.x) / (count + 1);
            float dfYLength = Math.Abs(endPoint.y - startPoint.y) / (count + 1);
            float x, y;
            if(hasStart)
                dfPoint.Add(startPoint);
            for (int i = 0; i < count; i++)
            {
                dfPoint.Add(new Vector2());
                if (endPoint.x >= startPoint.x) x = endPoint.x - ((i + 1) * dfXLength);
                else x = endPoint.x + ((i + 1) * dfXLength);

                if (endPoint.y >= startPoint.y) y = endPoint.y - ((i + 1) * dfYLength);
                else y = endPoint.y + ((i + 1) * dfYLength);
                
                dfPoint[i] = new Vector2(x, y);
            }
            
            if(hasEnd)
                dfPoint.Add(endPoint);

            return dfPoint;
        }
        
        public static int GetStageIdInChapterIndex(int stageId, ConfigChapterUnit chapterUnit)
        {
            // string[] stageIdArr = chapterUnit.StageId.Split(',');
            // for (int i = 0; i < stageIdArr.Length; i++)
            // {
            //     if (stageId == int.Parse(stageIdArr[i]))
            //     {
            //         return i;
            //     }
            // }
            //
            return -1;
        }

        public static void OpenBuyGoldView()
        {
            var goldMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
            if (goldMap.Item1)
            {
                // UIManager.Instance.ShowUIPanel("BuryGiftPack", Gift_Bury.Gift_goldAdd);// 关闭商业化
            }
            else
            {
                UIManager.Instance.Toast(goldMap.Item2);
            }
        }

        public static void OpenBuyDiamondView()
        {
            var summonMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
            if (summonMap.Item1)
            {
                SummonSystemView systemView = UIManager.Instance.FindByName("SummonSystem") as SummonSystemView;
                if (systemView != null && systemView.IsShow())
                {
                    systemView.UpdateParams(1);
                }
                else
                {
                    UIManager.Instance.ShowUIPanel("SummonSystem",1);
                    LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                    lobbyView?.OpenBottomPanel(5,1);
                }
                
            }
            else
            {
                UIManager.Instance.Toast(summonMap.Item2);
            }

        }

        public static void OpenBuySpeedTicketView()
        {
            UIManager.Instance.ShowUIPanel("BuySpeedTicket");
        }

        public static void SetSpineColor(SkeletonAnimation spine, string colorStr = "#6c4d32")
        {
            if (ColorUtility.TryParseHtmlString(colorStr, out var color))
            {
                spine.skeleton.SetColor(color);
            }
        }

        public static void ItemNotEnough(int itemId)
        {
            MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
            {
                OkCallBack = () =>
                {
                    LobbyView lobbyView = UIManager.Instance.FindByName("Lobby")  as LobbyView;
                    lobbyView?.OpenBottomPanel(5, 0);
                }
            };
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemId);
            
            UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.FormatStringByKey(10160, itemTypeUnit.Name), param);
        }

        public static void Pay(Action paySuccess)
        {
            UIManager.Instance.ShowUIPanel("PayLoading");
            
            paySuccess?.Invoke();
        }
        
        public static (int, int) GetHeroStar(int breakLevel)
        {
            if (breakLevel <= 3)
            {
                return (0, breakLevel);
            }
            else if (breakLevel <= 6)
            {
                return (1, breakLevel - 3);
            }
            else
            {
                return (2, Mathf.Min(3, breakLevel - 6));
            }
        }
        
        private static Dictionary<string, Action> _onPlayCbDict = new Dictionary<string, Action>();
        public static void PlaySpineAnim(GLoader3D spine, string aniName, bool isLoop, Action onPlayComplete = null, bool isTpose =true)
        {
            spine.spineAnimation.state.Complete -= EntryCallBack;
            if (isTpose)
            {
                spine.spineAnimation.skeleton.SetToSetupPose();
                spine.spineAnimation.state.ClearTracks();
            }
            spine.spineAnimation.state.Data.DefaultMix = 0f;
            spine.spineAnimation.state.SetAnimation(0, aniName, isLoop);
            if (!isLoop)
            {
                spine.spineAnimation.state.Complete += EntryCallBack;
                _onPlayCbDict[aniName] = onPlayComplete;
            }
        }
        
        /// <summary>
        /// 按照指定速度播放spine
        /// </summary>
        /// <param name="spine"></param>
        /// <param name="aniName"></param>
        /// <param name="isLoop"></param>
        /// <param name="speed">指定速度</param>
        /// <param name="onPlayComplete"></param>
        /// <param name="isTpose"></param>
        public static void PlaySpineAnim2(GLoader3D spine, string aniName, bool isLoop, float speed, Action onPlayComplete = null, bool isTpose =true)
        {
            spine.spineAnimation.state.Complete -= EntryCallBack;
            if (isTpose)
            {
                spine.spineAnimation.skeleton.SetToSetupPose();
                spine.spineAnimation.state.ClearTracks();
            }
            spine.spineAnimation.state.Data.DefaultMix = 0f;
            spine.spineAnimation.state.SetAnimation(0, aniName, isLoop);
            spine.spineAnimation.timeScale = speed;
            if (!isLoop)
            {
                spine.spineAnimation.state.Complete += EntryCallBack;
                _onPlayCbDict[aniName] = onPlayComplete;
            }
        }

        private static void EntryCallBack(TrackEntry e)
        {
            // Debug.Log(e.Animation.Name);
            _onPlayCbDict[e.Animation.Name]?.Invoke();
            _onPlayCbDict[e.Animation.Name] = null;
        }

        private static long _sundayEndTime;
        public static long GetSundayEndTime(bool isGet = false)
        {
            // if (_sundayEndTime == 0 || isGet)
            // {
            //     DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //     long lTime = long.Parse(ServerTimeManager.Instance.CurServerTime + "0000000");
            //     TimeSpan toNow = new TimeSpan(lTime);
            //     DateTime dt = dtStart.Add(toNow);
            //     DateTime mondayTime = (dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")))).Date;
            //     DateTime dateTime = mondayTime.AddDays(6).AddDays(1).AddSeconds(-1);
            //     System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            //     _sundayEndTime = (int)(dateTime - startTime).TotalSeconds;
            // }
            // return _sundayEndTime;
            return (long) ServerTimeManager.Instance.GetNextWeekServerTime();
        }
        
        public static void YxlNotEnough()
        {
            ConfigGiftUnit giftUnit = ConfigUtils.GetGiftUnitsById((int) Gift_Bury.Gift_yxl);
            int yxlBuyNum = ShopInfoManager.Instance.GetPackNum(giftUnit.Id).BuyCounter;
            if (yxlBuyNum < giftUnit.BuyNumber)
            {
                //英雄令界面
                // UIManager.Instance.ShowUIPanel("BuryGiftPack", Gift_Bury.Gift_yxl);// 关闭商业化
            }
            else
            {
                UIManager.Instance.ToastByKey(10184);
            }
        }
        
        public static Gift_Bury GetDungeonGiftBuryByType(DungeonType type)
        {
            Gift_Bury giftBury = 0;
            switch (type)
            {
                case DungeonType.Gold:
                    giftBury = Gift_Bury.Gift_fb_jktz;
                    break;
                case DungeonType.Zhuzhao:
                    giftBury = Gift_Bury.Gift_fb_zbtz;
                    break;
                case DungeonType.Exp:
                    giftBury = Gift_Bury.Gift_fb_yxtz;
                    break;
                case DungeonType.PetMaterial:
                    giftBury = Gift_Bury.Gift_fb_fstz;
                    break;
            }

            return giftBury;
        }

        public static long GetTimestampByDateStr(string dateString)
        {
            // 定义日期字符串
            // string dateString = "2025/1/1";
            // 解析日期字符串为DateTime对象
            // 假设日期字符串的格式是"yyyy/M/d"，并且它是本地时间（如果不是本地时间，需要转换为UTC）
            DateTime date;
            if (DateTime.TryParseExact(dateString, "yyyy/M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                // 计算Unix时间戳
                long timestamp = (long)(date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

                return timestamp;
            }

            return 0;
        }
        
        public static DateTime GetDateTime(ulong timeStamp)  
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));  
            long lTime = ((long)timeStamp * 10000000);  
            TimeSpan toNow = new TimeSpan(lTime);  
            DateTime targetDt = dtStart.Add(toNow);  
            return targetDt;  
        }
        
		// 根据权重列表，返回随机索引
        private static System.Random random = new System.Random();
        public static List<int> GetIndexByWeight(List<int> weights, int count)
        {
            var result = new List<int>();
            var tempWeights = weights.ToList();
            int totalWeight = weights.Sum();

            while (result.Count < count && totalWeight > 0)
            {
                int r = random.Next(totalWeight);
                int sum = 0;

                for (int i = 0; i < tempWeights.Count; i++)
                {
                    if (tempWeights[i] == 0) continue;
                
                    sum += tempWeights[i];
                    if (r < sum)
                    {
                        result.Add(i);
                        totalWeight -= tempWeights[i];
                        tempWeights[i] = 0;
                        break;
                    }
                }
            }
            return result;
        }
        
        private static Dictionary<int,List<int>> entryDic = new Dictionary<int, List<int>>();
        public static List<int> GetMonsterEntryById(int id)
        {
            if (entryDic.TryGetValue(id, out var result))
            {
                return result;
            }
            return null;
        }

        private static string[] entrys;
        public static void InitMonsterEntry(bool isNew)
        {
            bool isReset = false;
            var saveMonsterEntrys = LocalSave.GetString(SaveKey.monsterEntry,"");
            if (saveMonsterEntrys == "")
            {
                isReset = true;
            }
            
            if (isReset || isNew)
            {
                string dataS = "0," + ServerTimeManager.Instance.CurServerTime;
                List<ConfigStageUnit> stageUnitsList = new List<ConfigStageUnit>();
                stageUnitsList.AddRange(ConfigUtils.GetStageUnitByChapterId(DataManager.Instance.GetRoleData().chapterId) );
                stageUnitsList.AddRange(ConfigUtils.GetStageUnitByChapterId(DataManager.Instance.GetRoleData().chapterId + 1) );
                List<ConfigStageUnit> stageList = new List<ConfigStageUnit>();
                foreach (var data in stageUnitsList)
                {
                    if (data.Node == 5)
                    {
                        stageList.Add(data);
                    }
                }
                
                foreach (var stageUnit in stageList)
                {
                    ConfigStageMonsterAttrUnit monsterAttr = ConfigUtils.GetStageMonsterAttrUnitByIndexId(stageUnit.Id);
                    List<ConfigMonsterEntryUnit> monsterEntryUnits = ConfigUtils.GetMonsterEntryByGroup(monsterAttr.EntryGroup).ToList(); //改词条组列表
                    var rates = new List<int>();
                    for (int i = 0; i < monsterEntryUnits.Count; i++)
                    {
                        rates.Add(monsterEntryUnits[i].Rate);
                    }

                    if (monsterAttr.EntryNumber > 0)
                    {
                        dataS = dataS + "|" + stageUnit.Id;
                        List<int> entryList = Utils.GetIndexByWeight(rates, monsterAttr.EntryNumber);
                        foreach (var entryId in entryList)
                        {
                            dataS = dataS + "," + entryId;
                        }
                    }
                    
                }
                LocalSave.SetString(SaveKey.monsterEntry, dataS);
                LocalSave.Save();
            }
            
            saveMonsterEntrys = LocalSave.GetString(SaveKey.monsterEntry,"");
            if (saveMonsterEntrys != "")
            {
                string[] entryStrings = saveMonsterEntrys.Split("|");
                for (int i = 0; i < entryStrings.Length; i++)
                {
                    string[] entry = entryStrings[i].Split(",");
                    entryDic[int.Parse(entry[0])] = new List<int>();
                    for (int j = 1; j < entry.Length; j++)
                    {
                        entryDic[int.Parse(entry[0])].Add(int.Parse(entry[j]));
                    }
                }
            }
        }
        
        public static void AddMonsterEntry(int id, List<int> values)
        {
            entryDic.Add(id,values);
        }

        public static List<int> GetEventStageMonsterEntry(ConfigEventStageUnit eventStageUnit)
        {
            if (eventStageUnit.EntryNumber > 0)
            {
                var traitArr = Utils.GetMonsterEntryById(eventStageUnit.Id);
                if (traitArr == null)
                {
                    List<ConfigMonsterEntryUnit> monsterEntryUnits = ConfigUtils.GetMonsterEntryByGroup(eventStageUnit.EntryGroup).ToList(); //改词条组列表
                    var rates = new List<int>();
                    for (int i = 0; i < monsterEntryUnits.Count; i++)
                    {
                        rates.Add(monsterEntryUnits[i].Rate);
                    }
                    // entryList.Clear();
                    traitArr = Utils.GetMonsterEntryByWeight(rates, eventStageUnit.EntryNumber);
                    Utils.AddMonsterEntry(eventStageUnit.Id, traitArr);
                    return traitArr;
                }
                else
                {
                    return traitArr;
                }
            }
            return null;
        }
        
        // 获得怪物buff词条
        private static System.Random evnentRandom = new System.Random();
        public static List<int> GetMonsterEntryByWeight(List<int> weights, int count)
        {
            var result = new List<int>();
            var tempWeights = weights.ToList();
            int totalWeight = weights.Sum();

            while (result.Count < count && totalWeight > 0)
            {
                int r = evnentRandom.Next(totalWeight);
                int sum = 0;

                for (int i = 0; i < tempWeights.Count; i++)
                {
                    if (tempWeights[i] == 0) continue;
                
                    sum += tempWeights[i];
                    if (r < sum)
                    {
                        result.Add(i);
                        totalWeight -= tempWeights[i];
                        tempWeights[i] = 0;
                        break;
                    }
                }
            }
            return result;
        }
    }
}