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
    public class ConfigAptitudeUnlockHelper
    {
        private static string s_ClassNameRaw = "AptitudeUnlock";
        private static string s_ClassName = "ConfigAptitudeUnlock";

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
                int nFieldNum = 7;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigAptitudeUnlock FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigAptitudeUnlock data = new ConfigAptitudeUnlock();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigAptitudeUnlockUnit unit = new ConfigAptitudeUnlockUnit();
                    
                    unit.BuffId = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Name = curData[1].ToString();
                    unit.Type = ExporterUtils.GetInt(curData[2].ToString());
                    unit.PreId = curData[3].ToString();
                    unit.Lv = curData[4].ToString();
                    unit.Doc = curData[5].ToString();
                    unit.Icon = ExporterUtils.GetInt(curData[6].ToString());
                    
                    data.Data.Add(unit);
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigAptitudeUnlock>(data, strPathOut);

            }
        }
    }
}
