using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
	public class DataHelper
	{
		static DataHelper s_instance = null;

		public const int MAX_STAGES_COUNT = 25;
		public const int MAX_DAILY_STAGES_COUNT = 7;
		public const int MAX_DAILY_NUM = 41; // 42 days or 6 weeks

		public const int STAGE_RANK_MAX = 3;


		public static DataHelper Instance => s_instance ?? (s_instance = new DataHelper());

		public bool DailyEntrance { get; private set; }

		public bool DailyEntranceDisturbed => GameSaveData.GetDailyEntranceNumber() == 0;
		public bool InformDailyEntrance { get; private set; }

		public void CheckDailyEntrance(DateTime? nowDateTime)
		{
			DailyEntrance = false;

			if (nowDateTime != null)
			{
				var str = GameSaveData.GetLastDailyEntranceDate();
				if (str != string.Empty)
				{
					var jsonObj = JSONObject.Create(str);
					int year = (int) jsonObj["year"].i;
					int month = (int) jsonObj["month"].i;
					int day = (int) jsonObj["day"].i;
					var date = new DateTime(year, month, day, 0, 0, 0, 0);
					var diff = nowDateTime.Value - date;
					if (diff.TotalSeconds > 0)
					{
						if (diff.TotalHours < 24)
							DailyEntrance = true;
						else
							GameSaveData.SetDailyEntranceNumber(0);
					}
				}

				//Calculate next daily entrance
				{
					var today = nowDateTime.Value;
					int year = today.Year, month = today.Month, day = today.Day;
					if (day < DateTime.DaysInMonth(year, month))
					{
						day++;
					}
					else
					{
						day = 1;
						if (month < 12)
						{
							month++;
						}
						else
						{
							month = 1;
							year++;
						}
					}

					var jsonObj = JSONObject.Create();
					jsonObj.AddField("year", year);
					jsonObj.AddField("month", month);
					jsonObj.AddField("day", day);
					GameSaveData.SetNextDailyEntranceDate(jsonObj.Print());
				}
			}


			if (DailyEntrance)
				GameSaveData.IncreaseDailyEntranceNumber();

			InformDailyEntrance = DailyEntrance && GameSaveData.GetDailyEntranceNumber() > 0;
		}

		public bool IsFirstSession()
		{
			return GameSaveData.GetSessionNumber() == 1;
		}

		public DataHelper()
		{
		}

		public PuzzlePlayedInfo GetLastUnlockedInfo()
		{
			var gameLevel = GameLevels.None + 1;
			for (int l = (int) GameLevels.Count - 1; l > -1; --l)
			{
				var level = (GameLevels) l;
				if (GameSaveData.IsLevelUnlocked(level))
				{
					gameLevel = level;
					break;
				}
			}

			int gameStage = 0;
			for (int s = MAX_STAGES_COUNT - 1; s > -1; --s)
			{
				if (GameSaveData.IsStageUnlocked(gameLevel, s))
				{
					gameStage = s;
					break;
				}
			}

			var info = new PuzzlePlayedInfo {Level = gameLevel, Stage = gameStage};
			return info;
		}

		public PuzzlePlayedInfo LastPlayedInfo { get; } = new PuzzlePlayedInfo {Level = GameLevels.None, Stage = -1};
	}

}