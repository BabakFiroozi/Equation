using System;
using UnityEngine;

namespace CheshmakMe
{
    public class CheshmakLib
    {
        public delegate void eventCallBack(string type,string eventName,string paramString);
        public static void showInterstitialAds()
        {
            AndroidJavaClass interstitialAds = new AndroidJavaClass("unity.cheshmak.wrapper.InterstitialAds");
            interstitialAds.CallStatic("show");
        }
        
        // top | center | bottom
        public static void initializeBannerAds(string position)
        {
            AndroidJavaObject BannerAds = new AndroidJavaClass("unity.cheshmak.wrapper.BannerAds");
            BannerAds.CallStatic("Initialize", position);
        }
        
            
        // top | center | bottom
        public static void initializeMRECAds(string position)
        {
            AndroidJavaObject MrecAds = new AndroidJavaClass("unity.cheshmak.wrapper.MrecAds");
            MrecAds.CallStatic("Initialize", position);
        }


        
        public static void removeBannerAds()
        {
            AndroidJavaObject BannerAds = new AndroidJavaClass("unity.cheshmak.wrapper.BannerAds");
            BannerAds.CallStatic("removeBannerAds");
        }  
        public static void removeMrecAds()
        {
            AndroidJavaObject MrecAds = new AndroidJavaClass("unity.cheshmak.wrapper.MrecAds");
            MrecAds.CallStatic("removeMrecAds");
        }
        
        public static void initializeInterstitialAds()
        {
            AndroidJavaObject interstitialAds = new AndroidJavaClass("unity.cheshmak.wrapper.InterstitialAds");
            interstitialAds.CallStatic("Initialize");
        }
        
        public static void initializeRewardedAd()
        {
            AndroidJavaObject interstitialAds = new AndroidJavaClass("unity.cheshmak.wrapper.RewardedAd");
            interstitialAds.CallStatic("Initialize");
        }
        
        public static void showRewardedAd()
        {
            AndroidJavaClass interstitialAds = new AndroidJavaClass("unity.cheshmak.wrapper.RewardedAd");
            interstitialAds.CallStatic("show");
        }
        
        public static void initEvents(eventCallBack callBack = null)
        {
                AndroidJavaClass myCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
                InnerCheshmakListener innerlistener = new InnerCheshmakListener(callBack);
                myCheshmak.CallStatic("setListener",  new object[] {new CheshmakListenerProxy(innerlistener)  });
        }

        
        public static void setTestMode()
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            MyCheshmak.CallStatic("setTestMode");
        }

      
        public static string getCheshmakID()
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            return MyCheshmak.CallStatic<string>("getCheshmakID");
        }

      


        public static void sendTag(string tag)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            MyCheshmak.CallStatic("sendTag", tag);
        }

        public static void sendTags(string[] tags)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            MyCheshmak.CallStatic("sendTags", new object[] {javaArrayFromCS(tags)});
        }

        public static void deleteTag(string tag)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            MyCheshmak.CallStatic("deleteTag", tag);
        }

        public static void deleteTags(string[] tags)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            MyCheshmak.CallStatic("deleteTags", new object[] {javaArrayFromCS(tags)});
        }

        public static void deleteAllTags()
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            MyCheshmak.CallStatic("deleteAllTags");
        }
        
        //config
        public static int ConfigGetInt(string  key, int defaultValue)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            return MyCheshmak.CallStatic<int>("getInt",  new object[] { key, defaultValue }  );
        }
        
        public static bool ConfigGetBoolean(string  key, bool defaultValue)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            return MyCheshmak.CallStatic<bool>("getBoolean",  new object[] { key ,defaultValue}  );
        }
        
        public static string ConfigGetString(string  key, string defaultValue)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            return MyCheshmak.CallStatic<string>("getString",  new object[] { key, defaultValue }  );
        }
        
        public static double  ConfigGetDouble(string  key, double  defaultValue)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            return MyCheshmak.CallStatic<double>("getDouble",  new object[] { key , defaultValue}  );
        }
        
        public static long ConfigGetLong(string  key, long defaultValue)
        {
            AndroidJavaClass MyCheshmak = new AndroidJavaClass("unity.cheshmak.wrapper.MyCheshmak");
            return MyCheshmak.CallStatic<long>("getLong",  new object[] { key, defaultValue }  );
        }
        
      
      
        

        

        private static AndroidJavaObject javaArrayFromCS(string[] values)
        {
            AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
            AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance",
                new AndroidJavaClass("java.lang.String"), values.Length);
            for (int i = 0; i < values.Length; ++i)
            {
                arrayClass.CallStatic("set", arrayObject, i, new AndroidJavaObject("java.lang.String", values[i]));
            }

            return arrayObject;
        }
        
        class InnerCheshmakListener : ICheshmakListener
        {
            private eventCallBack _callBack ;

            public InnerCheshmakListener(eventCallBack callBack)
            {
                _callBack = callBack;

            }
            public void onCheshmakEvent(string type, string eventName, string paramString)
            {
                if (_callBack != null)
                {
                    _callBack(type , eventName ,paramString);
                }
               
            }
        }
   
        

    }
}