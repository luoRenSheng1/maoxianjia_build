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
    public class ConfigBuildHelper
    {
        private static string s_ClassNameRaw = "Build";
        private static string s_ClassName = "ConfigBuild";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigBuild FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigBuild data = new ConfigBuild();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigBuildUnit unit = new ConfigBuildUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.BuildNameNote = curData[1].ToString();
                    unit.BuildName = curData[2].ToString();
                    unit.BuildType = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Item = ExporterUtils.GetInt(curData[4].ToString());
                    unit.ProduceTime = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Number = ExporterUtils.GetInt(curData[6].ToString());
                    unit.PeakTime = ExporterUtils.GetInt(curData[7].ToString());
                    unit.ResourcesIcon = curData[8].ToString();
                    unit.BuildIcon = curData[9].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigBuild Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigBuild>(data, strPathOut);

            }
        }
    }
}
