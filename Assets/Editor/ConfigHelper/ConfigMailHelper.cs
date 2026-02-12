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
    public class ConfigMailHelper
    {
        private static string s_ClassNameRaw = "Mail";
        private static string s_ClassName = "ConfigMail";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigMail FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigMail data = new ConfigMail();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigMailUnit unit = new ConfigMailUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.TitleNote = curData[1].ToString();
                    unit.Title = curData[2].ToString();
                    unit.TitleParam = curData[3].ToString();
                    unit.TxtNote = curData[4].ToString();
                    unit.Txt = curData[5].ToString();
                    unit.TxtParam = curData[6].ToString();
                    unit.SenderNameNote = curData[7].ToString();
                    unit.SenderName = curData[8].ToString();
                    unit.LiveTime = ExporterUtils.GetInt(curData[9].ToString());
                    unit.StartTime = curData[10].ToString();
                    unit.EndTime = curData[11].ToString();
                    unit.IsOneClick = ExporterUtils.GetInt(curData[12].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigMail Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigMail>(data, strPathOut);

            }
        }
    }
}
