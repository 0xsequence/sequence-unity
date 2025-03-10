using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sequence.Boilerplates.Inventory
{
    public class SequenceInventoryTile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private RawImage _tokenImage;

        private UnityAction _onClick;
        
        public async void Load(TokenBalance token, UnityAction onClick)
        {
            _onClick = onClick;
            _nameText.text = $"{token.tokenMetadata.name}";
            _amountText.text = $"{token.balance}";
            _tokenImage.texture = await AssetHandler.GetTexture2DAsync(token.tokenMetadata.image);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }
    }
}
