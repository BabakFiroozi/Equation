﻿using System;
using System.Collections;
using Equation.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class StagesPanel : MonoBehaviour
    {
        [SerializeField] RectTransform _stagesContent;
        [SerializeField] GameObject _stageItemObj;
        [SerializeField] PopupScreen _popupScreen;
        [SerializeField] Text _levelText;

        void Start()
        {
            _stageItemObj.SetActive(false);
        }

        public void Show()
        {
            _popupScreen.Show();
            
            _stagesContent.anchoredPosition = Vector2.zero;

            var playedInfo = DataHelper.Instance.LastPlayedInfo;
            _levelText.text = $"<color=#73D6FF>{playedInfo.Level + 1}</color> <color=#F0FF00>{Translator.GetString("Stages_Of_level")}</color>";

            foreach (var c in _stagesContent)
            {
                var tr = c as Transform;
                Destroy(tr.gameObject);
            }
            
            var level = Resources.Load<TextAsset>($"Puzzles/level_{playedInfo.Level:000}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
            foreach (var puzzle in puzzlesPack.puzzles)
            {
                var obj = Instantiate(_stageItemObj, _stagesContent);
                obj.SetActive(true);
                var stageSelect = obj.GetComponent<StageSelect>();
                stageSelect.FillData(puzzlesPack.level, puzzle.stage);
                if (LevelsPanel.StageResumed != null && LevelsPanel.StageResumed.Level == puzzlesPack.level && LevelsPanel.StageResumed.Stage == puzzle.stage)
                {
                    stageSelect.SetAsLastPlayed();
                    StartCoroutine(_ScrollToPos(puzzle.stage));
                }
            }
        }

        IEnumerator _ScrollToPos(int stage)
        {
            yield return new  WaitForEndOfFrame();
            Vector2 pos = _stagesContent.anchoredPosition;
            float itemHeight =_stageItemObj.GetComponent<RectTransform>().rect.height + 10;
            int itemsCount = (int)(_stagesContent.parent.GetComponent<RectTransform>().rect.width / itemHeight);
            if ((stage + 1) / 5 > itemsCount)
                pos.y += ((stage + 1) / 5 - itemsCount) * itemHeight + itemHeight + 10;
            _stagesContent.anchoredPosition = pos;
        }
    }
}