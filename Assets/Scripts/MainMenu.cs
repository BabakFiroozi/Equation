using System;
using Equation.Models;
using FiroozehGameService.Core;
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
        [SerializeField] GameObject _shopPanelObj;
        [SerializeField] ConfirmScreen _quitPopup;
        [SerializeField] NativeBannerLayout _nativeBannerLayout;

        static TapsellNativeBannerAd _nativeBannerAd;
        static bool _nativeBannerSeen;


        void Start()
        {
            _playButton.onClick.AddListener(PlayButtonClick);
            _continueButton.onClick.AddListener(ContinueButtonClick);
            _shopButton.onClick.AddListener(ShopButtonClick);

            CalcLastPlayed();

            if (GameSaveData.GetSessionNumber() == 1)
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

            SubmitScoreAsync();

            if (Random.Range(0, 100) < GameConfig.Instance.ExitAdChance && _nativeBannerAd == null)
            {
                MyTapsellAds.Instance.ReqNativeBannerAd(this, MyTapsellAds.GameExit,
                    ad => { _nativeBannerAd = ad; },
                    msg => { },
                    error => { },
                    onNoNetworkEvent => { }
                );
            }
        }


        async void SubmitScoreAsync()
        {
            string token = GameSaveData.GetPlayerToken();
            if (token != string.Empty && !GameService.IsAuthenticated())
                await GameService.Login(token);
            if (GameService.IsAuthenticated())
            {
                await GameService.SubmitScore(GameConfig.Instance.LeaderboardId, StatsHelper.Instance.TotalStagesRank);
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
            _shopPanelObj.GetComponent<ShopPanel>().ShowPanel();
        }

        void CalcLastPlayed()
        {
            int levels = DataHelper.Instance.LevelsCount;
            int lastUnlockedLevel = 0;
            for (int l = 0; l < levels; ++l)
            {
                if (GameSaveData.IsLevelUnlocked(l))
                    continue;
                lastUnlockedLevel = l - 1;
                break;
            }

            var level = Resources.Load<TextAsset>($"Puzzles/level_{lastUnlockedLevel:000}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(level.text);
            var playedInfo = new PuzzlePlayedInfo();
            playedInfo.Level = lastUnlockedLevel;
            for (int s = 0; s < puzzlesPack.puzzles.Count; ++s)
            {
                playedInfo.Stage = s;
                if (GameSaveData.IsStageUnlocked(lastUnlockedLevel, s) && !GameSaveData.IsStageSolved(playedInfo))
                {
                    playedInfo.Stage = s;
                    break;
                }
            }

            DataHelper.Instance.LastPlayedInfo.Level = playedInfo.Level;
            DataHelper.Instance.LastPlayedInfo.Stage = playedInfo.Stage;
        }

        void PlayButtonClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU);
        }

        void ContinueButtonClick()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
        }

    }
}