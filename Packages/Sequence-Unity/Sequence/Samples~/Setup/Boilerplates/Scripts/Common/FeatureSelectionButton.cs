using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class FeatureSelectionButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _button;

        public void Show(string title, string description, UnityAction onClick)
        {
            _titleText.text = title;
            _descriptionText.text = description;
            _button.onClick.AddListener(onClick);
        }
    }
}