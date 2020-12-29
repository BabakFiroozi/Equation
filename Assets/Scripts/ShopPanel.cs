using System;
using UnityEngine;
using System.Collections;
using BazaarInAppBilling;
using CheshmakMe;
using UnityEngine.UI;
using DG.Tweening;
using TapsellSDK;


namespace Equation
{
	[RequireComponent(typeof(PopupScreen))]
	public class ShopPanel : MonoBehaviour
	{
		[SerializeField] Button[] _buyButtons = null;
		[SerializeField] Button _adButton = null;
		[SerializeField] Button _wheelButton = null;
		[SerializeField] Text _coinText = null;
		[SerializeField] Text _adCoinAmountText = null;

		[SerializeField] ShopPanelResult _shopPanelResult = null;

		[SerializeField] GameObject _spinnerPanelObj;

		[SerializeField] int[] _prices = null;
		[SerializeField] int[] _coins = null;

		PopupScreen _popupScreen;

		int _selectedProductIndex;
		
		void Awake()
		{
			PrepareAd();
		}

		// Use this for initialization
		void Start()
		{
			_shopPanelResult.gameObject.SetActive(false);

			_adButton.onClick.AddListener(ShowAd);
			_wheelButton.onClick.AddListener(WheelClick);

			RefreshCoinText();

			for(int i = 0; i < _buyButtons.Length; ++i)
			{
				int b = i;
				_buyButtons[i].onClick.AddListener(() => BuyButtonClick(b));
				_buyButtons[i].transform.Find("Image/amount").GetComponent<Text>().text = $"{Translator.CROSS_SIGN}{_coins[i]}";
				_buyButtons[i].transform.Find("price").GetComponent<Text>().text = $"{Translator.GetString("Toman")} {_prices[i]}";
			}

			_adCoinAmountText.text = $"{Translator.CROSS_SIGN}{GameConfig.Instance.FreeCoinAmount}";
			
			GameSaveData.CoinChangedEvent += CoinChangedHandler;
			
			MyTapsellAds.Instance.OnFinishedEvent += MyTapsell_OnFinishedHandler;
		}

		void CoinChangedHandler(int coin, bool anim, float vol)
		{
			RefreshCoinText();
		}

		void OnDestroy()
		{
			GameSaveData.CoinChangedEvent -= CoinChangedHandler;

			MyTapsellAds.Instance.OnFinishedEvent -= MyTapsell_OnFinishedHandler;
		}

		void RefreshCoinText()
		{
			_coinText.text = $"{Translator.CROSS_SIGN}{GameSaveData.GetCoin()}";
		}

		void WheelClick()
		{
			var obj = Instantiate(_spinnerPanelObj, transform.parent);
			obj.GetComponent<PopupScreen>().HideEvent = () => { Destroy(obj); };
			obj.GetComponent<SpinnerPanel>().Show();
			MyAnalytics.SendEvent(MyAnalytics.wheelOfFortune_button_clicked);
		}

		void BuyButtonClick(int index)
		{
			_selectedProductIndex = index;
			StoreHandler.instance.Purchase(index, Purchase_ErrorHadnler, Purchase_SuccessEvent);
			if (Application.platform == RuntimePlatform.WindowsEditor)
				Purchase_SuccessEvent(new Purchase(), index);

			MyAnalytics.SendEvent(MyAnalytics.purchase_coin_pack, index + 1);

		}

		void Purchase_SuccessEvent(Purchase detail, int index)
		{
			Debug.Log($"<color=green>Purchase_SuccessEvent index: {index}, detail: {detail}</color>");
			int coin = _coins[index];
			_shopPanelResult.ShowResult(coin, Translator.GetString("Thanks_For_Buy"), false);

			MyAnalytics.SendEvent(MyAnalytics.purchase_coin_pack_succeed, index + 1);

			var jsonObj = new JSONObject();
			var jsonData = new JSONObject();
			jsonObj.AddField("Shop_Data", jsonData);
			jsonData.Add(CheshmakLib.getCheshmakID());
			jsonData.Add(GameSaveData.GetSignupEmail());
			jsonData.Add(detail.productId);
			CheshmakLib.sendTag(jsonObj.Print());
		}

		void Purchase_ErrorHadnler(int code, string message)
		{
			switch (code)
			{
				case StoreHandler.SERVICE_IS_NOW_READY_RETRY_OPERATION:
					
					BuyButtonClick(_selectedProductIndex);
					return;
				
				case StoreHandler.ERROR_WRONG_SETTINGS:

					break;
				case StoreHandler.ERROR_BAZAAR_NOT_INSTALLED:

					break;
				case StoreHandler.ERROR_SERVICE_NOT_INITIALIZED:

					break;
				case StoreHandler.ERROR_INTERNAL:

					break;
				case StoreHandler.ERROR_OPERATION_CANCELLED:

					break;
				case StoreHandler.ERROR_CONSUME_PURCHASE:

					break;
				case StoreHandler.ERROR_NOT_LOGGED_IN:

					break;
				case StoreHandler.ERROR_HAS_NOT_PRODUCT_IN_INVENTORY:

					break;
				case StoreHandler.ERROR_CONNECTING_VALIDATE_API:

					break;
				case StoreHandler.ERROR_PURCHASE_IS_REFUNDED:

					break;
				case StoreHandler.ERROR_NOT_SUPPORTED_IN_EDITOR:

					break;
				case StoreHandler.ERROR_WRONG_PRODUCT_INDEX:

					break;
				case StoreHandler.ERROR_WRONG_PRODUCT_ID:

					break;
			}

			if (code != 5)
				_shopPanelResult.ShowResult(0, $"{Translator.GetString("Yor_Purchase_Failed")}\n<color=red>code:{code}, {message}</color>", false);
		}

		public void ShowPanel()
		{
			if (_popupScreen == null)
				_popupScreen = gameObject.GetComponent<PopupScreen>();

			_popupScreen.Show();
		}

		
		TapsellAd _freeCoinAd;

		void PrepareAd()
		{
			_adButton.gameObject.SetActive(false);

			if (!GameSaveData.HasDailyFreeCoin(GameConfig.Instance.FreeCoinDayCap))
				return;
			
			MyTapsellAds.Instance.ReqAd(MyTapsellAds.FreeCoin, false, ad =>
			{
				_freeCoinAd = ad;
				_adButton.gameObject.SetActive(true);
			}, s => { }, error => { }, s => { }, ad => { });
		}
		
		void ShowAd()
		{
			_adButton.gameObject.SetActive(false);
			MyTapsellAds.Instance.ShowAd(_freeCoinAd);
			MyAnalytics.SendEvent(MyAnalytics.freeCoin_button_clicked);
		}
		
		void MyTapsell_OnFinishedHandler(TapsellAdFinishedResult resultAd)
		{
			if(resultAd.zoneId != MyTapsellAds.FreeCoin)
				return;
			
			if (resultAd.rewarded)
			{
				if (resultAd.completed)
				{
					_shopPanelResult.ShowResult(GameConfig.Instance.FreeCoinAmount, Translator.GetString("Thanks_For_See"), true);
					GameSaveData.IncDailyFreeCoin();
				}
			}
		}
		
	}

}