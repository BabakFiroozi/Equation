using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Equation.FarsiSaz;
using UnityEngine;


namespace Equation
{
    internal static class Translator
    {

        public const string CROSS_SIGN = "×";
        
        static bool _inited = false;
        
        static Dictionary<string, string> _translateDic = new Dictionary<string, string>();


        public static string GetString(string key, bool fix = true)
        {
            if (!_inited)
                InitTranslateDic();

            if (_translateDic.ContainsKey(key))
            {
                if (fix)
                    return Farsi.Fix(_translateDic[key], true);
                else
                    return _translateDic[key];
            }
            return key.ToUpper();
        }

        public static string FixFarsi(string text)
        {
            return Farsi.Fix(text, true);
        }

        static void InitTranslateDic()
        {
            _translateDic.Clear();
            var textAsset = Resources.Load<TextAsset>("Translates");
            var translateLines = textAsset.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in translateLines)
            {
                if (line.StartsWith("*") || line.StartsWith(" ") || line == "" || line.Length < 2)
                    continue;
                var strsArr = line.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                _translateDic[strsArr[0]] = strsArr[1];
            }
        }

        static string GetLetter(string letter)
        {
            return "";
        }
    }
}