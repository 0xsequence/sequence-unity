using System;
using Sequence.Marketplace;
using Sequence.Utils;
using UnityEngine;
namespace Sequence.Demo
{
    public class ListItemPanel : UIPanel
    {
        public ListItemPage ListItemPage { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            ListItemPage = GetComponentInChildren<ListItemPage>();

        }
        
        public override void Open(params object[] args)
        {

            base.Open(args);

            ICheckout _checkout = args.GetObjectOfTypeIfExists<ICheckout>();
            if (_checkout == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ICheckout)} as an argument");
            }

            TokenBalance _nft = args.GetObjectOfTypeIfExists<TokenBalance>();
            if (_nft == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(TokenBalance)} as an argument");
            }
        }
    }
}