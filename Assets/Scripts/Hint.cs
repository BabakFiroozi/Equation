using System;
using RTLTMPro;
using UnityEngine;

namespace Equation
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] RTLTextMeshPro3D _contentText;

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