using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Equation
{
    public class SpinnerPanel : MonoBehaviour
    {
        [SerializeField] RectTransform _spinnerRectTr;
        [SerializeField] Button _rotateButton;
        [SerializeField] GameObject _trackObj;
        [SerializeField] Text _timerText;
        [SerializeField] Text _rewardText;
        [SerializeField] Text _messageText;
        [SerializeField] Text _receiveingText;
        [SerializeField] AudioSource _spinAudio;
        [SerializeField] int[] _trackValues;
        [SerializeField] AudioSource _coinSound;
        [SerializeField] AudioSource _gainedSound;

        List<int> _valuesList = new List<int>();
        List<Transform> _trackObjsList = new List<Transform>();
        
        PopupScreen _popupScreen;

        Vector3 _lastDir;

        float _timer = 0;

        int _tracksCount = 8;
        float _trackAngle = 450;

        void Awake()
        {
            _tracksCount = _trackValues.Length;
            _trackAngle = 360f / _tracksCount;
        }


        public void Show()
        {
            if (_popupScreen == null)
                _popupScreen = GetComponent<PopupScreen>();

            _popupScreen.Show();

            ValidateSpinner();
            
            _rewardText.gameObject.SetActive(false);
        }
        
        Coroutine ValidateSpinner()
        {
            var c = StartCoroutine(ValidateSpinnerCoroutine());
            return c;
        }

        IEnumerator ValidateSpinnerCoroutine()
        {
            SetExistSpinner(false, false);

            _receiveingText.text = "";

            yield return new WaitForEndOfFrame();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("No Internet access");
                _receiveingText.text = Translator.GetString("No_Internet_Access");
                yield return new WaitWhile(() => Application.internetReachability == NetworkReachability.NotReachable);
            }

            _receiveingText.text = Translator.GetString("Receveing_Data");

            if (GameConfig.Instance.DailyIsLocal)
                OnGetLiveTime(DateTime.Now); //Mock only for testing
            else
                SceneTransitor.Instance.GetLiveDateTime(OnGetLiveTime);
        }

        DateTime? _lastDateTime;
        void OnGetLiveTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                Debug.LogError("Unable to get live datetime");
                _receiveingText.text = $"<color=red>{Translator.GetString("Unable_To_Get_Time")}</color>";
                return;
            }
            _lastDateTime = dateTime;

            SetExistSpinner(true);
            SetExistSpinner(HandleGetLiveTime(dateTime));
        }

        bool HandleGetLiveTime(DateTime? dateTime)
        {
            bool hasSpinner = !GameSaveData.SpinnerVisited(GameSaveData.GetDailyEntranceNumber());

            var nowDateTime = dateTime.Value;
            int day = nowDateTime.Day, month = nowDateTime.Month, year = nowDateTime.Year;
            if (day < DateTime.DaysInMonth(year, month))
            {
                day++;
            }
            else
            {
                day = 1;
                if (month < 12)
                {
                    month++;
                }
                else
                {
                    month = 1;
                    year++;
                }
            }

            var tomorrow = new DateTime(year, month, day, 0, 0, 0, 0);
            _timer = (float) (tomorrow - nowDateTime).TotalSeconds;

            return hasSpinner;
        }

        void SetExistSpinner(bool exist, bool access = true)
        {
            _receiveingText.text = "";
            if (access)
                _messageText.text = exist ? Translator.GetString("You_May_Be_Lucky") : $"<color=#B5009A>{Translator.GetString("You_Need_To_Wait")}</color>";
            else
                _messageText.text = "";
            _rotateButton.gameObject.SetActive(exist && access);
            _timerText.transform.parent.gameObject.SetActive(!exist && access);
        }

        void Start()
        {
            _lastDir = _spinnerRectTr.rotation * _xAxis;

            for (int i = 0; i < _tracksCount; ++i)
            {
                var obj = _trackObj;
                var tr = _trackObj.transform;
                obj = i == 0 ? _trackObj : Instantiate(_trackObj, tr.position, tr.rotation, _trackObj.transform.parent);
                tr = obj.transform;
                int reward = _trackValues[i];
                _valuesList.Add(reward);
                _trackObjsList.Add(tr);
                tr.Find("amount").GetComponent<Text>().text = $"{reward}";
                tr.Rotate(0, 0, i * _trackAngle);
            }
            
            _rotateButton.onClick.AddListener(() => StartCoroutine(RotateSpinnerCoroutine()));
            
            _rotateButton.gameObject.SetActive(false);
            _messageText.text = "";
        }

        IEnumerator<YieldInstruction> RotateSpinnerCoroutine()
        {
            MyAnalytics.SendEvent(MyAnalytics.rotateWheel_button_clicked);
            
            _rotateButton.interactable = false;
            _rotateButton.transform.GetComponentInChildren<Text>().color = Color.gray;
            _popupScreen.AllowHide(false);
            GameSaveData.VisitSpinner(GameSaveData.GetDailyEntranceNumber(), true);

            float step = 360f / _tracksCount;
            int track = Random.Range(0, _tracksCount);
            float spinsAngle = (_trackValues.Length - 3) * 360 + track * step;
            int reward = _valuesList[track];

            print("spinner reward: " + reward);

            const float spin_duration = 10;

            _spinAudio.PlayDelayed(.2f);

            _spinnerRectTr.DORotate(new Vector3(0, 0, -spinsAngle), spin_duration, RotateMode.WorldAxisAdd).SetEase(Ease.InOutQuart);

            yield return new WaitForSeconds(spin_duration);

            var larges = _valuesList.OrderByDescending(v => v).Take(3).ToList();

            if (reward >= larges[0])
                _messageText.text = Translator.GetString("You_Very_Lucky");
            else if (reward >= larges[1])
                _messageText.text = Translator.GetString("You_Fair_lucky");
            else if (reward >= larges[2])
                _messageText.text = Translator.GetString("Bad_The_Luck");
            else
                _messageText.text = Translator.GetString("Damn_The_Luck");
            
            _rewardText.gameObject.SetActive(true);

            _rewardText.text = "";
            var textTr = _trackObjsList[track].Find("amount");
            textTr = Instantiate(textTr.gameObject, textTr.position, textTr.rotation, _rewardText.transform.parent).transform;
            textTr.GetComponent<Text>().color = Color.green;
            textTr.DOMove(_rewardText.transform.position, .6f);
            textTr.DOScale(2.5f, .3f);
            textTr.DOScale(.5f, .3f).SetDelay(.3f);

            _gainedSound.Play();

            yield return new WaitForSeconds(.7f);

            Destroy(textTr.gameObject);


            float interval = .1f;
            float coinStep = reward < 100f ? 1 : reward / 100f;

            if (reward > 30)
            {
                interval = .033f;
                coinStep = reward / 30f / 3f;
            }
            
            var waitForSec = new WaitForSeconds(interval);

            for (float c = 0; c < reward; c += coinStep)
            {
                _rewardText.text = $"<color=white>{Translator.CROSS_SIGN}</color>{(int) (c + 1)}";
                if (!_coinSound.isPlaying)
                    _coinSound.Play();
                yield return waitForSec;
            }

            _rewardText.text = $"<color=white>{Translator.CROSS_SIGN}</color>{reward}";

            GameSaveData.AddCoin(reward, false, 0);

            _popupScreen.AllowHide(true);

            _rotateButton.gameObject.SetActive(false);
            _timerText.transform.parent.gameObject.SetActive(true);
            HandleGetLiveTime(_lastDateTime + TimeSpan.FromSeconds((int) spin_duration));
        }


        readonly Vector3 _xAxis = new Vector3(1, 0, 0);

        void Update()
        {
            Vector3 dir = _spinnerRectTr.rotation * _xAxis;
            float diff = Vector3.Angle(dir, _lastDir);

            if (diff >= _trackAngle)
            {
                _spinAudio.Play();
                _lastDir = dir;
            }

            if (_timer != 0)
            {
                _timer -= Time.deltaTime;
                _timerText.text = HelperMethods.TimeToString((int) _timer);

                if (_timer <= 0)
                {
                    _timer = 0;
                    ValidateSpinner();
                }
            }
        }
    }
}