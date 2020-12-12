using System;
using Equation.FarsiSaz;
using UnityEditor;
using UnityEngine;

namespace Equation.Tools
{
    public class FarsiFixWindow : EditorWindow
    {
        [MenuItem("Tools/Farsi Fix")]
        public static void ShowWindow()
        {
            var window = GetWindowWithRect<FarsiFixWindow>(new Rect(0, 0, 240, 130), true, "Farsi Fix", true);
            window.ShowUtility();
        }

        Font _englishFont;
        Font _persianFont;

        string _typingString = "";

        void Awake()
        {
            _englishFont = null; //Use default
            _persianFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Editor/Fonts/B_Yekan_Editor.ttf");
        }

        void OnGUI()
        {
            GUI.Label(new Rect(20, 10, 200, 20), "Type your string");

            GUI.skin.textField.font = _persianFont;

            var align = GUI.skin.textField.alignment;

            EditorGUI.DrawRect(new Rect(10, 40, 220, 20), new Color(.9f, .8f, .5f, .7f));

            align = GUI.skin.label.alignment;

            GUI.skin.label.font = _persianFont;

            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUI.Label(new Rect(10, 40, 220, 20), Farsi.Fix(_typingString, true));
            GUI.skin.label.alignment = align;

            GUI.SetNextControlName("Typing_String");
            GUI.skin.textField.alignment = TextAnchor.MiddleRight;
            _typingString = GUI.TextField(new Rect(new Rect(10, 65, 220, 20)), _typingString);
            GUI.skin.textField.alignment = align;
            GUI.FocusControl("Typing_String");
            
            if (GUI.Button(new Rect(10, 100, 220, 20), "Copy"))
            {
                CopyText();
            }

            GUI.skin.label.font = _englishFont;
            GUI.skin.textField.font = _englishFont;
        }

        void CopyText()
        {
            GUIUtility.systemCopyBuffer = Farsi.Fix(_typingString, true);
            Close();
        }
    }
}