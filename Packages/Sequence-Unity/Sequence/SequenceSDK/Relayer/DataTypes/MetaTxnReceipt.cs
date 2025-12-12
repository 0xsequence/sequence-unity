using System;
using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [Preserve]
    public class MetaTxnReceipt
    {
        public string id;
        public string status;
        public string revertReason;
        public int index;
        public string txnReceipt;
        public string txnHash;
        public string blockNumber;
        public MetaTxnReceipt[] receipts;
        public MetaTxnReceiptLog[] logs;
        
        public OperationStatus EvaluateStatus()
        {
            return status switch
            {
                "QUEUED" => OperationStatus.Pending,
                "SENT" => OperationStatus.Pending,
                "PENDING_PRECONDITION" => OperationStatus.Pending,
                "SUCCEEDED" => OperationStatus.Confirmed,
                "FAILED" => OperationStatus.Failed,
                "PARTIALLY_FAILED" => OperationStatus.Failed,
                "DROPPED" => OperationStatus.Failed,
                "UNKNOWN" => OperationStatus.Unknown,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}