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
            
            _menuPanel.position += new Vector3(0, 100, 0);
            _levelPanel.position += new Vector3(0, 100, 0);
            _stagePanel.position += new Vector3(0, 100, 0);
            _gamePanel.position += new Vector3(0, 100, 0);
            
            GoToMenuPanel(false);
        }

        public void GoToMenuPanel(bool anim = true)
        {
            _oldPanel = _currentPanel;
            _currentPanel = _menuPanel;
            GoToPanel(anim);
        }
        
        public void GoToLevelPanel(bool anim = true)
        {
            _oldPanel = _currentPanel;
            _currentPanel = _levelPanel;
            GoToPanel(anim);
        }
        
        public void GoToStagePanel(bool anim = true)
        {
            _oldPanel = _currentPanel;
            _currentPanel = _stagePanel;
            GoToPanel(anim);
        }
        
        public void GoToGameplay(bool anim = true)
        {
            _oldPanel = _currentPanel;
            _currentPanel = _gamePanel;
            GoToPanel(anim);
        }

        void GoToPanel(bool anim = true)
        {
            if (anim)
            {
                _oldPanel.DOMoveX(-20, _moveTime);
                var pos = _currentPanel.position;
                pos = new Vector3( 20, 0, 0);
                _currentPanel.position = pos;
                _currentPanel.DOMoveX(0, _moveTime);
            }
            else
            {
                if(_oldPanel != null)
                {
                    var pos = _oldPanel.position;
                    pos.x = 20;
                    _oldPanel.position = pos;
                }

                {
                    _currentPanel.position = new Vector3(0, 0, 0);
                }
            }
        }
    }
}