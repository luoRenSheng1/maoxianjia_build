using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckOrChangeUnityShader : EditorWindow
{
    public enum MatType
    {
        Standard = 0,
        ParticleSystem = 1,
        Sprite = 2,
        Normal = 3,
    }

    static CheckOrChangeUnityShader self;
    static Material replacedStandardMat;

    static Material replacedSpriteMat;

    static Material replacedParticleSystemMat;
    static Dictionary<MatType, Material> typeAndMaterial;

    static List<Shader> shadersInProject;
    static List<Material> matsInProject;

    static Dictionary<string, Shader> shaderNameAndShaders;
    static Dictionary<string, List<Material>> shaderNameAndMats;
    static Dictionary<string, List<Material>> wrongShaderNameAndMats;
    static List<string> NotFoundShaderList;

    static List<Material> sameNameMat;
    static List<Material> noEnvenSameNameMat;
    static List<Material> standardMat;

    public static void InitReplacedMats()
    {
        replacedStandardMat =
            AssetDatabase.LoadAllAssetsAtPath("Assets/Editor Default Resources/Common/Materials/Default_Mat.mat")[0] as
                Material;
        replacedSpriteMat =
            AssetDatabase.LoadAllAssetsAtPath("Assets/Editor Default Resources/Common/Materials/Default_Sprite.mat")[0]
                as Material;
        replacedParticleSystemMat = 
            AssetDatabase.LoadAllAssetsAtPath("Assets/Editor Default Resources/Common/Materials/Default_ParticleSystem.mat")[0]
                as Material;
        typeAndMaterial = new Dictionary<MatType, Material>();
        typeAndMaterial.Add(MatType.ParticleSystem, replacedParticleSystemMat);
        typeAndMaterial.Add(MatType.Sprite, replacedSpriteMat);
        typeAndMaterial.Add(MatType.Standard, replacedStandardMat);
        typeAndMaterial.Add(MatType.Normal, replacedStandardMat);
        wrongShaderNameAndMats = new Dictionary<string, List<Material>>();

        shadersInProject = new List<Shader>();
        shaderNameAndShaders = new Dictionary<string, Shader>();
        matsInProject = new List<Material>();
        shaderNameAndMats = new Dictionary<string, List<Material>>();
        NotFoundShaderList = new List<string>();
        sameNameMat = new List<Material>();
        noEnvenSameNameMat = new List<Material>();
        standardMat = new List<Material>();
    }

    private static void ClearShaderData()
    {
        shadersInProject = null;
        matsInProject = null;

        shaderNameAndShaders = null;
        shaderNameAndMats = null;
        wrongShaderNameAndMats = null;

        sameNameMat = null;
        noEnvenSameNameMat = null;
        standardMat = null;

        NotFoundShaderList = null;
    }

    public static MatType GetReplacedMatType(Material srcMat)
    {
        if (srcMat == null)
        {
            return MatType.Normal;
        }
        else if (srcMat.name.Equals("Default-ParticleSystem")
                 || srcMat.shader.name.Equals("Particles/Standard Unlit"))
        {
            return MatType.ParticleSystem;
        }
        else if (srcMat.name.Equals("Sprites-Default") || srcMat.shader.name.Equals("Sprites/Default"))
        {
            return MatType.Sprite;
        }
        else if (srcMat.name.Equals("Default-Material")
                 || srcMat.shader.name.Equals("Standard"))
        {
            return MatType.Standard;
        }

        return MatType.Normal;
    }

    [MenuItem("Tools/E. Editor/OptimizeShaders")]
    public static CheckOrChangeUnityShader GetWindow()
    {
        if (self == null)
            self = GetWindow<CheckOrChangeUnityShader>();
        self.Show();
        return self;
    }

    private void OnGUI()
    {
        GUILayout.Label("OptimizeShaders");
        if (GUILayout.Button("Check And Optimize Shaders"))
        {
            InitReplacedMats();

            CheckAndFixAllFbxStandardMat();

            FixPrefabStandardShader();

            ClearShaderData();
        }
    }

    #region fbx

    static void CheckAndFixAllFbxStandardMat()
    {
        string dirPath = "/Editor Default Resources/Common/fbx";
        string dirPathWindwos = Application.dataPath + dirPath;
        DirectoryInfo dir = new DirectoryInfo(dirPathWindwos);

        if (!dir.Exists)
        {
            return;
        }

        var files = dir.GetFiles("*.fbx");
        var i = 0;

        foreach (var item in files)
        {
            ++i;

            var assetPath = "Assets" + dirPath + "/" + item.Name;

            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (importer == null)
            {
                continue;
            }

            EditorUtility.DisplayProgressBar("ChangeModelMatExternal", "Fbx Step1 " + assetPath, i / files.Length);

            bool bChange = false;
            
            var sourceMaterials = typeof(ModelImporter)
                .GetProperty("sourceMaterials", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(importer) as AssetImporter.SourceAssetIdentifier[];

            if (sourceMaterials != null && sourceMaterials.Length > 0)
            {
                if (importer.materialImportMode != ModelImporterMaterialImportMode.ImportViaMaterialDescription)
                {
                    importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
                    bChange = true;
                }
            
                if (importer.materialLocation != ModelImporterMaterialLocation.InPrefab)
                {
                    importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
                    bChange = true;
                }
            }
            else
            {
                if (importer.materialImportMode != ModelImporterMaterialImportMode.ImportStandard)
                {
                    importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                    bChange = true;
                }
            
                if (importer.materialLocation != ModelImporterMaterialLocation.InPrefab)
                {
                    importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
                    bChange = true;
                }
            }

            if (bChange)
            {
                importer.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();

        i = 0;
        foreach (var item in files)
        {
            ++i;

            var assetPath = "Assets" + dirPath + "/" + item.Name;

            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (importer == null)
            {
                continue;
            }

            EditorUtility.DisplayProgressBar("ChangeModelMatExternal", "Fbx Step2 " + assetPath, i / files.Length);
            
            //所有renderer
            var sources = new List<Renderer>();
            var assets = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath);
            foreach (var asset in assets)
            {
                var source = asset as Renderer;

                if (source != null)
                {
                    sources.Add(source);
                }
            }

            //所有material
            var keys = new Dictionary<string, bool>();
            foreach (var render in sources)
            {
                foreach (var mat in render.sharedMaterials)
                {
                    if (mat != null && !keys.ContainsKey(mat.name))
                        keys.Add(mat.name, true);
                }
            }

            if (keys.Count > 0 || importer.materialLocation != ModelImporterMaterialLocation.InPrefab)
            {
                var newMaterial = replacedStandardMat;
                var kind = typeof(UnityEngine.Material);
                foreach (var it in keys)
                {
                    var id = new AssetImporter.SourceAssetIdentifier();
                    id.name = it.Key;
                    id.type = kind;
                    importer.RemoveRemap(id);
                    importer.AddRemap(id, newMaterial);
                }

                importer.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
    }

    #endregion

    #region prefabFix

    // 每个物体的检查和替换代码封装
    private static bool CheckAndReplaceObjectMats(ref GameObject obj)
    {
        Renderer[] allRender = obj.GetComponentsInChildren<Renderer>(true);

        bool needSave = false;
        for (int j = 0; j < allRender.Length; j++)
        {
            Material[] srcMats = allRender[j].sharedMaterials;
            bool needSaveMats = false;
            if (srcMats != null)
            {
                for (int k = 0; k < srcMats.Length; k++)
                {
                    var type = GetReplacedMatType(srcMats[k]);

                    if (type != MatType.Normal)
                    {
                        needSaveMats = true;
                        needSave = true;
                        srcMats[k] = typeAndMaterial[type];
                    }
                }

                if (needSaveMats)
                {
                    allRender[j].sharedMaterials = srcMats;
                }
            }
        }

        
        ParticleSystemRenderer[] allParticleSystemRenderers = obj.GetComponentsInChildren<ParticleSystemRenderer>(true);
        for (int j = 0; j < allParticleSystemRenderers.Length; j++)
        {
            var type = MatType.Normal;
            Material[] srcMats = allParticleSystemRenderers[j].sharedMaterials;
            if (srcMats != null)
            {
                bool needSaveMats = false;
                for (int k = 0; k < srcMats.Length; k++)
                {
                    type = MatType.Normal;
                    if (GetReplacedMatType(srcMats[k]) != MatType.Normal)
                    {
                        needSaveMats = true;
                        needSave = true;
                        srcMats[k] = typeAndMaterial[type];
                    }
                }

                if (needSaveMats)
                {
                    allParticleSystemRenderers[j].sharedMaterials = srcMats;
                }
            }

            Material trailMat = allParticleSystemRenderers[j].trailMaterial;
            type = MatType.Normal;
            if (GetReplacedMatType(trailMat) != MatType.Normal)
            {
                needSave = true;
                allParticleSystemRenderers[j].trailMaterial = typeAndMaterial[type];
            }
        }

        return needSave;
    }

    // 所有prefab检查修改
    [MenuItem("Assets/Shader/FixPrefabStandardShader")]
    static void FixPrefabStandardShader()
    {
        InitReplacedMats();
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;

        Dictionary<string, bool> wrongStandardPrefabList = new Dictionary<string, bool>();
        Dictionary<string, bool> wrongSpritePrefabList = new Dictionary<string, bool>();
        Dictionary<string, bool> wrongParticleSystemPrefabList = new Dictionary<string, bool>();

        Dictionary<MatType, Dictionary<string, bool>> wrongPrefabList =
            new Dictionary<MatType, Dictionary<string, bool>>();
        wrongPrefabList.Add(MatType.ParticleSystem, wrongParticleSystemPrefabList);
        wrongPrefabList.Add(MatType.Sprite, wrongSpritePrefabList);
        wrongPrefabList.Add(MatType.Standard, wrongStandardPrefabList);

        bool needSave = false;

        List<Material> ms = new List<Material>();
        GameObject prefabObj = null;

        string[] prefabGUIDArray =
            AssetDatabase.FindAssets("t:prefab", new string[1] { "Assets/Editor Default Resources" });
        for (int i = 0; i < prefabGUIDArray.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGUIDArray[i]);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
            {
                continue;
            }

            prefabObj = PrefabUtility.InstantiatePrefab(asset, SceneManager.GetActiveScene()) as GameObject;
            EditorUtility.DisplayProgressBar("FixPrefabStandardShader", "Prefab -> " + prefabObj.name,
                i / prefabGUIDArray.Length);

            needSave = CheckAndReplaceObjectMats(ref prefabObj);

            if (needSave)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabObj, path);
            }
        }

        EditorUtility.ClearProgressBar();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    #endregion
}