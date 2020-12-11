using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class InputFieldFixer : MonoBehaviour
    {
        InputField _inputField;

        Text _fixText;
        
        void Start()
        {
            _inputField = GetComponent<InputField>();
            var obj = Instantiate(_inputField.textComponent.gameObject, transform);
            obj.name = "TextFixed";
            _fixText = obj.GetComponent<Text>();
            _inputField.textComponent.color = new Color(0, 0, 0, 0);
        }

        void Update()
        {
            _fixText.text = FarsiSaz.Farsi.Fix(_inputField.text, true);
        }
    }
}