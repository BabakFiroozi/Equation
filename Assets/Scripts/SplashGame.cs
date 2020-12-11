using System;
using System.Collections;
using System.Collections.Generic;
using BazaarInAppBilling;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Equation
{
	public class SplashGame : MonoBehaviour
	{
		[SerializeField] Image _splashImage = null;


		// Use this for initialization
		void Start()
		{
			Application.runInBackground = true;
			
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			_splashImage.fillAmount = 0;
			_splashImage.DOFillAmount(1, 1).SetEase(Ease.Linear).SetDelay(.1f);

			GameSaveData.IncreaseSessionNumber();

			if (DataHelper.Instance.IsFirstSession())
			{
				GameSaveData.UnlockLevel(0);
				GameSaveData.UnlockStage(0, 0);

				GameSaveData.AddCoin(GameConfig.Instance.PlayerInitialCoin);
			}

			if (GameConfig.Instance.DailyIsLocal)
				OnGetLiveTime(DateTime.Now); //Mock only for testing
			else
				SceneTransitor.Instance.GetLiveDateTime(OnGetLiveTime);

			if (GameSaveData.IsGameSoundOn())
				SoundManager.Instance.UnMuteSounds();
			else
				SoundManager.Instance.MuteSounds();

			if (GameSaveData.IsGameMusicOn())
				SoundManager.Instance.UnMuteMusics();
			else
				SoundManager.Instance.MuteMusics();

			StoreHandler.instance.InitializeBillingService(Store_ErrorHandler, Store_SuccessHandler);
		}

		void Store_SuccessHandler()
		{
			Debug.Log("<color=green>Store initialized successfully</color>");
		}

		void Store_ErrorHandler(int code, string message)
		{
			Debug.LogError($"Store initialized failed. code: {code}, message: {message}");
		}

		void OnGetLiveTime(DateTime? dateTime)
		{
			if (dateTime == null)
				Debug.LogError("Unable to get live datetime");
			
			DataHelper.Instance.CheckDailyEntrance(dateTime);

			if (DataHelper.Instance.DailyEntranceDisturbed)
			{
				//Lock again
			}

			//
			{
				//Unlock new daily
			}

			StartCoroutine(_GoToMenu());
		}


		IEnumerator<WaitForSeconds> _GoToMenu()
		{
			yield return new WaitForSeconds(1.5f);
			_splashImage.fillOrigin = 1;
			_splashImage.DOFillAmount(0, 1);
			MyAnalytics.Init("");
			yield return new WaitForSeconds(1);
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU, 0, true, false);
			MyAnalytics.SendEvent(MyAnalytics.game_entrance);
		}
	}
}