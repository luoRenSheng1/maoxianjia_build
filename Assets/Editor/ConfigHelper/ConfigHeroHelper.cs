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
    public class ConfigHeroHelper
    {
        private static string s_ClassNameRaw = "Hero";
        private static string s_ClassName = "ConfigHero";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigHero FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigHero data = new ConfigHero();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigHeroUnit unit = new ConfigHeroUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.Vocation = ExporterUtils.GetInt(curData[3].ToString());
                    unit.HarmType = ExporterUtils.GetInt(curData[4].ToString());
                    unit.VocationAttr = ExporterUtils.GetInt(curData[5].ToString());
                    unit.HeroQuality = ExporterUtils.GetInt(curData[6].ToString());
                    unit.AtkSkill = ExporterUtils.GetInt(curData[7].ToString());
                    unit.ActiveSkill = ExporterUtils.GetInt(curData[8].ToString());
                    unit.Passive = curData[9].ToString();
                    unit.HeroBody = curData[10].ToString();
                    unit.Model = curData[11].ToString();
                    unit.Model1 = curData[12].ToString();
                    unit.AtkSpeed = ExporterUtils.GetInt(curData[13].ToString());
                    unit.Speed = ExporterUtils.GetInt(curData[14].ToString());
                    unit.ModelScale = ExporterUtils.GetInt(curData[15].ToString());
                    unit.Item = ExporterUtils.GetInt(curData[16].ToString());
                    unit.Conflate = ExporterUtils.GetInt(curData[17].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigHero Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigHero>(data, strPathOut);

            }
        }
    }
}
