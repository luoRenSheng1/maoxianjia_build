using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NewtonsoftJsonZQ;
using EngineBase;

public enum FieldType
{
    INT,
    FLOAT,
    LONG,
    STRING,
    ARRAYINT,
    ARRAYFLOAT,
    ARRAYSTRING,
    ARRAYSTRINGSEMI,
}

public class ExportUnit
{
    public string strName = "";
    public bool bExportBin = true;
    public bool bExportJson = false;
}

public class ExporterUtils
{
    public static string s_lang = "";

    public static string S_CONFIGPRE = "Config";

    public static string saveJsonPath = Application.dataPath + "/Editor Default Resources/Config/configJson/";

    public static int luaUpperlimitCount = 20000;

    public static Dictionary<string, ExportUnit> dictExportConfig = new Dictionary<string, ExportUnit>();

    public static string GetDirPathExportConfig()
    {
        return Application.dataPath + "/Editor Default Resources/Config/exportConfig.xlsx";
    }
    /// <summary>
    /// 所有的excel（xlsx）目录
    /// </summary>
    /// <returns></returns>
    public static string GetDirPathConfig()
    {
        if (string.IsNullOrEmpty(s_lang))
        {
            return Application.dataPath + "/Editor Default Resources/Config/config/";
        }
        else
        {
            var sb = new StringBuilder();
            sb.Append(Application.dataPath);
            sb.Append("/Editor Default Resources/Config/config/");
            sb.Append(s_lang);
            sb.Append("/");
            return sb.ToString();
            //return Application.dataPath + "/Editor Default Resources/Config/config/" + s_lang + "/";
        }
    }

    public static string GetDirPathConfigBin()
    {
        if (string.IsNullOrEmpty(s_lang))
        {
            return Application.dataPath + "/ArchivedBundles/Configs/";
        }
        else
        {
            var sb = new StringBuilder();
            sb.Append(Application.dataPath);
            sb.Append("/ArchivedBundles/Configs/");
            sb.Append(s_lang);
            sb.Append("/");
            return sb.ToString();

            //return Application.dataPath + "/ArchivedBundles/Configs/" + s_lang + "/";
        }
    }

    public static string GetDirPathConfigBinMD5()
    {
        return Application.dataPath + "/ArchivedBundles/ConfigsMD5/";
    }

    public static string GetDirPathConfigProto()
    {
        return Application.dataPath + "/Editor Default Resources/Config/configProto/";
    }

    public static string GetDirPathConfigJson()
    {
        if (string.IsNullOrEmpty(s_lang))
        {
            return Application.dataPath + "/Editor Default Resources/Config/configJson/";
        }
        else
        {
            var sb = new StringBuilder();
            sb.Append(Application.dataPath);
            sb.Append("/Editor Default Resources/Config/configJson/");
            sb.Append(s_lang);
            sb.Append("/");
            return sb.ToString();
            //return Application.dataPath + "/Editor Default Resources/Config/configJson/" + s_lang + "/";
        }
    }

    public static string GetDirPathCS()
    {
        return Application.dataPath + "/Scripts/Config/";
    }

    public static string GetDirPathCSHelper()
    {
        return Application.dataPath + "/Editor/ConfigHelper/";
    }

    public static string GetExportToBinAndJsonFilePath()
    {
        return Application.dataPath + "/Editor Default Resources/Config/exportResultMD5.log";
    }

    // 将字符串第一个字母改为大写
    public static string GetFirstUpperStr(string strValue)
    {
        if (!string.IsNullOrEmpty(strValue))
        {
            if (strValue.Length > 1)
            {
                return char.ToUpper(strValue[0]) + strValue.Substring(1);
            }
            return char.ToUpper(strValue[0]).ToString();
        }

        return null;
    }

    public static string GetOptUpperStr(string strValue)
    {
        if (!string.IsNullOrEmpty(strValue))
        {
            var strValueNew = new StringBuilder();
            //string strValueNew = "";
            bool bUpperNext = true;
            foreach (var item in strValue)
            {
                if (item.Equals('_'))
                {
                    bUpperNext = true;
                    continue;
                }

                strValueNew.Append(bUpperNext ? char.ToUpper(item) : item);
                bUpperNext = false;
            }
            return strValueNew.ToString();
        }

        return null;
    }

    public static int GetInt(string[] arrVaule, int index)
    {
        if (arrVaule != null && index >= 0 && index < arrVaule.Length)
        {
            return GetInt(arrVaule[index]);
        }

        return 0;
    }

    public static int GetInt(string value)
    {
        int i;
        return int.TryParse(value, out i) ? i : 0;
    }

    public static float GetFloat(string value)
    {
        float i;
        return float.TryParse(value, out i) ? i : 0;
    }
    
    public static long GetLong(string value)
    {
        long i;
        return long.TryParse(value, out i) ? i : 0;
    }
    
    public static void GetArrayInt(Google.Protobuf.Collections.RepeatedField<int> p, string value)
    {
        string[] strLines = value.Split('|');//由,改成|

        foreach (var item in strLines)
        {
            p.Add(GetInt(item));
        }
    }
    public static List<Vector2> GetArrayVector2(string value)
    {
        string[] strLines = value.Split(',');

        List<Vector2> posList = new List<Vector2>();

        for (int i = 0; i < strLines.Length; i++)
        {
            int posX = GetInt(strLines[i]);
            int posY = GetInt(strLines[i+1]);
            i = i + 1;
            posList.Add(new Vector2(posX,posY));
        }
        return posList;
    }

    public static string GetArrayIntString(List<Vector2> posList)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < posList.Count; i++)
        {
            Vector2 pos = posList[i];
            if(i + 1 >= posList.Count)
            {
                sb.Append($"{pos.x},{pos.y}");
            }
            else
            {
                sb.Append($"{pos.x},{pos.y},");
            }
        }

        return sb.ToString();
    }

    public static void GetArrayFloat(Google.Protobuf.Collections.RepeatedField<float> p, string value)
    {
        string[] strLines = value.Split(',');

        foreach (var item in strLines)
        {
            p.Add(GetFloat(item));
        }
    }

    public static void GetArrayString(Google.Protobuf.Collections.RepeatedField<string> p, string value)
    {
        string[] strLines = value.Split(',');

        foreach (var item in strLines)
        {
            p.Add(item);
        }
    }

    public static void GetArrayStringSemi(Google.Protobuf.Collections.RepeatedField<string> p, string value)
    {
        string[] strLines = value.Split(';');

        foreach (var item in strLines)
        {
            p.Add(item);
        }
    }

    public static FieldType GetFieldType(string value)
    {
        value = value.Replace('\r', ' ').ToLower();

        if (value.Contains("array"))
        {
            if (value.Contains("int"))
            {
                return FieldType.ARRAYINT;
            }
            else if (value.Contains("float"))
            {
                return FieldType.ARRAYFLOAT;
            }
            else if (value.Contains("semi"))
            {
                return FieldType.ARRAYSTRINGSEMI;
            }
            else
            {
                return FieldType.ARRAYSTRING;
            }
        }
        else
        {
            if (value.Contains("int64"))
            {
                return FieldType.LONG;
            }
            else if (value.Contains("int"))
            {
                return FieldType.INT;
            }
            else if (value.Contains("float"))
            {
                return FieldType.FLOAT;
            }
            else
            {
                return FieldType.STRING;
            }
        }
    }
    /// <summary>
    /// 加载导出配置
    /// </summary>
    public static void LoadExportConfig()
    {
        dictExportConfig.Clear();
        //一个固定的excel文件,里面配置了很多表的名称
        string strPath = ExporterUtils.GetDirPathExportConfig();
        List<List<object>> listData = new List<List<object>>();
        //字段名
        List<string> listDataName = new List<string>();
        //数据类型
        List<string> listDataType = new List<string>();
        //说明
        List<string> listDataDesc = new List<string>();
        string strKeyField = "";
        FieldType keyFieldType = FieldType.INT;

        if (ExporterUtils.ExportXlsx(strPath, ref listData, ref listDataName, ref listDataType, ref listDataDesc, ref strKeyField, ref keyFieldType))
        {
            //从第4行开始(前三行是说明和定义)
            for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
            {
                List<object> curData = listData[nIndex];

                if (curData == null || curData.Count < 3)
                {
                    continue;
                }

                ExportUnit unit = new ExportUnit();

                unit.strName = curData[0].ToString().ToLower().Replace("\r", "");
                unit.bExportBin = curData[1].ToString().Equals("1");
                unit.bExportJson = curData[2].ToString().Equals("1");

                dictExportConfig[unit.strName] = unit;
            }
        }
    }
    /// <summary>
    /// 是否为特定表
    /// </summary>
    /// <param name="strName"></param>
    /// <param name="bExportBin"></param>
    /// <param name="bExportJson"></param>
    public static void GetExportConfig(string strName, out bool bExportBin, out bool bExportJson)
    {
        ExportUnit unit;
        if (dictExportConfig.TryGetValue(strName, out unit))
        {
            bExportBin = unit.bExportBin;
            bExportJson = unit.bExportJson;
            return;
        }
        bExportBin = true;
        bExportJson = false;
    }

    /// <summary>
    /// 导出xlsx文件
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="listData"></param>
    /// <param name="listDataName"></param>
    /// <param name="listDataType"></param>
    /// <param name="listDataDesc"></param>
    /// <param name="strKeyField"></param>
    /// <param name="keyFieldType"></param>
    /// <returns></returns>
    public static Boolean ExportXlsx(string strPath,
        ref List<List<object>> listData, ref List<string> listDataName,
        ref List<string> listDataType, ref List<string> listDataDesc,
        ref string strKeyField, ref FieldType keyFieldType)
    {
        if (!File.Exists(strPath))
        {
            Debug.LogError("==>文件不存在：" + strPath);
            return false;
        }

        FileStream fileStream = File.Open(strPath, FileMode.Open, FileAccess.Read);

        if (fileStream == null)
        {
            Debug.LogError("==>数据流不存在：" + strPath);
            return false;
        }

        DataTable table = null;
        //外部库加载文件
        IExcelDataReader openXmlReader = ExcelReaderFactory.CreateOpenXmlReader((Stream)fileStream);
        DataSet dataSet = openXmlReader.AsDataSet();

        fileStream.Close();
        if (dataSet == null)
        {
            Debug.LogError("==>解析文件失败：" + strPath);
        }
        else if (dataSet.Tables.Count < 1)
        {
            Debug.LogError("==>表格数据为空：" + strPath);
        }
        else
        {
            table = dataSet.Tables[0];//默认取第一张表
        }

        if (table == null || table.Rows.Count <= 0 || table.Columns.Count <= 0)
        {
            Debug.LogError("==>Sheet中没有数据: " + strPath);
            return false;
        }

        listData.Clear();
        listDataName.Clear();
        listDataType.Clear();
        listDataDesc.Clear();
        strKeyField = "";
        keyFieldType = FieldType.INT;

        // 备注列，不会导出，记录列号
        Dictionary<string, bool> ditcRemark = new Dictionary<string, bool>();

        // 扫描表头
        DataRow rowBase = table.Rows[0];
        List<object> lstObj1 = new List<object>();
        for (int index = 0; index < table.Columns.Count; ++index)
        {
            string rowName = rowBase[index].ToString().ToLower();
            if (rowName.Length >= 6 && rowName.Substring(0, 6) == "column")
            {
                ditcRemark.Add(index.ToString(), true);
            }
            else if (rowName == "")
            {
                ditcRemark.Add(index.ToString(), true);
            }
            else
            {
                lstObj1.Add(rowBase[index].ToString());
            }
        }
        listData.Add(lstObj1);

        // 扫描表数据
        for (int index1 = 1; index1 < table.Rows.Count; ++index1)
        {
            DataRow row = table.Rows[index1];
            List<object> lstObjData = new List<object>();
            for (int index2 = 0; index2 < table.Columns.Count; ++index2)
            {
                DataColumn column = table.Columns[index2];
                column.ToString().ToLower();
                if (!ditcRemark.ContainsKey(index2.ToString()))
                {
                    object obj = row[column];
                    if (obj.GetType() == typeof(double))
                    {
                        double num = (double)obj;
                        if ((double)(int)num == num)
                            obj = (object)(int)num;
                    }
                    lstObjData.Add(obj);
                }
            }

            if (lstObjData.Count > 0)
            {
                listData.Add(lstObjData);
            }
        }

        // 组装数据
        StringBuilder strValueNew = new StringBuilder();
        // List<int> lstKeyFieldIndex = new List<int>();
        for (int index1 = 0; index1 < listData.Count; ++index1)
        {
            if (index1 > 2)
            {
                break;
            }

            List<object> objectList = listData[index1];

            for (int index = 0; index < objectList.Count; ++index)
            {
                string strValue = objectList[index].ToString();

                if (index1 == 0)
                {
                    strValueNew.Clear();
                    bool bGang = false;

                    foreach (char c in strValue)
                    {
                        if (c == '_')
                        {
                            bGang = true;
                        }
                        else
                        {
                            if (bGang)
                            {
                                strValueNew.Append(c.ToString().ToUpper());
                            }
                            else
                            {
                                strValueNew.Append(c.ToString());
                            }

                            bGang = false;
                        }
                    }

                    listDataName.Add(strValueNew.ToString());
                }
                else if (index1 == 1)
                {
                    listDataType.Add(strValue);
                }
                else if (index1 == 2)
                {
                    listDataDesc.Add(strValue);
                }
                //                 else if (index1 == 3)
                //                 {
                //                     if (strValue.ToLower().Equals("key"))
                //                     {
                //                         lstKeyFieldIndex.Add(index);
                //                     }
                //                 }
            }
        }

        // 唯一键值
        //         if (lstKeyFieldIndex.Count == 1)
        //         {
        //             int nKeyIndex = lstKeyFieldIndex[0];
        // 
        //             if (nKeyIndex >=0 && nKeyIndex < listDataName.Count)
        //             {
        //                 strKeyField = listDataName[nKeyIndex];
        //             }
        //         }

        if (listDataName.Count > 0 && listDataName[0].ToLower().Equals("id"))
        {
            // 固定为Id字段为key
            strKeyField = "Id";

            if (listDataType.Count > 0)
            {
                keyFieldType = GetFieldType(listDataType[0]);
            }
        }
        else
        {
            strKeyField = "";
            keyFieldType = FieldType.INT;
        }

        return true;
    }
    /// <summary>
    /// 单个文件导出为proto
    /// </summary>
    /// <param name="strPath"></param>
    public static void doXlsxToProto(string strPath)
    {
        List<List<object>> listData = new List<List<object>>();

        //字段名
        List<string> listDataName = new List<string>();
        //数据类型
        List<string> listDataType = new List<string>();
        //说明
        List<string> listDataDesc = new List<string>();
        string strKeyField = "";
        FieldType keyFieldType = FieldType.INT;

        if (ExporterUtils.ExportXlsx(strPath, ref listData, ref listDataName, ref listDataType, ref listDataDesc, ref strKeyField, ref keyFieldType))
        {
            string strFileName = System.IO.Path.GetFileNameWithoutExtension(strPath);
            string strFileDirPath = ExporterUtils.GetDirPathConfigProto();
            string className = ExporterUtils.S_CONFIGPRE + ExporterUtils.GetOptUpperStr(strFileName);
            string unitClassName = className + "Unit";

            CreateUnitClassFile(listData, listDataName, listDataType, listDataDesc, strKeyField, strFileDirPath, className, unitClassName);
            CreateMainClassFile(listData, listDataName, listDataType, listDataDesc, strKeyField, keyFieldType, strFileDirPath, className, unitClassName);
        }
    }
    
    private static void CreateUnitClassFile(
        List<List<object>> listData, List<string> listDataName,
        List<string> listDataType, List<string> listDataDesc,
        string strKeyField,
        string strFileDirPath, string className, string unitClassName)
    {
        var TargetCodeFile = new StringBuilder();
        TargetCodeFile.Append(strFileDirPath);
        TargetCodeFile.Append("\\");
        TargetCodeFile.Append(unitClassName);
        TargetCodeFile.Append(".proto");
        //string TargetCodeFile = strFileDirPath + "\\" + unitClassName + ".proto";

        var utf8WithoutBom = new UTF8Encoding(false);

        using (StreamWriter writer = new StreamWriter(TargetCodeFile.ToString(), false, utf8WithoutBom))
        {
            try
            {
                string code = GenerateUnitCode(listData, listDataName, listDataType, listDataDesc, strKeyField, strFileDirPath, className, unitClassName);
                writer.Write("{0}\n", code);
            }
            catch (System.Exception ex)
            {
                string msg = "threw:\n" + ex.ToString();
                Debug.LogError(msg);
            }
        }
    }

    public static void doXlsxToEditCS(string strPath)
    {
        List<List<object>> listData = new List<List<object>>();

        //字段名
        List<string> listDataName = new List<string>();
        //数据类型
        List<string> listDataType = new List<string>();
        //说明
        List<string> listDataDesc = new List<string>();
        string strKeyField = "";
        FieldType keyFieldType = FieldType.INT;

        if (!ExporterUtils.ExportXlsx(strPath, ref listData, ref listDataName, ref listDataType, ref listDataDesc, ref strKeyField, ref keyFieldType))
        {
            return;
        }

        string strFileName = System.IO.Path.GetFileNameWithoutExtension(strPath);
        string className = ExporterUtils.S_CONFIGPRE + ExporterUtils.GetOptUpperStr(strFileName);
        string unitClassName = className + "Unit";
        string classNamePath = ExporterUtils.GetDirPathCS() + className + ".cs";

        if (!File.Exists(classNamePath))
        {
            return;
        }

        string strContent = File.ReadAllText(classNamePath);

        // Replace 1
        var strTip1 = new StringBuilder();
        var strTip2 = new StringBuilder();
        strTip1.Append("using pb = global::Google.Protobuf;");
        strTip2.Append("using Engine;\nusing pb = global::Google.Protobuf;");
        strContent = strContent.Replace(strTip1.ToString(), strTip2.ToString());

        // Replace 2
        strTip1.Clear();
        strTip1.Append("pb::IMessage<");
        strTip1.Append(className);
        strTip1.Append(">");

        strTip2.Clear();
        strTip2.Append(strTip1);
        strTip2.Append(", EngineBase.IConfigBase");
        strContent = strContent.Replace(strTip1.ToString(), strTip2.ToString());

        // Replace 3
        strTip1.Clear();
        strTip1.Append("private static readonly pb::MessageParser<");
        strTip1.Append(className);
        strTip1.Append(">");

        string strTipFormat = "";
        if (strKeyField != "")
        {
            if (keyFieldType == FieldType.INT)
            {
                strTipFormat = @"
    public static {0} GetNewConfig(byte[] bytes) { 
        if (bytes == null) return null;
        {0} config = ({0}){0}.Descriptor.Parser.ParseFrom(bytes);
        return config;
    }
    public {1} Get(int key) {
        {1} value;
        if (this.Data.TryGetValue(key, out value))
            return value;
        return null;
    }
    public scg::ICollection<int> Keys {
        get { return this.Data.Keys; }
    }
    public int Count {
        get { return this.Data.Count; }
    }
";
            }
            else
            {
                strTipFormat = @"
    public static {0} GetNewConfig(byte[] bytes) { 
        if (bytes == null) return null;
        {0} config = ({0}){0}.Descriptor.Parser.ParseFrom(bytes);
        return config;
    }
    public {1} Get(string key) {
        {1} value;
        if (this.Data.TryGetValue(key, out value))
            return value;
        return null;
    }
    public scg::ICollection<string> Keys {
        get { return this.Data.Keys; }
    }
    public int Count {
        get { return this.Data.Count; }
    }
";
            }
        }
        else
        {
            strTipFormat = @"
    public static {0} GetNewConfig(byte[] bytes) { 
        if (bytes == null) return null;
        {0} config = ({0}){0}.Descriptor.Parser.ParseFrom(bytes);
        return config;
    }
    public {1} Get(int key) {
        if (key >= 0 && key < this.Data.Count)
            return this.Data[key];
        return null;
    }
    public scg::ICollection<int> Keys {
        get {
            if (this.Data.Count <= 0) return null;
            var lst = new scg::List<int>();
            for (int i = 0; i < this.Data.Count; i++) lst.Add(i);
            return lst;
        }
    }
    public int Count {
        get { return this.Data.Count; }
    }
";
        }

        strTipFormat = strTipFormat.Replace("{0}", className);
        strTipFormat = strTipFormat.Replace("{1}", unitClassName);
        strTip2.Clear();
        strTip2.Append(string.Format("{0}    {1}", strTipFormat, strTip1));
        strContent = strContent.Replace(strTip1.ToString(), strTip2.ToString());

        var utf8WithoutBom = new UTF8Encoding(false);

        using (StreamWriter writer = new StreamWriter(classNamePath, false, utf8WithoutBom))
        {
            try
            {
                strContent = strContent.Replace("\r", "");
                writer.Write("{0}\n", strContent);
            }
            catch (System.Exception ex)
            {
                string msg = "threw:\n" + ex.ToString();
                Debug.LogError(msg);
            }
        }
    }

    public static void doXlsxToCSHelper(string strPath)
    {
        List<List<object>> listData = new List<List<object>>();
        //字段名
        List<string> listDataName = new List<string>();
        //数据类型
        List<string> listDataType = new List<string>();
        //说明
        List<string> listDataDesc = new List<string>();
        string strKeyField = "";
        FieldType keyFieldType = FieldType.INT;

        if (ExporterUtils.ExportXlsx(strPath, ref listData, ref listDataName, ref listDataType, ref listDataDesc, ref strKeyField, ref keyFieldType))
        {
            string strFileName = System.IO.Path.GetFileNameWithoutExtension(strPath);
            string className = ExporterUtils.S_CONFIGPRE + ExporterUtils.GetOptUpperStr(strFileName);
            string unitClassName = className + "Unit";

            CreateHelperClassFile(listData, listDataName, listDataType, listDataDesc, strKeyField, strFileName, className, unitClassName);
        }
    }

    private static void CreateMainClassFile(
        List<List<object>> listData, List<string> listDataName,
        List<string> listDataType, List<string> listDataDesc,
        string strKeyField, FieldType keyFieldType,
        string strFileDirPath, string className, string unitClassName)
    {
        var TargetCodeFile = new StringBuilder();
        TargetCodeFile.Append(strFileDirPath);
        TargetCodeFile.Append("\\");
        TargetCodeFile.Append(className);
        TargetCodeFile.Append(".proto");
        
        var utf8WithoutBom = new UTF8Encoding(false);

        using (StreamWriter writer = new StreamWriter(TargetCodeFile.ToString(), false, utf8WithoutBom))
        {
            try
            {
                string code = CreateMainClass(listData, listDataName, listDataType, listDataDesc, strKeyField, keyFieldType, strFileDirPath, className, unitClassName);
                writer.Write("{0}\n", code);
            }
            catch (System.Exception ex)
            {
                string msg = "threw:\n" + ex.ToString();
                Debug.LogError(msg);
            }
        }
    }
    /// <summary>
    /// 拼接proto数据
    /// </summary>
    /// <param name="listData"></param>
    /// <param name="listDataName"></param>
    /// <param name="listDataType"></param>
    /// <param name="listDataDesc"></param>
    /// <param name="strKeyField"></param>
    /// <param name="strFileDirPath"></param>
    /// <param name="className"></param>
    /// <param name="unitClassName"></param>
    /// <returns></returns>
    private static string GenerateUnitCode(
        List<List<object>> listData, List<string> listDataName,
        List<string> listDataType, List<string> listDataDesc,
        string strKeyField,
        string strFileDirPath, string className, string unitClassName)
    {
        var code = new StringBuilder();

        code.Append("syntax = \"proto3\";\n");
        code.Append("\n");
        code.Append("package Config;\n");
        code.Append("\n");
        code.Append(string.Format("message {0} {1}\n", unitClassName, "{"));

        int nIndex = 1;

        for (int i = 0; i < listDataName.Count && i < listDataType.Count; i++)
        {
            string newStr = ExporterUtils.GetFirstUpperStr(listDataName[i]);
            string newStr2 = listDataType[i];

            newStr = newStr.Replace('\r', ' ');
            newStr2 = newStr2.Replace('\r', ' ');

            if (string.IsNullOrEmpty(newStr))
            {
                continue;
            }

            FieldType fieldType = GetFieldType(newStr2);

            if (fieldType == FieldType.ARRAYINT)
            {
                code.Append(System.String.Format("  repeated int32 {0} = {1:d};\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.ARRAYFLOAT)
            {
                code.Append(System.String.Format("  repeated float {0} = {1:d};\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.ARRAYSTRING || fieldType == FieldType.ARRAYSTRINGSEMI)
            {
                code.Append(System.String.Format("  repeated string {0} = {1:d};\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.INT)
            {
                code.Append(System.String.Format("  int32 {0} = {1:d};\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.FLOAT)
            {
                code.Append(System.String.Format("  float {0} = {1:d};\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.LONG)
            {
                code.Append(System.String.Format("  int64 {0} = {1:d};\n", newStr, nIndex));
            }
            else
            {
                code.Append(System.String.Format("  string {0} = {1:d};\n", newStr, nIndex));
            }

            ++nIndex;
        }

        code.Append(string.Format("{0}", "}"));

        return code.ToString();
    }

    private static string CreateMainClass(
        List<List<object>> listData, List<string> listDataName,
        List<string> listDataType, List<string> listDataDesc,
        string strKeyField, FieldType keyFieldType,
        string strFileDirPath, string className, string unitClassName)
    {
        StringBuilder code = new StringBuilder();

        code.Append("syntax = \"proto3\";\n");
        code.Append("\n");
        code.Append(string.Format("import \"{0}.proto\";\n", unitClassName));
        code.Append("\n");
        code.Append("package Config;\n");
        code.Append("\n");
        code.Append(string.Format("message {0} {1}\n", className, "{"));

        if (strKeyField != "")
        {
            if (keyFieldType == FieldType.INT)
            {
                code.Append(string.Format("  map<int32, {0}> data = 1;\n", unitClassName));
            }
            else
            {
                code.Append(string.Format("  map<string, {0}> data = 1;\n", unitClassName));
            }
        }
        else
        {
            code.Append(string.Format("  repeated {0} data = 1;\n", unitClassName));
        }

        code.Append(string.Format("{0}", "}"));

        return code.ToString();
    }

    private static void CreateHelperClassFile(
        List<List<object>> listData, List<string> listDataName,
        List<string> listDataType, List<string> listDataDesc,
        string strKeyField,
        string strFileName,
        string className,
        string unitClassName)
    {
        string strFormat =
@"using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;
using UnityEngine;
using Config;

namespace Google.Protobuf
{
    public class {1}Helper
    {
        private static string s_ClassNameRaw = ""{0}"";
        private static string s_ClassName = ""{1}"";

        public static void doXlsxToBin()
        {
            string strPath = ExporterUtils.GetDirPathConfig() + s_ClassNameRaw + "".xlsx"";
            string strPathOut = ExporterUtils.GetDirPathConfigBin() + s_ClassName + "".bin"";
            
            List<List<object>> listData = new List<List<object>>();
            List<string> listDataName = new List<string>();
            List<string> listDataType = new List<string>();
            List<string> listDataDesc = new List<string>();
            string strKeyField = """";
            FieldType keyFieldType = FieldType.INT;

            if (ExporterUtils.ExportXlsx(strPath, ref listData, ref listDataName, ref listDataType, ref listDataDesc, ref strKeyField, ref keyFieldType))
            {
{2}
            }
        }
    }
}";

        var strFieldNameSingle = new StringBuilder();
        //string strFieldNameSingle = "";

        int nIndex = 0;

        for (int i = 0; i < listDataName.Count && i < listDataType.Count; i++)
        {
            string newStr = ExporterUtils.GetFirstUpperStr(listDataName[i]);
            string newStr2 = listDataType[i];

            newStr = newStr.Replace('\r', ' ');
            newStr2 = newStr2.Replace('\r', ' ');

            if (string.IsNullOrEmpty(newStr))
            {
                continue;
            }

            FieldType fieldType = GetFieldType(newStr2);

            if (fieldType == FieldType.ARRAYINT)
            {
                strFieldNameSingle.Append(System.String.Format("                    ExporterUtils.GetArrayInt(unit.{0}, curData[{1:d}].ToString());\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.ARRAYFLOAT)
            {
                strFieldNameSingle.Append(System.String.Format("                    ExporterUtils.GetArrayFloat(unit.{0}, curData[{1:d}].ToString());\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.ARRAYSTRING)
            {
                strFieldNameSingle.Append(System.String.Format("                    ExporterUtils.GetArrayString(unit.{0}, curData[{1:d}].ToString());\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.ARRAYSTRINGSEMI)
            {
                strFieldNameSingle.Append(System.String.Format("                    ExporterUtils.GetArrayStringSemi(unit.{0}, curData[{1:d}].ToString());\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.INT)
            {
                strFieldNameSingle.Append(System.String.Format("                    unit.{0} = ExporterUtils.GetInt(curData[{1:d}].ToString());\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.FLOAT)
            {
                strFieldNameSingle.Append(System.String.Format("                    unit.{0} = ExporterUtils.GetFloat(curData[{1:d}].ToString());\n", newStr, nIndex));
            }
            else if (fieldType == FieldType.LONG)
            {
                strFieldNameSingle.Append(System.String.Format("                    unit.{0} = ExporterUtils.GetLong(curData[{1:d}].ToString());\n", newStr, nIndex));
            }
            else
            {
                strFieldNameSingle.Append(System.String.Format("                    unit.{0} = curData[{1:d}].ToString();\n", newStr, nIndex));
            }

            ++nIndex;
        }

        string strFieldName =
@"{4}
                
                {0} data = new {0}();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    {1} unit = new {1}();
                    
{2}                    
{3}
                }
                
                EngineBase.ConfigSerializer.Serialize<{0}>(data, strPathOut);
";

        //string strFieldNameKey = "";
        var strFieldNameKey = new StringBuilder();

        if (strKeyField != "")
        {
            string strKeyFieldFormat =
@"                    if (data.Data.ContainsKey(unit.{0}))
                    {
                        Debug.LogError(""[XlsxToBin] doXlsxToBin Error: {1} Repeat Key, Please Check. "" + unit.{0});
                    }
                    
                    data.Data[unit.{0}] = unit;";

            string strTmp = strKeyFieldFormat;
            strTmp = strTmp.Replace("{0}", ExporterUtils.GetFirstUpperStr(strKeyField));
            strTmp = strTmp.Replace("{1}", className);

            strFieldNameKey.Append(strTmp);
        }
        else
        {
            strFieldNameKey.Append(string.Format("                    data.Data.Add(unit);"));
        }

        string strFieldNum =
@"                int nFieldNum = {0};
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError(""[XlsxToBin]	doXlsxToBin Fail: {1} FieldNum NotMatch With Excel, Please Check."");
                     return;
                }";

        strFieldNum = strFieldNum.Replace("{0}", nIndex.ToString());
        strFieldNum = strFieldNum.Replace("{1}", className);

        strFieldName = strFieldName.Replace("{0}", className);
        strFieldName = strFieldName.Replace("{1}", unitClassName);
        strFieldName = strFieldName.Replace("{2}", strFieldNameSingle.ToString());
        strFieldName = strFieldName.Replace("{3}", strFieldNameKey.ToString());
        strFieldName = strFieldName.Replace("{4}", strFieldNum);

        string strFileContent = strFormat;
        strFileContent = strFileContent.Replace("{0}", strFileName);
        strFileContent = strFileContent.Replace("{1}", className);
        strFileContent = strFileContent.Replace("{2}", strFieldName);

        string TargetCodeFile = GetDirPathCSHelper() + className + "Helper.cs";

        var utf8WithoutBom = new UTF8Encoding(false);

        using (StreamWriter writer = new StreamWriter(TargetCodeFile, false, utf8WithoutBom))
        {
            try
            {
                strFileContent = strFileContent.Replace("\r", "");
                writer.Write("{0}\n", strFileContent);
            }
            catch (System.Exception ex)
            {
                string msg = " threw:\n" + ex.ToString();
                Debug.LogError(msg);
            }
        }
    }

    private static Dictionary<string, string> localExportHash = new Dictionary<string, string>();

    public static void InitExportLog()
    {
        localExportHash.Clear();

        string strExportLogFilePath = ExporterUtils.GetExportToBinAndJsonFilePath();

        if (!File.Exists(strExportLogFilePath))
        {
            return;
        }

        using (StreamReader sr = File.OpenText(strExportLogFilePath))
        {
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] fileDatas = line.Split('&');
                if (fileDatas.Length >= 2)
                {
                    localExportHash[fileDatas[0]] = fileDatas[1];
                }
            }
            sr.Close();
        }
    }

    public static bool CheckExportLog(string fileFullName)
    {
        string strHashNow = "";
        using (HashAlgorithm hash = HashAlgorithm.Create())
        {
            using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read))
            {
                byte[] hashByte = hash.ComputeHash(file);
                strHashNow = BitConverter.ToString(hashByte).ToLower();
            }
        }

        // 须统一路径格式
        string strRootDir = Application.dataPath + "/Editor Default Resources/Config/config/";
        strRootDir = strRootDir.Replace("\\", "/");
        string strFileSave = fileFullName.Replace("\\", "/");
        strFileSave = strFileSave.Replace(strRootDir, "");

        string strHashLast = "";

        if (localExportHash.TryGetValue(strFileSave, out strHashLast))
        {
            if (strHashLast.ToLower().Equals(strHashNow))
            {
                // 文件无变动
                return true;
            }
        }

        return false;
    }

    public static void RecordExportLog(string fileFullName)
    {
        string strHashNow = "";
        using (HashAlgorithm hash = HashAlgorithm.Create())
        {
            using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read))
            {
                byte[] hashByte = hash.ComputeHash(file);
                strHashNow = BitConverter.ToString(hashByte).ToLower();
            }
        }

        // 须统一路径格式
        string strRootDir = Application.dataPath + "/Editor Default Resources/Config/config/";
        strRootDir = strRootDir.Replace("\\", "/");
        string strFileSave = fileFullName.Replace("\\", "/");
        strFileSave = strFileSave.Replace(strRootDir, "");

        localExportHash[strFileSave] = strHashNow;
    }

    public static void SaveExportLog()
    {
        string filePath = ExporterUtils.GetExportToBinAndJsonFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        StreamWriter sw = File.CreateText(filePath);
        StringBuilder strlog = new StringBuilder();
        foreach (var item in localExportHash)
        {
            strlog.Append(item.Key).Append("&").Append(item.Value);
            sw.WriteLine(strlog.ToString());
            strlog.Clear();
        }
        sw.Close();
    }

    public static void ClearExportLog()
    {
        string strExportLogFilePath = ExporterUtils.GetExportToBinAndJsonFilePath();

        if (File.Exists(strExportLogFilePath))
        {
            File.Delete(strExportLogFilePath);
        }

        InitExportLog();
    }

    public static void doXlsxToJson(string strPath)
    {
        List<List<object>> listData = new List<List<object>>();

        //字段名
        List<string> listDataName = new List<string>();
        //数据类型
        List<string> listDataType = new List<string>();
        //说明
        List<string> listDataDesc = new List<string>();
        string strKeyField = "";
        FieldType keyFieldType = FieldType.INT;
        if (ExporterUtils.ExportXlsx(strPath, ref listData, ref listDataName, ref listDataType, ref listDataDesc,
            ref strKeyField, ref keyFieldType))
        {
            FirstFieldName(ref listDataName);
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string strFileName = System.IO.Path.GetFileNameWithoutExtension(strPath);
            string strFileDirPath = ExporterUtils.GetDirPathConfigJson();

            for (int valueIndex = 3; valueIndex < listData.Count; valueIndex++)
            {
                Dictionary<string, object> dir = new Dictionary<string, object>();
                for (int keyIndex = 0;
                    keyIndex < listDataName.Count && keyIndex < listDataType.Count && keyIndex < listDataDesc.Count;
                    keyIndex++)
                {
                    String value = listData[valueIndex][keyIndex].ToString();
                    FieldType fieldType = GetFieldType(listDataType[keyIndex]);
                    switch (fieldType)
                    {
                        case FieldType.ARRAYSTRING:
                            dir.Add(listDataName[keyIndex], value.Split(','));
                            break;
                        case FieldType.ARRAYSTRINGSEMI:
                            dir.Add(listDataName[keyIndex], value.Split(';'));
                            break;
                        case FieldType.ARRAYINT:
                            List<int> intList = new List<int>();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                foreach (String item in value.Split(','))
                                {
                                    if (!string.IsNullOrWhiteSpace(item))
                                    {
                                        intList.Add(GetInt(item));
                                    }
                                }
                            }

                            dir.Add(listDataName[keyIndex], intList);
                            break;
                        case FieldType.ARRAYFLOAT:
                            List<float> floatList = new List<float>();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                foreach (String item in value.Split(','))
                                {
                                    if (!string.IsNullOrWhiteSpace(item))
                                    {
                                        floatList.Add(float.Parse(item));
                                    }
                                }
                            }

                            dir.Add(listDataName[keyIndex], floatList);
                            break;
                        case FieldType.INT:
                            dir.Add(listDataName[keyIndex], ExporterUtils.GetInt(value));
                            break;
                        case FieldType.FLOAT:
                            dir.Add(listDataName[keyIndex], ExporterUtils.GetFloat(value));
                            break;
                        case FieldType.LONG:
                            dir.Add(listDataName[keyIndex], ExporterUtils.GetLong(value));
                            break;
                        case FieldType.STRING:
                        default:
                            dir.Add(listDataName[keyIndex], value);
                            break;
                    }

                }

                list.Add(dir);
            }

            string jsonStr = SimpleJson.SerializeObject(list);
            string jsonStrIndented = JsonConvert.SerializeObject(list, Formatting.Indented);
            var utf8WithoutBom = new UTF8Encoding(false);
            string strFilePath = strFileDirPath + strFileName + ".json";
            using (StreamWriter writer = new StreamWriter(strFilePath, false, utf8WithoutBom))
            {
                try
                {
                    writer.Write("{0}", jsonStrIndented);
                }
                catch (System.Exception ex)
                {
                    string msg = " threw:\n" + ex.ToString();
                    Debug.LogError(msg);
                }
            }
        }
    }

    private static void FirstFieldName(ref List<string> listDataName)
    {
        for (int i = 0; i < listDataName.Count; i++)
        {
            string val = listDataName[i];
            if (!string.IsNullOrEmpty(val))
            {
                char[] chars = listDataName[i].ToCharArray();
                char fc = chars[0];
                if (fc >= 'a' && fc <= 'z')
                {
                    fc = (char)(fc - 32);
                    chars[0] = fc;
                    listDataName[i] = new String(chars);
                }
            }
        }
    }
}
