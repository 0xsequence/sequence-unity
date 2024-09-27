using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class QrCodeView : MonoBehaviour
    {
        const string ApiEndpoint = "https://api.qrserver.com/v1/create-qr-code/";
        
        [SerializeField] private Image _qrImage;
        
        public async Task Show(int chainId, string destinationAddress, string amount)
        {
            gameObject.SetActive(false);
            var texture = await GenerateQrCodeAsync(chainId, destinationAddress, amount);
            _qrImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            gameObject.SetActive(true);
        }

        private async Task<Texture2D> GenerateQrCodeAsync(int chainId, string destinationAddress, string amount)
        {
            var url = ApiEndpoint +"?color=000000&bgcolor=FFFFFF&data=https%3A//metamask.app.link/send/" + 
                      NativeTokenAddress.GetNativeTokenAddress(chainId) + "@"+ chainId.ToString() + 
                      "/transfer%3Faddress%3D"+ destinationAddress+ "%26uint256%3D" + amount + 
                      "8&qzone=1&margin=0&size=250x250&ecc=L";
            
            return await AssetHandler.GetTexture2DAsync(url);
        }
    }
}