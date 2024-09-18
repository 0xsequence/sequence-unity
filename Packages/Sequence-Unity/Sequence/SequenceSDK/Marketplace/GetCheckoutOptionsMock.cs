using System;
public class GetCheckoutOptionsMock
{

    public CheckoutOptions[] GetCheckoutOptions(int number)
    {
        switch (number)
        {
            case 0:
                return new CheckoutOptions[] { CheckoutOptions.TransferFundsViaQR };
            case 1:
                return new CheckoutOptions[] { CheckoutOptions.CryptoPurchase };
            case 2:
                return new CheckoutOptions[] { CheckoutOptions.CryptoPurchase, CheckoutOptions.SwapAndPay };
            case 3:
                return new CheckoutOptions[] { CheckoutOptions.CryptoPurchase, CheckoutOptions.SwapAndPay, CheckoutOptions.SardineCheckout };
            case 4:
                return new CheckoutOptions[] { CheckoutOptions.CryptoPurchase, CheckoutOptions.SwapAndPay, CheckoutOptions.SardineCheckout, CheckoutOptions.TransferFundsViaQR };
            case 5:
                return new CheckoutOptions[] { CheckoutOptions.CryptoPurchase, CheckoutOptions.SwapAndPay, CheckoutOptions.SardineCheckout, CheckoutOptions.TransferFundsViaQR, CheckoutOptions.TransakOnramp };
            default:
                throw new ArgumentOutOfRangeException(nameof(number), "Invalid number of checkout options.");
        }
    }
}

