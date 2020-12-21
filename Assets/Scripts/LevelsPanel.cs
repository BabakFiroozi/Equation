using System;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class LevelsPanel : MonoBehaviour
    {
        [SerializeField] Transform _levelsContent;
        [SerializeField] GameObject _levelItemObj;

        [SerializeField] HeadingBar _headingBar;
        
        void Start()
        {
            var levels = Resources.LoadAll<TextAsset>("Puzzles/");
            foreach (var level in levels)
            {
                var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
                var obj = Instantiate(_levelItemObj, _levelsContent);
                var levelSelect = obj.GetComponent<LevelSelect>();
                levelSelect.FillData(puzzlesPack.level, puzzlesPack.puzzles.Count);
            }
            
            _levelItemObj.SetActive(false);

            CheshmakMe.CheshmakLib.initializeBannerAds("bottom");
        }

        public static void ResetStageHistoryScroll()
        {
            //TODO - scroll
        }
    }
}