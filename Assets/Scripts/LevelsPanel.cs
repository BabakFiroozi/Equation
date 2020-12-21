using System;
using System.Collections;
using DG.Tweening;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class LevelsPanel : MonoBehaviour
    {
        [SerializeField] RectTransform _levelsContent;
        [SerializeField] GameObject _levelItemObj;

        [SerializeField] HeadingBar _headingBar;

        public static bool StageResumed;
        
        void Start()
        {
            var levels = Resources.LoadAll<TextAsset>("Puzzles/");
            foreach (var level in levels)
            {
                var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
                var obj = Instantiate(_levelItemObj, _levelsContent);
                var levelSelect = obj.GetComponent<LevelSelect>();
                levelSelect.FillData(puzzlesPack.level, puzzlesPack.puzzles.Count);
                if (StageResumed && puzzlesPack.level == DataHelper.Instance.LastPlayedInfo.Level)
                {
                    levelSelect.OpenStages();
                    StartCoroutine(_ScrollToPos(DataHelper.Instance.LastPlayedInfo.Level));
                }
            }
            
            _levelItemObj.SetActive(false);

            CheshmakMe.CheshmakLib.initializeBannerAds("bottom");
        }

        IEnumerator _ScrollToPos(int level)
        {
            yield return new  WaitForEndOfFrame();
            _levelsContent.DOAnchorPosY(_levelsContent.anchoredPosition.y + level * 120, .3f);
        }

        public static void ResetStageHistoryScroll()
        {
            //TODO - scroll
        }

        void OnDestroy()
        {
            StageResumed = false;
        }
    }
}