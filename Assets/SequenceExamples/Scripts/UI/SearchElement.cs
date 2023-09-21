using System;
using Sequence.Demo.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class SearchElement : WalletUIElement
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _numberOwnedText;

        public ISearchable Searchable;
        private WalletPanel _walletPanel;
        
        public void Assemble(ISearchable searchable)
        {
            Searchable = searchable;
            _icon.sprite = searchable.GetIcon();
            _name.text = searchable.GetName();
            _networkIcon.sprite = GetNetworkIcon();
            _numberOwnedText.text = $"{searchable.GetNumberOwned()} >";
        }

        public Sprite GetNetworkIcon()
        {
            return NetworkIcons.GetIcon(GetNetwork());
        }

        public void OpenInfoPage()
        {
            if (_walletPanel == null)
            {
                _walletPanel = FindObjectOfType<WalletPanel>();
            }

            if (Searchable is SearchableCollection searchableCollection)
            {
                _walletPanel.OpenCollectionInfoPage(NetworkIcons, searchableCollection.GetCollection());
            }else if (Searchable is TokenElement token)
            {
                _walletPanel.OpenTokenInfoPage(token, NetworkIcons, TransactionDetailsFetcher);
            }
            else
            {
                throw new SystemException(
                    $"Encountered unexpected type of {nameof(ISearchable)}, given {Searchable.GetType()}");
            }
        }

        public override Chain GetNetwork()
        {
            return Searchable.GetNetwork();
        }
    }
}