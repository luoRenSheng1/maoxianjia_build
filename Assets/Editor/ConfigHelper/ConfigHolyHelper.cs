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
    public class ConfigHolyHelper
    {
        private static string s_ClassNameRaw = "Holy";
        private static string s_ClassName = "ConfigHoly";

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
                int nFieldNum = 12;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigHoly FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigHoly data = new ConfigHoly();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigHolyUnit unit = new ConfigHolyUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.HolyId = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Name = curData[2].ToString();
                    unit.Notes1 = curData[3].ToString();
                    unit.Type = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Lv = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Consume1 = ExporterUtils.GetInt(curData[6].ToString());
                    unit.Consume2 = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Doc = curData[8].ToString();
                    unit.Notes2 = curData[9].ToString();
                    unit.Common = curData[10].ToString();
                    unit.ItemID = ExporterUtils.GetInt(curData[11].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigHoly Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigHoly>(data, strPathOut);

            }
        }
    }
}
