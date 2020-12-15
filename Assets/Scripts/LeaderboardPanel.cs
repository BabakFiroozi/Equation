using System;
using System.Threading.Tasks;
using DG.Tweening;
using FiroozehGameService.Core;
using FiroozehGameService.Models.BasicApi;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    [RequireComponent(typeof(PopupScreen))]
    public class LeaderboardPanel : MonoBehaviour
    {
        [SerializeField] GameObject _scoreItemObj;
        [SerializeField] GameObject _betweenObj;
        [SerializeField] Transform _content;
        [SerializeField] int _maxShow = 10;
        [SerializeField] Button _infoButton;
        [SerializeField] Button _infoPaper;
        [SerializeField] GameObject _loadingOverlay;
        [SerializeField] Button _editButton;
        [SerializeField] GameObject _editProfilePrefab;

        string _meNickName;
        string _meEmailAddress;
        string _meLogoUrl;
        
        void Start()
        {
            _scoreItemObj.SetActive(false);
            _betweenObj.SetActive(false);
            _infoButton.onClick.AddListener(InfoButtonClick);
            _infoPaper.onClick.AddListener(InfoPaperClick);
            _infoPaper.gameObject.SetActive(false);

            GetComponent<PopupScreen>().HideEvent = InfoPaperClick;
            
            _editButton.onClick.AddListener(ShowEdit);
        }
        
        
        void ShowEdit()
        {
            var obj = Instantiate(_editProfilePrefab, transform.parent);
            obj.GetComponent<EditProfilePanel>().ShowPanel(_meNickName, _meEmailAddress, _meLogoUrl, EditedHandler);
            obj.GetComponent<ConfirmScreen>().ClosedEvent = () => Destroy(obj);
        }

        void EditedHandler()
        {
            SwitchedHandler();
        }


        void SwitchedHandler()
        {
            ShowLeaderboard();
        }

        void InfoButtonClick()
        {
            _infoButton.enabled = false;
            _infoPaper.enabled = true;
            _infoPaper.gameObject.SetActive(true);
            var tr = _infoPaper.transform;
            tr.localScale = Vector3.one * .1f;
            tr.DOScale(Vector3.one, .25f);
        }

        void InfoPaperClick()
        {
            _infoButton.enabled = true;
            _infoPaper.enabled = false;
            var tr = _infoPaper.transform;
            tr.DOScale(Vector3.one * .1f, .25f).onComplete = () => _infoPaper.gameObject.SetActive(false);;
        }

        public void Show()
        {
            GetComponent<PopupScreen>().Show();
            ShowLeaderboard();
        }
        
        void ShowLeaderboard()
        {
            ShowLeaderboardAsync();
        }

        async void ShowLeaderboardAsync()
        {
            _loadingOverlay.SetActive(true);
            
            foreach (var obj in _content)
            {
                var tr = obj as Transform;
                Destroy(tr.gameObject);
            }
            
            // var leaderboards = await GameService.GetLeaderBoards();
            LeaderBoardDetails details = null;
            details = await GameService.GetLeaderBoardDetails(GameConfig.Instance.LeaderboardId);

            print(details.ToString());

            await Task.Delay(200);
            
            _loadingOverlay.SetActive(false);

            bool meWasInLeaderboaed = false;
            var scores = details.Scores;
            for (int i = 0; i < scores.Count; ++i)
            {
                var score = scores[i];
                var obj = Instantiate(_scoreItemObj, _content);
                obj.SetActive(true);
                obj.GetComponent<LeaderboardPanelItem>().FillData(score.Submitter.Name, score.Rank, score.Value, score.Submitter.Logo, score.Submitter.User.IsMe);
                if (score.Submitter.User.IsMe)
                    meWasInLeaderboaed = true;
                await Task.Delay(30);
                if (i + 1 == _maxShow)
                    break;
            }

            if (!meWasInLeaderboaed)
            {
                var score = scores.Find(s => s.Submitter.User.IsMe);
                var obj = Instantiate(_betweenObj, _content);
                obj.SetActive(true);
                obj = Instantiate(_scoreItemObj, _content);
                obj.SetActive(true);
                obj.GetComponent<LeaderboardPanelItem>().FillData(score.Submitter.Name, score.Rank, score.Value, score.Submitter.User.Logo, score.Submitter.User.IsMe);
            }

            {
                var meScore = scores.Find(s => s.Submitter.User.IsMe);
                _meNickName = meScore.Submitter.Name;
                _meEmailAddress = meScore.Submitter.User.Email;
                _meLogoUrl = meScore.Submitter.Logo;
            }
        }
    }
}