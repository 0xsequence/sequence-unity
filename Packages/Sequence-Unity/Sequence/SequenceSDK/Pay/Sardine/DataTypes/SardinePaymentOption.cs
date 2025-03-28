using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    public class SardinePaymentOption
    {
        public string name;
        public double dailyLimit;
        public double weeklyLimit;
        public double monthlyLimit;
        public double maxAmount;
        public double minAmount;
        public string[] subTypes;
        public string type;
        public string subType;
        public string processingTime;

        [Preserve]
        public SardinePaymentOption(string name, double dailyLimit, double weeklyLimit, double monthlyLimit, double maxAmount, double minAmount, string[] subTypes, string type, string subType, string processingTime)
        {
            this.name = name;
            this.dailyLimit = dailyLimit;
            this.weeklyLimit = weeklyLimit;
            this.monthlyLimit = monthlyLimit;
            this.maxAmount = maxAmount;
            this.minAmount = minAmount;
            this.subTypes = subTypes;
            this.type = type;
            this.subType = subType;
            this.processingTime = processingTime;
        }
    }
}