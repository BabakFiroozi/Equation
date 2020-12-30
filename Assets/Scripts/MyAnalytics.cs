using System.Threading.Tasks;
using GameAnalyticsSDK;
// using DataBeenConnection;

namespace Equation
{
    public static class MyAnalytics
    {
        public const string game_entrance = "game_entrance";
        public const string daily_button_clicked = "daily_button_clicked";
        public const string daily_done = "daily_done";
        public const string daily_played = "daily_done";
        public const string shop_button_clicked = "shop_button_clicked";
        public const string shop_button_clicked_ingame = "shop_button_clicked_ingame";
        public const string went_to_shop_not_enough_coin = "went_to_shop_not_enough_coin";
        public const string not_enough_coin = "not_enough_coin";
        public const string leaderboard_button_clicked = "leaderboard_button_clicked";
        public const string setting_button_clicked = "setting_button_clicked";
        public const string hint_button_clicked = "hint_button_clicked";
        public const string help_button_clicked = "help_button_clicked";
        public const string next_button_clicked = "next_button_clicked";
        public const string replay_button_clicked_result = "repeat_button_clicked_result";
        public const string back_menu_stages = "back_menu_stages";
        public const string freeHint_button_clicked = "freeHint_button_clicked";
        public const string freeCoin_button_clicked = "free_coin_clicked";
        public const string quit_ad_clicked = "quit_ad_clicked";
        public const string wheelOfFortune_button_clicked = "wheelOfFortune_button_clicked";
        public const string rotateWheel_button_clicked = "rotateWheel_button_clicked";
        public const string purchase_coin_pack = "purchase_coin_pack";
        public const string purchase_coin_pack_succeed = "purchase_coin_pack_succeed";
        public const string profile_signed_up = "profile_signed_up";
        public const string profile_logged_in = "profile_logged_in";
        public const string profile_edited = "profile_edited";
        public const string back_ingame_clicked = "back_ingame_clicked";
        public const string levels_end_vivsited = "game_end_vivsited";
		public const string share_button_clicked = "share_button_clicked";
		public const string rate_button_clicked = "rate_button_clicked";
		public const string email_button_clicked = "email_button_clicked";
		public const string grid_button_clicked = "grid_button_clicked";
		public const string lang_button_clicked = "lang_button_clicked";
		public const string other_prod_button_clicked = "other_prod_button_clicked";
        
        


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