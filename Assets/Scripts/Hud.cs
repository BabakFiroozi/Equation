using System;
using UnityEngine;
using UnityEngine.UI;


namespace Equation
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] Button _hintButton;
        [SerializeField] Button _helpButton;

        void Start()
        {
            _hintButton.onClick.AddListener(HintButtonClick);
            _helpButton.onClick.AddListener(HelpButtonClick);
        }

        void HintButtonClick()
        {
            Board.Instance.DoHint();
        }

        void HelpButtonClick()
        {
            Board.Instance.DoHelp();
        }
    }
}