using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Equation
{
    public class LeaderboardPanelItem : MonoBehaviour
    {
        [SerializeField] Text _rankText;
        [SerializeField] Text _nameText;
        [SerializeField] Text _valueText;
        [SerializeField] Image _logoImage;
        [SerializeField] GameObject _meBadge;
        [SerializeField] Image _crownImage;
        [SerializeField] Sprite[] _crownSprites;


        public void FillData(string name, int rank, int value, string logo, bool isMe)
        {
            if (rank < 4)
            {
                _crownImage.sprite = _crownSprites[rank - 1];
            }
            else
            {
                _crownImage.gameObject.SetActive(false);
            }
            
            _nameText.text = NBidi.NBidi.LogicalToVisual(name);
            _rankText.text = $".{rank}";
            _valueText.text = value.ToString();
            SetSpriteFromUrl(_logoImage, logo);
            _meBadge.SetActive(isMe);
        }

        void SetSpriteFromUrl(Image image, string url)
        {
            SceneTransitor.Instance.DownloadTexture(url, (tex, error) =>
            {
                if (tex != null)
                    image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
            });
        }

        
    }
}