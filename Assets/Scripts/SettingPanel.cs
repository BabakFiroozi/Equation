using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace Equation
{
	public class SettingPanel : MonoBehaviour
	{
		[SerializeField] Button _musicButton = null;
		[SerializeField] Button _soundButton = null;
		[SerializeField] Button _rateButton = null;
		[SerializeField] Button _shareButton = null;
		[SerializeField] Button _contactButton = null;
		[SerializeField] GameObject _musicOnBadge = null;
		[SerializeField] GameObject _musicOffBadge = null;
		[SerializeField] GameObject _soundOnBadge = null;
		[SerializeField] GameObject _soundOffBadge = null;
		[SerializeField] Text _versionText;

		static string _bundleId = "com.babgames.tablemath";

		PopupScreen _popupScreen;

		// Use this for initialization
		void Start()
		{
			_musicOnBadge.SetActive(GameSaveData.IsGameMusicOn());
			_musicOffBadge.SetActive(!GameSaveData.IsGameMusicOn());
			_soundOnBadge.SetActive(GameSaveData.IsGameSoundOn());
			_soundOffBadge.SetActive(!GameSaveData.IsGameSoundOn());
			
			
			_musicButton.onClick.AddListener(MusicClick);
			_soundButton.onClick.AddListener(SoundClick);
			_rateButton.onClick.AddListener(GoRatePage);
			_shareButton.onClick.AddListener(ShareGame);
			_contactButton.onClick.AddListener(SendEmail);

			_versionText.text = $"{Application.version} {Translator.GetString("Version")}";
		}

		void MusicClick()
		{
			GameSaveData.SetGameMusicOn(!GameSaveData.IsGameMusicOn());

			if (GameSaveData.IsGameMusicOn())
			{
				SoundManager.Instance.UnMuteMusics();
			}
			else
			{
				SoundManager.Instance.MuteMusics();
			}

			_musicOnBadge.SetActive(GameSaveData.IsGameMusicOn());
			_musicOffBadge.SetActive(!GameSaveData.IsGameMusicOn());
		}
		
		void SoundClick()
		{
			GameSaveData.SetGameSoundOn(!GameSaveData.IsGameSoundOn());

			if (GameSaveData.IsGameSoundOn())
			{
				SoundManager.Instance.UnMuteSounds();
			}
			else
			{
				SoundManager.Instance.MuteSounds();
			}

			_soundOnBadge.SetActive(GameSaveData.IsGameSoundOn());
			_soundOffBadge.SetActive(!GameSaveData.IsGameSoundOn());
		}

		// Update is called once per frame
		void Update()
		{
		}

		public void ShowSetting()
		{
			if (_popupScreen == null)
				_popupScreen = gameObject.GetComponent<PopupScreen>();
			_popupScreen.Show();

		}


		public static void GoRatePage()
		{
			if (Application.platform != RuntimePlatform.Android)
				return;
			
			MyAnalytics.SendEvent(MyAnalytics.rate_button_clicked);
			
#if UNITY_ANDROID
			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://details?id=" + _bundleId);
			AndroidJavaObject intrObject = new AndroidJavaObject("android.content.Intent", "android.intent.action.EDIT", uriObject);

			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
			currentActivity.Call("startActivity", intrObject);
#endif
		}

		void ShareGame()
		{
			if (Application.platform != RuntimePlatform.Android)
				return;
			
			MyAnalytics.SendEvent(MyAnalytics.share_button_clicked);

			string shareLink = "";
			if (GameConfig.Instance.StoreName == StoreNames.Cafebazar)
				shareLink = $"https://cafebazaar.ir/app/{_bundleId}/";
			if (GameConfig.Instance.StoreName == StoreNames.Iranapps)
				shareLink = $"http://iranapps.com/app/{_bundleId}";
			if (GameConfig.Instance.StoreName == StoreNames.Myket)
				shareLink = $"https://myket.ir/app/{_bundleId}";


#if UNITY_ANDROID
		// Get the required Intent and UnityPlayer classes.
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

		// Construct the intent.
		AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
		intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareLink);
		intent.Call<AndroidJavaObject>("setType", "text/plain");

		// Display the chooser.
		AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, "Share");
		currentActivity.Call("startActivity", chooser);
#endif
		}

		void SendEmail()
		{
			//Open email application
			MyAnalytics.SendEvent(MyAnalytics.email_button_clicked);
			SendEmail("bkfiroozi@gamil.com", Translator.GetString("Game_Name", false), "Type email body here");
		}

		void SendEmail(string toEmail, string emailSubject, string emailBody)
		{
			emailSubject = System.Uri.EscapeUriString(emailSubject);
			emailBody = System.Uri.EscapeUriString(emailBody);
			Application.OpenURL("mailto:" + toEmail + "?subject=" + emailSubject + "&body=" + emailBody);
		}
	}

}