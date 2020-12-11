using System;
using UnityEngine;
using System.Collections;

namespace Equation
{
	public class GameConfig : ScriptableObject
	{
		[Serializable]
		public class WorldTimeAPIInfo
		{
			public string url;
			public string field;
		}
		
		[SerializeField] WorldTimeAPIInfo _worldTimeAPI;
		
		[SerializeField] bool _tutorialIsActive = true;
		[SerializeField] bool _gameIsUnlock = false;

		[SerializeField] int _playerInitialCoin = 10000;

		[SerializeField] int _hintCost = 30;
		[SerializeField] int _helpCost = 90;

		[SerializeField] StoreNames _storeName = StoreNames.Cafebazar;

		[SerializeField] string _leaderboardId = "";

		[SerializeField] int _freeCoinAmount = 100;
		
		[SerializeField] float _timeScale = 1.0f;

		[SerializeField] bool _dailyIsLocal;
		
		[SerializeField] int _freeCoinDayCap = 5;
		[SerializeField] int _freeGuideDayCap = 7;
		[SerializeField] int _exitAdChance = 50;
		
		
		public WorldTimeAPIInfo WorldTimeAPI => _worldTimeAPI;
		
		public bool TutorialIsActive => _tutorialIsActive;
		public bool GameIsUnlock => _gameIsUnlock;

		public int HintCost => _hintCost;
		public int HelpCost => _helpCost;

		public StoreNames StoreName => _storeName;

		public int PlayerInitialCoin => _playerInitialCoin;
		
		public int FreeCoinAmount => _freeCoinAmount;
		
		public float TimeScale => _timeScale;

		public string LeaderboardId => _leaderboardId;
		
		public bool DailyIsLocal => _dailyIsLocal;
		
		public int FreeGuideDayCap => _freeGuideDayCap;
		public int FreeCoinDayCap => _freeCoinDayCap;
		public int ExitAdChance => _exitAdChance;

		static GameConfig _instance;

		public static GameConfig Instance
		{
			get
			{
				if (_instance == null)
					_instance = Resources.Load<GameConfig>("GameConfig");
				return _instance;
			}
		}
	}
}