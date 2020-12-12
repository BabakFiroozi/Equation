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
        }

        void HintButtonClick()
        {
            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo) && !Board.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HintCost))
                return;

            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo))
                GameSaveData.SubCoin(GameConfig.Instance.HintCost, true);
            
            Board.Instance.DoHint();
        }

        void HelpButtonClick()
        {
            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo) && !Board.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HelpCost))
                return;

            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo))
                GameSaveData.SubCoin(GameConfig.Instance.HelpCost, true);

            Board.Instance.DoHelp();
        }

        void ResetButtonClick()
        {
            Board.Instance.DoResetBoard();
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
        }
    }
}