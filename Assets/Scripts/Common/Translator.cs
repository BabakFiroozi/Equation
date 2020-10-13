using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Equation
{
    internal static class Translator
    {
        public const string Level = "Level";
        public const string In_Every_Stage = "In_Every_Stage";
        public const string Solved_Move = "Solved_Move";
        public const string With = "With";
        
        static bool _inited = false;
        
        static Dictionary<string, string> _translateDic = new Dictionary<string, string>();


        public static string GetString(string key)
        {
            if (!_inited)
                InitTranslateDic();
            
            if (_translateDic.ContainsKey(key))
                return NBidi.NBidi.LogicalToVisual(_translateDic[key]);
            return key.ToUpper();
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