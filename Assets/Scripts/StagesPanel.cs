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
                if (LevelsPanel.StageResumed && puzzle.stage == playedInfo.Stage)
                {
                    LevelsPanel.StageResumed = false;
                    stageSelect.SetAsLastPlayed();
                    StartCoroutine(_ScrollToPos(puzzle.stage));
                }
            }
        }

        IEnumerator _ScrollToPos(int stage)
        {
            yield return new  WaitForEndOfFrame();
            Vector2 pos = _stagesContent.anchoredPosition;
            pos.y += stage / 5 * 120;
            _stagesContent.anchoredPosition = pos;
        }
        
    }
}