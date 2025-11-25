using System;
using Sequence.Relayer;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class FeeOptionWindow : MonoBehaviour
    {
        [SerializeField] private GenericObjectPool<FeeOptionTile> _tilePool;

        private Action<FeeOption> _onSelected;

        public void WaitForSelection(Address walletAddress, FeeOption[] feeOptions, Action<FeeOption> onSelected)
        {
            gameObject.SetActive(true);
            _onSelected = onSelected;
            
            _tilePool.Cleanup();
            foreach (var feeOption in feeOptions)
                _tilePool.GetObject().Load(walletAddress, feeOption, SelectFee);
        }

        public void Close()
        {
            SelectFee(null);
        }

        private void SelectFee(FeeOption feeOption)
        {
            _onSelected?.Invoke(feeOption);
            gameObject.SetActive(false);
        }
    }
}