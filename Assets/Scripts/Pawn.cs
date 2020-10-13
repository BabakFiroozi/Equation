using System;
using UnityEngine;

namespace Equation
{
    public class Pawn : MonoBehaviour
    {
        [SerializeField] TextMesh _textMesh;
        [SerializeField] MeshRenderer _meshRendere;
        [SerializeField] Material[] _materials;
        
        public Transform Trans { get; private set; }

        public GameBoard.BoardCell Cell { get; private set; }
        public string Content => _textMesh.text;
        public bool Movable { get; private set; }

        public void SetCell(GameBoard.BoardCell cell, bool init = false)
        {
            if (Cell != null)
                Cell.Pawn = null;
            
            Cell = cell;
            Cell.Pawn = this;
            Trans.position = cell.pos;

            if (!init)
            {
                //Play sound
            }
        }
        
        public void SetData(string content, bool movable)
        {
            _textMesh.text = HelperMethods.CorrectOpperatorContent(content);
            Movable = movable;

            if (!movable)
                _meshRendere.material = _materials[1];
        }

        void Awake()
        {
            Trans = transform;
        }

        void OnMouseDown()
        {
            if(!Movable)
                return;
            GameBoard.Instance.SetDraggingPiece(this);
        }

        void OnMouseUp()
        {
            if(!Movable)
                return;
            GameBoard.Instance.SetDraggingPiece(null);
        }

        public void Put(float x, float z)
        {
            Vector3 pos = Trans.position;
            pos.x = x;
            pos.z = z;
            Trans.position = pos;
        }
    }
}