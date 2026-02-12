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
    public class ConfigMonsterEntryHelper
    {
        private static string s_ClassNameRaw = "MonsterEntry";
        private static string s_ClassName = "ConfigMonsterEntry";

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
                int nFieldNum = 9;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigMonsterEntry FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigMonsterEntry data = new ConfigMonsterEntry();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigMonsterEntryUnit unit = new ConfigMonsterEntryUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.EntryGroup = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Rate = ExporterUtils.GetInt(curData[2].ToString());
                    unit.MonsterEntryName = curData[3].ToString();
                    unit.MonsterEntryDoc = curData[4].ToString();
                    unit.SkillAchieveId = curData[5].ToString();
                    unit.Notes1 = curData[6].ToString();
                    unit.Notes2 = curData[7].ToString();
                    unit.Icon = ExporterUtils.GetInt(curData[8].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigMonsterEntry Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigMonsterEntry>(data, strPathOut);

            }
        }
    }
}
