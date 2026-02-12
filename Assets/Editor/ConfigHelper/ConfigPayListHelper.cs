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
    public class ConfigPayListHelper
    {
        private static string s_ClassNameRaw = "PayList";
        private static string s_ClassName = "ConfigPayList";

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
                int nFieldNum = 13;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigPayList FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigPayList data = new ConfigPayList();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigPayListUnit unit = new ConfigPayListUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[1].ToString());
                    unit.DescNote = curData[2].ToString();
                    unit.Desc = curData[3].ToString();
                    unit.DescParam = curData[4].ToString();
                    unit.XmProductId = curData[5].ToString();
                    unit.Remarks = curData[6].ToString();
                    unit.RechargeAmount = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Sort = ExporterUtils.GetInt(curData[8].ToString());
                    unit.FirstAddDiamonds = ExporterUtils.GetInt(curData[9].ToString());
                    unit.AddDiamonds = ExporterUtils.GetInt(curData[10].ToString());
                    unit.Icon = ExporterUtils.GetInt(curData[11].ToString());
                    unit.MoneyType = ExporterUtils.GetInt(curData[12].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigPayList Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigPayList>(data, strPathOut);

            }
        }
    }
}
