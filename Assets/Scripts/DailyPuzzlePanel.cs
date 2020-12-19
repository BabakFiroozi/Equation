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
            puzzleItemObj.SetActive(false);
            _finishedItemObj.SetActive(false);

            int dayNum = GameSaveData.GetDailyEntranceNumber();

            bool finished = false;
            if (dayNum > DataHelper.MAX_DAILY_NUM)
            {
                dayNum = DataHelper.MAX_DAILY_NUM;
                finished = true;
            }

            var puzzleItems = new List<DailyPuzzleItem>();
            
            for (int i = 0; i < dayNum + 1; ++i)
            {
                var obj = Instantiate(puzzleItemObj, _content);
                obj.SetActive(true);
                var puzzleItem = obj.GetComponent<DailyPuzzleItem>();
                int level = i / DataHelper.MAX_DAILY_STAGES_COUNT;
                int stage = i % DataHelper.MAX_DAILY_STAGES_COUNT;
                puzzleItem.FillData(level, stage, i);
                puzzleItems.Add(puzzleItem);
            }

            if (finished)
            {
                _finishedItemObj.SetActive(true);
                _finishedItemObj.transform.SetParent(_content);
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