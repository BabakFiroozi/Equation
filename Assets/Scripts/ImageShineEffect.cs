using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Cacao
{
    public class ImageShineEffect : MonoBehaviour
    {
        [SerializeField] Material _shineMaterial;
        [SerializeField] float _shineSpeed = 1;
        [SerializeField] float _shineDelay = .5f;
        [SerializeField] float _shineInterval = 3;
        [SerializeField] bool _random;

        Sequence _sequence;
        Material _mat;
        

        void OnEnable()
        {
            if(_mat == null)
            {
                var image = GetComponent<Image>();
                image.material = Instantiate<Material>(_shineMaterial);
                _mat = image.material;
            }
            
            _sequence?.Kill();

            var seq = DOTween.Sequence();
            seq.SetDelay(_shineDelay);
            seq.Append(_mat.DOFloat(0, "_ShineLocation", 0));
            seq.Append(_mat.DOFloat(1, "_ShineLocation", _shineSpeed));
            seq.AppendInterval(_shineInterval);
            seq.SetLoops(-1);
            _sequence = seq;
        }
    }
}