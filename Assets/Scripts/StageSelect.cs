using System;
using Equation;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class StageSelect : MonoBehaviour
    {
        [SerializeField] Button _goButton;
        [SerializeField] Text _stageText;
        [SerializeField] GameObject _lockObject;

        int _stage;
        int _level;
        
        
        void Start()
        {
            _goButton.onClick.AddListener(StageButtonClick);
        }

        void StageButtonClick()
        {
            DataHelper.Instance.LastPlayedInfo.Stage = _stage;
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
        }

        public void FillData(int level, int stage)
        {
            _stage = stage;
            _level = level;

            _stageText.text = $"{stage + 1}";
            bool unlcoked = GameSaveData.IsStageUnlocked(_level, _stage) || GameConfig.Instance.GameIsUnlock;
            _lockObject.SetActive(!unlcoked);
            _stageText.gameObject.SetActive(unlcoked);
        }
    }
}