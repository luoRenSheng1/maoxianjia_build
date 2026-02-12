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
    public class ConfigBossTraitHelper
    {
        private static string s_ClassNameRaw = "BossTrait";
        private static string s_ClassName = "ConfigBossTrait";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigBossTrait FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigBossTrait data = new ConfigBossTrait();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigBossTraitUnit unit = new ConfigBossTraitUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.Clock = curData[3].ToString();
                    unit.Icon = ExporterUtils.GetInt(curData[4].ToString());
                    unit.DescriptionNote = curData[5].ToString();
                    unit.Description = curData[6].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigBossTrait Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigBossTrait>(data, strPathOut);

            }
        }
    }
}
