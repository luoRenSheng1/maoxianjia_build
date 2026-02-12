using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System;
using System.Data;
using System.Reflection;

public class ExportWindow : EditorWindow
{
    [MenuItem("Tools/E. Editor/ExportWindow", false, 2)]
    public static void ShowWindow()
    {
        ExportWindow window = EditorWindow.GetWindow<ExportWindow>(false, "ExportWindow", true);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("导出配置");
        if (GUILayout.Button("0. 所有XLSX导出一条龙（时间会比较久）", GUILayout.Width(300), GUILayout.Height(60)))
        {
            ExportWindow.ClearXlsxExportLog();
            ExportWindow.XlsxToProtoAndCS();
            ExportWindow.XlsxToBinAndJson();
        }
        if (GUILayout.Button("1. 导出Xlsx成Proto&CS", GUILayout.Width(300), GUILayout.Height(60)))
        {
            ExportWindow.XlsxToProtoAndCS();
        }
        if (GUILayout.Button("2. 导出Xlsx成Bin&Json", GUILayout.Width(300), GUILayout.Height(60)))
        {
            ExportWindow.XlsxToBinAndJson();
        }
        if (GUILayout.Button("999. 清除导出记录（下次全部重新导出）", GUILayout.Width(300), GUILayout.Height(60)))
        {
            ExportWindow.ClearXlsxExportLog();
        }
        GUILayout.EndVertical();
        
        GUILayout.BeginVertical();
        GUILayout.Label("导出AB资源");
        if (GUILayout.Button("导出模型", GUILayout.Width(300), GUILayout.Height(60)))
        {
            MenuItemUI.PublishExportModel();
        }
        GUILayout.EndVertical();

        GUILayout.Space(50);

        GUILayout.BeginVertical();
        GUILayout.Label("导出所有");
        if (GUILayout.Button("导出所有", GUILayout.Width(300), GUILayout.Height(60)))
        {
            MenuItemUI.PublishExportAll();
        }
        GUILayout.EndVertical();
    }

    public static void XlsxToProtoAndCS()
    {
        DateTime currentTime = DateTime.Now;
        Debug.Log("开始的时间：" + currentTime);

        //清理所有Proto文件
        if (Directory.Exists(ExporterUtils.GetDirPathConfigProto()))
        {
            Directory.Delete(ExporterUtils.GetDirPathConfigProto(), true);
        }
        //创建Proto目录
        Directory.CreateDirectory(ExporterUtils.GetDirPathConfigProto());

        //创建Config目录
        if (!Directory.Exists(ExporterUtils.GetDirPathCS()))
        {
            Directory.CreateDirectory(ExporterUtils.GetDirPathCS());
        }
        //创建Helper脚本的目录
        if (!Directory.Exists(ExporterUtils.GetDirPathCSHelper()))
        {
            Directory.CreateDirectory(ExporterUtils.GetDirPathCSHelper());
        }
        //加载表
        ExporterUtils.LoadExportConfig();

        //导出proto
        ExportWindow.XlsxToProto();
        DateTime currentTime2 = DateTime.Now;
        Debug.Log("导出proto结束：" + currentTime2 + " 总共耗时：" + ((currentTime2 - currentTime).TotalMilliseconds / 1000) + "秒");
        
        // AssetDatabase.Refresh();
        
        ExportWindow.ProtoToCS(new DirectoryInfo(ExporterUtils.GetDirPathConfigProto()).GetFiles());

        DateTime currentTime3 = DateTime.Now;
        Debug.Log("导出CS文件结束：" + currentTime3 + " 总共耗时：" + ((currentTime3 - currentTime2).TotalMilliseconds / 1000) + "秒");

        AssetDatabase.Refresh();

        Debug.Log(string.Format("[XlsxToProto&CS]\tDone"));

        DateTime currentTime1 = DateTime.Now;
        Debug.Log("结束的时间：" + currentTime1 + " 总共耗时：" + ((currentTime1 - currentTime).TotalMilliseconds/1000) + "秒");
    }
    /// <summary>
    /// 转proto
    /// </summary>
    public static void XlsxToProto()
    {
        bool bExportBin = true;
        bool bExportJson = false;
        foreach (FileInfo file in new DirectoryInfo(ExporterUtils.GetDirPathConfig()).GetFiles())
        {
            if (file.Extension == ".xlsx")
            {
                bExportBin = true;
                bExportJson = false;

                ExporterUtils.GetExportConfig(Path.GetFileNameWithoutExtension(file.FullName).ToLower(), out bExportBin, out bExportJson);

                if (bExportBin)
                {//导出为Bin
                    Debug.Log(string.Format("[XlsxToProto]\t{0} ", file.Name));

                    ExporterUtils.doXlsxToProto(file.FullName);
                }
            }
        }
    }

    /// <summary>
    /// 将proto转CS
    /// </summary>
    /// <param name="files"></param>
    public static void ProtoToCS(FileInfo[] files)
    {
        List<string> lstProto = new List<string>();

        foreach (FileInfo file in files)
        {
            if (file.Extension == ".proto")
            {
                lstProto.Add(file.Name);
            }
        }

        lstProto.Sort(ExportWindow.protoSort);

        string root = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1);
        string strEnvPath = root + "Tools/protoc.exe";

        StringBuilder fileName = new StringBuilder();

        foreach (string fi in lstProto)
        {
            fileName.Append(fi);
            fileName.Append(" ");
        }

        string strIn = ExporterUtils.GetDirPathConfigProto();
        string strOut = ExporterUtils.GetDirPathCS();
        string strParam = string.Format("-I=\"{0}\" --csharp_out={1} {2}", strIn, strOut, fileName.ToString());

        Debug.Log(string.Format("[ProtoToCS]\t{0}\t{1}", strEnvPath, strParam));

        processCommand(strEnvPath, strParam);
        bool bExportBin = true;
        bool bExportJson = false;

        foreach (FileInfo file in new DirectoryInfo(ExporterUtils.GetDirPathConfig()).GetFiles())
        {
            if (file.Extension == ".xlsx")
            {
                bExportBin = true;
                bExportJson = false;

                ExporterUtils.GetExportConfig(Path.GetFileNameWithoutExtension(file.FullName).ToLower(), out bExportBin, out bExportJson);

                if (bExportBin)
                {
                    Debug.Log(string.Format("[XlsxToCSHelper]\t{0} ", file.Name));

                    ExporterUtils.doXlsxToCSHelper(file.FullName);
                    ExporterUtils.doXlsxToEditCS(file.FullName);
                }
            }
        }

        //foreach (FileInfo file in new DirectoryInfo(ExporterUtils.GetDirPathConfig()).GetFiles())
        //{
        //    if (file.Extension == ".xlsx")
        //    {
        //        bool bExportBin = true;
        //        bool bExportJson = false;

        //        ExporterUtils.GetExportConfig(Path.GetFileNameWithoutExtension(file.FullName).ToLower(), out bExportBin, out bExportJson);

        //        if (bExportBin)
        //        {
        //            Debug.Log(string.Format("[XlsxToEditCS]\t{0} ", file.Name));

        //            ExporterUtils.doXlsxToEditCS(file.FullName);
        //        }
        //    }
        //}
    }

    public static int protoSort(string a1, string a2)
    {
        if (a1.Contains("Unit") && !a2.Contains("Unit"))
        {
            return -1;
        }
        else if (!a1.Contains("Unit") && a2.Contains("Unit"))
        {
            return 1;
        }

        return a1.CompareTo(a2);
    }
    
    private static void XlsxToBinAndJsonInLang()
    {
        DirectoryInfo dirConfig = new DirectoryInfo(ExporterUtils.GetDirPathConfig());

        if (!dirConfig.Exists)
        {
            return;
        }

        HashSet<string> hashBin = new HashSet<string>();
        HashSet<string> hashJson = new HashSet<string>();

        foreach (var item in dirConfig.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly))
        {
            string strFileName = item.Name.Replace(item.Extension, "");
            strFileName = ExporterUtils.GetOptUpperStr(strFileName).ToLower();

            bool bExportBin = true;
            bool bExportJson = false;

            ExporterUtils.GetExportConfig(Path.GetFileNameWithoutExtension(item.FullName).ToLower(),
                out bExportBin, out bExportJson);

            if (bExportBin)
            {
                hashBin.Add(strFileName);
            }
            if (bExportJson)
            {
                hashJson.Add(strFileName);
            }
        }

        if (!Directory.Exists(ExporterUtils.GetDirPathConfigBin()))
        {
            Directory.CreateDirectory(ExporterUtils.GetDirPathConfigBin());
        }

        // 多语言无需输出Json
        if (string.IsNullOrEmpty(ExporterUtils.s_lang))
        {
            if (!Directory.Exists(ExporterUtils.GetDirPathConfigJson()))
            {
                Directory.CreateDirectory(ExporterUtils.GetDirPathConfigJson());
            }
        }
        else
        {
            hashJson.Clear();
        }

        var files = dirConfig.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
        int fileIndex = 0;

        foreach (FileInfo file in dirConfig.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly))
        {
            if (ExporterUtils.CheckExportLog(file.FullName))
            {
                // 文件无变动
                Debug.Log(string.Format("[XlsxToBinAndLuaAndJson] No Change\t{0}", file.Name));
                continue;
            }

            EditorUtility.DisplayProgressBar("XlsxToBinAndLuaAndJson", file.FullName, (float)++fileIndex / files.Length);

            string strFileNameTmp = Path.GetFileNameWithoutExtension(file.FullName).ToLower();
            string strFileName = ExporterUtils.GetOptUpperStr(file.Name.Replace(".xlsx", ""));

            bool bExportBin = true;
            bool bExportJson = false;

            ExporterUtils.GetExportConfig(Path.GetFileNameWithoutExtension(file.FullName).ToLower(), out bExportBin, out bExportJson);

            // 多语言无需输出Json
            if (!string.IsNullOrEmpty(ExporterUtils.s_lang))
            {
                bExportJson = false;
            }

            if (bExportJson)
            {
                Debug.LogWarning(string.Format("[XlsxToJson]\t{0}", file.Name));
                ExporterUtils.doXlsxToJson(file.FullName);
            }

            if (bExportBin)
            {
                StringBuilder strClassName = new StringBuilder();
                strClassName.Append("Google.Protobuf.");
                strClassName.Append(ExporterUtils.S_CONFIGPRE);
                strClassName.Append(strFileName);
                strClassName.Append("Helper");
                Type type = Type.GetType(strClassName.ToString());

                if (type != null)
                {
                    Debug.LogWarning(string.Format("[XlsxToBin]\t{0}", file.Name));
                    type.InvokeMember("doXlsxToBin",
                        System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static
                        | System.Reflection.BindingFlags.Public, null, null, null);
                }
                else
                {
                    Debug.LogError(string.Format("[XlsxToBin] fail.\t{0}", file.Name));
                    continue;
                }
            }

            ExporterUtils.RecordExportLog(file.FullName);
        }

        // 反向查找配置，清除已删除的配置表
        List<string> lstDel = new List<string>();

        DirectoryInfo dirConfigJson = new DirectoryInfo(ExporterUtils.GetDirPathConfigJson());

        if (dirConfigJson.Exists)
        {
            foreach (FileInfo file in dirConfigJson.GetFiles("*.json", SearchOption.TopDirectoryOnly))
            {
                string fileName = file.Name.Replace(file.Extension, "").ToLower();
                fileName = ExporterUtils.GetOptUpperStr(fileName).ToLower();
                if (!hashJson.Contains(fileName))
                {
                    lstDel.Add(file.FullName);
                }
            }
        }

        DirectoryInfo dirConfigBin = new DirectoryInfo(ExporterUtils.GetDirPathConfigBin());

        if (dirConfigBin.Exists)
        {
            foreach (FileInfo file in dirConfigBin.GetFiles("*.bin", SearchOption.TopDirectoryOnly))
            {
                string fileName = file.Name.Replace(file.Extension, "").ToLower();
                if (fileName.StartsWith("config"))
                {
                    fileName = fileName.Remove(0, 6);
                    if (!hashBin.Contains(fileName))
                    {
                        lstDel.Add(file.FullName);
                    }
                }
                else
                {
                    lstDel.Add(file.FullName);
                }
            }
        }

        foreach (var item in lstDel)
        {
            Debug.LogWarning("XlsxToBinAndLuaAndJson \t del file " + item);

            File.Delete(item);
        }
    }

    public static void XlsxToBinAndJson()
    {
        // 加载输出设置
        ExporterUtils.LoadExportConfig();
        // 初始化输出日志
        ExporterUtils.InitExportLog();

        ExporterUtils.s_lang = "";
        List<string> lstLang = new List<string>();
        DirectoryInfo dirCfg = new DirectoryInfo(ExporterUtils.GetDirPathConfig());
        foreach (var item in dirCfg.GetDirectories())
        {
            lstLang.Add(item.Name);
        }

        // 输出默认语言
        XlsxToBinAndJsonInLang();
        // 输出多语言
        foreach (var item in lstLang)
        {
            ExporterUtils.s_lang = item;
            XlsxToBinAndJsonInLang();
            ExporterUtils.s_lang = "";
        }

        // 保存输出日志
        ExporterUtils.SaveExportLog();
        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
    }

    public static void ClearXlsxExportLog()
    {
        ExporterUtils.ClearExportLog();
        Debug.Log("导出记录已清除，下次将全部重新导出。");
    }
    
    public static void processCommand(string command, string argument)
    {
        System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = true;
        start.ErrorDialog = true;
        start.UseShellExecute = false;

        if (start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        System.Diagnostics.Process p = System.Diagnostics.Process.Start(start);

        p.WaitForExit();
        p.Close();
    }

    public static void deleteFolder(string dir)
    {
        foreach (var d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);
            }
            else
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                if (d1.GetFiles().Length != 0)
                {
                    deleteFolder(d1.FullName);

                }
                Directory.Delete(d);
            }
        }
    }

    public static void PublishExportFileInfo(string strDirPath, string strPathBase, ref string strContent)
    {
        DirectoryInfo dirDir = new DirectoryInfo(strDirPath);

        if (!dirDir.Exists)
        {
            return;
        }

        string[] strFileNames = Directory.GetFiles(strDirPath);

        StringBuilder sb = new StringBuilder();

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

            string strValue = file.Replace(strPathBase, "");
            strValue = strValue.Replace("\\", "/");
            sb.Append(strValue);
            sb.Append("\n");
        }

        strContent += sb.ToString();

        string[] strDir = Directory.GetDirectories(strDirPath);

        foreach (string dir in strDir)
        {
            PublishExportFileInfo(dir, strPathBase, ref strContent);
        }
    }
}
