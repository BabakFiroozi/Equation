﻿using UnityEngine;
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

		PuzzlePlayedInfo _nextPlayedInfo;


		// Use this for initialization
		void Start()
		{
			_backButton.onClick.AddListener(BackButtonClick);
			_infoButton.onClick.AddListener(InfoButtonClick);
			_nextButton.onClick.AddListener(NextButtonClick);

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
				
				_nextPlayedInfo = DataHelper.Instance.LastPlayedInfo.Copy();
				_nextPlayedInfo.Stage++;
				if (_nextPlayedInfo.Stage == Board.Instance.StagesCount)
				{
					_nextPlayedInfo.Stage = 0;
					if (_nextPlayedInfo.Level < DataHelper.Instance.LevelsCount)
						_nextPlayedInfo.Level++;
				}

				bool nextIsUnlock = GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo) && GameSaveData.IsStageUnlocked(_nextPlayedInfo.Level, _nextPlayedInfo.Stage);
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

		void NextButtonClick()
		{
			DataHelper.Instance.LastPlayedInfo.Level = _nextPlayedInfo.Level;
			DataHelper.Instance.LastPlayedInfo.Stage = _nextPlayedInfo.Stage;
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
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