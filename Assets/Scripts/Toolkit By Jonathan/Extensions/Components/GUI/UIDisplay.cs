using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace ToolkitByJonathan
{
    [RequireComponent(typeof(RectTransform))]
    public class UIDisplay<T> : MonoBehaviour
    {
        [SerializeField]
        [HideIf("_tryGetLabel")]
        protected TextMeshProUGUI _label;
        [SerializeField] private bool _tryGetLabel = false;

        private void Awake()
        {
            if (!_tryGetLabel)
                return;

            _label = GetComponent<TextMeshProUGUI>();
        }

        public void RefreshLabel(T value)
        {
            if (_label == null)
                return;

            _label.text = FormatValue(value);
        }

        public void ClearLabel()
        {
            if (_label == null)
                return;

            _label.text = "";
        }

        public void SetLabelText(string text)
        {
            if (_label == null)
                return;

            _label.text = text;
        }

        protected virtual string FormatValue(T value) => value.ToString();
    }
}
