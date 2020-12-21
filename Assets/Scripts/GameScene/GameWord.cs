using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class GameWord : MonoBehaviour
    {
        [SerializeField] Button _gridButton;
        [SerializeField] Button _fontButton;
        [SerializeField] Image _solvedBadge;
        [SerializeField] CoinBox _coinBox;
        [SerializeField] Board _board;
        [SerializeField] GameObject _resultPanel;
        [SerializeField] AudioSource _solvSound;

        public static GameWord Instance { get; private set; }
        
        public CoinBox CoinBox => _coinBox;

        public Board Board => _board;

        public PuzzlePlayedInfo CurrentPlayedInfo { get; private set; }

        public PuzzlePlayedInfo NextPlayedInfo;


        void Awake()
        {
            Instance = this;
            
            Board.Init();
            
            CurrentPlayedInfo = DataHelper.Instance.LastPlayedInfo.Copy();
            NextPlayedInfo = CurrentPlayedInfo.Copy();
            
            if (CurrentPlayedInfo.Stage < Board.StagesCount - 1)
            {
                NextPlayedInfo.Stage++;
            }
            else
            {
                if (CurrentPlayedInfo.Level < DataHelper.Instance.LevelsCount - 1)
                {
                    NextPlayedInfo.Stage = 0;
                    NextPlayedInfo.Level++;
                }
            }
        }


        void Start()
        {
            FontButtonClick(false);
            GridButtonClick(false);
            
            _fontButton.onClick.AddListener(() => FontButtonClick());
            _gridButton.onClick.AddListener(() => GridButtonClick());
            
            _board.GameFinishedEvent += GameFinishedHandler;

            _solvedBadge.fillAmount = 0;
            
            Invoke(nameof(RemoveBannerAd), 3);
        }
        
        void RemoveBannerAd()
        {
            CheshmakMe.CheshmakLib.removeBannerAds();
        }

        void GameFinishedHandler(bool alreadySolved, int stageRank)
        {
            _solvSound.PlayDelayed(.2f);
            _solvedBadge.DOFillAmount(1, .4f).SetDelay(.3f);

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

            MyAnalytics.SendEvent(MyAnalytics.lang_button_clicked);
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
            
            MyAnalytics.SendEvent(MyAnalytics.grid_button_clicked);
        }

        void OnDestroy()
        {
            _board.GameFinishedEvent -= GameFinishedHandler;
            
            if (Instance == this)
                Instance = null;
        }
    }
}