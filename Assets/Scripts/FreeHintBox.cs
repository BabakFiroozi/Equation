using System;
using UnityEngine;
using UnityEngine.UI;
using TapsellSDK;
using DG.Tweening;
using Random = UnityEngine.Random;


namespace Equation
{
    public class FreeHintBox : MonoBehaviour
    {
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] Text _messageText;
        [SerializeField] Button _continueButton;

        Vector2 _initPos;
        TapsellAd _freeHintAd;
        RectTransform _rectTr;

        bool _done;

        void Awake()
        {
            if (!GameSaveData.HasDailyFreeGuide(GameConfig.Instance.FreeGuideDayCap))
            {
                gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (_done)
                return;

            if (GameWord.Instance != null && GameWord.Instance.Board.GameFinished)
            {
                _done = true;
                Dismiss(true);
            }
        }

        void Start()
        {
            _rectTr = gameObject.GetComponent<RectTransform>();

            _canvasGroup.DOFade(0, 0);
            _messageText.DOFade(0, 0);

            const float fade_time = .2f;
            const float interval_time = .5f;

            var seq = DOTween.Sequence();
            seq.SetLoops(-1);
            seq.Append(_canvasGroup.DOFade(0, fade_time).SetEase(Ease.Linear));
            seq.Append(_messageText.DOFade(1, fade_time).SetEase(Ease.Linear));
            seq.AppendInterval(interval_time);
            seq.Append(_messageText.DOFade(0, fade_time).SetEase(Ease.Linear));
            seq.Append(_canvasGroup.DOFade(1, fade_time).SetEase(Ease.Linear));
            seq.AppendInterval(interval_time);

            _continueButton.onClick.AddListener(OnClickContinue);

            _rectTr.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                Dismiss();
            });

            MyTapsellAds.Instance.OnFinishedEvent += MyTapsell_OnFinishedHandler;

            PrepareAd();
        }

        void OnDestroy()
        {
            MyTapsellAds.Instance.OnFinishedEvent -= MyTapsell_OnFinishedHandler;
        }

        void Dismiss(bool onlyHide = false)
        {
            if (_freeHintAd == null)
                return;
            _rectTr.DOAnchorPosY(_initPos.y, .3f);
            
            if(onlyHide)
                return;
            
            MyTapsellAds.Instance.ShowAd(_freeHintAd);
            MyAnalytics.SendEvent(MyAnalytics.freeGuide_button_clicked);
        }

        void OnClickContinue()
        {
            _continueButton.gameObject.SetActive(false);
            GameWord.Instance.Board.DoHint();
        }

        void PrepareAd()
        {
            _initPos = _rectTr.anchoredPosition;
            _rectTr.anchoredPosition = new Vector2(_initPos.x, -_initPos.y);
            
            var info = DataHelper.Instance.LastPlayedInfo;
            if (info.Level == 0 && info.Level == 0 && info.Stage < 5)
                return;

            _freeHintAd = null;
            MyTapsellAds.Instance.ReqAd(MyTapsellAds.FreeHint, false, ad =>
            {
                _freeHintAd = ad;
                _rectTr.DOAnchorPosY(_initPos.y, .3f);
            }, s => { }, error => { }, s => { }, ad => { });
        }
        
        void MyTapsell_OnFinishedHandler(TapsellAdFinishedResult resultAd)
        {
            if(resultAd.zoneId != MyTapsellAds.FreeHint)
                return;
            
            if (resultAd.rewarded)
            {
                if (resultAd.completed)
                {
                    _continueButton.gameObject.SetActive(true);
                    GameSaveData.IncDailyFreeGuide();
                }
            }
        }
    }
}