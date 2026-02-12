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
    public class ConfigBuffActionTemplateHelper
    {
        private static string s_ClassNameRaw = "BuffActionTemplate";
        private static string s_ClassName = "ConfigBuffActionTemplate";

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
                int nFieldNum = 14;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigBuffActionTemplate FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigBuffActionTemplate data = new ConfigBuffActionTemplate();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigBuffActionTemplateUnit unit = new ConfigBuffActionTemplateUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Name = curData[1].ToString();
                    unit.BuffEventType = ExporterUtils.GetInt(curData[2].ToString());
                    unit.CoverLastTime = ExporterUtils.GetFloat(curData[3].ToString());
                    unit.EffectTime = ExporterUtils.GetFloat(curData[4].ToString());
                    unit.COpFrequency = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Function = curData[6].ToString();
                    unit.Object = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Range = curData[8].ToString();
                    unit.Ruantity = ExporterUtils.GetInt(curData[9].ToString());
                    unit.Common = curData[10].ToString();
                    unit.BuffEffect = ExporterUtils.GetInt(curData[11].ToString());
                    unit.BuffTime = ExporterUtils.GetInt(curData[12].ToString());
                    unit.DevSide = ExporterUtils.GetInt(curData[13].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigBuffActionTemplate Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigBuffActionTemplate>(data, strPathOut);

            }
        }
    }
}
