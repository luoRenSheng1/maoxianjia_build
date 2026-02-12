using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Engine
{
    public class CRetryHttpGetUnit
    {
        public string url = "";
        public string retryTip = "";
        public EN_LOAD_TYPE loadType = EN_LOAD_TYPE.INIT;
        public byte[] bys = null;
    }
}