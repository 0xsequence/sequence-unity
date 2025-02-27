using System;
using System.Text;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class QrCodeView : MonoBehaviour
    {
        const string ApiEndpoint = "https://api.sequence.app/qr/";
        
        [SerializeField] private string _format;
        [SerializeField] private int _size;
        [SerializeField] private RawImage _qrImage;
        
        public async Task Show(string paymentToken, string destinationAddress, string amount)
        {
            var deeplink = string.Format(_format, paymentToken, destinationAddress, amount);
            await Show(deeplink);
        }

        public async Task Show(string deeplink)
        {
            _qrImage.texture = null;
            _qrImage.texture = await GenerateQrCodeAsync(deeplink);
        }

        private async Task<Texture2D> GenerateQrCodeAsync(string deeplink)
        {
            var encodedLink = Convert.ToBase64String(Encoding.UTF8.GetBytes(deeplink));
            var qrLink = ApiEndpoint + encodedLink + $"/{_size}";
            return await AssetHandler.GetTexture2DAsync(qrLink);
        }
    }
}