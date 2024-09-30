using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
namespace Sequence.Demo
{
    public class GetCheckoutOptionsPage : UIPage
    {
        [SerializeField] Button [] checkoutOptionButton;
        CollectibleOrder _order;
        SequenceWallet _wallet;
        protected override void Awake()
        {
            base.Awake();
            SequenceWallet.OnWalletCreated += wallet => OnWalletCreated(wallet); 
        }

        void OnWalletCreated(SequenceWallet wallet)
        {
            _wallet = wallet;

        }
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
                int index = (int)option;
                if (index >= 0 && index < checkoutOptionButton.Length)
                {
                    if (checkoutOptionButton[index].gameObject.TryGetComponent(out ICheckoutOption checkout))
                    {
                        checkoutOptionButton[index].gameObject.SetActive(true);
                        checkout.SetCollectibleOrder(_order);
                        checkout.SetWallet(_wallet);
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

