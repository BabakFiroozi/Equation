﻿using System;
using System.Collections.Generic;
using System.Linq;
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

                var crossGroup = !isHor ? _horGroups.Last() : _verGroups.Last();

                var parts = group.parts.Where(p => p.index % 2 == 0).ToList();
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

        bool TryNumbers()
        {
            var group = _horGroups.First();
            foreach (var p in group.parts)
            {
                
            }

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
    }
}