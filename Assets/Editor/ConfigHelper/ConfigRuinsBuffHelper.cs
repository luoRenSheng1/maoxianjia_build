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
    public class ConfigRuinsBuffHelper
    {
        private static string s_ClassNameRaw = "RuinsBuff";
        private static string s_ClassName = "ConfigRuinsBuff";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigRuinsBuff FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigRuinsBuff data = new ConfigRuinsBuff();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigRuinsBuffUnit unit = new ConfigRuinsBuffUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.Lv = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Attr = curData[4].ToString();
                    unit.Rate = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Time = ExporterUtils.GetInt(curData[6].ToString());
                    unit.Note1 = curData[7].ToString();
                    unit.Icon = ExporterUtils.GetInt(curData[8].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigRuinsBuff Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigRuinsBuff>(data, strPathOut);

            }
        }
    }
}
