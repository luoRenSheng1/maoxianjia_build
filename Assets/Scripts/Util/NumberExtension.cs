namespace Engine
{
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
 
public static class NumberExtension
{
    private static int MONEY = 99999;
    #region 转换大数值
    private static string[] toLargeNumSign = new string[] {"","万", "亿", "兆", "京", "垓", "秭", "穰", "沟", "涧", "正", "载", "极", "恒河沙", "阿僧祗", "那由他", "不可思议", 
        "无量", "大数","大数2","大数3","大数4","大数5","大数6","大数7","大数8","大数9","大数11","大数12","大数13","大数14","大数15","大数16","大数18"};
 
    public static string ToLargeNum(this int value)
    {
        if (value < MONEY && value > -MONEY)
            return value.ToString();
        string str = value.ToString("#");
        return GetToLargeNum(str);
    }
    public static string ToLargeNum(this long value)
    {
        if (value < MONEY && value > -MONEY)
            return value.ToString();
        string str = value.ToString("#");
        return GetToLargeNum(str);
    }
    public static string ToLargeNum(this float value)
    {
        if (value < MONEY && value > -MONEY)
            return value.ToString();
        string str = value.ToString("#");
        return GetToLargeNum(str);
    }
    public static string ToLargeNum(this double value)
    {
        if (value < MONEY && value > -MONEY)
            return value.ToString();
        string str = value.ToString("#");
        return GetToLargeNum(str);
    }
 
    private static string GetToLargeNum(string str)
    {
        if (double.Parse(str) < 99999999)
        {
            int count1 = str.Length;
            StringBuilder valueStr1 = new StringBuilder();
            int dotLeftCount1 = count1 - 4;
            if (dotLeftCount1 == 1)
            {
                valueStr1.Append(str[0]);
                valueStr1.Append('.');
                valueStr1.Append(str[1]);
                valueStr1.Append(str[2]);
            }
            else if (dotLeftCount1 == 2)
            {
                valueStr1.Append(str[0]);
                valueStr1.Append(str[1]);
                valueStr1.Append('.');
                valueStr1.Append(str[2]);
                valueStr1.Append(str[3]);
            }
            else if (dotLeftCount1 == 3)
            {
                valueStr1.Append(str[0]);
                valueStr1.Append(str[1]);
                valueStr1.Append(str[2]);
                valueStr1.Append('.');
                valueStr1.Append(str[3]);
                valueStr1.Append(str[4]);
            }
            else if(dotLeftCount1 == 4)
            {
                valueStr1.Append(str[0]);
                valueStr1.Append(str[1]);
                valueStr1.Append(str[2]);
                valueStr1.Append(str[3]);
                valueStr1.Append('.');
                valueStr1.Append(str[4]);
                valueStr1.Append(str[5]);
            }
            valueStr1.Append(toLargeNumSign[1]);
            return valueStr1.ToString();
        }
        bool isNegative = str[0] == '-';
        if (isNegative)
            str = str.Remove(0, 1);
        int count = str.Length - 1;
        int sinNum = Mathf.FloorToInt(count / 4f);
        string sign = toLargeNumSign[sinNum];
        int dotLeftCount = count - sinNum * 4 + 1;
 
        StringBuilder valueStr = new StringBuilder();
        if (dotLeftCount == 1)
        {
            valueStr.Append(str[0]);
            valueStr.Append('.');
            valueStr.Append(str[1]);
            valueStr.Append(str[2]);
        }
        else if (dotLeftCount == 2)
        {
            valueStr.Append(str[0]);
            valueStr.Append(str[1]);
            valueStr.Append('.');
            valueStr.Append(str[2]);
            valueStr.Append(str[3]);
        }
        else if (dotLeftCount == 3)
        {
            valueStr.Append(str[0]);
            valueStr.Append(str[1]);
            valueStr.Append(str[2]);
            valueStr.Append('.');
            valueStr.Append(str[3]);
            valueStr.Append(str[4]);
        }else if(dotLeftCount == 4)
        {
            valueStr.Append(str[0]);
            valueStr.Append(str[1]);
            valueStr.Append(str[2]);
            valueStr.Append(str[3]);
            valueStr.Append('.');
            valueStr.Append(str[4]);
            valueStr.Append(str[5]);
        }
        
        valueStr.Append(sign);
        if (isNegative)
            return '-' + valueStr.ToString();
        else
            return valueStr.ToString();
    }
    #endregion
}
}

