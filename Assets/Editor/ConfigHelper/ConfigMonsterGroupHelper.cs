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
    public class ConfigMonsterGroupHelper
    {
        private static string s_ClassNameRaw = "MonsterGroup";
        private static string s_ClassName = "ConfigMonsterGroup";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigMonsterGroup FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigMonsterGroup data = new ConfigMonsterGroup();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigMonsterGroupUnit unit = new ConfigMonsterGroupUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.GroupId = ExporterUtils.GetInt(curData[1].ToString());
                    unit.MonsterId = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Num = ExporterUtils.GetInt(curData[3].ToString());
                    unit.IsBoss = curData[4].ToString();
                    unit.Remark = curData[5].ToString();
                    unit.AtkSkill = ExporterUtils.GetInt(curData[6].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigMonsterGroup Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigMonsterGroup>(data, strPathOut);

            }
        }
    }
}
