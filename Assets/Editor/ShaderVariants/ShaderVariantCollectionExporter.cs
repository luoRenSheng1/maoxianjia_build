using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using marijnz.EditorCoroutines;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using ShaderVariantCollector;

public class ShaderVariantCollectionExporter : EditorWindow
{
    private static EditorWindow _window;
    private static EditorWindow _gameView;
    private static string projectPath = string.Empty;
    private static string scenePath = "Assets/Editor Default Resources/Scene/TestScene.unity";
    private static UnityEngine.Object shader_dir;
    private static UnityEngine.Object material_dir;
    private static UnityEngine.Object material_dir2;
    private static string shaderDirPath = "Assets/Editor Default Resources/Shader";
    private static string materialDirPath = "Assets/Editor Default Resources";
    private static string materialDirPath2 = "Assets/Editor Default Resources";
    private static Action completeActionAction;

    [MenuItem("Tools/E. Editor/CollectShaderVariantsWindow")]
    public static void ShowWindow()
    {
        _window = EditorWindow.GetWindow(typeof(ShaderVariantCollectionExporter));
        shader_dir = AssetDatabase.LoadMainAssetAtPath(shaderDirPath);
        material_dir = AssetDatabase.LoadMainAssetAtPath(materialDirPath);
        material_dir2 = AssetDatabase.LoadMainAssetAtPath(materialDirPath2);
    }

    void OnGUI()
    {
        projectPath = Application.dataPath.Replace("Assets", String.Empty);
        shader_dir = EditorGUILayout.ObjectField("Shader Directory:", shader_dir, typeof(UnityEngine.Object), false);
        material_dir = EditorGUILayout.ObjectField("Dynamic Material Directory", material_dir, typeof(UnityEngine.Object), false);
        material_dir2 = EditorGUILayout.ObjectField("Dynamic Material Directory2", material_dir2, typeof(UnityEngine.Object), false);

        if (GUILayout.Button("CollectShaderVariant"))
        {
            CollectShaderVariant();
        }
    }

    public static void CollectShaderVariantCommand(Action action = null)
    {

        if (_window == null)
        {
            _window = EditorWindow.GetWindow(typeof(ShaderVariantCollectionExporter), true, "CollectShaderVariant") as ShaderVariantCollectionExporter;
        }
        
        projectPath = Application.dataPath.Replace("Assets", String.Empty);
        shader_dir = AssetDatabase.LoadMainAssetAtPath(shaderDirPath);
        material_dir = AssetDatabase.LoadMainAssetAtPath(materialDirPath);
        material_dir2 = AssetDatabase.LoadMainAssetAtPath(materialDirPath2);
        
        completeActionAction = action;
        _window.StartCoroutine(DynamicShaderVariantsCollector());
    }

    [MenuItem("Assets/CollectShaderVariant")]
    public static void CollectShaderVariant()
    {
        if (_window == null)
        {
            _window = EditorWindow.GetWindow( typeof( ShaderVariantCollectionExporter ), true, "CollectShaderVariant" ) as ShaderVariantCollectionExporter;
        }

        if (material_dir != null)
        {
            materialDirPath = AssetDatabase.GetAssetPath(material_dir);
        }
        else
        {
            materialDirPath = string.Empty;
        }
        
        if (material_dir2 != null)
        {
            materialDirPath2 = AssetDatabase.GetAssetPath(material_dir2);
        }
        else
        {
            materialDirPath2 = string.Empty;
        }
        
        if (shader_dir != null)
        {
            shaderDirPath = AssetDatabase.GetAssetPath(shader_dir);
        }

        if(!string.IsNullOrEmpty(materialDirPath) && !string.IsNullOrEmpty(shaderDirPath))
        {
            _window.StartCoroutine(DynamicShaderVariantsCollector()); 
        }
        else if (!string.IsNullOrEmpty(shaderDirPath))
        {
            ShaderVariantCollection();
        }
    }

    static void ShaderVariantCollection()
    {
        var tempPath = shaderDirPath + "/ShaderVariants.shadervariants";
        int addCount = 0;
        //if (File.Exists(tempPath)) 
        //{
        //    AssetDatabase.DeleteAsset(tempPath);
        //    ShaderVariantUtils.ClearCurrentShaderVariantCollection();
        //    ShaderVariantUtils.ClearCache();
        //}
        
        if (!File.Exists(tempPath)) 
        {
            ShaderVariantCollection _svc = new ShaderVariantCollection();
            AssetDatabase.CreateAsset(_svc, tempPath);
            AssetDatabase.Refresh();
        }
        var svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(tempPath);

        string[] files = Directory.GetFiles(shaderDirPath, "*.shader", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            if (item.EndsWith(".meta"))
            {
                continue;
            }
            
            //string fPath = item.Substring(projectPath.Length, item.Length - projectPath.Length);
            string fPath = item.Replace("\\", "/");
            Shader _shader = AssetDatabase.LoadMainAssetAtPath(fPath) as Shader;
            if (_shader != null)
            {
                ShaderVariantCollection.ShaderVariant sv = new ShaderVariantCollection.ShaderVariant();
                sv.shader = _shader;
                if (svc.Add(sv))
                {
                    addCount++;
                }
            }
        }
        
        AssetDatabase.Refresh();
        Debug.LogFormat("add {0} ShaderVariants", addCount);
    }

    static IEnumerator DynamicShaderVariantsCollector()
    {
        ShaderVariantUtils.ClearCurrentShaderVariantCollection();
        ShaderVariantUtils.ClearCache();
        EditorSceneManager.OpenScene(scenePath);
        yield return new WaitForSeconds(0.5f);

        var assembly = typeof( UnityEditor.EditorWindow ).Assembly;
        System.Type GameViewType = assembly.GetType( "UnityEditor.GameView" );
        _gameView = EditorWindow.GetWindow( GameViewType );
        
        List<Material> materialList = CollectAllMaterialAssets();
        Debug.LogFormat("materialList {0} ", materialList.Count);
        
        List<Renderer> renderAbles;
        _CreateProxyRenderers(out renderAbles);
        for (int i = 0; i < renderAbles.Count; ++i) 
        {
            renderAbles[i].gameObject.SetActive(false);
        }

        int batchIndex = 0;
        foreach (var mat in materialList)
        {
            var renderer = renderAbles[ batchIndex ];
            renderer.sharedMaterial = mat;
            renderer.lightProbeUsage = LightProbeUsage.BlendProbes;
            renderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
            renderer.gameObject.SetActive(true);
            batchIndex++;
            
            if (batchIndex >= renderAbles.Count) 
            {
                batchIndex = 0;
                yield return null;
                if ( _gameView != null ) 
                {
                    _gameView.Repaint();
                }
            }
        }
        
        if (batchIndex > 0) 
        {
            yield return null;

            if ( _gameView != null ) 
            {
                _gameView.Repaint();
            }
        }
        
        yield return null;
        var shaderCount = ShaderVariantUtils.GetCurrentShaderVariantCollectionShaderCount();
        var shaderVariantCount = ShaderVariantUtils.GetCurrentShaderVariantCollectionVariantCount();
        Debug.LogFormat( "Currently tracked: {0} shaders {1} total variants.", shaderCount, shaderVariantCount);
        
        yield return new WaitForSeconds(0.5f);
        SaveShaderVariants();
        yield return new WaitForSeconds(0.2f);
        ShaderVariantCollection();
        yield return new WaitForSeconds(0.2f);

        if (completeActionAction != null)
        {
            completeActionAction.Invoke();
            _window.Close();
            _gameView.Close();
        }
    }
    
    static void SaveShaderVariants() 
    {
  
        string tempPath = shaderDirPath + "/ShaderVariants.shadervariants";        
        try {
            if (File.Exists(tempPath)) 
            {
                AssetDatabase.DeleteAsset( tempPath );
            }
            ShaderVariantUtils.SaveCurrentShaderVariantCollection( tempPath );
            AssetDatabase.Refresh();
            
        } finally {

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }
    }

    static List<Material> CollectAllMaterialAssets() 
    {
        List<Material> materialList = new List<Material>();
        string dirPath = projectPath + materialDirPath;
        
        string[] files = Directory.GetFiles(dirPath, "*.mat", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            string fPath = item.Substring(projectPath.Length, item.Length - projectPath.Length);
            fPath = fPath.Replace("\\", "/");
            Material mat = AssetDatabase.LoadMainAssetAtPath(fPath) as Material;
            if (mat != null)
            {
                materialList.Add(mat);
            }
        }
        
        dirPath = projectPath + materialDirPath2;
        files = Directory.GetFiles(dirPath, "*.mat", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            string fPath = item.Substring(projectPath.Length, item.Length - projectPath.Length);
            fPath = fPath.Replace("\\", "/");
            Material mat = AssetDatabase.LoadMainAssetAtPath(fPath) as Material;
            if (mat != null)
            {
                materialList.Add(mat);
            }
        }
        
        return materialList;
    }

    static void FitAllRenderersBoundingSphereInView() 
    {
        var camera = Camera.main;
        
        if (camera == null) 
        {
            Debug.LogError("no camear");
            return;
        }
        //Debug.Assert(camera.orthographic == false);
        
        var renderers = new Renderer[ 0 ];
        var roots = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        var offset = 0;
        for (int i = 0; i < roots.Length; ++i) 
        {
            var _renderers = roots[ i ].GetComponentsInChildren<Renderer>();
            Array.Resize( ref renderers, renderers.Length + _renderers.Length );
            Array.Copy( _renderers, 0, renderers, offset, _renderers.Length );
            offset += _renderers.Length;
        }
        
    }
    
    static Transform _CreateProxyRenderers(out List<Renderer> renderAbles) 
    {
        renderAbles = new List<Renderer>();
        var tempRoot = new GameObject( "__temp_root" ).transform;
        //tempRoot.hideFlags |= HideFlags.DontSave;
        // 创建用于材质渲染的几何体，渲染代理球直径1，间距为2
        var offset = new Vector3( -10, -10, -10 );
        offset.y += 10 * 2 + 2;
        for ( int k = 0; k < 15; ++k ) 
        {
            for ( int j = 0; j < 15; ++j ) 
            {
                for ( int i = 0; i < 15; ++i ) 
                {
                    var pos = new Vector3( i * 2, j * 2, k * 2 );
                    pos += offset;
                    var go = GameObject.CreatePrimitive( PrimitiveType.Sphere );
                    go.transform.parent = tempRoot;
                    go.transform.position = pos;
                    go.name = String.Format( "temp_renderer_{0}_{1}_{2}", i, j, k );
                    go.hideFlags |= HideFlags.DontSave;
                    var renderer = go.GetComponent<Renderer>();
                    renderAbles.Add( renderer );

                    renderer.receiveShadows = false;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                    renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                }
            }
        }

        tempRoot.position = new Vector3(0, -19, -6);
        //var camera = EnsureMainCamera();
        //if ( camera.CompareTag( "MainCamera" ) && camera.gameObject.hideFlags == HideFlags.DontSave ) 
        //{
        //    camera.transform.parent = tempRoot;
        //}
        //FitAllRenderersBoundingSphereInView();
        return tempRoot;
    }
    
    static Camera EnsureMainCamera() 
    {
        var camera = Camera.main;
        if (camera == null) 
        {
            var go = new GameObject( "__temp_camera" );
            camera = go.AddComponent<Camera>();
            camera.cullingMask = -1;
            camera.gameObject.hideFlags = HideFlags.DontSave;
            camera.tag = "MainCamera";
        }
        return camera;
    }
}
