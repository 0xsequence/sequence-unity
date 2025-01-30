using Sequence;
using Sequence.Demo;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class SequenceInventoryTile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private RawImage _tokenImage;

        private UnityAction _onClick;
        
        public async void Load(TokenBalance token, UnityAction onClick)
        {
            _onClick = onClick;
            _nameText.text = $"{token.balance}x {token.tokenMetadata.name}";
            _tokenImage.texture = await AssetHandler.GetTexture2DAsync(token.tokenMetadata.image);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }
    }
}
