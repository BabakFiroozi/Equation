using System;
using UnityEngine;

namespace Equation
{
    public class GamePanel : MonoBehaviour
    {
        [SerializeField] GameObject _bordPrefab;
        [SerializeField] GameObject _boardObj;
        
        void Awake()
        {
            Destroy(_boardObj);
            transform.position = Vector3.zero;
        }

        public void RefreshBoard()
        {
            var obj = Instantiate(_bordPrefab, transform);
            var board = obj.GetComponent<Board>();
            board.Initialize();
        }
    }
}