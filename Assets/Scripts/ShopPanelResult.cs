using System;
using UnityEngine;
using UnityEngine.UI;


namespace Equation
{
    public class ShopPanelResult : MonoBehaviour
    {
        [SerializeField] Text _messageText;
        [SerializeField] Text _messageText2;
        [SerializeField] Text _coinText;
        [SerializeField] Button _continueButton;
        [SerializeField] AudioSource _purchasedSound;
        [SerializeField] GameObject _flowers;

        void Start()
        {
            _continueButton.onClick.AddListener(ContinueButtonClick);
        }

        void ContinueButtonClick()
        {
            Hide();
        }
        
        public void ShowResult(int coin, string message, bool wasAd)
        {
            gameObject.SetActive(true);

            _coinText.text = $"{Translator.CROSS_SIGN}{coin}";
            _coinText.gameObject.SetActive(coin > 0);
            _messageText2.gameObject.SetActive(coin > 0);

            _messageText.text = message;

            _flowers.SetActive(coin > 0 && !wasAd);

            if (coin > 0)
            {
                GameSaveData.AddCoin(coin);
                _purchasedSound.Play();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}