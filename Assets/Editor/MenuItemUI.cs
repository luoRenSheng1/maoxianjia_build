#pragma warning disable
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Engine;
using EngineBase;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using UnityEditor.Compilation;
using UnityEngine.Rendering;
using UnityEditor.U2D;
using UnityEngine.U2D;
using YooAsset.Editor;
using AndroidArchitecture = UnityEditor.AndroidArchitecture;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;

#if PF_WEIXIN && UNITY_WEBGL
using WeChatWASM;
#endif

#if PF_DOUYIN && UNITY_WEBGL
using StarkSDKTool;
#endif

public class MenuItemUI
{
    private static string s_strSuffix = ".bin";
    private static List<string> s_arrABFileExt = new List<string>();

    private static int TEXTURE_MAX_SIZE = 256;

    static string GetSystemTimeName()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    public static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        string nameTotal = "";

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null) continue;

            if (!e.path.ToLower().Contains("ci") && !e.path.ToLower().Contains("ready"))
            {
                continue;
            }

            if (e.enabled) names.Add(e.path);

            nameTotal += e.path + "\n";
        }

        Debug.LogWarning("GetBuildScenes() " + nameTotal);

        return names.ToArray();
    }

    public static void PublishRes()
    {
        AssetDatabase.Refresh();
        Caching.ClearCache();

        PublishExportAll();
    }

    [MenuItem("Tools/P. Publish/Export Model", false, 8001)]
    public static void PublishExportModel()
    {
        StopwatchMgr.BegineStopwatch("PublishExportModel");

        string abname = "model";
        string strDir = "/ArchivedBundles/" + abname;
        string strDirAssets = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;

        DirectoryInfo dirOutDir = new DirectoryInfo(strDirPath);

        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(strDirPath);
        }

        SetAssetBundlesNameMerge(Application.dataPath + "/Editor Default Resources/Shader/", "shader/shader.bin", true);

        SetAssetBundlesNameAlone(Application.dataPath + "/Editor Default Resources/Common/", Application.dataPath + "/Editor Default Resources/Common/", "Common", true);
        SetAssetBundlesNameAlone(Application.dataPath + "/Editor Default Resources/Font/", Application.dataPath + "/Editor Default Resources/Font/", "Font", false);
        SetAssetBundlesNameAlone(Application.dataPath + "/Editor Default Resources/Manager/", Application.dataPath + "/Editor Default Resources/Manager/", "Manager", false);
        SetAssetBundlesNameAlone(Application.dataPath + "/Editor Default Resources/Texture/", Application.dataPath + "/Editor Default Resources/Texture/", "Texture", true);
        SetAssetBundlesNameAlone(Application.dataPath + "/Editor Default Resources/Materials/", Application.dataPath + "/Editor Default Resources/Materials/", "Materials", true);
        SetAssetBundlesNameAlone(Application.dataPath + "/Editor Default Resources/Sound/", Application.dataPath + "/Editor Default Resources/Sound/", "Sound", true);

        SetAssetBundlesNameMerge(Application.dataPath + "/Editor Default Resources/UIPanel/", false);
        

        var dirPrefabs = new DirectoryInfo(Application.dataPath + "/Editor Default Resources/Prefabs/");

        foreach (var item in dirPrefabs.GetDirectories())
        {
            SetAssetBundlesNameAlone(Application.dataPath + "/Editor Default Resources/Prefabs/" + item.Name + "/",
                Application.dataPath + "/Editor Default Resources/Prefabs/" + item.Name + "/", "Prefabs/" + item.Name,
                false);
        }

        ClearAssetBundlesName("Editor Default Resources");

        AssetDatabase.Refresh();

        BuildPipeline.BuildAssetBundles(strDirAssets, GetBuildAssetBundleOptions(),
            EditorUserBuildSettings.activeBuildTarget);

        // ClearManifestFile(strDirPath);

        ClearNotUseAssetBundle(strDirAssets, abname, s_strSuffix);

        // 生成公有信息
        PublishExportModelCommonFile(strDirPath);

        var strRootPath = strDirAssets + "\\" + abname;

        if (File.Exists(strRootPath))
        {
            File.Copy(strRootPath, strRootPath + ".ress", true);
        }

        Debug.Log("PublishExportModel.");

        StopwatchMgr.EndStopwatch("PublishExportModel");
    }

    [MenuItem("Tools/P. Publish/Export Config", false, 8002)]
    public static void PublishExportConfig()
    {
        ExportWindow.XlsxToBinAndJson();

        Debug.Log("PublishConfig.");
    }

    [MenuItem("Tools/P. Publish/Export ExternalRes", false, 8003)]
    public static void PublishExportExternalRes()
    {
        string strDir = "/ArchivedBundles/res";
        string strInputPath = Application.dataPath + "/Editor Default Resources/ExternalRes/";
        string strOutputPath = Application.dataPath + strDir;
        
        DeleteDirectory(strOutputPath);
        Directory.CreateDirectory(strOutputPath);

        CopyDirectoryFile(strInputPath, strOutputPath);

        var dirOutput = new DirectoryInfo(strOutputPath);

        if (dirOutput.Exists)
        {
            var files = dirOutput.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Extension.Contains("jpg") || file.Extension.Contains("jpeg") || file.Extension.Contains("png") || file.Extension.Contains("PNG") )
                {
                    var newFile = file.FullName.Replace(file.Extension, ".bin");
                    File.Move(file.FullName, newFile);
                }
            }
        }
        
        AssetDatabase.Refresh();
        
        Debug.Log("PublishExportExternalRes.");
    }

    [MenuItem("Tools/P. Publish/Export HybridCLR", false, 8011)]
    public static void PublishExportHybridCLR()
    {
        DoPublishExportHybridCLR(true);
    }

    [MenuItem("Tools/P. Publish/Export HybridCLR(Update)", false, 8012)]
    public static void PublishExportHybridCLRUpdate()
    {
        DoPublishExportHybridCLR(false);
    }
    
    public static void DoPublishExportHybridCLR(bool generateAll)
    {
#if USE_HYBRIDCLR
        if (generateAll)
        {
            PrebuildCommand.GenerateAll();

            AssetDatabase.Refresh();
        }

        // 生成DLL
        BuildAndCopyABAOTHotUpdateDlls();

        // 重置DLL-Others
        List<string> aotMetaAssemblyFiles = new List<string>();
        
        foreach (var dll in SettingsUtil.AOTAssemblyNames)
        {
            aotMetaAssemblyFiles.Add(dll + ".bin");
        }

        byte[] tpdllNew = null;
        
        foreach (var aotDllName in aotMetaAssemblyFiles)
        {
            var tpdllpath = string.Format("{0}/ArchivedBundles/mxj/{1}", Application.dataPath, aotDllName);

            if (!File.Exists(tpdllpath))
            {
                UnityEngine.Debug.LogErrorFormat("PublishExportHybridCLR Error GameDLL Not Found {0}", tpdllpath);
                return;
            }

            var tpdllbys = File.ReadAllBytes(tpdllpath);
            int nHeadLen = sizeof(uint);
            int nDataLen = tpdllbys.Length;
            var indexOld = tpdllNew != null ? tpdllNew.Length : 0;
            var tpdllCur = new byte[indexOld + nHeadLen + nDataLen];
            if (tpdllNew != null)
            {
                Array.Copy(tpdllNew, 0, tpdllCur, 0, tpdllNew.Length);
            }
            tpdllNew = tpdllCur;
            
            byte[] data = BitConverter.GetBytes((uint)nDataLen);
            Array.Copy(data, 0, tpdllNew, indexOld, nHeadLen);
            Array.Copy(tpdllbys, 0, tpdllNew, indexOld + nHeadLen, nDataLen);
            File.Delete(tpdllpath);
        }

        if (tpdllNew == null)
        {
            UnityEngine.Debug.LogError("PublishExportHybridCLR Error TPDLL Not Found");
            return;
        }
        
        var tpdllpathNew = string.Format("{0}/ArchivedBundles/mxj/mxj9.bin", Application.dataPath);
        File.WriteAllBytes(tpdllpathNew, tpdllNew);

        {
            // 重置DLL-Engine
            var gamedllpath = string.Format("{0}/ArchivedBundles/mxj/Engine.bin", Application.dataPath);
            var gamedllpathNew = string.Format("{0}/ArchivedBundles/mxj/mxj1.bin", Application.dataPath);

            if (!File.Exists(gamedllpath))
            {
                UnityEngine.Debug.LogError("PublishExportHybridCLR Error Engine Not Found");
                return;
            }

            var bysMain = File.ReadAllBytes(gamedllpath);
            File.Delete(gamedllpath);
            File.WriteAllBytes(gamedllpathNew, bysMain);
        }
        
        {
            // 重置DLL-EngineBase
            var gamedllpath = string.Format("{0}/ArchivedBundles/mxj/enginebase.bin", Application.dataPath);
            var gamedllpathNew = string.Format("{0}/ArchivedBundles/mxj/mxj2.bin", Application.dataPath);

            if (!File.Exists(gamedllpath))
            {
                UnityEngine.Debug.LogError("PublishExportHybridCLR Error EngineBase Not Found");
                return;
            }

            var bysMain = File.ReadAllBytes(gamedllpath);
            File.Delete(gamedllpath);
            File.WriteAllBytes(gamedllpathNew, bysMain);
        }
        
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
        {
            string strDirPath = Application.dataPath + "/ArchivedBundles/webgl/mxj";

            {
                DirectoryInfo dirSrcDir = new DirectoryInfo(strDirPath);
                var files = dirSrcDir.GetFiles("*.bin");

                for (int i = files.Length - 1; i >= 0; i--)
                {
                    if (files[i].Name.Contains("_"))
                    {
                        File.Delete(files[i].FullName);
                    }
                }
            }

            {
                string strFileHash = "";

                DirectoryInfo dirSrcDir = new DirectoryInfo(strDirPath);

                foreach (var item in dirSrcDir.GetFiles("*.bin"))
                {
                    var bys = File.ReadAllBytes(item.FullName);

                    string hash = HashHelper.GetMD5(item.FullName).ToLower();

                    var fileNameNew = item.Name.Replace(".bin", "_" + hash + ".bin");

                    var path = strDirPath + "/" + fileNameNew;

                    File.WriteAllBytes(path, bys);

                    strFileHash += item.Name + "," + fileNameNew + "\n";
                }

                string strMD5FilePath = strDirPath + "/manifest.ress";
                File.Delete(strMD5FilePath);
                File.WriteAllText(strMD5FilePath, strFileHash);
            }

            {
                DirectoryInfo dirSrcDir = new DirectoryInfo(strDirPath);
                var files = dirSrcDir.GetFiles("*.bin");

                for (int i = files.Length - 1; i >= 0; i--)
                {
                    if (!files[i].Name.Contains("_"))
                    {
                        File.Delete(files[i].FullName);
                    }
                }
            }
        }

        Debug.Log("DoPublishExportHybridCLR.");
#endif
    }

    private static void BuildAndCopyABAOTHotUpdateDlls()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        CompileDllCommand.CompileDll(target);
        CopyABAOTHotUpdateDlls(target);
        AssetDatabase.Refresh();
    }

    private static void CopyABAOTHotUpdateDlls(BuildTarget target)
    {
        CopyAOTAssembliesToStreamingAssets(target);
        CopyHotUpdateAssembliesToStreamingAssets(target);
    }

    private static void CopyAOTAssembliesToStreamingAssets(BuildTarget target)
    {
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string aotAssembliesDstDir = Application.dataPath + "/ArchivedBundles";//Application.streamingAssetsPath;

        string dllBytesDirPath = $"{aotAssembliesDstDir}/mxj";

        if (!Directory.Exists(dllBytesDirPath))
        {
            Directory.CreateDirectory(dllBytesDirPath);
        }

        foreach (var dll in SettingsUtil.AOTAssemblyNames)
        {
            string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
            if (!File.Exists(srcDllPath))
            {
                Debug.LogError(
                    $"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                continue;
            }

            string dllBytesPath = $"{aotAssembliesDstDir}/mxj/{dll}.bin";
            File.Copy(srcDllPath, dllBytesPath, true);
            Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
        }
    }

    private static void CopyHotUpdateAssembliesToStreamingAssets(BuildTarget target)
    {
        string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        string hotfixAssembliesDstDir = Application.dataPath + "/ArchivedBundles";//Application.streamingAssetsPath;

        string dllBytesDirPath = $"{hotfixAssembliesDstDir}/mxj";

        if (!Directory.Exists(dllBytesDirPath))
        {
            Directory.CreateDirectory(dllBytesDirPath);
        }

        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string dllBytesPath = $"{hotfixAssembliesDstDir}/mxj/{dll}.bytes";
            dllBytesPath = dllBytesPath.Replace(".dll.bytes", ".bin");
            
            string dllPath = $"{hotfixDllSrcDir}/{dll}";

            if (File.Exists(dllPath))
            {
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
            else
            {
                foreach (var dir in HybridCLRSettings.Instance.externalHotUpdateAssembliyDirs)
                {
                    dllPath = $"{dir}/{dll}";

                    if (File.Exists(dllPath))
                    {
                        File.Copy(dllPath, dllBytesPath, true);
                        Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
                        break;
                    }
                }
            }
        }
    }

    [MenuItem("Tools/P. Publish/Export CommonFile", false, 8061)]
    public static void PublishExportModelCommonFile()
    {
        string abname = "model";
        string strDir = "/ArchivedBundles/" + abname;
        string strDirAssets = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;

        // 生成公有信息
        PublishExportModelCommonFile(strDirPath);

        Debug.Log("PublishExportModelCommonFile.");
    }

    [MenuItem("Tools/P. Publish/Export Archived Bundles", false, 8062)]
    public static void PublishArchivedBundles()
    {
        DoPublishArchivedBundles(true);
    }

    private static void DoPublishArchivedBundles(bool copyBuildinFiles)
    {
        AssetDatabase.Refresh();
        
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        Debug.Log($"Export Archived Bundles : {buildTarget}");

        var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
        var packageName = "DefaultPackage";
        var strVersion = GetDefaultPackageVersion();
        
        // 构建参数
        RawFileBuildParameters buildParameters = new RawFileBuildParameters();
        buildParameters.BuildOutputRoot = buildoutputRoot;
        buildParameters.BuildinFileRoot = streamingAssetsRoot;
        buildParameters.BuildPipeline = EBuildPipeline.RawFileBuildPipeline.ToString();
        buildParameters.BuildTarget = buildTarget;
        buildParameters.BuildMode = EBuildMode.ForceRebuild;
        buildParameters.PackageName = packageName;
        buildParameters.PackageVersion = strVersion;
        buildParameters.VerifyBuildingResult = true;
        buildParameters.FileNameStyle = EFileNameStyle.BundleName_HashName;
        buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.None;
        buildParameters.BuildinFileCopyParams = string.Empty;
        buildParameters.EncryptionServices = new EncryptionNone();
    
        // 执行构建
        RawFileBuildPipeline pipeline = new RawFileBuildPipeline();
        var buildResult = pipeline.Run(buildParameters, true);
        if (buildResult.Success)
        {
            Debug.Log($"Export Archived Bundles Success : {buildResult.OutputPackageDirectory}");

            if (copyBuildinFiles)
            {
                string strOutputPath = streamingAssetsRoot + packageName;
                DeleteDirectory(strOutputPath);
                Directory.CreateDirectory(strOutputPath);
            
                EditorUtility.DisplayProgressBar("拷贝内置文件：", "Doing some work...", 100);
                CopyDirectoryFile(buildResult.OutputPackageDirectory, strOutputPath);
                EditorUtility.ClearProgressBar();
            }
        }
        else
        {
            Debug.LogError($"Export Archived Bundles Failed: {buildResult.ErrorInfo}");
        }
    }

    [MenuItem("Tools/P. Publish/ExportAll(Update)", false, 8098)]
    public static void PublishOriginRes()
    {
        RefreshAssetXML();
        PublishExportModel();
        PublishExportConfig();
        PublishExportExternalRes();
        DoPublishExportHybridCLR(false);
        DoPublishArchivedBundles(false);
    }

    [MenuItem("Tools/P. Publish/ExportAll(Full)", false, 8099)]
    public static void PublishExportAll()
    {
        PublishExportModel();
        PublishExportConfig();
        PublishExportExternalRes();
        DoPublishExportHybridCLR(true);
        PublishArchivedBundles();
    }

    private static BuildAssetBundleOptions GetBuildAssetBundleOptions()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android
            || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            return BuildAssetBundleOptions.ChunkBasedCompression /* | BuildAssetBundleOptions.DisableWriteTypeTree*/;
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
        {
            return BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.AppendHashToAssetBundleName;
        }

        return BuildAssetBundleOptions.ChunkBasedCompression;
    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
    /// </summary>
    public static void ClearAssetBundlesName(string resPath)
    {
        var bChange = false;
        var allAssets = AssetDatabase.GetAllAssetBundleNames();
        var lstClearAssetBundleNames = new List<string>();

        for (int i = 0; i < allAssets.Length; i++)
        {
            var paths = AssetDatabase.GetAssetPathsFromAssetBundle(allAssets[i]);

            foreach (var path in paths)
            {
                if (!path.Contains(resPath))
                {
                    var importer = AssetImporter.GetAtPath(path);
                    importer.assetBundleName = string.Empty;
                    bChange = true;
                }
            }
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 设置AssetBundleName(统一打包)
    /// </summary>
    private static void SetAssetBundlesNameMerge(string source, string strName, bool bSubDir)
    {
        DirectoryInfo folder = new DirectoryInfo(source);

        if (!folder.Exists)
        {
            return;
        }

        FileSystemInfo[] files = folder.GetFileSystemInfos();

        int length = files.Length;

        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                if (bSubDir)
                {
                    SetAssetBundlesNameMerge(files[i].FullName, strName, bSubDir);
                }
            }
            else
            {
                if (IsAssetBundlesFile(files[i].Extension))
                {
                    SetAssetBundlesNameFileMerge(files[i].FullName, strName);
                }
            }
        }
    }

    /// <summary>
    /// 设置AssetBundleName(统一打包)
    /// </summary>
    private static void SetAssetBundlesNameMerge(string source, bool bSubDir)
    {
        DirectoryInfo folder = new DirectoryInfo(source);

        if (!folder.Exists)
        {
            return;
        }

        FileSystemInfo[] files = folder.GetFileSystemInfos();

        int length = files.Length;

        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                if (bSubDir)
                {
                    SetAssetBundlesNameMerge(files[i].FullName, bSubDir);
                }
            }
            else
            {
                if (IsAssetBundlesFile(files[i].Extension))
                {
                    SetAssetBundlesNameFileMerge(files[i].FullName);
                }
            }
        }
    }
    
    /// <summary>
    /// 设置AssetBundleName(根据文件名分别打包)
    /// </summary>
    private static void SetAssetBundlesNameAlone(string strSource, string strRoot, string strABRootName, bool bSubDir)
    {
        DirectoryInfo folder = new DirectoryInfo(strSource);

        if (!folder.Exists)
        {
            return;
        }

        FileSystemInfo[] files = folder.GetFileSystemInfos();

        int length = files.Length;

        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                if (bSubDir)
                {
                    SetAssetBundlesNameAlone(files[i].FullName, strRoot, strABRootName, bSubDir);
                }
            }
            else
            {
                if (IsAssetBundlesFile(files[i].Extension))
                {
                    SetAssetBundlesNameFileAlone(files[i].FullName, strRoot, strABRootName);
                }
            }
        }
    }

    private static bool IsAssetBundlesFile(string extension)
    {
        if (s_arrABFileExt.Count <= 0)
        {
            s_arrABFileExt.Add("prefab");
            s_arrABFileExt.Add("png");
            s_arrABFileExt.Add("jpg");
            s_arrABFileExt.Add("jpeg");
            s_arrABFileExt.Add("tga");
            s_arrABFileExt.Add("shadervariants");
            s_arrABFileExt.Add("shader");
            s_arrABFileExt.Add("mat");
            s_arrABFileExt.Add("bytes");
            s_arrABFileExt.Add("asset");
            s_arrABFileExt.Add("mp3");
            s_arrABFileExt.Add("wav");
            s_arrABFileExt.Add("ogg");
            s_arrABFileExt.Add("wma");
            s_arrABFileExt.Add("unity");
            s_arrABFileExt.Add("spriteatlas");
            s_arrABFileExt.Add("fbx");
            s_arrABFileExt.Add("anim");
            s_arrABFileExt.Add("ttf");
            s_arrABFileExt.Add("otf");
            s_arrABFileExt.Add("controller");
            s_arrABFileExt.Add("mesh");
            s_arrABFileExt.Add("bytes");
            s_arrABFileExt.Add("json");
			s_arrABFileExt.Add("txt");
        }

        foreach (var item in s_arrABFileExt)
        {
            if (extension.ToLower().Contains(item.ToLower()))
            {
                return true;
            }
        }

        return false;
    }

    private static void SetAssetBundlesNameFileAlone(string strSource, string strRoot, string strABRootName)
    {
        string _source = ReplaceSeparator(strSource);
        string _root = ReplaceSeparator(strRoot);
        string _diff = _source.Replace(_root, "").ToLower();

        //在代码中给资源设置AssetBundleName
        AssetImporter assetImporter = AssetImporter.GetAtPath("Assets" + _source.Substring(Application.dataPath.Length));
        if (assetImporter != null)
        {
            string assetName = _diff;
            if (!string.IsNullOrEmpty(strABRootName))
            {
                assetName = strABRootName.ToLower() + "/" + assetName;
            }

            assetName = assetName.Replace(Path.GetExtension(strSource).ToLower(), s_strSuffix);
            if (!assetImporter.assetBundleName.Equals(assetName))
            {
                assetImporter.assetBundleName = assetName;
            }
        }
        else
        {
            Debug.LogError("SetAssetBundlesNameFileAlone Error " + strSource);
        }
    }

    private static void SetAssetBundlesNameFileMerge(string source, string strName)
    {
        string _source = ReplaceSeparator(source);
        string _sourceDir = ReplaceSeparator(strName);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);

        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);

        if (!assetImporter.assetBundleName.Equals(_sourceDir))
        {
            assetImporter.assetBundleName = _sourceDir;
        }
    }

    private static void SetAssetBundlesNameFileMerge(string source)
    {
        string _source = ReplaceSeparator(source);
        string _fileName = Path.GetFileNameWithoutExtension(source);
        string[] _fileNames = _fileName.Split('_');

        if (_fileNames.Length > 0)
        {
            _fileName = _fileNames[0];
        }

        string _rootDirName = "";

        int index1 = _source.LastIndexOf('/');
        int index2 = _source.LastIndexOf('/', index1 - 1);

        if (index1 != -1 && index2 != -1)
        {
            _rootDirName = _source.Substring(index2 + 1, index1 - index2 - 1);
            _rootDirName = _rootDirName.ToLower() + "/";
        }

        //在代码中给资源设置AssetBundleName
        AssetImporter assetImporter = AssetImporter.GetAtPath("Assets" + _source.Substring(Application.dataPath.Length));

        string assetName = _rootDirName + _fileName + s_strSuffix;
        assetImporter.assetBundleName = assetName.ToLower();
    }
    
    private static string ReplaceSeparator(string s)
    {
        return s.Replace("\\", "/");
    }

    private static void ClearManifestFile(string source)
    {
        if (!Directory.Exists(source))
        {
            return;
        }

        ClearManifestFileSingle(source);

        DirectoryInfo folder = new DirectoryInfo(source);

        FileSystemInfo[] files = folder.GetFileSystemInfos();

        int length = files.Length;

        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                ClearManifestFile(files[i].FullName);
            }
        }
    }

    private static void ClearManifestFileSingle(string source)
    {
        if (!Directory.Exists(source))
        {
            return;
        }

        ArrayList urlAddr = new ArrayList();

        string[] fileList = Directory.GetFileSystemEntries(source);

        foreach (string file in fileList)
        {
            string newStr = Path.GetFullPath(file);
            string filename = Path.GetFileName(file);
            string strExtension = Path.GetExtension(newStr).ToLower();

            if (strExtension == ".manifest")
            {
                urlAddr.Add(filename);
            }
        }

        for (int i = 0; i < urlAddr.Count; i++)
        {
            FileInfo ofile = new FileInfo(source + "/" + urlAddr[i]);

            if (ofile.Exists)
            {
                ofile.Delete();
            }
        }
    }

    public static void ClearNotUseAssetBundle(string strDirPath, string strRootFile, string strSuffix)
    {
        // 清理无效资源
        AssetBundle bundle = AssetBundle.LoadFromFile(strDirPath + "\\" + strRootFile);

        if (bundle != null)
        {
            AssetBundleManifest Manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] arrAssets = Manifest.GetAllAssetBundles();

            for (int i = 0; i < arrAssets.Length; i++)
            {
                arrAssets[i] = ReplaceSeparator(arrAssets[i]);
            }

            ClearNotUseAssetBundleDir(strDirPath, s_strSuffix, arrAssets);

            bundle.Unload(false);
        }
    }

    public static void ClearNotUseAssetBundleDir(string strDirPath, string strSuffix, string[] arrAllAB)
    {
        DirectoryInfo dirDir = new DirectoryInfo(strDirPath);

        if (!dirDir.Exists)
        {
            return;
        }

        string[] strFileNames = Directory.GetFiles(strDirPath);
        List<string> lstDel = new List<string>();

        foreach (string file in strFileNames)
        {
            if (!File.Exists(file))
            {
                continue;
            }

            if (file.Contains(".meta"))
            {
                continue;
            }

            if (!file.Contains(strSuffix))
            {
                continue;
            }

            string strFileSrc = ReplaceSeparator(file);

            bool bExist = false;
            strFileSrc = strFileSrc.ToLower();

            foreach (var item in arrAllAB)
            {
                if (strFileSrc.Contains(item))
                {
                    bExist = true;
                    break;
                }
            }

            if (!bExist)
            {
                lstDel.Add(file);
            }
        }

        foreach (var item in lstDel)
        {
            File.Delete(item);
        }

        string[] strDir = Directory.GetDirectories(strDirPath);

        foreach (string dir in strDir)
        {
            ClearNotUseAssetBundleDir(dir, strSuffix, arrAllAB);
        }
    }

    public static void PublishExportModelCommonFile(string strModel)
    {
        string strContent = "";
        string strContentTmp = "";

        strContentTmp = "";
        PublishExportModelCommonFile(strModel + "/common", strModel, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        PublishExportModelCommonFile(strModel + "/uicommon", strModel, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        PublishExportModelCommonFile(strModel + "/atlas", strModel, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        PublishExportModelCommonFile(strModel + "/sprite", strModel, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        PublishExportModelCommonFile(strModel + "/font", strModel, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        PublishExportModelCommonFile(strModel + "/shader", strModel, ref strContentTmp);
        strContent += strContentTmp;

        string strModelCommonFile = Application.dataPath + "/ArchivedBundles/common.ress";

        if (File.Exists(strModelCommonFile))
        {
            File.Delete(strModelCommonFile);
        }

        File.WriteAllText(strModelCommonFile, strContent, Encoding.UTF8);
    }

    public static void PublishExportModelCommonFile(string strDirPath, string strRoot, ref string strContent)
    {
        DirectoryInfo dirDir = new DirectoryInfo(strDirPath);

        if (!dirDir.Exists)
        {
            return;
        }

        string strRootSep = ReplaceSeparator(strRoot);

        if (!strRootSep.EndsWith("/"))
        {
            strRootSep += "/";
        }

        foreach (var item in dirDir.GetFiles())
        {
            if (!item.Extension.Contains("bin"))
            {
                continue;
            }

            string strFileSep = ReplaceSeparator(item.FullName);
            string _diff = strFileSep.Replace(strRootSep, "").ToLower();
            strContent += _diff + "\n";
        }

        foreach (var item in dirDir.GetDirectories())
        {
            PublishExportModelCommonFile(item.FullName, strRoot, ref strContent);
        }
    }

    #region CopyPlatformResources
    public static void CopyPlatformResources(int platform)
    {
        var strRoot = Application.dataPath.Replace("Assets", "");
        strRoot = strRoot.Replace("/", "\\");
        var strPlatformRes = string.Format("{0}\\Tools\\PlatformResources\\{1}", strRoot, platform);

        CopyDirectoryFile(strPlatformRes, strRoot);

        AssetDatabase.Refresh();

        // 获取要清除的目录
        string directoryPath = "Assets/Resources/Empty/";
        string directoryPath2 = "Assets/EngineLauncher/Res/";

        // 获取目录下的所有文件
        string[] assetPaths = AssetDatabase.FindAssets("", new string[] { directoryPath, directoryPath2 });

        // 遍历所有文件并清除修改标记
        foreach (string assetPath in assetPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(assetPath);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (asset != null)
            {
                EditorUtility.ClearDirty(asset);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        Debug.LogWarning("CopyPlatformResources success " + platform.ToString());
    }
    #endregion
    
    [MenuItem("Tools/Z. ZBuild/RefreshCompilation", false, 9000)]
    public static void RefreshCompilation()
    {
        AssetDatabase.Refresh();
        CompilationPipeline.RequestScriptCompilation();
    }
    
    public static string GetPlatformName(int platform)
    {
        switch (platform)
        {
            case SDKInterface.PLATFORM_CODE_WEIXIN:
                return "WebGLWX";

            case SDKInterface.PLATFORM_CODE_DOUYIN:
                return "WebGLDY";
            
            case SDKInterface.PLATFORM_CODE_COMBO:
                return "omniAndroid";

            case SDKInterface.PLATFORM_CODE_COMBO_IOS:
                return "omniIOS";

            default:
                break;
        }

        return "";
    }
    
    [MenuItem("Tools/Z. ZBuild/PlatformSwitch/SwitchToWindows", false, 7001)]
    public static void SwitchToWindows()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64
            && EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone,
                BuildTarget.StandaloneWindows64);
        }
    }
    
    [MenuItem("Tools/Z. ZBuild/BuildWindows", false, 9101)]
    public static void PublishWindowsExe()
    {
        PlayerSettings.runInBackground = true;
        UnityEditor.WindowsStandalone.UserBuildSettings.createSolution = false;

#if USE_HYBRIDCLR
        EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
#endif

        string strOutputPath = "";

        string[] args = System.Environment.GetCommandLineArgs();

        foreach (var item in args)
        {
            if (item.Contains("BatOutputPath"))
            {
                strOutputPath = item.Replace("BatOutputPath", "");
                break;
            }
        }

        string strDir = Application.dataPath.Replace("/", "\\");
        strDir = Application.dataPath.Replace("Assets", "");

        string path = "";

        if (!string.IsNullOrEmpty(strOutputPath))
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);

            if (dir.Exists && dir.Parent.Exists)
            {
                path = dir.Parent + "\\" + strOutputPath + "\\"
                       + PlayerSettings.applicationIdentifier + "\\mxj.exe";
            }
        }
        else
        {
            path = strDir + "Tools\\"
                          + PlayerSettings.applicationIdentifier + "\\mxj.exe";
        }

        path = path.Replace("\\", "/");

        BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.StandaloneWindows64, BuildOptions.None);

        UnityEngine.Debug.Log(path + " Publish Done! ");
    }

    private static void DoPublishAndroid(bool bExportProject)
    {
#if USE_HYBRIDCLR
        EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
#endif

        PlayerSettings.Android.useAPKExpansionFiles = false;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SplashScreen.showUnityLogo = false;

        EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality32Bit;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.connectProfiler = false;
        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.buildScriptsOnly = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = bExportProject;
        EditorUserBuildSettings.buildAppBundle = false;

        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
        //    "ES3GLOBAL_DISABLED;CROSS_PLATFORM_INPUT;MOBILE_INPUT;UNITY_POST_PROCESSING_STACK_V2;EASY_MOBILE;EASY_MOBILE_PRO;EM_GPGS");

        string strOutputPath = "Builds";

        string[] args = System.Environment.GetCommandLineArgs();

        foreach (var item in args)
        {
            if (item.Contains("BatOutputPath"))
            {
                strOutputPath = item.Replace("BatOutputPath", "");
            }
        }

        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = "Tools/SHUAIKU.keystore";
        PlayerSettings.Android.keystorePass = "ShuaiKu";
        PlayerSettings.Android.keyaliasName = "bieming";
        PlayerSettings.Android.keyaliasPass = "ShuaiKu";

        string strDir = Application.dataPath.Replace("/", "\\");
        strDir = Application.dataPath.Replace("Assets", "");

        string path = "";
        string fileName = "";

        fileName = string.Format("{0}", PlayerSettings.applicationIdentifier);

        if (!bExportProject)
        {
            fileName += ".apk";
        }

        if (!string.IsNullOrEmpty(strOutputPath))
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);

            if (dir.Exists)
            {
                path = string.Format("{0}\\{1}", dir + "\\" + strOutputPath, fileName);
            }
        }
        else
        {
            path = string.Format("{0}\\{1}", strDir + "Tools", fileName);
        }

        UnityEngine.Debug.Log(path + " Publish Begin " + PlayerSettings.Android.useAPKExpansionFiles);

        var res = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);

        if (res.summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log(path + " Publish Fail! ");
        }
        else
        {
            UnityEngine.Debug.Log(path + " Publish Done! ");
        }

        PlayerSettings.Android.useCustomKeystore = false;
        PlayerSettings.Android.keystoreName = "";
        PlayerSettings.Android.keystorePass = "";
        PlayerSettings.Android.keyaliasName = "";
        PlayerSettings.Android.keyaliasPass = "";
    }
    
    [MenuItem("Tools/Z. ZBuild/PlatformSwitch/SwitchToAndroid", false, 7002)]
    public static void SwitchToAndroid()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }
    }
    
    [MenuItem("Tools/Z. ZBuild/BuildAndroid-APK", false, 9201)]
    public static void PublishAndroidAPK()
    {
        DoPublishAndroid(false);
    }

    [MenuItem("Tools/Z. ZBuild/BuildAndroid-Project", false, 9202)]
    public static void PublishAndroidProject()
    {
        DoPublishAndroid(true);
    }

    [MenuItem("Tools/Z. ZBuild/PlatformSwitch/SwitchToIOS", false, 7003)]
    public static void SwitchToIOS()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        }
    }

    [MenuItem("Tools/Z. ZBuild/BuildIOS-Project", false, 9301)]
    public static void PublishIOSProject()
    {
#if USE_HYBRIDCLR
        EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
#endif

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        // PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
        PlayerSettings.SplashScreen.showUnityLogo = false;

        EditorUserBuildSettings.connectProfiler = false;
        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.buildScriptsOnly = false;

        string strOutputPath = "";

        string[] args = System.Environment.GetCommandLineArgs();

        foreach (var item in args)
        {
            if (item.Contains("BatOutputPath"))
            {
                strOutputPath = item.Replace("BatOutputPath", "");
            }
        }

        string strDir = Application.dataPath.Replace("/", "\\");
        strDir = Application.dataPath.Replace("Assets", "");

        string path = "";
        string fileName = "";

        fileName = string.Format("{0}", PlayerSettings.applicationIdentifier);

        if (!string.IsNullOrEmpty(strOutputPath))
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);

            if (dir.Exists && dir.Parent.Exists)
            {
                path = string.Format("{0}\\{1}", dir.Parent + "\\" + strOutputPath, fileName);
            }
        }
        else
        {
            path = string.Format("{0}\\{1}", strDir + "Tools", fileName);
        }

        UnityEngine.Debug.Log(path + " Publish Begin ");

        var res = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.None);

        if (res.summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log(path + " Publish Fail! ");
        }
        else
        {
            UnityEngine.Debug.Log(path + " Publish Done! ");
        }
    }

    [MenuItem("Tools/Z. ZBuild/PlatformSwitch/SwitchToWEBGL", false, 7004)]
    public static void SwitchToWEBGL()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        }

#if USE_HYBRIDCLR
        if (EditorUserBuildSettings.il2CppCodeGeneration != UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed)
        {
            EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
        }
#if UNITY_WEBGL
        if (UnityEditor.WebGL.UserBuildSettings.codeOptimization != UnityEditor.WebGL.CodeOptimization.Speed)
        {
            UnityEditor.WebGL.UserBuildSettings.codeOptimization = UnityEditor.WebGL.CodeOptimization.Speed;
        }
#endif
#endif
        
        var path = Application.dataPath + "/HybridCLRData";

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    [MenuItem("Tools/Z. ZBuild/BuildWebGLWeiXin", false, 9401)]
    public static void PublishWEBGLWeiXin()
    {
#if USE_HYBRIDCLR
        EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
#if UNITY_WEBGL
        UnityEditor.WebGL.UserBuildSettings.codeOptimization = UnityEditor.WebGL.CodeOptimization.Speed;
#endif
#endif

#if PF_WEIXIN && UNITY_WEBGL
        WXEditorWin.DoExport(true);
#endif

        UnityEngine.Debug.Log("PublishWEBGLWeiXin Done! ");
    }

    [MenuItem("Tools/Z. ZBuild/BuildWebGLDouYin", false, 9402)]
    public static void PublishWEBGLDouYin()
    {
#if USE_HYBRIDCLR
        EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed;
#if UNITY_WEBGL
        UnityEditor.WebGL.UserBuildSettings.codeOptimization = UnityEditor.WebGL.CodeOptimization.Speed;
#endif
#endif

#if PF_DOUYIN && UNITY_WEBGL
        string strOutputPath = "";

        string[] args = System.Environment.GetCommandLineArgs();

        foreach (var item in args)
        {
            if (item.Contains("BatOutputPath"))
            {
                strOutputPath = item.Replace("BatOutputPath", "");
            }
        }

        string strDir = Application.dataPath.Replace("/", "\\");
        strDir = Application.dataPath.Replace("Assets", "");

        string path = "";
        string fileName = "com.zhuque.catsoup.webgldy.zip";

        if (!string.IsNullOrEmpty(strOutputPath))
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);

            if (dir.Exists && dir.Parent.Exists)
            {
                path = string.Format("{0}\\{1}", dir.Parent + "\\" + strOutputPath, fileName);
            }
        }
        else
        {
            path = string.Format("{0}\\{1}", strDir + "Tools", fileName);
        }

        UnityEngine.Debug.Log("PublishWEBGLDouYin Begin " + path);
        
        bool isCancelBuild = false;
        StarkSDKTool.Builder.BuildWebGL(StarkBuilderSettings.Instance, path, out isCancelBuild);
#endif
        
        UnityEngine.Debug.Log("PublishWEBGLDouYin Done! ");
    }
    
    /// <summary>
    /// A文件夹所有文件复制到b文件夹下
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="destPath"></param>
    public static void CopyDirectoryFile(string srcPath, string destPath)
    {
        if (Directory.Exists(srcPath))
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);
                        }

                        CopyDirectoryFile(i.FullName, destPath + "\\" + i.Name);
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("CopyDirectoryFile" + e);
            }
        }
        else
        {
            Debug.LogError("CopyDirectoryFile error path");
        }
    }

    public static void DeleteDirectory(string destinationDir)
    {
        if (!Directory.Exists(destinationDir))
        {
            return;
        }

        const int magicDust = 10;

        for (var gnomes = 1; gnomes <= magicDust; gnomes++)
        {
            try
            {
                Directory.Delete(destinationDir, true);
            }
            catch (DirectoryNotFoundException)
            {
                return; //good!
            }
            catch (IOException)
            {
                // IOException: The directory is not empty
                Console.WriteLine("********************************************");
                Console.WriteLine("********************************************");
                Console.WriteLine("********************************************");
                Console.WriteLine("Gnomes prevent deletion of {0}! Applying magic dust, attempt #{1}.", destinationDir,
                    gnomes);
                Console.WriteLine("********************************************");
                Console.WriteLine("********************************************");
                Console.WriteLine("********************************************");
                //see http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true for more magic
                Thread.Sleep(50);
                continue;
            }

            return;
        }
    }

    public static void CreateDirectory(string strDestPath)
    {
        Directory.CreateDirectory(strDestPath);
    }

    //刷新全部的xmlasset
    [MenuItem("Tools/T. Tools/刷新全部的配置")]
    public static void RefreshAssetXML()
    {
        // 获取要清除的目录
        string directoryPath = "Assets/ConfigExcels/";

        // 获取目录下的所有文件
        string[] assetPaths = AssetDatabase.FindAssets("", new string[] { directoryPath });

        // 遍历所有文件并清除修改标记
        foreach (string assetPath in assetPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(assetPath);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            Debug.LogWarning("刷新资源" + AssetDatabase.GUIDToAssetPath(assetPath));
            if (asset != null)
            {
                EditorUtility.ClearDirty(asset);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    [MenuItem("Tools/T. Tools/FindOutFontLostWords")]
    public static void FindOutFontLostWords()
    {
        {
            string strFontAllWords = Application.dataPath + "/Editor/FontAllWords.txt";
            string strChineseSimplified =
                Application.dataPath + "/Editor Default Resources/Localization/ChineseSimplified.txt";
            string textFontAllWords = File.ReadAllText(strFontAllWords);
            string textChineseSimplified = File.ReadAllText(strChineseSimplified);
            HashSet<char> setFontAllWords = new HashSet<char>();
            HashSet<char> setChineseSimplified = new HashSet<char>();

            foreach (char c in textChineseSimplified)
            {
                if (IsChinese(c) && !setChineseSimplified.Contains(c))
                {
                    setChineseSimplified.Add(c);
                }
            }

            foreach (char c in textFontAllWords)
            {
                if (IsChinese(c) && !setFontAllWords.Contains(c))
                {
                    setFontAllWords.Add(c);
                }
            }

            foreach (var chinese in setChineseSimplified)
            {
                if (!setFontAllWords.Contains(chinese))
                {
                    Debug.LogWarning("FindOutFontLostWords Normal " + chinese.ToString());
                }
            }
        }

        {
            string strFontAllWords = Application.dataPath + "/Editor/FontMiniWords.txt";
            string strChineseSimplified = Application.dataPath + "/EngineLauncher/Res/UIString.txt";
            string strReadyScene = Application.dataPath + "/EngineLauncher/ready.unity";
            string textFontAllWords = File.ReadAllText(strFontAllWords);
            string textChineseSimplified = File.ReadAllText(strChineseSimplified);
            var textReadyScene = File.ReadAllLines(strReadyScene);
            HashSet<char> setFontAllWords = new HashSet<char>();
            HashSet<char> setChineseSimplified = new HashSet<char>();

            foreach (char c in textChineseSimplified)
            {
                if (IsChinese(c) && !setChineseSimplified.Contains(c))
                {
                    setChineseSimplified.Add(c);
                }
            }

            foreach (var ready in textReadyScene)
            {
                if (ready.Contains("mText:"))
                {
                    string strCov = Regex.Unescape(ready);

                    foreach (char c in strCov)
                    {
                        if (IsChinese(c) && !setChineseSimplified.Contains(c))
                        {
                            setChineseSimplified.Add(c);
                        }
                    }
                }
            }

            foreach (char c in textFontAllWords)
            {
                if (IsChinese(c) && !setFontAllWords.Contains(c))
                {
                    setFontAllWords.Add(c);
                }
            }

            foreach (var chinese in setChineseSimplified)
            {
                if (!setFontAllWords.Contains(chinese))
                {
                    Debug.LogWarning("FindOutFontLostWords Mini " + chinese.ToString());
                }
            }
        }

        Debug.LogWarning("FindOutFontLostWords Done");
    }

    [MenuItem("Tools/T. Tools/FindOutUIAtlasLost")]
    public static void FindOutUIAtlasLost()
    {
        {
            string strPath = Application.dataPath + "/Editor Default Resources/Atlas/atlas_ui1.asset";
            string[] strLines = File.ReadAllLines(strPath);
            HashSet<string> setName = new HashSet<string>();

            foreach (var line in strLines)
            {
                if (line.Contains("- name:"))
                {
                    var name = line.Replace("- name:", "").Trim();

                    if (!setName.Contains(name))
                    {
                        setName.Add(name);
                    }
                }
            }

            HashSet<string> setFile = new HashSet<string>();

            DirectoryInfo dirOutDir = new DirectoryInfo(Application.dataPath + "/Editor Default Resources/Atlas/ui1");

            if (dirOutDir.Exists)
            {
                var files = dirOutDir.GetFiles();

                foreach (var file in files)
                {
                    if (file.Name.Contains("meta"))
                    {
                        continue;
                    }

                    var name = file.Name.Replace(".jpg", "").Trim();
                    name = name.Replace(".jpeg", "");
                    name = name.Replace(".png", "");
                    name = name.Replace(".PNG", "");
                    if (!setFile.Contains(name))
                    {
                        setFile.Add(name);
                    }
                }
            }

            HashSet<string> setFile2 = new HashSet<string>();

            DirectoryInfo dirOutDir2 = new DirectoryInfo(Application.dataPath + "/Z_2.23/Png/ui");

            if (dirOutDir2.Exists)
            {
                var files = dirOutDir2.GetFiles();

                foreach (var file in files)
                {
                    if (file.Name.Contains("meta"))
                    {
                        continue;
                    }

                    var name = file.Name.Replace(".jpg", "").Trim();
                    name = name.Replace(".jpeg", "");
                    name = name.Replace(".png", "");
                    name = name.Replace(".PNG", "");
                    if (!setFile2.Contains(name))
                    {
                        setFile2.Add(name);
                    }
                }
            }

            string strContent = "";
            string strContent2 = "";

            foreach (var name in setName)
            {
                if (!setFile.Contains(name))
                {
                    if (setFile2.Contains(name))
                    {
                        strContent2 += name + "\n";
                    }

                    strContent += name + "\n";
                }
            }

            File.WriteAllText(Application.dataPath + "/UIAtlasFile.txt", strContent);
            File.WriteAllText(Application.dataPath + "/UIAtlasFile2.txt", strContent2);

            strContent = "";

            foreach (var name in setFile)
            {
                if (!setName.Contains(name))
                {
                    strContent += name + "\n";
                }
            }

            File.WriteAllText(Application.dataPath + "/UIAtlasName.txt", strContent);
        }

        Debug.LogWarning("FindOutUIAtlasLost Done");
    }
    
    /*[MenuItem("Tools/T. Tools/FindOutUIAtlasLostEx")]
    public static void FindOutUIAtlasLostEx()
    {
        {
            string strPath = Application.dataPath + "/Editor Default Resources/Atlas/atlas_ui1.asset";
            string[] strLines = File.ReadAllLines(strPath);
            HashSet<string> setName = new HashSet<string>();
            
            foreach (var line in strLines)
            {
                if (line.Contains("- name:"))
                {
                    var name = line.Replace("- name:", "").Trim();

                    if (!setName.Contains(name))
                    {
                        setName.Add(name);
                    }
                }
            }

            string strPath2 = Application.dataPath + "/Editor Default Resources/Atlas/atlas_ui2.asset";
            string[] strLines2 = File.ReadAllLines(strPath2);
            HashSet<string> setName2 = new HashSet<string>();
            
            foreach (var line in strLines2)
            {
                if (line.Contains("- name:"))
                {
                    var name = line.Replace("- name:", "").Trim();

                    if (!setName2.Contains(name))
                    {
                        setName2.Add(name);
                    }
                }
            }

            string strContent = "";
            
            foreach (var name in setName)
            {
                if (!setName2.Contains(name))
                {
                    strContent += name + "\n";
                }
            }
            
            File.WriteAllText(Application.dataPath + "/UIAtlasFile.txt", strContent);
            
            strContent = "";
            
            foreach (var name in setName2)
            {
                if (!setName.Contains(name))
                {
                    strContent += name + "\n";
                }
            }
            
            File.WriteAllText(Application.dataPath + "/UIAtlasName.txt", strContent);
        }
        
        Debug.LogWarning("FindOutUIAtlasLostEx Done");
    }*/

    [MenuItem("Tools/T. Tools/CheckCommnoResName")]
    public static void CheckCommnoResName()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Editor Default Resources/Common");

        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Name.Contains("meta"))
                {
                    continue;
                }

                if (file.Name.Contains(" "))
                {
                    UnityEngine.Debug.LogWarning("CheckCommnoResName Have space " + file.FullName);
                }
            }
        }
        
        Debug.LogWarning("CheckCommnoResName Done");
    }
    
    [MenuItem("Tools/O. Optimize/优化模型-FBX设置", false, 5003)]
    public static void ChangeModelImporter()
    {
        {
            string strDir = Application.dataPath + "/Editor Default Resources/Common/fbx";

            DirectoryInfo dirOutDir = new DirectoryInfo(strDir);

            ChangeModelImporter(dirOutDir);
        }

        {
            string strDir = Application.dataPath + "/Editor Default Resources/fbx";

            DirectoryInfo dirOutDir = new DirectoryInfo(strDir);

            ChangeModelImporter(dirOutDir);
        }
    }

    public static void ChangeModelImporter(DirectoryInfo dirDir)
    {
        if (!dirDir.Exists)
        {
            return;
        }

        foreach (var item in dirDir.GetFiles())
        {
            if (item.FullName.Contains(".meta"))
            {
                continue;
            }

            string _source = ReplaceSeparator(item.FullName);
            string _root = ReplaceSeparator(Application.dataPath);
            string goPath = "Assets" + _source.Replace(_root, "");

            ModelImporter importer = AssetImporter.GetAtPath(goPath) as ModelImporter;

            if (importer == null)
            {
                continue;
            }

            bool bChange = false;

            if (importer.importAnimation)
            {
                if (importer.animationType == ModelImporterAnimationType.Legacy)
                {
                    if (importer.animationCompression !=
                        ModelImporterAnimationCompression.KeyframeReductionAndCompression)
                    {
                        importer.animationCompression =
                            ModelImporterAnimationCompression.KeyframeReductionAndCompression;
                        bChange = true;
                    }
                }
                else if (importer.animationType == ModelImporterAnimationType.Generic)
                {
                    if (importer.animationCompression != ModelImporterAnimationCompression.Optimal)
                    {
                        importer.animationCompression = ModelImporterAnimationCompression.Optimal;
                        bChange = true;
                    }
                }
            }

            //             if (importer.isReadable)
            //             {
            //                 importer.isReadable = false;
            //                 bChange = true;
            //             }

            if (bChange)
            {
                AssetDatabase.ImportAsset(goPath);
            }
        }

        AssetDatabase.Refresh();

        foreach (var item in dirDir.GetDirectories())
        {
            ChangeModelImporter(item);
        }
    }

    public static bool IsChinese(char c)
    {
        if (c >= 0x4E00 && c <= 0x9FA5)
        {
            return true;
        }

        return false;
    }

    [MenuItem("Tools/O. Optimize/优化模型-Prefab设置", false, 5005)]
    public static void ChangePrefabImporter()
    {
        OnChangePrefabImporter("Assets/Editor Default Resources/Prefabs", true);
        OnChangePrefabImporter("Assets/Editor Default Resources/Prefabs", true);
        OnChangePrefabImporter("Assets/Editor Default Resources/UI", false);
        OnChangePrefabImporter("Assets/Editor Default Resources/UI", false);
        AssetDatabase.Refresh();

        Debug.LogWarning("ChangePrefabImporter Done");
    }

    private static void OnChangePrefabImporter(string prefabPath, bool bPassRender)
    {
        try
        {
            string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabPath });

            var length = ids.Length;
            for (int i = 0; i < ids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(ids[i]);
                
                EditorUtility.DisplayProgressBar("优化Prefab：", $"Doing some work...{path}", i * 1.0f / length);

                GameObject originTwoCube = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                GameObject prefabGameObj = PrefabUtility.InstantiatePrefab(originTwoCube) as GameObject;
                bool bChange = false;
                ParticleSystem[] arrPS = prefabGameObj.GetComponentsInChildren<ParticleSystem>();
                if (arrPS != null)
                {
                    foreach (var ps in arrPS)
                    {
                        if (ps.maxParticles > 50)
                        {
                            ps.maxParticles = 50;
                            bChange = true;
                        }
                    }
                }

                if (bPassRender)
                {
                    if (arrPS != null && arrPS.Length > 0)
                    {
                        var arrRender = prefabGameObj.GetComponentsInChildren<ParticleSystemRenderer>();
                        foreach (var ren in arrRender)
                        {
                            if (ren.lightProbeUsage != LightProbeUsage.Off)
                            {
                                ren.lightProbeUsage = LightProbeUsage.Off;
                                bChange = true;
                            }
    
                            if (ren.reflectionProbeUsage != ReflectionProbeUsage.Off)
                            {
                                ren.reflectionProbeUsage = ReflectionProbeUsage.Off;
                                bChange = true;
                            }
    
                            if (ren.shadowCastingMode != ShadowCastingMode.Off)
                            {
                                ren.shadowCastingMode = ShadowCastingMode.Off;
                                bChange = true;
                            }
    
                            if (ren.receiveShadows != false)
                            {
                                ren.receiveShadows = false;
                                bChange = true;
                            }
    
                            if (ren.motionVectorGenerationMode != MotionVectorGenerationMode.ForceNoMotion)
                            {
                                ren.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                                bChange = true;
                            }
    
                            if (ren.allowOcclusionWhenDynamic != true)
                            {
                                ren.allowOcclusionWhenDynamic = true;
                                bChange = true;
                            }
                        }
                    }
    
                    {
                        var arrRender = prefabGameObj.GetComponentsInChildren<MeshRenderer>();
                        foreach (var ren in arrRender)
                        {
                            if (ren.lightProbeUsage != LightProbeUsage.Off)
                            {
                                ren.lightProbeUsage = LightProbeUsage.Off;
                                bChange = true;
                            }
    
                            if (ren.reflectionProbeUsage != ReflectionProbeUsage.Off)
                            {
                                ren.reflectionProbeUsage = ReflectionProbeUsage.Off;
                                bChange = true;
                            }
    
                            if (ren.shadowCastingMode != ShadowCastingMode.Off)
                            {
                                ren.shadowCastingMode = ShadowCastingMode.Off;
                                bChange = true;
                            }
    
                            if (ren.receiveShadows != false)
                            {
                                ren.receiveShadows = false;
                                bChange = true;
                            }
    
                            if (ren.motionVectorGenerationMode != MotionVectorGenerationMode.ForceNoMotion)
                            {
                                ren.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                                bChange = true;
                            }
    
                            if (ren.allowOcclusionWhenDynamic != true)
                            {
                                ren.allowOcclusionWhenDynamic = true;
                                bChange = true;
                            }
                        }
                    }
    
                    {
                        var arrRender = prefabGameObj.GetComponentsInChildren<SkinnedMeshRenderer>();
                        foreach (var ren in arrRender)
                        {
                            if (ren.lightProbeUsage != LightProbeUsage.Off)
                            {
                                ren.lightProbeUsage = LightProbeUsage.Off;
                                bChange = true;
                            }
    
                            if (ren.reflectionProbeUsage != ReflectionProbeUsage.Off)
                            {
                                ren.reflectionProbeUsage = ReflectionProbeUsage.Off;
                                bChange = true;
                            }
    
                            if (ren.shadowCastingMode != ShadowCastingMode.Off)
                            {
                                ren.shadowCastingMode = ShadowCastingMode.Off;
                                bChange = true;
                            }
    
                            if (ren.receiveShadows != false)
                            {
                                ren.receiveShadows = false;
                                bChange = true;
                            }
    
                            if (ren.motionVectorGenerationMode != MotionVectorGenerationMode.ForceNoMotion)
                            {
                                ren.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                                bChange = true;
                            }
    
                            if (ren.allowOcclusionWhenDynamic != true)
                            {
                                ren.allowOcclusionWhenDynamic = true;
                                bChange = true;
                            }
                        }
                    }
                }
                
                if (bChange)
                {
                    bool savePrefabResult;
                    PrefabUtility.SaveAsPrefabAsset(prefabGameObj, path, out savePrefabResult);
                    GameObject.DestroyImmediate(prefabGameObj);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    GameObject.DestroyImmediate(prefabGameObj);
                }
            }
            
            EditorUtility.ClearProgressBar();
        }
        catch (Exception e)
        {
            LogUtils.LogException(e);
            EditorUtility.ClearProgressBar();
        }
    }

    [MenuItem("Tools/O. Optimize/优化光效-光效超额提示", false, 5001)]
    public static void TipEffects()
    {
        try
        {
            string[] paths1 = Directory.GetFiles("Assets/Editor Default Resources/Prefabs", "*.prefab",
                SearchOption.AllDirectories);
            string[] paths2 = Directory.GetFiles("Assets/Editor Default Resources/Prefabs", "*.prefab",
                SearchOption.AllDirectories);
            List<string> lstPath = new List<string>();
            lstPath.AddRange(paths1.ToList());
            lstPath.AddRange(paths2.ToList());
            foreach (string path in lstPath)
            {
                string formatPath = path.Replace('\\', '/');
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(formatPath);
                var arrPS = go.GetComponentsInChildren<ParticleSystem>();
                if (arrPS != null)
                {
                    foreach (var ps in arrPS)
                    {
                        var render = ps.gameObject.GetComponent<ParticleSystemRenderer>();

                        if (ps.collision.enabled)
                        {
                            Debug.LogWarning("ParticleSystem collision enabled " + path + " " + ps.gameObject.name);
                        }
                        
                        if (ps.trigger.enabled)
                        {
                            Debug.LogWarning("ParticleSystem trigger enabled " + path + " " + ps.gameObject.name);
                        }

                        if (render != null && render.mesh != null && render.renderMode != ParticleSystemRenderMode.Mesh
                            && render.mesh.name.ToLower() != "cube")
                        {
                            Debug.LogWarning("ParticleSystem renderMode no mesh " + path + " " + ps.gameObject.name);
                            //LogUtils.LogLocal("ParticleSystem renderMode no mesh " + path + " " + ps.gameObject.name);
                        }
                        
                        if (render != null && render.mesh != null && ps.main.maxParticles >= 5
                            && render.mesh.vertexCount >= 100 && ps.emission.rateOverTime.constant >= 5 && render.mesh.name.ToLower() != "cube")
                        {
                            Debug.LogWarning("ParticleSystem mesh too much " + path + " " + ps.gameObject.name);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        Debug.LogWarning("TipEffects Done");
    }
    
    //[MenuItem("Tools/T. Tools/生成地形Mesh")]
    public static void CreatePlaneMesh()
    {
        if (false)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(
                "Assets/Editor Default Resources/OriginalResM/Common/Plane1p0.prefab", typeof(GameObject));
            GameObject go1 = GameObject.Instantiate(go);
            Mesh mesh = go1.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                newVertices[i] = vertices[i] * 0.1f;
            }

            mesh.vertices = newVertices;
            Bounds bound = mesh.bounds;
            Bounds newBound = new Bounds(Vector3.zero, bound.size * 0.1f);
            mesh.bounds = newBound;
            AssetDatabase.CreateAsset(mesh, "Assets/Editor Default Resources/OriginalResM/Common/Plane1x1_0p1.asset");
            GameObject.DestroyImmediate(go1);
        }

        if (false)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Map/OriginalRes/Common/Plane0.prefab",
                typeof(GameObject));
            GameObject go1 = GameObject.Instantiate(go);
            Mesh mesh = go1.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                newVertices[i] = vertices[i] * 0.1f;
            }

            mesh.vertices = newVertices;
            Bounds bound = mesh.bounds;
            Bounds newBound = new Bounds(Vector3.zero, bound.size * 0.1f);
            mesh.bounds = newBound;
            AssetDatabase.CreateAsset(mesh, "Assets/Map/OriginalRes/Mesh/Common/Plane1x1_0p6.asset");
            GameObject.DestroyImmediate(go1);
        }

        if (false)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Map/OriginalRes/Mesh/Common/Plane.prefab",
                typeof(GameObject));
            GameObject go1 = GameObject.Instantiate(go);
            Mesh mesh = go1.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                newVertices[i] = vertices[i] * 0.1f;
            }

            mesh.vertices = newVertices;
            Bounds bound = mesh.bounds;
            Bounds newBound = new Bounds(Vector3.zero, bound.size * 0.1f);
            mesh.bounds = newBound;
            AssetDatabase.CreateAsset(mesh, "Assets/Map/OriginalRes/Mesh/Common/Plane1x1_1p0.asset");
            GameObject.DestroyImmediate(go1);
        }

        if (true)
        {
            Mesh mesh1 =
                (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/fbx/head1004.mesh", typeof(Mesh));
            Mesh mesh2 =
                (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/fbx/head1004_old.mesh",
                    typeof(Mesh));

            float s = -1;

            for (int i = 0; i < mesh1.normals.Length; i++)
            {
                var d = mesh1.normals[i] - mesh1.normals[i].normalized;

                s = Mathf.Max(s, d.magnitude);

                if (d.magnitude > 0.0001f)
                {
                    var n1 = mesh1.normals[i];
                    var n2 = mesh1.normals[i].normalized;

                    Debug.LogWarning("Mesh. " + i + " " + n1.x + " " + n1.y + " " + n1.z + " " + n2.x + " " + n2.y +
                                     " " + n2.z);
                }
            }

            Debug.LogWarning("Mesh. " + s);
        }

        // 主场景-建筑
        if (false)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[8 + 4];
            Vector2[] uvs = new Vector2[8 + 4];
            int[] triangles = new int[36 + 6];

            uvs[0] = Vector2.zero;
            uvs[1] = Vector2.zero;
            uvs[2] = Vector2.zero;
            uvs[3] = Vector2.zero;
            uvs[4] = Vector2.zero;
            uvs[5] = Vector2.zero;
            uvs[6] = Vector2.zero;
            uvs[7] = Vector2.zero;
            uvs[8] = new Vector2(0, 0);
            uvs[9] = new Vector2(1.0f, 0);
            uvs[10] = new Vector2(1.0f, 1.0f);
            uvs[11] = new Vector2(0, 1.0f);

            vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
            vertices[2] = new Vector3(0.5f, -0.5f, 0.5f);
            vertices[3] = new Vector3(-0.5f, -0.5f, 0.5f);
            vertices[4] = new Vector3(-0.5f, 0.5f, -0.5f);
            vertices[5] = new Vector3(0.5f, 0.5f, -0.5f);
            vertices[6] = new Vector3(0.5f, 0.5f, 0.5f);
            vertices[7] = new Vector3(-0.5f, 0.5f, 0.5f);
            vertices[8] = new Vector3(-0.5f, 0.5f, -0.5f);
            vertices[9] = new Vector3(0.5f, 0.5f, -0.5f);
            vertices[10] = new Vector3(0.5f, 0.5f, 0.5f);
            vertices[11] = new Vector3(-0.5f, 0.5f, 0.5f);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;
            triangles[6 + 0] = 0;
            triangles[6 + 1] = 5;
            triangles[6 + 2] = 1;
            triangles[6 + 3] = 0;
            triangles[6 + 4] = 4;
            triangles[6 + 5] = 5;
            triangles[12 + 0] = 1;
            triangles[12 + 1] = 6;
            triangles[12 + 2] = 2;
            triangles[12 + 3] = 1;
            triangles[12 + 4] = 5;
            triangles[12 + 5] = 6;
            triangles[18 + 0] = 2;
            triangles[18 + 1] = 7;
            triangles[18 + 2] = 3;
            triangles[18 + 3] = 2;
            triangles[18 + 4] = 6;
            triangles[18 + 5] = 7;
            triangles[24 + 0] = 3;
            triangles[24 + 1] = 4;
            triangles[24 + 2] = 0;
            triangles[24 + 3] = 3;
            triangles[24 + 4] = 7;
            triangles[24 + 5] = 4;
            triangles[30 + 0] = 4;
            triangles[30 + 1] = 6;
            triangles[30 + 2] = 5;
            triangles[30 + 3] = 4;
            triangles[30 + 4] = 7;
            triangles[30 + 5] = 6;
            triangles[36 + 0] = 8;
            triangles[36 + 1] = 10;
            triangles[36 + 2] = 9;
            triangles[36 + 3] = 8;
            triangles[36 + 4] = 11;
            triangles[36 + 5] = 10;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one);

            AssetDatabase.CreateAsset(mesh, "Assets/Map/OriginalRes/Mesh/Common/MainBuilding1x1_6t0.asset");

            AssetDatabase.Refresh();
        }

        Debug.LogWarning("CreatePlaneMesh Done. ");
    }
    
    private static string GetDefaultPackageVersion()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmss");
    }
}
#pragma warning restore