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
    public class ConfigRaffleLevelHelper
    {
        private static string s_ClassNameRaw = "RaffleLevel";
        private static string s_ClassName = "ConfigRaffleLevel";

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
                int nFieldNum = 11;
                
                if (listDataName.Count != nFieldNum)
                {
                     Debug.LogError("[XlsxToBin]	doXlsxToBin Fail: ConfigRaffleLevel FieldNum NotMatch With Excel, Please Check.");
                     return;
                }
                
                ConfigRaffleLevel data = new ConfigRaffleLevel();
                
                for (int nIndex = 3; nIndex < listData.Count; ++nIndex)
                {
                    List<object> curData = listData[nIndex];
                    
                    if (curData == null)
                    {
                        continue;
                    }
                    
                    ConfigRaffleLevelUnit unit = new ConfigRaffleLevelUnit();
                    
                    unit.Id = ExporterUtils.GetInt(curData[0].ToString());
                    unit.Type = ExporterUtils.GetInt(curData[1].ToString());
                    unit.Level = ExporterUtils.GetInt(curData[2].ToString());
                    unit.Quality1Pro = curData[3].ToString();
                    unit.Quality2Pro = curData[4].ToString();
                    unit.Quality3Pro = curData[5].ToString();
                    unit.Quality4Pro = curData[6].ToString();
                    unit.Quality5Pro = curData[7].ToString();
                    unit.Quality6Pro = curData[8].ToString();
                    unit.Quality7Pro = curData[9].ToString();
                    unit.Exp = ExporterUtils.GetInt(curData[10].ToString());
                    
                    if (data.Data.ContainsKey(unit.Id))
                    {
                        Debug.LogError("[XlsxToBin] doXlsxToBin Error: ConfigRaffleLevel Repeat Key, Please Check. " + unit.Id);
                    }
                    
                    data.Data[unit.Id] = unit;
                }
                
                EngineBase.ConfigSerializer.Serialize<ConfigRaffleLevel>(data, strPathOut);

            }
        }
    }
}
