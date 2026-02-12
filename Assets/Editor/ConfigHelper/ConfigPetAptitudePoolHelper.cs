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
    public class ConfigPetAptitudePoolHelper
    {
        private static string s_ClassNameRaw = "PetAptitudePool";
        private static string s_ClassName = "ConfigPetAptitudePool";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigPetAptitudePool FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigPetAptitudePool data = new ConfigPetAptitudePool();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigPetAptitudePoolUnit unit = new ConfigPetAptitudePoolUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[1].ToString());
                    unit.AptitudeId = ExporterUtils.GetInt(curData[2].ToString());
                    unit.AptitudeLV = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Rate = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Notes1 = curData[5].ToString();
                    unit.Notes1Note = curData[6].ToString();
                    unit.Notes2 = curData[7].ToString();
                    unit.Notes2Note = curData[8].ToString();
                    unit.Common = curData[9].ToString();
                    unit.Interval = curData[10].ToString();
                    unit.HandleObject = ExporterUtils.GetInt(curData[11].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigPetAptitudePool Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigPetAptitudePool>(data, strPathOut);

            }
        }
    }
}
