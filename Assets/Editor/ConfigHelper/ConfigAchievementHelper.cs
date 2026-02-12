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
    public class ConfigAchievementHelper
    {
        private static string s_ClassNameRaw = "Achievement";
        private static string s_ClassName = "ConfigAchievement";

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
                int nFieldNum = 13;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigAchievement FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigAchievement data = new ConfigAchievement();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigAchievementUnit unit = new ConfigAchievementUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.AID = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Name = curData[2].ToString();
                    unit.AchievementType = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Doc = curData[5].ToString();
                    unit.NotesDoc = curData[6].ToString();
                    unit.PreAchievementID = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Param1 = curData[8].ToString();
                    unit.TexParam = curData[9].ToString();
                    unit.Reward = curData[10].ToString();
                    unit.SortID = ExporterUtils.GetInt(curData[11].ToString());
                    unit.Item = ExporterUtils.GetInt(curData[12].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigAchievement Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigAchievement>(data, strPathOut);

            }
        }
    }
}
