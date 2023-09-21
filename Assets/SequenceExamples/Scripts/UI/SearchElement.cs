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

        public ISearchable Searchable;
        private NetworkIcons _networkIcons;
        
        public void Assemble(ISearchable searchable, NetworkIcons networkIcons)
        {
            Searchable = searchable;
            _icon.sprite = searchable.GetIcon();
            _name.text = searchable.GetName();
            _networkIcons = networkIcons;
            _networkIcon.sprite = GetNetworkIcon();
            _numberOwnedText.text = $"{searchable.GetNumberOwned()} >";
        }

        public Sprite GetNetworkIcon()
        {
            return _networkIcons.GetIcon(Searchable.GetNetwork());
        }
    }
}