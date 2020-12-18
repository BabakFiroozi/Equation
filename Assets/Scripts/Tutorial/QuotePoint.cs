using System;
using DG.Tweening;
using UnityEngine;

namespace Cacao
{
    public class QuotePoint : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTr;
        [SerializeField] float _delay = 0;

        void Start()
        {
            _rectTr.localScale = Vector3.zero;
            _rectTr.DOScale(Vector3.one, .5f).SetEase(Ease.OutBounce).SetDelay(_delay);
        }
    }
}