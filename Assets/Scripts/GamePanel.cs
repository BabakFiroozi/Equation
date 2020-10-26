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
            Destroy(_boardObj);
            _boardObj = Instantiate(_bordPrefab, transform);
            var board = _boardObj.GetComponent<Board>();
            board.Initialize();
        }
    }
}