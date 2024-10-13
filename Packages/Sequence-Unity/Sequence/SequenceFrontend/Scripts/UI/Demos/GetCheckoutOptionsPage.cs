using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sequence.Marketplace;
namespace Sequence.Demo
{
    public class GetCheckoutOptionsPage : UIPage
    {
        [SerializeField] Button [] checkoutOptionButton;
        CollectibleOrder _order;

        public override void Open(params object[] args)
        {
            base.Open(args);

            CollectibleOrder order = args.OfType<CollectibleOrder>().FirstOrDefault();

            if (order != null) _order = order;
            
            GetCheckoutOptions(0);
        }

        public void GetCheckoutOptions(int randomOptions)
        {
            var options = new GetCheckoutOptionsMock().GetCheckoutOptions(randomOptions);

            foreach (var option in options)
            {
                // Ensure option is a valid index and within the bounds of the checkoutOptionButton array/list
                int index = (int)option;
                if (index >= 0 && index < checkoutOptionButton.Length)
                {
                    // Try to get the ICheckoutOption component
                    if (checkoutOptionButton[index].gameObject.TryGetComponent(out ICheckoutOption checkout))
                    {
                        checkoutOptionButton[index].gameObject.SetActive(true);
                        checkout.SetCollectibleOrder(_order);
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid option index: {index}");
                }
            }
        }

    }

}

