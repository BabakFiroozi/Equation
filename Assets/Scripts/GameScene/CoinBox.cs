using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Equation.Models;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace Equation
{
    public class CoinBox : MonoBehaviour
    {
        [SerializeField] Button _buyCoinButton;
        [SerializeField] Text _coinText;
        [SerializeField] Text _coinChangedText;
        [SerializeField] ShopPanel _shopPanel;

        [SerializeField] AudioSource _coinSound;
        [SerializeField] AudioSource _coinsSound;

        [SerializeField] int _sortingOrder = 7;

        [SerializeField] Renderer _effectRenderer;

        [SerializeField] ParticleSystem _effect;
        
        Canvas _canvas = null;
        
        void Start()
        {
            GameSaveData.CoinChangedEvent += CoinChangedEvent;

            _buyCoinButton.onClick.AddListener(BuyCoinButtonClick);
            RefreshCoinText(false);
        }
        
        void CoinChangedEvent(int coin, bool anim, float vol)
        {
            StartCoroutine(_CoinChangedEvent(coin, anim,vol));
        }

        IEnumerator<WaitForSeconds> _CoinChangedEvent(int coinDiff, bool anim, float vol)
        {
            int effectSortingOrder = _effectRenderer.sortingOrder;
            if (anim && _canvas == null)
            {
                _canvas = gameObject.AddComponent<Canvas>();
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = _sortingOrder;
                _effectRenderer.sortingOrder = _sortingOrder + 1;
            }

            // if (coinDiff > 0)
            // {
            //     _effect.gameObject.SetActive(true);
            //     _effect.Play();
            // }

            if (anim)
            {
                int coinCount =  Mathf.Abs(coinDiff);
                const float min_interval = .15f;
                float interval = min_interval;
                float coinStep = Mathf.Sign(coinDiff) * 1;

                if (coinDiff > 10)
                {
                    coinStep += (coinDiff / 10f) * Mathf.Sign(coinStep);
                }
                
                if (coinDiff < 0)
                {
                    coinStep = Mathf.Sign(coinDiff) * (coinCount / 3f);
                    interval = .4f / 3f;
                }

                _coinChangedText.DOKill();
                _coinChangedText.text = (coinDiff > 0 ? "+" : "") + $"{coinDiff}";
                _coinChangedText.DOFade(1, 0);
                _coinChangedText.color = coinDiff < 0 ? Color.red : Color.green;

                _coinSound.volume = vol;

                var waitForSec = new WaitForSeconds(interval);

                float targetCoin = GameSaveData.GetCoin();
                float currentCoin = targetCoin - coinDiff;
                do
                {
                    currentCoin += coinStep;
                    _coinText.text = $"{(int) currentCoin}";
                    _coinSound.Play();
                    yield return waitForSec;
                } while (coinDiff > 0 ? currentCoin < targetCoin : currentCoin > targetCoin);

                _coinText.text = $"{(int) targetCoin}";

                _coinChangedText.DOFade(0, .5f).SetDelay(1);
            }
            else
            {
                _coinText.text = GameSaveData.GetCoin().ToString();
                _coinChangedText.DOFade(0, .5f).SetDelay(1.5f);
                _coinsSound.volume = vol;
                _coinsSound.Play();
            }

            if (_canvas != null)
            {
                yield return new WaitForSeconds(.3f);
                Destroy(_canvas);
                _canvas = null;
                _effectRenderer.sortingOrder = effectSortingOrder;
            }
            
            // _effect.Stop();
        }


        public bool CheckEnoughCoin(int needCoin, bool showShop = true)
        {
            bool notEnoughHint = GameSaveData.GetCoin() < needCoin;
            if (notEnoughHint)
            {
                if (showShop)
                    _shopPanel.ShowPanel(true);
                return false;
            }

            return true;
        }
        
        void BuyCoinButtonClick()
        {
            _shopPanel.ShowPanel();
            MyAnalytics.SendEvent(MyAnalytics.shop_button_clicked_ingame);
        }
        
        void RefreshCoinText(bool anim)
        {
            _coinText.text = GameSaveData.GetCoin().ToString();
        }

        void OnDestroy()
        {
            GameSaveData.CoinChangedEvent -= CoinChangedEvent;
        }
    }
}