using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] Font[] _fonts;
        
        [SerializeField] AudioSource _helpSound;

        [SerializeField] Color _helpColor;
        [SerializeField] GameObject _helpEffectObj;
        
        public RectTransform RectTr => _rectTr;

        
        public BoardCell Cell { get; private set; }
        public string Content { get; private set; }
        public bool Movable { get; private set; }

        public int Id { get; set; }
        

        PawnStates _state;

        public PawnStates State => _state;

        float _initWidth;
        float _initFontSize;

        int _fontSize;
        
        
        public void SetState(PawnStates state)
        {
            _state = state;
            _frameImage.DOKill();
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
                GameSaveData.SavePawnCell(DataHelper.Instance.LastPlayedInfo, Id, Cell.index);
                float dist = (cell.pos - RectTr.anchoredPosition).magnitude;
                moveTime = Mathf.Clamp(dist / 720, .01f, .5f);

                GameObject effect = null;
                
                if (help)
                {
                    _helpSound.Play();
                    effect = Instantiate(_helpEffectObj, _rectTr);
                    StartCoroutine(_SetEffectSize(effect.GetComponent<ParticleSystem>()));

                    _frameImage.DOColor(_helpColor, moveTime);
                }
                
                RectTr.DOAnchorPos(cell.pos, moveTime).onComplete = () =>
                {
                    if (effect != null)
                        Destroy(effect, 2);
                };
            }
            else
            {
                RectTr.anchoredPosition = cell.pos;
                _valueText.fontSize = (int) (RectTr.rect.width / _initWidth * _initFontSize);
                _fontSize = _valueText.fontSize;
            }

            if (anim)
            {
                //Play sound
            }

            return moveTime;
        }
        
        IEnumerator<WaitForEndOfFrame> _SetEffectSize(ParticleSystem effect)
        {
            yield return new WaitForEndOfFrame();
            float sizeCeof = _rectTr.rect.width / 140;
            var main = effect.main;
            main.startSizeMultiplier *= sizeCeof;
            var shape = effect.shape;
            shape.radius *= sizeCeof;
        }

        public void SetData(int id, string content, bool movable)
        {
            Id = id;
            Content = content;
            _valueText.text = HelperMethods.CorrectOpperatorContent(content);
            Movable = movable;
            _fixedBadge.enabled = !movable;
        }

        public void SetFontEng(bool eng)
        {
            _valueText.font = !eng ? _fonts[0] : _fonts[1];
            _valueText.fontSize = eng ? _fontSize - 4 : _fontSize;
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