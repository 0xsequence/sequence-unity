using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Sequence;
using Sequence.Marketplace;

public class TransferFundsViaQR : MonoBehaviour, ICheckoutOption
{
    [SerializeField] GameObject QrPanel;
    [SerializeField] Image QrImage;
    string apiEndpoint = "https://api.qrserver.com/v1/create-qr-code/";
   


    public async void Checkout(params object [] args)
    {
        CollectibleOrder order = args.OfType<CollectibleOrder>().FirstOrDefault();

        if (order != null)
        {
            await SetQrCode((int)order.order.chainId, "0x00000000000000000000000000000000", "1e2"); 
            QrPanel.SetActive(true);
        }
    }

    public async Task SetQrCode(int chainId, string destinationAddress, string amount)
    {
        QrImage.sprite = Sprite.Create(await GenerateQRCodeAsync( chainId, destinationAddress, "1e2"), new Rect(0, 0, 200, 200), new UnityEngine.Vector2(0.5f, 0.5f));
    }

    public async Task<Texture2D> GenerateQRCodeAsync(int chainId, string destinationAddress, string amount)
    {
        var url = apiEndpoint +"?color=000000&bgcolor=FFFFFF&data=https%3A//metamask.app.link/send/" + NativeTokenAddress.GetNativeTokenAddress(chainId) + "@"+ chainId.ToString() + "/transfer%3Faddress%3D"+ destinationAddress+ "%26uint256%3D"+amount+"8&qzone=1&margin=0&size=250x250&ecc=L";
        return await UnityWebRequestExtensions.DownloadImage(url);
    }
    
    public void Close()
    {
        QrPanel.SetActive(false);
    }

   

}

public static class UnityWebRequestExtensions
{
    public static Task ToTask(this UnityWebRequestAsyncOperation asyncOperation)
    {
        var tcs = new TaskCompletionSource<bool>();
        asyncOperation.completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }

    public async static Task<Texture2D> DownloadImage(string url)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            await request.SendWebRequest().ToTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerTexture.GetContent(request);
            }
            else
            {
                Debug.LogError($"Failed to download QR code: {request.error}");
                return null;
            }
        }
    }
}

