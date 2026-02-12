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
    public class ConfigSkillTargetHelper
    {
        private static string s_ClassNameRaw = "SkillTarget";
        private static string s_ClassName = "ConfigSkillTarget";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigSkillTarget FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigSkillTarget data = new ConfigSkillTarget();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigSkillTargetUnit unit = new ConfigSkillTargetUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.RangeType = ExporterUtils.GetInt(curData[1].ToString());
                    unit.RangeRefer = ExporterUtils.GetInt(curData[2].ToString());
                    unit.RangeData = ExporterUtils.GetInt(curData[3].ToString());
                    unit.TargetType = ExporterUtils.GetInt(curData[4].ToString());
                    unit.TargetPriority = ExporterUtils.GetInt(curData[5].ToString());
                    unit.RangeParam = ExporterUtils.GetInt(curData[6].ToString());
                    unit.TargetNumber = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Remarks = curData[8].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigSkillTarget Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigSkillTarget>(data, strPathOut);

            }
        }
    }
}
