using System;
using System.Numerics;
using Sequence.Relayer;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class FeeOptionTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _valueText;

        private FeeOption _feeOption;
        private Action<FeeOption> _onSelected;
        
        public void Load(FeeOption feeOption, Action<FeeOption> onSelected)
        {
            _feeOption = feeOption;
            _onSelected = onSelected;
            _nameText.text = feeOption.token.name;
            _valueText.text = DecimalNormalizer.ReturnToNormalPrecise(BigInteger.Parse(feeOption.value), feeOption.token.decimals).ToString();
        }

        public void Select()
        {
            _onSelected?.Invoke(_feeOption);
        }
    }
}