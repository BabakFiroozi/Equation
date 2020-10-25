using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Equation
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Transform _menuPanel;
        [SerializeField] Transform _levelPanel;
        [SerializeField] Transform _stagePanel;
        [SerializeField] Transform _gamePanel;
        [SerializeField] float _moveTime = .5f;

        public static GameManager Instance { get; private set; }


        Transform _currentPanel;
        Transform _oldPanel;

        void Awake()
        {
            Instance = this;
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(.1f);

            _menuPanel.position = new Vector3(0, 50, 0);
            _levelPanel.position = new Vector3(0, 50, 0);
            _stagePanel.position = new Vector3(0, 50, 0);
            _gamePanel.position = new Vector3(0, 50, 0);

            GoToMenuPanel(false, false);
        }

        public void GoToMenuPanel(bool back, bool anim = true)
        {
            _oldPanel = _currentPanel;
            _currentPanel = _menuPanel;
            GoToPanel(back, anim);
        }

        public void GoToLevelPanel(bool back, bool anim = true)
        {
            _oldPanel = _currentPanel;
            _currentPanel = _levelPanel;
            GoToPanel(back, anim);
        }

        public void GoToStagePanel(bool back, bool anim = true)
        {
            _oldPanel = _currentPanel;
            _currentPanel = _stagePanel;
            GoToPanel(back, anim);
        }

        public void GoToGameplay(bool back, bool anim = true)
        {
            _gamePanel.gameObject.GetComponent<GamePanel>().RefreshBoard();
            _oldPanel = _currentPanel;
            _currentPanel = _gamePanel;
            GoToPanel(back, anim);
        }

        void GoToPanel(bool back, bool anim)
        {
            if (anim)
            {
                _oldPanel.DOMoveX(back ? 20 : -20, _moveTime).OnComplete(() => { _oldPanel.position = new Vector3(0, 50, 0); });
                var pos = _currentPanel.position;
                pos = new Vector3(back ? -20 : 20, 0, 0);
                _currentPanel.position = pos;
                _currentPanel.DOMoveX(0, _moveTime);
            }
            else
            {
                if (_oldPanel != null)
                {
                    _oldPanel.position = new Vector3(0, 50, 0);
                }

                _currentPanel.position = new Vector3(0, 0, 0);
            }
        }
    }
}