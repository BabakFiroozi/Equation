using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace Equation
{
    public class SwitchButton : MonoBehaviour
    {
        [SerializeField] Button _switcheButton;
        [SerializeField] RectTransform[] _switchPositions;
        [SerializeField] RectTransform _indicRectTr;
        [SerializeField] Text _indicLabel;

        /// <summary>
        /// Start from zero
        /// </summary>
        public int CurrentSwitch { get; private set; }

        public Action SwitchedEvent { get; set; }

        void Start()
        {
            _switcheButton.onClick.AddListener(ButtonClick);
        }
        
        void ButtonClick()
        {
            CurrentSwitch = (CurrentSwitch + 1) % 2;
            const float animTime = .25f;
            Vector2 pos = _switchPositions[CurrentSwitch].anchoredPosition;
            _indicRectTr.DOAnchorPosX(pos.x, animTime);
            _indicLabel.text = _switchPositions[CurrentSwitch].GetComponentInChildren<Text>().text;
            SwitchedEvent?.Invoke();
        }
    }
}