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
    public class ConfigSkillEffectHelper
    {
        private static string s_ClassNameRaw = "SkillEffect";
        private static string s_ClassName = "ConfigSkillEffect";

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
                int nFieldNum = 17;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigSkillEffect FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigSkillEffect data = new ConfigSkillEffect();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigSkillEffectUnit unit = new ConfigSkillEffectUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Remark = curData[1].ToString();
                    unit.Type = ExporterUtils.GetInt(curData[2].ToString());
                    unit.TypeData = curData[3].ToString();
                    unit.Path = curData[4].ToString();
                    unit.Ani = curData[5].ToString();
                    unit.Loop = ExporterUtils.GetInt(curData[6].ToString());
                    unit.Size = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Delay = ExporterUtils.GetInt(curData[8].ToString());
                    unit.Time = ExporterUtils.GetInt(curData[9].ToString());
                    unit.Attach = ExporterUtils.GetInt(curData[10].ToString());
                    unit.Bone = curData[11].ToString();
                    unit.TargetBone = curData[12].ToString();
                    unit.Speed = ExporterUtils.GetInt(curData[13].ToString());
                    unit.Endfx = ExporterUtils.GetInt(curData[14].ToString());
                    unit.Endsound = ExporterUtils.GetInt(curData[15].ToString());
                    unit.AniScale = ExporterUtils.GetInt(curData[16].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigSkillEffect Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigSkillEffect>(data, strPathOut);

            }
        }
    }
}
