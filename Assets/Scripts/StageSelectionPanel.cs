using System;
using UnityEngine;

namespace Equation
{
    public class StageSelectionPanel : MonoBehaviour
    {
        void Start()
        {
            DataHelper.Instance.LastPlayedInfo.Stage = 0;
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME);
        }
    }
}