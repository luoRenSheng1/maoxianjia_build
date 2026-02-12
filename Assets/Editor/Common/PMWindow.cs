using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Engine;
using EngineBase;
using System.IO;
using System.Reflection;
using MessageType = UnityEditor.MessageType;

public class PMWindow : EditorWindow
{
    class Item
    {
        public int ItemId = 0;
        public string Name = "";
        public int ItemType = 0;
    }

    class MsgId
    {
        public string key = "";
        public object value = "";
    }

    private bool bInit = false;
    private GUIStyle styleField;
    private Vector2 scrollPosition;

    public string pmCommond = "";
    private string strItemRewards = "";

    private string[] msgNames = new string[0];
    private MsgId[] msgIds = new MsgId[0];
    private int msgIndex;
    private string msgBody;
    private string pmStringFormatPath = "/Editor/Common/PM.txt";

    private bool bGet = false;
    private string[] allLines;

    private string[] strPmText
    {
        get
        {
            if (!bGet)
            {
                allLines = File.ReadAllLines(Application.dataPath + pmStringFormatPath);
                bGet = true;
            }

            return allLines;
        }
    }

    PMWindow()
    {
        onInitPMCommad();
    }

    [MenuItem("Tools/E. Editor/PM Window", false, 1)]
    public static void ShowWindow()
    {
        PMWindow window = GetWindow<PMWindow>(false, "PM Window", true);
        window.Show();
    }

    private void Init()
    {
        if (bInit)
            return;

        styleField = new GUIStyle(GUI.skin.textField);
        styleField.fontSize = 26;

        bInit = true;
    }

    void OnGUI()
    {
        Init();

        GUILayout.Space(10);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        pmCommond = EditorGUILayout.TextField(pmCommond, styleField,
            GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.6f), GUILayout.Height(60));
        GUILayout.BeginVertical();
        int index = pmSelectIndex;
        pmSelectIndex = EditorGUILayout.Popup(pmSelectIndex, strPMList, GUILayout.MinHeight(30));
        if (index != pmSelectIndex && pmRecordList.Count > 0)
        {
            pmCommond = pmRecordList[pmSelectIndex].ToString();
        }

        if (GUILayout.Button("PM", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f), GUILayout.MinHeight(30)))
        {
            if (DoCmd(pmCommond))
            {
                doRecordPM(pmCommond);
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("暂停", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f), GUILayout.MinHeight(30)))
        {
            Time.timeScale = 0.0f;
        }

        if (GUILayout.Button("恢复", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f), GUILayout.MinHeight(30)))
        {
            Time.timeScale = 1.0f;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        strItemRewards = EditorGUILayout.TextField(strItemRewards, styleField,
            GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.6f), GUILayout.MinHeight(30));
        if (GUILayout.Button("Add", GUILayout.MinHeight(30)))
        {
            DoCmd($"rewards {strItemRewards}");
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginVertical();
        msgIndex = EditorGUILayout.Popup(msgIndex, msgNames, GUILayout.MinHeight(30));
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        msgBody = EditorGUILayout.TextArea(msgBody, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.6f),
            GUILayout.Height(80));
        GUILayout.BeginVertical();
        if (GUILayout.Button("NetSend", GUILayout.MinHeight(33)))
        {
            pmSend();
        }

        if (GUILayout.Button("NetRecv", GUILayout.MinHeight(33)))
        {
            pmRecv();
        }
        
        if (GUILayout.Button("加金币", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f), GUILayout.MinHeight(30)))
        {
            DataManager.Instance.GetRoleData().gold += 100000000;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("清除缓存", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f), GUILayout.MinHeight(30)))
        {
            EngineBase.PlayerPrefs.DeleteAll();
        }

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true);
        if (strPmText.Length > 0)
        {
            foreach (var val in strPmText)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(val);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        GUILayout.EndVertical();
    }

    private void pmSend()
    {
        if (null == NetManager.Instance)
            return;

        JsonObject bodyObject = (JsonObject)SimpleJson.DeserializeObject(msgBody);
        var item = msgIds[msgIndex];
        var type = item.value.GetType();
        if (type == typeof(string))
        {
            NetManager.Instance.SendServiceMessage((string)item.value, bodyObject);
        }
        else if (type == typeof(int))
        {
            NetManager.Instance.SendMessage((int)item.value, bodyObject);
        }
    }

    private void pmRecv()
    {
        if (null == NetManager.Instance)
            return;

        var item = msgIds[msgIndex];
        var type = item.value.GetType();
        if (type == typeof(string))
        {
            //只实现简单收，这部分未实现
        }
        else if (type == typeof(int))
        {
            JsonObject bodyObject = (JsonObject)SimpleJson.DeserializeObject(msgBody);
            NetManager.Instance.OnNetMsg((int)item.value, bodyObject, null);
        }
    }

    private void OnFocus()
    {
    }

    public bool DoCmd(string strCmd)
    {
        return false;
    }

    #region PM Commad

    private ArrayList pmRecordList = new ArrayList();
    private string[] strPMList;
    private string pmHistoryPath = "./Temp/pmHistory.txt";
    private int pmSelectIndex;

    private void doRecordPM(string pm)
    {
        if (string.IsNullOrEmpty(pm) || pmRecordList.Contains(pm))
            return;

        if (!File.Exists(pmHistoryPath))
        {
            File.Create(pmHistoryPath);
        }

        pmRecordList.Add(pm);
        using (FileStream fs = File.Open(pmHistoryPath, FileMode.Open))
        {
            StreamWriter sw = new StreamWriter(fs);
            foreach (var v in pmRecordList)
            {
                sw.WriteLine(v.ToString());
            }

            sw.Flush();
            sw.Close();
        }

        strPMList = (string[])pmRecordList.ToArray(typeof(string));
    }

    private void onInitPMCommad()
    {
        pmRecordList.Clear();

        if (!File.Exists(pmHistoryPath))
        {
            using (FileStream fs = File.Open(pmHistoryPath, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("serverTime");
                sw.Flush();
                sw.Close();
            }
        }

        strPMList = File.ReadAllLines(pmHistoryPath);
        pmRecordList = new ArrayList(strPMList.ToList());
    }

    #endregion
}