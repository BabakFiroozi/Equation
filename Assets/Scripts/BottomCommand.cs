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
                GameSaveData.SubCoin(GameConfig.Instance.HintCost, true, .1f);
            
            GameWord.Instance.Board.DoHint();
        }

        void HelpButtonClick()
        {
            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo) && !GameWord.Instance.CoinBox.CheckEnoughCoin(GameConfig.Instance.HelpCost))
                return;

            if (!GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo))
                GameSaveData.SubCoin(GameConfig.Instance.HelpCost, true, .1f);

            GameWord.Instance.Board.DoHelp();
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