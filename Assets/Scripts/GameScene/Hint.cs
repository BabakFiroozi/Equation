using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTr;
        [SerializeField] Text _contentText;

        public RectTransform RectTr => _rectTr;
        
        public string Content { get; private set; }

        public bool Revealed { get; private set; }

        public BoardCell Cell { get; private set; }

        float _initWidth;
        float _initFontSize;
        
        void Awake()
        {
            _initWidth = RectTr.rect.width;
            _initFontSize = _contentText.fontSize;
        }
        
        public void SetData(string content, BoardCell cell)
        {
            Content = content;
            Cell = cell;
            _contentText.text = Content;
            RectTr.anchoredPosition = cell.pos;
            _contentText.fontSize = (int) (RectTr.rect.width / _initWidth * _initFontSize);
        }

        public void Reveal()
        {
            Revealed = true;
            _contentText.text = "?";
            gameObject.SetActive(true);
        }
    }
}