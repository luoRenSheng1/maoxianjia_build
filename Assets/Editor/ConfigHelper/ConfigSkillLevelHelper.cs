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
    public class ConfigSkillLevelHelper
    {
        private static string s_ClassNameRaw = "SkillLevel";
        private static string s_ClassName = "ConfigSkillLevel";

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
                int nFieldNum = 10;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigSkillLevel FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigSkillLevel data = new ConfigSkillLevel();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigSkillLevelUnit unit = new ConfigSkillLevelUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.SkillId = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Name = curData[2].ToString();
                    unit.SkillLevel = ExporterUtils.GetInt(curData[3].ToString());
                    unit.CardNumber = ExporterUtils.GetInt(curData[4].ToString());
                    unit.SkillValue = ExporterUtils.GetInt(curData[5].ToString());
                    unit.CarryValue = ExporterUtils.GetInt(curData[6].ToString());
                    unit.AttrValue = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Doc = curData[8].ToString();
                    unit.DocNote = curData[9].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigSkillLevel Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigSkillLevel>(data, strPathOut);

            }
        }
    }
}
