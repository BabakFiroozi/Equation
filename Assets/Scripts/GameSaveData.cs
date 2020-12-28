using System;
using System.Collections.Generic;
using Equation.Models;
using SimpleJSON;
using UnityEngine;


namespace Equation
{
	public class PuzzlePlayedInfo
	{
		public int Level { get; set; }
		public bool Daily { get; set; }
		public int Stage { get; set; }

		public PuzzlePlayedInfo Copy()
		{
			var info = new PuzzlePlayedInfo {Level = Level, Stage = Stage, Daily = Daily};
			return info;
		}
	}

	public static class GameSaveData
	{
		public static void UnlockLevel(int level, bool daily = false)
		{
			string key = $"Level_Unlocked_{level}_{daily}";
			SetBool(key, true);
		}

		public static bool IsLevelUnlocked(int level, bool daily = false)
		{
			string key = $"Level_Unlocked_{level}_{daily}";
			return GetBool(key);
		}

		public static void UnlockStage(int level, int stage, bool daily = false)
		{
			string key = $"Stage_Unlocked_{level}_{stage}_{daily}";
			SetBool(key, true);
		}

		public static bool IsStageUnlocked(int level, int stage, bool daily = false)
		{
			string key = $"Stage_Unlocked_{level}_{stage}_{daily}";
			return GetBool(key);
		}

		public static void SetStageRank(PuzzlePlayedInfo info, int rank)
		{
			int oldRank = GetStageRank(info);
			if(rank <= oldRank)
				return;
			string key = $"Stage_Rank_{info.Level}_{info.Stage}_{info.Daily}";
			PlayerPrefs.SetInt(key, rank);
		}

		public static int GetStageRank(PuzzlePlayedInfo info)
		{
			string key = $"Stage_Rank_{info.Level}_{info.Stage}_{info.Daily}";
			return PlayerPrefs.GetInt(key, 0);
		}
		
		public static void DelStageRank(PuzzlePlayedInfo info)
		{
			string key = $"Stage_Rank_{info.Level}_{info.Stage}_{info.Daily}";
			PlayerPrefs.DeleteKey(key);
		}
		
		public static bool IsStageSolved(PuzzlePlayedInfo info)
		{
			string key = $"Stage_Solved_{info.Level}_{info.Stage}_{info.Daily}";
			return GetBool(key);
		}

		public static void SolveStage(PuzzlePlayedInfo info)
		{
			string key = $"Stage_Solved_{info.Level}_{info.Stage}_{info.Daily}";
			SetBool(key, true);
		}

		public static void SaveUsedHints(PuzzlePlayedInfo info, int cell)
		{
			string keyName = $"UsedHints_{info.Level}_{info.Stage}_{info.Daily}";

			var list = LoadUsedHints(info);
			list.Add(cell);
			
			var jsonObj = JSONObject.arr;
			list.ForEach(e => jsonObj.Add(e));

			PlayerPrefs.SetString(keyName, jsonObj.Print());
		}

		public static List<int> LoadUsedHints(PuzzlePlayedInfo info)
		{
			string keyName = $"UsedHints_{info.Level}_{info.Stage}_{info.Daily}";

			var str = PlayerPrefs.GetString(keyName);

			List<int> cellsList = new List<int>();
			if (!string.IsNullOrEmpty(str))
			{
				var list = JSONObject.Create(str).list;
				list.ForEach(e => cellsList.Add((int) e.i));
			}

			return cellsList;
		}
		
		public static void ResetUsedHints(PuzzlePlayedInfo info)
		{
			string keyName = $"UsedHints_{info.Level}_{info.Stage}_{info.Daily}";
			PlayerPrefs.DeleteKey(keyName);
		}

		public static void SavePawnCell(PuzzlePlayedInfo info, int pawn, int cell)
		{
			string keyName = $"PawnCell_{info.Level}_{info.Stage}_{info.Daily}_{pawn}";
			PlayerPrefs.SetInt(keyName, cell);
		}

		public static int LoadPawnCell(PuzzlePlayedInfo info, int pawn, int cell)
		{
			string keyName = $"PawnCell_{info.Level}_{info.Stage}_{info.Daily}_{pawn}";
			return PlayerPrefs.GetInt(keyName, cell);
		}
		
		public static void ResetPawnCell(PuzzlePlayedInfo info, int pawn)
		{
			string keyName = $"PawnCell_{info.Level}_{info.Stage}_{info.Daily}_{pawn}";
			PlayerPrefs.DeleteKey(keyName);
		}
		
		public static void SavePawnHelped(PuzzlePlayedInfo info, int pawn, bool helped)
		{
			string keyName = $"PawnHelp_{info.Level}_{info.Stage}_{info.Daily}_{pawn}";
			SetBool(keyName, helped);
		}

		public static bool LoadPawnHelped(PuzzlePlayedInfo info, int pawn)
		{
			string keyName = $"PawnHelp_{info.Level}_{info.Stage}_{info.Daily}_{pawn}";
			return GetBool(keyName);
		}
		
		public static void ResetPawnHelped(PuzzlePlayedInfo info, int pawn)
		{
			string keyName = $"PawnHelp_{info.Level}_{info.Stage}_{info.Daily}_{pawn}";
			PlayerPrefs.DeleteKey(keyName);
		}
		
		public static bool IsDailyPuzzleRewarded(PuzzlePlayedInfo info)
		{
			string keyName = $"DailyPuzzleRewarded_{info.Level}_{info.Stage}_{info.Daily}";
			return GetBool(keyName, false);
		}
		
		public static void SetDailyPuzzleRewarded(PuzzlePlayedInfo info)
		{
			string keyName = $"DailyPuzzleRewarded_{info.Level}_{info.Stage}_{info.Daily}";
			SetBool(keyName, true);
		}

		public static bool IsNextLevelReachedRewarded(int level)
		{
			string keyName = $"NextLevelReachedRewarded_{level}";
			return GetBool(keyName, false);
		}
		
		public static void RewardNextLevelReached(int level)
		{
			string keyName = $"NextLevelReachedRewarded_{level}";
			SetBool(keyName, true);
		}

		public static void SavePrefs()
		{
			PlayerPrefs.Save();
		}

		public static int GetSessionNumber()
		{
			return PlayerPrefs.GetInt("Session_Number", 0);
		}

		public static void IncreaseSessionNumber()
		{
			int num = GetSessionNumber() + 1;
			PlayerPrefs.SetInt("Session_Number", num);
		}

		public static bool IsGameVisited()
		{
			string key = "IsGameVisited";
			return GetBool(key, false);
		}

		public static void VisitGame()
		{
			string key = "IsGameVisited";
			SetBool(key, true);
		}

		public static bool IsGameSoundOn()
		{
			string key = "Game_Sound_On";
			int on = PlayerPrefs.GetInt(key, 1);
			return on == 1;
		}

		public static void SetGameSoundOn(bool on)
		{
			string key = "Game_Sound_On";
			PlayerPrefs.SetInt(key, on ? 1 : 0);
		}

		public static bool IsGameMusicOn()
		{
			string key = "Game_Music_On";
			int on = PlayerPrefs.GetInt(key, 1);
			return on == 1;
		}

		public static void SetGameMusicOn(bool on)
		{
			string key = "Game_Music_On";
			PlayerPrefs.SetInt(key, on ? 1 : 0);
		}

		public static bool IsTutorialCompleted()
		{
			string key = "Game_Tutorial_Completed";
			return PlayerPrefs.GetInt(key, 0) == 1;
		}

		public static void SetTutorialCompleted()
		{
			string key = "Game_Tutorial_Completed";
			PlayerPrefs.SetInt(key, 1);
		}

		public static bool IsLikedTelegram()
		{
			bool like = PlayerPrefs.HasKey("Telegram_Liked");
			return like;
		}

		public static void LikeTelegram()
		{
			PlayerPrefs.SetInt("Telegram_Liked", 1);
		}

		public static bool IsLikedInstageram()
		{
			bool like = PlayerPrefs.HasKey("Instageram_Liked");
			return like;
		}

		public static void LikeInstageram()
		{
			PlayerPrefs.SetInt("Instageram_Liked", 1);
		}

		public static void SetCoin(int coin)
		{
			if (coin > 999999)
				coin = 999999;

			string key = "Coin_Count";
			PlayerPrefs.SetInt(key, coin);
		}

		public static int GetCoin()
		{
			string key = "Coin_Count";
			int hint = PlayerPrefs.GetInt(key);
			return hint;
		}

		public static void SubCoin(int coin, bool anim = false, float vol = 1)
		{
			SetCoin(GetCoin() - coin);
			CoinChangedEvent?.Invoke(-coin, anim, vol);
		}

		public static void AddCoin(int coin, bool anim = false, float vol = 1)
		{
			SetCoin(GetCoin() + coin);
			CoinChangedEvent?.Invoke(coin, anim, vol);
		}

		public static Action<int, bool, float> CoinChangedEvent { get; set; }


		public static void SaveInt(string keyName, int val)
		{
			PlayerPrefs.SetInt(keyName, val);
		}

		public static int LoadInt(string keyName)
		{
			int val = PlayerPrefs.GetInt(keyName);
			return val;
		}

		public static bool GetBool(string key, bool def = false)
		{
			if (!PlayerPrefs.HasKey(key))
				return def;
			return PlayerPrefs.GetInt(key, 0) == 1;
		}

		public static void SetBool(string key, bool val)
		{
			PlayerPrefs.SetInt(key, val == true ? 1 : 0);
		}

		public static void SetNextDailyEntranceDate(string data)
		{
			PlayerPrefs.SetString("DailyEntranceDate", data);
		}

		public static string GetLastDailyEntranceDate()
		{
			return PlayerPrefs.GetString("DailyEntranceDate", string.Empty);
		}

		public static void SetDailyEntranceNumber(int dayNum)
		{
			PlayerPrefs.SetInt("DailyEntranceNumber", dayNum);
			if (dayNum == 0)
			{
				ResetDailyFreeGuide();
				ResetDailyFreeCoin();
			}
		}

		/// <summary>
		/// Start from zero
		/// </summary>
		/// <returns></returns>
		public static int GetDailyEntranceNumber()
		{
			return PlayerPrefs.GetInt("DailyEntranceNumber", 0);
		}

		public static void IncreaseDailyEntranceNumber()
		{
			SetDailyEntranceNumber(GetDailyEntranceNumber() + 1);
		}

		public static bool DailyVisited(int dayNumber)
		{
			string key = $"DailyDayNumberVisited_{dayNumber}";
			return GetBool(key, false);
		}

		public static void VisitDaily(int dayNumber, bool visit)
		{
			string key = $"DailyDayNumberVisited_{dayNumber}";
			SetBool(key, visit);
		}

		public static bool SpinnerVisited(int dayNumber)
		{
			string key = $"SpinnerVisited_{dayNumber}";
			return GetBool(key, false);
		}

		public static void VisitSpinner(int dayNumber, bool visit)
		{
			string key = $"SpinnerVisited_{dayNumber}";
			SetBool(key, visit);
		}

		public static void IncLevelUp()
		{
			int c = GetLevelUp();
			PlayerPrefs.SetInt("LevelUp", c + 1);
		}

		public static int GetLevelUp()
		{
			return PlayerPrefs.GetInt("LevelUp", 0);
		}
		
		public static void SetPlayerToken(string token)
		{
			PlayerPrefs.SetString("Player_Token", token);
		}
		
		public static string GetPlayerToken()
		{
			return PlayerPrefs.GetString("Player_Token", string.Empty);
		}

		public static void SetSignupEmail(string email)
		{
			PlayerPrefs.SetString("Signup_Email", email);
		}
		
		public static string GetSignupEmail()
		{
			return PlayerPrefs.GetString("Signup_Email");
		}

		public static bool HasDailyFreeGuide(int cap)
		{
			int dayNum = GetDailyEntranceNumber();
			string keyName = $"HasDailyFreeGuide_{dayNum}";
			return PlayerPrefs.GetInt(keyName, 0) < cap;
		}
		
		public static void IncDailyFreeGuide()
		{
			int dayNum = GetDailyEntranceNumber();
			string keyName = $"HasDailyFreeGuide_{dayNum}";
			int c = PlayerPrefs.GetInt(keyName, 0);
			PlayerPrefs.SetInt(keyName, c + 1);
		}

		static void ResetDailyFreeGuide()
		{
			for (int i = 0; i < 200; ++i)
			{
				int dayNum = i;
				string keyName = $"HasDailyFreeGuide_{dayNum}";
				if (PlayerPrefs.HasKey(keyName))
					PlayerPrefs.DeleteKey(keyName);
			}
		}
		
		public static bool HasDailyFreeCoin(int cap)
		{
			int dayNum = GetDailyEntranceNumber();
			string keyName = $"HasDailyFreeCoin_{dayNum}";
			return PlayerPrefs.GetInt(keyName, 0) < cap;
		}
		
		public static void IncDailyFreeCoin()
		{
			int dayNum = GetDailyEntranceNumber();
			string keyName = $"HasDailyFreeCoin_{dayNum}";
			int c = PlayerPrefs.GetInt(keyName, 0);
			PlayerPrefs.SetInt(keyName, c + 1);
		}

		static void ResetDailyFreeCoin()
		{
			for (int i = 0; i < 200; ++i)
			{
				int dayNum = i;
				string keyName = $"HasDailyFreeCoin_{dayNum}";
				if (PlayerPrefs.HasKey(keyName))
					PlayerPrefs.DeleteKey(keyName);
			}
		}

		public static bool IsGameRated()
		{
			string key = "GameRated";
			return GetBool(key, false);
		}
		
		public static void RateGame()
		{
			string key = "GameRated";
			SetBool(key, true);
		}

		public static void SetNumberFontEng(bool eng)
		{
			string key = "NumberFont_Eng";
			SetBool(key, eng);
		}
		
		public static bool IsNumberFontEng()
		{
			string key = "NumberFont_Eng";
			return GetBool(key);
		}
		
		public static void SetGridVisible(bool eng)
		{
			string key = "Grid_Visisble";
			SetBool(key, eng);
		}
		
		public static bool IsGridVisible()
		{
			string key = "Grid_Visisble";
			return GetBool(key, false);
		}

		public static void Reset()
		{
			PlayerPrefs.DeleteAll();
		}
	}
}