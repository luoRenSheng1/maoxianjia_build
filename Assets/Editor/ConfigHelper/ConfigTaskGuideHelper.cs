using System;
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
    public class ConfigTaskGuideHelper
    {
        private static string s_ClassNameRaw = "TaskGuide";
        private static string s_ClassName = "ConfigTaskGuide";

        public static void doXlsxToBin()
        {
            string strPath = ExporterUtils.GetDirPathConfig() + s_ClassNameRaw + ".xlsx";
            string strPathOut = ExporterUtils.GetDirPathConfigBin() + s_ClassName + ".bin";
            
            List<List<object>> listData = new List<List<object>>();
            List<string> listDataName = new List<string>();
            List<string> listDataType = new List<string>();
            List<string> listDataDesc = new List<string>();
            string strKeyField = "";
            FieldType keyFieldType = FieldType.INT;

            if (ExporterUtils.ExportXlsx(strPath, ref listData, ref listDataName, ref listDataType, ref listDataDesc, ref strKeyField, ref keyFieldType))
            {
                int nFieldNum = 8;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigTaskGuide FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigTaskGuide data = new ConfigTaskGuide();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigTaskGuideUnit unit = new ConfigTaskGuideUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.TaskName = curData[1].ToString();
                    unit.Conduct = ExporterUtils.GetInt(curData[2].ToString());
                    unit.ConductNote = curData[3].ToString();
                    unit.JumpPath = curData[4].ToString();
                    unit.FingerPosition = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Doc = ExporterUtils.GetInt(curData[6].ToString());
                    unit.DocNote = curData[7].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigTaskGuide Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigTaskGuide>(data, strPathOut);

            }
        }
    }
}
