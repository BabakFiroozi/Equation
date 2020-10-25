using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Equation.Gui;
using UnityEngine;

namespace Equation
{
    public class StageScroll : MonoBehaviour
    {
        [SerializeField] Transform _container;
        [SerializeField] float _limit = 160;
        
        Vector3 _scrollPos;
        bool _scrolling;
        bool _mouseIsDown;
        float _scrollingDiff;

        Transform _tr;

        List<StageButton> _buttons = new List<StageButton>();
        
        float _space;


        
        void Start()
        {
            _tr = transform;
        }

        public void SetButtons(IReadOnlyCollection<StageButton> buttons)
        {
            _buttons = buttons.ToList();
            for (int i = 0; i < _buttons.Count; ++i)
            {
                var button = _buttons[i];
                button.SetIndex(i);
            }

            _space = _buttons[0].button.Tr.position.z - _buttons[5].button.Tr.position.z;
        }


        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                var ray = Camera.main.ScreenPointToRay(mousePos);
                bool hit = Physics.Raycast(ray, out var hitInfo, 1000, LayerMaskUtil.GetLayerMask("Scroll"));
                if (hit)
                {
                    Vector3 hitPos = hitInfo.point;

                    if (_scrolling)
                    {
                        _scrollingDiff = (hitPos - _scrollPos).z;

                        _scrollingDiff = Mathf.Clamp(_scrollingDiff, -1, 1);
                        
                        if (_scrollingDiff > 0 && _container.position.z < _limit || _scrollingDiff < 0 && _container.position.z > 0)
                        {
                            Vector3 pos = _container.position;
                            pos.z += _scrollingDiff;
                            _container.position = pos;
                        }
                    }

                    _scrollPos = hitPos;
                    _scrolling = true;
                }
                else
                {
                    _scrolling = false;
                    _scrollPos = Vector3.zero;
                }
            }
            else
            {
                if (_scrolling)
                {
                    if (_container.position.z < 0)
                    {
                        _container.DOMoveZ(0, .2f);
                    }
                    else if (_container.position.z > _limit + 1)
                    {
                        _container.DOMoveZ(160, .2f);
                    }
                    else
                    {
                        // print("scrolling diff: " + _scrollingDiff);
                        _scrollingDiff = Mathf.Clamp(_scrollingDiff, -2, 2);
                        _container.DOKill();
                        float z = _container.position.z + _scrollingDiff * 10;
                        z = Mathf.Clamp(z, -(_limit + 1), _limit + 1);
                        _container.DOMoveZ(z, Mathf.Abs(_scrollingDiff));
                    }
                }
                
                _scrolling = false;
                _scrollPos = Vector3.zero;
            }
            
            Replace();

            foreach (var button in _buttons)
            {
                button.gameObject.SetActive(button.button.Tr.position.z < 8 && button.button.Tr.position.z > -10);
            }
        }


        public void Replace()
        {
            const float poolLimit = 8f;

            for (int i = 0; i < _buttons.Count; ++i)
            {
                var b = _buttons[i];
                
                if (b.button.Tr.position.z > poolLimit)
                {
                    var p = b.button.Tr.position;
                    p.z -= 10 * _space;
                    b.button.Tr.position = p;
                    b.SetIndex(b.Index + 50);
                }

                if (b.button.Tr.position.z < -poolLimit)
                {
                    var p = b.button.Tr.position;
                    p.z += 10 * _space;
                    b.button.Tr.position = p;
                    b.SetIndex(b.Index - 50);
                }
            }
        }
    }
}