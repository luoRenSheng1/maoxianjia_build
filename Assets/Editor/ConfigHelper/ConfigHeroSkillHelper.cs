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
    public class ConfigHeroSkillHelper
    {
        private static string s_ClassNameRaw = "HeroSkill";
        private static string s_ClassName = "ConfigHeroSkill";

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
                int nFieldNum = 8;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigHeroSkill FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigHeroSkill data = new ConfigHeroSkill();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigHeroSkillUnit unit = new ConfigHeroSkillUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Hero = ExporterUtils.GetInt(curData[1].ToString());
                    unit.SkillNameNote = curData[2].ToString();
                    unit.SkillName = curData[3].ToString();
                    unit.SkillLevel = ExporterUtils.GetInt(curData[4].ToString());
                    unit.SkillDamage = ExporterUtils.GetInt(curData[5].ToString());
                    unit.SkillGem = ExporterUtils.GetInt(curData[6].ToString());
                    unit.Gold = ExporterUtils.GetInt(curData[7].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigHeroSkill Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigHeroSkill>(data, strPathOut);

            }
        }
    }
}
