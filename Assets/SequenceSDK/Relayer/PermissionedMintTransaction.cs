namespace Sequence.Relayer
{
    public class PermissionedMintTransaction
    {
        public string TokenId;
        public uint Amount;
        
        public PermissionedMintTransaction(string tokenId, uint amount)
        {
            TokenId = tokenId;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"Mint {Amount} of Token Id {TokenId}";
        }
    }
}