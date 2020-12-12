using System;
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
        public static Board Instance { get; private set; }
        
        [SerializeField] RectTransform _tableRectTr;
        [SerializeField] GameObject _cellObj;
        [SerializeField] GameObject _pawnObj;
        [SerializeField] GameObject _hintObj;
        [SerializeField] float _tableMargin = 20;
        [SerializeField] float _tableBorder = 10;
        [SerializeField] CoinBox _coinBox;
        [SerializeField] GameObject _solvedBadge;

        public List<Pawn> Pawns { get; } = new List<Pawn>();
        public List<Hint> Hints { get; } = new List<Hint>();

        public List<BoardCell> Cells { get; } = new List<BoardCell>();
        
        public Action GameFinishedEvent { get; set; }

        public CoinBox CoinBox => _coinBox;
        
      
        Pawn _draggingPawn;

        Puzzle _puzzle;

        Transform _tr;
        float _cellSize;

        int _stagesCount;

        void Awake()
        {
            Instance = this;
            _tr = transform;
            
            MakePuzzleUI();
            
            _solvedBadge.SetActive(GameSaveData.IsStageSolved(DataHelper.Instance.LastPlayedInfo));
        }


        void MakePuzzleUI()
        {
            var textAsset = Resources.Load<TextAsset>($"Puzzles/level_{DataHelper.Instance.LastPlayedInfo.Level:000}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(textAsset.text);
            _puzzle = puzzlesPack.puzzles[DataHelper.Instance.LastPlayedInfo.Stage];
            _stagesCount = puzzlesPack.puzzles.Count;

            float cellSize = (Screen.width - _tableMargin) / _puzzle.columns;
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
                var cellRectTr = cellObj.GetComponent<RectTransform>();
                cellRectTr.sizeDelta = new Vector2(cellSize, cellSize);
                cellRectTr.anchoredPosition = cell.pos;
                cell.rectTr = cellRectTr;

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
            }


            ProcessTable();
        }


        public void SetDraggingPawn(Pawn pawn)
        {
            var draggedPawn = _draggingPawn;

            if (pawn == null)
            {
                BoardCell nearestCell = null;
                bool isInTable = _tableRectTr.rect.Contains(draggedPawn.RectTr.anchoredPosition);
                if (isInTable)
                {
                    float minDist = 1000;
                    Vector3 pos = draggedPawn.RectTr.anchoredPosition;
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
                }
                else
                {
                    nearestCell = draggedPawn.Cell;
                }

                draggedPawn.SetCell(nearestCell);
            }

            _draggingPawn = pawn;

            if (_draggingPawn != null)
                _draggingPawn.RectTr.SetAsLastSibling();

            ProcessTable();

            if (draggedPawn != null && draggedPawn.State == PawnStates.Right)
                OnRightMove();
        }

        void OnRightMove()
        {
        }

        void ProcessTable()
        {
            var horDic = new Dictionary<Pawn, bool>();
            ProcessTable(true, horDic);
            foreach (var pair in horDic)
                pair.Key.SetState(pair.Value ? PawnStates.Right : PawnStates.Wrong);

            var otherPawns = Pawns.Where(p => !horDic.ContainsKey(p)).ToList();
            foreach (var p in otherPawns)
                p.SetState(PawnStates.Normal);

            var verDic = new Dictionary<Pawn, bool>();
            ProcessTable(false, verDic);
            foreach (var pair in verDic)
            {
                if(horDic.ContainsKey(pair.Key))
                    continue;
                pair.Key.SetState(pair.Value ? PawnStates.Right : PawnStates.Wrong);
            }
            
            if (Pawns.All(p => p.State == PawnStates.Right))
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

        public void DoHint()
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
                return;
            var hint = hints[Random.Range(0, hints.Count)];
            hint.Reveal(true);
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


            var pawnsList = Pawns.Where(p => p.Movable).ToList();

            var selectedHint = hintsList[Random.Range(0, hintsList.Count)];

            var selectedPawn = pawnsList.First(p =>
            {
                var hint = Hints.Find(h => h.Cell == p.Cell);
                return (hint == null || hint.Content != p.Content) && p.Content == selectedHint.Content;
            });
            
            if (selectedHint.Cell.Pawn != null)
            {
                var emptyCells = Cells.Where(c => c.Pawn == null).ToList();
                var emptyCell = emptyCells[Random.Range(0, emptyCells.Count)];
                selectedHint.Cell.Pawn.SetCell(emptyCell);
            }
            selectedPawn.RectTr.SetAsLastSibling();
            float time = selectedPawn.SetCell(selectedHint.Cell, true, true);
            
            yield return new WaitForSeconds(time);

            ProcessTable();
        }

        public void DoResetBoard()
        {
            foreach (var pawn in Pawns)
                GameSaveData.ResetPawnCell(DataHelper.Instance.LastPlayedInfo, pawn.Id);
        }

        void FinishGame()
        {
            Debug.Log("<color=green>Game Finished!!!</color>");
            GameSaveData.SolveStage(DataHelper.Instance.LastPlayedInfo);
            DoResetBoard();
            GameFinishedEvent?.Invoke();

            UnlockNextPuzzle();
        }

        void UnlockNextPuzzle()
        {
            DataHelper.Instance.LastPlayedInfo.Stage++;
            if (DataHelper.Instance.LastPlayedInfo.Stage == _stagesCount)
            {
                DataHelper.Instance.LastPlayedInfo.Stage = 0;
                if (DataHelper.Instance.LastPlayedInfo.Level < DataHelper.Instance.LevelsCount)
                {
                    DataHelper.Instance.LastPlayedInfo.Level++;
                }
            }

            GameSaveData.UnlockLevel(DataHelper.Instance.LastPlayedInfo.Level);
            GameSaveData.UnlockStage(DataHelper.Instance.LastPlayedInfo.Level, DataHelper.Instance.LastPlayedInfo.Stage);
        }
        
        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var dragObj = eventData.pointerCurrentRaycast.gameObject;

            if (dragObj == null)
                return;
            
            var pawn = dragObj.GetComponent<Pawn>();
            if (pawn == null)
                return;
            
            if(!pawn.Movable)
                return;

            SetDraggingPawn(pawn);
            // Debug.Log(dragObj.name);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!eventData.dragging)
                return;

            if (_draggingPawn != null)
            {
                Vector2 pos = eventData.position;
                pos.y -= Screen.height / 2f;
                pos.x -= Screen.width / 2f;
                _draggingPawn.RectTr.anchoredPosition = pos;
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
    }
}