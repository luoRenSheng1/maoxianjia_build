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
    public class ConfigEventStageHelper
    {
        private static string s_ClassNameRaw = "EventStage";
        private static string s_ClassName = "ConfigEventStage";

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
                int nFieldNum = 24;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigEventStage FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigEventStage data = new ConfigEventStage();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigEventStageUnit unit = new ConfigEventStageUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.Type = ExporterUtils.GetInt(curData[3].ToString());
                    unit.ChapterId = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Rate = ExporterUtils.GetInt(curData[5].ToString());
                    unit.MonsterData = ExporterUtils.GetInt(curData[6].ToString());
                    unit.HarmType = ExporterUtils.GetInt(curData[7].ToString());
                    unit.EntryNumber = ExporterUtils.GetInt(curData[8].ToString());
                    unit.EntryGroup = ExporterUtils.GetInt(curData[9].ToString());
                    unit.PhysicalAtk = curData[10].ToString();
                    unit.MagicAtk = curData[11].ToString();
                    unit.SorceryAtk = curData[12].ToString();
                    unit.PhysicalDef = curData[13].ToString();
                    unit.MagicDef = curData[14].ToString();
                    unit.SorceryDef = curData[15].ToString();
                    unit.Hp = curData[16].ToString();
                    unit.Reply = curData[17].ToString();
                    unit.Time = ExporterUtils.GetInt(curData[18].ToString());
                    unit.Lv = ExporterUtils.GetInt(curData[19].ToString());
                    unit.LoreRate = ExporterUtils.GetInt(curData[20].ToString());
                    unit.Number = curData[21].ToString();
                    unit.Drop = ExporterUtils.GetInt(curData[22].ToString());
                    unit.Note1 = curData[23].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigEventStage Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigEventStage>(data, strPathOut);

            }
        }
    }
}
