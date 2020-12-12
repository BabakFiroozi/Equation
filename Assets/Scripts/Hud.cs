using System;
using UnityEngine;
using UnityEngine.UI;


namespace Equation
{
    public class Hud : MonoBehaviour
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
            if (!Board.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HintCost))
            {
                return;
            }

            GameSaveData.SubCoin(GameConfig.Instance.HintCost);
            Board.Instance.DoHint();
        }

        void HelpButtonClick()
        {
            if (!Board.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HelpCost))
            {
                return;
            }
            
            GameSaveData.SubCoin(GameConfig.Instance.HelpCost);
            Board.Instance.DoHelp();
        }

        void ResetButtonClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
        }
    }
}