using System;
using Equation.Gui;
using UnityEngine;

namespace Equation
{
    [RequireComponent(typeof(Button))]
    public class StageButton : MonoBehaviour
    {
        public Button button { get; private set; }

        public int Index { get; private set; }

        void Awake()
        {
            button = gameObject.GetComponent<Button>();
        }

        void Start()
        {
            button.OnClick += OnClick;
        }

        void OnDestroy()
        {
            button.OnClick -= OnClick;
        }

        public void SetIndex(int index)
        {
            button.SetText((index + 1).ToString());
            Index = index;
        }
        
        void OnClick()
        {
            GameManager.Instance.GoToGameplay();
        }
    }
}