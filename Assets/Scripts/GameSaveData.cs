using System;
using System.Collections.Generic;
using Equation.Models;
using SimpleJSON;
using UnityEngine;


namespace Equation
{
	public class PuzzlePlayedInfo
	{
		public GameModes Mode { get; set; }
		public GameLevels Level { get; set; }
		public int Stage { get; set; }

		// public string Subject { get; set; }

		public string InfoString()
		{
			string str = (Stage + 1) + " - " + Translator.GetString(Level.ToString()) + " - " + Translator.GetString(Mode.ToString());
			return str;
		}
	}


	public static class GameSaveData
	{

		public static void UnlockMode(GameModes mode)
		{
			string key = $"{mode}_Unlocked";
			SetBool(key, true);
		}

		public static bool IsModeUnlocked(GameModes mode)
		{
			string key = $"{mode}_Unlocked";
			return GetBool(key);
		}

		public static void UnlockLevel(GameModes mode, GameLevels level)
		{
			string key = $"{mode}_{level}_Unlocked";
			SetBool(key, true);
		}

		public static bool IsLevelUnlocked(GameModes mode, GameLevels level)
		{
			string key = $"{mode}_{level}_Unlocked";
			return GetBool(key);
		}

		public static void UnlockStage(GameModes mode, GameLevels level, int stage)
		{
			string key = $"{mode}_{level}_{stage}_Unlocked";
			SetBool(key, true);
		}

		public static bool IsStageUnlocked(GameModes mode, GameLevels level, int stage)
		{
			string key = $"{mode}_{level}_{stage}_Unlocked";
			return GetBool(key);
		}

		public static void SetStageRecord(PuzzlePlayedInfo info, int record)
		{
			string key = $"{info.Mode}_{info.Level}_{info.Stage}_Record";
			PlayerPrefs.SetInt(key, record);
		}
		
		public static int GetStageRecord(PuzzlePlayedInfo info)
		{
			string key = $"{info.Mode}_{info.Level}_{info.Stage}_Record";
			return PlayerPrefs.GetInt(key, -1);
		}

		public static void SetStageRank(PuzzlePlayedInfo info, int rank)
		{
			int oldRank = GetStageRank(info);
			if (oldRank != -1 && oldRank < rank)
				return;

			string key = $"{info.Mode}_{info.Level}_{info.Stage}_Rank";
			PlayerPrefs.SetInt(key, rank);
		}

		public static int GetStageRank(PuzzlePlayedInfo info)
		{
			string key = $"{info.Mode}_{info.Level}_{info.Stage}_Rank";
			return PlayerPrefs.GetInt(key, -1);
		}

		public static bool IsStageSolved(PuzzlePlayedInfo info)
		{
			string key = $"{info.Mode}_{info.Level}_{info.Stage}_Solved";
			return GetBool(key);
		}

		public static void SolveStage(PuzzlePlayedInfo info)
		{
			string key = $"{info.Mode}_{info.Level}_{info.Stage}_Solved";
			SetBool(key, true);

			key = "Level_Stages_Count_Solved_" + info.Level;
			PlayerPrefs.SetInt(key, GetLevelSolvedStagesCount(info.Level) + 1);
		}

		public static int GetLevelSolvedStagesCount(GameLevels level)
		{
			string key = "Level_Stages_Count_Solved_" + level;
			int count = PlayerPrefs.GetInt(key, 0);
			return count;
		}

		public static void SaveLastPlayedInfo()
		{
			JSONObject obj = JSONObject.Create();
			obj.AddField("mode", (int) DataHelper.Instance.LastPlayedInfo.Mode);
			obj.AddField("level", (int) DataHelper.Instance.LastPlayedInfo.Level);
			obj.AddField("stage", DataHelper.Instance.LastPlayedInfo.Stage);
			string infoStr = obj.Print();
			PlayerPrefs.SetString("LastPlayedInfo", infoStr);
		}

		public static void LoadLastPlayedInfo(PuzzlePlayedInfo info)
		{
			string str = PlayerPrefs.GetString("LastPlayedInfo", "");
			if (str == "null" || string.IsNullOrEmpty(str))
			{
				info.Mode = GameModes.Easy;
				info.Level = GameLevels.Beginner;
				info.Stage = 0;
				return;
			}
			
			JSONObject obj = JSONObject.Create(str);
			info.Mode = (GameModes) obj.GetField("mode").i;
			info.Level = (GameLevels) obj.GetField("level").i;
			info.Stage = (int) obj.GetField("stage").i;
		}

		public static void SaveSolvedWord(bool hidden, List<string> wordsList)
		{
			PuzzlePlayedInfo info = DataHelper.Instance.LastPlayedInfo;
			string keyName = $"{info.Mode}_{info.Level}_{info.Stage}_{(hidden ? "Hidden" : "Main")}";
			JSONObject obj = JSONObject.Create();
			foreach (var s in wordsList)
				obj.Add(s);
			string str = obj.Print();
			PlayerPrefs.SetString(keyName, str);
		}

		public static void LoadSolvedWords(bool hidden, List<string> wordsList, PuzzlePlayedInfo info)
		{
			string keyName = $"{info.Mode}_{info.Level}_{info.Stage}_{(hidden ? "Hidden" : "Main")}";
			string str = PlayerPrefs.GetString(keyName);

			if (str == "null" || string.IsNullOrEmpty(str))
				return;

			var objsList = JSONObject.Create(str).list;
			foreach (var s in objsList)
				wordsList.Add(s.str);
		}

		public static void SaveUsedHints(PuzzlePlayedInfo info, int hintIndex)
		{
			string keyName = $"{info.Mode}_{info.Level}_{info.Stage}_UsedHints";

			var list = LoadUsedHints(info);
			list.Add(hintIndex);
			
			var jsonObj = JSONObject.arr;
			list.ForEach(e => jsonObj.Add(e));

			PlayerPrefs.SetString(keyName, jsonObj.Print());
		}

		public static List<int> LoadUsedHints(PuzzlePlayedInfo info)
		{
			string keyName = $"{info.Mode}_{info.Level}_{info.Stage}_UsedHints";

			var str = PlayerPrefs.GetString(keyName);

			List<int> indicesList = new List<int>();
			if (!string.IsNullOrEmpty(str))
			{
				var list = JSONObject.Create(str).list;
				list.ForEach(e => indicesList.Add((int) e.i));
			}

			return indicesList;
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

		public static float GetGameSoundVolume()
		{
			string key = "Game_Sound_Volume";
			float vol = PlayerPrefs.GetFloat(key, 1);
			return vol;
		}

		public static void SetGameSoundVolume(float vol)
		{
			string key = "Game_Sound_Volume";
			PlayerPrefs.SetFloat(key, vol);
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

		public static bool IsLikedMoreGames()
		{
			bool like = PlayerPrefs.HasKey("MoreGames_Liked");
			return like;
		}

		public static void LikeMoreGames()
		{
			PlayerPrefs.SetInt("MoreGames_Liked", 1);
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

		public static void SubCoin(int coin, bool anim = false, bool sound = true)
		{
			SetCoin(GetCoin() - coin);
			CoinChangedEvent?.Invoke(-coin, anim, sound);
			
			//TODO - calculate hint count
			AddConsumeHint();
		}

		public static void AddCoin(int coin, bool anim = false, bool sound = true)
		{
			SetCoin(GetCoin() + coin);
			CoinChangedEvent?.Invoke(coin, anim, sound);
		}

		public static Action<int, bool, bool> CoinChangedEvent { get; set; }


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
		}

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

		static void AddConsumeHint()
		{
			int c = GetConsumeHint();
			PlayerPrefs.SetInt("ConsumedHint", c + 1);
		}

		public static int GetConsumeHint()
		{
			return PlayerPrefs.GetInt("ConsumedHint", 0);
		}

		public static bool PuzzleSubjectGuided(PuzzlePlayedInfo info)
		{
			string keyName = $"{info.Mode}_{info.Level}_{info.Stage}_Guided";
			return GetBool(keyName, false);
		}

		public static void GuidePuzzleSubject(PuzzlePlayedInfo info)
		{
			string keyName = $"{info.Mode}_{info.Level}_{info.Stage}_Guided";
			SetBool(keyName, true);
		}
		//
		// public static void SetQuestProgress(GameModes mode, GameLevels level, int stage, QuestTypes type, List<string> progress)
		// {
		// 	string keyName = $"{mode}_{level}_{stage}_{type}_QuestProgress";
		// 	var arr = JSONObject.arr;
		// 	progress.ForEach(p => arr.Add(p));
		// 	PlayerPrefs.SetString(keyName, arr.Print());
		// }
		//
		// public static List<string> GetQuestProgress(GameModes mode, GameLevels level, int stage, QuestTypes type)
		// {
		// 	string keyName = $"{mode}_{level}_{stage}_{type}_QuestProgress";
		// 	string str = PlayerPrefs.GetString(keyName);
		// 	var list = new List<string>();
		// 	if (!string.IsNullOrEmpty(str))
		// 	{
		// 		var arr = JSONObject.Create(str).list;
		// 		arr.ForEach(a => list.Add(a.str));
		// 	}
		// 	return list;
		// }
		//
		// public static void DelQuestProgress(GameModes mode, GameLevels level, int stage, QuestTypes type)
		// {
		// 	string keyName = $"{mode}_{level}_{stage}_{type}_QuestProgress";
		// 	PlayerPrefs.DeleteKey(keyName);
		// }
		//
		// public static void SetQuestState(GameModes mode, GameLevels level, int stage, QuestTypes type, QuestStates state)
		// {
		// 	string keyName = $"{mode}_{level}_{stage}_{type}_QuestState";
		// 	PlayerPrefs.SetInt(keyName, (int) state);
		// }
		//
		// public static int GetQuestState(GameModes mode, GameLevels level, int stage, QuestTypes type)
		// {
		// 	string keyName = $"{mode}_{level}_{stage}_{type}_QuestState";
		// 	return PlayerPrefs.GetInt(keyName, 0);
		// }
		
		public static void SetAskRevealHiddenVisited(bool visit)
		{
			string keyName = "AskRevelHiddenVisited";
			SetBool(keyName, visit);
		}
		
		public static bool GetAskRevealHiddenVisited()
		{
			string keyName = "AskRevelHiddenVisited";
			return GetBool(keyName, false);
		}

		public static void SetTaggedQuestId(string questId)
		{
			string keyName = "TaggedQuestId";
			PlayerPrefs.SetString(keyName, questId);
		}
		
		public static string GetTaggedQuestId()
		{
			string keyName = "TaggedQuestId";
			return PlayerPrefs.GetString(keyName, "");
		}
		
		public static void DelTaggedQuestId()
		{
			string keyName = "TaggedQuestId";
			PlayerPrefs.DeleteKey(keyName);
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
	}
}