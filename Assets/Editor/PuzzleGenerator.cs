using System;
using System.Collections.Generic;
using System.IO;
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
        
        // [MenuItem("Equation/Puzzle Generator/Delete Prefs")]
        // public static void DeleteWindowPrefs()
        // {
        // 	DeletePrefs();
        // }

        const int Window_Width = 840;
        const int Window_Height = 740;

        int _rowsCount = 12;
        int _colsCount = 8;
        int _clausesCount = 3;
        int _shuffleCount = 1;
        
        int _numMinRange = 1;
        int _numMaxRange = 20;

        string _genOppers = "td";

        List<Clause> _horClauses = new List<Clause>();
        List<Clause> _verClauses = new List<Clause>();
        
        List<Cell> _allCellsList = new List<Cell>();
        List<Piece> _allPiecesList = new List<Piece>();

        Puzzle _puzzle;
        PuzzlesPackModel _puzzlesPack;

        PuzzlesPackModel _culledPuzzlesPack;

        const string SAVE_PATH = "Resources/Puzzles";
        int _saveGameLevel;
        int _trimSaveGameLevel;
        
        int _loadedLevel = 0;
        int _selectedStage = -1;
        int _culledSelectedStage = -1;

        Font _fontPersian;
        Font _fontEnglish;
        
        Vector2 _stagesScrollPos;
        Vector2 _culledStagesScrollPos;
        
        int _generateCount = 1;

        int _cullClausesCount = 2;
        int _cullShuffleCount = 1;
        int _cullMaxNum = 99;
        
        bool _isGenerating;
        

        void Awake()
        {
            _colsCount = EditorPrefs.GetInt("PuzzleGenerator_colsCount", _colsCount);
            _rowsCount = EditorPrefs.GetInt("PuzzleGenerator_rowsCount", _rowsCount);
            _clausesCount = EditorPrefs.GetInt("PuzzleGenerator_clausesCount", _clausesCount);
            _shuffleCount = EditorPrefs.GetInt("PuzzleGenerator_shuffleCount", _shuffleCount);
            _numMinRange = EditorPrefs.GetInt("PuzzleGenerator_numMinRange", _numMinRange);
            _numMaxRange = EditorPrefs.GetInt("PuzzleGenerator_numMaxRange", _numMaxRange);
            _loadedLevel = EditorPrefs.GetInt("PuzzleGenerator_loadedLevel", _loadedLevel);
            _cullClausesCount = EditorPrefs.GetInt("PuzzleGenerator_cullClausesCount", _cullClausesCount);
            _cullShuffleCount = EditorPrefs.GetInt("PuzzleGenerator_cullShuffleCount", _cullShuffleCount);
            _cullMaxNum = EditorPrefs.GetInt("PuzzleGenerator_cullMaxNum", _cullMaxNum);
            _genOppers = EditorPrefs.GetString("PuzzleGenerator_genOppers", _genOppers);
        }

        void OnDestroy()
        {
            EditorPrefs.SetInt("PuzzleGenerator_colsCount", _colsCount);
            EditorPrefs.SetInt("PuzzleGenerator_rowsCount", _rowsCount);
            EditorPrefs.SetInt("PuzzleGenerator_clausesCount", _clausesCount);
            EditorPrefs.SetInt("PuzzleGenerator_shuffleCount", _shuffleCount);
            EditorPrefs.SetInt("PuzzleGenerator_numMinRange", _numMinRange);
            EditorPrefs.SetInt("PuzzleGenerator_numMaxRange", _numMaxRange);
            EditorPrefs.SetInt("PuzzleGenerator_loadedLevel", _loadedLevel);
            EditorPrefs.SetInt("PuzzleGenerator_cullClausesCount", _cullClausesCount);
            EditorPrefs.SetInt("PuzzleGenerator_cullShuffleCount", _cullShuffleCount);
            EditorPrefs.SetInt("PuzzleGenerator_cullMaxNum", _cullMaxNum);
            EditorPrefs.SetString("PuzzleGenerator_genOppers", _genOppers);
        }
        
        static void DeletePrefs()
        {
            EditorPrefs.DeleteKey("PuzzleGenerator_colsCount");
            EditorPrefs.DeleteKey("PuzzleGenerator_rowsCount");
            EditorPrefs.DeleteKey("PuzzleGenerator_clausesCount");
            EditorPrefs.DeleteKey("PuzzleGenerator_shuffleCount");
            EditorPrefs.DeleteKey("PuzzleGenerator_numMinRange");
            EditorPrefs.DeleteKey("PuzzleGenerator_numMaxRange");
            EditorPrefs.DeleteKey("PuzzleGenerator_loadedLevel");
            EditorPrefs.DeleteKey("PuzzleGenerator_cullClausesCount");
            EditorPrefs.DeleteKey("PuzzleGenerator_cullShuffleCount");
            EditorPrefs.DeleteKey("PuzzleGenerator_cullMaxNum");
            EditorPrefs.DeleteKey("PuzzleGenerator_genOppers");
        }
        
        

        void OnGUI()
        {
            if (_fontPersian == null)
            {
                _fontPersian = AssetDatabase.LoadAssetAtPath<Font>("Assets/Editor/Fonts/B_Yekan_Editor.ttf");
                _fontEnglish = GUI.skin.label.font;
            }

            GUI.Label(new Rect(20, 20, 50, 20), "Row");
            _rowsCount = EditorGUI.IntField(new Rect(25 + 50, 20, 30, 20), _rowsCount);
            GUI.Label(new Rect(20, 45, 50, 20), "Column");
            _colsCount = EditorGUI.IntField(new Rect(25 + 50, 45, 30, 20), _colsCount);

            _rowsCount = Mathf.Clamp(_rowsCount, 6, 21);
            _colsCount = Mathf.Clamp(_colsCount, 5, 18);

            GUI.Label(new Rect(20, 70, 50, 20), "Clauses");
            _clausesCount = EditorGUI.IntField(new Rect(25 + 50, 70, 30, 20), _clausesCount);

            const float width_ref = 340;
            const float cell_margine = 4;
            float tableWidth = width_ref;
            float cellSize = width_ref / _colsCount;
            float tableHeight = tableWidth + Mathf.Abs(_colsCount - _rowsCount) * cellSize;

            Rect tableRect = new Rect(160, 180, tableWidth + cell_margine * 1.5f, tableHeight + cell_margine * 1.5f);
            EditorGUI.DrawRect(tableRect, new Color(.8f, .7f, .4f, 1));

            _allCellsList.Clear();
            Vector2 cellPos = new Vector2(tableRect.x, tableRect.y);
            for (int i = 0; i < _rowsCount * _colsCount; i++)
            {
                Rect rect = new Rect(cellPos.x + cell_margine, cellPos.y + cell_margine, cellSize - cell_margine / 2, cellSize - cell_margine / 2);
                EditorGUI.DrawRect(rect, new Color(.7f, .5f, .3f, 1));

                var cell = new Cell {index = i, rect = rect, isBusy = false};
                _allCellsList.Add(cell);

                cellPos.x += cellSize;

                if ((i + 1) % _colsCount == 0)
                {
                    cellPos.y += cellSize;
                    cellPos.x = tableRect.x;
                }
            }

            _generateCount = EditorGUI.IntField(new Rect(520 - 250, 20, 30, 20), _generateCount);
            if (GUI.Button(new Rect(450 - 250, 20, 65, 20), "Generate"))
            {
                GeneratePuzzles();
            }
            
            if (GUI.Button(new Rect(400 - 250, 20, 42, 20), "Clear"))
            {
                if (EditorUtility.DisplayDialog("Clear", "Are you sure to clear loaded or generated puzzles?", "yes", "no"))
                    ClearGenPuzzles();
            }
            
            GUI.Label(new Rect(400 - 250, 47, 50, 20), "Range");

            _numMinRange = EditorGUI.IntField(new Rect(450 - 250, 45, 45, 20), _numMinRange);
            _numMaxRange = EditorGUI.IntField(new Rect(505 - 250, 45, 45, 20), _numMaxRange);

            GUI.Label(new Rect(400 - 250, 70, 50, 20), "Oppers");
            _genOppers = GUI.TextField(new Rect(450 - 250, 70, 45, 20), _genOppers);
            
            GUI.Label(new Rect(400 - 250, 95, 60, 20), "Shuffle");
            _shuffleCount = EditorGUI.IntField(new Rect(450 - 250, 95, 30, 20), _shuffleCount);
            
            
            if (GUI.Button(new Rect(450, 20, 60, 20), "Cull"))
            {
                CullGeneratedPuzzles();
            }

            GUI.Label(new Rect(450 - 60, 45, 60, 20), "Clauses");
            _cullClausesCount = EditorGUI.IntField(new Rect(450, 45, 60, 20), _cullClausesCount);

            GUI.Label(new Rect(450 - 60, 70, 60, 20), "MaxNum");
            _cullMaxNum = EditorGUI.IntField(new Rect(450, 70, 40, 20), _cullMaxNum);
            
            GUI.Label(new Rect(450 - 60, 95, 60, 20), "Shuffle");
            _cullShuffleCount = EditorGUI.IntField(new Rect(450, 95, 30, 20), _cullShuffleCount);
            
            _trimSaveGameLevel = EditorGUI.IntField(new Rect(780, 20, 40, 20), _trimSaveGameLevel);
            if (GUI.Button(new Rect(720, 20, 50, 20), "Trim"))
                TrimSavePuzzles();
            
            _saveGameLevel = EditorGUI.IntField(new Rect(780, 50, 40, 20), _saveGameLevel);
            if (GUI.Button(new Rect(720, 50, 50, 20), "Save"))
                SavePuzzles();
            
            _loadedLevel = EditorGUI.IntField(new Rect(780, 80, 40, 20), _loadedLevel);
            if (GUI.Button(new Rect(720, 80, 50, 20), "Load"))
                LoadPuzzlePack(_loadedLevel);

            if (_puzzlesPack != null)
            {
                if(GUI.Button(new Rect(650, 150, 60, 20), "Put ->"))
                {
                    PourToCulled();
                }
            }

            
            if (_puzzlesPack != null)
            {
                int stagesCount = _puzzlesPack.puzzles.Count;

                EditorGUI.DrawRect(new Rect(540, tableRect.y, 100, 400), new Color(1, .5f, .9f));

                _stagesScrollPos = GUI.BeginScrollView(new Rect(540, tableRect.y, 100, 400), _stagesScrollPos, new Rect(0, 0, 80, stagesCount * 20));

                for (int i = 0; i < stagesCount; ++i)
                {
                    if (i == _selectedStage)
                        EditorGUI.DrawRect(new Rect(0, i * 20, 80, 20), new Color(1, .1f, .9f));

                    GUI.Label(new Rect(0, i * 20, 70, 20), $" {i + 1:000}");
                    if (GUI.Button(new Rect(65, i * 20, 20, 20), "«"))
                    {
                        _selectedStage = i;
                        _culledSelectedStage = -1;
                    }
                }

                GUI.EndScrollView();
            }
            
            if (_culledPuzzlesPack != null)
            {
                int stagesCount = _culledPuzzlesPack.puzzles.Count;

                EditorGUI.DrawRect(new Rect(580 + 140, tableRect.y, 100, 400), new Color(.5f, .5f, .9f));

                _culledStagesScrollPos = GUI.BeginScrollView(new Rect(580 + 140, tableRect.y, 100, 400), _culledStagesScrollPos, new Rect(0, 0, 80, stagesCount * 20));

                for (int i = 0; i < stagesCount; ++i)
                {
                    if (i == _culledSelectedStage)
                        EditorGUI.DrawRect(new Rect(0, i * 20, 80, 20), new Color(.5f, .1f, .9f));

                    GUI.Label(new Rect(0, i * 20, 70, 20), $" {i + 1:000}");
                    if (GUI.Button(new Rect(65, i * 20, 20, 20), "«"))
                    {
                        _culledSelectedStage = i;
                        _selectedStage = -1;
                    }
                }

                GUI.EndScrollView();
            }
            
            
            int fontSize = GUI.skin.label.fontSize;
            var alignment = GUI.skin.label.alignment;
            GUI.skin.label.font = _fontPersian;
            GUI.skin.label.fontSize = (int) (cellSize * .5f);
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            
            if(_puzzlesPack != null || _culledPuzzlesPack != null)
            {
                Puzzle puzzle = null;
                
                if (_puzzlesPack != null && _selectedStage > -1 && _selectedStage < _puzzlesPack.puzzles.Count)
                    puzzle = _puzzlesPack.puzzles[_selectedStage];
                else if (_culledPuzzlesPack != null && _culledSelectedStage > -1 && _culledSelectedStage < _culledPuzzlesPack.puzzles.Count)
                    puzzle = _culledPuzzlesPack.puzzles[_culledSelectedStage];

                foreach (var cell in _allCellsList)
                {
                    if (puzzle == null)
                        break;
                    foreach (var seg in puzzle.segments)
                    {
                        if (cell.index == seg.cellIndex)
                        {
                            if (seg.type != SegmentTypes.Hollow)
                            {
                                EditorGUI.DrawRect(cell.rect, new Color(.5f, .3f, .1f, .9f));
                                GUI.Label(cell.rect, seg.type == SegmentTypes.Modified ? "" : HelperMethods.CorrectOpperatorContent(seg.content));
                                if (seg.type == SegmentTypes.Fixed)
                                    EditorGUI.DrawRect(new Rect(cell.rect.x, cell.rect.y, 10, 10), new Color(.5f, .5f, .5f, .5f));
                            }
                            else
                            {
                                if (seg.hold != -1)
                                {
                                    var holdPiece = puzzle.segments[seg.cellIndex];
                                    var holdCell = _allCellsList[seg.cellIndex];
                                    EditorGUI.DrawRect(holdCell.rect, new Color(.3f, .3f, .5f, .3f));
                                    GUI.Label(holdCell.rect, HelperMethods.CorrectOpperatorContent(holdPiece.content));
                                }
                            }
                        }
                    }
                }
                
                GUI.skin.label.font = _fontEnglish;
                GUI.skin.label.fontSize = fontSize;
                GUI.skin.label.alignment = alignment;
                
                if(puzzle != null)
                {
                    GUI.Label(new Rect(tableRect.x - 100, tableRect.y, 90, 20), $"Size: {puzzle.rows} X {puzzle.columns}");
                    GUI.Label(new Rect(tableRect.x - 100, tableRect.y + 20, 90, 20), $"Clauses: {puzzle.clauses}");
                    GUI.Label(new Rect(tableRect.x - 100, tableRect.y + 40, 90, 20), $"Shuffle: {puzzle.shuffle}");
                }
            }

            GUI.skin.label.font = _fontEnglish;
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.label.alignment = alignment;
            
            var currentEvent = Event.current;
            if (currentEvent.isKey && currentEvent.type == EventType.KeyDown)
            {
                if (_selectedStage > -1)
                {
                    if (currentEvent.keyCode == KeyCode.DownArrow)
                    {
                        if (_selectedStage < _puzzlesPack.puzzles.Count - 1)
                        {
                            _selectedStage++;
                            if (_selectedStage >= 19)
                                _stagesScrollPos.y += 20;
                        }
                    }
                    if (currentEvent.keyCode == KeyCode.UpArrow)
                    {
                        if (_selectedStage > 0)
                        {
                            _selectedStage--;
                            if (_selectedStage <= 6 && _stagesScrollPos.y > 0)
                                _stagesScrollPos.y -= 20;
                        }
                    }
                }
                
                if (_culledSelectedStage > -1)
                {
                    if (currentEvent.keyCode == KeyCode.DownArrow)
                    {
                        if (_culledSelectedStage < _culledPuzzlesPack.puzzles.Count - 1)
                        {
                            _culledSelectedStage++;
                            if (_culledSelectedStage >= 19)
                                _culledStagesScrollPos.y += 20;
                        }
                    }
                    if (currentEvent.keyCode == KeyCode.UpArrow)
                    {
                        if (_culledSelectedStage > 0)
                        {
                            _culledSelectedStage--;
                            if (_culledSelectedStage <= 6 && _culledStagesScrollPos.y > 0)
                                _culledStagesScrollPos.y -= 20;
                        }
                    }
                }
                
                currentEvent.Use();
                Repaint();
            }
            
            
            if (_isGenerating)
            {
                GUI.Label(new Rect(Window_Width / 2 - 200, Window_Height - 50, 200, 30), "Generating puzzles...");
            }
        }
        
        

        void PourToCulled()
        {
            if (_culledPuzzlesPack == null)
                _culledPuzzlesPack = new PuzzlesPackModel {level = 0, puzzles = new List<Puzzle>()};
            
            int initCount = _culledPuzzlesPack.puzzles.Count;
            for (int i = 0; i < _puzzlesPack.puzzles.Count; ++i)
            {
                var p = _puzzlesPack.puzzles[i];
                var puzzle = new Puzzle();
                puzzle.id = p.id;
                puzzle.rows = p.rows;
                puzzle.columns = p.columns;
                puzzle.clauses = p.clauses;
                puzzle.segments = p.segments.ToList();
                puzzle.shuffle = p.shuffle;
                puzzle.stage = initCount + i;
                _culledPuzzlesPack.puzzles.Add(puzzle);
            }
        }


        void ClearGenPuzzles()
        {
            _puzzlesPack = null;
            _puzzle = null;
        }


        void LoadPuzzlePack(int level)
        {
            try
            {
                string loadPath = $"{Application.dataPath}/{SAVE_PATH}/level_{level:000}.json";
                string data = File.ReadAllText(loadPath);
                _culledPuzzlesPack = JsonUtility.FromJson<PuzzlesPackModel>(data);
                _culledSelectedStage = 0;
                _selectedStage = -1;
                _puzzle = _culledPuzzlesPack.puzzles[_culledSelectedStage];
                _rowsCount = _puzzle.rows;
                _colsCount = _puzzle.columns;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }


        async void GeneratePuzzles()
        {
            ClearGenPuzzles();

            if (_puzzlesPack == null)
                _puzzlesPack = new PuzzlesPackModel {level = 0, puzzles = new List<Puzzle>()};
            
            _selectedStage = 0;

            _isGenerating = true;

            int loopIter = 0;
            do
            {
                if(_clausesCount < 0)
                {
                    if (_clausesCount == -1)
                        GeneratePatternSingle(true);
                    if (_clausesCount == -2)
                        GeneratePatternSingle(false);
                }
                
                if(_clausesCount > 0)
                {
                    GeneratePattern();
                }
                
                await Task.Delay(100);
                Repaint();
                if (!GenerateSegments(loopIter))
                {
                    Debug.LogWarning("<color=yellow>GenerateSegments retried in GeneratePuzzles.</color>");
                    continue;
                }

                await Task.Delay(100);
                ShuffleSegments();
                _selectedStage = loopIter;
                Repaint();
                await Task.Delay(100);

                loopIter++;
                
                if (loopIter > 20)
                    _stagesScrollPos.y += 20;

            } while (loopIter < _generateCount);

            _isGenerating = false;
        }

        void CullGeneratedPuzzles()
        {
            if (_puzzlesPack == null)
            {
                Debug.LogError("Cull failed. Puzzle pack is null");
                return;
            }
            
            var puzzles = _puzzlesPack.puzzles.Where(p =>
            {
                bool ret = p.clauses == _cullClausesCount && (p.shuffle == 0 || p.shuffle == p.clauses + _cullShuffleCount - 1);
                var numbers = new List<int>();
                foreach (var s in p.segments)
                {
                    bool res = int.TryParse(s.content, out int num);
                    if (res)
                        numbers.Add(num);
                }

                if (numbers.Exists(n => n > _cullMaxNum))
                    ret = false;

                return ret;
            });

            _puzzlesPack.puzzles = puzzles.ToList();
        }

        void GeneratePatternSingle(bool hor)
        {
            _horClauses.Clear();
            _verClauses.Clear();

            var clause = MakeClause(hor, 0);
            if (hor)
                _horClauses.Add(clause);
            else
                _verClauses.Add(clause);

            foreach (var p in clause.parts.Where(p => !p.isNum))
                _allCellsList[p.cellIndex].isBusy = true;

            _allPiecesList.Clear();
        }

        void GeneratePattern()
        {
            _horClauses.Clear();
            _verClauses.Clear();

            var clause = MakeClause(true, 0);
            _horClauses.Add(clause);
            foreach (var p in clause.parts.Where(p => !p.isNum))
                _allCellsList[p.cellIndex].isBusy = true;

            const int change_iter = 2000;
            int loopIter = 0;
            
            int clauseCounter = _horClauses.Count;
            do
            {
                loopIter++;
                
                bool isHor = clauseCounter % 2 == 0;
                
                if (loopIter == change_iter * 10)
                    break;
                if (loopIter % change_iter == 0)
                    isHor = !isHor;
                
                clause = MakeClause(isHor, isHor ? _horClauses.Count : _verClauses.Count);
                
                if(clause == null)
                    continue;
                
                var parts = clause.parts.Where(p => p.index % 2 == 0).ToList();

                var crossClause = !isHor ? _horClauses[Random.Range(0, _horClauses.Count)] : _verClauses[Random.Range(0, _verClauses.Count)];

                var crossParts = crossClause.parts.Where(p => p.index % 2 == 0).ToList();

                var crossCellIndices = crossParts.Select(p => p.cellIndex).ToList();
                var numParts = parts.Select(p => p.cellIndex).ToList();
                var intersect = numParts.Intersect(crossCellIndices).ToList();
                if (intersect.Count == 0)
                    continue;

                var notCrossClauses = isHor ? _horClauses : _verClauses;
                var notCrossCellIndices = notCrossClauses.SelectMany(g => g.parts.Select(p => p.cellIndex)).ToList();
                intersect = numParts.Intersect(notCrossCellIndices).ToList();
                if (intersect.Count > 0)
                    continue;

                if (isHor)
                    _horClauses.Add(clause);
                else
                    _verClauses.Add(clause);

                foreach (var p in clause.parts)
                {
                    if (!p.isNum)
                        _allCellsList[p.cellIndex].isBusy = true;
                }

                clauseCounter++;
            } while (clauseCounter < _clausesCount);

            _allPiecesList.Clear();
        }

        Clause MakeClause(in bool isHor, in int clauseIndex)
        {
            const int eq_len = 5;

            var freeCellIndices = _allCellsList.Where(c => !c.isBusy).ToList().Select(c => c.index).ToList();

            Clause clause = null;

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
                    int horCellIndex = cellIndex % _colsCount;

                    if (horCellIndex > _colsCount - eq_len)
                        continue;
                }
                else
                {
                    int verCellIndex = cellIndex / _colsCount;

                    if (verCellIndex > _rowsCount - eq_len)
                        continue;
                }

                var parts = new List<Part>();
                for (int i = 0; i < eq_len; ++i)
                {
                    var part = new Part {cellIndex = cellIndex + i * (isHor ? 1 : _colsCount), index = i};

                    if (!freeCellIndices.Contains(part.cellIndex))
                    {
                        failed = true;
                        break;
                    }

                    parts.Add(part);
                }

                if (failed)
                    continue;

                clause = new Clause {index = clauseIndex, parts = parts};

                break;
            } while (true);

            return clause;
        }
        

        bool GenerateSegments(int stage)
        {
            int failedCounter = 0;
            while (true)
            {
                bool succeed = TryGenerateSegments();
                if (succeed)
                    break;
                failedCounter++;

                if (failedCounter == 3000)
                {
                    Debug.Log($"<color=red>GenerateSegments failed with {failedCounter} try.</color>");
                    return false;
                }
            }

            Debug.Log($"<color=green>GenerateSegments Succeeded with</color> <color=blue>{failedCounter} try.</color>");

            int clauses = _horClauses.Count + _verClauses.Count;

            _puzzle = new Puzzle {stage = stage, id = Guid.NewGuid().ToString(), rows = _rowsCount, columns = _colsCount, segments = new List<Segment>(), clauses = clauses};

            for (int index = 0; index < _allCellsList.Count; ++index)
            {
                var cell = _allCellsList[index];
                var piece = _allPiecesList.Find(p => p.cellIndex == cell.index);
                if (piece != null && piece.cellIndex == cell.index)
                    _puzzle.segments.Add(new Segment {cellIndex = piece.cellIndex, type = SegmentTypes.Fixed, content = piece.content, hold = piece.cellIndex});
                else
                    _puzzle.segments.Add(new Segment {cellIndex = cell.index, type = SegmentTypes.Hollow, content = "", hold = -1});
            }

            return true;
        }

        bool TryGenerateSegments()
        {
            bool failed = false;

            var allClauses = new List<Clause>();
            var horClausesList = _horClauses.ToList();
            var verClausesList = _verClauses.ToList();
            
            bool selectHor = false;
            do
            {
                selectHor = !selectHor;

                if (selectHor) //is horizontal
                {
                    if(horClausesList.Count > 0)
                    {
                        allClauses.Add(horClausesList.ElementAt(0));
                        horClausesList.RemoveAt(0);
                    }
                }
                else
                {
                    if(verClausesList.Count > 0)
                    {
                        allClauses.Add(verClausesList.ElementAt(0));
                        verClausesList.RemoveAt(0);
                    }
                }
            } while (horClausesList.Count > 0 || verClausesList.Count > 0);

            var oppsList = new List<string> {"p", "m"};
            if (_genOppers.Contains("t"))
                oppsList.Add("t");
            if (_genOppers.Contains("d"))
                oppsList.Add("d");

            int numberMin = _numMinRange;
            int numberMax = _numMaxRange;

            _allPiecesList.Clear();

            do
            {
                var clause = allClauses.ElementAt(0);

                var piecesList = new List<Piece>();
                foreach (var p in clause.parts)
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

                foreach (var p1 in _allPiecesList)
                {
                    foreach (var p2 in numPieces)
                    {
                        if (p1.cellIndex == p2.cellIndex)
                            p2.content = p1.content;
                    }

                    if (resPiece.cellIndex == p1.cellIndex)
                        resPiece.content = p1.content;
                }


                int loopIter = 0;
                const int max_iterate = 10000;
                var emptyPieces = new List<Piece>();

                //Try numbers
                do
                {
                    emptyPieces.Clear();

                    string opperator = oppPieces.Find(p => p.content != "e").content;
                    foreach (var p in numPieces)
                    {
                        if (p.content == "")
                        {
                            p.content = Random.Range(numberMin, numberMax).ToString();
                            emptyPieces.Add(p);
                        }
                    }

                    if (resPiece.content == "")
                    {
                        if (opperator == "p")
                            resPiece.content = $"{int.Parse(numPieces[0].content) + int.Parse(numPieces[1].content)}";
                        if (opperator == "m")
                            resPiece.content = $"{int.Parse(numPieces[0].content) - int.Parse(numPieces[1].content)}";
                        if (opperator == "t")
                            resPiece.content = $"{int.Parse(numPieces[0].content) * int.Parse(numPieces[1].content)}";
                        if (opperator == "d")
                            resPiece.content = $"{int.Parse(numPieces[0].content) / int.Parse(numPieces[1].content)}";
                        emptyPieces.Add(resPiece);
                    }

                    //Process pieces

                    int resContentNumber = int.Parse(resPiece.content);
                    var numContentsNumber = numPieces.Select(p => int.Parse(p.content)).ToList();
                    if (opperator == "p")
                    {
                        int sum = numContentsNumber[0] + numContentsNumber[1];
                        if (sum != resContentNumber)
                        {
                            emptyPieces.ForEach(p => p.content = "");
                            continue;
                        }
                    }

                    if (opperator == "m")
                    {
                        int diff = numContentsNumber[0] - numContentsNumber[1];
                        if (diff <= 0 || diff != resContentNumber)
                        {
                            emptyPieces.ForEach(p => p.content = "");
                            continue;
                        }
                    }

                    if (opperator == "t")
                    {
                        int cross = numContentsNumber[0] * numContentsNumber[1];
                        if (cross != resContentNumber || cross > 999)
                        {
                            emptyPieces.ForEach(p => p.content = "");
                            continue;
                        }
                    }

                    if (opperator == "d")
                    {
                        int devide = numContentsNumber[0] / numContentsNumber[1];
                        int mod = numContentsNumber[0] % numContentsNumber[1];
                        if (mod != 0 || devide <= 0 || devide != resContentNumber)
                        {
                            emptyPieces.ForEach(p => p.content = "");
                            continue;
                        }
                    }

                    break;
                } while (loopIter++ < max_iterate);

                if (loopIter >= max_iterate)
                {
                    failed = true;
                    _allPiecesList.Clear();
                    break;
                }

                _allPiecesList.AddRange(piecesList);
                allClauses.RemoveAt(0);

            } while (allClauses.Count > 0);

            return !failed;
        }


        void ShuffleSegments()
        {
            int totalShuffle = 0;
            int shuffleIter = 0;
            do
            {
                int shuffledCount = 0;
                int loopIter = 0;
                do
                {
                    if (loopIter++ > 1000)
                        break;
                } while (!TryShuffleSegments(shuffleIter == 0, ref shuffledCount));

                totalShuffle += shuffledCount;
            } while (++shuffleIter < _shuffleCount);

            // if (_puzzlesPack.puzzles.Exists(p => p.id == _puzzle.id))
            //     return;

            _puzzle.shuffle = totalShuffle;
            _puzzlesPack.puzzles.Add(_puzzle);
        }

        bool TryShuffleSegments(bool firstShuffle, ref int shuffledCount)
        {
            var hollowSegs = _puzzle.segments.Where(s => s.type == SegmentTypes.Hollow && s.hold == -1).ToList();
            var fixedSegs = _puzzle.segments.Where(s => s.type == SegmentTypes.Fixed).ToList();

            if (hollowSegs.Count == 0 || fixedSegs.Count == 0)
            {
                Debug.LogWarning("Shuffle skipped because holdsList.Count or heldsList.Count is zero.");
                return false;
            }
            
            var horClausesList = new List<List<int>>();
            var verClausesList = new List<List<int>>();
            _horClauses.ForEach(g => horClausesList.Add(g.parts.Select(p => p.cellIndex).ToList()));
            _verClauses.ForEach(g => verClausesList.Add(g.parts.Select(p => p.cellIndex).ToList()));

            int needShuffles = firstShuffle ? horClausesList.Count + verClausesList.Count : 1;

            var holdsList = new List<int>();
            do
            {
                int index = Random.Range(0, hollowSegs.Count);
                holdsList.Add(hollowSegs[index].cellIndex);
                hollowSegs.RemoveAt(index);
                if(hollowSegs.Count == 0)
                    break;
            } while (holdsList.Count < needShuffles);
            
            var heldsList = new List<int>();
            do
            {
                int index = Random.Range(0, fixedSegs.Count);
                heldsList.Add(fixedSegs[index].cellIndex);
                fixedSegs.RemoveAt(index);
                if(fixedSegs.Count == 0)
                    break;
            } while (heldsList.Count < needShuffles);

            
            if (holdsList.Count != heldsList.Count)
            {
                Debug.LogWarning("Shuffle skipped because holdsList.Count and heldsList.Count must be equal.");
                return false;
            }

            if(firstShuffle)
            {
                bool failed = false;
                foreach (var g in horClausesList)
                {
                    if (!g.Intersect(heldsList).Any())
                    {
                        failed = true;
                        break;
                    }
                }

                foreach (var g in verClausesList)
                {
                    if (!g.Intersect(heldsList).Any())
                    {
                        failed = true;
                        break;
                    }
                }

                if (failed)
                    return false;
            }
            
            shuffledCount = heldsList.Count;

            do
            {
                int hold = holdsList.ElementAt(0);
                int held = heldsList.ElementAt(0);
                holdsList.RemoveAt(0);
                heldsList.RemoveAt(0);

                _puzzle.segments[hold].hold = held;
                _puzzle.segments[hold].content = _puzzle.segments[held].content;
                _puzzle.segments[held].type = SegmentTypes.Modified;
                // _puzzle.segments[held].content = "";
                
            } while (holdsList.Count > 0);
            
            return true;
        }
        
        
        
        Dictionary<string, string> _translateDic = new Dictionary<string, string>();
        void InitTranslateDic()
        {
            _translateDic.Clear();
            string dataPath = Application.dataPath;
            var translateLines = File.ReadAllLines(dataPath + "/Resources/Translates.txt");
            foreach (var line in translateLines)
            {
                if (line.StartsWith("*") || line.StartsWith(" ") || line == "")
                    continue;
                var strsArr = line.Split('=');
                _translateDic[strsArr[0]] = strsArr[1];
            }
        }

        string GetTranslate(string key)
        {
            if (_translateDic.ContainsKey(key))
                return _translateDic[key];
            return key;
        }

        void TrimSavePuzzles()
        {
            try
            {
                _culledPuzzlesPack.puzzles = _culledPuzzlesPack.puzzles.Take(_trimSaveGameLevel).ToList();
                _culledSelectedStage = _trimSaveGameLevel - 1;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        void SavePuzzles()
        {
            try
            {
                if (_culledPuzzlesPack == null || _culledPuzzlesPack.puzzles.Count == 0)
                {
                    Debug.LogError("Can not save. CulledPuzzlesPack is invalid");
                    return;
                }
                
                _culledPuzzlesPack.level = _saveGameLevel;

                string savePath = $"{Application.dataPath}/{SAVE_PATH}/level_{_saveGameLevel:000}.json";
                File.WriteAllText(savePath, JsonUtility.ToJson(_culledPuzzlesPack));
                //Import asset again for updating file
                string filePath = $"Assets/{SAVE_PATH}/{_saveGameLevel:000}.json";
                AssetDatabase.ImportAsset(filePath);
                AssetDatabase.Refresh();
                Debug.Log($"Puzzle saved level_{_saveGameLevel:000}");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        class Cell
        {
            public int index = -1;
            public Rect rect;
            public bool isBusy;
        }

        
        class Clause
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