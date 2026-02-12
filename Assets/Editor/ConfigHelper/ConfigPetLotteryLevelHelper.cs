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
    public class ConfigPetLotteryLevelHelper
    {
        private static string s_ClassNameRaw = "PetLotteryLevel";
        private static string s_ClassName = "ConfigPetLotteryLevel";

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
                int nFieldNum = 14;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigPetLotteryLevel FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigPetLotteryLevel data = new ConfigPetLotteryLevel();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigPetLotteryLevelUnit unit = new ConfigPetLotteryLevelUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Level = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Quality1Pro = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Quality2Pro = ExporterUtils.GetInt(curData[3].ToString());
                    unit.Quality3Pro = ExporterUtils.GetInt(curData[4].ToString());
                    unit.Quality4Pro = ExporterUtils.GetInt(curData[5].ToString());
                    unit.Quality5Pro = ExporterUtils.GetInt(curData[6].ToString());
                    unit.Quality6Pro = ExporterUtils.GetInt(curData[7].ToString());
                    unit.Quality7Pro = ExporterUtils.GetInt(curData[8].ToString());
                    unit.Quality8Pro = ExporterUtils.GetInt(curData[9].ToString());
                    unit.Quality9Pro = ExporterUtils.GetInt(curData[10].ToString());
                    unit.Quality10Pro = ExporterUtils.GetInt(curData[11].ToString());
                    unit.Quality11Pro = ExporterUtils.GetInt(curData[12].ToString());
                    unit.Exp = ExporterUtils.GetInt(curData[13].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigPetLotteryLevel Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigPetLotteryLevel>(data, strPathOut);

            }
        }
    }
}
