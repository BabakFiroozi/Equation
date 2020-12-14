﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Equation.Models;
using UnityEngine.UI;
using DG.Tweening;

namespace Equation
{
	public class ResultPanel : MonoBehaviour
	{
		[SerializeField] Button _replayButton = null;
		[SerializeField] Button _stageSelButton = null;
		[SerializeField] Button _nextButton = null;
		[SerializeField] RectTransform _frameRectTr;
		[SerializeField] Image _backgOverlay;
		[SerializeField] GameObject _preventTouchObj = null;
		[SerializeField] Text _movesCountText = null;
		[SerializeField] Text _rewardText;
		[SerializeField] GameObject _alreadySolvedText;
		[SerializeField] RectTransform[] _stars;

		[SerializeField] Text _nextLevelText;
		[SerializeField] GameObject _dailyRewardObj;
		[SerializeField] GameObject _rateAskPageObj;

		bool _ratePageAsked;

		bool _alreadySolved;

		int _stageRank;


		// Use this for initialization
		void Start()
		{
			_nextButton.onClick.AddListener(NextStage);
			_stageSelButton.onClick.AddListener(GoStageSel);
			_replayButton.onClick.AddListener(ReplayGame);
		}
		
		
		public void ShowResult(bool alreadySolved, int stageRank)
		{
			_alreadySolved = alreadySolved;
			_stageRank = stageRank;
			
			UnlockNextPuzzle();

			gameObject.SetActive(true);

			if (GameWord.Instance.CurrentPlayedInfo.Daily)
			{
				_stageSelButton.gameObject.SetActive(false);
			}

			StartCoroutine(_ShowFrame());
		}

		IEnumerator _ShowFrame()
		{
			_nextLevelText.gameObject.SetActive(false);
			
			var buttonsObj = _replayButton.transform.parent.gameObject;
			buttonsObj.SetActive(false);

			foreach (var star in _stars)
				star.gameObject.SetActive(false);

			_alreadySolvedText.SetActive(_alreadySolved);

			_rewardText.gameObject.SetActive(!_alreadySolved);
			var rewardGroup = _rewardText.gameObject.GetComponent<CanvasGroup>();
			rewardGroup.alpha = 0;

			_movesCountText.text = $"<color=yellow>{GameWord.Instance.Board.MovesCount}</color> :{Translator.GetString("Moves_You_Did")}";

			yield return new WaitForSeconds(.5f);
			
			_backgOverlay.DOFade(.7f, .5f);
			_frameRectTr.DOAnchorPosY(0, .5f).SetEase(Ease.OutCubic);
			
			yield return new WaitForSeconds(.5f);


			float delay = 0;
			for (int i = 0; i < _stars.Length; i++)
			{
				var star = _stars[i];
				if (i + 1 <= _stageRank)
				{
					star.gameObject.SetActive(true);
					star.localScale = new Vector3(0, 0, 1);
					star.DOScale(1, .25f).SetEase(Ease.OutBounce).SetDelay(delay);
					star.GetComponent<AudioSource>().PlayDelayed(delay);
					delay += .25f;
				}
				else
				{
					star.gameObject.SetActive(false);
				}
			}
			
			yield return new WaitForSeconds(delay + .15f);

			var currentPlayedInfo = GameWord.Instance.CurrentPlayedInfo;
			const int reward_per_level = 50;
			const int reward_per_stage = 2;
			int reward = (currentPlayedInfo.Level + 1) * reward_per_level + (currentPlayedInfo.Stage + 1) * reward_per_stage;
			
			_rewardText.text = $"<color=white>{Translator.CROSS_SIGN}</color>{reward}";
			rewardGroup.DOFade(1, .3f);
			_rewardText.gameObject.GetComponent<AudioSource>().Play();
			GameSaveData.AddCoin(reward, false, 0);

			yield return new WaitForSeconds(.5f);

			buttonsObj.SetActive(true);
			var buttonsGroup = buttonsObj.GetComponent<CanvasGroup>();
			buttonsGroup.alpha = 0;
			buttonsGroup.DOFade(1, .3f);
			buttonsGroup.GetComponent<AudioSource>().Play();

			yield return new WaitForSeconds(.3f);

			if (currentPlayedInfo.Stage == GameWord.Instance.Board.StagesCount - 1)
			{
				_nextLevelText.gameObject.SetActive(true);
				if(currentPlayedInfo.Level < DataHelper.Instance.LevelsCount - 1)
				{
					_nextLevelText.text = $"{Translator.GetString("Become")} <color=yellow>{currentPlayedInfo.Level + 2}</color> {Translator.GetString("You_Entered_Level")}";
					LevelsPanel.ResetStageHistoryScroll();
				}
				else
				{
					_nextLevelText.text = $"{Translator.GetString("You_Finished_Levels")}";
				}
				yield return new WaitForSeconds(.3f);
			}
			
			_preventTouchObj.SetActive(false);
		}

		void ReplayGame()
		{
			if (!GameWord.Instance.CurrentPlayedInfo.Daily)
			{
				var currInfo = GameWord.Instance.CurrentPlayedInfo;
				DataHelper.Instance.LastPlayedInfo.Level = currInfo.Level;
				DataHelper.Instance.LastPlayedInfo.Stage = currInfo.Stage;
			}

			MyAnalytics.SendEvent(MyAnalytics.replay_button_clicked_result);
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
		}

		void GoStageSel()
		{
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_STAGE_MENU);
		}

		void NextStage()
		{
			var currentPlayedInfo = GameWord.Instance.CurrentPlayedInfo;

			if (currentPlayedInfo.Daily)
			{
				if (GameSaveData.IsDailyPuzzleRewarded(currentPlayedInfo))
				{
					SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU);
				}
				else
				{
					// var obj = Instantiate(_dailyRewardObj, transform.parent);
					// obj.GetComponent<DailyRewardPage>().ShowPage();
					// GameSaveData.SetDailyPuzzleRewarded(lastPlayedInfo);
				}

				return;
			}


			if (!_ratePageAsked && !GameSaveData.IsGameRated())
			{
				if (currentPlayedInfo.Stage == 1)
				{
					_ratePageAsked = true;
					var obj = Instantiate(_rateAskPageObj, transform.parent);
					var confirm = obj.GetComponent<ConfirmScreen>();
					confirm.ClosedEvent = () => Destroy(obj);
					confirm.OpenConfirm((type) =>
					{
						if (type == ConfirmScreen.ConfirmTypes.Ok)
						{
							SettingPanel.GoRatePage();
							GameSaveData.RateGame();
							confirm.CloseConfirm();
						}
					});
					return;
				}
			}
			
			var nextPlayedInfo = GameWord.Instance.NextPlayedInfo;
			DataHelper.Instance.LastPlayedInfo.Level = nextPlayedInfo.Level;
			DataHelper.Instance.LastPlayedInfo.Stage = nextPlayedInfo.Stage;
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
		}

		void UnlockNextPuzzle()
		{
			var info = GameWord.Instance.NextPlayedInfo;
			GameSaveData.UnlockLevel(info.Level);
			GameSaveData.UnlockStage(info.Level, info.Stage);
		}
	}
}