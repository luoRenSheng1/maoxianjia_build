using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using EngineBase;
using FairyGUI;

namespace Engine
{
    public delegate void UIGLoaderLoadDelegate(Texture texture);

    public class UIGLoaderLoad
    {
        public UIGLoader texture = null;
        public string strName = "";
        public string strPath = "";
        public bool bBGRA32 = false;
        public bool bRun = false;
        public bool bDone = false;
        public bool bSave = false;
        public bool bAdjustPixel = false;
        public bool bFixMode = false;
        public int nQueueWeight = 0;
        public GameObject goTarget = null;
    }

    public class UIGLoaderABCache
    {
        public string strResName = "";
        public AssetBundle abTexture = null;
        public Texture texture = null;
        public int nRenderedFrameCount = 0;
    }

    public class UIGLoaderManager : TSingleton<UIGLoaderManager>
    {
        public static int MAX_UIRESIFNO_CACHE_AMOUNT_NORMAL = 60;   // 缓存图片的数量
        public static int DEFAULT_RENDERED_FRAME_COUNT_DIFF = 300;
        public static int MAX_SAVE_LOAD_TIMES = 10;                  // 保存的图片认为是小图，小图一帧允许同时加载3个

        private const int MAX_QUEUE_WEIGHT = 60;
        private const float DEFAULT_LOAD_TIME = 0.03f;

        private MonoBehaviour MonoHost { set; get; }

        /// <summary>
        /// 资源加载
        /// </summary>
        private List<UIGLoaderLoad> lstUIResLoad = new List<UIGLoaderLoad>();
        private CTimer timerLoad = new CTimer();

        /// <summary>
        /// 资源缓存策略
        /// </summary>
        private Dictionary<string, Texture> mapResTexture = new Dictionary<string, Texture>();
        private List<string> lstResCacheKey = new List<string>(); // 需要缓存的资源名
        private int nResCacheLimit = 0; // 最大缓存资源数量
        private Dictionary<string, int> mapResTextureHit = new Dictionary<string, int>(); // 缓存计数

        /// <summary>
        /// 资源卸载
        /// </summary>
        // 资源引用计数，引用计数归0清除资源
        private static Dictionary<int, int> dcitTextureCount = new Dictionary<int, int>();
        // 延迟卸载ABTexture，同一帧卸载ABTexture会报错
        private List<UIGLoaderABCache> lstUIResABCache = new List<UIGLoaderABCache>();

        public override void Init()
        {
            this.InitParam();

            this.InitMonoHost();
        }

        public void InitParam()
        {
            timerLoad.Startup(DEFAULT_LOAD_TIME);

            nResCacheLimit = MAX_UIRESIFNO_CACHE_AMOUNT_NORMAL;
        }

        public void InitMonoHost()
        {
            if (this.MonoHost != null)
            {
                return;
            }

            GameObject go = new GameObject();
            go.name = "UIGLoaderManager";
            GameObject.DontDestroyOnLoad(go);

            this.MonoHost = go.AddComponent<LoaderBehaviour>();
        }

        public void Dispose()
        {
            lstUIResLoad.Clear();

            foreach (var item in mapResTexture)
            {
                DelTextureCount(item.Value);
            }

            mapResTexture.Clear();
            mapResTextureHit.Clear();
        }

        public void Tick(float deltaSeconds)
        {
            UnLoadUIResByTick();
            LoadUIResByTick();
        }

        public bool IsLoadResFromAssetBundle()
        {
            return false;
        }
        
        // 获取图片名称
        public void GetResName(string strName, out string strResName, out string strResPath)
        {
            int nIndexSep = strName.LastIndexOf('/');
            int nIndexPoint = strName.LastIndexOf('.');
            int nIndexBegin = 0;
            int nIndexEnd = 0;

            if (nIndexSep == -1)
            {
                nIndexBegin = 0;
            }
            else
            {
                nIndexBegin = nIndexSep + 1;
            }

            if (nIndexPoint == -1)
            {
                nIndexEnd = strName.Length - 1;
            }
            else
            {
                nIndexEnd = nIndexPoint - 1;
            }

            if (nIndexEnd >= nIndexBegin)
            {
                strResName = strName.Substring(nIndexBegin, nIndexEnd - nIndexBegin + 1);
            }
            else
            {
                strResName = strName;
            }

#if UNITY_EDITOR
            strResPath = strName;
#else
            // 后缀换成bin
            nIndexPoint = strName.LastIndexOf('.');

            if (nIndexPoint != -1)
            {
                strResPath = string.Format("res/{0}bin", strName.Substring(0, nIndexPoint + 1));
            }
            else
            {
                strResPath = string.Format("res/{0}", strName);
            }
            
            // strResName = strResName.ToLower();
            // strResPath = strResPath.ToLower();
#endif
        }
        
        public void ClearUIResLoad()
        {
            lstUIResLoad.Clear();
        }

        // FairyGUI需要手动管理外部图片内存, 使用引用计数实现
        public void AddTextureCount(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            // 记录引用计数
            int nCount = 0;
            int nTextureID = texture.GetInstanceID();

            if (dcitTextureCount.TryGetValue(nTextureID, out nCount))
            {
                dcitTextureCount[nTextureID] = ++nCount;
            }
            else
            {
                dcitTextureCount.Add(nTextureID, 1);
            }
        }

        // FairyGUI需要手动管理外部图片内存, 使用引用计数实现
        public void DelTextureCount(NTexture texture)
        {
            if (texture == null)
            {
                return;
            }

            if (texture.nativeTexture != null)
            {
                int nCount = 0;
                int nTextureID = texture.nativeTexture.GetInstanceID();
                if (dcitTextureCount.TryGetValue(nTextureID, out nCount))
                {
                    dcitTextureCount[nTextureID] = --nCount;
                }

                if (nCount <= 0)
                {
                    dcitTextureCount.Remove(nTextureID);

                    // 如果还在ABCache队列中不清除图片
                    if (!IsInUIResABCache(texture.nativeTexture))
                    {
                        UnityEngine.Object.DestroyImmediate(texture.nativeTexture, true);
                    }
                }
            }

            texture.Dispose();
        }

        // FairyGUI需要手动管理外部图片内存, 使用引用计数实现
        public void DelTextureCount(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            int nCount = 0;
            int nTextureID = texture.GetInstanceID();
            if (dcitTextureCount.TryGetValue(nTextureID, out nCount))
            {
                dcitTextureCount[nTextureID] = --nCount;
            }

            if (nCount <= 0)
            {
                dcitTextureCount.Remove(nTextureID);

                // 如果还在ABCache队列中不清除图片
                if (!IsInUIResABCache(texture))
                {
                    UnityEngine.Object.DestroyImmediate(texture, true);
                }
            }
        }

        public void RecordResCacheKey(string strKey)
        {
            if (!lstResCacheKey.Contains(strKey))
            {
                lstResCacheKey.Add(strKey);
            }
        }

        public void UnRecordResCacheKey(string strKey)
        {
            if (lstResCacheKey.Contains(strKey))
            {
                lstResCacheKey.Remove(strKey);
            }
        }

        public void UnRecordAllResCacheKey()
        {
            lstResCacheKey.Clear();
        }

        private bool IsNeedResCacheByResName(string strResName)
        {
            foreach (var item in lstResCacheKey)
            {
                if (strResName.StartsWith(item))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddUIResLoadHit(string strName)
        {
            if (mapResTextureHit.ContainsKey(strName))
            {
                ++mapResTextureHit[strName];
            }
            else
            {
                mapResTextureHit.Add(strName, 1);
            }
        }

        public bool IsInUIResABCache(Texture texture)
        {
            if (texture == null)
            {
                return false;
            }

            foreach (var item in lstUIResABCache)
            {
                if (item != null && item.texture.GetInstanceID() == texture.GetInstanceID())
                {
                    return true;
                }
            }

            return false;
        }

        public UIGLoaderABCache GetUIResABCache(string strResName)
        {
            foreach (var item in lstUIResABCache)
            {
                if (item != null && item.strResName == strResName)
                {
                    // 成功获取，延迟清除
                    item.nRenderedFrameCount = Time.renderedFrameCount;
                    return item;
                }
            }

            return null;
        }

        public void AddUIResABCache(string strResName, AssetBundle abTexture, Texture texture)
        {
            if (abTexture != null)
            {
                UIGLoaderABCache cache = new UIGLoaderABCache();
                cache.strResName = strResName;
                cache.abTexture = abTexture;
                cache.texture = texture;
                cache.nRenderedFrameCount = Time.renderedFrameCount;
                lstUIResABCache.Add(cache);
            }
        }

        private void LoadUIResDone(UIGLoader loader, string strResPath, bool bAdjustPixel, bool bFixMode, bool bSave, Texture texture)
        {
            if (loader == null)
            {
                return;
            }

            if (bAdjustPixel)
            {
                loader.autoSize = true;
            }

            if (bFixMode && texture != null)
            {
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
            }

            loader.OnLoadSuccess(texture);

            if (bSave)
            {
                CheckAndSaveResTexture(strResPath, texture);
            }
        }

        private void LoadUIResFail(UIGLoader loader)
        {
            if (null == loader)
            {
                return;
            }

            loader.OnLoadFail();
        }

        /// <summary>
        /// 异步加载图片
        /// </summary>
        /// <param name="loader">宿主控件</param>
        /// <param name="strName">图片名</param>
        /// <param name="bPriority">是否优先加载</param>
        /// <param name="bAdjustPixel">是否根据图片尺寸自动调整宿主控件尺寸</param>
        /// <param name="bFixMode">是否修正图片<黑边问题></param>
        public void AsyncLoadUIRes(UIGLoader loader, string strName, bool bPriority = false, bool bAdjustPixel = false, bool bFixMode = false)
        {
            bool bSave = IsNeedResCacheByResName(strName);
            string strResName = "";
            string strResPath = "";

            GetResName(strName, out strResName, out strResPath);

            AddUIResLoadHit(strResPath);

            if (loader != null)
            {
                // 检查是否存在同一对象
                foreach (UIGLoaderLoad resOld in lstUIResLoad)
                {
                    if (resOld != null && resOld.texture != null && !resOld.bRun && resOld.texture.GID == loader.GID)
                    {
                        lstUIResLoad.Remove(resOld);

                        resOld.texture = loader;
                        resOld.strName = strResName;
                        resOld.strPath = strResPath;
                        resOld.bBGRA32 = strName.EndsWith("png");
                        resOld.bRun = false;
                        resOld.bDone = false;
                        resOld.bSave = bSave;
                        resOld.bAdjustPixel = bAdjustPixel;
                        resOld.bFixMode = bFixMode;
                        resOld.nQueueWeight = 0;

                        if (bPriority)
                        {
                            lstUIResLoad.Insert(0, resOld);
                        }
                        else
                        {
                            lstUIResLoad.Add(resOld);
                        }

                        return;
                    }
                }
            }

            UIGLoaderLoad res = new UIGLoaderLoad();
            res.texture = loader;
            res.strName = strResName;
            res.strPath = strResPath;
            res.bBGRA32 = strName.EndsWith("png");
            res.bRun = false;
            res.bDone = false;
            res.bSave = bSave;
            res.bAdjustPixel = bAdjustPixel;
            res.bFixMode = bFixMode;

            if (bPriority)
            {
                lstUIResLoad.Insert(0, res);
            }
            else
            {
                lstUIResLoad.Add(res);
            }
        }

        private void OnAsyncLoadUIRes(UIGLoaderLoad resLoad)
        {
            if (null == resLoad)
            {
                return;
            }
            
            if (IsLoadResFromAssetBundle())
            {
                // 否则读本地目录-AB
                // 卸载队列中，直接获取
                UIGLoaderABCache abCache = GetUIResABCache(resLoad.strName);

                if (abCache == null)
                {
                    byte[] bytes = ReadFileAllBytes(resLoad.strPath);

                    if (bytes == null)
                    {
                        LoadUIResFail(resLoad);
                        return;
                    }

                    AssetBundle ab = AssetBundle.LoadFromMemory(bytes);
                    
                    if (ab == null)
                    {
                        LoadUIResFail(resLoad);
                        return;
                    }
                    
                    Texture2D texture = ab.LoadAsset<Texture2D>(resLoad.strName);
                    
                    AddUIResABCache(resLoad.strName, ab, texture);
                    
                    LoadUIResDone(resLoad, texture);
                }
                else
                {
                    LoadUIResDone(resLoad, abCache.texture);
                }
            }
            else
            {
                // 否则读本地目录-原图
                byte[] bytes = ReadFileAllBytes(resLoad.strPath);

                if (bytes == null)
                {
                    LoadUIResFail(resLoad);
                    return;
                }

                Texture2D texture = new Texture2D(0, 0, resLoad.bBGRA32 ? TextureFormat.BGRA32 : TextureFormat.RGB24, false);
                texture.LoadImage(bytes);

                LoadUIResDone(resLoad, texture);
            }
        }

        private void LoadUIResDone(UIGLoaderLoad resLoad, Texture texture)
        {
            if (null == resLoad)
            {
                return;
            }

            if (null != resLoad.texture)
            {
                resLoad.texture.OnLoadSuccess(texture);

                if (resLoad.bAdjustPixel)
                {
                    resLoad.texture.autoSize = true;
                }

                if (resLoad.bFixMode && texture != null)
                {
                    texture.filterMode = FilterMode.Point;
                    texture.wrapMode = TextureWrapMode.Clamp;
                }
            }

            resLoad.bDone = true;

            if (resLoad.bSave)
            {
                CheckAndSaveResTexture(resLoad.strPath, texture);
            }
        }

        private void LoadUIResFail(UIGLoaderLoad resLoad)
        {
            if (null == resLoad)
            {
                return;
            }

            if (null != resLoad.texture)
            {
                resLoad.texture.OnLoadFail();
            }

            resLoad.bDone = true;
        }

        public void CheckAndSaveResTexture(string strName, Texture texture)
        {
            if (mapResTexture.ContainsKey(strName))
            {
                return;
            }

            bool bCacheTexture = false;

            if (mapResTexture.Count < nResCacheLimit)
            {
                bCacheTexture = true;
            }
            else
            {
                int nCountHit = 0;

                if (mapResTextureHit.ContainsKey(strName))
                {
                    nCountHit = mapResTextureHit[strName];
                }

                foreach (var kv in mapResTexture)
                {
                    int nCountHitCur = 0;

                    if (mapResTextureHit.ContainsKey(kv.Key))
                    {
                        nCountHitCur = mapResTextureHit[kv.Key];
                    }

                    if (nCountHitCur < nCountHit)
                    {
                        bCacheTexture = true;
                        DelTextureCount(kv.Value);
                        mapResTexture.Remove(kv.Key);
                        break;
                    }
                }
            }

            if (bCacheTexture)
            {
                AddTextureCount(texture);
                mapResTexture[strName] = texture;
            }
        }

        private static bool DoneUIResLoad(UIGLoaderLoad resLoad)
        {
            if (null == resLoad || resLoad.bDone)
            {
                return true;
            }

            return false;
        }

        private void LoadUIResByTick()
        {
            if (lstUIResLoad.Count > 0)
            {
                lstUIResLoad.RemoveAll(DoneUIResLoad);
            }

            if (lstUIResLoad.Count > 0 && timerLoad.ToNextTime())
            {
                bool bAllDisactive = true;

                int nUIResLoadCount = lstUIResLoad.Count;

                for (int nUIResLoadIndex = 0; nUIResLoadIndex < nUIResLoadCount; ++nUIResLoadIndex)
                {
                    UIGLoaderLoad resLoad = lstUIResLoad[nUIResLoadIndex];

                    if (resLoad != null
                        && resLoad.texture != null
                        && resLoad.texture.displayObject != null
                        && resLoad.texture.displayObject.visible)
                    {
                        bAllDisactive = false;
                        break;
                    }
                }

                nUIResLoadCount = lstUIResLoad.Count;

                int nResLoadCountCur = 0;

                for (int nUIResLoadIndex = 0; nUIResLoadIndex < nUIResLoadCount; ++nUIResLoadIndex)
                {
                    UIGLoaderLoad resLoad = lstUIResLoad[nUIResLoadIndex];

                    if (resLoad == null)
                    {
                        continue;
                    }

                    if (resLoad.texture == null || resLoad.texture.isDisposed)
                    {
                        LoadUIResFail(resLoad);
                        continue;
                    }

                    // 隐藏的节点优先级降低
                    if (!resLoad.texture.visible)
                    {
                        if (!bAllDisactive)
                        {
                            ++resLoad.nQueueWeight;

                            if (resLoad.nQueueWeight <= MAX_QUEUE_WEIGHT)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resLoad.nQueueWeight = 0;
                    }

                    Texture texture;

                    if (mapResTexture.TryGetValue(resLoad.strPath, out texture))
                    {
                        LoadUIResDone(resLoad, texture);
                        continue;
                    }

                    if (!resLoad.bRun)
                    {
                        resLoad.bRun = true;
                        OnAsyncLoadUIRes(resLoad);
                        ++nResLoadCountCur;
                    }

                    if (!resLoad.bSave || nResLoadCountCur >= MAX_SAVE_LOAD_TIMES)
                    {
                        nResLoadCountCur = 0;
                        break;
                    }
                }
            }
        }

        private void UnLoadUIResByTick()
        {
            if (lstUIResABCache.Count <= 0)
            {
                return;
            }

            int nRenderedFrameCountCur = Time.renderedFrameCount;

            for (int i = lstUIResABCache.Count - 1; i >= 0; i--)
            {
                UIGLoaderABCache unload = lstUIResABCache[i];

                if (unload == null || (nRenderedFrameCountCur >= (unload.nRenderedFrameCount + DEFAULT_RENDERED_FRAME_COUNT_DIFF)))
                {
                    lstUIResABCache.RemoveAt(i);

                    if (unload != null && unload.abTexture != null)
                    {
                        unload.abTexture.Unload(false);
                    }
                }
            }
        }
        
        private byte[] ReadFileAllBytes(string strPath)
        {
#if UNITY_EDITOR
            string strFilePath = string.Format(@"{0}\Editor Default Resources\ExternalRes\{1}", Application.dataPath, strPath);
            
            if (!File.Exists(strFilePath))
            {
                LogUtils.LogErrorFormat("ReadFileAllBytes File Not Found {0}", strPath);
                return null;
            }

            return File.ReadAllBytes(strFilePath);
#else
            return GameManager.Instance.ReadFileAllBytes(strPath);
#endif
        }
    }
}
