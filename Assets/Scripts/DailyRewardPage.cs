using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Equation
{
    public class DailyRewardPage : MonoBehaviour
    {
        [SerializeField] Button _backButton;
        [SerializeField] Button[] _chestButtons;
        [SerializeField] Transform[] _chestOpens;
        [SerializeField] GameObject _luckMessageObj;
        [SerializeField] Text _gotMessageText;
        
        [SerializeField] AudioSource _openChestAudio;
        [SerializeField] AudioSource _gainRewardAudio;
        [SerializeField] AudioSource _backButtonApearAudio;
        
        [SerializeField] RectTransform _backRectTr;
        

        int[] _rewards;

        bool _done;


        void Start()
        {
            //TODO - reward
            _rewards = new int[3];
            _rewards[0] = Random.Range(40, 70);
            _rewards[1] = Random.Range(100, 130);
            _rewards[2] = Random.Range(160, 190);

            for (int i = 0; i < _rewards.Length; ++i)
            {
                var reward = _rewards[i];
                _rewards[i] = reward - reward % 5;
            }

            var rewardsList = _rewards.ToList();

            for (int i = 0; i < _rewards.Length; ++i)
            {
                int randomIndex = Random.Range(0, rewardsList.Count);
                int reward = rewardsList[randomIndex];
                _rewards[i] = reward;
                rewardsList.RemoveAt(randomIndex);
            }

            _backButton.onClick.AddListener(GoToMainMenu);

            for (int i = 0; i < _chestButtons.Length; ++i)
            {
                var button = _chestButtons[i];
                int b = i;
                button.onClick.AddListener(() => SelectChest(b));
            }

            for (int i = 0; i < _chestOpens.Length; ++i)
            {
                var chest = _chestOpens[i];
                chest.Find("coin/amount").GetComponent<Text>().text = $"{Translator.CROSS_SIGN}{_rewards[i]}";
            }

            _gotMessageText.gameObject.SetActive(false);
            _backButton.gameObject.SetActive(false);

            MyAnalytics.SendEvent(MyAnalytics.daily_done, GameSaveData.GetDailyEntranceNumber());
        }
        
        void OnEnable()
        {
            var tr = transform;
            tr.SetSiblingIndex(tr.parent.childCount - 2);
        }

        public void ShowPage()
        {
            gameObject.SetActive(true);

            float posY = _backRectTr.anchoredPosition.y;
            _backRectTr.anchoredPosition += new Vector2(0, _backRectTr.rect.height);
            _backRectTr.DOAnchorPosY(posY, .7f).SetEase(Ease.OutBounce);
            
        }

        void GoToMainMenu()
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU, false);
        }

        void SelectChest(int c)
        {
            if(_done)
                return;
            _done = true;
            StartCoroutine(__OpenChests(c));
            
        }

        IEnumerator __OpenChests(int c)
        {
            var tr = _chestButtons[c].GetComponent<RectTransform>();
            tr.DOPunchScale(new Vector3(.1f, .15f, 0), .3f);
            
            _openChestAudio.Play();
            
            yield return new WaitForSeconds(.8f);

            _luckMessageObj.SetActive(false);

            _gainRewardAudio.Play();
            
            yield return StartCoroutine(__OpenChest(c));

            yield return new WaitForSeconds(.8f);

            int reward = _rewards[c];
            _gotMessageText.gameObject.SetActive(true);
            _gotMessageText.text = $"{Translator.GetString("You_Got_Some_Coin")} <color=#00E701>{reward}</color>";

            GameSaveData.AddCoin(reward, true);

            yield return new WaitForSeconds(1);

            _openChestAudio.Play();
            
            for (int i = 0; i < _chestOpens.Length; ++i)
            {
                if (i == c)
                    continue;
                StartCoroutine(__OpenChest(i));
            }

            yield return new WaitForSeconds(2);

            _backButton.gameObject.SetActive(true);

            tr = _backButton.GetComponent<RectTransform>();
            float posY = tr.anchoredPosition.y;
            tr.anchoredPosition += new Vector2(0, -_backRectTr.rect.height / 2);
            tr.DOAnchorPosY(posY, .3f).SetEase(Ease.OutCubic);
            _backButtonApearAudio.Play();
        }

        IEnumerator __OpenChest(int b)
        {
            var chest = _chestOpens[b];
            _chestButtons[b].gameObject.SetActive(false);
            chest.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(.3f);

            var tr = chest.Find("coin").GetComponent<RectTransform>();
            tr.DOAnchorPosY(tr.anchoredPosition.y + 200, .5f).SetEase(Ease.OutQuad);
        }

    }
}