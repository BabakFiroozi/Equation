using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Equation.Models;
using Random = UnityEngine.Random;


namespace Equation.Tools
{
    public class PuzzleGenerator : EditorWindow
    {
        [MenuItem("Equation/Puzzle Generator")]
        static void ShowWindow()
        {
            var window = GetWindowWithRect<PuzzleGenerator>(new Rect(0, 0, Window_Width, Window_Height), true, "Puzzle Generator", true);
            window.Show();
        }

        const int Window_Width = 840;
        const int Window_Height = 720;

        int _tableColumn = 8;
        int _tableRow = 8;
        int _groupsCount = 3;

        List<Cell> _cellsList = new List<Cell>();

        LevelPuzzle_Model _levelPuzzle;

        List<Group> _horGroups = new List<Group>();
        List<Group> _verGroups = new List<Group>();

        void Awake()
        {
            _tableColumn = PlayerPrefs.GetInt("PuzzleGenerator_tableColumn", _tableColumn);
            _tableRow = PlayerPrefs.GetInt("PuzzleGenerator__tableRow", _tableRow);
            _groupsCount = PlayerPrefs.GetInt("PuzzleGenerator__groupsCount", _groupsCount);
        }

        void OnDestroy()
        {
            PlayerPrefs.SetInt("PuzzleGenerator_tableColumn", _tableColumn);
            PlayerPrefs.SetInt("PuzzleGenerator__tableRow", _tableRow);
            PlayerPrefs.SetInt("PuzzleGenerator__groupsCount", _groupsCount);
        }


        void OnGUI()
        {
            GUI.Label(new Rect(160, 20, 30, 20), "Row");
            _tableRow = EditorGUI.IntField(new Rect(160 + 30, 20, 30, 20), _tableRow);
            GUI.Label(new Rect(240 - 5, 20, 35, 20), "Colm");
            _tableColumn = EditorGUI.IntField(new Rect(240 + 30, 20, 30, 20), _tableColumn);

            _tableRow = Mathf.Clamp(_tableRow, 5, 12);
            _tableColumn = Mathf.Clamp(_tableColumn, 5, 10);
            
            GUI.Label(new Rect(320, 20, 50, 20), "Groups");
            _groupsCount = EditorGUI.IntField(new Rect(320 + 50, 20, 30, 20), _groupsCount);
            
            
            const float cell_margine = 4;
            float tableWidth = 420f;
            float cellSize = 420f / _tableColumn;
            float tableHeight = tableWidth + Mathf.Abs(_tableColumn - _tableRow) * cellSize;

            Rect tableRect = new Rect(140, 100, tableWidth + cell_margine * 1.5f, tableHeight + cell_margine * 1.5f);
            EditorGUI.DrawRect(tableRect, new Color(.8f, .7f, .4f, 1));

            _cellsList.Clear();
            Vector2 cellPos = new Vector2(tableRect.x, tableRect.y);
            for (int i = 0; i < _tableRow * _tableColumn; i++)
            {
                Rect rect = new Rect(cellPos.x + cell_margine, cellPos.y + cell_margine, cellSize - cell_margine / 2, cellSize - cell_margine / 2);
                EditorGUI.DrawRect(rect, new Color(.7f, .5f, .3f, 1));

                var cell = new Cell {index = i, rect = rect, isBusy = false};
                _cellsList.Add(cell);

                cellPos.x += cellSize;

                if ((i + 1) % _tableColumn == 0)
                {
                    cellPos.y += cellSize;
                    cellPos.x = tableRect.x;
                }
            }
            
            if (GUI.Button(new Rect(20, 20, 100, 20), "Generate"))
            {
                GeneratePattern();
            }
            if (GUI.Button(new Rect(20, 40, 100, 20), "Pieces"))
            {
                GeneratePieces();
            }

            var groups = new List<Group>();
            groups.AddRange(_horGroups);
            groups.AddRange(_verGroups);
            foreach (var group in groups)
            {
                var cellIndices = group.parts.Select(p => p.cellIndex).ToList();
                var cellsList = _cellsList.Where(c => cellIndices.Contains(c.index)).ToList();
                foreach (var cell in cellsList)
                {
                    EditorGUI.DrawRect(cell.rect, new Color(.5f, .3f, .1f, .9f));
                }
            }

            GUI.Label(new Rect(tableRect.x - 80, tableRect.y, 100, 20), $"Hors: {_horGroups.Count}");
            GUI.Label(new Rect(tableRect.x - 80, tableRect.y + 20, 100, 20), $"Vers: {_verGroups.Count}");
        }

        bool GeneratePattern()
        {
            _horGroups.Clear();
            _verGroups.Clear();

            var group = MakeGoup(true, 0);
            _horGroups.Add(group);
            foreach (var p in @group.parts.Where(p => !p.isNum))
                _cellsList[p.cellIndex].isBusy = true;

            const int change_iter = 2000;
            int loopIter = 0;
            
            int groupCounter = _horGroups.Count;
            do
            {
                loopIter++;
                
                bool isHor = groupCounter % 2 == 0;
                
                if (loopIter == change_iter * 10)
                    break;
                if (loopIter % change_iter == 0)
                    isHor = !isHor;
                
                group = MakeGoup(isHor, isHor ? _horGroups.Count : _verGroups.Count);
                
                if(group == null)
                    continue;
                
                var parts = group.parts.Where(p => p.index % 2 == 0).ToList();

                var crossGroup = !isHor ? _horGroups[Random.Range(0, _horGroups.Count)] : _verGroups[Random.Range(0, _verGroups.Count)];

                var crossParts = crossGroup.parts.Where(p => p.index % 2 == 0).ToList();

                var crossCellIndices = crossParts.Select(p => p.cellIndex).ToList();
                var numParts = parts.Select(p => p.cellIndex).ToList();
                var intersect = numParts.Intersect(crossCellIndices).ToList();
                if (intersect.Count == 0)
                    continue;

                var notCrossGroups = isHor ? _horGroups : _verGroups;
                var notCrossCellIndices = notCrossGroups.SelectMany(g => g.parts.Select(p => p.cellIndex)).ToList();
                intersect = numParts.Intersect(notCrossCellIndices).ToList();
                if (intersect.Count > 0)
                    continue;

                if (isHor)
                    _horGroups.Add(group);
                else
                    _verGroups.Add(group);

                foreach (var p in group.parts)
                {
                    if (!p.isNum)
                        _cellsList[p.cellIndex].isBusy = true;
                }

                groupCounter++;
            } while (groupCounter < _groupsCount);

            return true;
        }

        Group MakeGoup(in bool isHor, in int groupIndex)
        {
            const int eq_len = 5;

            var freeCellIndices = _cellsList.Where(c => !c.isBusy).ToList().Select(c => c.index).ToList();

            Group group = null;

            int loopIter = 0;
            do
            {
                if (loopIter++ == 5000)
                    break;
                
                bool failed = false;

                int listIndex = Random.Range(0, freeCellIndices.Count);
                int cellIndex = freeCellIndices[listIndex];
                if (isHor)
                {
                    int horCellIndex = cellIndex % _tableColumn;

                    if (horCellIndex > _tableColumn - eq_len)
                        continue;
                }
                else
                {
                    int verCellIndex = cellIndex / _tableColumn;

                    if (verCellIndex > _tableRow - eq_len)
                        continue;
                }

                var parts = new List<Part>();
                for (int i = 0; i < eq_len; ++i)
                {
                    var part = new Part {cellIndex = cellIndex + i * (isHor ? 1 : _tableColumn), index = i};

                    if (!freeCellIndices.Contains(part.cellIndex))
                    {
                        failed = true;
                        break;
                    }

                    parts.Add(part);
                }

                if (failed)
                    continue;

                group = new Group {index = groupIndex, parts = parts};

                break;
            } while (true);

            return group;
        }

        bool GeneratePieces()
        {
            var allGroups = new List<Group>();
            var horGroupsList = _horGroups.ToList();
            var verGroupsList = _verGroups.ToList();
            bool selectHor = false;
            do
            {
                selectHor = !selectHor;

                if (selectHor) //is horizontal
                {
                    allGroups.Add(horGroupsList.ElementAt(0));
                    horGroupsList.RemoveAt(0);
                }
                else
                {
                    allGroups.Add(verGroupsList.ElementAt(0));
                    verGroupsList.RemoveAt(0);
                }
            } while (horGroupsList.Count > 0 || verGroupsList.Count > 0);
            
            var oppsList = new List<string>();
            oppsList.Add("p");
            oppsList.Add("m");

            int numberMin = 1;
            int numberMax = 20;
            
            
            var totalPiecesList = new List<Piece>();
            do
            {
                var group = allGroups.ElementAt(0);
                allGroups.RemoveAt(0);
                
                var piecesList = new List<Piece>();
                foreach (var p in group.parts)
                {
                    var piece = new Piece {cellIndex = p.cellIndex, index = p.index, content = ""};
                    piecesList.Add(piece);
                }

                var oppPieces = piecesList.Where(p => p.index % 2 != 0).ToList();
                oppPieces[0].content = Random.Range(0, 100) > 50 ? "e" : oppsList[Random.Range(0, oppsList.Count)];
                oppPieces[1].content = oppPieces[0].content == "e" ? oppsList[Random.Range(0, oppsList.Count)] : "e";

                var numPieces = new List<Piece>();

                foreach (var op in oppPieces)
                {
                    if (op.content != "e")
                    {
                        numPieces.Add(piecesList[op.index - 1]);
                        numPieces.Add(piecesList[op.index + 1]);
                    }
                }

                var resPiece = piecesList.Find(p => !oppPieces.Contains(p) && !numPieces.Contains(p));
                
                foreach (var p1 in totalPiecesList)
                {
                    foreach (var p2 in numPieces)
                    {
                        if (p1.cellIndex == p2.cellIndex)
                            p2.content = p1.content;
                    }

                    if (resPiece.cellIndex == p1.cellIndex)
                        resPiece.content = p1.content;
                }

                //Try numbers
                do
                {
                    string opperator = oppPieces.Find(p => p.content != "e").content;
                    foreach (var p in numPieces)
                    {
                        if (p.content == "")
                            p.content = Random.Range(numberMin, numberMax).ToString();
                    }

                    if (resPiece.content == "")
                    {
                        if (opperator == "p")
                            resPiece.content = $"{int.Parse(numPieces[0].content) + int.Parse(numPieces[1].content)}";
                        if (opperator == "m")
                            resPiece.content = $"{int.Parse(numPieces[0].content) - int.Parse(numPieces[1].content)}";
                    }

                } while (false);
                
                totalPiecesList.AddRange(piecesList);

            } while (allGroups.Count > 0);

            //Make puzzle pieces
            //Shuffle
            
            Debug.Log("<color=green>Succeed!</color>");

            return true;
        }


        class Cell
        {
            public int index = -1;
            public Rect rect;
            public bool isBusy;
        }

        
        class Group
        {
            public List<Part> parts = new List<Part>();
            public int index = -1;
        }

        class Part
        {
            public int cellIndex = -1;
            public int index = -1;
            public bool isNum => index % 2 == 0;
        }

        class Piece
        {
            public int cellIndex = -1;
            public int index = -1;
            public string content = "";
        }
    }
}