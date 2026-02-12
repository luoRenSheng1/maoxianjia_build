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
    public class ConfigExplorationTasksHelper
    {
        private static string s_ClassNameRaw = "ExplorationTasks";
        private static string s_ClassName = "ConfigExplorationTasks";

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
                int nFieldNum = 13;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigExplorationTasks FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigExplorationTasks data = new ConfigExplorationTasks();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigExplorationTasksUnit unit = new ConfigExplorationTasksUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Quality = ExporterUtils.GetInt(curData[1].ToString());
                    unit.ExplorationTasksGroup = ExporterUtils.GetInt(curData[2].ToString());
                    unit.TaskPro = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Time = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Model = curData[5].ToString();
                    unit.Desc = curData[6].ToString();
                    ExporterUtils.GetArrayInt(unit.ReqOccupation, curData[7].ToString());
                    ExporterUtils.GetArrayInt(unit.OccupationPro, curData[8].ToString());
                    unit.Power = ExporterUtils.GetInt(curData[9].ToString());
                    unit.PowerPro = ExporterUtils.GetInt(curData[10].ToString());
                    unit.PhysicalExertion = ExporterUtils.GetInt(curData[11].ToString());
                    unit.Reward = curData[12].ToString();
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigExplorationTasks Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigExplorationTasks>(data, strPathOut);

            }
        }
    }
}
