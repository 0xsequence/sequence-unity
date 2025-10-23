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

        private FeeOption _feeOption;
        private Action<FeeOption> _onSelected;
        
        public async void Load(FeeOption feeOption, Action<FeeOption> onSelected)
        {
            _feeOption = feeOption;
            _onSelected = onSelected;
            _nameText.text = $"{feeOption.token.name} ({feeOption.token.symbol})";
            _valueText.text = DecimalNormalizer.ReturnToNormalPrecise(BigInteger.Parse(feeOption.value), feeOption.token.decimals).ToString();
            _logoImage.sprite = await AssetHandler.GetSpriteAsync(feeOption.token.logoURL);
        }

        public void Select()
        {
            _onSelected?.Invoke(_feeOption);
        }
    }
}