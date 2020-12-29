using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Equation.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Equation
{
    public class Board : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] RectTransform _tableRectTr;
        [SerializeField] GameObject _cellObj;
        [SerializeField] GameObject _pawnObj;
        [SerializeField] GameObject _hintObj;
        [SerializeField] int _tableMargin = 20;
        [SerializeField] int _tableBorder = 20;
        [SerializeField] GameObject _touchBlockObj;
        
        [SerializeField] AudioSource _putSound;
        [SerializeField] AudioSource _rightSound;
        
        [SerializeField] Text _wrongDragMessage;
        
        [SerializeField] CanvasScaler _canvasScaler;

        
        public List<Pawn> Pawns { get; } = new List<Pawn>();
        public List<Hint> Hints { get; } = new List<Hint>();

        public List<BoardCell> Cells { get; } = new List<BoardCell>();
        
        public Action<bool> PawnMovedEvent { get; set; }
        public Action<bool, int> GameFinishedEvent { get; set; }

        public int StagesCount { get; private set; }
      
        public int MovesCount { get; private set; }
        public bool GameFinished { get; private set; }
        
        public int ClausesCount { get; private set; }
        
        public int ShufflesCount { get; private set; }

        
        Pawn _draggingPawn;

        Puzzle _puzzle;

        float _cellSize;

        public Pawn DraggingPawn => _draggingPawn;

        void Start()
        {
            UnBlobkTouch();

            _wrongDragMessage.DOFade(0, 0);
        }

        public void Init()
        {
            MakePuzzleUI();
        }

        void MakePuzzleUI()
        {

            string filePath = !DataHelper.Instance.LastPlayedInfo.Daily ? $"Puzzles/level_{DataHelper.Instance.LastPlayedInfo.Level:000}" : "DailyPuzzles/level_999";
            var textAsset = Resources.Load<TextAsset>(filePath);
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(textAsset.text);
            _puzzle = puzzlesPack.puzzles[DataHelper.Instance.LastPlayedInfo.Stage];
            
            StagesCount = puzzlesPack.puzzles.Count;
            ClausesCount = _puzzle.clauses;
            ShufflesCount = _puzzle.shuffle;

            float cellSize = (_canvasScaler.referenceResolution.x - _tableMargin) / _puzzle.columns;
            _cellSize = cellSize;

            _tableRectTr.sizeDelta = new Vector2(cellSize * _puzzle.columns, cellSize * _puzzle.rows);

            Rect tableRect = _tableRectTr.rect;

            int columnsCount = _puzzle.columns;
            Vector2 startPos = new Vector2(-(tableRect.width / 2f - cellSize * .5f), tableRect.height / 2f - cellSize * .5f);
            _tableRectTr.sizeDelta += new Vector2(_tableBorder, _tableBorder);

            int pawnId = 0;
            foreach (var seg in _puzzle.segments)
            {
                var cell = new BoardCell();
                cell.index = seg.cellIndex;
                cell.pos = startPos + new Vector2((seg.cellIndex % columnsCount) * cellSize, (-seg.cellIndex / columnsCount) * cellSize);
                Cells.Add(cell);

                var cellObj = Instantiate(_cellObj, _tableRectTr);
                cellObj.name = $"Cell_{cell.index + 1}";
                var cellRectTr = cellObj.GetComponent<RectTransform>();
                cellRectTr.sizeDelta = new Vector2(cellSize, cellSize);
                cellRectTr.anchoredPosition = cell.pos;
                cell.rectTr = cellRectTr;
                cell.frame = cellObj.GetComponent<Image>();

                if (seg.type == SegmentTypes.Modified)
                {
                    Vector2 hintPos = cell.pos;
                    hintPos.y -= cellSize / 2 - .02f;
                    var hintObj = Instantiate(_hintObj, _tableRectTr);
                    hintObj.name = $"hint_{seg.content}";
                    var hint = hintObj.GetComponent<Hint>();
                    hint.RectTr.anchoredPosition = hintPos;
                    hint.RectTr.sizeDelta = new Vector2(cellSize, cellSize);
                    hint.SetData(seg.content, cell);
                    hintObj.SetActive(false);
                    Hints.Add(hint);
                }

                if (seg.type == SegmentTypes.Fixed || seg.type == SegmentTypes.Hollow && seg.hold != -1)
                {
                    Vector2 pawnPos = cell.pos;
                    var pawnObj = Instantiate(_pawnObj, pawnPos, Quaternion.identity, _tableRectTr);
                    pawnObj.name = $"pawn_{seg.content}";
                    var pawn = pawnObj.GetComponent<Pawn>();
                    pawn.RectTr.sizeDelta = new Vector2(cellSize, cellSize);
                    pawn.SetData(++pawnId, seg.content, seg.type != SegmentTypes.Fixed);
                    pawn.SetCell(cell, false);
                    Pawns.Add(pawn);
                }
            }

            foreach (var cell in Cells)
                cell.rectTr.SetAsLastSibling();
            foreach (var hint in Hints)
                hint.RectTr.SetAsLastSibling();
            foreach (var pawn in Pawns)
                pawn.RectTr.SetAsLastSibling();

            _cellObj.SetActive(false);
            _pawnObj.SetActive(false);
            _hintObj.SetActive(false);


            var usedHints = GameSaveData.LoadUsedHints(DataHelper.Instance.LastPlayedInfo);
            var hints = Hints.Where(h => usedHints.Contains(h.Cell.index)).ToList();
            foreach (var hint in hints)
                hint.Reveal(false);

            foreach (var pawn in Pawns)
            {
                var cell = GameSaveData.LoadPawnCell(DataHelper.Instance.LastPlayedInfo, pawn.Id, pawn.Cell.index);
                pawn.SetCell(Cells[cell], false);
                bool helped = GameSaveData.LoadPawnHelped(DataHelper.Instance.LastPlayedInfo, pawn.Id);
                if (pawn.Movable && helped)
                    pawn.SetData(pawn.Id, pawn.Content, false);
            }

            ProcessTable();

            if(DataHelper.Instance.LastPlayedInfo.Daily)
                foreach (var hint in Hints)
                    hint.Reveal(false, true);
        }


        public void SetPawnsFont(bool eng)
        {
            foreach (var pawn in Pawns)
            {
                pawn.SetFontEng(eng);
            }
        }
        
        public void SetGridVisible(bool on)
        {
            Color color = Color.white;
            foreach (var cell in Cells)
            {
                color = cell.frame.color;
                color.a = on ? 1 : 0;
                cell.frame.color = color;
            }
        }


        public void SetDraggingPawn(Pawn pawn)
        {
            var prevDragging = _draggingPawn;

            if (pawn == null)
            {
                BoardCell nearestCell = null;
                bool isInTable = _tableRectTr.rect.Contains(prevDragging.RectTr.anchoredPosition);
                if (isInTable)
                {
                    float minDist = 1000;
                    Vector3 pos = prevDragging.RectTr.anchoredPosition;
                    var emptyCells = Cells.Where(c => c.Pawn == null).ToList();
                    foreach (var cell in emptyCells)
                    {
                        Vector3 cellPos = cell.pos;
                        cellPos.z -= _cellSize / 2;
                        float dist = (pos - cellPos).magnitude;
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearestCell = cell;
                        }
                    }

                    _putSound.Play();
                }
                else
                {
                    nearestCell = prevDragging.Cell;
                    OnCancelMove();
                }

                prevDragging.SetCell(nearestCell);
            }
            else
            {
                pawn.RectTr.SetAsLastSibling();
                MovesCount++;
            }

            _draggingPawn = pawn;

            ProcessTable();

            if (prevDragging != null)
                OnRightMove(prevDragging.RightState);
        }

        void OnCancelMove()
        {
            if(!GameSaveData.IsGridVisible())
            {
                Color color = Color.white;
                foreach (var cell in Cells)
                {
                    color = cell.frame.color;
                    color.a = 0;
                    cell.frame.color = color;
                    cell.frame.DOFade(1, .25f);
                    cell.frame.DOFade(0, .25f).SetDelay(.25f + .15f);
                }
            }
        }

        void OnRightMove(bool right)
        {
            if (right)
                _rightSound.Play();
            PawnMovedEvent?.Invoke(right);
        }

        void ProcessTable()
        {
            foreach (var p in Pawns)
                p.SetState(false);
            
            var horDic = new Dictionary<Pawn, bool>();
            ProcessTable(true, horDic);
            foreach (var pair in horDic)
            {
                if (pair.Value)
                    pair.Key.SetState(pair.Value);
            }

            var verDic = new Dictionary<Pawn, bool>();
            ProcessTable(false, verDic);
            foreach (var pair in verDic)
            {
                if (pair.Value)
                    pair.Key.SetState(true);
            }
            
            if (Pawns.All(p => p.RightState))
                FinishGame();
        }

        void ProcessTable(bool horizontally, IDictionary<Pawn, bool> statePawnsDic)
        {
            int rows = _puzzle.rows;
            int cols = _puzzle.columns;

            int rowsCount = horizontally ? cols * rows : cols;
            int colsCount = horizontally ? cols : (rows - 0) * cols;

            int rowStep = horizontally ? cols : 1;
            int colStep = horizontally ? 1 : cols;

            var pawnsList = new List<Pawn>();
            int lastNumIndex = -1;

            for (int r = 0; r < rowsCount; r += rowStep)
            {
                pawnsList.Clear();
                lastNumIndex = -1;
                
                for (int c = 0; c < colsCount; c += colStep)
                {
                    var cell = Cells[r + c];
                    var pawn = cell.Pawn;
                    if (pawn == null)
                    {
                        pawnsList.Clear();
                        lastNumIndex = -1;
                        continue;
                    }

                    if (pawn == _draggingPawn)
                    {
                        pawnsList.Clear();
                        lastNumIndex = -1;
                        continue;
                    }

                    bool parsed = int.TryParse(pawn.Content, out var number);

                    if (pawnsList.Count % 2 == 0 && !parsed || pawnsList.Count % 2 != 0 && parsed)
                    {
                        pawnsList.Clear();
                        if (lastNumIndex != -1)
                        {
                            c = lastNumIndex;
                            lastNumIndex = -1;
                        }
                        continue;
                    }

                    if (parsed)
                        lastNumIndex = c;

                    pawnsList.Add(pawn);

                    if (pawnsList.Count == 5 && pawnsList.Count(p => p.Content == "e") == 1)
                    {
                        int num1 = 0, num2 = 0, numRes = 0;
                        int eqIndex = pawnsList.FindIndex(p => p.Content == "e");
                        string opp = "";
                        if (eqIndex == 1)
                        {
                            numRes = int.Parse(pawnsList[0].Content);
                            num1 = int.Parse(pawnsList[2].Content);
                            opp = pawnsList[3].Content;
                            num2 = int.Parse(pawnsList[4].Content);
                        }

                        if (eqIndex == 3)
                        {
                            num1 = int.Parse(pawnsList[0].Content);
                            opp = pawnsList[1].Content;
                            num2 = int.Parse(pawnsList[2].Content);
                            numRes = int.Parse(pawnsList[4].Content);
                        }

                        int res = 0;
                        if (opp == "p")
                            res = num1 + num2;
                        if (opp == "m")
                            res = num1 - num2;
                        if (opp == "t")
                            res = num1 * num2;
                        if (opp == "d")
                            res = num1 / num2;

                        foreach (var p in pawnsList)
                            statePawnsDic[p] = res == numRes;

                        pawnsList.Clear();
                        lastNumIndex = -1;
                    }

                }
            }
        }

        void BlobkTouch()
        {
            _touchBlockObj.SetActive(true);
        }
        void UnBlobkTouch()
        {
            _touchBlockObj.SetActive(false);
        }

        public bool RestAnyHint()
        {
            return Hints.Where(h => !h.Revealed).Any(h => h.Cell.Pawn == null || h.Cell.Pawn.Content != h.Content);
        }

        public Coroutine DoHint()
        {
            var c = StartCoroutine(_DoHint());
            return c;
        }
        
        IEnumerator<WaitForSeconds> _DoHint()
        {
            var hints = new List<Hint>();

            foreach (var h in Hints)
            {
                if(h.Revealed)
                    continue;
                if(h.Cell.Pawn != null && h.Cell.Pawn.Content == h.Content)
                    continue;
                hints.Add(h);
            }
            
            if(hints.Count == 0)
                yield break;

            BlobkTouch();
            
            var hint = hints[Random.Range(0, hints.Count)];
            float ainmTime = hint.Reveal(true);

            var pawn = Pawns.Find(p => p.Cell.index == hint.Cell.index);
            if (pawn)
            {
                var emptyCells = Cells.Where(c => c.Pawn == null).ToList();
                var emptyCell = emptyCells[Random.Range(0, emptyCells.Count)];
                pawn.SetCell(emptyCell); 
                ProcessTable();
            }

            yield return new WaitForSeconds(ainmTime);
            
            UnBlobkTouch();
        }

        public Coroutine DoHelp()
        {
            var c = StartCoroutine(_DoHelp());
            return c;
        }

        IEnumerator<WaitForSeconds> _DoHelp()
        {
            var hintsList = Hints.Where(h => !h.Revealed).ToList();
            hintsList = hintsList.Where(h => h.Cell.Pawn == null || h.Content != h.Cell.Pawn.Content).ToList();

            if (hintsList.Count == 0)
            {
                hintsList = Hints.Where(h => h.Revealed).ToList();
                hintsList = hintsList.Where(h => h.Cell.Pawn == null || h.Content != h.Cell.Pawn.Content).ToList();
            }

            if (hintsList.Count == 0)
                yield break;

            var pawnsList = Pawns.Where(p => p.Movable).ToList();

            var selectedHint = hintsList[Random.Range(0, hintsList.Count)];

            var selectedPawn = pawnsList.First(p =>
            {
                var hint = Hints.Find(h => h.Cell == p.Cell);
                return (hint == null || hint.Content != p.Content) && p.Content == selectedHint.Content;
            });

            BlobkTouch();

            if (selectedHint.Cell.Pawn != null)
            {
                var emptyCells = Cells.Where(c => c.Pawn == null).ToList();
                var emptyCell = emptyCells[Random.Range(0, emptyCells.Count)];
                selectedHint.Cell.Pawn.SetCell(emptyCell);
            }

            selectedPawn.RectTr.SetAsLastSibling();
            float animTime = selectedPawn.SetCell(selectedHint.Cell, true, true);

            yield return new WaitForSeconds(animTime);

            UnBlobkTouch();
            
            MovesCount++;

            ProcessTable();
        }

        public void DoResetBoard()
        {
            GameSaveData.ResetUsedHints(DataHelper.Instance.LastPlayedInfo);
            foreach (var pawn in Pawns)
            {
                GameSaveData.ResetPawnCell(DataHelper.Instance.LastPlayedInfo, pawn.Id);
                GameSaveData.ResetPawnHelped(DataHelper.Instance.LastPlayedInfo, pawn.Id);
            }
        }

        void FinishGame()
        {
            GameFinished = true;
            
            _touchBlockObj.SetActive(true);
            
            Debug.Log("<color=green>Game Finished!!!</color>");

            bool alreadySolved = GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo);
            
            GameSaveData.SolveStage(DataHelper.Instance.LastPlayedInfo);
            DoResetBoard();

            int limit = ShufflesCount * ClausesCount;

            int rank = 1;
            if (MovesCount <= limit)
                rank = 3;
            else if (MovesCount <= limit * 2)
                rank = 2;

            GameSaveData.SetStageRank(DataHelper.Instance.LastPlayedInfo, rank);
            
            GameFinishedEvent?.Invoke(alreadySolved, rank);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var dragObj = eventData.pointerCurrentRaycast.gameObject;

            if (dragObj == null)
                return;
            
            var pawn = dragObj.GetComponent<Pawn>();
            if (pawn == null)
                return;

            if (!pawn.Movable)
            {
                _wrongDragMessage.DOKill();
                _wrongDragMessage.DOFade(1, .2f);
                _wrongDragMessage.DOFade(0, .2f).SetDelay(.6f);
                return;
            }

            SetDraggingPawn(pawn);
            // Debug.Log(dragObj.name);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!eventData.dragging)
                return;

            if (_draggingPawn != null)
            {
                Vector2 eventPos = eventData.position;
                
                Vector2 centericPos = Vector2.zero;

                centericPos.x = eventPos.x - Screen.width / 2f;
                centericPos.y = -(Screen.height / 2f - eventPos.y);
                
                Vector2 refRes = _canvasScaler.referenceResolution;

                float widthCoef = (refRes.x / 2f) / (Screen.width / 2f);
                float heightCoef = (refRes.x / 2f) / (Screen.width / 2f);

                float screenRatio = Screen.height / (float) Screen.width;
                float refResRatio = refRes.y / refRes.x;
                if(screenRatio < refResRatio)
                {
                    float adaptedRation = refResRatio / screenRatio;
                    widthCoef *= adaptedRation;
                    heightCoef *= adaptedRation;
                }

                Vector2 adaptedPos = new Vector2(centericPos.x * widthCoef, centericPos.y * heightCoef);
                
                _draggingPawn.RectTr.anchoredPosition = adaptedPos;
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (_draggingPawn == null)
                return;
            SetDraggingPawn(null);
        }
    }
    
    
    public class BoardCell
    {
        public int index;
        public Vector2 pos;
        public Pawn Pawn;
        public RectTransform rectTr;
        public Image frame;
    }
}