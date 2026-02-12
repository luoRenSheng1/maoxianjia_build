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
    public class ConfigVipHelper
    {
        private static string s_ClassNameRaw = "Vip";
        private static string s_ClassName = "ConfigVip";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigVip FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigVip data = new ConfigVip();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigVipUnit unit = new ConfigVipUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Level = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Discount = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Remarks = curData[3].ToString();
                    unit.Desc = curData[4].ToString();
                    unit.VipExp = ExporterUtils.GetInt(curData[5].ToString());
                    unit.VipEeceive = curData[6].ToString();
                    unit.VipPurchase = curData[7].ToString();
                    ExporterUtils.GetArrayInt(unit.Price, curData[8].ToString());
                    unit.Background = curData[9].ToString();
                    unit.VipBonus = curData[10].ToString();
                    unit.Reward = ExporterUtils.GetInt(curData[11].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigVip Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigVip>(data, strPathOut);

            }
        }
    }
}
