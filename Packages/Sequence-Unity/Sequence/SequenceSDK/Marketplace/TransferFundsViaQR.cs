using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Sequence;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;

public class TransferFundsViaQR : MonoBehaviour, ICheckoutOption
{
    [SerializeField] GameObject _qrPanel;
    [SerializeField] Image _qrImage;

    SequenceWallet _wallet;
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
            await SetQrCode((int)_order.order.chainId, _wallet.GetWalletAddress(), ConvertToScientificNotation(float.Parse(_order.order.priceAmount)));
            _qrPanel.SetActive(true);
        }
        else Debug.LogError("Collectible order not set for checkout option.");
    }

    public void SetWallet(SequenceWallet wallet)
    {
        _wallet = wallet;
    }


    public void SetCollectibleOrder(CollectibleOrder checkoutOrder)
    {
        _order = checkoutOrder;
    }
    public async Task SetQrCode(int chainId, string destinationAddress, string amount)
    {
        var texture = await GenerateQRCodeAsync(chainId, destinationAddress, amount);
        _qrImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new UnityEngine.Vector2(0.5f, 0.5f));
    }

    public async Task<Texture2D> GenerateQRCodeAsync(int chainId, string destinationAddress, string amount)
    {
        string priceCurrencyAddress;
        if (_order.order.priceCurrencyAddress != "0x0000000000000000000000000000000000000000")
            priceCurrencyAddress = _order.order.priceCurrencyAddress;
        else
            priceCurrencyAddress = ChainTokenAddress.Get((int)_order.order.chainId);

        var url = apiEndpoint +"?color=000000&bgcolor=FFFFFF&data=https%3A//metamask.app.link/send/"  + priceCurrencyAddress + "@"+ chainId.ToString() + "/transfer%3Faddress%3D"+ destinationAddress+ "%26uint256%3D"+amount+"&qzone=1&margin=0&size=250x250&ecc=L";
        Debug.Log(url);
        return await UnityWebRequestExtensions.DownloadImage(url);

    }
    
    public void Close()
    {
        _qrPanel.SetActive(false);
    }

    string ConvertToScientificNotation(float value)
    {
        string formattedValue = value.ToString("0.##E+0");
        formattedValue = formattedValue.Replace("E", "e");
        formattedValue = formattedValue.Replace("e+", "e");

        return formattedValue;
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

