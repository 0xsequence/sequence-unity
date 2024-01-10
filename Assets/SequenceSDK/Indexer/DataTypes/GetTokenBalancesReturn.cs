namespace Sequence
{
    [System.Serializable]
    public class GetTokenBalancesReturn
    {
        public Page page;
        public TokenBalance[] balances;
        
        /// <summary>
        /// Return the index of the token with the given id in 'balances', or -1 if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int FindTokenId(int key)
        {
            for (int i = 0; i < balances.Length; i++)
            {
                if (balances[i].id == key)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}