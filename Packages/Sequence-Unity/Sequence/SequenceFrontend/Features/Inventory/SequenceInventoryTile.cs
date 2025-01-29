using Sequence;
using Sequence.Demo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class SequenceInventoryTile : MonoBehaviour
    {
        [SerializeField] private RawImage _tokenImage;
        
        public async void Load(TokenMetadata token, UnityAction callback)
        {
            _tokenImage.texture = await AssetHandler.GetTexture2DAsync(token.image);
        }
    }
}
