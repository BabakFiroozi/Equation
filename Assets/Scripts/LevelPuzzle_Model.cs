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
        public Segment[] segments;
    }
    
    public class Segment
    {
        public int cellIndex;
        public SegmentTypes type;
        public string content;
        public int hold;
    }

    public enum SegmentTypes
    {
        Hollow,
        Fixed,
        Movable
    }
}