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
    public class ConfigPetSkillGroupHelper
    {
        private static string s_ClassNameRaw = "PetSkillGroup";
        private static string s_ClassName = "ConfigPetSkillGroup";

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
                int nFieldNum = 9;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigPetSkillGroup FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigPetSkillGroup data = new ConfigPetSkillGroup();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigPetSkillGroupUnit unit = new ConfigPetSkillGroupUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Quality = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Group = ExporterUtils.GetInt(curData[3].ToString());
                    unit.SkillId = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Rate = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Item = ExporterUtils.GetInt(curData[6].ToString());
                    unit.NotesName = curData[7].ToString();
                    unit.NotesCommon = curData[8].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigPetSkillGroup Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigPetSkillGroup>(data, strPathOut);

            }
        }
    }
}
