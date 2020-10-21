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
        }

        void OnDestroy()
        {
            _hintButton.OnClick -= HintButton_OnClick;
            _helpButton.OnClick -= HelpButton_OnClick;
        }

        void HintButton_OnClick()
        {
            Board.Instance.DoHint();
        }

        void HelpButton_OnClick()
        {
            Board.Instance.DoHelp();
        }
    }
}