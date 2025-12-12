using System;

namespace Sequence.EcosystemWallet.IntegrationTests.Server
{
    [Serializable]
    internal abstract class JsonRpcResponse
    {
        public abstract bool IsError();
    }
}