using System.Numerics;

namespace Sequence
{
    [System.Serializable]
    public class EventLog
    {
        public BigInteger id;
        public EventLogType type;
        public BigInteger blockNumber;
        public string blockHash;
        public string contractAddress;
        public ContractType contractType;
        public string txnHash;
        public BigInteger txnIndex;
        public BigInteger txnLogIndex;
        public EventLogDataType logDataType;
        public string ts;
        public string logData;
        public EventDecoded @event;
    }
}