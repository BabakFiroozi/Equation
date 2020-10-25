using System;
using RTLTMPro;
using UnityEngine;

namespace Equation.Gui
{
    public class Button : MonoBehaviour
    {
        [SerializeField] RTLTextMeshPro3D _textMesh;
        public Action OnClick { get; set; }

        bool _mouseIsDown;
        Vector3 _mouseDownPos;


        public Transform Tr { get; private set; }

        void Awake()
        {
            Tr = transform;
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

            if ((_mouseDownPos - Input.mousePosition).magnitude < 20)
                OnClick?.Invoke();
        }

        public void SetText(string text)
        {
            _textMesh.text = text;
        }
    }
}