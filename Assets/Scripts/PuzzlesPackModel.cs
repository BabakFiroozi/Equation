﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Equation.Models
{
    [Serializable]
    public class PuzzlesPackModel
    {
        public GameLevels level;
        public List<Puzzle> puzzles;
    }
    
    [Serializable]
    public class Puzzle
    {
        public string id;
        public int rows;
        public int columns;
        public List<Segment> segments;
        public int distort;
    }
    
    [Serializable]
    public class Segment
    {
        public int cellIndex;
        public SegmentTypes type;
        public string content;
        public int hold;
    }

    [Serializable]
    public enum SegmentTypes
    {
        Hollow,
        Fixed,
        Movable
    }
}