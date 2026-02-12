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
    public class ConfigSkillAchieveHelper
    {
        private static string s_ClassNameRaw = "SkillAchieve";
        private static string s_ClassName = "ConfigSkillAchieve";

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
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigSkillAchieve FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigSkillAchieve data = new ConfigSkillAchieve();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigSkillAchieveUnit unit = new ConfigSkillAchieveUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[1].ToString());
                    unit.DevSide = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Name = curData[3].ToString();
                    unit.Doc = curData[4].ToString();
                    unit.AttrOrAction = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Trigger = ExporterUtils.GetInt(curData[6].ToString());
                    unit.TriggerValue = ExporterUtils.GetInt(curData[7].ToString());
                    unit.HandleObject = ExporterUtils.GetInt(curData[8].ToString());
                    unit.InvalidCondition = ExporterUtils.GetInt(curData[9].ToString());
                    unit.LastTime = ExporterUtils.GetFloat(curData[10].ToString());
                    unit.EffectTime = ExporterUtils.GetFloat(curData[11].ToString());
                    unit.OpFrequency = ExporterUtils.GetInt(curData[12].ToString());
                    unit.CommonAttr = curData[13].ToString();
                    unit.ValueChangeType = ExporterUtils.GetInt(curData[14].ToString());
                    unit.ActID = ExporterUtils.GetInt(curData[15].ToString());
                    unit.CoverLastTime = ExporterUtils.GetFloat(curData[16].ToString());
                    unit.CoverEffectTime = ExporterUtils.GetFloat(curData[17].ToString());
                    unit.COpFrequency = ExporterUtils.GetInt(curData[18].ToString());
                    unit.Ruantity = ExporterUtils.GetInt(curData[19].ToString());
                    unit.CoverCommonParam = curData[20].ToString();
                    unit.BuffEffect = ExporterUtils.GetInt(curData[21].ToString());
                    unit.BuffTime = ExporterUtils.GetInt(curData[22].ToString());
                    unit.Item = ExporterUtils.GetInt(curData[23].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigSkillAchieve Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigSkillAchieve>(data, strPathOut);

            }
        }
    }
}
