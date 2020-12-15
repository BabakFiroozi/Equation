using System;
using TapsellSDK;
using UnityEngine;

namespace Equation
{
    public class MyTapsellAds : MonoBehaviour
    {
        public const string FreeHint = "5fd395f44442e20001a9f63b";
        public const string FreeCoin = "5fd8ee868132f30001fedc81";
        public const string GameExit = "5fd8fe42ab770c000118d5fd";
        
        const string TapsellKey = "rgmifjqllkmjhjntghegqqljfgcriqqhgjpcigqkjgibotajtdchlpgcjrrrpnjlpaorrm";
        
        public static MyTapsellAds Instance { get; private set; }
        
        public Action<TapsellAdFinishedResult> OnFinishedEvent { get; set; }
        
        

        public void Start()
        {
            Instance = this;
            Tapsell.Initialize(TapsellKey);
            Tapsell.SetRewardListener(OnFinishedAction);

            Debug.LogWarning("Tapsell Initialized...");
        }

        void OnFinishedAction(TapsellAdFinishedResult result)
        {
            OnFinishedEvent?.Invoke(result);
        }

        public void ReqAd(string zoneId, bool isCached, Action<TapsellAd> onAdAvailableEvent, Action<string> onNoAdAvailableEvent, Action<TapsellError> onErrorEvent, 
            Action<string> onNoNetworkEvent, Action<TapsellAd> onExpiringEvent)
        {
            Debug.LogWarning("RequestAd...");
            Tapsell.RequestAd(zoneId, isCached, onAdAvailableEvent, onNoAdAvailableEvent, onErrorEvent, onNoNetworkEvent, onExpiringEvent);
        }
        
        public void ShowAd(TapsellAd ad)
        {
            Tapsell.ShowAd(ad);
        }

        public void ReqBannerAd(string zoneId, int bannerType, Action<string> onRequestFilledEvent, Action<string> onNoAdAvailableEvent, 
            Action<TapsellError> onErrorEvent, Action<string> onNoNetworkEvent,  Action<string> onHideBannerEvent)
        {
            Tapsell.RequestBannerAd(zoneId, bannerType, Gravity.CENTER, Gravity.CENTER, onRequestFilledEvent,
                onNoAdAvailableEvent, onErrorEvent, onNoNetworkEvent, onHideBannerEvent);
        }

        public void ShowBannerAd(string zoneId)
        {
            Tapsell.ShowBannerAd(zoneId);
        }
        
        public void ReqNativeBannerAd(MonoBehaviour monoBeh, string zoneId, Action<TapsellNativeBannerAd> onRequestFilledEvent, Action<string> onNoAdAvailableEvent, 
            Action<TapsellError> onErrorEvent, Action<string> onNoNetworkEvent)
        {
            Tapsell.RequestNativeBannerAd(monoBeh, zoneId, onRequestFilledEvent, onNoAdAvailableEvent, onErrorEvent, onNoNetworkEvent);
        }
    }
}