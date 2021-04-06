using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using EventTrigger = UnityEngine.EventSystems.EventTrigger;

namespace Cacao
{

    public class PageStepScrollHor : MonoBehaviour
    {
        [SerializeField] Button _prevPageButton;
        [SerializeField] Button _nextPageButton;
        [SerializeField] RectTransform _pageNumberContent;
        [SerializeField] Text _pageNumberText;
        [SerializeField] RectTransform _pagesContent = null;
        [SerializeField] GameObject _indicator;
        [SerializeField] bool _isIndicator;
        [SerializeField] int _pagesCount;

        public int PageIndex { get; private set; }
        public int PagesCount { get; private set; } = -1;
        public Action PageScrolledEvent { get; set; }

        public bool PageIsScrolling { get; private set; }

        List<Image> _indicImagesList = new List<Image>();


        void Start()
        {
            _prevPageButton.onClick.AddListener(GoToPreviousPage);
            _nextPageButton.onClick.AddListener(GoToNextPage);
            _prevPageButton.interactable = false;

            if (_pagesCount > 0)
                Init(_pagesCount);
        }
        
        public void OnEndDrag (BaseEventData eventData)
        {
            print(eventData);
            if (eventData is PointerEventData pointerEventData && pointerEventData.dragging)
            {
                var delta = pointerEventData.delta;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if(delta.x > 0)
                        GoToNextPage();
                    if(delta.x < 0)
                        GoToPreviousPage();
                }
            }
        }

        public void GoToPreviousPage()
        {
            if (PageIsScrolling)
                return;

            if (PageIndex > 0)
                GoToPage(PageIndex - 1);
        }

        public void GoToNextPage()
        {
            if (PageIsScrolling)
                return;

            if (PageIndex < PagesCount - 1)
                GoToPage(PageIndex + 1);
        }

        public void GoToPage(int index, bool fast = false)
        {
            if (PagesCount == 0)
            {
                Debug.LogError("PageScrolllStep - PagesCount is Zero");
                return;
            }

            if (index == -1 || index >= PagesCount)
            {
                Debug.LogError("PageScrolllStep - Invalid page index");
                return;
            }

            PageIsScrolling = true;
            int oldPageIndex = PageIndex;
            PageIndex = index;
            Vector2 pos = _pagesContent.anchoredPosition;
            pos.x = PageIndex * (_pagesContent.rect.width / PagesCount);

            _pagesContent.DOAnchorPos(pos, fast ? 0 : .4f).SetEase(Ease.OutCubic).onComplete = () =>
            {
                PageIsScrolling = false;
                PageScrolledEvent?.Invoke();
            };

            _prevPageButton.interactable = PageIndex > 0;

            _nextPageButton.interactable = PageIndex < PagesCount - 1;

            RefreshIndicator(oldPageIndex);
        }


        void RefreshIndicator(int oldPageIndex)
        {
            _indicImagesList[oldPageIndex].DOFade(0, .3f);
            _indicImagesList[PageIndex].DOFade(1, .3f);

            _pageNumberText.text = $"{PageIndex + 1}/{PagesCount}";
        }

        public void Init(int pagesCount)
        {
            if (PagesCount != -1) //Already inited
                return;

            PagesCount = pagesCount;

            _indicator.SetActive(_isIndicator);
            _pageNumberText.gameObject.SetActive(!_isIndicator);

            for (int i = 0; i < pagesCount; ++i)
            {
                GameObject indic = _indicator;
                if (i > 0)
                    indic = Instantiate(_indicator, _indicator.transform.parent);
                var tr = indic.transform;
                tr.SetAsFirstSibling();
                var image = tr.Find("on").GetComponent<Image>();
                _indicImagesList.Add(image);
                var col = image.color;
                col.a = 0;
                image.color = col;
            }

            _nextPageButton.transform.SetAsFirstSibling();

            RefreshIndicator(0);

            _pageNumberContent.gameObject.SetActive(PagesCount > 1);
        }
    }
}