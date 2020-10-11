using System;
using System.Collections;

namespace Equation.Models
{
    public class PuzzleModel
    {
        public string mode;
        public string level;
        public Piece[] pieces;
    }
    
    public class Piece
    {
        public PieceTypes type;
        public int index;
        public string content;
    }

    public enum PieceTypes
    {
        Hollow,
        Fixed,
        Movable
    }
}