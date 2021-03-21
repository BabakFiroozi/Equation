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
        [SerializeField] GameObject _resetAskConfirmObj;
        [SerializeField] Text _hintCost;
        [SerializeField] Text _helpCost;
        
        
        void Start()
        {
            _hintButton.onClick.AddListener(HintButtonClick);
            _helpButton.onClick.AddListener(HelpButtonClick);
            _resetButton.onClick.AddListener(ResetButtonClick);

            _hintCost.text = $"{Translator.GetString("Hint")}({GameConfig.Instance.HintCost})";
            _helpCost.text = $"{Translator.GetString("Help")}({GameConfig.Instance.HelpCost})";

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
                GameSaveData.SubCoin(GameConfig.Instance.HintCost, true, .0f);
            
            GameWord.Instance.Board.DoHint();

            MyAnalytics.SendEvent(MyAnalytics.hint_button_clicked);
        }

        void HelpButtonClick()
        {
            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo) && !GameWord.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HelpCost))
                return;

            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo))
                GameSaveData.SubCoin(GameConfig.Instance.HelpCost, true, .0f);

            GameWord.Instance.Board.DoHelp();
            
            MyAnalytics.SendEvent(MyAnalytics.help_button_clicked);
        }

        void ResetButtonClick()
        {
            var obj = Instantiate(_resetAskConfirmObj, transform.parent);
            var confirm = obj.GetComponent<ConfirmScreen>();
            confirm.ClosedEvent = () => Destroy(obj);
            confirm.OpenConfirm(type =>
            {
                if (type == ConfirmScreen.ConfirmTypes.Ok)
                {
                    GameWord.Instance.Board.DoResetBoard();
                    SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, false);
                }
            });
        }
    }
}