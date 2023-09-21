using System;
using Sequence.Demo.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class SearchElement : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _numberOwnedText;
        
        public void Assemble(ISearchable searchable, NetworkIcons networkIcons)
        {
            _icon.sprite = searchable.GetIcon();
            _name.text = searchable.GetName();
            _networkIcon.sprite = networkIcons.GetIcon(searchable.GetNetwork());
            _numberOwnedText.text = $"{searchable.GetNumberOwned()} >";
        }
    }
}