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

        public void SetData(string content, BoardCell cell)
        {
            Content = content;
            Cell = cell;
            _contentText.text = Content;
        }

        public void Reveal()
        {
            Revealed = true;
            _contentText.text = "?";
            gameObject.SetActive(true);
        }
    }
}