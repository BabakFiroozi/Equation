using UnityEngine;
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
		[SerializeField] Text _titleText;
		[SerializeField] GameObject _alreadySolvedText;
		[SerializeField] RectTransform[] _stars;

		[SerializeField] Text _nextLevelText;
		[SerializeField] GameObject _dailyRewardObj;
		[SerializeField] GameObject _rateAskPageObj;

		[SerializeField] Button _holdButton;
		[SerializeField] Text _holdText;
		
		bool _ratePageAsked;

		bool _alreadySolved;

		int _stageRank;

		bool _holded = true;

		// Use this for initialization
		void Start()
		{
			_nextButton.onClick.AddListener(NextStage);
			_stageSelButton.onClick.AddListener(GoStageSel);
			_replayButton.onClick.AddListener(ReplayGame);
			_holdButton.onClick.AddListener(HoldButtonClick);

			_holdButton.enabled = false;
			var color = _holdText.color;
			_holdText.color = new Color(color.r, color.g, color.b, 0);
		}

		void HoldButtonClick()
		{
			_holded = false;
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
			_alreadySolvedText.SetActive(false);
			
			var buttonsObj = _replayButton.transform.parent.gameObject;
			buttonsObj.SetActive(false);

			foreach (var star in _stars)
				star.gameObject.SetActive(false);

			var rewardGroup = _rewardText.transform.parent.gameObject.GetComponent<CanvasGroup>();
			rewardGroup.gameObject.SetActive(!_alreadySolved);
			rewardGroup.alpha = 0;

			_movesCountText.text = $"<color=yellow>{GameWord.Instance.Board.MovesCount}</color> :{Translator.GetString("Moves_You_Did")}";

			_titleText.text = Translator.GetString($"RankTitle_{_stageRank}");

			yield return new WaitForSeconds(1.0f);

			_backgOverlay.DOFade(.65f, .3f).SetEase(Ease.Linear);
			
			yield return new WaitForSeconds(.5f);

			_holdButton.enabled = true;
			_holdText.DOFade(1, .3f);
			yield return new WaitWhile(() => _holded);
			_holdText.DOFade(0, .3f);
			
			_frameRectTr.DOAnchorPosY(0, .5f).SetEase(Ease.OutBack);

			_frameRectTr.GetComponent<AudioSource>().Play();
			
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

			// int stageReward = (currentPlayedInfo.Level + 1) * 2 + (currentPlayedInfo.Stage + 1) / 5 + CalcShuffleBasedReward();
			int stageReward = GameWord.Instance.Board.ShufflesCount;
			
			if(!_alreadySolved)
			{
				rewardGroup.DOFade(1, .3f);
				GiveReward(stageReward);
			}
			else
			{
				_alreadySolvedText.SetActive(true);
				_alreadySolvedText.GetComponent<AudioSource>().Play();
			}

			yield return new WaitForSeconds(.4f);

			buttonsObj.SetActive(true);
			var buttonsGroup = buttonsObj.GetComponent<CanvasGroup>();
			buttonsGroup.alpha = 0;
			buttonsGroup.DOFade(1, .2f);
			buttonsGroup.GetComponent<AudioSource>().Play();

			yield return new WaitForSeconds(.2f);

			if (currentPlayedInfo.Stage == GameWord.Instance.Board.StagesCount - 1)
			{
				_nextLevelText.gameObject.SetActive(true);
				_nextLevelText.gameObject.GetComponent<AudioSource>().PlayDelayed(.1f);
				if(currentPlayedInfo.Level < DataHelper.Instance.LevelsCount - 1)
				{
					_nextLevelText.text = $"{Translator.GetString("Become")} <color=yellow>{currentPlayedInfo.Level + 2}</color> {Translator.GetString("You_Entered_Level")}";
					LevelsPanel.ResetStageHistoryScroll();
					if(!_alreadySolved)
					{
						yield return new WaitForSeconds(.2f);
						int levelReward = (currentPlayedInfo.Level + 1) * GameWord.Instance.Board.StagesCount;
						GiveReward(levelReward, stageReward);
					}
				}
				else
				{
					MyAnalytics.SendEvent(MyAnalytics.levels_end_vivsited);
					_nextLevelText.text = $"{Translator.GetString("You_Finished_Levels")}";
				}
				yield return new WaitForSeconds(.3f);
			}
			
			_preventTouchObj.SetActive(false);
		}

		void GiveReward(int reward, int oldReward = 0)
		{
			_rewardText.text = $"<color=white>{Translator.CROSS_SIGN}</color>{reward + oldReward}";
			_rewardText.gameObject.GetComponent<AudioSource>().Play();
			GameSaveData.AddCoin(reward, false, 0);
		}

		int CalcShuffleBasedReward()
		{
			var board = GameWord.Instance.Board;
			int limit = board.ClausesCount * 2;
			float coef = 0;
			if (board.ShufflesCount > limit)
			{
				//TODO - reward calcualation
				coef = (board.ShufflesCount - limit) / 5f;
			}

			int reward = (int) (coef * GameConfig.Instance.HintCost);
			return reward;
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
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, true);
		}

		void GoStageSel()
		{
			MyAnalytics.SendEvent(MyAnalytics.back_menu_stages);
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_LEVEL_MENU, false);
		}

		void NextStage()
		{
			var currentPlayedInfo = GameWord.Instance.CurrentPlayedInfo;

			if (currentPlayedInfo.Daily)
			{
				if (GameSaveData.IsDailyPuzzleRewarded(currentPlayedInfo))
				{
					SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU, true);
				}
				else
				{
					var obj = Instantiate(_dailyRewardObj, transform.parent);
					obj.GetComponent<DailyRewardPage>().ShowPage();
					GameSaveData.SetDailyPuzzleRewarded(currentPlayedInfo);
				}

				return;
			}


			if (!_ratePageAsked && !GameSaveData.IsGameRated())
			{
				if (currentPlayedInfo.Level > 1 && currentPlayedInfo.Stage > 0 && currentPlayedInfo.Stage % 10 == 0)
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
			SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, true);
		}

		void UnlockNextPuzzle()
		{
			var info = GameWord.Instance.NextPlayedInfo;
			GameSaveData.UnlockLevel(info.Level);
			GameSaveData.UnlockStage(info.Level, info.Stage);
		}
	}
}