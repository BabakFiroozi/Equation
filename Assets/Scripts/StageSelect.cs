﻿using System;
using Equation;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class StageSelect : MonoBehaviour
    {
        [SerializeField] Button _goButton;
        [SerializeField] Text _stageText;
        [SerializeField] GameObject _lockObject;
        [SerializeField] GameObject _lastObject;

        [SerializeField] GameObject[] _stars;
        
        [SerializeField] GameObject _starsObj;

        int _stage;
        int _level;

        void Awake()
        {
            _lastObject.SetActive(false);
        }

        void Start()
        {
            _goButton.onClick.AddListener(GoButtonClick);
        }

        void GoButtonClick()
        {
            DataHelper.Instance.LastPlayedInfo.Stage = _stage;
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, true);
        }

        public void FillData(int level, int stage)
        {
            _stage = stage;
            _level = level;

            _stageText.text = $"{stage + 1}";
            bool unlcoked = GameSaveData.IsStageUnlocked(_level, _stage) || GameConfig.Instance.GameIsUnlock;
            _lockObject.SetActive(!unlcoked);
            _stageText.gameObject.SetActive(unlcoked);

            _starsObj.SetActive(unlcoked);
            
            var info = new PuzzlePlayedInfo {Level = _level, Stage = _stage};
            int rank = GameSaveData.GetStageRank(info);
            for (int i = 0; i < 3; ++i)
                _stars[i].SetActive(i + 1 <= rank);
        }

        public void SetAsLastPlayed()
        {
            _lastObject.SetActive(true);
        }
    }
}