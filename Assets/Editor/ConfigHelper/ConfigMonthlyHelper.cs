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
    public class ConfigMonthlyHelper
    {
        private static string s_ClassNameRaw = "Monthly";
        private static string s_ClassName = "ConfigMonthly";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigMonthly FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigMonthly data = new ConfigMonthly();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigMonthlyUnit unit = new ConfigMonthlyUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.Type = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Time = ExporterUtils.GetInt(curData[4].ToString());
                    unit.ItemId = curData[5].ToString();
                    unit.DailyItemId = curData[6].ToString();
                    unit.Attr = curData[7].ToString();
                    unit.OtherAttr = curData[8].ToString();
                    unit.PayListID = ExporterUtils.GetInt(curData[9].ToString());
                    unit.TxtNote = curData[10].ToString();
                    unit.Txt = curData[11].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigMonthly Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigMonthly>(data, strPathOut);

            }
        }
    }
}
