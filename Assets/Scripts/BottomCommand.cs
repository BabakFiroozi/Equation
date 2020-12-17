using System;
using UnityEngine;
using UnityEngine.UI;


namespace Equation
{
    public class BottomCommand : MonoBehaviour
    {
        [SerializeField] Button _hintButton;
        [SerializeField] Button _helpButton;
        [SerializeField] Button _resetButton;

        void Start()
        {
            _hintButton.onClick.AddListener(HintButtonClick);
            _helpButton.onClick.AddListener(HelpButtonClick);
            _resetButton.onClick.AddListener(ResetButtonClick);

            if (GameWord.Instance.CurrentPlayedInfo.Daily)
                _hintButton.gameObject.SetActive(false);
        }

        void HintButtonClick()
        {
            if (!GameWord.Instance.Board.RestAnyHint())
                return;
            
            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo) && !GameWord.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HintCost))
                return;

            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo))
                GameSaveData.SubCoin(GameConfig.Instance.HintCost, true, .2f);
            
            GameWord.Instance.Board.DoHint();
        }

        void HelpButtonClick()
        {
            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo) && !GameWord.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HelpCost))
                return;

            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo))
                GameSaveData.SubCoin(GameConfig.Instance.HelpCost, true, .2f);

            GameWord.Instance.Board.DoHelp();
        }

        void ResetButtonClick()
        {
            GameWord.Instance.Board.DoResetBoard();
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
        }
    }
}