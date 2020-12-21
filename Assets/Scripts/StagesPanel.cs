using System;
using DefaultNamespace;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class StagesPanel : MonoBehaviour
    {
        [SerializeField] Transform _stagesContent;
        [SerializeField] GameObject _stageItemObj;

        [SerializeField] HeadingBar _headingBar;
        
        void Start()
        {
            var playedInfo = DataHelper.Instance.LastPlayedInfo;

            var level = Resources.Load<TextAsset>($"Puzzles/level_{playedInfo.Level:000}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
            foreach (var puzzle in puzzlesPack.puzzles)
            {
                var obj = Instantiate(_stageItemObj, _stagesContent);
                var stageSelect = obj.GetComponent<StageSelect>();
                stageSelect.FillData(puzzlesPack.level, puzzle.stage);
            }
            
            _stageItemObj.SetActive(false);
            
            CheshmakMe.CheshmakLib.initializeBannerAds("bottom");
        }
    }
}