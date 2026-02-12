using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Engine;
using EngineBase;
using ThirdParty;

namespace Engine
{
    public delegate void LoadDelegate(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object go);
    public delegate void LoadTextureDelegate(Texture tex);
    public delegate void LoadGameObjectDelegate(GameObject go);
    public delegate void LoadUIPanelDelegate(string name, AssetBundle ab);
    public delegate void LoadMaterialDelegate(Material mat);
    public delegate void LoadSpriteDelegate(Sprite spr);
    
    public enum MODEL_LOAD_TYPE
    {
        MODEL_LOAD_TYPE_NONE,
        MODEL_LOAD_TYPE_SCENE,                  // 场景
        MODEL_LOAD_TYPE_TEXTURE,                // 纹理
        MODEL_LOAD_TYPE_SPRITE,                 // 纹理
        MODEL_LOAD_TYPE_NORMAL_PREFAB,          // 预制体
        MODEL_LOAD_TYPE_COMMON_PREFAB,          // 公有预制体
        MODEL_LOAD_TYPE_MATERIAL,               // 材质
        MODEL_LOAD_TYPE_UI,                     // UI
        MODEL_LOAD_EFFECT,  //unity特效
        MODEL_LOAD_SOUND,
    }

    public enum ASYNC_LOAD_STATE
    {
        STATE_INIT,
        STATE_LOADING,
        STATE_LOADBUNDLEDONE,
        STATE_LOADED
    }

    public enum EN_LOAD_TYPE
    {
        INIT,
        LOADING,
        LOADED,
        LOADERROR,
        LOADWAITCONTINUE,
        TERMINATION,
    }
    
    public class ModelLoad
    {
        public MODEL_LOAD_TYPE nType = MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_NONE;
        public string strAssetName = string.Empty;
        public string strAssetBundle = string.Empty;
        public LoadDelegate onComplete = null;
        public bool bHaveTargetByOnComplete = false;
        public System.Object param = null;
        public System.Object param2 = null;
        public System.Object param3 = null;
        public int nLoadIndex = 0;  // 异步加载序号
    }

    public class AsyncModelLoad
    {
        public string strAssetBundle = string.Empty;
        public ASYNC_LOAD_STATE nState = ASYNC_LOAD_STATE.STATE_INIT;
        public List<ModelLoad> lstLoad = new List<ModelLoad>();
    }

    public class ModelManager
    {
        private static ModelManager _instance;

        public static ModelManager Instance
        {
            get { return _instance; }
        }

        private MonoBehaviour MonoHost { set; get; }

        #region 模型加载
        // 模型加载入口
        private AssetBundleManager m_assetManagerModel = null;

        // 公有资源是否已经加载
        public bool CommonResLoaded { get; private set; } = false;

        // 已经缓存的AB
        private Dictionary<string, int> m_cacheAssetBundle = new Dictionary<string, int>();

        // 异步加载队列
        private List<AsyncModelLoad> m_lstAsyncLoader = new List<AsyncModelLoad>();
        // 异步加载队列-临时队列
        private List<AsyncModelLoad> m_lstAsyncLoaderTmp = new List<AsyncModelLoad>();

        // 卸载AB队列，卸载AB不同步进行，而是通过Tick进行
        private Dictionary<string, int> m_setUnLoadAssetBundle = new Dictionary<string, int>();
        private Dictionary<string, int> m_setUnLoadAssetBundleTmp = new Dictionary<string, int>();

        // 卸载事件
        private bool m_hasUnloadObj = false;
        private float m_unloadUnusedAssetTimer = 0.0f;
        #endregion

        public void Init()
        {
            _instance = this;

            this.InitMonoHost();
        }

        public void InitMonoHost()
        {
            if (this.MonoHost != null)
            {
                return;
            }

            GameObject go = new GameObject();
            go.name = "ModelManager";
            GameObject.DontDestroyOnLoad(go);

            this.MonoHost = go.AddComponent<LoaderBehaviour>();
        }

        public void InitManifest(Action actionSuccess, Action actionFail)
        {
            if (!Utils.IsLoadModelFromAssetBundle())
            {
                actionSuccess?.Invoke();
                return;
            }

            m_assetManagerModel = new AssetBundleManager();
            m_assetManagerModel.MonoHost = this.MonoHost;
            m_assetManagerModel.FolderPath = "/model/";
            m_assetManagerModel.ManifestName = "model.ress";
            // m_assetManagerModel.Log = true;
            m_assetManagerModel.Init(actionSuccess, actionFail);
        }
        
        public void LoadCommonRes()
        {
            if (CommonResLoaded)
            {
                OnLoadCommonResDone();
            }
            else
            {
                this.MonoHost.StartCoroutine(OnLoadCommonRes());
            }
        }

        IEnumerator OnLoadCommonRes()
        {
            if (Utils.IsLoadModelFromAssetBundle())
            {
                var m_strAssetBundleCommonPath = "common.ress";

                m_assetManagerModel.lstAssetBundleCommonName = new List<string>();
                
#if UNITY_WEBGL && !UNITY_EDITOR
                var loadAssetBundleCommonNameDone = false;

                string path = VersionManager.Instance.GetGameDataPath(m_strAssetBundleCommonPath);
                
                VersionManager.Instance.HttpGetOutputByte(this.MonoHost, path,
                (byte[] bytes) =>
                {
                    if (bytes == null)
                    {
                        LogUtils.LogErrorFormat("CriticalError \tOnLoadCommonRes Content is Null {0}", path);
                        loadAssetBundleCommonNameDone = true;
                        return;
                    }
                    
                    string strFiles = Utils.GetUTF8StringWithoutBom(bytes);
                    strFiles = strFiles.Replace("\r", "");
                    string[] strLines = strFiles.Split('\n');

                    if (strLines != null)
                    {
                        foreach (var item in strLines)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                m_assetManagerModel.lstAssetBundleCommonName.Add(item);
                            }
                        }
                    }
                    
                    loadAssetBundleCommonNameDone = true;
                },
                () =>
                {
                    LogUtils.LogErrorFormat("CriticalError \tOnLoadCommonRes Fail {0}", path);
                    loadAssetBundleCommonNameDone = true;
                });
                
                while (false == loadAssetBundleCommonNameDone)
                {
                    yield return null;
                }
#else
                byte[] bytes = GameManager.Instance.ReadFileAllBytes(m_strAssetBundleCommonPath);

                if (bytes != null)
                {
                    string strFiles = BaseUtil.GetUTF8StringWithoutBom(bytes);
                    strFiles = strFiles.Replace("\r", "");
                    string[] strLines = strFiles.Split('\n');

                    if (strLines != null)
                    {
                        foreach (var item in strLines)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                m_assetManagerModel.lstAssetBundleCommonName.Add(item);
                            }
                        }
                    }
                }
#endif
            }

            ShaderMgr.Instance.Precache();

            while (false == ShaderMgr.Instance.Loaded)
            {
                yield return null;
            }

            OnLoadCommonResDone();
            CommonResLoaded = true;
        }

        public void OnLoadCommonResDone()
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOAD_COMMON_RES_SUCCESS);
        }

        public void Tick(float deltaSeconds)
        {
            LoadAssetBundleByTick();
            UnLoadAssetBundleByTick();
        }

        public void LateTick(float deltaSeconds)
        {
            LateTickByUnloadUnusedAssets(deltaSeconds);
        }

        public void CancelLoadAssetBundle()
        {
            if (this.MonoHost != null)
            {
                this.MonoHost.StopAllCoroutines();
            }

            m_lstAsyncLoader.Clear();
        }
        
        public void Cleanup(bool bReleaseCommonRes = false)
        {
            if (this.MonoHost != null)
            {
                this.MonoHost.StopAllCoroutines();
            }

            m_lstAsyncLoader.Clear();

            if (m_assetManagerModel != null)
            {
                m_assetManagerModel.Clear();
            }

            m_setUnLoadAssetBundle.Clear();
            m_setUnLoadAssetBundleTmp.Clear();

            if (m_assetManagerModel != null)
            {
                foreach (var item in m_cacheAssetBundle)
                {
                    for (int i = 0; i < item.Value; i++)
                    {
                        m_assetManagerModel.UnloadAssetBundle(item.Key);
                    }
                }
            }

            m_cacheAssetBundle.Clear();

            if (bReleaseCommonRes)
            {
                if (m_assetManagerModel != null)
                {
                    m_assetManagerModel.ClearCommonBundle();
                }
                
                CommonResLoaded = false;
            }

            Resources.UnloadUnusedAssets();
        }

        public void Dispose()
        {
            LogUtils.LogWarning("ModelManager:Dispose");

            this.CleanGameObj();
            this.Cleanup(true);
        }

        private string GetRealAssetName(string strAssetName)
        {
            return strAssetName.LastIndexOf('/') == -1 ?
                strAssetName : strAssetName.Substring(strAssetName.LastIndexOf('/') + 1,
                strAssetName.Length - strAssetName.LastIndexOf('/') - 1);
        }

        public string GetAssetBundleName(MODEL_LOAD_TYPE nType, string strAssetBundle)
        {
            if (string.IsNullOrEmpty(strAssetBundle))
            {
                return "";
            }

            strAssetBundle = string.Format("{0}.bin", strAssetBundle.ToLower());

            switch (nType)
            {
                case MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_SCENE:
                    {
                        strAssetBundle = string.Format("scenes/{0}", strAssetBundle);
                    }
                    break;

                case MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_TEXTURE:
                    {
                        strAssetBundle = string.Format("texture/{0}", strAssetBundle);
                    }
                    break;

                case MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_SPRITE:
                    {
                        strAssetBundle = string.Format("sprite/{0}", strAssetBundle);
                    }
                    break;

                case MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_NORMAL_PREFAB:
                    {
                        strAssetBundle = string.Format("prefabs/{0}", strAssetBundle);
                    }
                    break;

                case MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_COMMON_PREFAB:
                    {
                        strAssetBundle = string.Format("common/prefabs/{0}", strAssetBundle);
                    }
                    break;
                
                case MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_MATERIAL:
                    {
                        strAssetBundle = string.Format("{0}", strAssetBundle);
                    }
                    break;
                
                case MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_UI:
                    {
                        strAssetBundle = string.Format("uipanel/{0}", strAssetBundle);
                    }
                    break;
                case MODEL_LOAD_TYPE.MODEL_LOAD_EFFECT:
                {
                    strAssetBundle = string.Format("effect/{0}", strAssetBundle);
                }
                    break;
                case MODEL_LOAD_TYPE.MODEL_LOAD_SOUND:
                {
                    strAssetBundle = string.Format("sound/{0}", strAssetBundle);
                }
                    break;
                default:
                    break;
            }

            return strAssetBundle;
        }

        private void UnLoadAssetBundleByTick()
        {
            if (m_setUnLoadAssetBundle.Count <= 0)
            {
                return;
            }

            m_hasUnloadObj = true;
            m_setUnLoadAssetBundleTmp.Clear();

            foreach (var item in m_setUnLoadAssetBundle)
            {
                // 是否有模型加载中？加载中不允许卸载资源
                if (!IsModelLoading(item.Key))
                {
                    m_setUnLoadAssetBundleTmp[item.Key] = item.Value;
                }
            }

            foreach (var item in m_setUnLoadAssetBundleTmp)
            {
                m_setUnLoadAssetBundle.Remove(item.Key);

                for (int i = 0; i < item.Value; i++)
                {
                    m_assetManagerModel?.UnloadAssetBundle(item.Key);
                }

                int nValue = 0;

                if (m_cacheAssetBundle.TryGetValue(item.Key, out nValue))
                {
                    if (nValue <= item.Value)
                    {
                        m_cacheAssetBundle.Remove(item.Key);
                    }
                    else
                    {
                        m_cacheAssetBundle[item.Key] = nValue - item.Value;
                    }
                }
            }
        }

        public IEnumerator UnLoadAssetBundleAfterLogin()
        {
            if (SystemAdapter.DEFAULT_LOGIN_UNLOAD_UNUSED_ASSET_TIME <= 0.0f)
            {
                yield break;
            }

            yield return new WaitForSeconds(SystemAdapter.DEFAULT_LOGIN_UNLOAD_UNUSED_ASSET_TIME);

            LogUtils.LogWarning("UnLoadAssetBundleAfterLogin");

            Resources.UnloadUnusedAssets();

            yield return null;

            SDKInterface.Instance.TriggerGCBySDK();
        }

        private void LateTickByUnloadUnusedAssets(float deltaSeconds)
        {
            try
            {
                m_unloadUnusedAssetTimer += deltaSeconds;
                if (m_unloadUnusedAssetTimer >= SystemAdapter.DEFAULT_UNLOAD_UNUSED_ASSET_TIME && m_hasUnloadObj)
                {
                    Resources.UnloadUnusedAssets();
                    m_unloadUnusedAssetTimer = 0.0f;
                    m_hasUnloadObj = false;
                }
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }

        public bool IsModelLoading(string strAssetBundle)
        {
            for (int i = 0; i < m_lstAsyncLoader.Count; i++)
            {
                AsyncModelLoad item = m_lstAsyncLoader[i];

                if (item != null && item.strAssetBundle == strAssetBundle)
                {
                    return true;
                }
            }

            return false;
        }

        public void ClearModel(string strAssetBundle)
        {
            // 延迟卸载
            if (!m_setUnLoadAssetBundle.ContainsKey(strAssetBundle))
            {
                m_setUnLoadAssetBundle.Add(strAssetBundle, 1);
            }
            else
            {
                m_setUnLoadAssetBundle[strAssetBundle] = m_setUnLoadAssetBundle[strAssetBundle] + 1;
            }
        }
        
        // 释放AB存储关系，不真正释放AB资源（fairygui中AB资源自己维护）
        public void FreeModel(string assetBundleName)
        {
            m_assetManagerModel.FreeAssetBundle(assetBundleName);
        }

        public void RerocdAssetBundleLoad(string strAssetBundle)
        {
            if (!m_cacheAssetBundle.ContainsKey(strAssetBundle))
            {
                m_cacheAssetBundle.Add(strAssetBundle, 1);
            }
            else
            {
                m_cacheAssetBundle[strAssetBundle] = m_cacheAssetBundle[strAssetBundle] + 1;
            }
        }

        // 同步加载AB
        public AssetBundleInfo SyncLoadBundle(MODEL_LOAD_TYPE nType, string strAssetBundle)
        {
            ModelLoad load = new ModelLoad();

            load.strAssetBundle = GetAssetBundleName(nType, strAssetBundle);

            AssetBundleInfo ab = SyncLoadAssetBundle(load);

            if (ab == null || ab.assetBundle == null)
            {
                LogUtils.LogErrorFormat("assetBundle bundle is null!  {0}", load.strAssetBundle);
                return null;
            }

            return ab;
        }

        // 同步加载模型
        public UnityEngine.Object SyncLoadModel(MODEL_LOAD_TYPE nType, string strAssetBundle, string strAssetName)
        {
#if UNITY_EDITOR
            if (!Utils.IsLoadModelFromAssetBundle())
            {
                var path = string.Format("Assets/Editor Default Resources/Prefabs/{0}.prefab", strAssetName);
                GameObject synGo = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (synGo == null)
                {
                    LogUtils.LogErrorFormat(@"OnLoadNormalPrefab {0} Not Found", strAssetName);
                }
                return synGo;
            } 
#endif

            
            ModelLoad load = new ModelLoad();

            load.nType = nType;
            load.strAssetBundle = GetAssetBundleName(nType, strAssetBundle);
            load.strAssetName = string.IsNullOrEmpty(strAssetName) ? strAssetBundle : strAssetName;

            if (string.IsNullOrEmpty(load.strAssetBundle))
            {
                LogUtils.LogErrorFormat("SyncLoadModel strAssetBundle is null! type({0})", load.nType);
                return null;
            }

            AssetBundleInfo ab = SyncLoadAssetBundle(load);

            if (ab == null || ab.assetBundle == null)
            {
                LogUtils.LogErrorFormat("assetBundle bundle is null!  {0}", load.strAssetBundle);
                return null;
            }

            UnityEngine.Object go = ab.assetBundle.LoadAsset<UnityEngine.Object>(GetRealAssetName(load.strAssetName));

            if (go == null)
            {
                LogUtils.LogErrorFormat("assetBundle assetname is null!  {0}", load.strAssetName);
                return null;
            }

            return go;
        }

        private AssetBundleInfo SyncLoadAssetBundle(ModelLoad load)
        {
            if (load == null)
            {
                return null;
            }

            if (m_assetManagerModel == null)
            {
                return null;
            }

            AssetBundleInfo uiAssetbundle = m_assetManagerModel.LoadAssetBundle(load.strAssetBundle);

            if (uiAssetbundle == null || uiAssetbundle.assetBundle == null)
            {
                LogUtils.LogErrorFormat("assetBundle bundle is null!  {0}", load.strAssetBundle);
                return null;
            }

            RerocdAssetBundleLoad(load.strAssetBundle);

            return uiAssetbundle;
        }

        // 异步加载AB
        public ModelLoad AsyncLoadBundle(MODEL_LOAD_TYPE nType, string strAssetBundle, LoadDelegate onComplete,
            System.Object param = null, System.Object param2 = null, System.Object param3 = null,
            int nLoadIndex = -1, bool priority = false)
        {
            ModelLoad load = new ModelLoad();
            load.strAssetBundle = GetAssetBundleName(nType, strAssetBundle);
            load.onComplete = onComplete;
            load.bHaveTargetByOnComplete = (onComplete != null && onComplete.Target != null);
            load.param = param;
            load.param2 = param2;
            load.param3 = param3;
            load.nLoadIndex = nLoadIndex;

            AddAsyncModelLoad(load, priority);

            return load;
        }

        // 异步加载模型
        public ModelLoad AsyncLoadModel(MODEL_LOAD_TYPE nType, string strAssetBundle, LoadDelegate onComplete, string strAssetName = "",
            System.Object param = null, System.Object param2 = null, System.Object param3 = null, int nLoadIndex = -1,
            bool priority = false)
        {
            ModelLoad load = new ModelLoad();

            load.nType = nType;
            load.strAssetBundle = GetAssetBundleName(nType, strAssetBundle);
            load.strAssetName = string.IsNullOrEmpty(strAssetName) ? strAssetBundle : strAssetName;
            load.onComplete = onComplete;
            load.bHaveTargetByOnComplete = (onComplete != null && onComplete.Target != null);
            load.param = param;
            load.param2 = param2;
            load.param3 = param3;
            load.nLoadIndex = nLoadIndex;

            AddAsyncModelLoad(load, priority);

            return load;
        }

        private AsyncModelLoad AddAsyncModelLoad(ModelLoad load, bool priority = false)
        {
            if (load == null)
            {
                return null;
            }

            List<AsyncModelLoad> lstAsyncLoader = this.m_lstAsyncLoader;

            if (lstAsyncLoader == null)
            {
                return null;
            }

            AsyncModelLoad asyncLoad = GetAsyncModelLoadByName(load.strAssetBundle, lstAsyncLoader);

            if (asyncLoad == null)
            {
                asyncLoad = new AsyncModelLoad();
                asyncLoad.nState = ASYNC_LOAD_STATE.STATE_INIT;
                asyncLoad.strAssetBundle = load.strAssetBundle;
                lstAsyncLoader.Add(asyncLoad);
            }

            if (asyncLoad != null)
            {
                asyncLoad.lstLoad.Add(load);

                if (priority)
                {
                    lstAsyncLoader.Remove(asyncLoad);
                    lstAsyncLoader.Insert(0, asyncLoad);
                }
            }

            return asyncLoad;
        }

        private AsyncModelLoad GetAsyncModelLoadByName(string strAssetBundle, List<AsyncModelLoad> lstAsyncLoader)
        {
            foreach (var item in lstAsyncLoader)
            {
                if (item != null && item.strAssetBundle == strAssetBundle)
                {
                    return item;
                }
            }

            return null;
        }

        private void LoadAssetBundleByTick()
        {
            if (m_lstAsyncLoader.Count <= 0)
            {
                return;
            }

            if (m_assetManagerModel == null || !m_assetManagerModel.Inited)
            {
                return;
            }

            m_lstAsyncLoaderTmp.Clear();

            foreach (var item in m_lstAsyncLoader)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.nState == ASYNC_LOAD_STATE.STATE_INIT)
                {
                    m_lstAsyncLoaderTmp.Add(item);
                }

                if (m_lstAsyncLoaderTmp.Count >= SystemAdapter.MAX_LOAD_AMOUNT)
                {
                    break;
                }
            }

            foreach (var item in m_lstAsyncLoaderTmp)
            {
                if (item == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(item.strAssetBundle))
                {
                    if (item.lstLoad != null && item.lstLoad.Count > 0)
                    {
                        var load = item.lstLoad[0];
                        LogUtils.LogErrorFormat("LoadAssetBundleAsyn Error strAssetBundle is null type({0})", load.nType);
                    }

                    LoadBundleFailByName(item);
                }
                else
                {
                    item.nState = ASYNC_LOAD_STATE.STATE_LOADING;
                    m_assetManagerModel.LoadAssetBundleAsyn(item.strAssetBundle, this.OnAsyncLoadBundleByLoaderDone, item);
                }
            }
        }

        private void OnAsyncLoadBundleByLoaderDone(AssetBundleInfo ab, System.Object param)
        {
            AsyncModelLoad load = param as AsyncModelLoad;

            if (load == null)
            {
                LoadBundleFailByName(load);
                return;
            }

            load.nState = ASYNC_LOAD_STATE.STATE_LOADBUNDLEDONE;

            if (ab == null || ab.assetBundle == null)
            {
                LoadBundleFailByName(load);
                return;
            }

            RerocdAssetBundleLoad(load.strAssetBundle);

            // 遍历通知各个模型加载
            this.AsyncLoadBundleNameByLoader(ab, load);
        }

        private void AsyncLoadBundleNameByLoader(AssetBundleInfo ab, AsyncModelLoad load)
        {
            // 去除无效Load
            for (int i = load.lstLoad.Count - 1; i >= 0; --i)
            {
                ModelLoad loadItemTmp = load.lstLoad[i];

                if (loadItemTmp == null)
                {
                    load.lstLoad.RemoveAt(i);
                }
            }

            if (load.lstLoad.Count <= 0)
            {
                // 全部加载完毕
                load.nState = ASYNC_LOAD_STATE.STATE_LOADED;
                this.m_lstAsyncLoader.Remove(load);
                return;
            }

            ModelLoad loadItem = load.lstLoad[0];

            if (loadItem == null)
            {
                return;
            }

            m_assetManagerModel.LoadAssetBundleNameAsyn(ab, GetRealAssetName(loadItem.strAssetName), this.OnAsyncLoadBundleNameByLoaderDone, load);
        }

        private void OnAsyncLoadBundleNameByLoaderDone(AssetBundleInfo ab, string strAssetName, UnityEngine.Object obj, System.Object param)
        {
            AsyncModelLoad load = param as AsyncModelLoad;

            if (load == null)
            {
                return;
            }

            for (int i = load.lstLoad.Count - 1; i >= 0; --i)
            {
                ModelLoad item = load.lstLoad[i];

                if (item != null && GetRealAssetName(item.strAssetName) == strAssetName)
                {
                    load.lstLoad.RemoveAt(i);

                    if (strAssetName != string.Empty && obj == null)
                    {
                        LogUtils.LogErrorFormat("assetBundle assetname is null! {0}", item.strAssetName);
                        LoadBundleFail(item);
                        continue;
                    }

                    LoadBundleSuccess(item, ab, obj as UnityEngine.Object);
                }
            }

            AsyncLoadBundleNameByLoader(ab, load);
        }

        private void LoadBundleFailByName(AsyncModelLoad load)
        {
            if (load == null)
            {
                return;
            }

            foreach (var item in load.lstLoad)
            {
                LoadBundleFail(item);
            }

            this.m_lstAsyncLoader.Remove(load);
        }

        private void LoadBundleFail(ModelLoad load)
        {
            if (load == null)
            {
                return;
            }

            if (load.onComplete != null && load.bHaveTargetByOnComplete && load.onComplete.Target == null)
            {
                return;
            }

            if (load.onComplete != null)
            {
                load.onComplete(load, null, null);
            }
        }

        private void LoadBundleSuccess(ModelLoad load, AssetBundleInfo ab = null, UnityEngine.Object go = null)
        {
            if (load == null)
            {
                return;
            }

            // 存在委托事件，委托对象已销毁，此时创建对象就是空节点对象了，不创建对象，直接通知成功
            if (load.onComplete != null && load.bHaveTargetByOnComplete && load.onComplete.Target == null)
            {
                return;
            }

            if (load.onComplete != null)
            {
                load.onComplete(load, ab, go);
            }
        }

        #region 游戏业务
        // 缓存鱼的纹理
        // 缓存装备的纹理
        private Dictionary<string, Texture> dic_equipTex = new Dictionary<string, Texture>();

        public void CleanGameObj()
        {
            foreach (var item in dic_equipTex)
            {
                Resources.UnloadAsset(item.Value);
            }

            dic_equipTex.Clear();

            Resources.UnloadUnusedAssets();
        }

        public Texture GetEquipTex(string resNameSuf)
        {
            var resName = string.Format("EquipTex/{0}", resNameSuf);

            Texture tex = null;

            if (dic_equipTex.TryGetValue(resName, out tex))
            {
                return tex;
            }

            return null;
        }

        public void LoadEquipTex(string resNameSuf, LoadTextureDelegate actionDone)
        {
            var resName = string.Format("EquipTex/{0}", resNameSuf);

            if (string.IsNullOrEmpty(resName))
            {
                OnLoadEquipTexDone(null, actionDone);
                return;
            }

            Texture tex = null;

            if (dic_equipTex.TryGetValue(resName, out tex))
            {
                OnLoadEquipTexDone(tex, actionDone);
                return;
            }

            OnLoadEquipTex(resName, actionDone);
        }

        private void OnLoadEquipTex(string resName, LoadTextureDelegate actionDone)
        {
#if UNITY_EDITOR
            if (!Utils.IsLoadModelFromAssetBundle())
            {
                var path = string.Format("Assets/Editor Default Resources/Texture/{0}.png", resName);
                var tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(path);
                if (tex == null)
                {
                    LogUtils.LogErrorFormat(@"OnLoadEquipTex {0} Not Found", resName);
                }
                CacheEquipTex(resName, tex);
                OnLoadEquipTexDone(tex, actionDone);
                return;
            }
#endif

            AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_TEXTURE, resName, OnLoadEquipTexComplete, param: resName, param2: actionDone);
        }

        private void OnLoadEquipTexComplete(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
        {
            if (loader != null)
            {
                var tex = obj as Texture;
                var resName = (string)loader.param;
                var actionDone = (LoadTextureDelegate)loader.param2;
                if (tex != null)
                {
                    CacheEquipTex(resName, tex);
                }
                OnLoadEquipTexDone(tex, actionDone);
                ModelManager.Instance.ClearModel(loader.strAssetBundle);
            }
        }

        private void CacheEquipTex(string resName, Texture tex)
        {
            if (!dic_equipTex.ContainsKey(resName))
            {
                dic_equipTex.Add(resName, tex);
            }
        }

        private void OnLoadEquipTexDone(Texture tex, LoadTextureDelegate actionDone)
        {
            try
            {
                if (actionDone != null)
                {
                    actionDone.Invoke(tex);
                    actionDone = null;
                }
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }

        #region NormalPrefab
        public void LoadNormalPrefab(string resName, LoadGameObjectDelegate actionDone)
        {
            if (string.IsNullOrEmpty(resName))
            {
                OnLoadNormalPrefabDone(null, actionDone);
                return;
            }

            OnLoadNormalPrefab(resName, actionDone);
        }

        private void OnLoadNormalPrefab(string resName, LoadGameObjectDelegate actionDone)
        {
#if UNITY_EDITOR
            if (!Utils.IsLoadModelFromAssetBundle())
            {
                var path = string.Format("Assets/Editor Default Resources/Prefabs/{0}.prefab", resName);
                var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (go == null)
                {
                    LogUtils.LogErrorFormat(@"OnLoadNormalPrefab {0} Not Found", resName);
                }
                OnLoadNormalPrefabDone(go, actionDone);
                return;
            }
#endif

            AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_NORMAL_PREFAB, resName, OnLoadNormalPrefabComplete, param: resName, param2: actionDone);
        }

        private void OnLoadNormalPrefabComplete(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
        {
            if (loader != null)
            {
                var go = obj as GameObject;
                var resName = (string)loader.param;
                var actionDone = (LoadGameObjectDelegate)loader.param2;
                OnLoadNormalPrefabDone(go, actionDone);
                ModelManager.Instance.ClearModel(loader.strAssetBundle);
            }
        }

        private void OnLoadNormalPrefabDone(GameObject go, LoadGameObjectDelegate actionDone)
        {
            try
            {
                if (actionDone != null)
                {
                    actionDone.Invoke(go);
                    actionDone = null;
                }
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }
        #endregion
        
        #region CommonPrefab
        public void LoadCommonPrefab(string resName, LoadGameObjectDelegate actionDone)
        {
            if (string.IsNullOrEmpty(resName))
            {
                OnLoadCommonPrefabDone(null, actionDone);
                return;
            }

            OnLoadCommonPrefab(resName, actionDone);
        }

        private void OnLoadCommonPrefab(string resName, LoadGameObjectDelegate actionDone)
        {
#if UNITY_EDITOR
            if (!Utils.IsLoadModelFromAssetBundle())
            {
                var path = string.Format("Assets/Editor Default Resources/Common/Prefabs/{0}.prefab", resName);
                var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (go == null)
                {
                    LogUtils.LogErrorFormat(@"OnLoadCommonPrefab {0} Not Found", resName);
                }
                OnLoadCommonPrefabDone(go, actionDone);
                return;
            }
#endif

            AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_COMMON_PREFAB, resName, OnLoadCommonPrefabComplete, param: resName, param2: actionDone);
        }

        private void OnLoadCommonPrefabComplete(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
        {
            if (loader != null)
            {
                var go = obj as GameObject;
                var resName = (string)loader.param;
                var actionDone = (LoadGameObjectDelegate)loader.param2;
                OnLoadCommonPrefabDone(go, actionDone);
                ModelManager.Instance.ClearModel(loader.strAssetBundle);
            }
        }

        private void OnLoadCommonPrefabDone(GameObject go, LoadGameObjectDelegate actionDone)
        {
            try
            {
                if (actionDone != null)
                {
                    actionDone.Invoke(go);
                    actionDone = null;
                }
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }
        #endregion

        public void LoadNormalMaterial(string resName, LoadMaterialDelegate actionDone)
        {
            if (string.IsNullOrEmpty(resName))
            {
                OnLoadNormalMaterialDone(null, actionDone);
                return;
            }

            OnLoadNormalMaterial(resName, actionDone);
        }

        private void OnLoadNormalMaterial(string resName, LoadMaterialDelegate actionDone)
        {
#if UNITY_EDITOR
            if (!Utils.IsLoadModelFromAssetBundle())
            {
                var path = string.Format("Assets/Editor Default Resources/{0}.mat", resName);
                var go = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
                if (go == null)
                {
                    LogUtils.LogErrorFormat(@"OnLoadNormalMaterial {0} Not Found", resName);
                }
                OnLoadNormalMaterialDone(go, actionDone);
                return;
            }
#endif

            AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_MATERIAL, resName, OnLoadNormalMaterialComplete, param: resName, param2: actionDone);
        }

        private void OnLoadNormalMaterialComplete(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
        {
            if (loader != null)
            {
                var go = obj as Material;
                var resName = (string)loader.param;
                var actionDone = (LoadMaterialDelegate)loader.param2;
                OnLoadNormalMaterialDone(go, actionDone);
                ModelManager.Instance.ClearModel(loader.strAssetBundle);
            }
        }

        private void OnLoadNormalMaterialDone(Material mat, LoadMaterialDelegate actionDone)
        {
            try
            {
                if (actionDone != null)
                {
                    actionDone.Invoke(mat);
                    actionDone = null;
                }
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }
        
        public void LoadUIPanel(string resName, LoadUIPanelDelegate actionDone)
        {
            if (string.IsNullOrEmpty(resName))
            {
                OnLoadUIPanelDone(resName, null, actionDone);
                return;
            }

            AsyncLoadBundle(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_UI, resName, OnLoadUIPanelComplete, param: resName, param2: actionDone);
        }

        private void OnLoadUIPanelComplete(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
        {
            if (loader != null)
            {
                var resName = (string)loader.param;
                var actionDone = (LoadUIPanelDelegate)loader.param2;
                OnLoadUIPanelDone(resName, ab != null ? ab.assetBundle : null, actionDone);
                ModelManager.Instance.FreeModel(loader.strAssetBundle);
            }
        }

        private void OnLoadUIPanelDone(string resName, AssetBundle ab, LoadUIPanelDelegate actionDone)
        {
            try
            {
                if (actionDone != null)
                {
                    actionDone.Invoke(resName, ab);
                    actionDone = null;
                }
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }
        #endregion
    }
}
