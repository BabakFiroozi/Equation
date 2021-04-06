using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Cacao
{
    public class PageStepScrollHorArea : MonoBehaviour, IDragHandler
    {
        [SerializeField] PageStepScrollHor _pageStepScroll;
        Image _pagesScrollRectImage;

        void Awake()
        {
            _pagesScrollRectImage = gameObject.GetComponent<Image>();
        }
        
        void Update()
        {
            _pagesScrollRectImage.raycastTarget = !_pageStepScroll.PageIsScrolling;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData is PointerEventData pointerEventData && pointerEventData.dragging)
            {
                var delta = pointerEventData.delta;

                float horSens = Screen.width * (15f / 720);
                float verSens = Screen.height * (3f / 1280);

                float x = Mathf.Abs(delta.x);
                float y = Mathf.Abs(delta.y);
                
                if (x > horSens && y < verSens)
                {
                    if(delta.x > horSens)
                        _pageStepScroll.GoToNextPage();
                    if(delta.x < -horSens)
                        _pageStepScroll.GoToPreviousPage();
                }
            }
        }
    }
}