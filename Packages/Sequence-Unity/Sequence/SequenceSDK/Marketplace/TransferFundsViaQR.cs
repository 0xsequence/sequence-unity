using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Sequence;
using Sequence.Marketplace;

public class TransferFundsViaQR : MonoBehaviour, ICheckoutOption
{
    [SerializeField] GameObject _qrPanel;
    [SerializeField] Image _qrImage;

    CollectibleOrder _order;

    string apiEndpoint = "https://api.qrserver.com/v1/create-qr-code/";
   

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => Checkout());
    }
    public async void Checkout()
    {
        if (_order != null)
        {
            await SetQrCode((int)_order.order.chainId, "0x00000000000000000000000000000000", "1e2"); 
            _qrPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Collectible order not set for checkout.");
        }
    }

    public void SetCollectibleOrder(CollectibleOrder checkoutOrder)
    {
        _order = checkoutOrder;
    }
    public async Task SetQrCode(int chainId, string destinationAddress, string amount)
    {
        var texture = await GenerateQRCodeAsync(chainId, destinationAddress, "1e2");
        _qrImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new UnityEngine.Vector2(0.5f, 0.5f));
    }

    public async Task<Texture2D> GenerateQRCodeAsync(int chainId, string destinationAddress, string amount)
    {
        var url = apiEndpoint +"?color=000000&bgcolor=FFFFFF&data=https%3A//metamask.app.link/send/" + NativeTokenAddress.GetNativeTokenAddress(chainId) + "@"+ chainId.ToString() + "/transfer%3Faddress%3D"+ destinationAddress+ "%26uint256%3D"+amount+"8&qzone=1&margin=0&size=250x250&ecc=L";
        return await UnityWebRequestExtensions.DownloadImage(url);
    }
    
    public void Close()
    {
        _qrPanel.SetActive(false);
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

