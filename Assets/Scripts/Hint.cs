using UnityEngine;

namespace Equation
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] TextMesh _contentText;

        public string Content { get; private set; }
        
        public void SetData(string content)
        {
            Content = content;
            _contentText.text = Content;
        }
    }
}