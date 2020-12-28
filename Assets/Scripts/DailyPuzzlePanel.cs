using System;
using System.Collections.Generic;
using System.Linq;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class DailyPuzzlePanel : MonoBehaviour
    {
        [SerializeField] GameObject puzzleItemObj;
        [SerializeField] GameObject _finishedItemObj;
        [SerializeField] Transform _content;

        [SerializeField] GameObject _todayMessage;
        [SerializeField] GameObject _tomorrowMessage;

        void Start()
        {
            var textAsset = Resources.Load<TextAsset>("DailyPuzzles/level_999");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(textAsset.text);

            int stagesCount = puzzlesPack.puzzles.Count;

            stagesCount = 2;
            
            puzzleItemObj.SetActive(false);
            _finishedItemObj.SetActive(false);

            int dayNum = GameSaveData.GetDailyEntranceNumber();

            bool finished = false;
            if (dayNum >= stagesCount)
            {
                dayNum = stagesCount;
                finished = true;
            }

            var puzzleItems = new List<DailyPuzzleItem>();
            
            for (int i = 0; i < dayNum + 1; ++i)
            {
                if (i == dayNum && finished)
                {
                    _finishedItemObj.SetActive(true);
                    _finishedItemObj.transform.SetParent(_content);
                    break;
                }

                var obj = Instantiate(puzzleItemObj, _content);
                obj.SetActive(true);
                var puzzleItem = obj.GetComponent<DailyPuzzleItem>();
                int stage = i % stagesCount;
                puzzleItem.FillData(stage, i);
                puzzleItems.Add(puzzleItem);
            }

            bool solved = puzzleItems.ToList().Exists(p => p.Rank > -1);
            _todayMessage.SetActive(!solved);
            _tomorrowMessage.SetActive(solved);
        }

        public void Show()
        {
            gameObject.GetComponent<PopupScreen>().Show();
        }
    }
}