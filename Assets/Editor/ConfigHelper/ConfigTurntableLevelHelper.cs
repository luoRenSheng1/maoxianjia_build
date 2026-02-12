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
    public class ConfigTurntableLevelHelper
    {
        private static string s_ClassNameRaw = "TurntableLevel";
        private static string s_ClassName = "ConfigTurntableLevel";

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
                int nFieldNum = 6;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigTurntableLevel FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigTurntableLevel data = new ConfigTurntableLevel();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigTurntableLevelUnit unit = new ConfigTurntableLevelUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Level = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Exp = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Reward = curData[3].ToString();
                    ExporterUtils.GetArrayInt(unit.BoxRewardsId, curData[4].ToString());
                    ExporterUtils.GetArrayInt(unit.NumReq, curData[5].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigTurntableLevel Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigTurntableLevel>(data, strPathOut);

            }
        }
    }
}
