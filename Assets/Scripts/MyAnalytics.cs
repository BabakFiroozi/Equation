using System.Threading.Tasks;
using GameAnalyticsSDK;
// using DataBeenConnection;

namespace Equation
{
    public static class MyAnalytics
    {
        public const string game_entrance = "game_entrance";
        public const string quest_button_clicked = "quest_button_clicked";
        public const string quest_done = "quest_done";
        public const string quest_claimed = "quest_claimed";
        public const string daily_button_clicked = "daily_button_clicked";
        public const string daily_done = "daily_done";
        public const string daily_played = "daily_done";
        public const string shop_button_clicked = "shop_button_clicked";
        public const string shop_button_clicked_ingame = "shop_button_clicked_ingame";
        public const string not_enough_coin = "not_enough_coin";
        public const string stats_button_clicked = "stats_button_clicked";
        public const string leaderboard_button_clicked = "leaderboard_button_clicked";
        public const string setting_button_clicked = "setting_button_clicked";
        public const string found_words_button_clicked = "found_words_button_clicked";
        public const string guide_button_clicked = "guide_button_clicked";
        public const string hint_button_clicked = "hint_button_clicked";
        public const string hint_opt_button_clicked = "hint_opt_button_clicked";
        public const string undo_button_clicked = "undo_button_clicked";
        public const string next_button_clicked = "next_button_clicked";
        public const string replay_button_clicked_result = "repeat_button_clicked_result";
        public const string levelup_button_clicked = "levelup_button_clicked";
        public const string reveal_hidden_button_clicked = "reveal_hidden_button_clicked";
        public const string hidden_revealed = "hidden_revealed";
        public const string freeGuide_button_clicked = "free_guide_clicked";
        public const string freeCoin_button_clicked = "free_coin_clicked";
        public const string quit_ad_clicked = "quit_ad_clicked";
        public const string wheelOfFortune_button_clicked = "wheelOfFortune_button_clicked";
        public const string rotateWheel_button_clicked = "rotateWheel_button_clicked";
        public const string purchase_coin_pack = "purchase_coin_pack";
        public const string purchase_coin_pack_succeed = "purchase_coin_pack_succeed";
        public const string profile_signed_up = "profile_signed_up";
        public const string profile_logged_in = "profile_logged_in";
        public const string profile_edited = "profile_edited";
        public const string back_menu_restart = "back_menu_restart";
        public const string back_menu_main_menu = "back_menu_main_menu";
        public const string back_menu_stages = "back_menu_stages";
        public const string ask_button_clicked = "ask_button_clicked";
        public const string game_end_vivsited = "game_end_vivsited";
		public const string level_up = "level_up";
		public const string hidden_word_found = "hidden_word_found";
		public const string share_button_clicked = "share_button_clicked";
		public const string rate_button_clicked = "rate_button_clicked";
		public const string email_button_clicked = "email_button_clicked";
		public const string mode_unlocked = "mode_unlocked";
        
        


        /*
        public class EventData
        {
            public string key;
            public string value;
        }

        public static readonly EventData[] EmptyEventData = {new EventData {key = "empty", value = "null"}};

        public static void SendEvent(string eventName, EventData[] data)
        {
            //DataBeen
            {
                var infos = new List<CustomEventInfo>();
                foreach (var d in data)
                {
                    var info = new CustomEventInfo {key = d.key, value = d.value};
                    infos.Add(info);
                }

                if (data == null)
                    infos.Add(new CustomEventInfo {key = "empty", value = "null"});
                
                DataBeen.SendCustomEventData(eventName, infos.ToArray());
            }
        }*/


        public static void SendEvent(string eventName, float eventValue)
        {
            GameAnalytics.NewDesignEvent(eventName, eventValue);
        }
        
        public static void SendEvent(string eventName)
        {
            GameAnalytics.NewDesignEvent(eventName);
        }

        public static async void Init(string data)
        {
            GameAnalytics.Initialize();
            await Task.Delay(200);
            if (data != "")
                GameAnalytics.SetCustomId(data);
        }
    }
}