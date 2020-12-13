using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class GameWord : MonoBehaviour
    {
        [SerializeField] Button _gridButton;
        [SerializeField] Button _fontButton;
        [SerializeField] GameObject _solvedBadge;

        
        void Start()
        {
            FontButtonClick(false);
            GridButtonClick(false);
            
            _fontButton.onClick.AddListener(() => FontButtonClick());
            _gridButton.onClick.AddListener(() => GridButtonClick());
            
            _solvedBadge.SetActive(GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo));
        }

        void FontButtonClick(bool change = true)
        {
            bool isEng = GameSaveData.IsNumberFontEng();
            if (change)
                isEng = !isEng;
            GameSaveData.SetNumberFontEng(isEng);
            var tr = _fontButton.transform;
            tr.Find("en").gameObject.SetActive(isEng);
            tr.Find("fa").gameObject.SetActive(!isEng);

            Board.Instance.SetPawnsFont(isEng);
        }

        void GridButtonClick(bool change = true)
        {
            bool visible = GameSaveData.IsGridVisible();
            if (change)
                visible = !visible;
            GameSaveData.SetGridVisible(visible);
            var tr = _gridButton.transform;
            tr.Find("on").gameObject.SetActive(visible);

            Board.Instance.SetGridVisible(visible);
        }
    }
}