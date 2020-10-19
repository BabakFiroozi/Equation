using System;
using System.Collections.Generic;
using System.Linq;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class Board : MonoBehaviour
    {
        public static Board Instance { get; private set; }
        
        [SerializeField] GameObject _pawnPrefab;
        [SerializeField] GameObject _hintPrefab;
        [SerializeField] Transform _groundTr;
        [SerializeField] Material _groundMat;

        public List<Pawn> Pawns { get; } = new List<Pawn>();
        public List<Hint> Hints { get; } = new List<Hint>();

        public List<BoardCell> Cells { get; } = new List<BoardCell>();

      
        Pawn _draggingPawn;

        Puzzle _puzzle;

        Transform _tr;

        
        void Awake()
        {
            Instance = this;
       
            _tr = transform;
            
            GameLevels levelName = GameLevels.Beginner;
            var textAsset = Resources.Load<TextAsset>($"Puzzles/{levelName}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(textAsset.text);
            _puzzle = puzzlesPack.puzzles[0];

            float cellSize = 10f / _puzzle.columns;

            _groundTr.localScale = new Vector3(cellSize * _puzzle.columns, _groundTr.localScale.y, cellSize * _puzzle.rows);

            Vector3 scale = _groundTr.localScale;

            int columnsCount = _puzzle.columns;
            int rowsCount = _puzzle.rows;
            Vector3 startPos = new Vector3(-(scale.x / 2f - cellSize * .5f), cellSize / 2, scale.z / 2f - cellSize * .5f);

            _groundMat.mainTextureScale = new Vector2(columnsCount, rowsCount);
            
            foreach (var seg in _puzzle.segments)
            {
                var cell = new BoardCell();
                cell.index = seg.cellIndex;
                cell.pos = startPos + new Vector3((seg.cellIndex % columnsCount) * cellSize, 0, (-seg.cellIndex / columnsCount) * cellSize);
                Cells.Add(cell);

                if (seg.type == SegmentTypes.Modified)
                {
                    Vector3 hintPos = cell.pos;
                    hintPos.y = _hintPrefab.transform.position.y;
                    var hintObj = Instantiate(_hintPrefab, hintPos, _hintPrefab.transform.rotation, _tr);
                    hintObj.name = $"hit_{seg.content}";
                    hintObj.transform.localScale = new Vector3(cellSize, _hintPrefab.transform.localScale.y, cellSize);
                    var hint = hintObj.GetComponent<Hint>();
                    hint.SetData(seg.content);
                    hintObj.SetActive(false);
                }

                if (seg.type == SegmentTypes.Fixed || seg.type == SegmentTypes.Hollow && seg.hold != -1)
                {
                    var pieceObj = Instantiate(_pawnPrefab, _pawnPrefab.transform.position, _pawnPrefab.transform.rotation, _tr);
                    pieceObj.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                    pieceObj.name = $"piece_{seg.content}";
                    var pawn = pieceObj.GetComponent<Pawn>();
                    pawn.SetData(seg.content, seg.type != SegmentTypes.Fixed);
                    pawn.SetCell(cell, true);
                    Pawns.Add(pawn);
                }
            }
            _hintPrefab.SetActive(false);
            _pawnPrefab.SetActive(false);
            
            ProcessTable();
        }

        bool _isOnGround;

        void Update()
        {
            if (_draggingPawn != null)
            {
                Vector3 mousePos = Input.mousePosition;
                var ray =  Camera.main.ScreenPointToRay(mousePos);
                bool castRes = Physics.Raycast(ray, out var hitInfo, 1000, LayerMaskUtil.GetLayerMask("Ground"));
                if (castRes)
                {
                    Vector3 putPos = hitInfo.point;
                    _draggingPawn.Move(putPos.x, putPos.z);
                    _isOnGround = hitInfo.collider.gameObject.name == "ground";
                }
            }
        }

        public void SetDraggingPiece(Pawn pawn)
        {
            var draggedPawn = _draggingPawn;

            if (pawn == null)
            {
                BoardCell nearestCell = null;
                if (_isOnGround)
                {
                    float minDist = 1000;
                    Vector3 pos = draggedPawn.Trans.position;
                    var emptyCells = Cells.Where(c => c.Pawn == null).ToList();
                    foreach (var cell in emptyCells)
                    {
                        float dist = (pos - cell.pos).magnitude;
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

            ProcessTable();
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

        void ProcessTable(bool horizontally, Dictionary<Pawn, bool> statePawnsDic)
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

        void FinishGame()
        {
            Debug.Log("<color=green>Game Finished!!!</color>");
        }
        
        void OnDestroy()
        {
            Instance = null;
            
            if(Application.isEditor)
                _groundMat.mainTextureScale = new Vector2(10, 14);
        }
    }
    
    
    public class BoardCell
    {
        public int index;
        public Vector3 pos;
        public Pawn Pawn;
    }
}