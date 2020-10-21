using System;
using UnityEngine;

namespace Equation.Gui
{
    public class Button : MonoBehaviour
    {
        public Action OnClick { get; set; }

        bool _mouseIsDown;
        Vector3 _mouseDownPos;

        void Start()
        {

        }

        public void OnMouseDown()
        {
            _mouseIsDown = true;
            _mouseDownPos = Input.mousePosition;
        }

        public void OnMouseUp()
        {
            if (!_mouseIsDown)
                return;
            
            _mouseIsDown = false;

            if ((_mouseDownPos - Input.mousePosition).magnitude < 10)
                OnClick?.Invoke();
        }
    }
}