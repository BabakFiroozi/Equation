using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class TutMgr : MonoBehaviour
    {
        public RectTransform BoardTable;
        public Board Board;

        public RectTransform HintButton;
        public RectTransform HelpButton;

        public GameObject BackButton;

        public GameObject ResultPanelMenuButton;
        public GameObject ResultPanelReplayButton;

        void Start()
        {
            if (!GameConfig.Instance.TutorialIsActive)
                return;
            
            if (TutorialCanvas_Gameplay.Instance != null)
            {
                TutorialCanvas_Gameplay.Instance.GoToCurrentStep();
                return;
            }
            
            if (TutorialCanvas_Gameplay.CurrentStep == TutorialCanvas_Gameplay.Steps.End_1)
            {
            }
        }
    }
}