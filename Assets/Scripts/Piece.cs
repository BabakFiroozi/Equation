using System;
using UnityEngine;

namespace Equation
{
    public class Piece : MonoBehaviour
    {
        public Transform Trans { get; private set; }

        void Start()
        {
            Trans = transform;
        }

        void OnMouseDown()
        {
            GameBoard.Instance.SetDraggingPiece(this);
        }

        void OnMouseUp()
        {
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