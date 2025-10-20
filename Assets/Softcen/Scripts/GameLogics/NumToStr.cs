using UnityEngine;
using System.Collections;
using System.Text;

public class NumToStr {
    private static char[] int_parser = new char[128];

    public static string GetTimeStr(float time)
    {
        if (time < 60f)
        {
            return time.ToString("F2") + " sec";
        }
        float mins = time / 60f;
        if (mins < 60f)
            return mins.ToString("F2") + " min";
        float hours = mins / 60f;
        return hours.ToString("F2") + " hours";
    }
    public static string GetTimeStr(double time)
    {
        if (time < 60f)
        {
            return time.ToString("F2") + " sec";
        }
        double mins = time / 60f;
        if (mins < 60f)
            return mins.ToString("F2") + " min";
        double hours = mins / 60f;
        return hours.ToString("F2") + " hours";
    }
    public static string GetNumStr(float num)
    {
        float numStr;
        string suffix;
        if (num < 1000)
        {
            numStr = num;
            suffix = "";
        }
        else if (num < 1000000)
        {
            numStr = num / 1000;
            suffix = "K";
        }
        else if (num < 1000000000)
        {
            numStr = num / 1000000;
            suffix = "M";
        }
        else if (num < 1000000000000)
        {
            numStr = num / 1000000000;
            suffix = "B";
        }
        else
        {
            numStr = num / 1000000000000;
            suffix = "T";
        }

        if (numStr < 10)
            return numStr.ToString("F2") + suffix;
        else if (numStr < 100)
            return numStr.ToString("F1") + suffix;
        return numStr.ToString("F0") + suffix;

    }

    private static readonly string[] predefinedSuffix = new string[5] {
        "",
        "K",
        "M",
        "B",
        "T",
        };

    public static void GetNumStr(double num, StringBuilder sb)
    {
        double numStr;
        int suffixIndex = 0;
        sb.Length = 0;

        if (num < 1000)
        {
            numStr = num;
        }
        else if (num < 1000000)
        {
            numStr = num / 1000;
            suffixIndex = 1;
        }
        else if (num < 1000000000)
        {
            numStr = num / 1000000;
            suffixIndex = 2;
        }
        else if (num < 1000000000000)
        {
            numStr = num / 1000000000;
            suffixIndex = 3;
        }
        else
        {
            numStr = num / 1000000000000;
            suffixIndex = 4;
        }


        //AppendDouble(12345.6789, sb, 2);
        //sb.Length = 0;
        //AppendDouble(345.06789, sb, 2);
        //sb.Length = 0;
        //AppendDouble(962345.0007989, sb, 7);

        if (numStr < 10)
        {
            AppendDouble(numStr, sb, 2);
            //sb.AppendFormat("{0:F2}", numStr);
            if (suffixIndex > 0)
                sb.Append(predefinedSuffix[suffixIndex]);
        } else if (numStr < 100)
        {
            AppendDouble(numStr, sb, 1);
            //sb.AppendFormat("{0:F1}", numStr);
            if (suffixIndex > 0)
                sb.Append(predefinedSuffix[suffixIndex]);
        } else
        {
            AppendDouble(numStr, sb, 0);
            //sb.AppendFormat("{0:F0}", numStr);
            if (suffixIndex > 0)
                sb.Append(predefinedSuffix[suffixIndex]);
        }
    }

    public static void AppendDouble(double value, StringBuilder sb, int decimals)
    {

        int multiplier = 1;
        for (int i=0; i < decimals; i++)
        {
            multiplier *= 10;
        }
        int intValue = (int)value;
        int decimalValue = (int)((value - (int)value) * multiplier);
        AppendInt(intValue, sb);
        if (decimals > 0)
        {
            sb.Append(".");
            // Leading zeros
            int len = 1;
            for (uint rem = (uint)decimalValue / 10; rem > 0; rem /= 10)
            {
                len++;
            }
            for (int i=0; i < decimals- len; i++)
            {
                sb.Append("0");
            }

            AppendInt(decimalValue, sb);
        }
    }

    public static void AppendInt(int value, StringBuilder sb)
    {
        int count;
        if (value >= 0)
        {
            count = ToCharArray((uint)value, int_parser, 0);  
        } else
        {
            int_parser[0] = '-';
            count = ToCharArray((uint)-value, int_parser, 1) + 1;
        }
        for (int i = 0; i < count; i++)
        {
            sb.Append(int_parser[i]);
        }
    }

    private static int ToCharArray(uint value, char[] buffer, int bufferIndex)
    {
        if (value == 0)
        {
            buffer[bufferIndex] = '0';
            return 1;
        }
        int len = 1;
        for (uint rem = value/10; rem > 0; rem /= 10)
        {
            len++;
        }
        for (int i= len-1; i >= 0; i--)
        {
            buffer[bufferIndex + i] = (char)('0' + (value % 10));
            value /= 10;
        }
        return len;
    }

    public static string GetNumStr(double num)
    {
        double numStr;
        string suffix;
        if (num < 1000)
        {
            numStr = num;
            suffix = "";
        }
        else if (num < 1000000)
        {
            numStr = num / 1000;
            suffix = "K";
        }
        else if (num < 1000000000)
        {
            numStr = num / 1000000;
            suffix = "M";
        }
        else if (num < 1000000000000)
        {
            numStr = num / 1000000000;
            suffix = "B";
        }
        else
        {
            numStr = num / 1000000000000;
            suffix = "T";
        }

        if (numStr < 10)
            return numStr.ToString("F2") + suffix;
        else if (numStr < 100)
            return numStr.ToString("F1") + suffix;
        return numStr.ToString("F0") + suffix;
    }

    public static string GetNumStr(long num)
    {
        double numStr;
        string suffix;
        if (num < 1000)
        {
            numStr = num;
            suffix = "";
        }
        else if (num < 1000000)
        {
            numStr = (double)num / 1000;
            suffix = "K";
        }
        else if (num < 1000000000)
        {
            numStr = (double)num / 1000000;
            suffix = "M";
        }
        else if (num < 1000000000000)
        {
            numStr = (double)num / 1000000000;
            suffix = "B";
        }
        else
        {
            numStr = (double)num / 1000000000000;
            suffix = "T";
        }

        if (numStr < 10)
            return numStr.ToString("F2") + suffix;
        else if (numStr < 100)
            return numStr.ToString("F1") + suffix;
        return numStr.ToString("F0") + suffix;

    }

    public static string GarbageFreeString(StringBuilder sb)
    {
        string str = (string)sb.GetType().GetField(
            "_str",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance).GetValue(sb);

        //Optional: clear out the string
        //for (int i = 0; i &lt; sb.Capacity; i++) {
        //	sb.Append(" ");
        //}
        return str;
    }
}
