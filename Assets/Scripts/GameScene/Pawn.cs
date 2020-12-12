using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Equation
{
    public class Pawn : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTr;
        [SerializeField] Text _valueText;
        [SerializeField] Image _frameImage;
        [SerializeField] Image _fixedBadge;
        [SerializeField] Color[] _stateColors;

        public RectTransform RectTr => _rectTr;

        
        public BoardCell Cell { get; private set; }
        public string Content { get; private set; }
        public bool Movable { get; private set; }

        public int Id { get; set; }
        

        PawnStates _state;

        public PawnStates State => _state;

        float _initWidth;
        float _initFontSize;
        

        public void SetState(PawnStates state)
        {
            _state = state;
            _frameImage.color = _stateColors[(int) state];
            _fixedBadge.color = _stateColors[(int) state];
        }

        public float SetCell(BoardCell cell, bool anim = true, bool help = false)
        {
            if (Cell != null)
                Cell.Pawn = null;

            Cell = cell;
            Cell.Pawn = this;

            float moveTime = 0;

            if (anim)
            {
                GameSaveData.SavePawnCell(Board.Instance.CurrentPlayedInfo, Id, Cell.index);
                float dist = (cell.pos - RectTr.anchoredPosition).magnitude;
                moveTime = Mathf.Clamp(dist / 720, .01f, .5f);
                RectTr.DOAnchorPos(cell.pos, moveTime).OnComplete(() => { });
                //if help
            }
            else
            {
                RectTr.anchoredPosition = cell.pos;
                _valueText.fontSize = (int) (RectTr.rect.width / _initWidth * _initFontSize);
            }

            if (anim)
            {
                //Play sound
            }

            return moveTime;
        }

        public void SetData(int id, string content, bool movable)
        {
            Id = id;
            Content = content;
            _valueText.text = HelperMethods.CorrectOpperatorContent(content);
            Movable = movable;
            _fixedBadge.enabled = !movable;
        }

        void Awake()
        {
            _initWidth = RectTr.rect.width;
            _initFontSize = _valueText.fontSize;
        }
    }

    public enum PawnStates
    {
        Normal,
        Right,
        Wrong
    }
}