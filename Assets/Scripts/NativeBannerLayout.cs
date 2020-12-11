using System;
using TapsellSDK;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class NativeBannerLayout : MonoBehaviour
    {
        [SerializeField] Text _titleText;
        [SerializeField] Text _descText;
        [SerializeField] RawImage _iconImage;
        [SerializeField] Image _bannerImage;
        [SerializeField] Text _actionText;
        
        void Start()
        {
        }

        public void ShowAd(TapsellNativeBannerAd nativeBannerAd)
        {
            _titleText.text = FarsiSaz.Farsi.Fix(nativeBannerAd.title, true);
            _descText.text = FarsiSaz.Farsi.Fix(nativeBannerAd.description, true);
            _iconImage.texture = nativeBannerAd.iconImage;
            _bannerImage.sprite = Sprite.Create(nativeBannerAd.landscapeBannerImage, new Rect(0, 0, nativeBannerAd.landscapeBannerImage.width, nativeBannerAd.landscapeBannerImage.height), new Vector2(.5f, .5f));
            _actionText.text = FarsiSaz.Farsi.Fix(nativeBannerAd.callToActionText, true);
            
            var confirm = gameObject.GetComponent<ConfirmScreen>();
            confirm.OpenConfirm(type =>
            {
                if (type == ConfirmScreen.ConfirmTypes.Ok)
                {
                    nativeBannerAd.Clicked();
                    confirm.CloseConfirm();
                    MyAnalytics.SendEvent(MyAnalytics.quit_ad_clicked);
                }
            });

            confirm.ClosedEvent = () => Destroy(gameObject);
        }
    }
}