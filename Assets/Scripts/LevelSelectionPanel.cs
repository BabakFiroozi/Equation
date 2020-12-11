using System;
using UnityEngine;

namespace Equation
{
    public class LevelSelectionPanel : MonoBehaviour
    {
        void Start()
        {
            DataHelper.Instance.LastPlayedInfo.Level = 0;
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_STAGE_MENU);
        }
    }
}