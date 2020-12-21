using System;
using System.Collections;
using DG.Tweening;
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

            _levelText.text = $"{DataHelper.Instance.LastPlayedInfo.Level + 1} {Translator.GetString("Stages_Of_level")}";

            foreach (var c in _stagesContent)
            {
                var tr = c as Transform;
                Destroy(tr.gameObject);
            }

            var playedInfo = DataHelper.Instance.LastPlayedInfo;

            var level = Resources.Load<TextAsset>($"Puzzles/level_{playedInfo.Level:000}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
            foreach (var puzzle in puzzlesPack.puzzles)
            {
                var obj = Instantiate(_stageItemObj, _stagesContent);
                obj.SetActive(true);
                var stageSelect = obj.GetComponent<StageSelect>();
                stageSelect.FillData(puzzlesPack.level, puzzle.stage);
                if (LevelsPanel.StageResumed)
                {
                    LevelsPanel.StageResumed = false;
                    StartCoroutine(_ScrollToPos(puzzle.stage));
                }
            }
        }

        IEnumerator _ScrollToPos(int stage)
        {
            yield return new  WaitForEndOfFrame();
            _stagesContent.DOAnchorPosY(_stagesContent.anchoredPosition.y + stage / 5 * 120, .3f);
        }
        
    }
}