using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class GameWord : MonoBehaviour
    {
        [SerializeField] Button _fontButton;
        [SerializeField] GameObject _solvedBadge;

        
        void Start()
        {
            FontButtonClick(false);
            _fontButton.onClick.AddListener(() => FontButtonClick());
            _solvedBadge.SetActive(GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo));
        }

        void FontButtonClick(bool change = true)
        {
            bool isEng = GameSaveData.GetNumberFontEng();
            if (change)
                isEng = !isEng;
            GameSaveData.SetNumberFontEng(isEng);
            var tr = _fontButton.transform;
            tr.Find("en").gameObject.SetActive(isEng);
            tr.Find("fa").gameObject.SetActive(!isEng);

            Board.Instance.SetPawnsFont(isEng);
        }
    }
}