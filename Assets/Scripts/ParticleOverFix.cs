using UnityEngine;
using UnityEngine.UI;

namespace Cacao
{
    public class ParticleOverFix : MonoBehaviour
    {
        [SerializeField] int _sortingOrder = 4;
        //we add these to prevent showing particle over popup
        Canvas _canvas;
        GraphicRaycaster _rayCaster;
        void OnEnable()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = _sortingOrder;
            _rayCaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        void OnDisable()
        {
            Destroy(_rayCaster);
            Destroy(_canvas);
        }
    }
}