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
    public class ConfigTreasureChestHelper
    {
        private static string s_ClassNameRaw = "TreasureChest";
        private static string s_ClassName = "ConfigTreasureChest";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigTreasureChest FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigTreasureChest data = new ConfigTreasureChest();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigTreasureChestUnit unit = new ConfigTreasureChestUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NextID = ExporterUtils.GetInt(curData[1].ToString());
                    unit.GoldCoins = curData[2].ToString();
                    unit.GoldCoinsNum = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Consume = ExporterUtils.GetInt(curData[4].ToString());
                    unit.UpTime = ExporterUtils.GetInt(curData[5].ToString());
                    unit.QualityPro = curData[6].ToString();
                    unit.QualityProShow = curData[7].ToString();
                    unit.PartPro = curData[8].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigTreasureChest Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigTreasureChest>(data, strPathOut);

            }
        }
    }
}
