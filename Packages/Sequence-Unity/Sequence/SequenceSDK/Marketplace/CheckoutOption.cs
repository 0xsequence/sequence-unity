using UnityEngine;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
public interface ICheckoutOption 
{
    public void Checkout();

    public void SetWallet(SequenceWallet wallet);
    public void SetCollectibleOrder(CollectibleOrder checkoutOrder);
}
