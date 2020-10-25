using System;
using Equation.Gui;
using UnityEngine;

namespace Equation
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] Button _hintButton;
        [SerializeField] Button _helpButton;
        [SerializeField] Button _menuButton;
        [SerializeField] Button _resetButton;

        void Start()
        {
            _hintButton.OnClick += HintButton_OnClick;
            _helpButton.OnClick += HelpButton_OnClick;
            _menuButton.OnClick += MenuButton_OnClick;
        }

        void OnDestroy()
        {
            _hintButton.OnClick -= HintButton_OnClick;
            _helpButton.OnClick -= HelpButton_OnClick;
            _menuButton.OnClick -= MenuButton_OnClick;
        }

        void HintButton_OnClick()
        {
            Board.Instance.DoHint();
        }

        void HelpButton_OnClick()
        {
            Board.Instance.DoHelp();
        }
        
        void MenuButton_OnClick()
        {
            GameManager.Instance.GoToStagePanel(true);
        }
    }
}