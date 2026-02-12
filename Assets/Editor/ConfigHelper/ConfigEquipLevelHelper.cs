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
    public class ConfigEquipLevelHelper
    {
        private static string s_ClassNameRaw = "EquipLevel";
        private static string s_ClassName = "ConfigEquipLevel";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigEquipLevel FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigEquipLevel data = new ConfigEquipLevel();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigEquipLevelUnit unit = new ConfigEquipLevelUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.EquipQuality = ExporterUtils.GetInt(curData[1].ToString());
                    unit.EquipLevel = ExporterUtils.GetInt(curData[2].ToString());
                    unit.PhysicalAtk = curData[3].ToString();
                    unit.MagicAtk = curData[4].ToString();
                    unit.SorceryAtk = curData[5].ToString();
                    unit.Hp = curData[6].ToString();
                    unit.PhysicalDef = curData[7].ToString();
                    unit.MagicDef = curData[8].ToString();
                    unit.SorceryDef = curData[9].ToString();
                    unit.Reply = curData[10].ToString();
                    unit.AddEntry = ExporterUtils.GetInt(curData[11].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigEquipLevel Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigEquipLevel>(data, strPathOut);

            }
        }
    }
}
