using System;
using UnityEngine;

namespace Equation
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] float[] _tableOffsets;
        
        Transform _tr;

        
        void Start()
        {
            _tr = transform;

            Vector3 pos = _tr.position;
            
            float tableOffset = 0;
            if (Board.Instance.columnsCount == 6)
                tableOffset = _tableOffsets[0];
            if (Board.Instance.columnsCount == 7)
                tableOffset = _tableOffsets[1];
            if (Board.Instance.columnsCount == 8)
                tableOffset = _tableOffsets[2];
            if (Board.Instance.columnsCount == 9)
                tableOffset = _tableOffsets[3];
            if (Board.Instance.columnsCount == 10)
                tableOffset = _tableOffsets[4];
            if (Board.Instance.columnsCount == 11)
                tableOffset = _tableOffsets[5];
            pos -= _tr.forward * tableOffset;

            float refRatio = 720f / 1280;
            float screenRatio = (float)Screen.width / Screen.height;
            var ray = new Ray(pos, _tr.forward);
            bool res = Physics.Raycast(ray, out var hitInfo, 1000, LayerMaskUtil.GetLayerMask("Ground"));
            pos -= _tr.forward * hitInfo.distance * (1 - screenRatio / refRatio);
            _tr.position = pos;
        }

        void OnDestroy()
        {
        }
    }
}