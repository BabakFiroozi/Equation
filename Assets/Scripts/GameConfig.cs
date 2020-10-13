using UnityEngine;
using System.Collections;

namespace Cacao
{
	public class GameConfig : ScriptableObject
	{
		[SerializeField] bool _tutorialIsActive = true;
		[SerializeField] bool _gameIsUnlock = false;

		[SerializeField] int _playerInitialCoin = 10000;

		[SerializeField] int _normalMedalCost = 100;
		[SerializeField] int _hardMedalCost = 250;
		[SerializeField] int _extremeMedalCost = 450;
		[SerializeField] int _nightmareMedalCost = 700;

		[SerializeField] int _hintCost = 30;
		[SerializeField] int _hintCostOptional = 45;
		[SerializeField] int _guideCost = 90;
		[SerializeField] int _revealHiddenCostCoef = 2;

		[SerializeField] StoreNames _storeName = StoreNames.Cafebazar;

		[SerializeField] string _mainLeaderboardId = "";
		[SerializeField] string _hiddenLeaderboardId = "";

		[SerializeField] int _freeCoinAmount = 100;
		
		[SerializeField] float _timeScale = 1.0f;


		public bool TutorialIsActive => _tutorialIsActive;
		public bool GameIsUnlock => _gameIsUnlock;
		public int NormalMedalCost => _normalMedalCost;
		public int HardMedalCost => _hardMedalCost;
		public int ExtremeMedalCost => _extremeMedalCost;
		public int NightmareMedalCost => _nightmareMedalCost;

		public int HintCost => _hintCost;
		public int HintCostOptional => _hintCostOptional;
		public int GuideCost => _guideCost;
		public float RevealHiddenCostCoef => _revealHiddenCostCoef;

		public StoreNames StoreName => _storeName;

		public int PlayerInitialCoin => _playerInitialCoin;
		
		public int FreeCoinAmount => _freeCoinAmount;
		
		public float TimeScale => _timeScale;

		public string MainLeaderboardId => _mainLeaderboardId;
		public string HiddenLeaderboardId => _hiddenLeaderboardId;


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