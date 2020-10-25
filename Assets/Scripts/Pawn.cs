using System.Collections;
using DG.Tweening;
using RTLTMPro;
using UnityEngine;

namespace Equation
{
    public class Pawn : MonoBehaviour
    {
        [SerializeField] RTLTextMeshPro3D _textMesh;
        [SerializeField] MeshRenderer _meshRendere;
        [SerializeField] Material[] _typeMateials;
        [SerializeField] Color[] _stateColors;
        
        public Transform Trans { get; private set; }

        public BoardCell Cell { get; private set; }
        public string Content { get; private set; }
        public bool Movable { get; private set; }


        PawnStates _state;

        public PawnStates State => _state;


        public void SetState(PawnStates state)
        {
            _state = state;
            _meshRendere.material.color = _stateColors[(int) state];
        }

        public float SetCell(BoardCell cell, bool init = false, bool help = false)
        {
            if (Cell != null)
                Cell.Pawn = null;

            Cell = cell;
            Cell.Pawn = this;

            float moveTime = 0;

            if (init)
            {
                Trans.position = cell.pos;
            }
            else
            {
                float dist = (cell.pos - Trans.position).magnitude;
                moveTime = Mathf.Clamp(dist / 12, .01f, .5f);
                Trans.DOMove(cell.pos, moveTime).OnComplete(() => { });
                //if help
            }

            if (!init)
            {
                //Play sound
            }

            return moveTime;
        }

        public void SetData(string content, bool movable)
        {
            Content = content;
            _textMesh.text = HelperMethods.CorrectOpperatorContent(content);
            Movable = movable;
            _meshRendere.material = !movable ? _typeMateials[0] : _typeMateials[1];
        }

        void Awake()
        {
            Trans = transform;
        }

        
        bool _mousIsDown;
        Vector3 _mousePos;
        bool _mouseDragged;
        
        void OnMouseDown()
        {
            if(!Movable)
                return;
            
            _mousIsDown = true;
            _mouseDragged = false;
            _mousePos = Input.mousePosition;
        }

        void OnMouseDrag()
        {
            if(!Movable)
                return;
            if (!_mousIsDown)
                return;

            float sens = Screen.width * (12f / 720);
            
            if((_mousePos - Input.mousePosition).magnitude < sens)
                return;
            
            _mouseDragged = true;
            Board.Instance.SetDraggingPiece(this);
        }

        void OnMouseUp()
        {
            if(!Movable)
                return;
            
            if(!_mouseDragged)
                return;

            _mouseDragged = false;
            _mousIsDown = false;
            Board.Instance.SetDraggingPiece(null);
        }

        public void Move(float x, float y, float z)
        {
            Vector3 pos = new Vector3(x, y, z);
            Trans.position = pos;
        }
    }

    public enum PawnStates
    {
        Normal,
        Right,
        Wrong
    }
}