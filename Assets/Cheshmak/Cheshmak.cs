using System;
using CheshmakMe;
using UnityEngine;

namespace CheshmakMe
{
    
    public class Cheshmak : MonoBehaviour
    {
        static  string debugTag = "Cheshmak_unity";
        
        
        void Start()
        {
            try
            {
                CheshmakLib.initEvents(AdsEvents); 
                
                
                CheshmakLib.setTestMode(); // Remove this line before release !! 
                // CheshmakLib.initializeBannerAds("top"); // [top | center | bottom] load banner on the top of screen;
                // CheshmakLib.initializeMRECAds("bottom"); // [top | center | bottom] load banner on the top of screen;
                // CheshmakLib.initializeInterstitialAds(); // Waiting for a while .... then call showInterstitialAds();
                // CheshmakLib.initializeRewardedAd(); // Waiting for a while .... then call showRewardedAd(); 
                            
            }
            catch(Exception e)
            {
                Debug.Log(debugTag + " :  init error"  + e.Message);
            }
        }



        void AdsEvents(string type, string eventName, string paramString)
        {
            Debug.Log("c# AdsEvents "+type+"   "+eventName+"   "+paramString);
        }

     

        void onJsonReceived(string jsonString)
        {
            Debug.Log(debugTag + " : onJsonReceived");
            
            if (!String.IsNullOrEmpty(jsonString))
            {
                Debug.Log(debugTag + " : " + jsonString);
               
            }
            else
            {
                Debug.Log(debugTag + " : jsonString IsNullOrEmpty");
                
            }
        }

        void onCheshmakIdReceived(string cheshmakID)
        {
            Debug.Log(debugTag + " : onCheshmakIdReceived");
            
            if (!String.IsNullOrEmpty(cheshmakID))
            {
                Debug.Log(debugTag + " : " + cheshmakID);
               
            }
            else
            {
                Debug.Log(debugTag + " : Cheshmak ID IsNullOrEmpty");
                
            }
        }


    }
    
 

}