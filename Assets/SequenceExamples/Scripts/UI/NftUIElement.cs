using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class NftUIElement : MonoBehaviour
    {
        [SerializeField] private Image _nftImage;
        
        private NftElement _nftElement;
        public ITransactionDetailsFetcher TransactionDetailsFetcher = new MockTransactionDetailsFetcher(15); // Todo: replace mock with concrete implementation

        public void Assemble(NftElement nftElement)
        {
            _nftElement = nftElement;

            _nftImage.sprite = nftElement.TokenIconSprite;
        }
    }
}