using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using FiroozehGameService.Core;
using UnityEngine.Networking;
using UnityNative.Sharing.Example;
using Random = UnityEngine.Random;


namespace Equation
{
	public class SceneTransitor : MonoBehaviour
	{
		public const int SCENE_SPLASH = 0;
		public const int SCENE_MAIN_MENU = 1;
		public const int SCENE_LEVEL_MENU = 2;
		public const int SCENE_STAGE_MENU = 3;
		public const int SCENE_GAME = 4;


		[SerializeField] CanvasGroup _fadeBackg = null;
		[SerializeField] float _transitTime = .5f;

		[SerializeField] Canvas _canvas;

		[SerializeField] Text _tipsText;
		[SerializeField] TextAsset _tipsTextAsset;

		public static SceneTransitor Instance { get; private set; }

		public bool Transiting { get; private set; }


		public int PreviousScene { get; private set; } = SCENE_SPLASH;
		public int CurrentScene { get; private set; } = SCENE_SPLASH;

		public Action SceneTransitedEvent { get; set; }


		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
				_fadeBackg.alpha = 0;
				SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}
		}


		void Update()
		{
			if (Input.GetKeyDown(KeyCode.J))
			{
				var share = gameObject.GetComponent<TakeAndShareScreenshot>();
				share.ShareScreenshotWithText("");
			}
			
		}

		void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			_canvas.worldCamera = Camera.main;
		}

		public void TransitScene(int sceneIndex, float delay = 0, bool fade = true, bool showTrick = true)
		{
			StartCoroutine(_TransitScene(sceneIndex, delay, fade, showTrick));
		}

		IEnumerator _TransitScene(int sceneIndex, float delay, bool fade, bool showTrick)
		{
			Transiting = true;
			
			Time.timeScale = GameConfig.Instance.TimeScale;

			_tipsText.gameObject.SetActive(showTrick);
			if (showTrick)
			{
				var strsArr = _tipsTextAsset.text.Split('\n');
				string tip = strsArr[Random.Range(0, strsArr.Length)];
				_tipsText.text = Translator.FixFarsi(tip);
			}

			_fadeBackg.gameObject.SetActive(true);

			yield return new WaitForSeconds(delay);
			
			_fadeBackg.gameObject.SetActive(fade);

			if (fade)
				_fadeBackg.DOFade(1, _transitTime).SetEase(Ease.OutSine);

			yield return new WaitForSeconds(_transitTime);

			DOTween.KillAll();
			
			PreviousScene = CurrentScene;
			CurrentScene = sceneIndex;

			SceneManager.LoadScene(sceneIndex);

			_fadeBackg.DOFade(0, _transitTime);

			yield return new WaitForSeconds(_transitTime);

			_fadeBackg.gameObject.SetActive(false);
			Transiting = false;
		}

		public async void GetLiveDateTime(Action<DateTime?> onGetTime)
		{
			try
			{
				var res = await GameService.GetCurrentTime();
				var time = res.ServerTime;
				var dateTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
				var forwarded = new TimeSpan(4, 30, 0);
				if (dateTime.Month > 9 || dateTime.Month < 4)
					forwarded = new TimeSpan(3, 30, 0);
				dateTime += forwarded;
				onGetTime?.Invoke(dateTime);
				Debug.Log("GameServiceTime: " + dateTime);
			}
			catch (Exception e)
			{
				Debug.LogError($"GameService was unable to get gstime: {e.Message}");
				onGetTime?.Invoke(null);
			}
		}



		/*public Coroutine GetLiveDateTime(Action<DateTime?> onGetTime)
		{
			var c = StartCoroutine(GetLiveDateTimeCoroutine(onGetTime));
			return c;
		}*/

		IEnumerator GetLiveDateTimeCoroutine(Action<DateTime?> onGetTime)
		{
			var worldTimeApi = GameConfig.Instance.WorldTimeAPI;
			
			using (var req = UnityWebRequest.Get(worldTimeApi.url))
			{
				req.method = UnityWebRequest.kHttpVerbGET;

				req.timeout = 2;

				yield return req.SendWebRequest();

				if (string.IsNullOrWhiteSpace(req.error))
				{
					var jsonObj = JSONObject.Create(req.downloadHandler.text);
					string dateTimeString = jsonObj[worldTimeApi.field].str;
					var dateTime = DateTime.Parse(dateTimeString);
					onGetTime?.Invoke(dateTime);
					Debug.Log(dateTime);
				}
				else
				{
					Debug.LogError($"Error HttpGet from URL - code: {req.responseCode}, error: {req.error}");
					onGetTime?.Invoke(null);
				}
			}
		}


		public Coroutine DownloadTexture(string url, Action<Texture2D, string> loadedEvent)
		{
			var c = StartCoroutine(_DownloadTextureCoroutine(url, loadedEvent));
			return c;
		}
		
		IEnumerator<YieldInstruction> _DownloadTextureCoroutine(string url, Action<Texture2D, string> loadedEvent)
		{
			using (var webReq = UnityWebRequestTexture.GetTexture(url))
			{
				yield return webReq.SendWebRequest();

				if (webReq.isNetworkError || webReq.isHttpError)
				{
					Debug.LogError($"Problem with downloading texture - <color=red>{webReq.error} from url:{url}</color>");
					loadedEvent?.Invoke(null, webReq.error);
				}
				else
				{
					var tex2D = DownloadHandlerTexture.GetContent(webReq);
					loadedEvent?.Invoke(tex2D, "");
				}
			}
		}
	}

}