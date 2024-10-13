using UnityEngine;
using Sequence.Marketplace;
public interface ICheckoutOption 
{
    public void Checkout();

    public void SetCollectibleOrder(CollectibleOrder checkoutOrder);
}
