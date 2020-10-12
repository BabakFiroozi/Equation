using System;
using System.Collections;

namespace Equation.Models
{
    public class LevelPuzzle_Model
    {
        public PuzzleData[] puzzles;
    }
    public class PuzzleData
    {
        public string mode;
        public string level;
        public PuzzlePiece[] pieces;
    }
    
    public class PuzzlePiece
    {
        public int cellIndex;
        public PieceTypes type;
        public string content;
        public int hold;
    }

    public enum PieceTypes
    {
        Hollow,
        Fixed,
        Movable
    }
}