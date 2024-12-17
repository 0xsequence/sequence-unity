using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class QrCodeView : MonoBehaviour
    {
        const string ApiEndpoint = "https://api.sequence.app/qr/";
        
        [SerializeField] private string _format;
        [SerializeField] private int _size;
        [SerializeField] private Image _qrImage;
        
        public async Task Show(string paymentToken, BigInteger chainId, string destinationAddress, string amount)
        {
            _qrImage.gameObject.SetActive(false);
            _qrImage.sprite = await GenerateQrCodeAsync(paymentToken, chainId, destinationAddress, amount);
            _qrImage.gameObject.SetActive(true);
        }

        public async Task Show(string paymentToken, Chain chain, string destinationAddress, string amount)
        {
            await Show(paymentToken, BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]), destinationAddress, amount);
        }

        public async Task Show(QrCodeParams qrCodeParams)
        {
            await Show(qrCodeParams.PaymentToken, qrCodeParams.Chain, qrCodeParams.DestinationWallet,
                qrCodeParams.Amount);
        }

        private async Task<Sprite> GenerateQrCodeAsync(string paymentToken, BigInteger chainId, string destinationAddress, string amount)
        {
            var deeplink = string.Format(_format, paymentToken, chainId, destinationAddress, amount);
            var encodedLink = Convert.ToBase64String(Encoding.UTF8.GetBytes(deeplink));
            var qrLink = ApiEndpoint + encodedLink + $"/{_size}";
            return await AssetHandler.GetSpriteAsync(qrLink);
        }
    }
}