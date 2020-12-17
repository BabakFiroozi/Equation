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
        [SerializeField] GameObject _lockObject;

        int _level;
        int _count;
        int _progress;
        
        void Start()
        {
            _goButton.onClick.AddListener(GoButtonClick);
        }

        void GoButtonClick()
        {
            DataHelper.Instance.LastPlayedInfo.Level = _level;
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_STAGE_MENU, true);
        }

        public void FillData(int level, int count)
        {
            _level = level;
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
            _progressText.text = $"{_progress} / {_count}";

            bool unlcoked = GameSaveData.IsLevelUnlocked(_level) || GameConfig.Instance.GameIsUnlock;
            _lockObject.SetActive(!unlcoked);
        }
    }
}