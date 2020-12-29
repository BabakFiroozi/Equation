using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;


namespace Equation
{
    public class ConfirmScreen : MonoBehaviour
    {
        public enum ConfirmTypes
        {
            Ok,
            Cancel,
            Close
        }


        [SerializeField] Button _closeButton = null;
        [SerializeField] Button _okButton = null;
        [SerializeField] Button[] _cancelButtons = null;

        [SerializeField] RectTransform _framRectTr = null;

        [SerializeField] bool _notInteractableWhenPressed = false;

        [SerializeField] float _okDelay = 0;
        
        [SerializeField] Image _backgImage = null;
        
        [SerializeField] bool _allowHideWithEsc = true;

        const float _animTime = .4f;

        Action<ConfirmTypes> ConfirmedEvent { get; set; }
        
        public Action ClosedEvent { get; set; }
        
        static List<ConfirmScreen> s_popupsList = new List<ConfirmScreen>();

        [SerializeField] AudioSource _animAudio;

        public bool IsOpening { get; private set; }
        public bool IsClosing { get; private set; }

        public bool IsBusy { get; private set; }


        // Use this for initialization
        void Start()
        {
            _closeButton.onClick.AddListener(CloseButtonClick);
            _okButton.onClick.AddListener(OkButtonClick);
            foreach (var button in _cancelButtons)
                button.onClick.AddListener(CancelButtonClick);
        }

        void OnEnable()
        {
            var tr = transform;
            tr.SetSiblingIndex(tr.parent.childCount - 2);
        }

        void CancelButtonClick()
        {
            if(IsOpening)
                return;
            
            if(IsClosing)
                return;
            
            if (!_okButton.enabled)
                return;
            
            ConfirmedEvent?.Invoke(ConfirmTypes.Cancel);
            CloseConfirm();
        }

        void CloseButtonClick()
        {
            if(IsOpening)
                return;
            
            if(IsClosing)
                return;
            
            if (!_okButton.enabled)
                return;
            
            ConfirmedEvent?.Invoke(ConfirmTypes.Close);
            CloseConfirm();
        }

        void OkButtonClick()
        {
            if(IsOpening)
                return;
            
            _okButton.interactable = !_notInteractableWhenPressed;
            ConfirmedEvent?.Invoke(ConfirmTypes.Ok);
        }


        public void OpenConfirm(Action<ConfirmTypes> confirmedEvent, bool withAnim = true)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            
            IsOpening = true;

            IsClosing = false;
            
            s_popupsList.Add(this);
            
            gameObject.SetActive(true);
            
            _okButton.enabled = true;
            _backgImage.raycastTarget = true;
            
            if(_animAudio != null)
                _animAudio.Play();

            if (_okDelay > 0)
            {
                var canvasGroup = _okButton.gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = _okButton.gameObject.AddComponent<CanvasGroup>();
                canvasGroup.DOKill();
                canvasGroup.alpha = 0;
                _okButton.enabled = false;
                foreach (var btn in _cancelButtons)
                    btn.enabled = false;
                _closeButton.enabled = false;
                canvasGroup.DOFade(1, .5f).SetDelay(_okDelay).onComplete = () =>
                {
                    _okButton.enabled = true;
                    _closeButton.enabled = true;
                    foreach (var btn in _cancelButtons)
                        btn.enabled = true;
                };
            }

            _okButton.interactable = true;
            _framRectTr.gameObject.SetActive(true);

            if (withAnim)
            {
                _framRectTr.localScale = Vector3.one * .1f;
                _framRectTr.DOScale(Vector3.one, _animTime).SetEase(Ease.OutBack);
                var color = _backgImage.color;
                _backgImage.color = new Color(color.r, color.g, color.b, 0);
                _backgImage.DOFade(.75f, _animTime).SetEase(Ease.Linear).onComplete = () =>
                {
                    IsOpening = false;
                };
            }
            else
            {
                _framRectTr.localScale = Vector3.one;
            }

            ConfirmedEvent = confirmedEvent;

            // SoundManager.Instance.PlaySound(SoundNames.PopupConfirm);
        }
        
        public void CloseConfirm()
        {
            IsClosing = true;
            _framRectTr.DOScale(Vector3.one * .1f, _animTime).SetEase(Ease.InBack);
            _backgImage.DOFade(0, _animTime).SetEase(Ease.Linear).onComplete = () =>
            {
                IsBusy = false;
                gameObject.SetActive(false);
                ClosedEvent?.Invoke();
            };
            
            s_popupsList.Remove(this);
        }

        void Update()
        {
            if (_allowHideWithEsc && Input.GetKeyUp(KeyCode.Escape))
            {
                CancelButtonClick();
            }
        }
        
        void OnDestroy()
        {
            if (s_popupsList.Contains(this))
                s_popupsList.Remove(this);
        }

        public void ShowButtons(bool onlyOk)
        {
            _okButton.gameObject.SetActive(true);
            if (!onlyOk)
            {
                foreach (var button in _cancelButtons)
                    button.gameObject.SetActive(true);
            }
        }
        
        public static bool AnyPopupOnTop()
        {
            return s_popupsList.Count > 0;
        }
    }
}