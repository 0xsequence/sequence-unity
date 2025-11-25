using System;
using System.Numerics;
using Sequence.Relayer;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class FeeOptionTile : MonoBehaviour
    {
        [SerializeField] private Image _logoImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private Color _outOfFundsColor = Color.red;

        private bool _allowSelect;
        private FeeOption _feeOption;
        private Action<FeeOption> _onSelected;
        
        public async void Load(Address walletAddress, FeeOption feeOption, Action<FeeOption> onSelected)
        {
            _feeOption = feeOption;
            _onSelected = onSelected;
            _nameText.text = $"{feeOption.token.name} ({feeOption.token.symbol})";
            _valueText.text = "...";
            _valueText.color = Color.white;
            _logoImage.sprite = await AssetHandler.GetSpriteAsync(feeOption.token.logoURL);
            _allowSelect = false;
            
            var indexer = new ChainIndexer(feeOption.token.chainId);
            if (feeOption.token.contractAddress == null)
            {
                var nativeResponse = await indexer.GetNativeTokenBalance(walletAddress);
                FormatFeeBalanceText(nativeResponse.balanceWei, feeOption);
            }
            else
            {
                var balancesResponse = await indexer.GetTokenBalances(new GetTokenBalancesArgs(walletAddress, feeOption.token.contractAddress));
                var balance = balancesResponse.balances.Length == 0 ? 0 : balancesResponse.balances[0].balance;
                FormatFeeBalanceText(balance, feeOption);
            }
        }

        public void Select()
        {
            if (_allowSelect)
                _onSelected?.Invoke(_feeOption);
        }

        private void FormatFeeBalanceText(BigInteger balance, FeeOption feeOption)
        {
            var currentBalance = DecimalNormalizer
                .ReturnToNormalPrecise(balance, feeOption.token.decimals).ToString();
            
            var requiredBalance = DecimalNormalizer
                .ReturnToNormalPrecise(BigInteger.Parse(feeOption.value), feeOption.token.decimals).ToString();

            var hasRequiredBalance = balance >= BigInteger.Parse(feeOption.value);
            _allowSelect = hasRequiredBalance;
            
            _valueText.color = hasRequiredBalance ? Color.white : _outOfFundsColor;
            _valueText.text = $"{requiredBalance} (You own: {currentBalance})";
        }
    }
}