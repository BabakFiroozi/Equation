using System;
using System.Collections.Generic;
using Equation.Gui;
using UnityEngine;

namespace Equation
{
    public class StagePanel : MonoBehaviour
    {
        [SerializeField] GameObject _buttonPrefab;
        [SerializeField] Transform _container;
        [SerializeField] StageScroll _scroll;
        [SerializeField] int _rowSize = 5;
        [SerializeField] float _buttonsSpace = 1.75f;

        List<StageButton> _buttons = new List<StageButton>();

        void Awake()
        {
            transform.position = Vector3.zero;
        }

        void Start()
        {
            
            _buttonPrefab.SetActive(false);

            Vector3 pos = _buttonPrefab.transform.position;

            for (int i = 0; i < 50; ++i)
            {
                if (i > 0 && i % _rowSize == 0)
                    pos.z -= _buttonsSpace;

                var obj = Instantiate(_buttonPrefab, _container);
                obj.transform.position = new Vector3(pos.x + i % _rowSize * _buttonsSpace, pos.y, pos.z);
                obj.SetActive(true);
                var button = obj.GetComponent<StageButton>();
                _buttons.Add(button);
                button.button.SetText((i + 1).ToString());
            }

            _scroll.SetButtons(_buttons);
            _scroll.Replace();
        }

        public void Refresh()
        {
            for (int i = 0; i < _buttons.Count; ++i)
            {

            }
        }
    }
}