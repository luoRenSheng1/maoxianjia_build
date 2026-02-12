using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Engine
{
    internal static class StringBuilderCache
    {
        // The value 360 was chosen in discussion with performance experts as a compromise between using
        // as litle memory (per thread) as possible and still covering a large part of short-lived
        // StringBuilder creations on the startup path of VS designers.
        private const int MAX_BUILDER_SIZE = 360;

        [ThreadStatic]
        private static StringBuilder CachedInstance;

        public static StringBuilder Acquire(int capacity = 16 /*StringBuilder.DefaultCapacity*/)
        {
            if(capacity <= MAX_BUILDER_SIZE)
            {
                StringBuilder sb = StringBuilderCache.CachedInstance;
                if (sb != null)
                {
                    // Avoid stringbuilder block fragmentation by getting a new StringBuilder
                    // when the requested size is larger than the current capacity
                    if(capacity <= sb.Capacity)
                    {
                        StringBuilderCache.CachedInstance = null;
                        sb.Clear();
                        return sb;
                    }
                }
            }
            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder sb)
        {
            if (sb.Capacity <= MAX_BUILDER_SIZE)
            {
                StringBuilderCache.CachedInstance = sb;
            }
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            string result = sb.ToString();
            Release(sb);
            return result;
        }
    }
    
    public static class StringUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <returns></returns>
        public static string ToStringAndCacheInst(this StringBuilder stringBuilder)
        {
            return StringBuilderCache.GetStringAndRelease(stringBuilder);;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Concat(params string[] value)
        {
            if (value.Length > 0)
            {
                var stringBuilder = StringBuilderCache.Acquire();
                for (int i = 0; i < value.Length; i++)
                {
                    var txt = value[i];
                    stringBuilder.Append(txt);
                }
                return stringBuilder.ToStringAndCacheInst();
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(string format, params object[] args)
        {
            try
            {
                var stringBuilder = StringBuilderCache.Acquire();
                stringBuilder.AppendFormat(format, args);
                return stringBuilder.ToStringAndCacheInst();
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
                return format;
            }
        }
        
        
        /// <summary>
        /// todo 属性ID转换为属性名称
        /// </summary>
        /// <param name="attrId"></param>
        /// <returns></returns>
        public static string ConvertToAttributeName(int attrId)
        {
            switch (attrId)
            {
                case (int)EN_BUFF_ADD_TYPE.ATK:
                    return ConfigUtils.GetStringByKey(StringDefine.STRING_ATTACK);
                case (int)EN_BUFF_ADD_TYPE.HP:
                    return ConfigUtils.GetStringByKey(StringDefine.STRING_LIFE);
                case (int)EN_BUFF_ADD_TYPE.AtkSpeed:
                    return ConfigUtils.GetStringByKey(StringDefine.STRING_SUDU);
                case (int)EN_BUFF_ADD_TYPE.CriticalStrike:
                    return ConfigUtils.GetStringByKey(2031);
                case (int)EN_BUFF_ADD_TYPE.ComboAtk:
                    return ConfigUtils.GetStringByKey(2032);
                case (int)EN_BUFF_ADD_TYPE.CounterAtk:
                    return ConfigUtils.GetStringByKey(2033);
                case (int)EN_BUFF_ADD_TYPE.Recovery:
                    return ConfigUtils.GetStringByKey(2034);
                case (int)EN_BUFF_ADD_TYPE.CriticalInjury:
                    return ConfigUtils.GetStringByKey(2035);
                case (int)EN_BUFF_ADD_TYPE.HP_ADD:
                    return ConfigUtils.GetStringByKey(2023);
                // case (int)EN_BUFF_ADD_TYPE.ATK_ADD:
                //     return ConfigUtils.GetStringByKey(2018);
                case (int)EN_BUFF_ADD_TYPE.BossDamageAdd:
                    return ConfigUtils.GetStringByKey(2036);
                case (int)EN_BUFF_ADD_TYPE.MonsterDamageAdd:
                    return ConfigUtils.GetStringByKey(2037);
                case (int)EN_BUFF_ADD_TYPE.Mitigation:
                    return ConfigUtils.GetStringByKey(2038);
                case (int)EN_BUFF_ADD_TYPE.GoldAdd:
                    return ConfigUtils.GetStringByKey(2039);
                case (int)EN_BUFF_ADD_TYPE.SkillCd:
                    return ConfigUtils.GetStringByKey(2040);
                case (int)EN_BUFF_ADD_TYPE.Bloodsucking:
                    return ConfigUtils.GetStringByKey(2019);
            }

            return "";
        }
        
        public static string GetTimeString(int totalSecond)
        {
            int second = totalSecond % 60;
            int totlaMinute = totalSecond / 60;
            int minute = totlaMinute % 60;
            int hour = totlaMinute / 60%24;
            int day = totlaMinute / 1440;
            string rtn = "";
            if (day > 0)
            {
                 rtn = ConfigUtils.FormatStringByKey(37, day, hour);//string.Format("{0}", day.ToString());
            }
            else if (hour > 0)
            {
                rtn = ConfigUtils.FormatStringByKey(34, hour, minute.ToString("d2"));//string.Format("{0}:{1}:{2}", hour.ToString("d2"), minute.ToString("d2"), second.ToString("d2"));
            }
            else
            {
                rtn = ConfigUtils.FormatStringByKey(39, minute.ToString("d2"), second.ToString("d2"));//string.Format("{0}:{1}", totlaMinute.ToString("d2"), second.ToString("d2"));
            }
            return rtn;
        }
        
        public static string GetTimeString2(int totalSecond)
        {
            int second = totalSecond % 60;
            int totlaMinute = totalSecond / 60;
            int minute = totlaMinute % 60;
            int hour = totlaMinute / 60%24;
            int day = totlaMinute / 1440;
            string rtn = "";
            if (day > 0)
            {
                rtn = ConfigUtils.FormatStringByKey(37, day, hour);
            }
            else if (hour > 0)
            {
                rtn = ConfigUtils.FormatStringByKey(38, hour);
            }
            else
            {
                rtn = ConfigUtils.FormatStringByKey(35, totlaMinute, second);
            }
            return rtn;
        }

        //一般是单独配一个单位表 读表获取
        static string[] unitList = new string[] { "", "K", "M", "G" };
        static Regex reg = new Regex(@"^\d+\.\d+$");

        /// <summary>
        /// 格式化货币
        /// </summary>
        /// digit:保留几位小数
        public static string FormatCurrency(double num, int digit = 1)
        {
            num = Convert.ToDouble(num.ToString("0.0000"));
            if (num <= 99999)
            {
                num = double.Parse(num.ToString("f5"));
                if (reg.IsMatch(num.ToString()))
                {
                    return num.ToString("f2");
                }
                else
                {
                    return num.ToString();
                }
            }
            
            return num.ToLargeNum();
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// digits:保留几位小数
        public static float Round(float value, int digits = 1)
        {
            float multiple = Mathf.Pow(10, digits);
            float tempValue = value * multiple + 0.5f;
            tempValue = Mathf.FloorToInt(tempValue);
            return tempValue / multiple;
        }

        public static int FormatAfterTime(int seconds)
        {
            if (seconds <= 86400)
            {
                return 1;
            }
            else
            {
                return Mathf.CeilToInt(seconds / 86400f);
            }
        }

        public static string FormatTimeDifference(int seconds)  
        {  
            // 定义时间单位及其对应的描述  
            var timeUnits = new[]  
            {  
                new { Threshold = 3600 * 24 * 7, Description = "大于7天" , stringID = 10121},  
                new { Threshold = 3600 * 24, Description = "天前", stringID = 10122 },  
                new { Threshold = 3600, Description = "小时前",stringID = 10123 },  
                new { Threshold = 60, Description = "分钟前",stringID = 10124 },  
                new { Threshold = 1, Description = "刚刚",stringID = 10125 }  
            };

            // 遍历时间单位，找到适合当前时间差的描述  
            foreach (var unit in timeUnits)  
            {  
                if (seconds >= unit.Threshold)  
                {  
                    // 计算时间差的数量（向下取整）  
                    int count = seconds / unit.Threshold;  
                    // 如果时间差小于下一个更小单位的阈值，则可能需要调整count（对于分钟和刚刚的情况）  
                    if (unit.Threshold > 1 && seconds % unit.Threshold != 0)  
                    {  
                        // 但在这个特定情况下，我们不需要调整，因为我们已经决定向下取整  
                        // 对于“刚刚”的情况，count会被忽略  
                    }  
  
                    // 根据时间差的数量和描述模板生成描述字符串  
                    // 注意：对于“刚刚”，我们不需要数量词  
                    if (unit.stringID == 10125)
                    {
                        return ConfigUtils.GetStringByKey(10125);
                    }else if (unit.stringID == 10121)
                    {
                        return ConfigUtils.GetStringByKey(10121);
                    }
                    else  
                    {  
                        return $"{count} {ConfigUtils.GetStringByKey(unit.stringID)}";  
                    }  
                }  
            }  
  
            // 如果时间差小于1秒（理论上不应该发生，因为seconds是int且最小为1），但为了完整性  
            return "未知时间";  
        }  

        public static string ConvertToAttributeValue(int attrId, double attrValue)
        {
            switch (attrId)
            {
                case (int)EN_BUFF_ADD_TYPE.PhysicAtk:
                case (int)EN_BUFF_ADD_TYPE.MagicAtk:
                case (int)EN_BUFF_ADD_TYPE.SorceryAtk:
                case (int)EN_BUFF_ADD_TYPE.HP:
                case (int)EN_BUFF_ADD_TYPE.PhysicDef:
                case (int)EN_BUFF_ADD_TYPE.MagicDef:
                case (int)EN_BUFF_ADD_TYPE.SorceryDef:
                case (int)EN_BUFF_ADD_TYPE.Recovery:
                case (int)EN_BUFF_ADD_TYPE.PetAtk:
                case (int)EN_BUFF_ADD_TYPE.ParryValue:
                case (int)EN_BUFF_ADD_TYPE.AtkHPRecovery:
                case (int)EN_BUFF_ADD_TYPE.MagicTimes:
                case (int)EN_BUFF_ADD_TYPE.OnlineAwardTimes:
                case (int)EN_BUFF_ADD_TYPE.HomeLimitMaxTime:
                case (int)EN_BUFF_ADD_TYPE.KilledRecovery:
                case (int)EN_BUFF_ADD_TYPE.ATK:
                case (int)EN_BUFF_ADD_TYPE.Def:
                    return attrValue.ToString("f2");
                case (int)EN_BUFF_ADD_TYPE.PhysicAtkADD:
                case (int)EN_BUFF_ADD_TYPE.MagicAtkADD:
                case (int)EN_BUFF_ADD_TYPE.SorceryAtkADD:
                case (int)EN_BUFF_ADD_TYPE.HP_ADD:
                case (int)EN_BUFF_ADD_TYPE.PhysicDefADD:
                case (int)EN_BUFF_ADD_TYPE.MagicDefADD:
                case (int)EN_BUFF_ADD_TYPE.SorceryDefADD:
                case (int)EN_BUFF_ADD_TYPE.EarthAtkADD:
                case (int)EN_BUFF_ADD_TYPE.WaterAtkADD:
                case (int)EN_BUFF_ADD_TYPE.FireAtkADD:
                case (int)EN_BUFF_ADD_TYPE.AirAtkADD:
                case (int)EN_BUFF_ADD_TYPE.PetAtkADD:
                case (int)EN_BUFF_ADD_TYPE.HPMultiple:
                case (int)EN_BUFF_ADD_TYPE.AtkMultiple:
                case (int)EN_BUFF_ADD_TYPE.JoukRate:
                case (int)EN_BUFF_ADD_TYPE.AtkHitRate:
                case (int)EN_BUFF_ADD_TYPE.ParryRate:
                case (int)EN_BUFF_ADD_TYPE.IgnoreDef:
                case (int)EN_BUFF_ADD_TYPE.CriticalStrike:
                case (int)EN_BUFF_ADD_TYPE.CriticalInjury:
                case (int)EN_BUFF_ADD_TYPE.BossDamageAdd:
                case (int)EN_BUFF_ADD_TYPE.MonsterDamageAdd:
                case (int)EN_BUFF_ADD_TYPE.Mitigation:
                case (int)EN_BUFF_ADD_TYPE.Bloodsucking:
                case (int)EN_BUFF_ADD_TYPE.SkillDamage:
                case (int)EN_BUFF_ADD_TYPE.MagicTimesAdd:
                case (int)EN_BUFF_ADD_TYPE.GoldAdd:
                case (int)EN_BUFF_ADD_TYPE.AtkSpeed:
                case (int)EN_BUFF_ADD_TYPE.ComboAtk:
                case (int)EN_BUFF_ADD_TYPE.CounterAtk:
                case (int)EN_BUFF_ADD_TYPE.LoreEquipRate:
                case (int)EN_BUFF_ADD_TYPE.DropLoreEquipRate:
                case (int)EN_BUFF_ADD_TYPE.BattleFinalAttack:
                case (int)EN_BUFF_ADD_TYPE.SkillCd:
                    return (attrValue * ConstDefine.CONFIG_PLACE_EX).ToString("f2");
            }

            return "";
        }
    }
}