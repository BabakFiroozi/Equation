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
		[SerializeField] Button _rightButton = null;
		[SerializeField] HeadingBarTypes _headingType = HeadingBarTypes.Level;

		[SerializeField] GameObject _exitPanel = null;

		public HeadingBarTypes HeadingType => _headingType;


		// Use this for initialization
		void Start()
		{
			_backButton.onClick.AddListener(BackButtonClick);
			_rightButton.onClick.AddListener(RightButtonClick);

			if (_headingType == HeadingBarTypes.Level)
			{
				string title = $"{Translator.GetString("Levels")}";
				SetData(title);

			}

			if (_headingType == HeadingBarTypes.Stage)
			{
				var playedInfo = DataHelper.Instance.LastPlayedInfo;
				string title = $"{playedInfo.Level + 1} {Translator.GetString("Level")}";
				SetData(title);
			}
			
			if (_headingType == HeadingBarTypes.Game)
			{
				var playedInfo = DataHelper.Instance.LastPlayedInfo;
				string title = $"{playedInfo.Stage + 1} {Translator.GetString("Stage")}  {playedInfo.Level + 1} {Translator.GetString("Level")}";
				SetData(title);
			}
		}

		void RightButtonClick()
		{
			if (_headingType == HeadingBarTypes.Game)
			{
			}
		}

		void BackButtonClick()
		{
			if (_headingType == HeadingBarTypes.Level)
			{
				SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU);
			}

			if (_headingType == HeadingBarTypes.Stage)
			{
				SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU);
			}

			if (_headingType == HeadingBarTypes.Game)
			{
				SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_STAGE_MENU);
				// _exitPanel.GetComponent<PopupScreen>().Show();
			}
		}

		// Update is called once per frame
		void Update()
		{
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