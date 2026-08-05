using UnityEngine;
using UnityEngine.EventSystems;

namespace ToolkitByJonathan
{
    [RequireComponent(typeof(RectTransform))]
    public class UIResizeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _resizeMultiplier = 1.25f;
        [SerializeField][Min(0f)] private float _resizeSpeed = 2.0f;

        private bool _isBeingHoveredOver = false;
        private RectTransform _rectTransform;
        private Vector2 _defaultScale;
        private Vector2 _targetScale;

        private void ResetSize()
        {
            _isBeingHoveredOver = false;
            _rectTransform.localScale = _defaultScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isBeingHoveredOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isBeingHoveredOver = false;
        }

        private void OnDisable()
        {
            ResetSize();
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _defaultScale = _rectTransform.localScale;
            _targetScale = _defaultScale;
        }

        private void Update()
        {
            Vector2 currentScale = _rectTransform.localScale;

            if (_isBeingHoveredOver)
            {
                _targetScale = _defaultScale * _resizeMultiplier;
            }
            else
            {
                _targetScale = _defaultScale;
            }

            _rectTransform.localScale = Vector2.Lerp(
                currentScale,
                _targetScale,
                Time.deltaTime * _resizeSpeed
            );
        }
    }

}