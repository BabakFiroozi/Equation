using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
		[SerializeField] RectTransform[] _stars;

		[SerializeField] GameObject _gotNextLevelPageObj;
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
			
			UnlockNextPuzzle();

			gameObject.SetActive(true);

			if (DataHelper.Instance.LastPlayedInfo.Daily)
			{
				_stageSelButton.gameObject.SetActive(false);
			}

			StartCoroutine(_ShowFrame());
		}

		IEnumerator _ShowFrame()
		{
			var buttonsObj = _replayButton.transform.parent.gameObject;
			buttonsObj.SetActive(false);

			_rewardText.gameObject.SetActive(!_alreadySolved);
			var rewardGroup = _rewardText.gameObject.GetComponent<CanvasGroup>();
			rewardGroup.alpha = 0;

			_movesCountText.text = $"<color=yellow>{GameWord.Instance.Board.MovesCount}</color> :{Translator.GetString("Moves_You_Did")}";

			yield return new WaitForSeconds(.8f);

			for (int i = 0; i < _stars.Length; i++)
			{
				var star = _stars[i];
				star.gameObject.SetActive(i + 1 > _stageRank);
			}

			_backgOverlay.DOFade(.7f, .5f);
			_frameRectTr.DOAnchorPosY(100, .5f).SetEase(Ease.OutCubic);
			
			yield return new WaitForSeconds(.5f);

			var info = DataHelper.Instance.LastPlayedInfo;
			const int reward_per_level = 50;
			const int reward_per_stage = 2;
			int reward = (info.Level + 1) * reward_per_level + (info.Stage + 1) * reward_per_stage;
			
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

			_preventTouchObj.SetActive(false);
		}

		void ShowReachedNextLevel(PuzzlePlayedInfo info, bool levelUnlocked, bool modeUnlocked)
		{
			LevelsPanel.ResetStageHistoryScroll();
			
			//TODO - next level

			// var obj = Instantiate(_gotNextLevelPagePrefab, transform.parent);
			// var page = obj.GetComponent<NextLevelReachedPage>();
			// page.Show(info.Stage, info.Level, info.Mode, levelUnlocked, modeUnlocked, () => { page.Hide(); });
		}

		void ReplayGame()
		{
			if (!DataHelper.Instance.LastPlayedInfo.Daily)
			{
				var currInfo = DataHelper.Instance.LastPlayedInfo;
				DataHelper.Instance.LastPlayedInfo.Level = currInfo.Level;
				DataHelper.Instance.LastPlayedInfo.Stage = currInfo.Stage;
			}

			MyAnalytics.SendEvent(MyAnalytics.replay_button_clicked_result);
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
		}

		void GoStageSel()
		{
			// DataHelper.Instance.LastPlayedInfo.Level = GameBoard.Instance.CurrentPlayedInfo.Level;
			// DataHelper.Instance.LastPlayedInfo.Stage = GameBoard.Instance.CurrentPlayedInfo.Stage;

			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_STAGE_MENU);
		}

		void NextStage()
		{
			var lastPlayedInfo = DataHelper.Instance.LastPlayedInfo;

			if (lastPlayedInfo.Daily)
			{
				if (GameSaveData.IsDailyPuzzleRewarded(lastPlayedInfo))
				{
					SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU);
				}
				else
				{
					var obj = Instantiate(_dailyRewardObj, transform.parent);
					// obj.GetComponent<DailyRewardPage>().ShowPage();
					// GameSaveData.SetDailyPuzzleRewarded(lastPlayedInfo);
				}

				return;
			}


			if (!_ratePageAsked && !GameSaveData.IsGameRated())
			{
				if (DataHelper.Instance.LastPlayedInfo.Stage == GameWord.Instance.Board.StagesCount - 1)
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

			//TODO - handle game end
			
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
		}

		void UnlockNextPuzzle()
		{
			DataHelper.Instance.LastPlayedInfo.Stage++;
			if (DataHelper.Instance.LastPlayedInfo.Stage == GameWord.Instance.Board.StagesCount)
			{
				DataHelper.Instance.LastPlayedInfo.Stage = 0;
				if (DataHelper.Instance.LastPlayedInfo.Level < DataHelper.Instance.LevelsCount)
					DataHelper.Instance.LastPlayedInfo.Level++;
			}

			GameSaveData.UnlockLevel(DataHelper.Instance.LastPlayedInfo.Level);
			GameSaveData.UnlockStage(DataHelper.Instance.LastPlayedInfo.Level, DataHelper.Instance.LastPlayedInfo.Stage);
		}
	}
}