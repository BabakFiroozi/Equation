using System;
using Equation.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button _playButton;
        [SerializeField] Button _continueButton;
        [SerializeField] Text _startText;
        [SerializeField] Text _continueText;
        [SerializeField] Button _shopButton;

        void Start()
        {
            _playButton.onClick.AddListener(PlayButtonClick);
            _continueButton.onClick.AddListener(ContinueButtonClick);

            CalcLastPlayed();

            if (GameSaveData.GetSessionNumber() == 1)
            {
                _startText.rectTransform.anchoredPosition = Vector2.zero;
                _startText.text = Translator.GetString("Start");
                _continueText.gameObject.SetActive(false);
            }
            else
            {
                var info = DataHelper.Instance.LastPlayedInfo;
                _startText.text = Translator.GetString("Continue");
                _continueText.text = $"{info.Stage + 1} {Translator.GetString("Stage")} - {info.Level + 1} {Translator.GetString("Level")}";
            }
        }

        void CalcLastPlayed()
        {
            int levels = DataHelper.Instance.LevelsCount;
            int lastUnlockedLevel = 0;
            for (int l = 0; l < levels; ++l)
            {
                if (GameSaveData.IsLevelUnlocked(l))
                    continue;
                lastUnlockedLevel = l - 1;
                break;
            }

            var level = Resources.Load<TextAsset>($"Puzzles/level_{lastUnlockedLevel:000}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
            var playedInfo = new PuzzlePlayedInfo();
            playedInfo.Level = lastUnlockedLevel;
            for (int s = 0; s < puzzlesPack.puzzles.Count; ++s)
            {
                playedInfo.Stage = s;
                if (GameSaveData.IsStageUnlocked(lastUnlockedLevel, s) && !GameSaveData.IsStageSolved(playedInfo))
                {
                    playedInfo.Stage = s;
                    break;
                }
            }

            DataHelper.Instance.LastPlayedInfo.Level = playedInfo.Level;
            DataHelper.Instance.LastPlayedInfo.Stage = playedInfo.Stage;
        }

        void PlayButtonClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU);
        }
        
        void ContinueButtonClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
        }
        
    }
}