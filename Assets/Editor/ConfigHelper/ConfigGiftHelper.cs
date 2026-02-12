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
    public class ConfigGiftHelper
    {
        private static string s_ClassNameRaw = "Gift";
        private static string s_ClassName = "ConfigGift";

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
                int nFieldNum = 15;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigGift FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigGift data = new ConfigGift();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigGiftUnit unit = new ConfigGiftUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.GiftType = ExporterUtils.GetInt(curData[1].ToString());
                    unit.NameNote = curData[2].ToString();
                    unit.Name = curData[3].ToString();
                    unit.NameParam = curData[4].ToString();
                    unit.ItemId = curData[5].ToString();
                    unit.Buy = ExporterUtils.GetInt(curData[6].ToString());
                    unit.MoneyType = ExporterUtils.GetInt(curData[7].ToString());
                    unit.BuyNumber = ExporterUtils.GetInt(curData[8].ToString());
                    unit.Price = ExporterUtils.GetInt(curData[9].ToString());
                    unit.Sort = ExporterUtils.GetInt(curData[10].ToString());
                    unit.Rebate = curData[11].ToString();
                    unit.PayList = ExporterUtils.GetInt(curData[12].ToString());
                    unit.System = ExporterUtils.GetInt(curData[13].ToString());
                    unit.BgUrl = ExporterUtils.GetInt(curData[14].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigGift Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigGift>(data, strPathOut);

            }
        }
    }
}
