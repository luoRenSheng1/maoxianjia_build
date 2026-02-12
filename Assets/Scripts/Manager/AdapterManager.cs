using System;
using Engine;
using EngineBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdapterManager : MonoBehaviour
{
    private static AdapterManager _instance = null;
    public static AdapterManager Instance { get { return _instance; } }

    public static EN_LOAD_TYPE LoadType { get; private set; } = EN_LOAD_TYPE.INIT;

    private List<string> lstFontLoader = new List<string>();

    private Font[] ttf = new Font[2];
    
    public class GroundMaterial
    {
        public EN_LOAD_TYPE loadType = EN_LOAD_TYPE.INIT;
        public int resLoadIndex = 0;
        public int resLoadMax = 0;
        public Delegate listeners;
        public Material base1;
        public Material base2;
        public Material back;
        public Sprite sprFarTree;
        public Material matWave;
        public Sprite[] sprFarObj;

        public void CheckLoadDone()
        {
            if (resLoadIndex >= resLoadMax)
            {
                loadType = EN_LOAD_TYPE.LOADED;

                if (listeners != null)
                {
                    var act = (Act)listeners;
                    act();
                    listeners = null;
                }
            }
        }
    }
    
    /// <summary>
    /// 地表地板、地板2、边框
    /// </summary>
    private Dictionary<int, GroundMaterial> dictMatGrount = new Dictionary<int, GroundMaterial>();
    
    public static void CreateNew()
    {
        LoadType = EN_LOAD_TYPE.LOADING;
        
#if UNITY_EDITOR
        if (!Utils.IsLoadModelFromAssetBundle())
        {
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor Default Resources/Manager/AdapterManager.prefab");
            var go = GameObject.Instantiate(obj) as GameObject;
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<AdapterManager>();
            _instance.Init();
        }
#endif

        if (_instance == null)
        {
            ModelManager.Instance.AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_NONE, "Manager/AdapterManager", OnLoadAssetDone);
        }
    }

    private static void OnLoadAssetDone(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
    {
        if (loader != null)
        {
            if (obj != null)
            {
                var go = GameObject.Instantiate(obj) as GameObject;
                DontDestroyOnLoad(go);
                _instance = go.GetComponent<AdapterManager>();
                _instance.Init();
            }
            else
            {
                LoadType = EN_LOAD_TYPE.LOADERROR;
            }
            
            ModelManager.Instance.ClearModel(loader.strAssetBundle);
        }
    }

    public static void CleanUp()
    {
        LoadType = EN_LOAD_TYPE.INIT;
        
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    public static void OnLoadWaitContinue()
    {
        LoadType = EN_LOAD_TYPE.LOADWAITCONTINUE;
    }
    
    public void InitFont()
    {

    }

    public void Init()
    {
        InitDone();
    }

    private void OnLoadAtlasDone(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
    {
        if (loader != null)
        {
            ModelManager.Instance.ClearModel(loader.strAssetBundle);

            CheckInited();
        }
    }

    private void OnLoadFontDone(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object obj)
    {
        if (loader != null)
        {
            var resName = loader.param as string;
            var font = obj as Font;
            lstFontLoader.Remove(resName);

            if (resName.Contains("Normal"))
            {
                ttf[0] = font;
            }
            else if (resName.Contains("maple_lit"))
            {
                ttf[1] = font;
            }

            ModelManager.Instance.ClearModel(loader.strAssetBundle);

            CheckInited();
        }
    }

    public void CheckInited()
    {
        if (lstFontLoader.Count <= 0)
        {
            InitDone();
        }
    }

    private void InitDone()
    {
        LoadType = EN_LOAD_TYPE.LOADED; 
    }

    public Font GetDefaultFont()
    {
        return GetFont(0);
    }

    public Font GetFont(int index)
    {
        if (index >= 0 && index < ttf.Length)
        {
            return ttf[index];
        }

        return null;
    }

    private GroundMaterial GetGroundMaterials(int index)
    {
        GroundMaterial ret = null;
        
        if (dictMatGrount.TryGetValue(index, out ret))
        {
            return ret;
        }

        return null;
    }

    public Material GetGroundBaseMaterial(int index)
    {
        var ret = GetGroundMaterials(index);

        if (ret != null)
        {
            return ret.base1;
        }

        return null;
    }
    
    public Material GetGroundBaseMaterial2(int index)
    {
        var ret = GetGroundMaterials(index);

        if (ret != null)
        {
            return ret.base2;
        }

        return null;
    }
    
    public Material GetGroundBackMaterial(int index)
    {
        var ret = GetGroundMaterials(index);

        if (ret != null)
        {
            return ret.back;
        }

        return null;
    }
    
    public Material GetGroundWaveMaterial(int index)
    {
        var ret = GetGroundMaterials(index);

        if (ret != null)
        {
            return ret.matWave;
        }

        return null;
    }
        
    public Sprite GetGroundFarTreeSprite(int index)
    {
        var ret = GetGroundMaterials(index);

        if (ret != null)
        {
            return ret.sprFarTree;
        }

        return null;
    }
    
    public Sprite[] GetGroundFarObjSprite(int index)
    {
        var ret = GetGroundMaterials(index);

        if (ret != null)
        {
            return ret.sprFarObj;
        }

        return null;
    }
    
    public void PreLoadGroundMaterials(int index, Act listener)
    {
    }
}
