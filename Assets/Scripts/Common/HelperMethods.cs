using System;

namespace Equation
{
    public static class HelperMethods
    {
        public static string TimeToString(int timeInSecond, bool onlyMinute = false)
        {
            int hour = timeInSecond / 3600;
            int min = timeInSecond % 3600 / 60;
            int sec = timeInSecond % 60;
            string str = $"{hour:00} : {min:00} : {sec:00}";
            if(onlyMinute)
                str = $"{min:00} : {sec:00}";
            return str;
        }

        public static string TrimStringToDot(string str, int len)
        {
            if (str.Length <= len)
                return str;
            return str.Substring(len) + new string('.', 3);
        }
        
        public static string ShortenNumberToString(int num)
        {
            string str = "";
            if (num > 1000000000)
                str = $"{num/1000000000}G";
            else if (num > 1000000)
                str = $"{num/100000}M";
            else if (num > 1000)
                str = $"{num/1000}K";
            else
                str = $"{num}";
            return str;
        }
        
        public static string CorrectOpperatorContent(string content)
        {
            if (content == "e")
                content = "=";
            if (content == "p")
                content = "+";
            if (content == "m")
                content = "-";
            if (content == "t")
                content = "×";
            if (content == "d")
                content = "÷";
            return content;
        }
        
    }
}