using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

namespace Equation
{

    public enum PopupAnimTypes
    {
        Move,
        Scale
    }

    public class PopupScreen : MonoBehaviour
    {
        [SerializeField] PopupAnimTypes _animType;
        [SerializeField] Button _closeButton;
        [SerializeField] Image _backCloseImage;
        [SerializeField] RectTransform _frameRectTr;
        [SerializeField] bool _allowHideWithEsc = true;
        [SerializeField] bool _moveHor;
        [SerializeField] int _moveDir = 1;
        [SerializeField] bool _avtiveWhenHidden = false;

        const float _animTime = .4f;

        [SerializeField] AudioSource _animAudio;

        static List<PopupScreen> s_popupsList = new List<PopupScreen>();
        
        public Action BeforeShowEvent { get; set; }

        public Action ShowEvent { get; set; }
        public Action HideEvent { get; set; }


        public bool IsBusy { get; private set; }

        public bool IsMoving { get; private set; }
        

        private Vector2 _initPos;
        
        bool _allowHide = true;

        Rect _backCloseRect;

        void Awake()
        {
            _initPos = _frameRectTr.anchoredPosition;
            _backCloseRect = _backCloseImage.rectTransform.rect;
        }

        void Start()
        {
            _closeButton.onClick.AddListener(() => Hide());
            _backCloseImage.gameObject.GetComponent<Button>().onClick.AddListener(() => Hide());
        }
        
        void OnEnable()
        {
            var tr = transform;
            tr.SetSiblingIndex(tr.parent.childCount - 2);
        }


        public void Show()
        {
            if (IsBusy)
                return;

            gameObject.SetActive(true);
            StartCoroutine(ShowCoroutine());
        }

        IEnumerator ShowCoroutine()
        {
            IsMoving = true;

            IsBusy = true;
            
            BeforeShowEvent?.Invoke();

            s_popupsList.Add(this);

            _backCloseImage.raycastTarget = true;

            if (_animAudio != null)
                _animAudio.Play();
            
            if (_animType == PopupAnimTypes.Move)
            {
                Vector2 pos = _initPos;

                if (!_moveHor)
                    pos.y += _moveDir * _backCloseRect.height;
                else
                    pos.x += _moveDir * _backCloseRect.width;

                _frameRectTr.anchoredPosition = pos;
                _frameRectTr.DOAnchorPos(_initPos, _animTime).SetEase(Ease.OutBack);
            }
            if (_animType == PopupAnimTypes.Scale)
            {
                _frameRectTr.localScale = Vector3.one * .1f;
                _frameRectTr.DOScale(Vector3.one, _animTime).SetEase(Ease.OutBack);
            }

            var col = _backCloseImage.color;
            col.a = 0;
            _backCloseImage.color = col;
            _backCloseImage.DOFade(.75f, _animTime).SetEase(Ease.Linear);

            yield return new WaitForSeconds(_animTime);

            ShowEvent?.Invoke();

            IsMoving = false;
        }

        public void Hide(bool fast = false)
        {
            if(!_allowHide)
                return;
            
            if (IsMoving)
                return;

            StartCoroutine(HideCoroutine(fast));
        }

        IEnumerator HideCoroutine(bool fast)
        {
            IsMoving = true;

            _backCloseImage.raycastTarget = false;

            if (_animType == PopupAnimTypes.Move)
            {
                Vector2 pos = _initPos;

                if (!_moveHor)
                    pos.y += _moveDir * _backCloseRect.height;
                else
                    pos.x += _moveDir * _backCloseRect.height;

                if (!fast)
                    _frameRectTr.DOAnchorPos(pos, _animTime).SetEase(Ease.InBack);
                else
                    _frameRectTr.anchoredPosition = pos;
            }

            if (_animType == PopupAnimTypes.Scale)
            {
                _frameRectTr.localScale = Vector3.one;
                _frameRectTr.DOScale(Vector3.one * .1f, _animTime).SetEase(Ease.InBack);
            }

            _backCloseImage.DOFade(0, !fast ? _animTime : 0).SetEase(Ease.Linear);

            yield return new WaitForSeconds(_animTime);

            IsBusy = false;

            s_popupsList.Remove(this);

            IsMoving = false;
            
            gameObject.SetActive(_avtiveWhenHidden);
            
            HideEvent?.Invoke();
        }

        void Update()
        {
            if (_allowHideWithEsc && Input.GetKeyUp(KeyCode.Escape))
            {
                var popup = s_popupsList.LastOrDefault();
                if (popup != null && !popup.IsMoving && !ConfirmScreen.AnyPopupOnTop())
                {
                    popup.Hide();
                }
            }
        }

        void OnDestroy()
        {
            if (s_popupsList.Contains(this))
                s_popupsList.Remove(this);
        }

        public void AllowHide(bool allow)
        {
            _allowHide = allow;
        }

        public static bool AnyPopupOnTop()
        {
            return s_popupsList.Count > 0;
        }
    }
}