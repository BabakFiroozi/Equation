using System;
using System.Collections.Generic;
using System.Linq;
using Equation.Models;
using UnityEngine;

namespace Equation
{
    public class GameBoard : MonoBehaviour
    {
        public class BoardCell
        {
            public int index;
            public Vector3 pos;
            public Pawn Pawn;
        }
        
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
                var emptyCells = Cells.Where(c => c.Pawn == null);
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
        }
        
        void OnDestroy()
        {
            Instance = null;
        }
    }
}