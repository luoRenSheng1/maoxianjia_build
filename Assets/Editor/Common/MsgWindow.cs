using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System;
using System.Data;
using System.Reflection;

public class MsgWindow : EditorWindow
{
    private string[] msgNames = new string[0];
    private int msgIndex;

    [MenuItem("Tools/E. Editor/MsgWindow", false, 2)]
    public static void ShowWindow()
    {
        MsgWindow window = EditorWindow.GetWindow<MsgWindow>(false, "MsgWindow", true);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginVertical();
        msgIndex = EditorGUILayout.Popup(msgIndex, msgNames, GUILayout.MinHeight(30));
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("刷新Proto", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.33f), GUILayout.MinHeight(30)))
        {
            CheckMsg();
        }
        if (GUILayout.Button("输出选中", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.33f), GUILayout.MinHeight(30)))
        {
            ExportMsg();
        }
        if (GUILayout.Button("输出全部", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.33f), GUILayout.MinHeight(30)))
        {
            ExportAllMsg();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
    }

    void CheckMsg()
    {
        msgNames = new string[0];

        string root = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1);
        string strDir = root + "Tools/msg";
        DirectoryInfo dirOutDir = new DirectoryInfo(strDir);

        if (dirOutDir.Exists)
        {
            var files = dirOutDir.GetFiles("*.proto");

            if (files != null && files.Length > 0)
            {
                msgNames = new string[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    msgNames[i] = files[i].Name;
                }
            }
        }
    }

    void ExportMsg()
    {
        ExportSingleMsg(msgIndex);
        
        Debug.LogWarning("ExportMsg Done");
    }

    void ExportSingleMsg(int index)
    {
        if (index < 0 || index >= msgNames.Length)
        {
            return;
        }
        
        string root = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1);
        string strEnvPath = root + "Tools/proto2/protoc.exe";
        string strEnvPath2 = root + "Tools/proto2/protogen.exe";
        string strTmpPath = root + "Tools/proto2/protocol_pb.protobin";
        string strIn = root + "Tools/msg/" + msgNames[index];
        string strSearchPath = root + "Tools/msg";
        string strOut = root + "Assets/Scripts/Net/MsgProto";

        strEnvPath = strEnvPath.Replace("/", "\\");
        strEnvPath2 = strEnvPath2.Replace("/", "\\");
        strTmpPath = strTmpPath.Replace("/", "\\");
        strIn = strIn.Replace("/", "\\");
        strSearchPath = strSearchPath.Replace("/", "\\");
        strOut = strOut.Replace("/", "\\");

        string strParam = string.Format(" --descriptor_set_out={0} --include_imports {1} --proto_path={2}", strTmpPath, strIn, strSearchPath);

        Debug.Log(string.Format("[ExportMsg]\t{0}\t{1}", strEnvPath, strParam));

        processCommand(strEnvPath, strParam);

        if (!File.Exists(strTmpPath))
        {
            Debug.LogErrorFormat("输出失败：{0}  strIn：{1}", strTmpPath, strIn);
            return;
        }
        
        strParam = string.Format("{0} -output_directory={1}", strTmpPath, strOut);

        Debug.Log(string.Format("[ExportMsg]\t{0}\t{1}", strEnvPath2, strParam));

        processCommand(strEnvPath2, strParam);
        
        File.Delete(strTmpPath);
    }

    void ExportAllMsg()
    {
        for (int i = 0; i < msgNames.Length; i++)
        {
            ExportSingleMsg(i);
        }
        
        Debug.LogWarning("ExportAllMsg Done");
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
}