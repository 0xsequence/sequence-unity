using Sequence.Contracts;
using UnityEngine;

namespace Sequence.Demo
{
    public class TokenElement
    {
        public ERC20 Erc20;
        public Sprite TokenIconSprite;
        public string TokenName;
        public Chain Network;
        public uint Balance;
        public string Symbol;
        public ICurrencyConverter CurrencyConverter;
        public float PreviousCurrencyValue;
        
        public TokenElement(ERC20 erc20, Sprite tokenIconSprite, string tokenName, Chain network, uint balance, string symbol, ICurrencyConverter currencyConverter)
        {
            Erc20 = erc20;
            TokenIconSprite = tokenIconSprite;
            TokenName = tokenName;
            Network = network;
            Balance = balance;
            Symbol = symbol;
            CurrencyConverter = currencyConverter;
            PreviousCurrencyValue = currencyConverter.ConvertToCurrency(balance, erc20).Amount;
        }

        public TokenElement(string tokenAddress, Sprite tokenIconSprite, string tokenName, Chain network,
            uint balance, string symbol, ICurrencyConverter currencyConverter)
        {
            Erc20 = new ERC20(tokenAddress);
            TokenIconSprite = tokenIconSprite;
            TokenName = tokenName;
            Network = network;
            Balance = balance;
            Symbol = symbol;
            CurrencyConverter = currencyConverter;
            PreviousCurrencyValue = currencyConverter.ConvertToCurrency(balance, Erc20).Amount;
        }
    }
}