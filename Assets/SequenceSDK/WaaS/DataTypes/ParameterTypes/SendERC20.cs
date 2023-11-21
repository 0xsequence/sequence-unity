namespace SequenceSDK.WaaS
{
    [System.Serializable]
    public class SendERC20 : SequenceSDK.WaaS.Transaction
    {
        public string type { get; private set; } = "erc20send";
        public string token { get; private set; }
        public string to { get; private set; }
        public string value { get; private set; }
        
        public SendERC20(string tokenAddress, string to, string value)
        {
            this.token = tokenAddress;
            this.to = to;
            this.value = value;
        }
    }
}