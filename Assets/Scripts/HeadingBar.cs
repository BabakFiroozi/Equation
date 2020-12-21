using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Equation
{
	public enum HeadingBarTypes
	{
		None = -1,
		Level,
		Stage,
		Game,
		Count
	}

	public class HeadingBar : MonoBehaviour
	{
		[SerializeField] Text _titleText = null;
		[SerializeField] Button _backButton = null;
		[SerializeField] Button _infoButton = null;
		[SerializeField] Button _nextButton = null;
		[SerializeField] HeadingBarTypes _headingType = HeadingBarTypes.None;

		[SerializeField] GameObject _infoPanelObj = null;

		public HeadingBarTypes HeadingType => _headingType;



		// Use this for initialization
		void Start()
		{
			_backButton.onClick.AddListener(BackButtonClick);
			_infoButton.onClick.AddListener(InfoButtonClick);
			_nextButton.onClick.AddListener(NextButtonClick);

			if (_headingType == HeadingBarTypes.Level)
			{
				string title = $"<color=#F0FF00>{Translator.GetString("Levels")}</color>";
				SetData(title);
			}

			if (_headingType == HeadingBarTypes.Stage)
			{
				var playedInfo = DataHelper.Instance.LastPlayedInfo;
				string title = $"<color=#73D6FF>{playedInfo.Level + 1}</color> <color=#F0FF00>{Translator.GetString("Level")}</color>";
				SetData(title);
			}
			
			if (_headingType == HeadingBarTypes.Game)
			{
				var playedInfo = DataHelper.Instance.LastPlayedInfo;
				string title = $"<color=#73D6FF>{playedInfo.Stage + 1}</color> <color=#F0FF00>{Translator.GetString("Stage")}</color>  <color=#73D6FF>{playedInfo.Level + 1}</color> <color=#F0FF00>{Translator.GetString("Level")}</color>";
				SetData(title);

				var nextPlayedInfo = GameWord.Instance.NextPlayedInfo;
				var currentPlayedInfo = GameWord.Instance.CurrentPlayedInfo;
				bool nextIsUnlock = GameSaveData.IsStageSolved(currentPlayedInfo) && GameSaveData.IsStageUnlocked(nextPlayedInfo.Level, nextPlayedInfo.Stage);
				_nextButton.gameObject.SetActive(nextIsUnlock);
				_infoButton.gameObject.SetActive(!nextIsUnlock);
			}
			else
			{
				_nextButton.gameObject.SetActive(false);
			}
		}


		void InfoButtonClick()
		{
			var obj = Instantiate(_infoPanelObj, transform.parent);
			var popup = obj.GetComponent<PopupScreen>();
			popup.HideEvent = () => Destroy(obj);
			popup.Show();
		}

		void BackButtonClick()
		{
			if (_headingType == HeadingBarTypes.Level)
			{
				SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU, false);
			}

			if (_headingType == HeadingBarTypes.Stage)
			{
				SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU, false);
			}

			if (_headingType == HeadingBarTypes.Game)
			{
				if (GameWord.Instance.CurrentPlayedInfo.Daily)
					SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU, false);
				else
					SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU, false);
				MyAnalytics.SendEvent(MyAnalytics.back_ingame_clicked);
			}
		}

		void NextButtonClick()
		{
			MyAnalytics.SendEvent(MyAnalytics.next_button_clicked);
			
			DataHelper.Instance.LastPlayedInfo.Level = GameWord.Instance.NextPlayedInfo.Level;
			DataHelper.Instance.LastPlayedInfo.Stage = GameWord.Instance.NextPlayedInfo.Stage;
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, true);
		}
		

		public void SetData(string title)
		{
			_titleText.text = title;

			if(_headingType == HeadingBarTypes.Game)
			{
				if (DataHelper.Instance.LastPlayedInfo.Daily)
				{
					var dayNum = GameSaveData.GetDailyEntranceNumber();
					if (dayNum > DataHelper.MAX_DAILY_NUM)
						dayNum = DataHelper.MAX_DAILY_NUM;
					_titleText.text = $"<color=#73D6FF>{dayNum + 1}</color> <color=#F0FF00>{Translator.GetString("Daily")}</color>";
				}
			}
		}
	}

}