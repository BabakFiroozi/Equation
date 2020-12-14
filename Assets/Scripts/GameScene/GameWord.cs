using System;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class GameWord : MonoBehaviour
    {
        [SerializeField] Button _gridButton;
        [SerializeField] Button _fontButton;
        [SerializeField] GameObject _solvedBadge;
        [SerializeField] CoinBox _coinBox;
        [SerializeField] Board _board;
        [SerializeField] GameObject _resultPanel;
        
        public static GameWord Instance { get; private set; }
        
        public CoinBox CoinBox => _coinBox;

        public Board Board => _board;

        void Awake()
        {
            Instance = this;
        }


        void Start()
        {
            FontButtonClick(false);
            GridButtonClick(false);
            
            _fontButton.onClick.AddListener(() => FontButtonClick());
            _gridButton.onClick.AddListener(() => GridButtonClick());
            
            _solvedBadge.SetActive(GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo));
            
            _board.GameFinishedEvent += GameFinishedHandler;
        }

        void GameFinishedHandler(bool alreadySolved, int stageRank)
        {
            _resultPanel.GetComponent<ResultPanel>().ShowResult(alreadySolved, stageRank);
        }

        void FontButtonClick(bool change = true)
        {
            bool isEng = GameSaveData.IsNumberFontEng();
            if (change)
                isEng = !isEng;
            GameSaveData.SetNumberFontEng(isEng);
            var tr = _fontButton.transform;
            tr.Find("en").gameObject.SetActive(isEng);
            tr.Find("fa").gameObject.SetActive(!isEng);

            Board.SetPawnsFont(isEng);
        }

        void GridButtonClick(bool change = true)
        {
            bool visible = GameSaveData.IsGridVisible();
            if (change)
                visible = !visible;
            GameSaveData.SetGridVisible(visible);
            var tr = _gridButton.transform;
            tr.Find("on").gameObject.SetActive(visible);

            Board.SetGridVisible(visible);
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}