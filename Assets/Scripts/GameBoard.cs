﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class GameBoard : MonoBehaviour
    {
        public static GameBoard Instance { get; private set; }
        
        [SerializeField] GameObject _pawnPrefab;

        public List<Pawn> Pawns { get; } = new List<Pawn>();

        public List<BoardCell> Cells { get; } = new List<BoardCell>();

      
        Pawn _draggingPawn;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameLevels levelName = GameLevels.Beginner;
            var textAsset = Resources.Load<TextAsset>($"Puzzles/{levelName}");
            var puzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(textAsset.text);
            var puzzle = puzzlesPack.puzzles[0];

            int columnsCount = 7;
            Vector3 posOffset = _pawnPrefab.transform.position;
            foreach (var seg in puzzle.segments)
            {
                var cell = new BoardCell();
                cell.index = seg.cellIndex;
                cell.pos = posOffset + new Vector3(seg.cellIndex % columnsCount, 0, -seg.cellIndex / columnsCount);
                Cells.Add(cell);

                if (seg.type == SegmentTypes.Fixed || seg.type == SegmentTypes.Hollow && seg.hold != -1)
                {
                    var pieceObj = Instantiate(_pawnPrefab, _pawnPrefab.transform.position, _pawnPrefab.transform.rotation);
                    var pawn = pieceObj.GetComponent<Pawn>();
                    pawn.SetData(seg.content, seg.type != SegmentTypes.Fixed);
                    pawn.SetCell(cell, true);
                    Pawns.Add(pawn);
                }
            }
            _pawnPrefab.SetActive(false);
        }

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
                    _draggingPawn.Put(putPos.x, putPos.z);
                }
            }
        }
        
        public void SetDraggingPiece(Pawn pawn)
        {
            var draggedPawn = _draggingPawn;

            if (pawn == null)
            {
                float minDist = 1000;
                BoardCell nearestCell = null;
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

                draggedPawn.SetCell(nearestCell);
            }
            
            _draggingPawn = pawn;

            if (pawn == null)
            {
                ProcessTable();
            }
        }

        void ProcessTable()
        {
            var statePawnsDic = new Dictionary<Pawn, bool>();
            ProcessTable(true, statePawnsDic);
            foreach (var pair in statePawnsDic)
                pair.Key.SetState(pair.Value ? PawnStates.Right : PawnStates.Wrong);

            var otherPawns = Pawns.Where(p => !statePawnsDic.ContainsKey(p)).ToList();
            foreach (var p in otherPawns)
                p.SetState(PawnStates.Normal);

            statePawnsDic.Clear();
            ProcessTable(false, statePawnsDic);
            foreach (var pair in statePawnsDic)
                pair.Key.SetState(pair.Value ? PawnStates.Right : PawnStates.Wrong);
            
            if (Pawns.All(p => p.State == PawnStates.Right))
                FinishGame();
        }

        void ProcessTable(bool horizontally, Dictionary<Pawn, bool> statePawnsDic)
        {
            int cols = 7;
            int rows = 10;

            int rowsCount = horizontally ? cols * rows : cols;
            int colsCount = horizontally ? cols : (cols - 1) * rows;

            int rowStep = horizontally ? cols : 1;
            int colStep = horizontally ? 1 : cols;

            var pawnsList = new List<Pawn>();
            
            for (int r = 0; r < rowsCount; r += rowStep)
            {
                pawnsList.Clear();
                for (int c = horizontally ? 0 : r; c < colsCount; c += colStep)
                {
                    var cell = Cells[r + c];
                    var pawn = cell.Pawn;
                    if (pawn == null)
                    {
                        pawnsList.Clear();
                        continue;
                    }

                    bool parsed = int.TryParse(pawn.Content, out var number);

                    if (pawnsList.Count % 2 == 0 && !parsed || pawnsList.Count % 2 != 0 && parsed)
                    {
                        pawnsList.Clear();
                        continue;
                    }

                    pawnsList.Add(pawn);
                    if (pawnsList.Count == 5)
                        break;
                }

                if (pawnsList.Count == 5)
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

                    if(res != 0 && numRes != 0)
                        foreach (var pawn in pawnsList)
                            statePawnsDic.Add(pawn, res == numRes);
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
        }
    }
    
    
    public class BoardCell
    {
        public int index;
        public Vector3 pos;
        public Pawn Pawn;
    }
}