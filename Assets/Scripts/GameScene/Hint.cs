using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTr;
        [SerializeField] Text _contentText;
        [SerializeField] RectTransform _lightEffect;
        [SerializeField] RectTransform _badgeEffect;
        [SerializeField] AudioSource _revealSound;

        public RectTransform RectTr => _rectTr;
        
        public string Content { get; private set; }

        public bool Revealed { get; private set; }

        public BoardCell Cell { get; private set; }

        float _initWidth;
        float _initFontSize;
        
        void Awake()
        {
            _initWidth = RectTr.rect.width;
            _initFontSize = _contentText.fontSize;

            _lightEffect.DORotate(new Vector3(0, 0, 180), 32, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
            var seq = DOTween.Sequence();
            seq.Append(_lightEffect.DOScale(1.2f, 2).SetEase(Ease.Linear));
            seq.Append(_lightEffect.DOScale(1.0f, 2).SetEase(Ease.Linear));
            seq.SetLoops(-1);
        }
        
        public void SetData(string content, BoardCell cell)
        {
            Content = content;
            Cell = cell;
            _contentText.text = Content;
            RectTr.anchoredPosition = cell.pos;
            _contentText.fontSize = (int) (RectTr.rect.width / _initWidth * _initFontSize);
        }

        public float Reveal(bool anim)
        {
            Revealed = true;
            _contentText.text = "?";
            gameObject.SetActive(true);

            float time = 0;

            if (anim)
            {
                GameSaveData.SaveUsedHints(DataHelper.Instance.LastPlayedInfo, Cell.index);
                _badgeEffect.localScale = Vector3.one * .01f;
                time = .3f;
                _badgeEffect.DOScale(1, time);
                RectTr.DOScale(1, time);
                _revealSound.Play();
            }

            return time;
        }
    }
}