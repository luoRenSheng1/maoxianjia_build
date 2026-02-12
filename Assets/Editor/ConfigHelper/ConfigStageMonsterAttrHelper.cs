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
    public class ConfigStageMonsterAttrHelper
    {
        private static string s_ClassNameRaw = "StageMonsterAttr";
        private static string s_ClassName = "ConfigStageMonsterAttr";

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
                int nFieldNum = 18;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigStageMonsterAttr FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigStageMonsterAttr data = new ConfigStageMonsterAttr();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigStageMonsterAttrUnit unit = new ConfigStageMonsterAttrUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Chapter = ExporterUtils.GetInt(curData[1].ToString());
                    unit.LevelId = ExporterUtils.GetInt(curData[2].ToString());
                    unit.NameNote = curData[3].ToString();
                    unit.Node = ExporterUtils.GetInt(curData[4].ToString());
                    unit.MonsterData = ExporterUtils.GetInt(curData[5].ToString());
                    unit.HarmType = ExporterUtils.GetInt(curData[6].ToString());
                    unit.EntryNumber = ExporterUtils.GetInt(curData[7].ToString());
                    unit.EntryGroup = ExporterUtils.GetInt(curData[8].ToString());
                    unit.PhysicalAtk = curData[9].ToString();
                    unit.MagicAtk = curData[10].ToString();
                    unit.SorceryAtk = curData[11].ToString();
                    unit.PhysicalDef = curData[12].ToString();
                    unit.MagicDef = curData[13].ToString();
                    unit.SorceryDef = curData[14].ToString();
                    unit.Hp = curData[15].ToString();
                    unit.Reply = curData[16].ToString();
                    unit.Note1 = curData[17].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigStageMonsterAttr Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigStageMonsterAttr>(data, strPathOut);

            }
        }
    }
}
