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
    public class ConfigPetBasisHelper
    {
        private static string s_ClassNameRaw = "PetBasis";
        private static string s_ClassName = "ConfigPetBasis";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigPetBasis FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigPetBasis data = new ConfigPetBasis();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigPetBasisUnit unit = new ConfigPetBasisUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.Type = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Quality = ExporterUtils.GetInt(curData[4].ToString());
                    unit.AttackSkillId = ExporterUtils.GetInt(curData[5].ToString());
                    unit.SkillGroupId = ExporterUtils.GetInt(curData[6].ToString());
                    unit.StoryNote = curData[7].ToString();
                    unit.Story = curData[8].ToString();
                    unit.IconPath = curData[9].ToString();
                    unit.FullBodyImage = curData[10].ToString();
                    unit.PetModel = curData[11].ToString();
                    unit.Inherit = ExporterUtils.GetInt(curData[12].ToString());
                    unit.Floor = ExporterUtils.GetInt(curData[13].ToString());
                    unit.Upper = ExporterUtils.GetInt(curData[14].ToString());
                    unit.AtkSpeed = ExporterUtils.GetInt(curData[15].ToString());
                    unit.ModelScale = ExporterUtils.GetInt(curData[16].ToString());
                    ExporterUtils.GetArrayInt(unit.PetQuotesId, curData[17].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigPetBasis Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigPetBasis>(data, strPathOut);

            }
        }
    }
}
