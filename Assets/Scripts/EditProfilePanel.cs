using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using FiroozehGameService.Core;
using FiroozehGameService.Models;
using CheshmakMe;
using FiroozehGameService.Models.BasicApi;


namespace Equation
{
    public class EditProfilePanel : MonoBehaviour
    {
        [SerializeField] InputField _nicknameInput;
        [SerializeField] InputField _emailInput;
        [SerializeField] Button _logoButton;
        [SerializeField] Image _logoImage;
        [SerializeField] Text _messageText;
        [SerializeField] Transform _logosContainer;
        [SerializeField] GameObject _loadingOverlay;
        [SerializeField] Sprite _defaultLogo;
        [SerializeField] TextAsset _forbiddenWords;

        
        Action EditedEvent;

        Color _messageInitColor;
        
        ConfirmScreen _confirmScreen;

        
        void Awake()
        {
            _messageInitColor = _messageText.color;
            _logosContainer.gameObject.SetActive(false);
        }

        void Start()
        {
            var tr = _logosContainer.Find("viewport/content");
            for (int i = 0; i < tr.childCount; ++i)
            {
                var button = tr.GetChild(i).GetComponent<Button>();
                var b = button;
                bool none = i == 0;
                button.onClick.AddListener(()=>LogoClicked(b, none));
            }

            _logoButton.onClick.AddListener(ShowLogos);
        }

        void ShowLogos()
        {
            _logosContainer.gameObject.SetActive(true);
        }

        void LogoClicked(Button button, bool none)
        {
            _logosContainer.gameObject.SetActive(false);
            if (none)
                return;
            var image = button.transform.Find("image").GetComponent<Image>();
            _logoImage.sprite = image.sprite;
        }

        void Clear()
        {
            _messageText.text = "";
            _nicknameInput.text = "";
            _emailInput.text = "";
            _logoImage.sprite = _defaultLogo;
        }

        public void ShowPanel(string nickName, string emailAddress, string logoUrl, Action editedEvent)
        {
            Clear();
            if (_confirmScreen == null)
                _confirmScreen = GetComponent<ConfirmScreen>();
            _confirmScreen.OpenConfirm(ConfirmedHandler);
            EditedEvent = editedEvent;
            ShowMessage(Translator.GetString("Your_Leaderboard_Info"), false);
            
            _nicknameInput.text = nickName;
            _emailInput.text = emailAddress;
            
            SceneTransitor.Instance.DownloadTexture(logoUrl, (tex, error) =>
            {
                if (tex != null)
                    _logoImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
            });
        }

        public void HidePanel()
        {
            _confirmScreen.CloseConfirm();
        }

        void ConfirmedHandler(ConfirmScreen.ConfirmTypes confirmType)
        {
            if (confirmType == ConfirmScreen.ConfirmTypes.Ok)
            {
                DoEdit();
            }
        }

        void ShowMessage(string text, bool error)
        {
            _messageText.color = error ? Color.red : Color.yellow;
            _messageText.text = text;
        }

        async void DoEdit()
        {
            _messageText.text = "";
            
            string nickName = _nicknameInput.text;
            string emailAddress = _emailInput.text;
            var logo = _logoImage.sprite.texture.EncodeToPNG();

            var forbiddens = _forbiddenWords.text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            bool isForbidden = forbiddens.Any(f => nickName.Contains(f));
            
            if (isForbidden)
            {
                ShowMessage(Translator.GetString("Name_Is_Forbidden"), true);
                return;
            }
            
            if (nickName.Length < SignUpPanel.MIN_NAME_LEN || char.IsDigit(nickName[0]))
            {
                ShowMessage(Translator.GetString("Name_Is_Invalid"), true);
                return;
            }
            
            if (emailAddress.Length > 0 && (emailAddress.Length < SignUpPanel.MIN_EMAIL_LEN || !emailAddress.Contains("@")))
            {
                ShowMessage(Translator.GetString("Email_Is_Wrong"), true);
                return;
            }

            _loadingOverlay.SetActive(true);
            
            await Task.Delay(500);
            
            try
            {
                var memberInfo = await GameService.EditCurrentPlayerProfile(new EditUserProfile(nickName, logo, "", emailAddress));
                Debug.Log("<color=green>Profile edited Succeesful!</color>");
                
                GameSaveData.SetSignupEmail(memberInfo.Email);

                // var jsonObj = new JSONObject();
                // var jsonData = new JSONObject();
                // jsonObj.AddField("Signup_Data_Edited", jsonData);
                // jsonData.AddField(CheshmakLib.getCheshmakID(), memberInfo.Email);
                // CheshmakLib.sendTag(jsonObj.Print());

                MyAnalytics.SendEvent(MyAnalytics.profile_edited);
            }
            catch (GameServiceException e)
            {
                if (e.Message == "used_email")
                {
                    ShowMessage(Translator.GetString("Name_Is_Invalid"), true);
                }

                Debug.LogError(e.Message);
            }
            
            _loadingOverlay.SetActive(false);
            
            EditedEvent?.Invoke();
            
            HidePanel();
        }
    }
}