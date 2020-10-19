using System;
using UnityEngine;

namespace Equation
{
    public class CameraControl : MonoBehaviour
    {
        Transform _tr;

        
        void Start()
        {
            _tr = transform;

            Vector3 pos = _tr.position;

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