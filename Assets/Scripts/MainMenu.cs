using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button _playButton;
        [SerializeField] Button _shopButton;

        void Start()
        {
            _playButton.onClick.AddListener(() =>
            {
                SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU);
            });
        }
    }
}