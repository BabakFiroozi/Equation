using RTLTMPro;
using UnityEngine;

namespace Equation
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] RTLTextMeshPro3D _contentText;

        public string Content { get; private set; }
        
        public void SetData(string content)
        {
            Content = content;
            _contentText.text = Content;
        }
    }
}