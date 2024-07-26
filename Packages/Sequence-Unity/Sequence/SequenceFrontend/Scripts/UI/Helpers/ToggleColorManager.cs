using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleColorManager : MonoBehaviour
    {
        private Toggle _toggle;
        private Color _toggleSelectedColor;
        private Color _toggleNotSelectedColor;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggleSelectedColor = _toggle.colors.normalColor;
            _toggleNotSelectedColor =
                new Color(_toggleSelectedColor.r, _toggleSelectedColor.g, _toggleSelectedColor.b, 0);
            _toggle.onValueChanged.AddListener(OnToggleChanged);
            OnToggleChanged(_toggle.isOn);
        }

        private void OnToggleChanged(bool isToggledOn)
        {
            if (isToggledOn)
            {
                _toggle.graphic.color = _toggleSelectedColor;
            }
            else
            {
                _toggle.graphic.color = _toggleNotSelectedColor;
            }
        }
    }
}