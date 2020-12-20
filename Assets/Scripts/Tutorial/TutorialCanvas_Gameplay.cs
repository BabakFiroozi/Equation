using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Equation
{
    public class TutorialCanvas_Gameplay : MonoBehaviour
    {
        public enum Steps
        {
            Welcome,
            Introduce,
            MovePawn,
            Gap_1,
            Gap_2,
            UseHint,
            UseHelp,
            End_1,
        }

       static Steps _curStep;

        public static Steps CurrentStep
        {
            get
            {
                return (Steps) PlayerPrefs.GetInt("Tutorial_Gameplay_Step", 0);
            }
            set
            {
                _curStep = value;
                PlayerPrefs.SetInt("Tutorial_Gameplay_Step", (int)_curStep);
            }
        }

        [SerializeField] RectTransform _handTr;
        [SerializeField] RectTransform _arrowTr;

        [SerializeField] Canvas _tutorialCanvas;
        [SerializeField] CanvasGroup _tutorialCanvasGroup;

        [SerializeField] Button _nextStepButton;
        [SerializeField] Text _nextStepButtonText;

        [SerializeField] Button _playButton;

        [SerializeField] GameObject Welcome_Message;
        [SerializeField] GameObject Introduce_Message;
        [SerializeField] GameObject MovePawn_Message;
        [SerializeField] GameObject Hint_Message;
        [SerializeField] GameObject Help_Message;
        [SerializeField] int _canvasElementOrder = 7;

        Canvas _stepCanvas;
        GraphicRaycaster _stepRayeCaster;

        TutMgr _gameSceneTutData;


        public static TutorialCanvas_Gameplay Instance { get; private set; }


        void Awake()
        {
            if (!GameConfig.Instance.TutorialIsActive)
            {
                Destroy(gameObject);
                return;
            }
            
            if (Instance == null)
            {
                if (CurrentStep == Steps.End_1)
                {
                    Instance = null;
                    Destroy(gameObject);
                    return;
                }
                
                if (CurrentStep > Steps.Welcome && CurrentStep < Steps.End_1)
                {
                    GameSaveData.Reset();
                    CurrentStep = Steps.Welcome;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject);
                _nextStepButton.onClick.AddListener(NextStepClick);
                gameObject.SetActive(false);
                _tutorialCanvas.worldCamera = Camera.main;
                SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
            Instance = null;
        }

        void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _tutorialCanvas.worldCamera = Camera.main;
        }

        public void GoToCurrentStep()
        {
            gameObject.SetActive(true);
            StartCoroutine(_GoToStep());
        }

        IEnumerator _GoToStep()
        {
            if (_gameSceneTutData == null)
                _gameSceneTutData = FindObjectOfType<TutMgr>();

            _playButton.gameObject.SetActive(false);
            _nextStepButton.gameObject.SetActive(false);
            _nextStepButton.enabled = true;
            Welcome_Message.SetActive(false);
            Introduce_Message.SetActive(false);
            MovePawn_Message.SetActive(false);
            Hint_Message.SetActive(false);
            Help_Message.SetActive(false);
            _handTr.gameObject.SetActive(false);
            _arrowTr.gameObject.SetActive(false);

            if (CurrentStep == Steps.Welcome)
            {
                _tutorialCanvasGroup.alpha = 0;
                yield return new WaitForSeconds(1.5f);
                _tutorialCanvasGroup.DOFade(1, .5f);
                Welcome_Message.SetActive(true);
                yield return new WaitForSeconds(3);
                _playButton.gameObject.SetActive(true);
                var canvasGroup = _playButton.gameObject.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, .5f);
                _playButton.onClick.AddListener(() =>
                {
                    _playButton.enabled = false;
                    CurrentStep = Steps.Introduce;
                    DataHelper.Instance.LastPlayedInfo.Level = 0;
                    DataHelper.Instance.LastPlayedInfo.Stage = 0;
                    SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME, true);
                });
            }

            if (CurrentStep == Steps.Introduce || CurrentStep == Steps.MovePawn || CurrentStep == Steps.Gap_1 || CurrentStep == Steps.Gap_2 || CurrentStep == Steps.UseHint)
            {
                _gameSceneTutData.BackButton.SetActive(false);
                _gameSceneTutData.ResultPanelMenuButton.SetActive(false);
                _gameSceneTutData.ResultPanelReplayButton.SetActive(false);
            }
            
            if (CurrentStep == Steps.Gap_1 || CurrentStep == Steps.Gap_2)
            {
                CurrentStep++;
                yield break;
            }
            
            if (CurrentStep == Steps.Introduce)
            {
                Introduce_Message.SetActive(true);
                _tutorialCanvasGroup.alpha = 0;
                yield return new WaitForSeconds(1);
                _tutorialCanvasGroup.DOFade(1, 1);
                yield return new WaitForSeconds(5);
                var tr = Introduce_Message.GetComponent<RectTransform>();
                ShowNextButton(tr.anchoredPosition + new Vector2(0, -tr.rect.height / 2));
            }

            if (CurrentStep == Steps.MovePawn)
            {
                _gameSceneTutData.Board.PawnMovedEvent += right =>
                {
                    if (right)
                    {
                        _gameSceneTutData.Board.GameFinishedEvent = null;
                        _tutorialCanvasGroup.DOFade(0, .2f).onComplete = () => _tutorialCanvasGroup.gameObject.SetActive(false);
                        Destroy(_stepRayeCaster);
                        Destroy(_stepCanvas);
                        _handTr.gameObject.SetActive(false);
                        CurrentStep = Steps.Gap_1;
                    }
                    else
                    {
                        ShowHandDrag();
                    }
                };

                _tutorialCanvasGroup.alpha = 0;
                _tutorialCanvasGroup.gameObject.SetActive(true);
                yield return new WaitForSeconds(.5f);
                _stepCanvas = _gameSceneTutData.BoardTable.gameObject.AddComponent<Canvas>();
                _stepCanvas.overrideSorting = true;
                _stepCanvas.sortingOrder = _canvasElementOrder;
                MovePawn_Message.SetActive(true);
                _tutorialCanvasGroup.DOFade(1, 1);

                yield return new WaitForSeconds(1);

                ShowHandDrag();

                void ShowHandDrag()
                {
                    var trans = new List<Transform>();
                    trans.Add(_gameSceneTutData.Board.Pawns[_gameSceneTutData.HandDragIndeices[0]].transform);
                    trans.Add(_gameSceneTutData.Board.Cells[_gameSceneTutData.HandDragIndeices[1]].rectTr.transform);
                    _handTr.gameObject.SetActive(true);
                    _handTr.position = trans[0].position;
                    var handImage = _handTr.GetComponent<Image>();
                    var color = handImage.color;
                    color.a = 0;
                    handImage.color = color;
                    _handDragSeq?.Kill();
                    _handTr.DOKill();
                    var seq = DOTween.Sequence();
                    seq.SetDelay(.5f);
                    seq.Append(handImage.DOFade(1, .3f));
                    seq.Append(_handTr.DOMove(trans[0].position, .5f));
                    seq.Append(_handTr.DOMove(trans[1].position, .5f));
                    seq.AppendInterval(.5f);
                    seq.Append(handImage.DOFade(0, .3f));
                    seq.AppendInterval(.2f);
                    seq.SetLoops(-1);
                    _handDragSeq = seq;
                }
                
                yield return new WaitForSeconds(1.5f);

                _stepRayeCaster = _gameSceneTutData.BoardTable.gameObject.AddComponent<GraphicRaycaster>();
            }

            if (CurrentStep == Steps.UseHint)
            {
                _tutorialCanvasGroup.gameObject.SetActive(true);
                _tutorialCanvasGroup.alpha = 0;
                Hint_Message.SetActive(true);
                yield return new WaitForSeconds(1);
                _tutorialCanvasGroup.DOFade(1, 1);

                yield return new WaitForSeconds(1);

                _stepCanvas = _gameSceneTutData.HintButton.gameObject.AddComponent<Canvas>();
                _stepCanvas.overrideSorting = true;
                _stepCanvas.sortingOrder = _canvasElementOrder;

                _stepRayeCaster = _gameSceneTutData.HintButton.gameObject.AddComponent<GraphicRaycaster>();

                GameSaveData.AddCoin(GameConfig.Instance.HintCost, false, 0);

                _gameSceneTutData.HintButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    CurrentStep = Steps.UseHelp;
                    _tutorialCanvasGroup.DOFade(0, .2f).onComplete = () => _tutorialCanvasGroup.gameObject.SetActive(false);
                    Destroy(_stepRayeCaster);
                    Destroy(_stepCanvas);
                });

                _arrowTr.gameObject.SetActive(true);
                _arrowTr.position = _gameSceneTutData.HintButton.position + new Vector3(-1.2f, 0, 0);
                
                Vector3 pos = _arrowTr.position;

                var seq = DOTween.Sequence();
                seq.Append(_arrowTr.DOMove(pos + new Vector3(.3f, 0, 0), .3f));
                seq.Append(_arrowTr.DOMove(pos, .3f));
                seq.SetLoops(-1);

                yield return new WaitForSeconds(.5f);
            }

            if (CurrentStep == Steps.UseHelp)
            {
                _tutorialCanvasGroup.gameObject.SetActive(true);
                _tutorialCanvasGroup.alpha = 0;
                Help_Message.SetActive(true);
                yield return new WaitForSeconds(1);
                _tutorialCanvasGroup.DOFade(1, 1);

                yield return new WaitForSeconds(1);

                _stepCanvas = _gameSceneTutData.HelpButton.gameObject.AddComponent<Canvas>();
                _stepCanvas.overrideSorting = true;
                _stepCanvas.sortingOrder = _canvasElementOrder;
                _stepRayeCaster = _gameSceneTutData.HelpButton.gameObject.AddComponent<GraphicRaycaster>();

                GameSaveData.AddCoin(GameConfig.Instance.HelpCost, false, 0);
                
                _gameSceneTutData.HelpButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    CurrentStep = Steps.End_1;
                    _tutorialCanvasGroup.DOFade(0, .2f).onComplete = () => _tutorialCanvasGroup.gameObject.SetActive(false);
                    Destroy(_stepRayeCaster);
                    Destroy(_stepCanvas);
                    _gameSceneTutData.BackButton.SetActive(true);
                    Destroy(gameObject);
                });

                _arrowTr.gameObject.SetActive(true);
                _arrowTr.position = _gameSceneTutData.HelpButton.position + new Vector3(-1.2f, 0, 0);
                Vector3 pos = _arrowTr.position;

                var seq = DOTween.Sequence();
                seq.Append(_arrowTr.DOMove(pos + new Vector3(.3f, 0, 0), .3f));
                seq.Append(_arrowTr.DOMove(pos, .3f));
                seq.SetLoops(-1);

                yield return new WaitForSeconds(.5f);
            }
        }
        
        Sequence _handDragSeq = null;

        void Update()
        {
            if (CurrentStep == Steps.MovePawn)
            {
                if (_gameSceneTutData != null && _handTr.gameObject.activeSelf)
                {
                    _handTr.gameObject.SetActive(_gameSceneTutData.Board.DraggingPawn == null);
                }
            }
        }


        void ShowNextButton(Vector2 pos)
        {
            _nextStepButton.gameObject.SetActive(true);

            if (CurrentStep == Steps.Welcome)
                _nextStepButtonText.text = Translator.GetString("Enter");
            if (CurrentStep == Steps.Introduce)
                _nextStepButtonText.text = Translator.GetString("Ok");

            var canvasGroup = _nextStepButton.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, .5f);

            var rectTr = _nextStepButton.GetComponent<RectTransform>();
            rectTr.anchoredPosition = pos + new Vector2(0, -rectTr.rect.height / 2 - 10);
        }

        void HideNextButton()
        {
            var canvasGroup = _nextStepButton.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, .3f).onComplete = () => _nextStepButton.gameObject.SetActive(false);
        }

        void NextStepClick()
        {
            _nextStepButton.enabled = false;
            FinishStep();
        }

        public void FinishStep()
        {
            StartCoroutine(_NextStepClick());
        }

        IEnumerator _NextStepClick()
        {
            if (CurrentStep == Steps.Welcome)
            {
            }

            if (CurrentStep == Steps.Introduce)
            {
                CurrentStep = Steps.MovePawn;
                GoToCurrentStep();
                yield break;
            }
        }
    }
}