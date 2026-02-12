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
    public class ConfigMonsterHelper
    {
        private static string s_ClassNameRaw = "Monster";
        private static string s_ClassName = "ConfigMonster";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigMonster FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigMonster data = new ConfigMonster();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigMonsterUnit unit = new ConfigMonsterUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.HarmType = ExporterUtils.GetInt(curData[3].ToString());
                    unit.AtkSkill = ExporterUtils.GetInt(curData[4].ToString());
                    unit.MonsterBody = curData[5].ToString();
                    unit.Model = curData[6].ToString();
                    unit.AtkSpeed = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Speed = ExporterUtils.GetInt(curData[8].ToString());
                    unit.Size = ExporterUtils.GetInt(curData[9].ToString());
                    unit.BossSize = ExporterUtils.GetInt(curData[10].ToString());
                    unit.HpShifting = ExporterUtils.GetInt(curData[11].ToString());
                    unit.PetBasis = ExporterUtils.GetInt(curData[12].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigMonster Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigMonster>(data, strPathOut);

            }
        }
    }
}
