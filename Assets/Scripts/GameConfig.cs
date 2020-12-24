using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

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
		
		[SerializeField] bool _tutorialIsActive = true;
		[SerializeField] bool _gameIsUnlock = false;

		[SerializeField] int _playerInitialCoin = 10000;

		[SerializeField] int _hintCost = 30;
		[SerializeField] int _helpCost = 90;

		[SerializeField] StoreNames _storeName = StoreNames.Cafebazar;

		[SerializeField] string _leaderboardId = "";

		[SerializeField] int _freeCoinAmount = 20;
		
		[SerializeField] float _timeScale = 1.0f;

		[SerializeField] bool _dailyIsLocal;
		
		[SerializeField] int _freeCoinDayCap = 5;
		[SerializeField] int _freeHintDayCap = 7;
		[SerializeField] int _exitAdChance = 50;
		
		[SerializeField] WorldTimeAPIInfo _worldTimeAPI;

		public bool TutorialIsActive => _tutorialIsActive;
		public bool GameIsUnlock => _gameIsUnlock;

		public int HintCost => _hintCost;
		public int HelpCost => _helpCost;

		public StoreNames StoreName => _storeName;

		public int PlayerInitialCoin => _playerInitialCoin;
		
		public int FreeCoinAmount { get; private set; }
		
		public float TimeScale => _timeScale;

		public string LeaderboardId => _leaderboardId;
		
		public bool DailyIsLocal => _dailyIsLocal;
		
		public int FreeHintDayCap => _freeHintDayCap;
		public int FreeCoinDayCap => _freeCoinDayCap;
		public int ExitAdChance => _exitAdChance;
		
		public WorldTimeAPIInfo WorldTimeAPI => _worldTimeAPI;
		

		static GameConfig _instance;

		public static GameConfig Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<GameConfig>("GameConfig");
					_instance.FreeCoinAmount = _instance._freeCoinAmount + Random.Range(0, _instance._freeCoinAmount / 2);
				}
				return _instance;
			}
		}
	}
}