using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    public class GameBoard : MonoBehaviour
    {
        public class BoardCell
        {
            public Vector3 pos;
            public Piece piece;
        }
        
        public static GameBoard Instance { get; private set; }
        
        
        
        [SerializeField] Piece _piecePrefab;

        public List<Piece> Pieces { get; } = new List<Piece>();

      
        Piece _draggingPiece;

        public void SetDraggingPiece(Piece piece)
        {
            _draggingPiece = piece;
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
        }

        void Update()
        {
            if (_draggingPiece != null)
            {
                Vector3 mousePos = Input.mousePosition;
                var ray =  Camera.main.ScreenPointToRay(mousePos);
                bool castRes = Physics.Raycast(ray, out var hitInfo, 1000, LayerMaskUtil.GetLayerMask("Ground"));
                if (castRes)
                {
                    Vector3 putPos = hitInfo.point;
                    _draggingPiece.Put(putPos.x, putPos.z);
                }
            }
        }
        
        
        void OnDestroy()
        {
            Instance = null;
        }
    }
}