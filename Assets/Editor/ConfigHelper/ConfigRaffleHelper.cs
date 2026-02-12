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
    public class ConfigRaffleHelper
    {
        private static string s_ClassNameRaw = "Raffle";
        private static string s_ClassName = "ConfigRaffle";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigRaffle FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigRaffle data = new ConfigRaffle();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigRaffleUnit unit = new ConfigRaffleUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Level = ExporterUtils.GetInt(curData[2].ToString());
                    unit.ItemType = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Number = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Probability = ExporterUtils.GetInt(curData[5].ToString());
                    unit.P1 = curData[6].ToString();
                    unit.P2 = curData[7].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigRaffle Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigRaffle>(data, strPathOut);

            }
        }
    }
}
