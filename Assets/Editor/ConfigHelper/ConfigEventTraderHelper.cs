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
    public class ConfigEventTraderHelper
    {
        private static string s_ClassNameRaw = "EventTrader";
        private static string s_ClassName = "ConfigEventTrader";

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
                int nFieldNum = 10;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigEventTrader FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigEventTrader data = new ConfigEventTrader();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigEventTraderUnit unit = new ConfigEventTraderUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.ItemId = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Number = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Position = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Rate = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Score = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Limit = ExporterUtils.GetInt(curData[6].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[7].ToString());
                    unit.SystemId = ExporterUtils.GetInt(curData[8].ToString());
                    unit.NotesUse = curData[9].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigEventTrader Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigEventTrader>(data, strPathOut);

            }
        }
    }
}
