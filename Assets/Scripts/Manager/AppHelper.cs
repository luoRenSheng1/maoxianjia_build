using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public static class AppHelper
    {
        private static AppConstData presettings = null;

        public static AppConstData GetAppPresettings()
        {
            if (null == presettings)
            {
                presettings = new AppConstData();
            }

            var assetVersion = Resources.Load<TextAsset>("Empty/Version");

            if (assetVersion != null)
            {
                presettings.LocalServerList.Clear();

                string strValue = assetVersion.text;
                strValue = strValue.Replace("\r", "");
                string[] strLines = strValue.Split('\n');

                for (int nIndex = 0; nIndex < strLines.Length; ++nIndex)
                {
                    string strFileDataLine = strLines[nIndex];
                    string[] strFileDataLines = strFileDataLine.Split('=');

                    if (strFileDataLines != null && strFileDataLines.Length > 1)
                    {
                        if (strFileDataLines[0].StartsWith("Server"))
                        {
                            var serverLines = strFileDataLines[1].Split(',');

                            if (serverLines != null && serverLines.Length > 1)
                            {
                                ServerInfo info = new ServerInfo();

                                info.name = serverLines[0];
                                info.url = serverLines[1];

                                presettings.LocalServerList.Add(info);
                            }
                        }
                    }
                }
            }

            return presettings;
        }
    }
}
