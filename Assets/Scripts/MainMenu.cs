using System;
using DG.Tweening;
using Equation.Models;
using FiroozehGameService.Core;
using FiroozehGameService.Models;
using TapsellSDK;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Equation
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button _playButton;
        [SerializeField] Button _continueButton;
        [SerializeField] Text _startText;
        [SerializeField] Text _continueText;
        [SerializeField] Button _shopButton;
        [SerializeField] Button _dailyButton;
        [SerializeField] RectTransform _dailyNotif;
        [SerializeField] Button _leaderboardButton;
        [SerializeField] ShopPanel _shopPanel;
        [SerializeField] DailyPuzzlePanel _dailyPuzzlePanel;
        [SerializeField] SignUpPanel _signupPanel;
        [SerializeField] LeaderboardPanel _leaderboardPanel;
        [SerializeField] ConfirmScreen _quitPopup;
        [SerializeField] NativeBannerLayout _nativeBannerLayout;
        [SerializeField] RectTransform _otherProduct;
        [SerializeField] Transform _logoTr;
        
        static TapsellNativeBannerAd _nativeBannerAd;
        static bool _nativeBannerSeen;

        int _totalStarsCount;


        void Start()
        {
            _playButton.onClick.AddListener(PlayButtonClick);
            _continueButton.onClick.AddListener(ContinueButtonClick);
            _shopButton.onClick.AddListener(ShopButtonClick);
            _leaderboardButton.onClick.AddListener(LeaderboardButtonClick);
            _dailyButton.onClick.AddListener(DailyButtonClick);
            
            _dailyNotif.gameObject.SetActive(!GameSaveData.DailyVisited(GameSaveData.GetDailyEntranceNumber()));

            _logoTr.localScale = Vector3.one * .9f;
            _logoTr.DOScale(Vector3.one, .2f).SetEase(Ease.OutFlash).SetDelay(.2f);

            var seq = DOTween.Sequence();
            seq.Append(_dailyNotif.DOScale(.7f, .5f));
            seq.Append(_dailyNotif.DOScale(1.0f, .5f));
            seq.SetLoops(-1);

            CalcLastPlayed();
            
            if (TutorialCanvas_Gameplay.Instance != null)
                TutorialCanvas_Gameplay.Instance.GoToCurrentStep();

            if (DataHelper.Instance.LastPlayedInfo.Level == 0 && DataHelper.Instance.LastPlayedInfo.Stage == 0)
            {
                _startText.rectTransform.anchoredPosition = Vector2.zero;
                _startText.text = Translator.GetString("Start");
                _continueText.gameObject.SetActive(false);
            }
            else
            {
                var info = DataHelper.Instance.LastPlayedInfo;
                _startText.text = Translator.GetString("Continue");
                _continueText.text = $"{info.Stage + 1} {Translator.GetString("Stage")} - {info.Level + 1} {Translator.GetString("Level")}";
            }

            if (Random.Range(0, 100) < GameConfig.Instance.ExitAdChance && _nativeBannerAd == null)
            {
                MyTapsellAds.Instance.ReqNativeBannerAd(this, MyTapsellAds.GameExit,
                    ad => { _nativeBannerAd = ad; },
                    msg => { },
                    error => { },
                    onNoNetworkEvent => { }
                );
            }

            _totalStarsCount = 1;
            var playedInfo = new PuzzlePlayedInfo();
            for (int i = 0; i < DataHelper.Instance.LevelsCount; ++i)
            {
                var level = Resources.Load<TextAsset>($"Puzzles/level_{i:000}");
                var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
                foreach (var puzzle in puzzlesPack.puzzles)
                {
                    playedInfo.Level = puzzlesPack.level;
                    playedInfo.Stage = puzzle.stage;
                    int rank = GameSaveData.GetStageRank(playedInfo);
                    _totalStarsCount += rank;
                }
            }

            //Other product
            {
                Vector2 otherProductPos = _otherProduct.anchoredPosition;
                _otherProduct.anchoredPosition = new Vector2(otherProductPos.x, -otherProductPos.y);
                if (TutorialCanvas_Gameplay.Instance == null)
                {
                    if (Random.Range(0, 100) < 50)
                    {
                        _nativeBannerSeen = true;
                        _otherProduct.DOAnchorPos(otherProductPos, .5f).SetEase(Ease.OutBack).SetDelay(.5f);
                        _otherProduct.GetComponent<Button>().onClick.AddListener(() => { Application.OpenURL("https://cafebazaar.ir/app/com.babgames.ganjyab"); });
                    }
                }
            }
            
            SubmitScoreAsync();
            
            Invoke(nameof(RemoveBannerAd), 3);
            
        }
        void RemoveBannerAd()
        {
            CheshmakMe.CheshmakLib.removeBannerAds();
        }
        
        void DailyButtonClick()
        {
            int day = GameSaveData.GetDailyEntranceNumber();
            _dailyPuzzlePanel.Show();
            _dailyNotif.gameObject.SetActive(false);
            GameSaveData.VisitDaily(day, true);
            MyAnalytics.SendEvent(MyAnalytics.daily_button_clicked);
        }


        void LeaderboardButtonClick()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                var tr = _leaderboardButton.transform.Find("popup");
                if (tr.gameObject.activeSelf)
                    return;
                tr.gameObject.SetActive(true);
                tr.DOKill();
                tr.localScale = new Vector3(.2f, .2f, 1);
                tr.DOScale(new Vector3(1, 1, 1), .3f);
                tr.DOScale(new Vector3(.2f, .2f, 1), .3f).SetDelay(1).onComplete = () => tr.gameObject.SetActive(false);
                return;
            }
			
            ShowLeaderboardAsync();
            MyAnalytics.SendEvent(MyAnalytics.leaderboard_button_clicked);
        }

        async void ShowLeaderboardAsync()
        {
            if (GameService.IsAuthenticated())
            {
                _leaderboardPanel.Show();
                return;
            }

            _signupPanel.ShowPanel(async isLogin =>
            {
                try
                {
                    _signupPanel.HidePanel();
                    await GameService.SubmitScore(GameConfig.Instance.LeaderboardId, _totalStarsCount);
                    _leaderboardPanel.GetComponent<LeaderboardPanel>().Show();
                }
                catch (GameServiceException e)
                {
                    Debug.LogError(e.Message);
                }
            });
        }


        async void SubmitScoreAsync()
        {
            string token = GameSaveData.GetPlayerToken();
            if (token != string.Empty && !GameService.IsAuthenticated())
                await GameService.Login(token);
            if (GameService.IsAuthenticated())
            {
                await GameService.SubmitScore(GameConfig.Instance.LeaderboardId, _totalStarsCount);
            }
        }


        float _quitMenuTimer;

        void Update()
        {
            if (_quitMenuTimer < 0)
            {
                //TODO-check tutorial
                if (!PopupScreen.AnyPopupOnTop() && !ConfirmScreen.AnyPopupOnTop() /*&& TutorialCanvas_Gameplay.Instance == null*/)
                {
                    if (Input.GetKeyUp(KeyCode.Escape) && !_quitPopup.IsBusy && !_quitPopup.IsOpening)
                    {
                        _quitMenuTimer = 2;

                        if (_nativeBannerAd != null && !_nativeBannerSeen)
                        {
                            var obj = Instantiate(_nativeBannerLayout, transform.parent);
                            obj.GetComponent<NativeBannerLayout>().ShowAd(_nativeBannerAd);
                            _nativeBannerAd = null;
                            _nativeBannerSeen = true;
                        }
                        else
                        {
                            _quitPopup.OpenConfirm(type =>
                            {
                                if (type == ConfirmScreen.ConfirmTypes.Ok)
                                    Application.Quit();
                            });
                        }
                    }
                }
            }
            else
            {
                _quitMenuTimer -= Time.deltaTime;
            }
        }

        void ShopButtonClick()
        {
            _shopPanel.ShowPanel();
            MyAnalytics.SendEvent(MyAnalytics.shop_button_clicked);
        }

        void CalcLastPlayed()
        {
            int levels = DataHelper.Instance.LevelsCount;
            int lastUnlockedLevel = -1;
            for (int l = 0; l < levels; ++l)
            {
                if (GameSaveData.IsLevelUnlocked(l))
                    lastUnlockedLevel++;
            }

            var level = Resources.Load<TextAsset>($"Puzzles/level_{lastUnlockedLevel:000}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
            var playedInfo = new PuzzlePlayedInfo();
            playedInfo.Level = lastUnlockedLevel;
            for (int s = 0; s < puzzlesPack.puzzles.Count; ++s)
            {
                playedInfo.Stage = s;
                if (GameSaveData.IsStageUnlocked(lastUnlockedLevel, s) && !GameSaveData.IsStageSolved(playedInfo))
                    break;
            }

            DataHelper.Instance.LastPlayedInfo.Daily = false;
            DataHelper.Instance.LastPlayedInfo.Level = playedInfo.Level;
            DataHelper.Instance.LastPlayedInfo.Stage = playedInfo.Stage;
        }

        void PlayButtonClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU, true);
        }

        void ContinueButtonClick()
        {
            var tr = _continueButton.transform;
            tr.DOScale(1.1f, .15f);
            tr.DOScale(1.0f, .15f).SetDelay(.15f);
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, true);
        }

    }
}