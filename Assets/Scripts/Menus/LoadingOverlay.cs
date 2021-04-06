using System;
using UnityEngine;
using DG.Tweening;

namespace Cacao
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] Transform _indic;
        
        void Start()
        {
            _indic.DORotate(new Vector3(0, 0, -360), 1, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        }
    }
}