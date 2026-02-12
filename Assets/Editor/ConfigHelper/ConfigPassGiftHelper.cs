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
    public class ConfigPassGiftHelper
    {
        private static string s_ClassNameRaw = "PassGift";
        private static string s_ClassName = "ConfigPassGift";

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
                int nFieldNum = 6;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigPassGift FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigPassGift data = new ConfigPassGift();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigPassGiftUnit unit = new ConfigPassGiftUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Level = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Score = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Gift = curData[3].ToString();
                    unit.FancyGift = curData[4].ToString();
                    unit.StageLevelId = ExporterUtils.GetInt(curData[5].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigPassGift Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigPassGift>(data, strPathOut);

            }
        }
    }
}
