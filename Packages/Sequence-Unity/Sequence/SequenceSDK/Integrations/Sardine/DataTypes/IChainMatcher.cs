namespace Sequence.Integrations.Sardine
{
    internal interface IChainMatcher
    {
        public bool MatchesChain(Chain chain);
    }
}