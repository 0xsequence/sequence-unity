using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class EstimatedTotal : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _estimatedPriceInUSDText;
        [SerializeField] private TextMeshProUGUI _estimatedPriceInTokenText;
        [SerializeField] private Image _tokenIcon;

        public void Assemble(string estimatedPriceInUSD, string estimatedPriceInToken, string tokenSymbol, Sprite tokenIcon)
        {
            _estimatedPriceInUSDText.text = $"${estimatedPriceInUSD} estimated total";
            _estimatedPriceInTokenText.text = $"{estimatedPriceInToken} {tokenSymbol}";
            _tokenIcon.sprite = tokenIcon;
        }
    }
}