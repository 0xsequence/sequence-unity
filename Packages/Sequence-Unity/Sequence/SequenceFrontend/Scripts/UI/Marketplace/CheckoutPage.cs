using System;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class CheckoutPage : UIPage
    {
        private Order[] _listings;
        private Chain _chain;
        private IWallet _wallet;
        private ICheckout _checkout;

        public override void Open(params object[] args)
        {
            base.Open(args);
            _listings = args.GetObjectOfTypeIfExists<Order[]>();
            if (_listings == null || _listings.Length == 0)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a non-empty {typeof(Order[])} as an argument");
            }
            
            _chain = ChainDictionaries.ChainById[_listings[0].chainId.ToString()];
            
            _wallet = args.GetObjectOfTypeIfExists<IWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(IWallet)} as an argument");
            }
            
            _checkout = new Checkout(_wallet, _chain);
            
            Assemble().Start();
        }

        private async Task Assemble()
        {
            Marketplace.CheckoutOptions options = await _checkout.GetCheckoutOptions(_listings);
        }

    }
}