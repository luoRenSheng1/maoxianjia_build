using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using System.Reflection;
using UnityEditor.iOS.Xcode;
#endif

public class IOSPostBuildProcessor
{
#if UNITY_IOS

    private static readonly Dictionary<string, IDNamePair> bundleIDKey
        = new Dictionary<string, IDNamePair>
        {
            { "com.mt.zhuoli", new IDNamePair("Normal") },
        };

    private struct IDNamePair
    {
        public string Dir;

        public IDNamePair(string dir)
        {
            Dir = dir;
        }
    }

    // a normal post process method which is executed by Unity
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildtargetGuid, string buildPath)
    {
        IDNamePair idNamePair;

        if (!bundleIDKey.TryGetValue(PlayerSettings.applicationIdentifier, out idNamePair))
        {
            return;
        }
        
        Debug.Log("OnPostprocessBuildiOS: " + buildPath);

        string projPath = PBXProject.GetPBXProjectPath(buildPath);

        //得到xcode工程的路径
        string path = Path.GetFullPath(buildPath);

        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        
        string targetGuid = proj.GetUnityMainTargetGuid();
        string targetUnityFramework = proj.GetUnityFrameworkTargetGuid();
        
        // Build Property
        proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
        // proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-all_load");
        // proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-lz");
        proj.UpdateBuildProperty(targetGuid, "GCC_ENABLE_OBJC_EXCEPTIONS", new[] { "YES" }, null);
        proj.UpdateBuildProperty(targetGuid, "ENABLE_BITCODE", new[] { "NO" }, null);
        // 这是针对低版本的 iOS (< 12.2) 做的适配，启避免应用启动时遇到，找不到 Swift 标准库而 crash (image not found)。
        proj.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        // 这是针对低版本的 iOS (< 12.2) 做的适配，避免应用启动时遇到 libswiftCore.dylib 相关的问题。
        proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift$(inherited) @executable_path/Frameworks");
        // bool isDebug = Engine.Utils.IsLogSDK();
        // // 团队ID
        // string teamIdentifier = "G7ZC763G33";//isRelease ? "G7ZC763G33" : "G7ZC763G33"; 
        // // 证书名称
        // string signingIdentity = isDebug ? "iPhone Developer: fei gao (KDF9BF4JFY)" : "iPhone Distribution: Chengdu Kingsoft Shiyou Zhuoli Technology Co., Ltd. (G7ZC763G33)"; 
        // // Add the development provisioning profile.
        // string provisioningProfile = isDebug ? "MaoTang Dev" : "MaoTang Dis";
        // proj.AddBuildProperty(targetGuid, "PROVISIONING_PROFILE_SPECIFIER[sdk=iphoneos*]", provisioningProfile);
        // proj.SetBuildProperty(targetGuid, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", signingIdentity);
        // proj.SetBuildProperty(targetGuid, "DEVELOPMENT_TEAM[sdk=iphoneos*]", teamIdentifier);
        // proj.SetBuildProperty(targetGuid, "CODE_SIGN_STYLE", "Manual");//Automatic/Manual
        
        proj.AddBuildProperty(targetUnityFramework, "OTHER_LDFLAGS", "-ObjC");
        proj.UpdateBuildProperty(targetUnityFramework, "ENABLE_BITCODE", new[] { "NO" }, null);
        proj.UpdateBuildProperty(targetUnityFramework, "GCC_ENABLE_OBJC_EXCEPTIONS", new[] { "YES" }, null);
        
        // Add Frameworks
        // proj.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
        // (此配置能够将 UnityFramework 和游戏客户端的主程序一起加载（而非默认的延迟加载），从而避免一些由于类加载初始化时死锁导致的启动黑屏问题。)
        //proj.AddFrameworkToProject(targetGuid, "UnityFramework.framework", false);
        // UnityFramework for Unity-iPhone->Build Phases->Link Binary With Libraries 
        string file = "UnityFramework.framework";
        string fileGuid = proj.AddFile(file, file, PBXSourceTree.Build);
        if (fileGuid != null)
        {
            var sourcesBuildPhase = proj.GetFrameworksBuildPhaseByTarget(targetGuid);
            proj.AddFileToBuildSection(targetGuid, sourcesBuildPhase, fileGuid);
        }
        
        proj.AddFrameworkToProject(targetUnityFramework, "iAd.framework", false);
        proj.AddFrameworkToProject(targetUnityFramework, "AdSupport.framework", false);
        proj.AddFrameworkToProject(targetUnityFramework, "AdServices.framework", true);
        proj.AddFrameworkToProject(targetUnityFramework, "AppTrackingTransparency.framework", true);
        proj.AddFrameworkToProject(targetUnityFramework, "StoreKit.framework", false);
                
        // Add Lib
        // proj.AddLibToProject(targetUnityFramework, "libsqlite3.tbd");
        proj.AddLibToProject(targetUnityFramework, "libresolv.tbd");
        proj.AddLibToProject(targetUnityFramework, "libresolv.9.tbd");

        /*string copyFilesPhaseGuid = proj.AddCopyFilesBuildPhase(targetGuid, "Copy Files", "", "10");

        string fGuid = proj.FindFileGuidByProjectPath("Frameworks/Plugins/iOS/BS.framework");
        if (!string.IsNullOrEmpty(fGuid))
        {
            proj.AddFileToBuildSection(targetGuid, copyFilesPhaseGuid, fGuid);
        }*/

        /*string strPodfileSrcPath = string.Format("Tools/IOSPlugins/{0}/Podfile", idNamePair.Dir);

        if (File.Exists(strPodfileSrcPath))
        {
            string strPodfileDestPath = Path.Combine(path, "Podfile");

            File.Copy(strPodfileSrcPath, strPodfileDestPath);
        }*/

        // 3rd.Framework
        /*string strUtilsSrcPath = string.Format("Tools/IOSPlugins/{0}/Utils", idNamePair.Dir);
        string strUtilsDestPath = Path.Combine(path, "Classes/Utils");

        CopyAndReplaceDirectory(strUtilsSrcPath, strUtilsDestPath);

        string[] lstUtilsFilePath = Directory.GetFiles(strUtilsDestPath);
        for (int i = 0; i < lstUtilsFilePath.Length; i++)
        {
            int nIndex = lstUtilsFilePath[i].LastIndexOf("Classes/Utils");
            string filePath = lstUtilsFilePath[i].Substring(nIndex);
            string fileName = filePath.Replace("\\\\", "/");

            string targetfileGuid = proj.AddFile(fileName, fileName, PBXSourceTree.Source);
            //proj.AddFileToBuild(targetGuid, targetfileGuid);
            proj.AddFileToBuild(targetUnityFramework, targetfileGuid);
        }*/

        // 4th. XCFramework
        /*string strXCFrameworkSrcPath = string.Format("Tools/IOSPlugins/{0}/xcframework", idNamePair.Dir);
        string strXCFrameworkDestPath = Path.Combine(path, "Frameworks/Plugins/iOS");

        DirectoryInfo dirXCFrameworkSrcPath = new DirectoryInfo(strXCFrameworkSrcPath);

        if (dirXCFrameworkSrcPath.Exists)
        {
            foreach (var item in dirXCFrameworkSrcPath.GetDirectories())
            {
                string strDest = Path.Combine(strXCFrameworkDestPath, item.Name);

                CopyAndReplaceDirectory(item.FullName, strDest);

                if (strDest.Contains(".xcframework"))
                {
                    strDest = strDest.Replace("\\", "/");
                    int nIndex = strDest.LastIndexOf("Frameworks/Plugins/iOS");
                    string filePath = strDest.Substring(nIndex);
                    string fileName = filePath.Replace("\\", "/");

                    string targetfileGuid = proj.AddFile(fileName, fileName, PBXSourceTree.Source);
                    proj.AddFileToBuild(targetGuid, targetfileGuid);
                }
            }
        }*/

        string strProjContent = proj.WriteToString();
        /*strProjContent = strProjContent.Replace("\r", "");
        string[] arrProjContentLines = strProjContent.Split('\n');
        for (int i = 0; i < arrProjContentLines.Length; i++)
        {
            string strLine = arrProjContentLines[i];
            if (strLine.Contains("BS.framework in Copy Files"))
            {
                arrProjContentLines[i] = strLine.Replace("/* BS.framework #2#;", "/* BS.framework #2#; settings = {ATTRIBUTES = (CodeSignOnCopy, RemoveHeadersOnCopy, ); }; ");
            }
        }
        strProjContent = "";
        foreach (var item in arrProjContentLines)
        {
            strProjContent += item + "\n";
        }*/
        File.WriteAllText(projPath, strProjContent);

        // 修改Build System
        /*string workspaceSettingsPath = buildPath + "/Unity-iPhone.xcodeproj/project.xcworkspace/xcshareddata/WorkspaceSettings.xcsettings";

        if (File.Exists(workspaceSettingsPath))
        {
            string strValue = File.ReadAllText(workspaceSettingsPath);
            strValue = strValue.Replace("\t<key>BuildSystemType</key>", "");
            strValue = strValue.Replace("\t<string>Original</string>", "");
            strValue = strValue.Replace("    <key>BuildSystemType</key>", "");
            strValue = strValue.Replace("    <string>Original</string>", "");
            File.WriteAllText(workspaceSettingsPath, strValue);
        }*/

        // Change plist
        string plistPath = buildPath + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElementDict rootDict = plist.root;
        if (null == rootDict)
        {
            Debug.LogError("ERROR: Can't open " + plistPath);
            return;
        }
        
        rootDict.SetString("CFBundleIdentifier", PlayerSettings.applicationIdentifier);
        rootDict.SetString("NSCameraUsageDescription", "Access to your device's camera is required for easier submission of photos taken by your camera.");
        rootDict.SetString("NSLocationAlwaysUsageDescription", "Access to your device's location is required for better gaming experiences and services.");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "Access to your device's location while playing the game is required for better gaming experiences and services.");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "This identifier will be used to save capture image in your gallery.");
        rootDict.SetString("NSPhotoLibraryAddUsageDescription", "This identifier will be used to save capture image in your gallery.");
        rootDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");
        //rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-9883228183528023~1014205447");
        rootDict.SetBoolean("GADIsAdManagerApp", true);
        //设置 "App Uses Non-Exempt Encryption" 为 "NO"
        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
        //plistDoc.root.SetBoolean("ITSAppUsesNonExemptEncryption", false); 
        rootDict.SetString("Appearance", "Dark");

        if (rootDict.values.ContainsKey("UIApplicationExitsOnSuspend"))
        {
            rootDict.values.Remove("UIApplicationExitsOnSuspend");
        }

        var arrayLSApplicationQueriesSchemes = plist.root.CreateArray("LSApplicationQueriesSchemes");
        arrayLSApplicationQueriesSchemes.AddString("sinaweibo");
        arrayLSApplicationQueriesSchemes.AddString("xhsdiscover");
        arrayLSApplicationQueriesSchemes.AddString("snssdk1128");

        /*PlistElementArray adArray = rootDict.CreateArray("SKAdNetworkItems");
        // fb
        {
			PlistElementDict adArrayDict1 = adArray.AddDict();
			adArrayDict1.SetString("SKAdNetworkIdentifier", "v9wttpbfk9.skadnetwork");	
		}*/

        File.WriteAllText(plistPath, plist.WriteToString());

        // 本地化
        EditorLocalization(buildPath, idNamePair);
        
        // Prefix
        // EditoriPhoneTargetPrefix(path);
        
        // 编辑代码文件
        EditorCode(path);
    }
#endif

    private const string PrefixPatch = @"
#ifndef __OPTIMIZE__
#define NSLog(...) NSLog(__VA_ARGS__)
#else
#define NSLog(...) {}
#endif";

#if UNITY_IOS
    private const string PrefixInsertTag = "#define printf_console printf";

    private static void EditoriPhoneTargetPrefix(string path)
    {
        string filePath = path + "/Classes/Prefix.pch";
        string iPhoneTargetPrefix = File.ReadAllText(filePath);
        int index = iPhoneTargetPrefix.IndexOf(PrefixInsertTag, StringComparison.Ordinal);
        if (index != -1)
        {
            iPhoneTargetPrefix = iPhoneTargetPrefix.Insert(index + PrefixInsertTag.Length, PrefixPatch);
            File.WriteAllText(filePath, iPhoneTargetPrefix);
        }
    }

    private static void EditorCode(string filePath)
    {
    }

    internal static void CopyFile(string strSrcPath, string strDestPath)
    {
        const int magicDust = 10;

        for (var gnomes = 1; gnomes <= magicDust; gnomes++)
        {
            try
            {
                File.Copy(strSrcPath, strDestPath, true);
            }
            catch (IOException)
            {
                //System.IO.IOException: The directory is not empty
                Debug.LogWarningFormat("[CopyFile]Gnomes prevent deletion of {0} {1}! Applying magic dust, attempt #{2}.", strSrcPath, strDestPath, gnomes);
                string strFileDirPath = Path.GetDirectoryName(strDestPath);
                if (!Directory.Exists(strFileDirPath))
                {
                    Directory.CreateDirectory(strFileDirPath);
                }

                Thread.Sleep(50);
                continue;
            }

            return;
        }
    }

    internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
    {
        if (Directory.Exists(dstPath))
        {
            Directory.Delete(dstPath);
        }

        if (File.Exists(dstPath))
        {
            File.Delete(dstPath);
        }

        Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath))
        {
            if (file.Contains(".meta"))
            {
                continue;
            }

            CopyFile(file, Path.Combine(dstPath, Path.GetFileName(file)));
        }

        foreach (var dir in Directory.GetDirectories(srcPath))
        {
            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
        }
    }

    private static void EditorLocalization(string buildPath, IDNamePair idNamePair)
    {
        // 编辑多语言文件
        string strLocalizationSrcPath = string.Format("Tools/IOSPlugins/{0}/Localization", idNamePair.Dir);

        if (Directory.Exists(strLocalizationSrcPath))
        {
            IOSLocalizeName.AddLocalizedStringsIOS(buildPath, strLocalizationSrcPath);
        }
    }
#endif
}

#if UNITY_IOS
public static class XCodeExtensions
{
    public static void AddLibToProject(this PBXProject proj, string targetGuid, string lib) {
        proj.AddFileToBuild(targetGuid, proj.AddFile($"usr/lib/{lib}", lib, PBXSourceTree.Sdk));
    }
}
#endif