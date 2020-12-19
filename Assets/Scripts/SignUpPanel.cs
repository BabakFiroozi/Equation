using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CheshmakMe;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using FiroozehGameService.Core;
using FiroozehGameService.Models;


namespace Equation
{
    [RequireComponent(typeof(ConfirmScreen))]
    public class SignUpPanel : MonoBehaviour
    {
        [SerializeField] InputField _nicknameInput;
        [SerializeField] InputField _emailInput;
        [SerializeField] InputField _passwordInput;
        [SerializeField] Button _loginButton;
        [SerializeField] Text _messageText;
        [SerializeField] GameObject _loadingOverlay;
        [SerializeField] TextAsset _forbiddenWords;
        
        ConfirmScreen _confirmScreen;

        Action<bool> SignedUpEvent;

        Color _messageInitColor;
        
        public const int MIN_NAME_LEN = 3;
        public const int MIN_PASSW_LEN = 4;
        public const int MIN_EMAIL_LEN = 10;

        void Awake()
        {
            _messageInitColor = _messageText.color;
        }

        void Start()
        {
            _loginButton.onClick.AddListener(DoLoginAsync);
        }


        void Clear()
        {
            _messageText.text = "";
            _nicknameInput.text = "";
            _emailInput.text = "";
            _passwordInput.text = "";
        }

        public void ShowPanel(Action<bool> signedUpEvent)
        {
            Clear();
            if (_confirmScreen == null)
                _confirmScreen = GetComponent<ConfirmScreen>();
            _confirmScreen.OpenConfirm(ConfirmedHandler);
            SignedUpEvent = signedUpEvent;
            ShowMessage(Translator.GetString("You_Need_To_Register"), false);
        }

        public void HidePanel()
        {
            _confirmScreen.CloseConfirm();
        }

        void ConfirmedHandler(ConfirmScreen.ConfirmTypes confirmType)
        {
            if (confirmType == ConfirmScreen.ConfirmTypes.Ok)
            {
                DoSignUpAsync();
            }
        }

        void ShowMessage(string text, bool error)
        {
            _messageText.color = error ? Color.red : Color.yellow;
            _messageText.text = text;
        }

        async void DoLoginAsync()
        {
            _messageText.text = "";
            
            string emailAddress = _emailInput.text;
            string password = _passwordInput.text;
            
            // emailAddress = $"E{MakeSignupData(SystemInfo.deviceUniqueIdentifier)}@ganj.ir";
            // password = MakeSignupData(SystemInfo.deviceUniqueIdentifier);

            if (emailAddress.Length < MIN_EMAIL_LEN || !emailAddress.Contains("@") || password.Length < MIN_PASSW_LEN)
            {
                ShowMessage(Translator.GetString("Email_Or_Password_Is_Wrong"), true);
                return;
            }

            _loadingOverlay.SetActive(true);

            await Task.Delay(500);
                
            try
            {
                string token = await GameService.Login(emailAddress, password);
                Debug.Log("<color=green>Logged in Succeesful!</color>");
                GameSaveData.SetPlayerToken(token);
                
                GameSaveData.SetSignupEmail(emailAddress);
                
                SignedUpEvent?.Invoke(true);
                
                _loadingOverlay.SetActive(false);

                MyAnalytics.SendEvent(MyAnalytics.profile_logged_in);
            }
            catch (GameServiceException e)
            {
                if (e.Message == "user_notfound")
                {
                    ShowMessage(Translator.GetString("You_Not_Signed_Up_Yet"), true);
                }

                Debug.LogError(e.Message);
            }
        }


        string MakeSignupData(string info)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(info));
                var sb = new StringBuilder();
                foreach (var h in hashBytes)
                    sb.Append(h.ToString("X"));

                string hexadecimalHash = sb.ToString(); //Data
                return hexadecimalHash;
            }
        }
        

        async void DoSignUpAsync()
        {
            _messageText.text = "";
            
            string nickName = _nicknameInput.text;
            string emailAddress = _emailInput.text;
            string password = _passwordInput.text;

            emailAddress = $"E{MakeSignupData(SystemInfo.deviceUniqueIdentifier)}@math.ir";
            password = MakeSignupData(SystemInfo.deviceUniqueIdentifier);

            var forbiddens = _forbiddenWords.text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            bool isForbidden = forbiddens.Any(f => nickName.Contains(f));

            if (isForbidden)
            {
                ShowMessage(Translator.GetString("Name_Is_Forbidden"), true);
                return;
            }
            
            if (nickName.Length < MIN_NAME_LEN || char.IsDigit(nickName[0]))
            {
                ShowMessage(Translator.GetString("Name_Is_Invalid"), true);
                return;
            }
            
            if (emailAddress.Length < MIN_EMAIL_LEN || !emailAddress.Contains("@") || password.Length < MIN_PASSW_LEN)
            {
                ShowMessage(Translator.GetString("Email_Or_Password_Is_Wrong"), true);
                return;
            }

            _loadingOverlay.SetActive(true);
            
            await Task.Delay(500);
            
            try
            {
                string token = await GameService.SignUp(nickName, emailAddress, password);
                Debug.Log("<color=green>Signed up Succeesful!</color>");
                GameSaveData.SetPlayerToken(token);
                
                GameSaveData.SetSignupEmail(emailAddress);
                
                SignedUpEvent?.Invoke(false);
                
                _loadingOverlay.SetActive(false);

                var jsonObj = new JSONObject();
                var jsonData = new JSONObject();
                jsonObj.AddField("Signup_Data", jsonData);
                jsonData.AddField(CheshmakLib.getCheshmakID(), emailAddress);
                CheshmakLib.sendTag(jsonObj.Print());
                
                MyAnalytics.SendEvent(MyAnalytics.profile_signed_up);
            }
            catch (GameServiceException e)
            {
                if (e.Message == "used_email")
                {
                    ShowMessage(Translator.GetString("You_Already_Signed_Up"), true);
                    DoLoginAsync();
                }

                Debug.LogError(e.Message);
            }
        }
    }
}