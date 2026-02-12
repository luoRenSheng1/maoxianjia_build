using System;
using System.Collections;
using System.Collections.Generic;
using EngineBase;

namespace Engine
{
    public class ProtocolUtil : TSingleton<ProtocolUtil>
    {
        //Simple type
        private string[] types;
        private Dictionary<string, int> typeMap;

        public override void Init()
        {
            this.initTypeMap();
            this.types = new string[] { "uInt32", "sInt32", "int32", "uInt64", "sInt64", "float", "double" };
        }

        /// <summary>
        /// Check out the given type. If it is simple, return ture.
        /// </summary>
        /// <returns>
        /// The simple type.
        /// </returns>
        /// <param name='type'>
        /// If set to <c>true</c> type.
        /// </param>
        public bool isSimpleType(string type)
        {
            int length = types.Length;
            bool flag = false;
            for (int i = 0; i < length; i++)
            {
                if (type == types[i])
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// Check out the given type. If the type exist in typeMap, return true.
        /// </summary>
        /// <returns>
        /// The type.
        /// </returns>
        /// <param name='type'>
        /// Type.
        /// </param>
        public int containType(string type)
        {
            int value, returnInt = 2;
            if (this.typeMap.TryGetValue(type, out value))
            {
                returnInt = value;
            }
            return returnInt;
        }

        //Init the typeMap
        private void initTypeMap()
        {
            this.typeMap = new Dictionary<string, int>();
            typeMap.Add("uInt32", 0);
            typeMap.Add("sInt32", 0);
            typeMap.Add("int32", 0);
            typeMap.Add("double", 1);
            typeMap.Add("string", 2);
            typeMap.Add("float", 5);
            typeMap.Add("message", 2);
            
        }

        
    }
}