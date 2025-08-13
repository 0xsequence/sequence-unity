using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sequence.EcosystemWallet;
using UnityEngine;

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
        [SerializeField] private SerializableTransaction _transaction;

        [Header("Components")] 
        [SerializeField] private GameObject _loadingObject;
        [SerializeField] private GameObject _sessionButtonObject;
        [SerializeField] private GameObject _transactionButtonObject;

        private IWallet _wallet;

        public async void Load(IWallet wallet)
        {
            _wallet = wallet;
            await CheckSupportedTransaction();
        }

        public async void AddSession()
        {
            SetState(State.Loading);
            var permission = new ContractPermission(new Address(_transaction.To), 0, 0);;
            await _wallet.AddSession(permission);
            await CheckSupportedTransaction();
        }

        public async void SendTransaction()
        {
            SetState(State.Loading);
            await _wallet.SendTransaction(_chain, _transaction.BuildTransaction());
            SetState(State.Transaction);
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
