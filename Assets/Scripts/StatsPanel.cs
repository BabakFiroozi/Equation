using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class StatsPanel : MonoBehaviour
    {
        [SerializeField] RectTransform _statsContent;

        [SerializeField] Text _gainedStarsText;
        [SerializeField] Text _consumedHintsText;
        [SerializeField] Text _consumedGuidesText;

        PopupScreen _popupScreen;

        void Start()
        {
            // StatsHelper.Instance.Calculate();

            var stats = StatsHelper.Instance;

            _gainedStarsText.text = $"{stats.StarsCount} {Translator.CROSS_SIGN}";
            _consumedHintsText.text = $"{stats.ConsumeHintCount}";
            _consumedGuidesText.text = $"{stats.ConsumeGuidCount}";
        }

        public void ShowStats()
        {
            if (_popupScreen == null)
                _popupScreen = gameObject.GetComponent<PopupScreen>();
            
            _statsContent.anchoredPosition = Vector2.zero;
            _popupScreen.Show();
        }
    }
}