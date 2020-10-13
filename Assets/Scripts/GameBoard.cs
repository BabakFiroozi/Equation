using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] Piece _piecePrefab;

        public List<Piece> Pieces { get; } = new List<Piece>();
        
        public class BoardCell
        {
            public Vector3 pos;
            public Piece piece;
        }

        void Start()
        {
            
        }
    }
}