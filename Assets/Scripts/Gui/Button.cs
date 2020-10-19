using System;
using UnityEngine;

namespace Equation.Gui
{
    public class Button : MonoBehaviour
    {
        public Action OnClick { get; set; }

        bool _mouseIsDown;
        
        void Start()
        {
            
        }

        public void OnMouseDown()
        {
            _mouseIsDown = true;
        }
        
        public void OnMouseUp()
        {
            if (_mouseIsDown)
            {
                _mouseIsDown = false;
                OnClick?.Invoke();
            }
        }
    }
}