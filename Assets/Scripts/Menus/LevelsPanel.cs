﻿using System;
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
        [SerializeField] RectTransform _commingSoon;

        [SerializeField] HeadingBar _headingBar;

        public static PuzzlePlayedInfo StageResumed;
        
        void Start()
        {
            var levels = Resources.LoadAll<TextAsset>("Puzzles/");
            foreach (var level in levels)
            {
                var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
                var obj = Instantiate(_levelItemObj, _levelsContent);
                var levelSelect = obj.GetComponent<LevelSelect>();
                levelSelect.FillData(puzzlesPack.level, puzzlesPack.puzzles[0].clauses, puzzlesPack.puzzles.Count);
                if (StageResumed != null && StageResumed.Level == puzzlesPack.level)
                {
                    levelSelect.OpenStages();
                    StartCoroutine(_ScrollToPos(DataHelper.Instance.LastPlayedInfo.Level));
                }
            }

            _commingSoon.transform.SetParent(_levelsContent);
            
            _levelItemObj.SetActive(false);

            CheshmakMe.CheshmakLib.initializeBannerAds("bottom");
        }

        IEnumerator _ScrollToPos(int level)
        {
            yield return new  WaitForEndOfFrame();
            Vector2 pos = _levelsContent.anchoredPosition;
            float itemHeight =_levelItemObj.GetComponent<RectTransform>().rect.height + 10;
            float itemsCount = (int)(_levelsContent.parent.GetComponent<RectTransform>().rect.width / itemHeight);
            if (level + 1 > itemsCount)
                pos.y += (level + 1 - itemsCount) * itemHeight + itemHeight + 10;
            _levelsContent.anchoredPosition = pos;
        }

        void OnDestroy()
        {
            StageResumed = null;
        }
    }
}