using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class ExitPanel : MonoBehaviour
    {
        [SerializeField] Button _restartButton;
        [SerializeField] Button _stageButton;
        [SerializeField] Button _menuButton;
        [SerializeField] Button _settingButton;

        [SerializeField] GameObject _settingPanel;
        
        

        void Start()
        {
            _menuButton.onClick.AddListener(MenuClick);
            _stageButton.onClick.AddListener(StageClick);
            _restartButton.onClick.AddListener(RestartClick);
            _settingButton.onClick.AddListener(SettingClick);

            // _stageButton.gameObject.SetActive(GameBoard.Instance.CurrentPlayedInfo.Mode != GameModes.Daily);
        }

        void SettingClick()
        {
            var obj = Instantiate(_settingPanel, transform.parent);
            obj.GetComponent<SettingPanel>().ShowSetting();
            obj.GetComponent<PopupScreen>().HideEvent = () => Destroy(obj);
        }

        void MenuClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU);
            MyAnalytics.SendEvent(MyAnalytics.back_menu_main_menu);
        }

        void StageClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_STAGE_MENU);
            MyAnalytics.SendEvent(MyAnalytics.back_menu_stages);
        }

        void RestartClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
            MyAnalytics.SendEvent(MyAnalytics.back_menu_restart);
        }
    }
}