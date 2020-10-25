using System;
using System.Collections;
using System.Collections.Generic;
using Equation.Gui;
using UnityEngine;

namespace Equation
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] Button _levelButton;
        
        // Start is called before the first frame update
        void Start()
        {
            _levelButton.OnClick += OnClick;
        }

        void OnDestroy()
        {
            _levelButton.OnClick -= OnClick;
        }

        void OnClick()
        {
            GameManager.Instance.GoToLevelPanel(false);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}