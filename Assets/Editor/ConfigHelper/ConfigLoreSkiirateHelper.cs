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
    public class ConfigLoreSkiirateHelper
    {
        private static string s_ClassNameRaw = "LoreSkiirate";
        private static string s_ClassName = "ConfigLoreSkiirate";

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
                int nFieldNum = 5;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigLoreSkiirate FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigLoreSkiirate data = new ConfigLoreSkiirate();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigLoreSkiirateUnit unit = new ConfigLoreSkiirateUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.SkillGroup = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Skill = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Rate = ExporterUtils.GetInt(curData[3].ToString());
                    unit.LV = ExporterUtils.GetInt(curData[4].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigLoreSkiirate Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigLoreSkiirate>(data, strPathOut);

            }
        }
    }
}
