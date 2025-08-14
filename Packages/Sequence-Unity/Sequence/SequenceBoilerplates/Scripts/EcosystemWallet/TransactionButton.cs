using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet;
using Sequence.Relayer;
using UnityEngine;
using UnityEngine.Events;

namespace Sequence.Boilerplates
{
    public class TransactionButton : MonoBehaviour
    {
        [Serializable]
        public class SerializableTransaction
        {
            [field: SerializeField] public string To { get; set; }
            [field: SerializeField] public string FunctionSelector { get; set; }

            public ITransaction BuildTransaction()
            {
                return new Transaction(new Address(To), 0, FunctionSelector);
            } 
        }

        private enum State
        {
            Loading,
            Session,
            Transaction
        }
        
        [Header("Transactions")]
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        [SerializeField] private bool _useFeeOptions;
        [SerializeField] private string[] _feeOptionAddress;
        [SerializeField] private SerializableTransaction _transaction;

        [Header("Components")] 
        [SerializeField] private GameObject _loadingObject;
        [SerializeField] private GameObject _sessionButtonObject;
        [SerializeField] private GameObject _transactionButtonObject;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private TransactionResultPopup _transactionResult;

        private IWallet _wallet;

        public async void Load(IWallet wallet)
        {
            _wallet = wallet;
            await CheckSupportedTransaction();
        }

        public async void AddSession()
        {
            SetState(State.Loading);
            
            var deadline = new BigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000 + 1000 * 60 * 5000);

            IPermissions permissions;
            if (_useFeeOptions)
            {
                var feeOptionsPermissions = new Permissions(_chain,
                    _feeOptionAddress
                        .Select(address => new ContractPermission(new Address(address), deadline, 0)).ToArray<IPermissions>());
                
                permissions = new Permissions(_chain, feeOptionsPermissions,
                    new ContractPermission(new Address(_transaction.To), deadline, 0));
            }
            else
                permissions = new ContractPermission(_chain, new Address(_transaction.To), deadline, 0);
            
            await _wallet.AddSession(permissions);
            await CheckSupportedTransaction();
        }

        public async void SendTransaction()
        {
            SetState(State.Loading);

            var transaction = _transaction.BuildTransaction();

            if (_useFeeOptions)
            {
                var feeOptions = await _wallet.GetFeeOption(_chain, transaction);
                EcosystemFeatureSelection.Instance.OpenFeeOptionWindow(feeOptions, async feeOption =>
                {
                    if (feeOption != null)
                        await TrySendTransaction(transaction, feeOption);
                    
                    SetState(State.Transaction);
                });

                return;
            }
            
            await TrySendTransaction(_transaction.BuildTransaction());
            SetState(State.Transaction);
        }

        private async Task TrySendTransaction(ITransaction transaction, FeeOption feeOption = null)
        {
            try
            {
                var txnHash = await _wallet.SendTransaction(_chain, transaction, feeOption);
                _transactionResult.Show(_chain, txnHash);
            }
            catch (Exception e)
            {
                _messagePopup.Show(e.Message, true);
                Debug.LogError($"{e.Message}");
            }
        }

        private async Task CheckSupportedTransaction()
        {
            SetState(State.Loading);
            var supported = await _wallet.SupportsTransaction(_chain, _transaction.BuildTransaction());
            SetState(supported ? State.Transaction : State.Session);
        }

        private void SetState(State state)
        {
            _loadingObject.SetActive(state == State.Loading);
            _sessionButtonObject.SetActive(state == State.Session);
            _transactionButtonObject.SetActive(state == State.Transaction);
        }
    }
}
