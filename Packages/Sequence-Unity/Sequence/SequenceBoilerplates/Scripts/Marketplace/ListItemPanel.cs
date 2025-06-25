using System;
using Sequence.Marketplace;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Demo
{
    public class ListItemPanel : MonoBehaviour
    {
        public ListItemPage ListItemPage { get; private set; }

        protected void Awake()
        {
            ListItemPage = GetComponentInChildren<ListItemPage>();
        }
        
        public void Open(params object[] args)
        {
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