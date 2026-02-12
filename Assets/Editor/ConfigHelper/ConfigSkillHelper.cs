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
    public class ConfigSkillHelper
    {
        private static string s_ClassNameRaw = "Skill";
        private static string s_ClassName = "ConfigSkill";

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
                int nFieldNum = 26;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigSkill FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigSkill data = new ConfigSkill();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigSkillUnit unit = new ConfigSkillUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.NameNote = curData[1].ToString();
                    unit.Name = curData[2].ToString();
                    unit.SkillType = ExporterUtils.GetInt(curData[3].ToString());
                    unit.SkillQuality = ExporterUtils.GetInt(curData[4].ToString());
                    unit.SkillTarget = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Radius = ExporterUtils.GetInt(curData[6].ToString());
                    unit.SkillDamage = ExporterUtils.GetInt(curData[7].ToString());
                    unit.BuffId = curData[8].ToString();
                    unit.Cd = ExporterUtils.GetInt(curData[9].ToString());
                    unit.SkillIcon = curData[10].ToString();
                    unit.SkillDesNote = curData[11].ToString();
                    unit.SkillDes = curData[12].ToString();
                    unit.RemarkNote = curData[13].ToString();
                    unit.Remark = curData[14].ToString();
                    unit.RemarkParam = curData[15].ToString();
                    unit.SkillAction = curData[16].ToString();
                    ExporterUtils.GetArrayInt(unit.CasterEffect, curData[17].ToString());
                    unit.AttackEffect = ExporterUtils.GetInt(curData[18].ToString());
                    unit.AttackTime = ExporterUtils.GetInt(curData[19].ToString());
                    unit.HitEffect = ExporterUtils.GetInt(curData[20].ToString());
                    ExporterUtils.GetArrayInt(unit.HitTime, curData[21].ToString());
                    unit.BulletEffect = ExporterUtils.GetInt(curData[22].ToString());
                    unit.BulletTime = ExporterUtils.GetInt(curData[23].ToString());
                    unit.BulletFlyTime = ExporterUtils.GetInt(curData[24].ToString());
                    unit.StartSound = ExporterUtils.GetInt(curData[25].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigSkill Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigSkill>(data, strPathOut);

            }
        }
    }
}
