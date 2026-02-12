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
    public class ConfigDungeonStageHelper
    {
        private static string s_ClassNameRaw = "DungeonStage";
        private static string s_ClassName = "ConfigDungeonStage";

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
                int nFieldNum = 20;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigDungeonStage FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigDungeonStage data = new ConfigDungeonStage();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigDungeonStageUnit unit = new ConfigDungeonStageUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.NameParam = curData[3].ToString();
                    unit.Type = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Stage = ExporterUtils.GetInt(curData[5].ToString());
                    ExporterUtils.GetArrayInt(unit.MonsterData, curData[6].ToString());
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
                    unit.PassReward = curData[19].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigDungeonStage Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigDungeonStage>(data, strPathOut);

            }
        }
    }
}
