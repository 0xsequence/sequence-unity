using System;
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
        
        public async Task Show(string paymentToken, int chainId, string destinationAddress, string amount)
        {
            gameObject.SetActive(false);
            var texture = await GenerateQrCodeAsync(paymentToken, chainId, destinationAddress, amount);
            _qrImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            gameObject.SetActive(true);
        }

        private async Task<Texture2D> GenerateQrCodeAsync(string paymentToken, int chainId, string destinationAddress, string amount)
        {
            var deeplink = string.Format(_format, paymentToken, chainId, destinationAddress, amount);
            var encodedLink = Convert.ToBase64String(Encoding.UTF8.GetBytes(deeplink));
            var qrLink = ApiEndpoint + encodedLink + $"/{_size}";
            return await AssetHandler.GetTexture2DAsync(qrLink);
        }
    }
}