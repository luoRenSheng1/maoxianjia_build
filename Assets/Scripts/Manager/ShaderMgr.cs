using UnityEngine;
using System.Collections.Generic;
using Engine;
using EngineBase;

namespace Engine
{
    public class ShaderMgr : TSingleton<ShaderMgr>
    {
        public const string LEGECY_SHADERS_DIFFUSE = "Legacy Shaders/Diffuse";

        private readonly string abPath = "shader/shader";

        public bool Loaded { get; private set; } = false;

        private Dictionary<string, Shader> m_dicShaders = new Dictionary<string, Shader>();

        public void Precache()
        {
            // StopwatchMgr.BegineStopwatch("LoadBundleVariant");

            if (Loaded)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Utils.IsLoadModelFromAssetBundle())
            {
                Loaded = true;
                return;
            }
#endif

            ModelManager.Instance.AsyncLoadBundle(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_NONE, abPath, this.OnLoadShadersDone);
        }

        public void Cleanup()
        {
            Loaded = false;

            m_dicShaders.Clear();
        }

        private void OnLoadShadersDone(ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object go)
        {
            if (ab == null || ab.assetBundle == null)
            {
                OnPreloadShader();
                return;
            }

            Object[] allAssets = ab.assetBundle.LoadAllAssets();

            if (null != allAssets)
            {
                ShaderVariantCollection variantCollection = null;

                foreach (var item in allAssets)
                {
                    Shader shader = item as Shader;
                    if (null != shader)
                    {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                        m_dicShaders[shader.name] = Shader.Find(shader.name);
#else
                        m_dicShaders[shader.name] = shader;
#endif
                    }
                    else
                    {
                        if (null == variantCollection)
                        {
                            variantCollection = item as ShaderVariantCollection;
                        }
                    }
                }

                if (null != variantCollection)
                {
                    variantCollection.WarmUp();
                }
            }

            OnPreloadShader();
        }

        private void OnPreloadShader()
        {
            Find("XSJ/VFX/FxStandard");

            Loaded = true;
            // StopwatchMgr.EndStopwatch("LoadBundleVariant");
        }

        private Shader GetShader(string shaderName)
        {
            Shader shader = null;
            if (m_dicShaders.TryGetValue(shaderName, out shader))
            {
                return shader;
            }
            else
            {
                shader = Shader.Find(shaderName);
                if (null != shader)
                {
                    m_dicShaders[shader.name] = shader;
                }
            }

            if (null == shader)
            {
                shader = Shader.Find(LEGECY_SHADERS_DIFFUSE);
                LogUtils.LogErrorFormat("Shader Not Find,Name:{0}", shaderName);
            }

            return shader;
        }

        public static Shader Find(string shaderName)
        {
            return ShaderMgr.Instance.GetShader(shaderName);
        }
    }
}