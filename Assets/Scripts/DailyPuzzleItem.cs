using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class DailyPuzzleItem : MonoBehaviour
    {
        [SerializeField] GameObject[] _stars;
        [SerializeField] Button _button;
        [SerializeField] Text _dayText;

        PuzzlePlayedInfo puzzleInfo;

        int _stage; //Day
        

        public int Rank { get; private set; } = -1;

        void Start()
        {
            _button.onClick.AddListener(ButtonClick);
        }

        void ButtonClick()
        {
            MyAnalytics.SendEvent(MyAnalytics.daily_played);

            DataHelper.Instance.LastPlayedInfo.Daily = true;
            DataHelper.Instance.LastPlayedInfo.Level = puzzleInfo.Level;
            DataHelper.Instance.LastPlayedInfo.Stage = puzzleInfo.Stage;

            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, true);
        }

        public void FillData(int stage, int day)
        {
            _stage = stage; //Day of week

            puzzleInfo = new PuzzlePlayedInfo {Level = 999, Stage = _stage, Daily = true};
            Rank = GameSaveData.GetStageRank(puzzleInfo);
            _dayText.text = $"{day + 1} {Translator.GetString("Day")}";
            for (int i = 0; i < 3; ++i)
                _stars[i].SetActive(i + 1 <= Rank);
        }
    }
}