using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class EstimatedTotal : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _estimatedPriceInUSDText;
        [SerializeField] private TextMeshProUGUI _estimatedPriceInTokenText;
        [SerializeField] private Image _tokenIcon;

        public bool Assemble(string estimatedPriceInUSD, string estimatedPriceInToken, string tokenSymbol, Sprite tokenIcon)
        {
            if (string.IsNullOrWhiteSpace(estimatedPriceInUSD))
            {
                Destroy(gameObject);
                return false;
            }
            _estimatedPriceInUSDText.text = $"${estimatedPriceInUSD} estimated total";
            _estimatedPriceInTokenText.text = $"{estimatedPriceInToken} {tokenSymbol}";
            _tokenIcon.sprite = tokenIcon;

            return true;
        }
    }
}