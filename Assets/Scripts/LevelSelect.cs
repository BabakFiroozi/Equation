using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class LevelSelect : MonoBehaviour
    {
        [SerializeField] Button _goButton;
        [SerializeField] Text _progressText;
        [SerializeField] Text _levelText;
        [SerializeField] Text _clauseText;
        [SerializeField] GameObject _lockObject;
        [SerializeField] GameObject _stagesPanelObj;

        int _level;
        int _clause;
        int _count;
        int _progress;
        
        void Start()
        {
            _goButton.onClick.AddListener(GoButtonClick);
        }

        void GoButtonClick()
        {
            DataHelper.Instance.LastPlayedInfo.Level = _level;
            _stagesPanelObj.GetComponent<StagesPanel>().Show();
        }

        public void FillData(int level, int clause, int count)
        {
            _level = level;
            _clause = clause;
            _count = count;
            _progress = 0;

            var info = new PuzzlePlayedInfo {Level = _level};
            for (int c = 0; c < count; ++c)
            {
                info.Stage = c;
                if (GameSaveData.IsStageSolved(info))
                    _progress++;
            }

            _levelText.text = $"{_level + 1} {Translator.GetString("Level")}";
            _clauseText.text = $"{Translator.GetString("Clause")} {_clause}";
            _progressText.text = $"{_progress} / {_count}";

            bool unlcoked = GameSaveData.IsLevelUnlocked(_level) || GameConfig.Instance.GameIsUnlock;
            _lockObject.SetActive(!unlcoked);
        }

        public void OpenStages()
        {
            GoButtonClick();
        }
    }
}