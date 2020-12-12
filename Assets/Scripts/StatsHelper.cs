using System.Collections.Generic;
using System.Linq;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class StatsHelper
    {
        static StatsHelper _instance;
        public static StatsHelper Instance => _instance ?? (_instance = new StatsHelper());
        
        public int ConsumeHintCount { get; private set; }
        public int ConsumeGuidCount { get; private set; }


        public int HiddenLettersFound { get; private set; }
        public int TotalStagesRank { get; private set; }
        public int StarsCount { get; private set; }
        
        

        public void Calculate()
        {
            TotalStagesRank = 0;
            StarsCount = 0;
            
            ConsumeHintCount = GameSaveData.GetConsumeHint();
            ConsumeGuidCount = GameSaveData.GetConsumeHelp();

        }

        
        public List<string> CullHiddenWords(PuzzlePlayedInfo playedInfo)
        {
            return null;
        }
        
        
        // public bool CalcLevelUp(out int level, out int current, out int need, out int reward)
        // {
        //     return false;
        // }
    }
}