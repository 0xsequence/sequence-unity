using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class FeatureSelectionButton : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _button;

        public void Show(Sprite icon, string title, UnityAction onClick)
        {
            if (icon)
                _iconImage.sprite = icon;
            
            _titleText.text = title;
            _button.onClick.AddListener(onClick);
        }
    }
}