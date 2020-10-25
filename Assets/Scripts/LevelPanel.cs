using System;
using Equation.Gui;
using UnityEngine;

namespace Equation
{
    public class LevelPanel : MonoBehaviour
    {
        [SerializeField] Button _levelButton;


        void Awake()
        {
            transform.position = Vector3.zero;

        }

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
            GameManager.Instance.GoToStagePanel();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}